using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
    public class CollarStatus
    {
		[JsonIgnore]
		public string CollarId { get; set; }

		[JsonProperty(PropertyName = "cn")]
		public byte CollarNumber { get; set; }
		[JsonProperty(PropertyName = "bn")]
		public byte BaseNumber { get; set; }

		[JsonProperty(PropertyName = "co")]
		[JsonConverter(typeof(BoolConverter))]
		public bool IsConnected { get; set; }

		[JsonProperty("cot")]
		public string ConnectedToName { get; set; }

		[JsonProperty(PropertyName = "gps")]
		[JsonConverter(typeof(BoolConverter))]
		public bool IsGPSConnected { get; set; }

		[JsonProperty(PropertyName = "bt")]
		public int Battery { get; set; }

		[JsonProperty(PropertyName = "r")]
		public int Radio { get; set; }

		[JsonIgnore]
		public string UserId { get; set; }
	}
}
