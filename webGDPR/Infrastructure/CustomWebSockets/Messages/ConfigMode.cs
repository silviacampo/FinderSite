using Newtonsoft.Json;
using webGDPR.Models;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
	public class ConfigMode
	{
		[JsonProperty(PropertyName = "t")]
		public PetModeTypes Type { get; set; }

		[JsonProperty(PropertyName = "cn")]
		public byte CollarNumber { get; set; }

		[JsonProperty(PropertyName = "c")]
		public byte[] Config { get; set; }
	}
}
