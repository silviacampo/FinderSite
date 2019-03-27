using System.Collections.Generic;
using webGDPR.Models;

namespace webGDPR.ViewModels
{
	public class BaseConnectionTimelineViewModel
	{
		public List<Infrastructure.TimelineItem> Logs { get; set; }
		public Base Base { get; set; }
		public string Parameter { get; set; }
	}
}
