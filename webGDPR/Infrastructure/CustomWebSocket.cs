using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace AgendaSignalR.Infrastructure
{
	public class CustomWebSocket
	{
		public WebSocket WebSocket { get; set; }
		public string Username { get; set; }
		public Guid Guid { get; set; }
	}
}
