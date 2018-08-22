using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webGDPR.Authorization;
using webGDPR.Data;
using webGDPR.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webGDPR.Controllers
{
    public class UserController : Controller
    {
		protected ApplicationDbContext Context { get; }
		protected IAuthorizationService AuthorizationService { get; }
		protected UserManager<ApplicationUser> UserManager { get; }

		public UserController(
	ApplicationDbContext context,
	IAuthorizationService authorizationService,
	UserManager<ApplicationUser> userManager) : base()
		{
			Context = context;
			UserManager = userManager;
			AuthorizationService = authorizationService;
		}

		// GET: /<controller>/
		[Authorize]
		public async Task<IActionResult> Index()
        {
			var contacts = from c in Context.User
						   select c;

			var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
							   User.IsInRole(Constants.ContactAdministratorsRole);

			var currentUserId = UserManager.GetUserId(User);

			// Only approved contacts are shown UNLESS you're authorized to see them
			// or you are the owner.
			if (!isAuthorized)
			{
				contacts = contacts.Where(c => c.OwnerID == currentUserId);
			}

			List<User> users = await contacts.ToListAsync();

            return View(users);
        }

		//
		// GET: /<controller>/Create
		[HttpGet]
		[Authorize]
		public IActionResult Create()
		{
			return View();
		}

		//
		// POST: /<controller>/Create

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Create(FormCollection collection)
		{
			try
			{
				string OwnerID = UserManager.GetUserId(User);
				string UserName = UserManager.GetUserName(User);

				ApplicationUser appUser = await UserManager.GetUserAsync(User);

				User u = new Models.User
				{
					Email = appUser.Email,
					Name = appUser.Name,
					OwnerID = appUser.Id
				};

				// requires using ContactManager.Authorization;
				var isAuthorized = await AuthorizationService.AuthorizeAsync(
															User, u,
															UserOperations.Create);
				if (!isAuthorized.Succeeded)
				{
					return new ChallengeResult();
				}

				Context.User.Add(u);
				await Context.SaveChangesAsync();

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		//
		// GET: /<controller>/Edit/5
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Edit(string id)
		{
			var Contact = await Context.User.FirstOrDefaultAsync(m => m.UserID == id);

			if (Contact == null)
			{
				return NotFound();
			}

			var isAuthorized = await AuthorizationService.AuthorizeAsync(
													  User, Contact,
													  UserOperations.Update);
			if (!isAuthorized.Succeeded)
			{
				return new ChallengeResult();
			}

			return View();
		}

		//
		// POST: /<controller>/Edit/5

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Edit(string id, User user)
		{
			try
			{
				// Fetch Contact from DB to get OwnerID.
				var contact = await Context
					.User.AsNoTracking()
					.FirstOrDefaultAsync(m => m.UserID == id);

				if (contact == null)
				{
					return NotFound();
				}

				var isAuthorized = await AuthorizationService.AuthorizeAsync(
														 User, contact,
														 UserOperations.Update);
				if (!isAuthorized.Succeeded)
				{
					return new ChallengeResult();
				}

				user.OwnerID = contact.OwnerID;

				Context.Attach(user).State = EntityState.Modified;

				await Context.SaveChangesAsync();

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		//
		// GET: /<controller>/Delete/5
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Delete(string id)
		{
			var Contact = await Context.User.FirstOrDefaultAsync(m => m.UserID == id);

			if (Contact == null)
			{
				return NotFound();
			}

			var isAuthorized = await AuthorizationService.AuthorizeAsync(
													  User, Contact,
													  UserOperations.Delete);
			if (!isAuthorized.Succeeded)
			{
				return new ChallengeResult();
			}

			return View();
		}

		//
		// POST: /<controller>/Delete/5

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Delete(string id, User user)
		{
			try
			{
				// Fetch Contact from DB to get OwnerID.
				var contact = await Context
					.User.FirstOrDefaultAsync(m => m.UserID == id);

				if (contact == null)
				{
					return NotFound();
				}

				var isAuthorized = await AuthorizationService.AuthorizeAsync(
														 User, contact,
														 UserOperations.Delete);
				if (!isAuthorized.Succeeded)
				{
					return new ChallengeResult();
				}

				Context.User.Remove(contact);
				await Context.SaveChangesAsync();

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

	}
}
