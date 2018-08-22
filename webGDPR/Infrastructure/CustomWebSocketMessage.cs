using System;
using System.Collections.Generic;
using System.Text;

namespace AgendaSignalR.Infrastructure
{
	class CustomWebSocketMessage
	{
		public string Text { get; set; }
		public DateTime MessagDateTime { get; set; }

		public bool IsIncoming => UserId == "system";

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
	}
}
