using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webGDPR.Models
{
	public class DeviceLog
    {
		public string DeviceLogId { get; set; }
		public string DeviceId { get; set; }

		//[ForeignKey("DeviceId")]
		public Device Device { get; set; }

		public DateTime CreationDate { get; set; }
		public string Reason { get; set; }
		public string Message { get; set; }
	}
}
