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
		public bool Aproved { get; set; }

		public string UserId { get; set; }

		public string GetName
		{
			get
			{
				return AliasName ?? Name;
			}
		}
	}
}
