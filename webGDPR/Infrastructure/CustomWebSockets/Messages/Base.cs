using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
	public class Base
		{
		[JsonIgnore]
		public string BaseId { get; set; }

		[JsonProperty(PropertyName = "hw")]
		//[JsonIgnore]
		public string HWId { get; set; }

		[JsonProperty(PropertyName = "n")]
		public string Name { get; set; }
		
		[JsonIgnore]
		public string Description { get; set; }

		[JsonProperty(PropertyName = "bn")]
		public byte BaseNumber { get; set; }

		[JsonProperty(PropertyName = "co")]
		public bool IsConnected { get; set; }
		[JsonIgnore]
		public string ConnectedTo { get; set; }
		[JsonProperty("cot")]
		public string ConnectedToName { get; set; }

		[JsonProperty(PropertyName = "pl")]
		public bool IsPlugged { get; set; }

		//public bool IsNotPlugged { get { return !IsPlugged; } }

		[JsonProperty(PropertyName = "ch")]
		public bool IsCharging { get; set; }
		[JsonProperty(PropertyName = "bt")]
		public int Battery { get; set; }
		[JsonProperty(PropertyName = "hbt")]
		public bool HasBattery { get; set; }

		//public bool IsMissingBattery { get { return !HasBattery; } }
		[JsonProperty(PropertyName = "r")]
		public int Radio { get; set; }

		//public string RadioPercentage	{ get {	return Radio.ToString() + "%";	}	}
		[JsonIgnore]
		public string Text { get; set; }

		[JsonIgnore]
		public string UserId { get; set; }
		}
	}

