using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AutoMapper;
using webGDPR.Data;

namespace AgendaSignalR.Infrastructure
{
    public interface ICustomWebSocketMessageHandler
    {
		Task SendInitialMessages(CustomWebSocket userWebSocket, ApplicationDbContext dbContext, IMapper mapper);
		Task HandleMessage(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext, IMapper mapper);
		Task BroadcastOthers(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
		Task BroadcastGroup(byte[] buffer, string username, ICustomWebSocketFactory wsFactory);
		Task BroadcastAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
		Task BroadcastBinaryAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
	}
}
