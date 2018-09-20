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
using Microsoft.AspNetCore.Mvc.Rendering;
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

			List<Pet> pets = await _context.Pet.AsNoTracking().Where(b => b.UserId == UserId && b.Deleted == false).Include(c => c.LastTrackingInfo).Include(b => b.LastCollar).ThenInclude(c => c.Collar).ToListAsync();
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

		// GET: Pet/Track/5
		public async Task<IActionResult> Track(string id)
		{
			if (id == null)
			{
				return NotFound();
			}
			
			var pet = await _context.Pet
				.FirstOrDefaultAsync(m => m.PetId == id);
			pet.LastTrackingInfo = new PetTrackingInfo
			{
				Latitude = 45.5261026,
				Longitude = -73.6830076
			};

			pet.TrackingInfos = new List<PetTrackingInfo>
				{
					new PetTrackingInfo
					{
						Latitude = 45.5273677,
						Longitude = -73.6839115
					},
					new PetTrackingInfo
					{
						Latitude = 45.5273298,
						Longitude = -73.6836115
					},
					new PetTrackingInfo
					{
						Latitude = 45.5270227,
						Longitude = -73.6833668
					},
					new PetTrackingInfo
					{
						Latitude = 45.5268926,
						Longitude = -73.6832776
					}
				};
			if (pet == null)
			{
				return NotFound();
			}
			return View(pet);
		}

		//http://localhost:51420/Pet/Map?username=SilviaCampo&password=As!123456&collarnumber=1
		[AllowAnonymous]
		public async Task<IActionResult> Map(string username, string password, int collarnumber)
		{
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(username) || collarnumber < 1)
			{
				return NotFound();
			}
			var result = await _signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: true);
			if (result.Succeeded) {
				var user = await _context.User.FirstOrDefaultAsync(m => m.Name == username);
				var collar = await _context.Collar.FirstOrDefaultAsync(m => m.UserId == user.UserID && m.CollarNumber == collarnumber);
				var petCollar = await _context.PetCollar.FirstOrDefaultAsync(m => m.IsActive && m.CollarId == collar.CollarId);
				var pet = await _context.Pet
					.FirstOrDefaultAsync(m => m.PetId == petCollar.PetId);
				pet.LastTrackingInfo = new PetTrackingInfo
				{
					Latitude = 45.5261026,
					Longitude = -73.6830076
				};

				pet.TrackingInfos = new List<PetTrackingInfo>
				{
					new PetTrackingInfo
					{
						Latitude = 45.5273677,
						Longitude = -73.6839115
					},
					new PetTrackingInfo
					{
						Latitude = 45.5273298,
						Longitude = -73.6836115
					},
					new PetTrackingInfo
					{
						Latitude = 45.5270227,
						Longitude = -73.6833668
					},
					new PetTrackingInfo
					{
						Latitude = 45.5268926,
						Longitude = -73.6832776
					}
				};
				if (pet == null)
				{
					return NotFound();
				}
				return View(pet);
			}
			return NotFound();
		}

		// GET: Pet/Create
		public async Task<IActionResult> Create()
		{
			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
			List<string> petCollars = await _context.PetCollar.Where(p => p.IsActive).Select(c => c.CollarId).ToListAsync();
			List<Collar> collars = await _context.Collar.AsNoTracking().Where(b => b.UserId == UserId && !petCollars.Contains(b.CollarId)).ToListAsync();

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
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("PetId,Name,Type,Breeding,Color,Age,HealthComments,CollarId")] EditPetViewModel pet, IList<IFormFile> imagesFiles, string pageContent)
		{
			if (ModelState.IsValid)
			{
				Pet p = _mapper.Map<Pet>(pet);
				p.UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
				_context.Add(p);
				await _context.SaveChangesAsync();

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

				SaveFiles(p, imagesFiles, pageContent);

				//send message to connected devices
				if (pet.CollarId != null)
				{
					var foundPetCollar = await _context.PetCollar.AsNoTracking().FirstAsync(c => c.PetCollarId == p.LastCollarId);
					var foundCollar = await _context.Collar.AsNoTracking().FirstAsync(c => c.CollarId == foundPetCollar.CollarId);
					foundCollar.Name = pet.Name;
					Infrastructure.CustomWebSockets.Messages.CollarCore cc = _mapper.Map<Infrastructure.CustomWebSockets.Messages.CollarCore>(foundCollar);
					await _webSocketMessageHandler.SendCollarCoreAsync(cc, _userManager.GetUserName(User), _wsFactory);
				}

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

			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
			List<string> petCollars = await _context.PetCollar.Where(p => p.IsActive && p.PetId != pet.PetId).Select(c => c.CollarId).ToListAsync();
			List<Collar> collars = await _context.Collar.AsNoTracking().Where(b => b.UserId == UserId && !petCollars.Contains(b.CollarId)).ToListAsync();

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
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, [Bind("PetId,Name,Type,Breeding,Color,Age,HealthComments,CollarId")] EditPetViewModel pet, IList<IFormFile> imagesFiles, string pageContent)
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
					PetCollar currentCollar = _context.PetCollar.FirstOrDefault(c => c.PetId == pet.PetId && c.IsActive);
					if (pet.CollarId == currentCollar.CollarId) {
						p.LastCollarId = currentCollar.PetCollarId;
					}
					_context.Update(p);
					await _context.SaveChangesAsync();
					
					if (pet.CollarId != currentCollar.CollarId)
					{
						currentCollar.IsActive = false;

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
						var foundCollar = await _context.Collar.AsNoTracking().FirstAsync(c => c.CollarId == foundPetCollar.CollarId);
						foundCollar.Name = pet.Name;
						Infrastructure.CustomWebSockets.Messages.CollarCore cc = _mapper.Map<Infrastructure.CustomWebSockets.Messages.CollarCore>(foundCollar);
						await _webSocketMessageHandler.SendCollarCoreAsync(cc, _userManager.GetUserName(User), _wsFactory);
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
			//_context.Pet.Remove(pet);
			pet.Deleted = true;
			var petCollar = _context.PetCollar.FirstOrDefault(c => c.PetCollarId == pet.LastCollarId);
			petCollar.IsActive = false;
			petCollar.EndDate = DateTime.Now;
			await _context.SaveChangesAsync();

			DeleteFiles(pet);

			//send message to connected devices
			if (pet.LastCollarId != null)
			{
				var foundPetCollar = await _context.PetCollar.AsNoTracking().FirstAsync(c => c.PetCollarId == pet.LastCollarId);
				var foundCollar = await _context.Collar.AsNoTracking().FirstAsync(c => c.CollarId == foundPetCollar.CollarId);
				Infrastructure.CustomWebSockets.Messages.CollarCore cc = _mapper.Map<Infrastructure.CustomWebSockets.Messages.CollarCore>(foundCollar);
				await _webSocketMessageHandler.SendCollarCoreAsync(cc, _userManager.GetUserName(User), _wsFactory);
			}

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
			return _context.Pet.Any(e => e.PetId == id);
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
