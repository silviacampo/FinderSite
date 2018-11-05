using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
	public class Collar
	{
		[JsonIgnore]
		public string CollarId { get; set; }

		[JsonProperty(PropertyName = "hw")]
		//[JsonIgnore]
		public string HWId { get; set; }

		[JsonProperty(PropertyName = "n")]
		public string Name { get; set; }

		[JsonIgnore]
		public string Description { get; set; }

		[JsonProperty(PropertyName = "cn")]
		public byte CollarNumber { get; set; }
		[JsonProperty(PropertyName = "bn")]
		public byte BaseNumber { get; set; }

		[JsonProperty(PropertyName = "co")]
		[JsonConverter(typeof(BoolConverter))]
		public bool IsConnected { get; set; }
		[JsonIgnore]
		public string ConnectedTo { get; set; }
		[JsonProperty("cot")]
		public string ConnectedToName { get; set; }

		[JsonProperty(PropertyName = "gps")]
		[JsonConverter(typeof(BoolConverter))]
		public bool IsGPSConnected { get; set; }

		[JsonIgnore]
		public bool IsNotGPSConnected { get { return !IsGPSConnected; } }

		[JsonProperty(PropertyName = "bt")]
		public int Battery { get; set; }

		[JsonProperty(PropertyName = "r")]
		public int Radio { get; set; }

		[JsonIgnore]
		public string RadioPercentage {	get	{ return Radio.ToString() + "%"; } }

		[JsonIgnore]
		public string UserId { get; set; }
	}
}


