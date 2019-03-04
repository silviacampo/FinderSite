//dotnet aspnet-codegenerator controller -name PetController -async -m webGDPR.Models.Pet -dc webGDPR.Data.ApplicationDbContext -namespace webGDPR.Controllers -outDir Controllers
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webGDPR.Data;
using webGDPR.Infrastructure;
using webGDPR.Infrastructure.CustomWebSockets;
using webGDPR.Models;
using webGDPR.ViewModels;

namespace webGDPR.Controllers
{
    [Authorize]
    public class PetController : Controller
    {
        private readonly ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        IMapper _mapper;
        ICustomWebSocketMessageHandler _webSocketMessageHandler;
        ICustomWebSocketFactory _wsFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PetController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IHostingEnvironment hostingEnvironment, ICustomWebSocketMessageHandler webSocketMessageHandler, ICustomWebSocketFactory wsFactory, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            _webSocketMessageHandler = webSocketMessageHandler;
            _wsFactory = wsFactory;
            _signInManager = signInManager;
        }

        // GET: Pet
        public async Task<IActionResult> Index()
        {
            List<PetViewModel> model = new List<PetViewModel>();
            string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;

            List<Pet> pets = await _context.Pet.AsNoTracking().Where(b => b.UserId == UserId && !b.Deleted).Include(c => c.LastTrackingInfo).Include(m => m.LastMode).Include(b => b.LastCollar).ThenInclude(c => c.Collar).ToListAsync();
            foreach (var c in pets)
            {
                PetViewModel cvm = _mapper.Map<PetViewModel>(new Tuple<Pet, PetTrackingInfo>(c, c.LastTrackingInfo));
                if (c.LastCollar != null && c.LastCollar.Collar != null)
                {
                    cvm.CollarName = c.LastCollar.Collar.Name;
                }
                cvm.EmergencyOn = false;
                if (c.LastMode != null && c.LastMode.Type == ConfigModeTypes.Emergency && c.LastMode.IsActive)
                {
                    cvm.EmergencyOn = true;
                }
                model.Add(cvm);
            }
            return View(model);
        }

        // GET: Pet/Details/5
        public async Task<IActionResult> Details(string id, string searchString, string currentFilter, int? pageIndex, string searchString2, string currentFilter2, int? pageIndex2)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pet.AsNoTracking().Include(c => c.LastTrackingInfo).Include(m => m.LastMode).Include(b => b.LastCollar).ThenInclude(c => c.Collar)
                .FirstOrDefaultAsync(m => m.PetId == id && !m.Deleted);
            if (pet == null)
            {
                return NotFound();
            }

            PetViewModel model = _mapper.Map<PetViewModel>(new Tuple<Pet, PetTrackingInfo>(pet, pet.LastTrackingInfo));
            if (pet.LastCollar != null && pet.LastCollar.Collar != null)
            {
                model.CollarName = pet.LastCollar.Collar.Name;
            }

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                if (currentFilter == null)
                {
                    searchString = string.Empty;
                }
                else
                {
                    searchString = currentFilter;
                }
            }
            model.CurrentFilter = searchString;
            int pageSize = 10;
            model.PetTrackingInfos = await PaginatedList<PetTrackingInfo>.CreateAsync(
                _context.PetTrackingInfo.Where(s => s.PetId == id && (s.Latitude.ToString().Contains(searchString) || s.Longitude.ToString().Contains(searchString))).Include(d => d.Collar).OrderByDescending(d => d.CreationDate).AsNoTracking(), pageIndex ?? 1, pageSize);

            if (searchString2 != null)
            {
                pageIndex2 = 1;
            }
            else
            {
                if (currentFilter2 == null)
                {
                    searchString2 = string.Empty;
                }
                else
                {
                    searchString2 = currentFilter2;
                }
            }
            model.CurrentFilterMode = searchString2;

            model.PetModes = await PaginatedList<PetMode>.CreateAsync(
                _context.PetMode.Where(s => s.PetId == id && (s.Type.ToString().Contains(searchString2) || s.CreationDate.ToString().Contains(searchString2))).OrderByDescending(d => d.CreationDate).AsNoTracking(), pageIndex2 ?? 1, pageSize);
            return View(model);
        }

        // GET: Pet/Track/5
        public async Task<IActionResult> Track(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pet = await _context.Pet.Include(b => b.LastTrackingInfo)
                .FirstOrDefaultAsync(m => m.PetId == id && !m.Deleted);
            if (pet == null)
            {
                return NotFound();
            }

            pet.TrackingInfos = await _context.PetTrackingInfo.Where(t => t.PetId == id).Take(10).ToListAsync();

            return View(pet);
        }

        // GET: Pet/Map?username=SilviaCampo&collarnumber=1
        //http://localhost:51420/Identity/Account/SilentLogin?Username=SilviaCampo&ReturnUrl=%2FPet%2FMap%3Fusername%3DSilviaCampo%26collarnumber%3D1
        public async Task<IActionResult> Map(string username, int collarnumber)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Name == username);
            if (user != null)
            {
                var collar = await _context.Collar.FirstOrDefaultAsync(m => m.UserId == user.UserID && m.CollarNumber == collarnumber);
                if (collar != null)
                {
                    var petCollar = await _context.PetCollar.FirstOrDefaultAsync(m => m.IsActive && m.CollarId == collar.CollarId);
                    if (petCollar != null)
                    {
                        var pet = await _context.Pet.Include(b => b.LastTrackingInfo).FirstOrDefaultAsync(m => m.PetId == petCollar.PetId);
                        if (pet == null)
                        {
                            return NotFound();
                        }
                        if (pet.LastTrackingInfo == null)
                        {
                            return NotFound(); //create a custom page to show that this pet is not tracking any information
                        }
                        pet.TrackingInfos = await _context.PetTrackingInfo.Where(t => t.PetId == petCollar.PetId).Take(10).ToListAsync();

                        return View(pet);
                    }
                    return NotFound();
                }
                return NotFound();
            }
            return NotFound();
        }

		// GET: Pet/Map2?u=SilviaCampo&g=34a33904-49d5-4edc-be95-00af3e64eecd&cn=2
		//http://localhost:51420/Identity/Account/SilentLogin?Username=SilviaCampo&ReturnUrl=%2FPet%2FMap%3Fusername%3DSilviaCampo%26collarnumber%3D1
		[AllowAnonymous]
		public async Task<IActionResult> Map2(string u, string g, int cn)
		{
			var device = await _context.Device.FirstOrDefaultAsync(d => d.DeviceId == g && !d.Banned);
			if (device != null) {
				var user = await _context.User.FirstOrDefaultAsync(m => m.Name == u);
				if (user != null)
				{
					var collar = await _context.Collar.FirstOrDefaultAsync(m => m.UserId == user.UserID && m.CollarNumber == cn);
					if (collar != null)
					{
						var petCollar = await _context.PetCollar.FirstOrDefaultAsync(m => m.IsActive && m.CollarId == collar.CollarId);
						if (petCollar != null)
						{
							var pet = await _context.Pet.Include(b => b.LastTrackingInfo).FirstOrDefaultAsync(m => m.PetId == petCollar.PetId);
							if (pet == null)
							{
								return NotFound();
							}
							if (pet.LastTrackingInfo == null)
							{
								return NotFound(); //create a custom page to show that this pet is not tracking any information
							}
							pet.TrackingInfos = await _context.PetTrackingInfo.Where(t => t.PetId == petCollar.PetId).Take(10).ToListAsync();

							return View(pet);
						}
						return NotFound();
					}
					return NotFound();
				}
			}
			return NotFound();
		}

		// GET: Pet/Stats/5
		public async Task<IActionResult> Stats(string id, string searchString, string currentFilter, int? pageIndex, string searchString2, string currentFilter2, int? pageIndex2)
		{
			if (id == null)
			{
				return NotFound();
			}

			var pet = await _context.Pet.AsNoTracking().Include(c => c.LastTrackingInfo).Include(m => m.LastMode).Include(b => b.LastCollar).ThenInclude(c => c.Collar)
				.FirstOrDefaultAsync(m => m.PetId == id && !m.Deleted);
			if (pet == null)
			{
				return NotFound();
			}

			PetStatsModel model = _mapper.Map<PetStatsModel>(new Tuple<Pet, PetTrackingInfo>(pet, pet.LastTrackingInfo));
			if (pet.LastCollar != null && pet.LastCollar.Collar != null)
			{
				model.CollarName = pet.LastCollar.Collar.Name;
			}

			if (searchString != null)
			{
				pageIndex = 1;
			}
			else
			{
				if (currentFilter == null)
				{
					searchString = string.Empty;
				}
				else
				{
					searchString = currentFilter;
				}
			}
			model.CurrentFilter = searchString;
			int pageSize = 10;
			model.PetTrackingInfos = await PaginatedList<PetTrackingInfo>.CreateAsync(
				_context.PetTrackingInfo.Where(s => s.PetId == id && (s.Latitude.ToString().Contains(searchString) || s.Longitude.ToString().Contains(searchString))).Include(d => d.Collar).OrderByDescending(d => d.CreationDate).AsNoTracking(), pageIndex ?? 1, pageSize);

			try
			{
				Tuple<string, List<string>> files = await ReadFiles(pet);
				ViewData["imagesFilenames"] = files.Item2;
			}
			catch (Exception e) { }

			model.MediumAvgDistance = new double[24];
			for (int j = 0; j < 24; j++)
			{
				model.MediumAvgDistance[j] = 1;
			}

			model.MediumAvgDistanceDay = model.MediumAvgDistance.Sum();
			model.AvgDistance = new double[24];
			
			await CalculateStatsAsync(model, id);

			if (searchString2 != null)
			{
				pageIndex2 = 1;
			}
			else
			{
				if (currentFilter2 == null)
				{
					searchString2 = string.Empty;
				}
				else
				{
					searchString2 = currentFilter2;
				}
			}
			model.CurrentFilterMode = searchString2;

			model.PetModes = await PaginatedList<PetMode>.CreateAsync(
				_context.PetMode.Where(s => s.PetId == id && (s.Type.ToString().Contains(searchString2) || s.CreationDate.ToString().Contains(searchString2))).OrderByDescending(d => d.CreationDate).AsNoTracking(), pageIndex2 ?? 1, pageSize);
			return View(model);
		}

		// GET: Pet/Create
		public async Task<IActionResult> Create()
        {
            string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
            List<string> petCollars = await _context.PetCollar.Where(p => p.IsActive).Select(c => c.CollarId).ToListAsync();
            List<Collar> collars = await _context.Collar.AsNoTracking().Where(b => b.UserId == UserId && !b.Deleted && !petCollars.Contains(b.CollarId)).ToListAsync();

            EditPetViewModel model = new EditPetViewModel();
            List<SelectListItem> collarsItems = new List<SelectListItem>();
            foreach (Collar c in collars)
            {
                collarsItems.Add(new SelectListItem
                {
                    Value = c.CollarId,
                    Text = c.Name
                });
            }

            model.Collars = collarsItems;
            return View(model);
        }

        // POST: Pet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PetId,Name,Type,Breeding,Color,Age,HealthComments,CollarId,DefaultMode")] EditPetViewModel pet, IList<IFormFile> imagesFiles, string pageContent)
        {
            if (ModelState.IsValid)
            {
                Pet p = _mapper.Map<Pet>(pet);
                p.UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
                _context.Add(p);
                await _context.SaveChangesAsync();

                if (pet.CollarId != null)
                {
                    PetCollar pc = new PetCollar
                    {
                        PetId = p.PetId,
                        CollarId = pet.CollarId,
                        StartDate = DateTime.Now,
                        CreationDate = DateTime.Now,
                        IsActive = true,
                        UserId = p.UserId
                    };
                    _context.Add(pc);

                    p.LastCollarId = pc.PetCollarId;
                    _context.Update(p);

                    await _context.SaveChangesAsync();
                }

                SaveFiles(p, imagesFiles, pageContent);

                //send message to connected devices
                if (pet.CollarId != null)
                {
                    var foundPetCollar = await _context.PetCollar.AsNoTracking().FirstAsync(c => c.PetCollarId == p.LastCollarId);
                    var foundCollar = await _context.Collar.AsNoTracking().FirstAsync(c => c.CollarId == foundPetCollar.CollarId && !c.Deleted);
                    foundCollar.Name = pet.Name;
                    Infrastructure.CustomWebSockets.Messages.CollarCore cc = _mapper.Map<Infrastructure.CustomWebSockets.Messages.CollarCore>(foundCollar);
                    cc.IsLost = false;
                    await _webSocketMessageHandler.SendCollarCoreAsync(cc, _userManager.GetUserName(User), _wsFactory);
                }

                await SetModeAsync(pet.DefaultMode, true, p.PetId);

                return RedirectToAction(nameof(Index));
            }
            return View(pet);
        }

        // GET: Pet/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pet.FirstOrDefaultAsync(e => e.PetId == id && !e.Deleted);
            if (pet == null)
            {
                return NotFound();
            }

            string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
            List<string> petCollars = await _context.PetCollar.Where(p => p.IsActive && p.PetId != pet.PetId).Select(c => c.CollarId).ToListAsync();
            List<Collar> collars = await _context.Collar.AsNoTracking().Where(b => b.UserId == UserId && !b.Deleted && !petCollars.Contains(b.CollarId)).ToListAsync();

            EditPetViewModel model = new EditPetViewModel();
            model = _mapper.Map<EditPetViewModel>(pet);
            if (pet.LastCollarId != null)
            {
                model.CollarId = _context.PetCollar.FirstOrDefault(c => c.PetCollarId == pet.LastCollarId).CollarId;
            }
            List<SelectListItem> collarsItems = new List<SelectListItem>();
            foreach (Collar c in collars)
            {
                collarsItems.Add(new SelectListItem
                {
                    Value = c.CollarId,
                    Text = c.Name
                });
            }

            model.Collars = collarsItems;

            try
            {
                Tuple<string, List<string>> files = await ReadFiles(pet);
                ViewData["pageContent"] = files.Item1;
                ViewData["imagesFilenames"] = files.Item2;
            }
            catch (Exception e) { }
            return View(model);
        }

        // POST: Pet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PetId,Name,Type,Breeding,Color,Age,HealthComments,CollarId,DefaultMode")] EditPetViewModel pet, IList<IFormFile> imagesFiles, string pageContent)
        {
            if (id != pet.PetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Pet p = _mapper.Map<Pet>(pet);
                    p.UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;

                    Pet currentPet = _context.Pet.AsNoTracking().Include(m => m.LastCollar).Include(m => m.LastMode).FirstOrDefault(g => g.PetId == pet.PetId);
                    PetCollar currentCollar = _context.PetCollar.FirstOrDefault(c => c.PetCollarId == currentPet.LastCollar.PetCollarId);
                    p.LastCollarId = currentPet.LastCollarId;
                    p.LastTrackingInfoId = currentPet.LastTrackingInfoId;

                    bool isLost = false;
                    PetMode currentPetMode = currentPet.LastMode;
                    p.LastModeId = currentPet.LastModeId;
                    if (currentPetMode != null)
                    {
                        if (currentPetMode.Type == ConfigModeTypes.Emergency && currentPetMode.IsActive)
                        {
                            isLost = true;
                        }
                    }
                    _context.Update(p);
                    await _context.SaveChangesAsync();

                    if (currentCollar == null || pet.CollarId != currentCollar.CollarId)
                    {
                        if (currentCollar != null)
                        {
                            currentCollar.IsActive = false;
                        }

                        PetCollar pc = new PetCollar
                        {
                            PetId = p.PetId,
                            CollarId = pet.CollarId,
                            StartDate = DateTime.Now,
                            CreationDate = DateTime.Now,
                            IsActive = true,
                            UserId = p.UserId
                        };
                        _context.Add(pc);

                        p.LastCollarId = pc.PetCollarId;
                        _context.Update(p);
                        await _context.SaveChangesAsync();

                    }
                    SaveFiles(p, imagesFiles, pageContent); 

                    //send message to connected devices
                    if (pet.CollarId != null)
                    {
                        var foundPetCollar = await _context.PetCollar.AsNoTracking().FirstAsync(c => c.PetCollarId == p.LastCollarId);
                        var foundCollar = await _context.Collar.AsNoTracking().FirstAsync(c => c.CollarId == foundPetCollar.CollarId && !c.Deleted);
                        foundCollar.Name = pet.Name;
                        Infrastructure.CustomWebSockets.Messages.CollarCore cc = _mapper.Map<Infrastructure.CustomWebSockets.Messages.CollarCore>(foundCollar);
                        cc.IsLost = isLost;
                        await _webSocketMessageHandler.SendCollarCoreAsync(cc, _userManager.GetUserName(User), _wsFactory);
                    }
                    if (currentPetMode == null || pet.DefaultMode != currentPet.DefaultMode)
                    {
                        await SetModeAsync(pet.DefaultMode, true, p.PetId);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetExists(pet.PetId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pet);
        }

        // GET: Pet/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pet
                .FirstOrDefaultAsync(m => m.PetId == id && !m.Deleted);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // POST: Pet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var pet = await _context.Pet.FindAsync(id);
            //_context.Pet.Remove(pet);
            //soft delete
            pet.Deleted = true;
            _context.Update(pet);

            var petCollar = _context.PetCollar.FirstOrDefault(c => c.PetCollarId == pet.LastCollarId);
            petCollar.IsActive = false;
            petCollar.EndDate = DateTime.Now;
            _context.Update(petCollar);

            await _context.SaveChangesAsync();

            DeleteFiles(pet);

            //send message to connected devices
            if (pet.LastCollarId != null)
            {
                var foundPetCollar = await _context.PetCollar.AsNoTracking().FirstAsync(c => c.PetCollarId == pet.LastCollarId);
                var foundCollar = await _context.Collar.AsNoTracking().FirstAsync(c => c.CollarId == foundPetCollar.CollarId);
                Infrastructure.CustomWebSockets.Messages.CollarCore cc = _mapper.Map<Infrastructure.CustomWebSockets.Messages.CollarCore>(foundCollar);
                cc.IsLost = false;
                await _webSocketMessageHandler.SendCollarCoreAsync(cc, _userManager.GetUserName(User), _wsFactory);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EmergencyOn(string id)
        {
            bool result = await SetModeAsync(ConfigModeTypes.Emergency, true, id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EmergencyOff(string id)
        {
            bool result = await SetModeAsync(ConfigModeTypes.Emergency, false, id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SetModeAsync(ConfigModeTypes type, bool activate, string id)
        {
            Pet pet = _context.Pet.Where(p => p.PetId == id).Include(b => b.LastCollar).FirstOrDefault();
            if (pet == null)
            {
                return false;
            }
            else
            {
                try
                {
                    PetMode currentpm = _context.PetMode.Find(pet.LastModeId);
                    if (currentpm != null)
                    {
                        currentpm.IsActive = false;
                        currentpm.EndDate = DateTime.Now;
                        _context.Update(currentpm);
                    }
                    PetMode pm = new PetMode()
                    {
                        PetId = pet.PetId,
                        CollarId = pet.LastCollar.CollarId,
                        Type = (activate ? type : pet.DefaultMode),
                        CreationDate = DateTime.Now,
                        StartDate = DateTime.Now,
                        UserId = pet.UserId,
                        IsActive = true
                    };

                    _context.Add(pm);
                    pet.LastModeId = pm.PetModeId;
                    _context.Update(pet);

                    await _context.SaveChangesAsync();
                    var collar = _context.Collar.Find(pet.LastCollar.CollarId);
                    await _webSocketMessageHandler.SendSwitchModeAsync(collar.CollarNumber, (activate ? type : pet.DefaultMode), _userManager.GetUserName(User), _wsFactory);
                }
                catch (Exception e)
                {
                    var test = e.Message;
                }
                return true;
            }
        }

		[HttpGet]
		public async Task<IActionResult> StatsPeriod(string id, string period = "W")
		{
			var pet = await _context.Pet.AsNoTracking().FirstOrDefaultAsync(m => m.PetId == id && !m.Deleted);

			PetStatsModel model = new PetStatsModel
			{
				Name = pet.Name,

				AvgDistance = new double[24]
			};

			await CalculateStatsAsync(model, id, period);
			 
			return PartialView("_ActivityStatsPartial", model);
		}
		private async Task CalculateStatsAsync(PetStatsModel model, string id, string period = "W")
		{
			List<PetTrackingInfo> PetTrackingInfos = new List<PetTrackingInfo>();

			if (period == "W")
			{
				PetTrackingInfos = _context.PetTrackingInfo.Where(s => s.PetId == id && s.CreationDate > DateTime.Now.AddDays(-7)).OrderBy(s => s.CreationDate).ToList();
			}
			else if (period == "M")
			{
				PetTrackingInfos = _context.PetTrackingInfo.Where(s => s.PetId == id && s.CreationDate > DateTime.Now.AddMonths(-1)).OrderBy(s => s.CreationDate).ToList();
			}
			else
			{
				PetTrackingInfos = _context.PetTrackingInfo.Where(s => s.PetId == id && s.CreationDate > DateTime.Now.AddMonths(-6)).OrderBy(s => s.CreationDate).ToList();
			}

			User user = await _context.User.FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));
			model.Latitude = user.Latitude;
			model.Longitude = user.Longitude;

			double[] totalDistance = new double[24];

			for (int i = 0; i < PetTrackingInfos.Count - 1; i++)
			{
				var distance = DistanceCalculation.Calculate(PetTrackingInfos[i].Latitude, PetTrackingInfos[i].Longitude, PetTrackingInfos[i + 1].Latitude, PetTrackingInfos[i + 1].Longitude, 'K');
				int hour = ((PetTrackingInfos[i + 1].CreationDate.Hour + user.Offset) < 0) ? (PetTrackingInfos[i + 1].CreationDate.Hour + user.Offset + 24) : (((PetTrackingInfos[i + 1].CreationDate.Hour + user.Offset) > 23) ? (PetTrackingInfos[i + 1].CreationDate.Hour + user.Offset - 24) : (PetTrackingInfos[i + 1].CreationDate.Hour + user.Offset));
				totalDistance[hour] += distance;
			}

			double totaldays = 0;
			if (PetTrackingInfos.Count > 0)
				totaldays = (PetTrackingInfos[PetTrackingInfos.Count - 1].CreationDate.Date - PetTrackingInfos[0].CreationDate.Date).TotalDays;

			for (int j = 0; j < 24; j++)
			{
				if (totaldays > 0)
					model.AvgDistance[j] = totalDistance[j] / totaldays;
				else
					model.AvgDistance[j] = totalDistance[j];
			}

			model.AvgDistanceDay = model.AvgDistance.Sum();

			model.PointVisited = new List<Tuple<PetTrackingInfo, string>>();
			foreach (PetTrackingInfo pti in PetTrackingInfos)
			{
				string color = string.Empty;
				int hour = ((pti.CreationDate.Hour + user.Offset) < 0) ? (pti.CreationDate.Hour + user.Offset + 24) : (((pti.CreationDate.Hour + user.Offset) > 23) ? (pti.CreationDate.Hour + user.Offset - 24) : (pti.CreationDate.Hour + user.Offset));

				if (hour < 2)
				{
					color = "purple";
				}
				else if (hour < 6)
				{
					color = "red";
				}
				else if (hour < 10)
				{
					color = "orange";
				}
				else if (hour < 14)
				{
					color = "yellow";
				}
				else if (hour < 18)
				{
					color = "green";
				}
				else if (hour < 22)
				{
					color = "blue";
				}
				else
				{
					color = "purple";
				}
				model.PointVisited.Add(new Tuple<PetTrackingInfo, string>(pti, color));
			}
		}

		[HttpPost]
        public FileStreamResult UploadImages(string PetId, IList<IFormFile> files)
        {
            FileStreamResult result = null;
            foreach (IFormFile file in files)
            {
                using (Image img = Image.FromStream(file.OpenReadStream()))
                {
                    Stream ms = new MemoryStream(img.Resize().ToByteArray());
                    string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
                    var path = Path.Combine(CustomPaths.GetImagesPetPath(UserId, PetId), file.FileName);

                    if (!Directory.Exists(Path.GetDirectoryName(path)))
                        Directory.CreateDirectory(Path.GetDirectoryName(path));

                    if (System.IO.File.Exists((path)))
                    {
                        System.IO.File.Delete(path);
                    }

                    img.Resize().Save(path);

                    result = new FileStreamResult(ms, "image/jpg");
                }
            }
            return result;
        }


        private bool PetExists(string id)
        {
            return _context.Pet.Any(e => e.PetId == id && !e.Deleted);
        }

        private async Task<Tuple<string, List<string>>> ReadFiles(Pet pet)
        {
            List<string> imagesFilenames = new List<string>();
            string pageContent = string.Empty;

            var imgpath = CustomPaths.GetImagesPetPath(pet.UserId, pet.PetId);
            if (Directory.Exists(Path.GetDirectoryName(imgpath)))
            {
                DirectoryInfo d = new DirectoryInfo(imgpath);
                FileInfo[] Files = d.GetFiles();
                foreach (FileInfo file in Files)
                {
                    imagesFilenames.Add(Request.Scheme + "://" + Request.Host + $"/user/{pet.UserId}/{pet.PetId}/images/{file.Name}");
                }
            }

            var htmlpath = Path.Combine(CustomPaths.GetPagesPetPath(pet.UserId, pet.PetId), "profile.html");
            if (System.IO.File.Exists((htmlpath)))
            {
                pageContent = await System.IO.File.ReadAllTextAsync(htmlpath);
            }
            return new Tuple<string, List<string>>(pageContent, imagesFilenames);
        }

        private void SaveFiles(Pet pet, IList<IFormFile> imagesFiles, string pageContent)
        {
            var dirpath = CustomPaths.GetImagesPetPath(pet.UserId, pet.PetId);
            foreach (IFormFile file in imagesFiles)
            {
                using (Image img = Image.FromStream(file.OpenReadStream()))
                {
                    Stream ms = new MemoryStream(img.Resize().ToByteArray());
                    var imgpath = Path.Combine(dirpath, file.FileName);

                    if (!Directory.Exists(Path.GetDirectoryName(imgpath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(imgpath));

                    if (System.IO.File.Exists((imgpath)))
                    {
                        System.IO.File.Delete(imgpath);
                    }

                    img.Resize().Save(imgpath);
                }
            }
            var htmlpath = Path.Combine(CustomPaths.GetPagesPetPath(pet.UserId, pet.PetId), "profile.html");
            if (!Directory.Exists(Path.GetDirectoryName(htmlpath)))
                Directory.CreateDirectory(Path.GetDirectoryName(htmlpath));

            if (System.IO.File.Exists((htmlpath)))
            {
                System.IO.File.Delete(htmlpath);
            }
            System.IO.File.WriteAllTextAsync(htmlpath, pageContent);
        }

        private void DeleteFiles(Pet pet)
        {
            var petpath = CustomPaths.GetPetPath(pet.UserId, pet.PetId);
            if (Directory.Exists(Path.GetDirectoryName(petpath)))
            {
                Directory.Delete(petpath, true);
            }
        }
    }
}
