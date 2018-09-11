using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
    public class CollarCore
    {
		[JsonIgnore]
		public string CollarId { get; set; }

		[JsonIgnore]
		public string HWId { get; set; }

		public string Name { get; set; }
		public byte CollarNumber { get; set; }
		public byte BaseNumber { get; set; }

		[JsonIgnore]
		public string Description { get; set; }

		[JsonIgnore]
		public string UserId { get; set; }
	}
}
