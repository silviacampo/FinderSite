using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
	public class File
	{
		[JsonProperty(PropertyName = "t")]
		public int Type { get; set; }

		[JsonProperty(PropertyName = "f")]
		public string Filename { get; set; }
	}
}
