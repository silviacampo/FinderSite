using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using webGDPR.Data;
using webGDPR.Models;

namespace AgendaSignalR.Infrastructure
{
	public class CustomWebSocketMessageHandler : ICustomWebSocketMessageHandler
	{
		public async Task SendInitialMessages(CustomWebSocket userWebSocket, ApplicationDbContext dbContext, IMapper mapper)
		{
			try
			{
				WebSocket webSocket = userWebSocket.WebSocket;
				string UserId = dbContext.User.FirstOrDefault(u => u.Email == userWebSocket.Username).UserID;

				List<Base> bases = await dbContext.Base.AsNoTracking().Where(b => b.UserId == UserId).Include(b => b.LastStatus).ToListAsync();

				List<webGDPR.Infrastructure.CustomWebSockets.Messages.Base> msgBases = new List<webGDPR.Infrastructure.CustomWebSockets.Messages.Base>();
				foreach (var b in bases)
				{
					webGDPR.Infrastructure.CustomWebSockets.Messages.Base mb = mapper.Map<webGDPR.Infrastructure.CustomWebSockets.Messages.Base>(new Tuple<Base, BaseStatus>(b, b.LastStatus));
					msgBases.Add(mb);
				}
				string serialisedText = JsonConvert.SerializeObject(msgBases);
				var msg = new CustomWebSocketMessage
				{
					MessagDateTime = DateTime.Now,
					Type = WSMessageType.Bases,
					Text = serialisedText,
					UserId = CustomWebSocketMessage.SystemUserId
				};

				string serialisedMessage = JsonConvert.SerializeObject(msg);
				byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
				await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);

				List<Collar> collars = await dbContext.Collar.AsNoTracking().Where(b => b.UserId == UserId).Include(b => b.LastStatus).ToListAsync();

				List<webGDPR.Infrastructure.CustomWebSockets.Messages.Collar> msgCollars = new List<webGDPR.Infrastructure.CustomWebSockets.Messages.Collar>();
				foreach (var b in collars)
				{
					webGDPR.Infrastructure.CustomWebSockets.Messages.Collar mb = mapper.Map<webGDPR.Infrastructure.CustomWebSockets.Messages.Collar>(new Tuple<Collar, CollarStatus>(b, b.LastStatus));
					msgCollars.Add(mb);
				}

				string serialisedText2 = JsonConvert.SerializeObject(msgCollars);
				var msg2 = new CustomWebSocketMessage
				{
					MessagDateTime = DateTime.Now,
					Type = WSMessageType.Collars,
					Text = serialisedText2,
					UserId = CustomWebSocketMessage.SystemUserId
				};

				string serialisedMessage2 = JsonConvert.SerializeObject(msg2);

				byte[] bytes2 = Encoding.ASCII.GetBytes(serialisedMessage2);
				await webSocket.SendAsync(new ArraySegment<byte>(bytes2, 0, bytes2.Length), WebSocketMessageType.Text, true, CancellationToken.None);

			}
			catch (Exception e)
			{
				string test = e.Message;
			}
		}

		public async Task HandleMessage(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext, IMapper mapper)
		{
			string msg = Encoding.ASCII.GetString(buffer);
			try
			{
				//{"Text":"{\"BaseId\":\"0ca407e1-5575-462c-9019-80643a9099e0\",\"BaseNumber\":\"1\",\"Name\":\"Home\",\"IsConnected\":true,\"ConnectedTo\":\"68b73ced-1659-483c-929e-274a97706405\",\"IsPlugged\":false,\"IsCharging\":true,\"Battery\":50,\"HasBattery\":true,\"Radio\":95,\"Description\":\"Home Description\",\"UserId\":\"bee7b8af-c902-4771-89f8-969a3318cbdb\"}","MessagDateTime":"2018-08-23T13:10:58.6645939-04:00","IsIncoming":true,"UserId":"scampo@test.com","Type":1}

				//{ "Text":"{\"CollarId\":\"68b73ced-1659-483c-929e-274a97706405\",\"BaseNumber\":\"1\", \"CollarNumber\":\"1\",\"Name\":\"Pepa\",\"IsConnected\":true,\"IsGPSConnected\":true,\"Battery\":60,\"Radio\":40,\"Description\":null,\"UserId\":\"bee7b8af-c902-4771-89f8-969a3318cbdb\"}","MessagDateTime":"2018-08-23T13:33:00.5057737-04:00","IsIncoming":true,"UserId":"scampo@test.com","Type":2}

				//{ "Text":"{\"DeviceId\":\"68b73ced-1659-483c-929e-274a97706405\",\"Name\":\"Silvia's Phone\",\"Model\":\"Nexus 5\",\"Manufacturer\":\"LG\",\"Type\":\"Phone\",\"Platform\":\"Android\",\"UserId\":\"bee7b8af-c902-4771-89f8-969a3318cbdb\"}","MessagDateTime":"2018-08-23T13:33:00.5057737-04:00","UserId":"scampo@test.com","Type":5}

				var message = JsonConvert.DeserializeObject<CustomWebSocketMessage>(msg);
				if (message.Type == WSMessageType.Base)
				{
					webGDPR.Infrastructure.CustomWebSockets.Messages.Base b = JsonConvert.DeserializeObject<webGDPR.Infrastructure.CustomWebSockets.Messages.Base>(message.Text);
					//if (b.ConnectedTo != userWebSocket.DeviceId) {
					//	throw new Exception("Wrong Device Id");
					//}
					BaseStatus lastStatus = dbContext.BaseStatus.FirstOrDefault(f => f.BaseId == b.BaseId && f.IsActive == true);
					if (lastStatus != null)
					{
						lastStatus.IsActive = false;
						dbContext.Update(lastStatus);
					}

					Base @base = mapper.Map<Base>(b);
					@base.LastStatus = mapper.Map<BaseStatus>(b);

					@base.LastStatus.CreationDate = message.MessagDateTime; //TODO: or now?
					@base.LastStatus.IsActive = true;
					dbContext.Add(@base.LastStatus);

					@base.LastStatusId = @base.LastStatus.BaseStatusId;
					dbContext.Update(@base);

					await dbContext.SaveChangesAsync();

					await BroadcastOthers(buffer, userWebSocket, wsFactory);
				}
				else if (message.Type == WSMessageType.Collar)
				{
					webGDPR.Infrastructure.CustomWebSockets.Messages.Collar c = JsonConvert.DeserializeObject<webGDPR.Infrastructure.CustomWebSockets.Messages.Collar>(message.Text);

					CollarStatus lastStatus = dbContext.CollarStatus.FirstOrDefault(f => f.CollarId == c.CollarId && f.IsActive == true);
					if (lastStatus != null)
					{
						lastStatus.IsActive = false;
						dbContext.Update(lastStatus);
					}

					Collar collar = mapper.Map<Collar>(c);
					collar.LastStatus = mapper.Map<CollarStatus>(c);

					collar.LastStatus.CreationDate = message.MessagDateTime; //TODO: or now?
					collar.LastStatus.IsActive = true;
					dbContext.Add(collar.LastStatus);

					collar.LastStatusId = collar.LastStatus.CollarStatusId;

					dbContext.Update(collar);
					await dbContext.SaveChangesAsync();
					await BroadcastOthers(buffer, userWebSocket, wsFactory);
				}
				else if (message.Type == WSMessageType.Device)
				{
					Device d = JsonConvert.DeserializeObject<Device>(message.Text);
					d.UserId = dbContext.User.FirstOrDefault(u => u.Email == message.UserId).UserID;
					Device found = dbContext.Device.FirstOrDefault(c => c.UserId == d.UserId && c.Type == d.Type && c.Platform == d.Platform && c.Manufacturer == d.Manufacturer && c.Model == d.Model && c.OSVersion == d.OSVersion);
					if (found != null)
					{
						found.Name = d.Name; // maybe the user update the phone's name
						dbContext.Update(found);
						d = found;
					}
					else
					{
						dbContext.Add(d);
					}
					await dbContext.SaveChangesAsync();
					//notify someone
					//return guid
					userWebSocket.DeviceId = d.DeviceId;
					await SendDeviceInformation(userWebSocket, d);
				}
			}
			catch (Exception e)
			{
				await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
			}
		}

		private async Task SendDeviceInformation(CustomWebSocket userWebSocket, Device c)
		{
			string serialisedText = JsonConvert.SerializeObject(c);
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.Device,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
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
