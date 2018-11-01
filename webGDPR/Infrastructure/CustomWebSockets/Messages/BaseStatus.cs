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

		public byte BaseNumber { get; set; }
		public bool IsConnected { get; set; }
        [JsonProperty("ConnectedTo")]
        public string ConnectedToName { get; set; }

		public bool IsPlugged { get; set; }

		public bool IsCharging { get; set; }
		public int Battery { get; set; }

		public bool HasBattery { get; set; }

		public int Radio { get; set; }

		[JsonIgnore]
		public string UserId { get; set; }
	}
}
