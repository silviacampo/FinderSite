using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using webGDPR.Data;

namespace webGDPR.Infrastructure.CustomWebSockets
{
	public class CustomWebSocketManager
	{
		private readonly RequestDelegate _next;

		public CustomWebSocketManager(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler, SignInManager<ApplicationUser> signInManager, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender)
		{
			if (context.Request.Path == "/ws")
			{
				if (context.WebSockets.IsWebSocketRequest)
				{
					string username = context.Request.Query["u"];
					string password = context.Request.Query["p"];
					string deviceId = context.Request.Query["g"];
					if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
					{
						var result = await signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: true);
						if (result.Succeeded)
						{
							WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
							CustomWebSocket userWebSocket = new CustomWebSocket()
							{
								WebSocket = webSocket,
								Username = username,
								DeviceId = deviceId,
								Guid = Guid.NewGuid()
							};
							wsFactory.Add(userWebSocket);
							await wsmHandler.SendInitialMessages(userWebSocket, dbContext, mapper);
							await Listen(context, userWebSocket, wsFactory, wsmHandler,dbContext, mapper, emailSender);
						}
						//log sthing
					}
					//log sthing
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
			WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			while (!result.CloseStatus.HasValue)
			{
				await wsmHandler.HandleMessage(result, buffer, userWebSocket, wsFactory, dbContext, mapper, emailSender);

				buffer = new byte[1024 * 4];
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}
			wsFactory.Remove(userWebSocket.Guid);
			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}
	}
}
