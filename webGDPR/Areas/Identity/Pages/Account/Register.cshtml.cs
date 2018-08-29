using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AgendaSignalR.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using webGDPR.Data;
using webGDPR.Models;

namespace webGDPR.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	//[ServiceFilter(typeof(HostFilter))]
	public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;
		private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
			ApplicationDbContext context,
			ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
			_context = context;
			_logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Full name")]
			public string Name { get; set; }

			[Required]
			[Range(0, 199, ErrorMessage = "Age must be between 0 and 199 years")]
			[Display(Name = "Age")]
			public string Age { get; set; }

			[Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Base Code")]
			public string BaseHWId { get; set; }

			[Required]
			[DataType(DataType.Text)]
			[Display(Name = "Collar Code")]
			public string CollarHWId { get; set; }
		}

        public void OnGet(string returnUrl = null)
        {
			ReturnUrl = returnUrl;
        }

		private async Task InitializeClient(ApplicationUser user) {
			User u = new Models.User
			{
				Email = user.Email,
				Name = user.Name,
				OwnerID = user.Id
			};
			_context.Add(u);

			Base b = new Base
			{
				HWId = Input.BaseHWId,
				UserId = u.UserID,
				Name = Base.InitialName,
				Description = Base.InitialDescription,
				BaseNumber = Base.InitialNumber
			};
			_context.Add(b);

			Collar c = new Collar
			{
				HWId = Input.CollarHWId,
				UserId = u.UserID,
				Name = Collar.InitialName,
				Description = Collar.InitialDescription,
				BaseNumber = Base.InitialNumber,
				CollarNumber = Collar.InitialNumber
			};
			_context.Add(c);

			await _context.SaveChangesAsync();
		}

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
				{
					Name = Input.Name,
					Age = Int32.Parse(Input.Age),
					UserName = Input.Email,
					Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
					await InitializeClient(user);

					_logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
