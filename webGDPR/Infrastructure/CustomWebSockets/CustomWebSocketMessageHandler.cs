using System;
using System.Collections.Generic;
using System.IO;
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

				#region Bases
				List<Base> bases = await dbContext.Base.AsNoTracking().Where(b => b.UserId == UserId && !b.Deleted).Include(b => b.LastStatus).ThenInclude(c => c.DeviceConnectedTo).ToListAsync();

				List<Messages.Base> msgBases = new List<Messages.Base>();
				foreach (var b in bases)
				{
					Messages.Base mb = mapper.Map<Messages.Base>(new Tuple<Base, BaseStatus>(b, b.LastStatus));
					if (!mb.IsConnected)
					{
						mb.ConnectedToName = null;
					}
					msgBases.Add(mb);
				}

				var msg = new CustomWebSocketMessage
				{
					MessagDateTime = DateTime.Now,
					Type = WSMessageType.Bases,
					Text = JsonConvert.SerializeObject(msgBases),
					UserId = CustomWebSocketMessage.SystemUserId
				};

				string serialisedBasesMessage = JsonConvert.SerializeObject(msg);
				log.Info("Initial Bases: " + serialisedBasesMessage);
				byte[] bytes = Encoding.ASCII.GetBytes(serialisedBasesMessage);
				await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);

				LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Initial Bases", serialisedBasesMessage);
				#endregion

				#region Collars
				List<Collar> collars = await dbContext.Collar.AsNoTracking().Where(b => b.UserId == UserId && !b.Deleted).Include(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).ToListAsync();

				List<Messages.Collar> msgCollars = new List<Messages.Collar>();
				foreach (var b in collars)
				{
					Messages.Collar mb = mapper.Map<Messages.Collar>(new Tuple<Collar, CollarStatus>(b, b.LastStatus));
					//go and pick up the name of the pet if pet assoc. to collar 
					PetCollar petCollar = await dbContext.PetCollar.FirstOrDefaultAsync(p => p.CollarId == b.CollarId && p.IsActive);
					if (petCollar != null)
					{
						Pet p = dbContext.Pet.Include(m => m.LastMode).FirstOrDefault(pet => pet.PetId == petCollar.PetId && !pet.Deleted);
						mb.Name = p.Name;
						mb.IsLost = false;
						if (p.LastMode != null && p.LastMode.Type == ConfigModeTypes.Emergency && p.LastMode.IsActive)
						{
							mb.IsLost = true;
						}
					}
					msgCollars.Add(mb);
				}

				var msg2 = new CustomWebSocketMessage
				{
					MessagDateTime = DateTime.Now,
					Type = WSMessageType.Collars,
					Text = JsonConvert.SerializeObject(msgCollars),
					UserId = CustomWebSocketMessage.SystemUserId
				};

				string serialisedCollarsMessage = JsonConvert.SerializeObject(msg2);
				log.Info("Initial Collars: " + serialisedCollarsMessage);
				byte[] bytes2 = Encoding.ASCII.GetBytes(serialisedCollarsMessage);
				await webSocket.SendAsync(new ArraySegment<byte>(bytes2, 0, bytes2.Length), WebSocketMessageType.Text, true, CancellationToken.None);

				LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Initial Collars", serialisedCollarsMessage);
				#endregion

				#region LastFiles
				List<string> msgFiles = new List<string>();
				DirectoryInfo dir = new DirectoryInfo(CustomPaths.GetDownloadPath());
				foreach (DirectoryInfo di in dir.GetDirectories())
				{
					FileInfo fi = di.GetFiles().OrderByDescending(f => f.CreationTime).FirstOrDefault();
					if (fi != null)
					{
						string filepath = CustomPaths.GetDownloadURL(di.Name, fi.Name);
						msgFiles.Add(filepath);
					}
				}
				var filesmsg = new CustomWebSocketMessage
				{
					MessagDateTime = DateTime.Now,
					Type = WSMessageType.DownloadFile,
					Text = JsonConvert.SerializeObject(msgFiles),
					UserId = CustomWebSocketMessage.SystemUserId
				};

				string serialisedFilesMessage = JsonConvert.SerializeObject(filesmsg);
				log.Info("Last Files: " + serialisedFilesMessage);
				byte[] filesbytes = Encoding.ASCII.GetBytes(serialisedFilesMessage);
				await webSocket.SendAsync(new ArraySegment<byte>(filesbytes, 0, filesbytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);

				LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Last Files", serialisedFilesMessage);
				#endregion
			}
			catch (Exception e)
			{
				log.Error("CustomWebSocketMessageHandler - SendInitialMessages Error: " + e.Message);
			}
		}

		public void LogDeviceActivity(ApplicationDbContext dbContext, string DeviceId, string Reason, string Message)
		{
			if (DeviceId != null)
			{
				DeviceId = DeviceId.Replace("\"", "");

				if (DeviceId == string.Empty || (dbContext.Device.FirstOrDefault(d => d.DeviceId == DeviceId) != null && dbContext.Device.FirstOrDefault(d => d.DeviceId == DeviceId).IsLogging))
				{
					dbContext.DeviceLog.Add(new Models.DeviceLog() { DeviceId = DeviceId, CreationDate = DateTime.Now, Reason = Reason, Message = Message });

					try
					{
						dbContext.SaveChanges();
					}
					catch (Exception e)
					{
						log.Error("CustomWebSocketMessageHandler - LogDeviceActivity Error: " + e.Message);
					}
				}
			}
		}

		public async Task HandleMessage(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
		{
			string msg = Encoding.ASCII.GetString(buffer).Trim('\0');
			try
			{
				#region Messages Samples
				//{"m":"As!123456","d":1541440447,"u":"SilviaCampo","t":20}

				//{"m":"{\"bn\":1,\"co\":1,\"cot\":\"\",\"pl\":0,\"ch\":0,\"bt\":0,\"hbt\":0,\"r\":0}","d":1541440447,"u":"SilviaCampo","t":12}

				//{"m":"{\"bn\":1,\"co\":0,\"cot\":\"\",\"pl\":0,\"ch\":0,\"bt\":0,\"hbt\":0,\"r\":0}","d":1542291224,"u":"SilviaCampo","t":12}

				//{ "m":"{\"cn\":1,\"bn\":1,\"co\":0,\"cot\":\"\",\"gps\":1,\"b\":60,\"r\":40}","d":"2018-08-23T13:33:00.5057737-04:00","u":"SilviaCampo","t":13}

				//{ "m":"{\"CollarNumber\":\"1\",\"BaseNumber\":\"1\",\"IsConnected\":true,\"IsGPSConnected\":true,\"Battery\":60,\"Radio\":40}","MessagDateTime":"2018-08-23T13:33:00.5057737-04:00","IsIncoming":true,"UserId":"SilviaCampo","Type":13}

				//{ "m":"{\"DeviceId\":\"68b73ced-1659-483c-929e-274a97706405\",\"Name\":\"Silvia's Phone\",\"Model\":\"Nexus 5\",\"Manufacturer\":\"LG\",\"Type\":\"Phone\",\"Platform\":\"Android\",\"UserId\":\"bee7b8af-c902-4771-89f8-969a3318cbdb\"}","MessagDateTime":"2018-08-23T13:33:00.5057737-04:00","UserId":"scampo@test.com","Type":5}

				//{"m":"{\"t\":\"Phone\",\"p\":\"Android\",\"n\":\"Nexus 5\",\"mo\":\"Nexus 5\",\"ma\":\"LGE\",\"os\":\"6.0.1\"}","d":1541440447,"u":"SilviaCampo","t":5}

				//{"m":"[\"Bluegiga CR Demo\"]","d":1541440447,"u":"SilviaCampo","t":14}

				//{"m":"{\"cn\":1,\"la\":45.513025,\"lo\":-73.720863,\"d\":3343659132}","d":1541440470,"u":"SilviaCampo","t":7}

				//{"m":"{\"cn\":1,\"co\":1,\"cot\":\"Me\",\"gps\":0,\"bt\":0,\"r\":0}","d":1542026359,"u":"SilviaCampo","t":13}
				#endregion

				var message = JsonConvert.DeserializeObject<CustomWebSocketMessage>(msg);
				LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Message from device", msg);
				log.Info(userWebSocket.DeviceId + " - " + msg);

				User user = dbContext.User.FirstOrDefault(u => u.Name == userWebSocket.Username);
				string UserId = user.UserID;

				if (message.Type == WSMessageType.Login)
				{
					if (!string.IsNullOrEmpty(userWebSocket.Username) && !string.IsNullOrEmpty(message.Text))
					{
						var foundUser = await signInManager.UserManager.FindByNameAsync(userWebSocket.Username);
						var validCredentials = signInManager.UserManager.CheckPasswordAsync(foundUser, message.Text);
						if (validCredentials.Result)
						{
							userWebSocket.CredentialsChecked = true;							

							//Device is banned or User has missing subscription
							Device found = dbContext.Device.AsNoTracking().FirstOrDefault(b => b.UserId == user.UserID && b.DeviceId == userWebSocket.DeviceId);
							if (found != null && found.Banned)
							{
								await SendDeviceBannedMessage(userWebSocket, true);
								await RemoveDeviceBanned(userWebSocket, wsFactory, dbContext);
							}
							else
							{
								await SendLoginAsync(userWebSocket, dbContext);
								if (user.MissingSubscription)
								{
									await SendMissingSubscriptionMessageAsync(true, userWebSocket.Username, wsFactory);
								}
								await SendInitialMessages(userWebSocket, dbContext, mapper);
							}							
						}
						else
						{
							LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Attempt Sign in fail", JsonConvert.SerializeObject(validCredentials) + " - " + message.Text);
							wsFactory.Remove(userWebSocket.Guid);
							LogDeviceActivity(dbContext, userWebSocket.DeviceId, "WebSocket Remove - Wrong Login Credentials", JsonConvert.SerializeObject(userWebSocket));
							try
							{
								await userWebSocket.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
							}
							catch (Exception e)
							{
								log.Error("CustomWebSocketMessageHandler - HandleMessage: Wrong Login Credentials - " + e.Message);
							}
						}
					}
					else
					{
						LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Attempt without username/password", JsonConvert.SerializeObject(userWebSocket) + " - " + message.Text);
						wsFactory.Remove(userWebSocket.Guid);
						LogDeviceActivity(dbContext, userWebSocket.DeviceId, "WebSocket Remove - Missing Login Credentials", JsonConvert.SerializeObject(userWebSocket));
						try
						{
							await userWebSocket.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
						}
						catch (Exception e)
						{
							log.Error("CustomWebSocketMessageHandler - HandleMessage: Missing Login Credentials - " + e.Message);
						}
					}
				}
				else if (userWebSocket.CredentialsChecked)
				{
					if (message.Type == WSMessageType.Packet)
					{
						Messages.Packet p = JsonConvert.DeserializeObject<Messages.Packet>(message.Text);
						Packet.Packet packetContent = Packet.Packet.DeserializeAndValidate(p.Data);
						List<Tuple<string, string>> values = packetContent.GetValues();
						string @base = p.HWId;
						byte collar = packetContent.GetCollar();
						Packet.PacketType command = packetContent.Header.Command;
						LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Message from device", $"BaseHWId: {@base}, CollarNumber: {collar}, Command: {command.ToString()}, Content: {packetContent.ToString()}");
						log.Info(userWebSocket.DeviceId + " - " + $"BaseHWId: {@base}, CollarNumber: {collar}, Command: {command.ToString()}, Content: {packetContent.ToString()}");

						//switch (command) {
						//	case Packet.PacketType.BaseStatus:
						//		Base ba2 = dbContext.Base.FirstOrDefault(f => f.HWId == p.HWId && f.UserId == UserId); //f.BaseNumber == bs.BaseNumber &&
						//		byte chargingStatus = byte.Parse(values.First(v => v.Item1 == "ChargingStatus").Item2); //1= charging , 2=no battery
						//		List<RadioStrength> strengths = collars.SelectMany(c => c.RadioStrengths).Where(s => s.BaseNumber == ba2.BaseNumber).ToList();
						//		Messages.BaseStatus bs = new Messages.BaseStatus
						//		{
						//			BaseId = ba2.BaseId,
						//			BaseNumber = ba2.BaseNumber,
						//			IsConnected = true,
						//			ConnectedToName = "Me",
						//			//TODO
						//			IsPlugged = int.Parse(values.First(v => v.Item1 == "UsbVoltage").Item2) != 0,
						//			IsCharging = chargingStatus % 2 == 1,
						//			HasBattery = (chargingStatus % 4) / 2 == 1,
						//		Radio = strengths.Sum(s => s.BaseRadio) / strengths.Count(),  // int.Parse(values.First(v => v.Item1 == "LoraRxPower").Item2);
						//		Battery = int.Parse(values.First(v => v.Item1 == "BatVoltage").Item2)
						//};

							
						//		break;
						//	case (byte)Packet.PacketType.CollarStatus:
						//		string collarn = values.First(v => v.Item1 == "CollarNumber").Item2;
						//		var collarnumber = byte.Parse(collarn);
						//		var basenumber = bases.FirstOrDefault(a => a.HWId == HWId).BaseNumber;
						//		var co = collars.Find(c => c.CollarNumber == collarnumber);
						//		co.IsConnected = true;
						//		//TODO: which one locks or seconds
						//		co.IsGPSConnected = byte.Parse(values.First(v => v.Item1 == "GpsLocks").Item2) != 0;

						//		var radioStrength = co.RadioStrengths.FirstOrDefault(r => r.BaseNumber == basenumber);
						//		if (radioStrength == null)
						//		{
						//			co.RadioStrengths.Add(new RadioStrength() { BaseNumber = basenumber, BaseRadio = int.Parse(values.First(v => v.Item1 == "LoraBasePower").Item2), CollarRadio = int.Parse(values.First(v => v.Item1 == "LoraCollarPower").Item2) });
						//		}
						//		else
						//		{
						//			radioStrength.CollarRadio = int.Parse(values.First(v => v.Item1 == "LoraCollarPower").Item2);
						//			radioStrength.BaseRadio = int.Parse(values.First(v => v.Item1 == "LoraBasePower").Item2);
						//		}

						//		co.Radio = co.RadioStrengths.Sum(r => r.BaseRadio) / co.RadioStrengths.Count();

						//		co.Battery = int.Parse(values.First(v => v.Item1 == "BatVoltage").Item2);
						//		CollarStatus cs = new CollarStatus
						//		{
						//			CollarNumber = co.CollarNumber,
						//			BaseNumber = bases.FirstOrDefault(a => a.HWId == HWId).BaseNumber,
						//			IsConnected = true,
						//			ConnectedTo = "Me",
						//			IsGPSConnected = co.IsGPSConnected,
						//			Battery = co.Battery,
						//			Radio = co.Radio
						//		};

						//		break;
						//	case (byte)Packet.PacketType.GpsPoint:
						//		GPSPoint point = new GPSPoint
						//		{
						//			CollarNumber = byte.Parse(values.First(v => v.Item1 == "CollarNumber").Item2),
						//			CreatedDate = UnixTimestampToDateTime(values.First(v => v.Item1 == "Epoch").Item2),
						//			Latitude = double.Parse(values.First(v => v.Item1 == "Latitude").Item2) / 10000000f,
						//			Longitude = double.Parse(values.First(v => v.Item1 == "Longitude").Item2) / 10000000f
						//		};

						//		break;
						//	default:
						//		break;
						//}
					}
					else if (message.Type == WSMessageType.BaseStatus)
					{
						#region BaseStatusMessage
						Messages.BaseStatus bs = JsonConvert.DeserializeObject<Messages.BaseStatus>(message.Text);
						Base b = dbContext.Base.FirstOrDefault(f => f.BaseNumber == bs.BaseNumber && f.UserId == UserId);
						if (b != null)
						{
							BaseStatus lastStatus = dbContext.BaseStatus.FirstOrDefault(f => f.BaseId == b.BaseId && f.IsActive == true);
							if (lastStatus != null)
							{
								lastStatus.IsActive = false;
								dbContext.Update(lastStatus);
							}

							var newStatus = new BaseStatus
							{
								BaseId = b.BaseId,
								IsConnected = bs.IsConnected,
								ConnectedTo = userWebSocket.DeviceId,
								IsPlugged = bs.IsPlugged,
								IsCharging = bs.IsCharging,								
								Battery = bs.Battery,
								HasBattery = bs.HasBattery,
								Radio = bs.Radio,
								UserId = b.UserId,
								CreationDate = message.MessagDateTime, //TODO: or now?
								IsActive = true
							};
							dbContext.Add(newStatus);

							b.LastStatusId = newStatus.BaseStatusId;
							dbContext.Update(b);

							await dbContext.SaveChangesAsync();

							if (bs.IsConnected)
							{
								bs.ConnectedToName = dbContext.Device.AsNoTracking().FirstOrDefault(d => d.DeviceId == userWebSocket.DeviceId).GetName;
							}
							message.Text = JsonConvert.SerializeObject(bs);
							await BroadcastOthers1(message, userWebSocket, wsFactory);
						}
						else
						{
							//Todo: notify device basenumber doesn't exist
							await SendDeletedBaseAsync(bs.BaseNumber, userWebSocket);
						}
						#endregion
					}
					else if (message.Type == WSMessageType.CollarStatus)
					{
						#region CollarStatusMessage
						Messages.CollarStatus cs = JsonConvert.DeserializeObject<Messages.CollarStatus>(message.Text);
						Collar collar = dbContext.Collar.FirstOrDefault(f => f.CollarNumber == cs.CollarNumber && f.UserId == UserId);
						Base b = dbContext.Base.FirstOrDefault(f => f.BaseNumber == cs.BaseNumber && f.UserId == UserId);
						if (collar != null && b != null)
						{
							CollarStatus lastStatus = dbContext.CollarStatus.FirstOrDefault(f => f.CollarId == collar.CollarId && f.IsActive == true);
							if (lastStatus != null)
							{
								lastStatus.IsActive = false;
								dbContext.Update(lastStatus);
							}

							var newStatus = new CollarStatus
							{
								CollarId = collar.CollarId,
								IsConnected = cs.IsConnected,
								ConnectedTo = b.BaseId,
								IsGPSConnected = cs.IsGPSConnected,
								Battery = cs.Battery,
								Radio = cs.Radio,								
								UserId = collar.UserId,
								CreationDate = message.MessagDateTime, //TODO: or now?
								IsActive = true 
							};
							dbContext.Add(newStatus);

							collar.LastStatusId = newStatus.CollarStatusId;
							dbContext.Update(collar);

							await dbContext.SaveChangesAsync();
							await BroadcastOthers(buffer, userWebSocket, wsFactory);
						}
						else
						{
							//Todo: notify device basenumber or collarnumber doesn't exist
							if (b == null)
								await SendDeletedBaseAsync(cs.BaseNumber, userWebSocket);
							if (collar == null)
								await SendDeletedCollarAsync(cs.BaseNumber, userWebSocket);
						}
						#endregion
					}
					else if (message.Type == WSMessageType.DeviceInfo)
					{
						#region DeviceInfoMessage
						Messages.Device d = JsonConvert.DeserializeObject<Messages.Device>(message.Text);
						d.UserId = UserId;						
						Device device = dbContext.Device.FirstOrDefault(c => c.UserId == d.UserId && (c.Type == d.Type || (string.IsNullOrEmpty(c.Type) && string.IsNullOrEmpty(d.Type))) && (c.Platform == d.Platform || (string.IsNullOrEmpty(c.Platform) && string.IsNullOrEmpty(d.Platform))) && (c.Manufacturer == d.Manufacturer || (string.IsNullOrEmpty(c.Manufacturer) && string.IsNullOrEmpty(d.Manufacturer))) && (c.Model == d.Model || (string.IsNullOrEmpty(c.Model) && string.IsNullOrEmpty(d.Model))) && (c.Name == d.Name || (string.IsNullOrEmpty(c.Name)&& string.IsNullOrEmpty(d.Name))));
						if (device != null)
						{
							device.OSVersion = d.OSVersion;
							dbContext.Update(device);
							await dbContext.SaveChangesAsync();
						}
						else
						{
							log.Debug($" New device (diff user, type, platform, manufacturer, model or name): { message.Text}");
							device = new Device
							{
								Manufacturer = d.Manufacturer,
								Model = d.Model,
								Name = d.Name,
								OSVersion = d.OSVersion,
								Platform = d.Platform,
								Type = d.Type,
								UserId = d.UserId,
								IsLogging = true
							};
							dbContext.Add(device);
							await dbContext.SaveChangesAsync();
							//notify the user
							await emailSender.SendEmailAsync(user.Email, "New Device is connecting to your account",
								$"A new device {d.Model} - {d.Name} is connected to your account. If this is not yours please, go <a href='https://test.whereisfinder.com/Device/Edit/{d.DeviceId}'>here</a> to modify its access.");
						}

						//return guid
						userWebSocket.DeviceId = device.DeviceId;
						await SendDeviceInformation(userWebSocket, device.DeviceId, dbContext);

						LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Device Id", device.DeviceId);
						#endregion
					}
					else if (message.Type == WSMessageType.DiscoverBases)
					{
						#region DiscoveredBasesMessage
						List<string> d = JsonConvert.DeserializeObject<List<string>>(message.Text);
						List<Base> bases = await dbContext.Base.AsNoTracking().Where(b => b.UserId == UserId && !b.Deleted && b.LastStatus.ConnectedTo == userWebSocket.DeviceId).Include(b => b.LastStatus).ToListAsync();
						foreach (Base b in bases)
						{
							if (d.Count == 0 || !d.Contains(b.HWId))
							{
								BaseStatus lastStatus = dbContext.BaseStatus.FirstOrDefault(f => f.BaseId == b.BaseId && f.IsActive == true);
								if (lastStatus != null)
								{
									lastStatus.IsActive = false;
									dbContext.Update(lastStatus);
								}

								var newStatus = new BaseStatus
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
								dbContext.Add(newStatus);
								b.LastStatusId = newStatus.BaseStatusId;
								dbContext.Update(b);

								await dbContext.SaveChangesAsync();

								await BroadcastOthers(buffer, userWebSocket, wsFactory);
							}
						}
						#endregion
					}
					else if (message.Type == WSMessageType.LastGPS)
					{

					}
					else if (message.Type == WSMessageType.TrackingInfo)
					{
						#region TrackingInfoMessage
						Messages.GPSPoint p = JsonConvert.DeserializeObject<Messages.GPSPoint>(message.Text);
						Collar collar = dbContext.Collar.FirstOrDefault(f => f.CollarNumber == p.CollarNumber && f.UserId == UserId);
						string petId = dbContext.PetCollar.FirstOrDefault(c => c.CollarId == collar.CollarId && c.IsActive).PetId;

						PetTrackingInfo pt = new PetTrackingInfo
						{
							PetId = petId,
							CollarId = collar.CollarId,
							Latitude = p.Latitude,
							Longitude = p.Longitude,
							UserId = collar.UserId,
							CreationDate = p.CreatedDate,
							IsActive = true
						};
						dbContext.Add(pt);

						Pet pet = dbContext.Pet.FirstOrDefault(c => c.PetId == petId);
						pet.LastTrackingInfoId = pt.PetTrackingInfoId;
						dbContext.Update(pet);

						await dbContext.SaveChangesAsync();
						#endregion
					}
				}
				else
				{
					wsFactory.Remove(userWebSocket.Guid);
					LogDeviceActivity(dbContext, userWebSocket.DeviceId, "WebSocket Remove - Missing Login Message", JsonConvert.SerializeObject(userWebSocket));
					try
					{
						await userWebSocket.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
					}
					catch (Exception e)
					{
						log.Error("CustomWebSocketMessageHandler - HandleMessage: Missing Login Message - " + e.Message);
					}
				}
			}
			catch (Exception e)
			{
				log.Error("CustomWebSocketMessageHandler - HandleMessage Error:" + msg + " - " + e.Message);
				LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Message from device rejected", msg + " - " + e.Message);
				//TODO: remove echo the wrong message
				await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
			}
		}

		private async Task SendLoginAsync(CustomWebSocket userWebSocket, ApplicationDbContext dbContext)
		{
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.Login,
				Text = string.Empty,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
			LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Send Login OK", serialisedMessage);
		}

		private async Task SendDeviceInformation(CustomWebSocket userWebSocket, string c, ApplicationDbContext dbContext)
		{
			string serialisedText = JsonConvert.SerializeObject(c);
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.DeviceInfo,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
			LogDeviceActivity(dbContext, userWebSocket.DeviceId, "Send Device Information GUID", serialisedMessage);

		}

		public async Task SendDownloadFile(List<string> msgFiles, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext)
		{
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.DownloadFile,
				Text = JsonConvert.SerializeObject(msgFiles),
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			await BroadcastAll(serialisedMessage, wsFactory, dbContext);
		}

		//any change coming from one device
		public async Task BroadcastOthers(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
		{
			var others = wsFactory.Others(userWebSocket);
			foreach (var uws in others)
			{
				if (uws.CredentialsChecked)
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
				if (uws.CredentialsChecked)
					await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
				else
					log.Debug($"Missing Credentials: {uws.DeviceId} - Message: {strMsg} ");
			}
		}

		//any change in the user acount started from the website
		public async Task BroadcastGroup(byte[] buffer, string username, ICustomWebSocketFactory wsFactory)
		{
			var all = wsFactory.Group(username);
			foreach (var uws in all)
			{
				if (uws.CredentialsChecked)
					await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}

		public async Task BroadcastAll(string message, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext)
		{
			byte[] buffer = Encoding.ASCII.GetBytes(message);
			var all = wsFactory.All();
			foreach (var uws in all)
			{
				if (uws.CredentialsChecked)
				{
					await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
					if (dbContext != null)
						LogDeviceActivity(dbContext, uws.DeviceId, "Message from broadcastAll", message);
				}
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
		public async Task SendDeletedBaseAsync(byte baseNumber, CustomWebSocket userWebSocket)
		{
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.DeletedBase,
				Text = baseNumber.ToString(),
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info("CustomWebSocketMessageHandler - SendDeletedBaseAsync " + serialisedMessage);
			byte[] buffer = Encoding.ASCII.GetBytes(serialisedMessage);
			await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
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
		public async Task SendDeletedCollarAsync(byte collarNumber, CustomWebSocket userWebSocket)
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
			log.Info("CustomWebSocketMessageHandler - SendDeletedCollarAsync " + serialisedMessage);
			byte[] buffer = Encoding.ASCII.GetBytes(serialisedMessage);
			await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
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

		public async Task SendDeviceBannedMessage(CustomWebSocket userWebSocket, bool value)
		{
			//{"Text":"True","MessagDateTime":"2018-10-05T12:43:47.647797-04:00","IsIncoming":true,"UserId":"system","Type":14}
			string serialisedText = value.ToString();
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

		public async Task RemoveDeviceBanned(CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext) {
			wsFactory.Remove(userWebSocket.Guid);
			LogDeviceActivity(dbContext, userWebSocket.DeviceId, "WebSocket Remove - Device Banned", JsonConvert.SerializeObject(userWebSocket));
			try
			{
				await userWebSocket.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
			}
			catch (Exception e)
			{
				log.Error("CustomWebSocketMessageHandler - RemoveDeviceBanned - " + e.Message);
			}
		}

		public async Task SendSwitchModeAsync(byte collarNumber, ConfigModeTypes mode, string username, ICustomWebSocketFactory wsFactory, byte[] customConfig = null, string DeviceId = null, string BaseId = null)
		{
			Messages.ConfigMode cm = Packet.PacketHelper.BuildMode(collarNumber, mode, customConfig);
			string serialisedText = JsonConvert.SerializeObject(cm);
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.SwitchMode,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);

			if (DeviceId == "127")
			{
                await BroadcastGroup(bytes, username, wsFactory);                
			}
			else
			{
                CustomWebSocket ws = wsFactory.ClientByDeviceId(DeviceId);
                if (ws != null)
                await ws.WebSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
		}

		public async Task SendMissingSubscriptionMessageAsync(bool value, string username, ICustomWebSocketFactory wsFactory)
		{
			//{"Text":"True","MessagDateTime":"2018-10-05T12:43:47.647797-04:00","UserId":"system","Type":19}
			string serialisedText = value.ToString();
			var msg = new CustomWebSocketMessage
			{
				MessagDateTime = DateTime.Now,
				Type = WSMessageType.MissingSubscription,
				Text = serialisedText,
				UserId = CustomWebSocketMessage.SystemUserId
			};

			string serialisedMessage = JsonConvert.SerializeObject(msg);
			log.Info(serialisedMessage);
			byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
			await BroadcastGroup(bytes, username, wsFactory);
		}
	}
}
