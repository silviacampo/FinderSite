using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
	public class Collar
	{
		[JsonIgnore]
		public string CollarId { get; set; }

		[JsonIgnore]
		public string HWId { get; set; }

		public string Name { get; set; }
		public byte CollarNumber { get; set; }
		public byte BaseNumber { get; set; }

		public bool IsConnected { get; set; }
		public bool IsGPSConnected { get; set; }

		[JsonIgnore]
		public bool IsNotGPSConnected { get { return !IsGPSConnected; } }

		public int Battery { get; set; }
		public int Radio { get; set; }

		[JsonIgnore]
		public string RadioPercentage {	get	{ return Radio.ToString() + "%"; } }

		[JsonIgnore]
		public string Description { get; set; }

		[JsonIgnore]
		public string UserId { get; set; }
	}
}


