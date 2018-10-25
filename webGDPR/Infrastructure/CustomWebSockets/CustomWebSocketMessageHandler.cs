using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using webGDPR.Data;
using webGDPR.Models;

namespace webGDPR.Infrastructure.CustomWebSockets
{
	public class CustomWebSocketMessageHandler : ICustomWebSocketMessageHandler
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public async Task SendInitialMessages(CustomWebSocket userWebSocket, ApplicationDbContext dbContext, IMapper mapper)
		{
			try
			{
				WebSocket webSocket = userWebSocket.WebSocket;
				string UserId = dbContext.User.FirstOrDefault(u => u.Name == userWebSocket.Username).UserID;

				List<Base> bases = await dbContext.Base.AsNoTracking().Where(b => b.UserId == UserId).Include(b => b.LastStatus).ThenInclude(c => c.DeviceConnectedTo).ToListAsync();

				List<Messages.Base> msgBases = new List<Messages.Base>();
				foreach (var b in bases)
				{
					Messages.Base mb = mapper.Map<Messages.Base>(new Tuple<Base, BaseStatus>(b, b.LastStatus));
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
				log.Info(serialisedMessage);
				byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
				await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);

				LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Initial Bases", serialisedMessage);

				List<Collar> collars = await dbContext.Collar.AsNoTracking().Where(b => b.UserId == UserId).Include(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).ToListAsync();

				List<Messages.Collar> msgCollars = new List<Messages.Collar>();
				foreach (var b in collars)
				{
					Messages.Collar mb = mapper.Map<Messages.Collar>(new Tuple<Collar, CollarStatus>(b, b.LastStatus));
					//go and pick up the name of the pet if pet assoc. to collar 
					PetCollar petCollar = await dbContext.PetCollar.FirstOrDefaultAsync(p => p.CollarId == b.CollarId && p.IsActive);
					if (petCollar != null) {
						mb.Name = dbContext.Pet.FirstOrDefault(pet => pet.PetId == petCollar.PetId).Name;
							}
					msgCollars.Add(mb);
				}

				string serialisedText2 = JsonConvert.SerializeObject(msgCollars);
				log.Info(serialisedText2);
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

				LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Initial Collars", serialisedMessage2);
			}
			catch (Exception e)
			{
				string test = e.Message;
			}
		}

		public void LogDeviceActivity(ApplicationDbContext dbContext, string DeviceId, string Reason, string Message)
		{
            DeviceId = DeviceId.Replace("\"", "");

            if (DeviceId == string.Empty || (dbContext.Device.FirstOrDefault(d=>d.DeviceId == DeviceId) != null && dbContext.Device.FirstOrDefault(d => d.DeviceId == DeviceId).IsLogging))
			{
				dbContext.DeviceLog.Add(new Models.DeviceLog() { DeviceId = DeviceId, CreationDate = DateTime.Now, Reason = Reason, Message = Message });
				dbContext.SaveChanges();
			}
		}

		public async Task HandleMessage(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender)
		{
			string msg = Encoding.ASCII.GetString(buffer).Trim('\0');
			try
			{
				//{"Text":"{\"BaseNumber\":\"1\",\"IsConnected\":true,\"IsPlugged\":false,\"IsCharging\":true,\"Battery\":50,\"HasBattery\":true,\"Radio\":95}","MessagDateTime":"2018-08-23T13:10:58.6645939-04:00","IsIncoming":true,"UserId":"SilviaCampo","Type":12}

				//{ "Text":"{\"CollarNumber\":\"1\",\"BaseNumber\":\"1\",\"IsConnected\":true,\"IsGPSConnected\":true,\"Battery\":60,\"Radio\":40}","MessagDateTime":"2018-08-23T13:33:00.5057737-04:00","IsIncoming":true,"UserId":"SilviaCampo","Type":13}

				//{ "Text":"{\"DeviceId\":\"68b73ced-1659-483c-929e-274a97706405\",\"Name\":\"Silvia's Phone\",\"Model\":\"Nexus 5\",\"Manufacturer\":\"LG\",\"Type\":\"Phone\",\"Platform\":\"Android\",\"UserId\":\"bee7b8af-c902-4771-89f8-969a3318cbdb\"}","MessagDateTime":"2018-08-23T13:33:00.5057737-04:00","UserId":"scampo@test.com","Type":5}

				//{"Text":"{\"DeviceId\":null,\"Type\":\"Phone\",\"Platform\":\"Android\",\"Name\":\"Nexus 5\",\"Model\":\"Nexus 5\",\"Manufacturer\":\"LGE\",\"OSVersion\":\"6.0.1\"}","MessagDateTime":"2018-09-27T09:24:43.584572-04:00","IsIncoming":false,"UserId":"gghg","Type":5}

				//{"Text":"[\"tkr\"]","MessagDateTime":"2018-10-17T13:39:57.294987-04:00","IsIncoming":false,"UserId":"SilviaCampo","Type":14}

				var message = JsonConvert.DeserializeObject<CustomWebSocketMessage>(msg);

				LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Message from device", msg);

				log.Info(userWebSocket.DeviceId + " - " + msg.Replace("\0", string.Empty));
				User user = dbContext.User.FirstOrDefault(u => u.Name == userWebSocket.Username);
				string UserId = user.UserID;

				if (message.Type == WSMessageType.BaseStatus)
				{
					Messages.BaseStatus bs = JsonConvert.DeserializeObject<Messages.BaseStatus>(message.Text);
					Base b = dbContext.Base.FirstOrDefault(f => f.BaseNumber == bs.BaseNumber && f.UserId == UserId);
					BaseStatus lastStatus = dbContext.BaseStatus.FirstOrDefault(f => f.BaseId == b.BaseId && f.IsActive == true);
					if (lastStatus != null)
					{
						lastStatus.IsActive = false;
						dbContext.Update(lastStatus);
					}

					b.LastStatus = new BaseStatus
					{
						BaseId = b.BaseId,
						ConnectedTo = userWebSocket.DeviceId,
						IsConnected = bs.IsConnected,
						IsCharging = bs.IsCharging,
						IsPlugged = bs.IsPlugged,
						Battery = bs.Battery,
						HasBattery = bs.HasBattery,
						UserId = b.UserId,
						CreationDate = message.MessagDateTime, //TODO: or now?
						IsActive = true
					};
					dbContext.Add(b.LastStatus);

					b.LastStatusId = b.LastStatus.BaseStatusId;
					dbContext.Update(b);

					await dbContext.SaveChangesAsync();
					bs.ConnectedToName = dbContext.Device.AsNoTracking().FirstOrDefault(d => d.DeviceId == userWebSocket.DeviceId).GetName;
					message.Text = JsonConvert.SerializeObject(bs);
					await BroadcastOthers1(message, userWebSocket, wsFactory);
				}
				else if (message.Type == WSMessageType.CollarStatus)
				{
					Messages.CollarStatus cs = JsonConvert.DeserializeObject<Messages.CollarStatus>(message.Text);
					Collar collar = dbContext.Collar.FirstOrDefault(f => f.CollarNumber == cs.CollarNumber && f.UserId == UserId);
					Base b = dbContext.Base.FirstOrDefault(f => f.BaseNumber == cs.BaseNumber && f.UserId == UserId);
					CollarStatus lastStatus = dbContext.CollarStatus.FirstOrDefault(f => f.CollarId == collar.CollarId && f.IsActive == true);
					if (lastStatus != null)
					{
						lastStatus.IsActive = false;
						dbContext.Update(lastStatus);
					}

					collar.LastStatus = new CollarStatus
					{
						CollarId = collar.CollarId,
						IsConnected = cs.IsConnected,
						IsGPSConnected = cs.IsGPSConnected,
						Battery = cs.Battery,
						Radio = cs.Radio,
						ConnectedTo = b.BaseId,
						UserId = collar.UserId,
						CreationDate = message.MessagDateTime, //TODO: or now?
						IsActive = true
					};
					dbContext.Add(collar.LastStatus);

					collar.LastStatusId = collar.LastStatus.CollarStatusId;

					dbContext.Update(collar);
					await dbContext.SaveChangesAsync();
					await BroadcastOthers(buffer, userWebSocket, wsFactory);
				}
				else if (message.Type == WSMessageType.Device)
				{
					Device d = JsonConvert.DeserializeObject<Device>(message.Text);
					d.UserId = UserId;
					Device found = dbContext.Device.FirstOrDefault(c => c.UserId == d.UserId && c.Type == d.Type && c.Platform == d.Platform && c.Manufacturer == d.Manufacturer && c.Model == d.Model && c.OSVersion == d.OSVersion);
					if (found != null)
					{
						found.Name = d.Name; // maybe the user update the phone's name
						dbContext.Update(found);
						d = found;
						await dbContext.SaveChangesAsync();
					}
					else
					{
						dbContext.Add(d);
						await dbContext.SaveChangesAsync();
						//notify someone
						await emailSender.SendEmailAsync(user.Email, "New Device is connecting to your account",
							$"A new device {d.Model} - {d.Name} is connected to your account. If this is not yours please, go <a href='https://test.whereisfinder.com/Device/Edit/{d.DeviceId}'>here</a> to modify its access.");
					}
					
					//return guid
					userWebSocket.DeviceId = d.DeviceId;
					await SendDeviceInformation(userWebSocket, d.DeviceId);

					LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Device Id", d.DeviceId);
				}
				else if (message.Type == WSMessageType.DiscoverDevices)
				{
					List<string> d = JsonConvert.DeserializeObject<List<string>>(message.Text);
					List<Base> bases = await dbContext.Base.AsNoTracking().Where(b => b.UserId == UserId && b.LastStatus.ConnectedTo == userWebSocket.DeviceId).Include(b => b.LastStatus).ToListAsync();
					foreach (Base b in bases) {
						if (!d.Contains(b.HWId)) {
							BaseStatus lastStatus = dbContext.BaseStatus.FirstOrDefault(f => f.BaseId == b.BaseId && f.IsActive == true);
							if (lastStatus != null)
							{
								lastStatus.IsActive = false;
								dbContext.Update(lastStatus);
							}

							b.LastStatus = new BaseStatus
							{
								BaseId = b.BaseId,
								ConnectedTo = null,
								IsConnected = false,
								IsCharging = false,
								IsPlugged = false,
								Battery = 0,
								HasBattery = false,
								UserId = b.UserId,
								CreationDate = DateTime.Now,
								IsActive = true
							};
							dbContext.Add(b.LastStatus);

							b.LastStatusId = b.LastStatus.BaseStatusId;
							dbContext.Update(b);

							await dbContext.SaveChangesAsync();

							await BroadcastOthers(buffer, userWebSocket, wsFactory);
						}
					}
					
				}
				else if (message.Type == WSMessageType.LastGPS) {

				}
				else if (message.Type == WSMessageType.TrackingInfo) {

				}
			}
			catch (Exception e)
			{
				log.Info("Error:" + msg.Replace("\0", string.Empty));
				await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);				
			}
		}

		private async Task SendDeviceInformation(CustomWebSocket userWebSocket, string c)
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
			log.Info(serialisedMessage);
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
		public async Task BroadcastOthers1(CustomWebSocketMessage message, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
		{
			var strMsg = JsonConvert.SerializeObject(message);
			byte[] buffer = Encoding.ASCII.GetBytes(strMsg);
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
//edit base
		public async Task SendBaseCoreAsync(webGDPR.Infrastructure.CustomWebSockets.Messages.BaseCore c, string username, ICustomWebSocketFactory wsFactory)
		{
			string serialisedText = JsonConvert.SerializeObject(c);
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.BaseCore,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await BroadcastGroup(bytes, username, wsFactory);
		}
//delete base
		public async Task SendDeletedBaseAsync(byte baseNumber, string username, ICustomWebSocketFactory wsFactory)
		{
			string serialisedText = JsonConvert.SerializeObject(baseNumber);
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.DeletedBase,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await BroadcastGroup(bytes, username, wsFactory);
		}
//edit collar
		public async Task SendCollarCoreAsync(webGDPR.Infrastructure.CustomWebSockets.Messages.CollarCore collar, string username, ICustomWebSocketFactory wsFactory)
		{
			string serialisedText = JsonConvert.SerializeObject(collar);
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.CollarCore,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await BroadcastGroup(bytes, username, wsFactory);
		}
//delete collar
		public async Task SendDeletedCollarAsync(byte collarNumber, string username, ICustomWebSocketFactory wsFactory)
		{
			string serialisedText = JsonConvert.SerializeObject(collarNumber);
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.DeletedCollar,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await BroadcastGroup(bytes, username, wsFactory);
		}
//add base
		public async Task SendBaseAsync(webGDPR.Infrastructure.CustomWebSockets.Messages.Base b, string username, ICustomWebSocketFactory wsFactory)
		{
			string serialisedText = JsonConvert.SerializeObject(b);
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.Base,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await BroadcastGroup(bytes, username, wsFactory);
		}
//add collar
		public async Task SendCollarAsync(webGDPR.Infrastructure.CustomWebSockets.Messages.Collar c, string username, ICustomWebSocketFactory wsFactory)
		{
			string serialisedText = JsonConvert.SerializeObject(c);
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.Collar,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await BroadcastGroup(bytes, username, wsFactory);
		}

		public async Task SendDeviceBannedMessage(CustomWebSocket userWebSocket)
		{
			//{"Text":"DeviceBanned","MessagDateTime":"2018-10-05T12:43:47.647797-04:00","IsIncoming":true,"UserId":"system","Type":14}
			string serialisedText = "DeviceBanned";
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.DeviceBanned,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
		}
	}
}
