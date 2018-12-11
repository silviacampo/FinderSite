using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Infrastructure
{
    public class CustomPaths
    {
		public static int GetType(string directoryName) {
			int type = 0;
			switch (directoryName) {
				case "gps":
					type = 1;
					break;
				case "baseloraupdate":
					type = 3;
					break;
				case "basebleupdate":
					type = 4;
					break;
				case "collarloraupdate":
					type = 5;
					break;
				case "collargpsupdate":
					type = 2;
					break;
				case "baseconfig":
					type = 6;
					break;
				case "collarconfig":
					type = 7;
					break;
				default:
					break;
			}
			return type;
		}

		public static string GetDownloadURL(string type, string filename) {
			return $"/device/download?type={GetType(type)}&filename={filename}";
				}
		public static string GetDownloadURL(int type, string filename)
		{
			return $"/device/download?type={type}&filename={filename}";
		}
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
