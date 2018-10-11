using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
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
								Guid = Guid.NewGuid(),
								CreationDate = DateTime.Now,
								IP = context.Connection.RemoteIpAddress.ToString()
							};
							wsFactory.Add(userWebSocket);
							dbContext.DeviceLog.Add(new Models.DeviceLog() { DeviceId = deviceId, CreationDate = DateTime.Now, Reason = "WebSocket Add", Message = JsonConvert.SerializeObject(userWebSocket) });
							dbContext.SaveChanges();
							if (dbContext.Device.FirstOrDefault(d => d.DeviceId == deviceId && d.Banned) != null)
							{
								await wsmHandler.SendDeviceBannedMessage(userWebSocket);
								wsFactory.Remove(userWebSocket.Guid);
								dbContext.DeviceLog.Add(new Models.DeviceLog() { DeviceId = deviceId, CreationDate = DateTime.Now, Reason = "WebSocket Remove", Message = JsonConvert.SerializeObject(userWebSocket) });
								dbContext.SaveChanges();
								await webSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
							}
							else
							{
								await wsmHandler.SendInitialMessages(userWebSocket, dbContext, mapper);
								await Listen(context, userWebSocket, wsFactory, wsmHandler, dbContext, mapper, emailSender);
							}
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

                /*
                 2018-09-30 19:48:10,393 [42] ERROR Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware - An unhandled exception has occurred while executing the request.
System.Net.WebSockets.WebSocketException (0x80004005): The remote party closed the WebSocket connection without completing the close handshake. ---> System.Net.WebSockets.WebSocketException (0x80004005): The remote party closed the WebSocket connection without completing the close handshake.
   at System.Net.WebSockets.ManagedWebSocket.ThrowIfEOFUnexpected(Boolean throwOnPrematureClosure)
   at System.Net.WebSockets.ManagedWebSocket.EnsureBufferContainsAsync(Int32 minimumRequiredBytes, Boolean throwOnPrematureClosure)
   at System.Net.WebSockets.ManagedWebSocket.ReceiveAsyncPrivate[TWebSocketReceiveResultGetter,TWebSocketReceiveResult](Memory`1 payloadBuffer, CancellationToken cancellationToken, TWebSocketReceiveResultGetter resultGetter)
   at System.Net.WebSockets.ManagedWebSocket.ReceiveAsyncPrivate[TWebSocketReceiveResultGetter,TWebSocketReceiveResult](Memory`1 payloadBuffer, CancellationToken cancellationToken, TWebSocketReceiveResultGetter resultGetter)
   at webGDPR.Infrastructure.CustomWebSockets.CustomWebSocketManager.Listen(HttpContext context, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender) in /home/silvia/Desktop/testwsgdpr/webGDPR/Infrastructure/CustomWebSockets/CustomWebSocketManager.cs:line 70
   at webGDPR.Infrastructure.CustomWebSockets.CustomWebSocketManager.Invoke(HttpContext context, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler, SignInManager`1 signInManager, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender) in /home/silvia/Desktop/testwsgdpr/webGDPR/Infrastructure/CustomWebSockets/CustomWebSocketManager.cs:line 46
   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware.Invoke(HttpContext context)
                 */
                try {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                catch (WebSocketException e) {
                    wsFactory.Remove(userWebSocket.Guid);
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
                
			}
			
			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

			dbContext.DeviceLog.Add(new Models.DeviceLog() { DeviceId = userWebSocket.DeviceId, CreationDate = DateTime.Now, Reason = "WebSocket Remove", Message = JsonConvert.SerializeObject(userWebSocket) });
			dbContext.SaveChanges();

			wsFactory.Remove(userWebSocket.Guid);
		}

		private void LogDeviceActivity(ApplicationDbContext dbContext, string DeviceId, string Reason, string Message)
		{
			if (dbContext.Device.Find(DeviceId).IsLogging)
			{
				dbContext.DeviceLog.Add(new Models.DeviceLog() { DeviceId = DeviceId, CreationDate = DateTime.Now, Reason = Reason, Message = Message });
				dbContext.SaveChanges();
			}
		}
	}
}
