using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
    public class BaseStatus
    {
		[JsonIgnore]
		public string BaseId { get; set; }

		[JsonProperty(PropertyName = "bn")]
		public byte BaseNumber { get; set; }

		[JsonProperty(PropertyName = "co")]
		[JsonConverter(typeof(BoolConverter))]
		public bool IsConnected { get; set; }
		[JsonProperty("cot")]
		public string ConnectedToName { get; set; }

		[JsonProperty(PropertyName = "pl")]
		[JsonConverter(typeof(BoolConverter))]
		public bool IsPlugged { get; set; }

		[JsonProperty(PropertyName = "ch")]
		[JsonConverter(typeof(BoolConverter))]
		public bool IsCharging { get; set; }

		[JsonProperty(PropertyName = "bt")]
		public int Battery { get; set; }

		[JsonProperty(PropertyName = "hbt")]
		[JsonConverter(typeof(BoolConverter))]
		public bool HasBattery { get; set; }

		[JsonProperty(PropertyName = "r")]
		public int Radio { get; set; }

		[JsonIgnore]
		public string UserId { get; set; }
	}
}
