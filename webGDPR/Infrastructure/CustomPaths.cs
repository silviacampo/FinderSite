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
	}
}
