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

		public byte CollarNumber { get; set; }
		public byte BaseNumber { get; set; }

		public bool IsConnected { get; set; }

        [JsonProperty("ConnectedTo")]
        public string ConnectedToName { get; set; }

		public bool IsGPSConnected { get; set; }

		public int Battery { get; set; }
		public int Radio { get; set; }

		[JsonIgnore]
		public string UserId { get; set; }
	}
}
