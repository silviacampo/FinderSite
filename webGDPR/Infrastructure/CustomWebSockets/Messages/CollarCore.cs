using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
    public class CollarCore
    {
		[JsonIgnore]
		public string CollarId { get; set; }

		[JsonProperty(PropertyName = "hw")]
		//[JsonIgnore]
		public string HWId { get; set; }

		[JsonProperty(PropertyName = "n")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "cn")]
		public byte CollarNumber { get; set; }
		[JsonProperty(PropertyName = "bn")]
		public byte BaseNumber { get; set; }

		[JsonIgnore]
		public string Description { get; set; }

		[JsonIgnore]
		public string UserId { get; set; }
	}
}
