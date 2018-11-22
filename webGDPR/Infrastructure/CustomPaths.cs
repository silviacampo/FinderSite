using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Infrastructure
{
    public class CustomPaths
    {
		public static string GetUserPath(string UserId)
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "user", UserId);
		}

		public static string GetPetPath(string UserId, string PetId)
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "user", UserId, PetId);
		}

		public static string GetImagesPetPath(string UserId, string PetId)
		{
			return Path.Combine(GetPetPath(UserId, PetId), "images");
		}

		public static string GetPagesPetPath(string UserId, string PetId)
		{
			return Path.Combine(GetPetPath(UserId, PetId), "pages");
		}

		public static string GetGPSEphemerisPath()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","download", "gps");
		}

		public static string GetBaseLoraUpdatePath()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "download", "baseloraupdate");
		}

		public static string GetBaseBleUpdatePath()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "download", "basebleupdate");
		}

		public static string GetCollarLoraUpdatePath()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "download", "collarloraupdate");
		}

		public static string GetCollarGPSUpdatePath()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "download", "collargpsupdate");
		}

		public static string GetBaseConfigPath()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "download", "baseconfig");
		}

		public static string GetCollarConfigPath()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "download", "collarconfig");
		}

		internal static string GetDownloadPath()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "download");
		}
	}
}
