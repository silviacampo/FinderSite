//dotnet aspnet-codegenerator controller -name PetController -async -m webGDPR.Models.Pet -dc webGDPR.Data.ApplicationDbContext -namespace webGDPR.Controllers -outDir Controllers
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AgendaSignalR.Infrastructure;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webGDPR.Data;
using webGDPR.Infrastructure;
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

		private const string petImageDir = @"wwwroot\\images\\{UserId}\\{PetId}";
		private const string petPageDir = @"wwwroot\html\{UserId}\{PetId}";

		public PetController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IHostingEnvironment hostingEnvironment, ICustomWebSocketMessageHandler webSocketMessageHandler, ICustomWebSocketFactory wsFactory)
		{
            _context = context;
			_userManager = userManager;
			_mapper = mapper;
			_hostingEnvironment = hostingEnvironment;
			_webSocketMessageHandler = webSocketMessageHandler;
			_wsFactory = wsFactory;
		}

        // GET: Pet
        public async Task<IActionResult> Index()
        {
			List<PetViewModel> model = new List<PetViewModel>();
			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;

			List<Pet> pets = await _context.Pet.AsNoTracking().Where(b => b.UserId == UserId).Include(c=>c.LastTrackingInfo).Include(b => b.LastCollar).ThenInclude(c => c.Collar).ToListAsync();
			foreach (var c in pets)
			{
				PetViewModel cvm = _mapper.Map<PetViewModel>(new Tuple<Pet, PetTrackingInfo>(c, c.LastTrackingInfo));
				if (c.LastCollar != null)
				{
					cvm.CollarName = c.LastCollar.Collar.Name;
				}
				model.Add(cvm);
			}

			return View(model);
        }

        // GET: Pet/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pet
                .FirstOrDefaultAsync(m => m.PetId == id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // GET: Pet/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PetId,Name,Type,Breeding,Color,Age,HealthComments,ImageFileName,PageFileName")] Pet pet, IList<IFormFile> imagesFiles, string pageContent)
        {
            if (ModelState.IsValid)
            {
				pet.UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
				_context.Add(pet);
                await _context.SaveChangesAsync();

				SaveFiles(pet, imagesFiles, pageContent);

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

            var pet = await _context.Pet.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
			Tuple<string, List<string>> files = await ReadFiles(pet);
			ViewData["pageContent"] = files.Item1;
			ViewData["imagesFilenames"] = files.Item2;
			return View(pet);
        }

        // POST: Pet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PetId,Name,Type,Breeding,Color,Age,HealthComments,ImageFileName,PageFileName")] Pet pet, IList<IFormFile> imagesFiles, string pageContent)
        {
            if (id != pet.PetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					var found = await _context.Pet.AsNoTracking().FirstAsync(c => c.PetId == id);
					pet.UserId = found.UserId;

					_context.Update(pet);
                    await _context.SaveChangesAsync();

					SaveFiles(pet, imagesFiles, pageContent);
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
                .FirstOrDefaultAsync(m => m.PetId == id);
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
            _context.Pet.Remove(pet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
					var path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{UserId}\\{PetId}", file.FileName);

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
            return _context.Pet.Any(e => e.PetId == id);
        }

		private async Task<Tuple<string, List<string>>> ReadFiles(Pet pet)
		{
			List<string> imagesFilenames = new List<string>();
			string pageContent = string.Empty;

			var imgpath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\user\\{pet.UserId}\\{pet.PetId}\\images");
			if (Directory.Exists(Path.GetDirectoryName(imgpath))) {
				DirectoryInfo d = new DirectoryInfo(imgpath);
				FileInfo[] Files = d.GetFiles();
				foreach (FileInfo file in Files)
				{
				 imagesFilenames.Add(Request.Scheme + "://"+ Request.Host + $"/user/{pet.UserId}/{pet.PetId}/images/{file.Name}");
				}
			}
			
			var htmlpath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\user\\{pet.UserId}\\{pet.PetId}\\pages\\profile.html");
			if (System.IO.File.Exists((htmlpath)))
			{
			 pageContent = await	System.IO.File.ReadAllTextAsync(htmlpath); 
			}
			return new Tuple<string, List<string>>(pageContent, imagesFilenames);
		}

		private void SaveFiles(Pet pet, IList<IFormFile> imagesFiles, string pageContent) {
			foreach (IFormFile file in imagesFiles)
			{
				using (Image img = Image.FromStream(file.OpenReadStream()))
				{
					Stream ms = new MemoryStream(img.Resize().ToByteArray());
					var imgpath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\user\\{pet.UserId}\\{pet.PetId}\\images", file.FileName);

					if (!Directory.Exists(Path.GetDirectoryName(imgpath)))
						Directory.CreateDirectory(Path.GetDirectoryName(imgpath));

					if (System.IO.File.Exists((imgpath)))
					{
						System.IO.File.Delete(imgpath);
					}

					img.Resize().Save(imgpath);
				}
			}
			var htmlpath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\user\\{pet.UserId}\\{pet.PetId}\\pages\\profile.html");
			if (!Directory.Exists(Path.GetDirectoryName(htmlpath)))
				Directory.CreateDirectory(Path.GetDirectoryName(htmlpath));

			if (System.IO.File.Exists((htmlpath)))
			{
				System.IO.File.Delete(htmlpath);
			}
			System.IO.File.WriteAllTextAsync(htmlpath, pageContent);
		}

	}
}
