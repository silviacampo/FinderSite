using System;
using System.Collections.Generic;
using System.Text;

namespace webGDPR.Infrastructure.CustomWebSockets
{
	class CustomWebSocketMessage
	{
		public const string SystemUserId = "system";

		public string Text { get; set; }
		public DateTime MessagDateTime { get; set; }

		public bool IsIncoming => UserId == SystemUserId;

		public string UserId { get; set; }

		public WSMessageType Type { get; set; }
	}

	

	public enum WSMessageType
	{
		None = 0,
		Base = 1,
		Collar = 2,
		Bases = 3,
		Collars = 4,
		Device = 5,
		LastGPS = 6,
		TrackingInfo = 7,
		DeletedBase = 8,
		DeletedCollar = 9,
		BaseCore = 10,
		CollarCore = 11,
		BaseStatus = 12,
		CollarStatus = 13,
		
	}
}
