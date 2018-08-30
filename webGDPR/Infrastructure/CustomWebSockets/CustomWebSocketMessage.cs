using System;
using System.Collections.Generic;
using System.Text;

namespace AgendaSignalR.Infrastructure
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
		None,
		Base,
		Collar,
		Bases,
		Collars,
		Device,
		LastGPS,
		TrackingInfo
	}
}
