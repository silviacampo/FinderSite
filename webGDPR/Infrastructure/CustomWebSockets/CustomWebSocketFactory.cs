using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace webGDPR.Infrastructure.CustomWebSockets
{
	public class CustomWebSocketFactory : ICustomWebSocketFactory
	{
		List<CustomWebSocket> List;
		private readonly ILogger<CustomWebSocketFactory> _logger;

		public CustomWebSocketFactory(ILogger<CustomWebSocketFactory> logger)
		{
			List = new List<CustomWebSocket>();
			_logger = logger;
		}

		//when connect
		public void Add(CustomWebSocket uws)
		{
			try
			{
				List<CustomWebSocket> socketswithsamedevice = List.Where(d => d.DeviceId == uws.DeviceId).ToList();
			_logger.LogWarning("CustomWebSocketFactory - Add Warning: Existing sockets with same device " + socketswithsamedevice.Count);			
			foreach(CustomWebSocket c in socketswithsamedevice)
			{
				List.Remove(c);
			}
			List.Add(uws);
			}
			catch (Exception e)
			{
				_logger.LogError("CustomWebSocketFactory - Add Error: " + e.Message);
			}
		}

		//when disconnect
		public void Remove(Guid guid) {
			try
			{
				List.Remove(Client(guid));
			}
			catch (Exception e) {
				_logger.LogError("CustomWebSocketFactory - Remove Error: " + e.Message);
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
			return List.FirstOrDefault(c => c.DeviceId == deviceId);
		}
	}
}
