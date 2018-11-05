using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
	public class Device
    {
		[JsonIgnore]
		public string DeviceId { get; set; }

		[JsonProperty(PropertyName = "t")]
		public string Type { get; set; }
		[JsonProperty(PropertyName = "p")]
		public string Platform { get; set; }
		[JsonProperty(PropertyName = "n")]
		public string Name { get; set; }
		[JsonProperty(PropertyName = "mo")]
		public string Model { get; set; }
		[JsonProperty(PropertyName = "ma")]
		public string Manufacturer { get; set; }
		[JsonProperty(PropertyName = "os")]
		public string OSVersion { get; set; }

		[JsonIgnore]
		public string UserId { get; set; }
	}
}
