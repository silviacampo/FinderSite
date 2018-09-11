using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
    public class BaseCore
    {
		[JsonIgnore]
		public string BaseId { get; set; }
		[JsonIgnore]
		public string HWId { get; set; }

		public string Name { get; set; }
		public byte BaseNumber { get; set; }

		[JsonIgnore]
		public string Description { get; set; }
		[JsonIgnore]
		public string UserId { get; set; }
	}
}