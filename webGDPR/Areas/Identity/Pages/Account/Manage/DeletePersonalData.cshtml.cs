using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using webGDPR.Data;
using webGDPR.Infrastructure;

namespace webGDPR.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
		private readonly ApplicationDbContext _context;

		public DeletePersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<DeletePersonalDataModel> logger, 
			ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
			_context = context;
		}

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

		private async Task EndClient(ApplicationUser user) {
			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;

			var userpath = CustomPaths.GetUserPath(UserId);
			if (Directory.Exists(userpath)) {
				Directory.Delete(userpath,true);
			}

			_context.RemoveRange(_context.PetTrackingInfo.Where(c=>c.UserId == UserId));
			_context.RemoveRange(_context.PetCollar.Where(c => c.UserId == UserId));
			var pets = _context.Pet.Where(c => c.UserId == UserId).ToList();
			foreach (Models.Pet p in pets) {
				p.LastCollarId = null;
			}
			var baseStatus = _context.BaseStatus.Where(c => c.UserId == UserId).ToList();
			foreach (Models.BaseStatus p in baseStatus)
			{
				p.ConnectedTo = null;
			}
			await _context.SaveChangesAsync();

			_context.RemoveRange(_context.Pet.Where(c => c.UserId == UserId));
			_context.RemoveRange(_context.CollarStatus.Where(c => c.UserId == UserId));
			_context.RemoveRange(_context.BaseStatus.Where(c => c.UserId == UserId));
			await _context.SaveChangesAsync();

			_context.RemoveRange(_context.Collar.Where(c => c.UserId == UserId));
			_context.RemoveRange(_context.Base.Where(c => c.UserId == UserId));
			_context.RemoveRange(_context.Device.Where(c => c.UserId == UserId));
			_context.Remove(_context.User.Find(UserId));

			await _context.SaveChangesAsync();
		}


		public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password not correct.");
                    return Page();
                }
            }
			//TODO: test delete all the other tables and files too
			await EndClient(user);
			var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleteing user with ID '{userId}'.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}