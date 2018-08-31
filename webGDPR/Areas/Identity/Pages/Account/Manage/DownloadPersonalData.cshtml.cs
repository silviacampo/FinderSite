using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using webGDPR.Data;
using webGDPR.Models;

namespace webGDPR.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;
		private readonly ApplicationDbContext _context;

		public DownloadPersonalDataModel(
            UserManager<ApplicationUser> userManager,
            ILogger<DownloadPersonalDataModel> logger, 
			ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
			_context = context;
		}

		private async Task<User> ReadClient(ApplicationUser user)
		{
			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;

			var userpath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\user\\{UserId}");
			if (Directory.Exists(Path.GetDirectoryName(userpath)))
			{
				
			}

			await _context.SaveChangesAsync();
			User client = new User(); // await _context.User.AsNoTracking().Where(b => b.UserID == UserId).Include(b => b.Devices).Include(b => b.Bases).ThenInclude(c => c.BaseStatus).Include(b => b.Collars).ThenInclude(c => c.CollarStatus).Include(b => b.Pets).ThenInclude(c => c.PetCollars)..ThenInclude(c => c.PetTracking).ToListAsync();
			return client;
		}

		public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }
			//TODO: zip it. include all other personal data and files
			string clientInfo = JsonConvert.SerializeObject(ReadClient(user));

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), "text/json");
        }
    }
}
