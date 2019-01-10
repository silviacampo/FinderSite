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
			List<CustomWebSocket> socketswithsamedevice = List.Where(d => d.DeviceId == uws.DeviceId).ToList();
			List.Add(uws);
			foreach(CustomWebSocket c in socketswithsamedevice)
			{
				List.Remove(c);
			}
		}

		//when disconnect
		public void Remove(Guid guid) {
			try
			{
				List.Remove(Client(guid));
			}
			catch (Exception e) {
				var test = e.Message;
			}
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
			return List.First(c => c.Guid == guid);
		}

		public CustomWebSocket ClientByDeviceId(string deviceId)
		{
			return List.First(c => c.DeviceId == deviceId);
		}
	}
}
