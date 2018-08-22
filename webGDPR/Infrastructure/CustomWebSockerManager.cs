using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AgendaSignalR.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using webGDPR.Data;

namespace webGDPR.Infrastructure
{
	public class CustomWebSocketManager
	{
		private readonly RequestDelegate _next;

		public CustomWebSocketManager(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler, SignInManager<ApplicationUser> signInManager)
		{
			if (context.Request.Path == "/ws")
			{
				if (context.WebSockets.IsWebSocketRequest)
				{
					string username = context.Request.Query["u"];
					string password = context.Request.Query["p"];
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
								Guid = Guid.NewGuid()
							};
							wsFactory.Add(userWebSocket);
							await wsmHandler.SendInitialMessages(userWebSocket);
							await Listen(context, userWebSocket, wsFactory, wsmHandler);
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

		private async Task Listen(HttpContext context, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler)
		{
			WebSocket webSocket = userWebSocket.WebSocket;
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			while (!result.CloseStatus.HasValue)
			{
				await wsmHandler.HandleMessage(result, buffer, userWebSocket, wsFactory);

				buffer = new byte[1024 * 4];
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}
			wsFactory.Remove(userWebSocket.Guid);
			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}
	}
}
