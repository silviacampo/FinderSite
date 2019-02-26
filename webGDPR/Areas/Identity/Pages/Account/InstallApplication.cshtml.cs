using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webGDPR.Data;
using webGDPR.Models;

namespace webGDPR.Areas.Identity.Pages.Account
{
    public class InstallApplicationModel : PageModel
    {
		public User UserInfo;

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;
		private readonly ILogger<RegisterModel> _logger;


		public InstallApplicationModel(
			UserManager<ApplicationUser> userManager,
			ApplicationDbContext context,
			ILogger<RegisterModel> logger)
		{
			_userManager = userManager;
			_context = context;
			_logger = logger;

		}

		public async Task OnGetAsync()
        {
			UserInfo = await _context.User.FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));
		}
    }
}