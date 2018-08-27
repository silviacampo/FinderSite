using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
		public class Base
		{
		[JsonIgnore]
		public string BaseId { get; set; }
		[JsonIgnore]
		public string HWId { get; set; }

		public string Name { get; set; }
			public byte BaseNumber { get; set; }

			public bool IsConnected { get; set; }
			public bool IsPlugged { get; set; }

			//public bool IsNotPlugged { get { return !IsPlugged; } }

			public bool IsCharging { get; set; }
			public int Battery { get; set; }

			public bool HasBattery { get; set; }

			//public bool IsMissingBattery { get { return !HasBattery; } }

			public int Radio { get; set; }

		//public string RadioPercentage	{ get {	return Radio.ToString() + "%";	}	}
		[JsonIgnore]
		public string Text { get; set; }
		[JsonIgnore]
		public string Description { get; set; }
		[JsonIgnore]
		public string UserId { get; set; }
		}
	}

