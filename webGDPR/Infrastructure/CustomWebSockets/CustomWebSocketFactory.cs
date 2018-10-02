using System;
using System.Collections.Generic;
using System.Linq;

namespace webGDPR.Infrastructure.CustomWebSockets
{
	public class CustomWebSocketFactory : ICustomWebSocketFactory
	{
		List<CustomWebSocket> List;

		public CustomWebSocketFactory(){
			List = new List<CustomWebSocket>();
		}

		//when connect
		public void Add(CustomWebSocket uws)
		{
			List.Add(uws);
		}
		
		//when disconnect
		public void Remove(Guid guid) {
			List.Remove(Client(guid));
		}

		public List<CustomWebSocket> All()
		{
			return List.Where(c=>c.WebSocket.State == System.Net.WebSockets.WebSocketState.Open || c.WebSocket.State == System.Net.WebSockets.WebSocketState.Connecting).ToList();
		}

		public List<CustomWebSocket> Group(string username)
		{
			return List.Where(c=>c.Username == username && (c.WebSocket.State == System.Net.WebSockets.WebSocketState.Open || c.WebSocket.State == System.Net.WebSockets.WebSocketState.Connecting)).ToList();
		}

		public List<CustomWebSocket> Others(CustomWebSocket client)
		{
			return List.Where(c => c.Username == client.Username && c.Guid != client.Guid && (c.WebSocket.State == System.Net.WebSockets.WebSocketState.Open || c.WebSocket.State == System.Net.WebSockets.WebSocketState.Connecting)).ToList();
		}

		public CustomWebSocket Client(Guid guid)
		{
			return List.First(c => c.Guid == guid && (c.WebSocket.State == System.Net.WebSockets.WebSocketState.Open || c.WebSocket.State == System.Net.WebSockets.WebSocketState.Connecting));
		}


	}
}
