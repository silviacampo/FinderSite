using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using webGDPR.Data;
using webGDPR.Models;

namespace webGDPR.Infrastructure.CustomWebSockets
{
	public class CustomWebSocketManager
	{
		private readonly RequestDelegate _next;
		static ICustomWebSocketFactory _wsFactory;
		static ICustomWebSocketMessageHandler _wsmHandler;
		static ApplicationDbContext _dbContext;

		public CustomWebSocketManager(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler, SignInManager<ApplicationUser> signInManager, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender)
		{
			if (_wsFactory == null) {
				_wsFactory = wsFactory;
			}

			if (_wsmHandler == null)
			{
				_wsmHandler = wsmHandler;
			}

			if (_dbContext == null)
			{
				_dbContext = dbContext;
			}

			if (context.Request.Path == "/ws")
			{
				if (context.WebSockets.IsWebSocketRequest)
				{
					string username = context.Request.Query["u"];
					string password = context.Request.Query["p"];
                    string deviceId = context.Request.Query["g"];
                    deviceId = deviceId?.Replace("\"", "");

					if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
					{
						var result = await signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: true);
						if (result.Succeeded)
						{
							WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

							string UserId = dbContext.User.FirstOrDefault(u => u.Name == username).UserID;

							Device found = dbContext.Device.AsNoTracking().FirstOrDefault(b => b.UserId == UserId && b.DeviceId == deviceId);

							CustomWebSocket userWebSocket = new CustomWebSocket()
							{
								WebSocket = webSocket,
								Username = username,
								DeviceId = (found == null) ? string.Empty : deviceId,
								Guid = Guid.NewGuid(),
								CreationDate = DateTime.Now,
								IP = context.Connection.RemoteIpAddress.ToString()
							};
							wsFactory.Add(userWebSocket);

							wsmHandler.LogDeviceActivity(dbContext, deviceId, "WebSocket Add", JsonConvert.SerializeObject(userWebSocket));
							//Device is banned or User has missing subscription
							if (dbContext.Device.FirstOrDefault(d => d.DeviceId == deviceId && d.Banned) != null 
								
								)
							{
								await wsmHandler.SendDeviceBannedMessage(userWebSocket);
								wsFactory.Remove(userWebSocket.Guid);

								wsmHandler.LogDeviceActivity(dbContext, deviceId, "WebSocket Remove", JsonConvert.SerializeObject(userWebSocket));
								await webSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
							}
							else
							{
								await wsmHandler.SendInitialMessages(userWebSocket, dbContext, mapper);
								await Listen(context, userWebSocket, wsFactory, wsmHandler, dbContext, mapper, emailSender);
							}
						}
						else {
							wsmHandler.LogDeviceActivity(dbContext, deviceId, "Attempt Sign in", JsonConvert.SerializeObject(result));
						}
					}
					else {
						wsmHandler.LogDeviceActivity(dbContext, deviceId, "Attempt without username/password", JsonConvert.SerializeObject(context.Request.QueryString));
					}
				}
				else
				{
					context.Response.StatusCode = 400;
				}
			}
			await _next(context);
		}

		private async Task Listen(HttpContext context, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender)
		{
            WebSocket webSocket = userWebSocket.WebSocket;
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;
            try {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    await wsmHandler.HandleMessage(result, buffer, userWebSocket, wsFactory, dbContext, mapper, emailSender);

                    buffer = new byte[1024 * 4];
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                wsmHandler.LogDeviceActivity(dbContext, userWebSocket.DeviceId, "WebSocket Remove", JsonConvert.SerializeObject(userWebSocket));
                wsFactory.Remove(userWebSocket.Guid);
            }
            catch (WebSocketException e)
            {
                //await webSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
                wsmHandler.LogDeviceActivity(dbContext, userWebSocket.DeviceId, "WebSocket Remove", JsonConvert.SerializeObject(userWebSocket));
                wsFactory.Remove(userWebSocket.Guid);
            }			

		}

		public static void CloseAll() {
			foreach (var userWebSocket in _wsFactory.All()) {
				_wsmHandler.LogDeviceActivity(_dbContext, userWebSocket.DeviceId, "WebSocket Remove - Server shutdown", JsonConvert.SerializeObject(userWebSocket));
				Task.Run(()=> userWebSocket.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None));
			}
		}

	}
}
