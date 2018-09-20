using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace webGDPR.Infrastructure.CustomWebSockets
{
	public class CustomWebSocket
	{
		public WebSocket WebSocket { get; set; }
		public string Username { get; set; }
		public string DeviceId { get; set; }
		public Guid Guid { get; set; }
	}
}
