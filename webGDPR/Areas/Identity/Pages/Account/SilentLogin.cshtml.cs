using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using webGDPR.Data;
using webGDPR.Controllers;
using System.Web;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System;

namespace webGDPR.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class SilentLoginModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ILogger<LoginModel> _logger;
		private readonly PetController _petController;


		public SilentLoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, PetController petController)
		{
			_signInManager = signInManager;
			_logger = logger;
			_petController = petController;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public string ReturnUrl { get; set; }
		public string Username { get; set; }
		public string Language { get; set; }

		[TempData]
		public string ErrorMessage { get; set; }

		public class InputModel
		{
			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Username")]
			public string Name { get; set; }

			public string Language { get; set; }

			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }
		}

		public async Task OnGetAsync(string username = null, string language = null, string returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}

			returnUrl = returnUrl ?? Url.Content("~/user/dashboard");

			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			ReturnUrl = returnUrl;
			Username = username;
			Language = language;
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl = returnUrl ?? Url.Content("~/user/dashboard");

			if (ModelState.IsValid)
			{
				// This doesn't count login failures towards account lockout
				// To enable password failures to trigger account lockout, set lockoutOnFailure: true
				var result = await _signInManager.PasswordSignInAsync(Input.Name, Input.Password, Input.RememberMe, lockoutOnFailure: true);
				if (result.Succeeded)
				{
					_logger.LogInformation("User logged in.");

					//Response.Cookies.Append(
				//CookieRequestCultureProvider.DefaultCookieName,
				//CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(Input.Language)),
				//new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true }
			//);

					string decodedReturn = HttpUtility.UrlDecode(returnUrl);
					string[] decodedReturnParts = decodedReturn.Split(new char[] { '/', '?', '&', '=' });
					if (decodedReturnParts[1] == "Pet" && decodedReturnParts[2] == "Map")
					{
						return await _petController.Map(decodedReturnParts[4], int.Parse(decodedReturnParts[6]), ViewData["Host"].ToString());
					}
					return LocalRedirect(returnUrl);
				}
				if (result.RequiresTwoFactor)
				{
					return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
				}
				if (result.IsLockedOut)
				{
					_logger.LogWarning("User account locked out.");
					return RedirectToPage("./Lockout");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return Page();
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
