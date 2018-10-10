using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Models
{
    public class DeviceLog
    {
		public string DeviceLogId { get; set; }
		public string DeviceId { get; set; }
		public DateTime CreationDate { get; set; }
		public string Reason { get; set; }
		public string Message { get; set; }
	}
}
