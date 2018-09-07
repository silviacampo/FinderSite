using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
    public class CollarStatus
    {
		//[JsonIgnore]
		public string CollarId { get; set; }

		public bool IsConnected { get; set; }
		public string ConnectedTo { get; set; }

		public bool IsGPSConnected { get; set; }

		[JsonIgnore]
		public bool IsNotGPSConnected { get { return !IsGPSConnected; } }

		public int Battery { get; set; }
		public int Radio { get; set; }

		[JsonIgnore]
		public string RadioPercentage { get { return Radio.ToString() + "%"; } }

		//[JsonIgnore]
		public string UserId { get; set; }
	}
}
