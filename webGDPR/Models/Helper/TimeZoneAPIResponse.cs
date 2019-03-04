namespace webGDPR.Models.Helper
{
	public class TimeZoneAPIResponse
	{
		public int dstOffset { get; set; }
		public int rawOffset { get; set; }
		public string status { get; set; }
		public string timeZoneId { get; set; }
		public string timeZoneName { get; set; }
	}
}
