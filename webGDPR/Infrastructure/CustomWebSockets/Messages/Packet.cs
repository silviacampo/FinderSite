using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
	public class Packet
	{
		[JsonProperty(PropertyName = "d")]
		public byte[] Data { get; set; }

		[JsonProperty(PropertyName = "hw")]
		public string HWId { get; set; }
	}
}
