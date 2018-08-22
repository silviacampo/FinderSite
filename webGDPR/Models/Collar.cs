namespace AgendaSignalR.Infrastructure
{
	public class Collar
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public bool IsConnected { get; set; }
		public bool IsGPSConnected { get; set; }

		public bool IsNotGPSConnected { get { return !IsGPSConnected; } }

		public int Battery { get; set; }
		public int Radio { get; set; }

		public string RadioPercentage
		{
			get
			{
				return Radio.ToString() + "%";
			}
		}

		public string Description { get; set; }

		public string UserId { get; set; }
	}
}


