using System;

namespace webGDPR.Infrastructure
{
	public class TimelineItem
	{
		public DateTime ItemDate { get; set; }
		public string ItemLeftTitle { get; set; }
		public string ItemMessage { get; set; }
		public string ItemMore { get; set; }
		public TimelineItemOrientation Orientation { get; set; }
	}

	public enum TimelineItemOrientation
	{
		left,
		right
	}
}
