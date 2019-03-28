using System.Collections.Generic;
using webGDPR.Models;

namespace webGDPR.ViewModels
{
	public class CollarTimelineViewModel
	{
		public List<Infrastructure.TimelineItem> Logs { get; set; }
		public Collar Collar { get; set; }
		public string Parameter { get; set; }
	}
}
