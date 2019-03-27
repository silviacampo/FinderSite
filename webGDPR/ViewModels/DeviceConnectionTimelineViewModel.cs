using System.Collections.Generic;
using webGDPR.Models;

namespace webGDPR.ViewModels
{
	public class DeviceConnectionTimelineViewModel
	{
		public List<Infrastructure.TimelineItem> Logs { get; set; }
		public Device Device { get; set; }
	}
}
