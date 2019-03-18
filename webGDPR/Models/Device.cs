using System;

namespace webGDPR.Models
{
	public class Device
    {
		public string DeviceId { get; set; }
		public string Type { get; set; }
		public string Platform { get; set; }
		public string Name { get; set; }
		public string Model { get; set; }
		public string Manufacturer { get; set; }
		public string OSVersion { get; set; }

		public string AliasName { get; set; }
		public bool Banned { get; set; }
		public bool IsLogging { get; set; }

		public string UserId { get; set; }

		public DateTime CreationDate { get; set; }

		public string GetName
		{
			get
			{
				return AliasName ?? Name;
			}
		}

		public string GetPlatform
		{
			get
			{
				if (Platform == "UWP" || Platform == "WPF") {
					return "Windows";
				}
				return Platform;
			}
		}

	}
}
