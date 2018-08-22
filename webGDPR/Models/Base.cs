namespace AgendaSignalR.Infrastructure
{
	public class Base
	{
		public string BaseId { get; set; }
		public string HWId { get; set; }
		public string Name { get; set; }
		public bool IsConnected { get; set; }
		public bool IsPlugged { get; set; }

		public bool IsNotPlugged { get { return !IsPlugged; } }

		public bool IsCharging { get; set; }
		public int Battery { get; set; }

		public bool HasBattery { get; set; }

		public bool IsMissingBattery { get { return !HasBattery; } }

		public int Radio { get; set; }

		public string RadioPercentage
		{
			get
			{
				return Radio.ToString() + "%";
			}
		}

		public string Text { get; set; }
		public string Description { get; set; }

		public string UserId { get; set; }
	}
}
