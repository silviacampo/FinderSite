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
using Microsoft.Extensions.Logging;
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
		static ILogger<CustomWebSocketManager> _logger;

		public CustomWebSocketManager(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler, SignInManager<ApplicationUser> signInManager, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender, ILogger<CustomWebSocketManager> logger)
		{
			if (_wsFactory == null)
			{
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

			if (_logger == null)
			{
				_logger = logger;
			}

			if (context.Request.Path == "/ws")
			{
				if (context.WebSockets.IsWebSocketRequest)
				{
					string username = context.Request.Query["u"];
					string deviceId = context.Request.Query["g"];
					deviceId = deviceId?.Replace("\"", "");

					WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

					User user = dbContext.User.FirstOrDefault(u => u.Name == username);

					Device found = dbContext.Device.AsNoTracking().FirstOrDefault(b => b.UserId == user.UserID && b.DeviceId == deviceId);

					CustomWebSocket userWebSocket = new CustomWebSocket()
					{
						WebSocket = webSocket,
						Username = username,
						DeviceId = (found == null) ? string.Empty : deviceId,
						Guid = Guid.NewGuid(),
						CreationDate = DateTime.Now,
						IP = context.Connection.RemoteIpAddress.ToString(),
						CredentialsChecked = false
					};
					wsFactory.Add(userWebSocket);

					wsmHandler.LogDeviceActivity(dbContext, deviceId, "WebSocket Add", JsonConvert.SerializeObject(userWebSocket));

					await Listen(context, userWebSocket, wsFactory, wsmHandler, dbContext, mapper, emailSender, signInManager);
				}
				else
				{
					context.Response.StatusCode = 400;
				}
			}
			await _next(context);
		}

		private async Task Listen(HttpContext context, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
		{
			WebSocket webSocket = userWebSocket.WebSocket;
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result;
			try
			{
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				while (!result.CloseStatus.HasValue)
				{
					await wsmHandler.HandleMessage(result, buffer, userWebSocket, wsFactory, dbContext, mapper, emailSender, signInManager);

					buffer = new byte[1024 * 4];
					result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				}
				try
				{
					await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
				}
				catch (Exception e)
				{
					_logger.LogError("CustomWebSocketManager - Listen: " + e.Message);
				}

				
				wsmHandler.LogDeviceActivity(dbContext, userWebSocket.DeviceId, "WebSocket Remove", JsonConvert.SerializeObject(userWebSocket));
				wsFactory.Remove(userWebSocket.Guid);
			}
			catch (WebSocketException e)
			{
				try
				{
					await webSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
				}
				catch (Exception ex)
				{
					_logger.LogError("CustomWebSocketManager - Listen: " + ex.Message);
				}
				wsmHandler.LogDeviceActivity(dbContext, userWebSocket.DeviceId, "WebSocket Remove", JsonConvert.SerializeObject(userWebSocket));
				wsFactory.Remove(userWebSocket.Guid);
			}

		}

		public static void CloseAll()
		{
			foreach (var userWebSocket in _wsFactory.All())
			{
				Task.Run(() =>
				{
					try
					{
						if (userWebSocket.WebSocket.State == WebSocketState.Open) {
							try
							{
								userWebSocket.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
							}
							catch (Exception e)
							{
								_logger.LogError("CustomWebSocketManager - CloseAll: " + e.Message);
							}
						}						
						_wsmHandler.LogDeviceActivity(_dbContext, userWebSocket.DeviceId, "WebSocket Remove - Server shutdown", JsonConvert.SerializeObject(userWebSocket));
					}
					catch (Exception ex)
					{
						_logger.LogError("CustomWebSocketManager - CloseAll: " + ex.Message);
					}
				}
				);
			}
		}

	}
}
