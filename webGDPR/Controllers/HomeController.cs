using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using webGDPR.Infrastructure;
using webGDPR.Models;

namespace webGDPR.Controllers
{
	//[ServiceFilter(typeof(HostFilter))]
	public class HomeController : Controller
    {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly IHtmlLocalizer<HomeController> _localizer;

		public HomeController(IHtmlLocalizer<HomeController> localizer)
		{
			_localizer = localizer;
		}

		public IActionResult Index()
		{
			log.Info("test");
			//ViewData["Host"] = Request.Host.Host;
			var test = ViewData["Host"];
			return View();
        }

		public IActionResult CreateAccountWizard()
		{
			return View();
		}

		[HttpPost]
		public FileStreamResult AddFileIndex(IList<IFormFile> files)
		{
			FileStreamResult result = null;
			foreach (IFormFile file in files)
			{
				using (Image img = Image.FromStream(file.OpenReadStream()))
				{
					Stream ms = new MemoryStream(img.Resize().ToByteArray());
					var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","images","posts", file.FileName);

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

		public IActionResult About()
        {
			ViewData["Message"] = _localizer["Your application description page."];

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

		public IActionResult TermsAndConditions()
		{
			return View();
		}

		[HttpPost]
		public IActionResult SetLanguage([FromForm]string culture, string returnUrl)
		{
			Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
				new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true }
			);

			return LocalRedirect(returnUrl);
		}

		public IActionResult Chat()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
