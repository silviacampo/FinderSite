using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity.UI.Services;
using webGDPR.Data;

namespace AgendaSignalR.Infrastructure
{
    public interface ICustomWebSocketMessageHandler
    {
		Task SendInitialMessages(CustomWebSocket userWebSocket, ApplicationDbContext dbContext, IMapper mapper);
		Task HandleMessage(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender);
		Task BroadcastOthers(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
		Task BroadcastGroup(byte[] buffer, string username, ICustomWebSocketFactory wsFactory);
		Task BroadcastAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
		Task BroadcastBinaryAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
		Task SendBaseAsync(Base @base, string username, ICustomWebSocketFactory wsFactory);
		Task SendDeletedBaseAsync(byte baseNumber, string username, ICustomWebSocketFactory wsFactory);
		Task SendCollarAsync(Collar collar, string username, ICustomWebSocketFactory wsFactory);
		Task SendDeletedCollarAsync(byte collarNumber, string username, ICustomWebSocketFactory wsFactory);
	}
}
