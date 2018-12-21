using System;
using System.Security.Cryptography;
using System.Text;

namespace webGDPR.Infrastructure
{
	public static class CryptoHelper
	{
		public static string HashSHA512(this string value, string salt)
		{
			using (var sha = SHA512.Create())
			{
				return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(value, salt))));
			}
		}
	}
}