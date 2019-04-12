using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using webGDPR.Data;
using webGDPR.Infrastructure;
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
		private readonly IStringLocalizer<LoginModel> _localizer;

		public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
			ApplicationDbContext context,
			ILogger<RegisterModel> logger,
            IEmailSender emailSender, IStringLocalizer<LoginModel> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
			_context = context;
			_logger = logger;
            _emailSender = emailSender;
			_localizer = localizer;
		}

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
			[Required(ErrorMessage = "Your Username is required")]
			[DataType(DataType.Text)]
			[Display(Name = "Username")]
			public string Name { get; set; }

			//[Required]
			//[Range(0, 199, ErrorMessage = "Age must be between 0 and 199 years")]
			//[Display(Name = "Age")]
			//public string Age { get; set; }

			[Required(ErrorMessage = "Your Email is required")]
			[EmailAddress]
            [Display(Name = "Email (To alert you about new phones connecting to your account)")]
            public string Email { get; set; }

			[Required(ErrorMessage = "Your Password is required")]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

			[Required(ErrorMessage = "The confirmation of your Password is required")]
			[DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

			[Required(ErrorMessage = "The Base Code is required")]
			[DataType(DataType.Text)]
			[Display(Name = "Base Code (ie FB654321)")]
			public string BaseHWId { get; set; }

			[Required(ErrorMessage = "The Collar Code is required")]
			[DataType(DataType.Text)]
			[Display(Name = "Collar Code (ie FC123456)")]
			public string CollarHWId { get; set; }

			[DataType(DataType.Text)]
			[Display(Name = "Pet Name (Optional, default to Pet 1)")]
			public string PetName { get; set; }

			[Display(Name = "I accept to subscribe to the monthly plan")]
			[MustBeTrue(ErrorMessage = "You have to accept the subscription.")]
			public bool Subscribe { get; set; }

			[Display(Name = "I accept the Terms and Conditions.")]
			[MustBeTrue(ErrorMessage = "You have to accept the Terms and Conditions.")]
			public bool AcceptTC { get; set; }
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

			Pet p = new Pet
			{
				Name = Input.PetName?? Pet.InitialName,
				UserId = u.UserID
			};
			_context.Add(p);
			await _context.SaveChangesAsync();

			PetCollar pc = new PetCollar
			{
				PetId = p.PetId,
				CollarId = c.CollarId,
				StartDate = DateTime.Now,
				CreationDate = DateTime.Now,
				IsActive = true,
				UserId = u.UserID
			};
			_context.Add(pc);

			p.LastCollarId = pc.PetCollarId;
			_context.Update(p);

			Subscription subs = new Subscription();
			subs.CreationDate = DateTime.Now;
			subs.UserId = u.UserID;
			subs.ProductId = _context.Product.FirstOrDefault(prod => prod.Name == "Subscription" && prod.IsActive).ProductId;
			_context.Add(subs);
			await _context.SaveChangesAsync();
		}

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Identity/Account/InstallApplication");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
				{
					Name = Input.Name,
					//Age = Int32.Parse(Input.Age),
					UserName = Input.Name,
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
                        values: new { userId = user.Id,  code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, _localizer["Confirm your email"],
                        $"{_localizer["Please confirm your account by "]}<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer["clicking here"]}</a>.");

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
