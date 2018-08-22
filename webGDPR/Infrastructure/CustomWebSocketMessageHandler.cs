using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AgendaSignalR.Infrastructure
{
	public class CustomWebSocketMessageHandler : ICustomWebSocketMessageHandler
	{
		public async Task SendInitialMessages(CustomWebSocket userWebSocket)
		{
			WebSocket webSocket = userWebSocket.WebSocket;
			var mockItems = new List<Base>
			{
				new Base { BaseId = Guid.NewGuid().ToString(), Name="Bluegiga CR Demo", IsConnected= true, IsPlugged = true, IsCharging = true, Battery=80, HasBattery = true, Radio=50, Text = "First item", Description="This is an item description." },
				new Base { BaseId = Guid.NewGuid().ToString(), Name="[TV] Samsung 7 Series (55) 2", IsConnected= false, IsPlugged = false, IsCharging = false, Battery=0, HasBattery = true, Radio=80, Text = "Second item", Description="This is an item description." },
			};
			string serialisedText = JsonConvert.SerializeObject(mockItems);
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.Bases,
				Text = serialisedText,
				UserId = "system"
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);

			var mockCollars = new List<Collar>
			{
				new Collar { CollarId = Guid.NewGuid().ToString(), Name="Peppa", IsConnected= true, IsGPSConnected= false, Battery=100, Radio=90, Description="This is an item description." },
				new Collar { CollarId = Guid.NewGuid().ToString(), Name="Hunter", IsConnected= false, IsGPSConnected = true,  Battery=50, Radio=20, Description="This is an item description." },
			};
			string serialisedText2 = JsonConvert.SerializeObject(mockCollars);
			var msg2 = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.Collars,
				Text = serialisedText2,
				UserId = "system"
			};

			string serialisedMessage2 = JsonConvert.SerializeObject(msg2);

			byte[] bytes2 = Encoding.ASCII.GetBytes(serialisedMessage2);
			await webSocket.SendAsync(new ArraySegment<byte>(bytes2, 0, bytes2.Length), WebSocketMessageType.Text, true, CancellationToken.None);
		}

		public async Task HandleMessage(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
		{
			string msg = Encoding.ASCII.GetString(buffer);
			try
			{
				var message = JsonConvert.DeserializeObject<CustomWebSocketMessage>(msg);
				if (message.Type == WSMessageType.Base)
				{
					//save and send to others in same account
					//{"Text":"[]","MessagDateTime":"2018-08-22T09:01:35.1117305-04:00","IsIncoming":true,"UserId":"user3","Type":1}
					await BroadcastOthers(buffer, userWebSocket, wsFactory);
				}
				else if (message.Type == WSMessageType.Collar)
				{
					//save and send to others in same account
					await BroadcastOthers(buffer, userWebSocket, wsFactory);
				}
			}
			catch (Exception e)
			{
				await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
			}
		}

		//any change coming from one device
		public async Task BroadcastOthers(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
		{
			var others = wsFactory.Others(userWebSocket);
			foreach (var uws in others)
			{
				await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}

		//any change in the user acount started from the website
		public async Task BroadcastGroup(byte[] buffer, string username, ICustomWebSocketFactory wsFactory)
		{
			var all = wsFactory.Group(username);
			foreach (var uws in all)
			{
				await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}

		public async Task BroadcastAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
		{
			var all = wsFactory.All();
			foreach (var uws in all)
			{
				await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}
		//updates?
		public async Task BroadcastBinaryAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
		{
			var all = wsFactory.All();
			foreach (var uws in all)
			{
				await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Binary, true, CancellationToken.None);
			}
		}
	}
}
