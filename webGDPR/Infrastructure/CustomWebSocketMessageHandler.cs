using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using webGDPR.Data;
using webGDPR.Models;

namespace AgendaSignalR.Infrastructure
{
	public class CustomWebSocketMessageHandler : ICustomWebSocketMessageHandler
	{
		public async Task SendInitialMessages(CustomWebSocket userWebSocket, ApplicationDbContext dbContext)
		{
			WebSocket webSocket = userWebSocket.WebSocket;
			string UserId = dbContext.User.FirstOrDefault(u => u.Email == userWebSocket.Username).UserID;

			List<Base> bases = await dbContext.Base.AsNoTracking().Where(b=>b.UserId == UserId).ToListAsync();
			string serialisedText = JsonConvert.SerializeObject(bases);
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

			List<Collar> collars = await dbContext.Collar.AsNoTracking().Where(b => b.UserId == UserId).ToListAsync();
			string serialisedText2 = JsonConvert.SerializeObject(collars);
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

		public async Task HandleMessage(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext)
		{
			string msg = Encoding.ASCII.GetString(buffer);
			try
			{ 
				//{"Text":"{\"BaseId\":\"0ca407e1-5575-462c-9019-80643a9099e0\",\"HWId\":\"12345678\",\"Name\":\"Home\",\"IsConnected\":false,\"IsPlugged\":false,\"IsNotPlugged\":true,\"IsCharging\":false,\"Battery\":0,\"HasBattery\":false,\"IsMissingBattery\":true,\"Radio\":0,\"RadioPercentage\":\"0%\",\"Text\":null,\"Description\":null,\"UserId\":\"bee7b8af-c902-4771-89f8-969a3318cbdb\"}","MessagDateTime":"2018-08-23T13:10:58.6645939-04:00","IsIncoming":true,"UserId":"11","Type":1}

				//{ "Text":"{\"CollarId\":\"68b73ced-1659-483c-929e-274a97706405\",\"HWId\":\"87654321\",\"Name\":\"Pepa\",\"IsConnected\":false,\"IsGPSConnected\":false,\"IsNotGPSConnected\":true,\"Battery\":0,\"Radio\":0,\"RadioPercentage\":\"0%\",\"Description\":null,\"UserId\":\"bee7b8af-c902-4771-89f8-969a3318cbdb\"}","MessagDateTime":"2018-08-23T13:33:00.5057737-04:00","IsIncoming":true,"UserId":"22","Type":2}

				var message = JsonConvert.DeserializeObject<CustomWebSocketMessage>(msg);
				if (message.Type == WSMessageType.Base)
				{
					Base b = JsonConvert.DeserializeObject<Base>(message.Text);
					dbContext.Update(b);
					await dbContext.SaveChangesAsync();
					await BroadcastOthers(buffer, userWebSocket, wsFactory);
				}
				else if (message.Type == WSMessageType.Collar)
				{
					Collar c = JsonConvert.DeserializeObject<Collar>(message.Text);
					dbContext.Update(c);
					await dbContext.SaveChangesAsync();
					await BroadcastOthers(buffer, userWebSocket, wsFactory);
				}
				else if (message.Type == WSMessageType.Device)
				{
					Device c = JsonConvert.DeserializeObject<Device>(message.Text);
					c.UserId = dbContext.User.FirstOrDefault(u => u.Email == message.UserId).UserID;
					dbContext.Add(c);
					await dbContext.SaveChangesAsync();
					//notify someone
					//return guid
					string test = c.DeviceId;
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
