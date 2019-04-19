using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using webGDPR.Data;
using webGDPR.Infrastructure.CustomWebSockets.Messages;
using webGDPR.Models;

namespace webGDPR.Infrastructure.CustomWebSockets
{
    public interface ICustomWebSocketMessageHandler
    {
		Task SendInitialMessages(CustomWebSocket userWebSocket, ApplicationDbContext dbContext, IMapper mapper);
		Task HandleMessage(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext, IMapper mapper, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager);
		Task BroadcastOthers(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
		Task BroadcastGroup(byte[] buffer, string username, ICustomWebSocketFactory wsFactory);
		Task BroadcastBinaryAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
		Task SendBaseCoreAsync(webGDPR.Infrastructure.CustomWebSockets.Messages.BaseCore b, string username, ICustomWebSocketFactory wsFactory);
		Task SendDeletedBaseAsync(byte baseNumber, string username, ICustomWebSocketFactory wsFactory);
		Task SendCollarCoreAsync(webGDPR.Infrastructure.CustomWebSockets.Messages.CollarCore collar, string username, ICustomWebSocketFactory wsFactory);
		Task SendDeletedCollarAsync(byte collarNumber, string username, ICustomWebSocketFactory wsFactory);
		Task SendBaseAsync(webGDPR.Infrastructure.CustomWebSockets.Messages.Base b, string username, ICustomWebSocketFactory wsFactory);
		Task SendCollarAsync(webGDPR.Infrastructure.CustomWebSockets.Messages.Collar c, string username, ICustomWebSocketFactory wsFactory);
		Task SendDeviceBannedMessage(CustomWebSocket userWebSocket, bool value);
		Task SendMissingSubscriptionMessageAsync(bool value, string username, ICustomWebSocketFactory wsFactory);
		Task SendDownloadFile(List<string> msgFiles, ICustomWebSocketFactory wsFactory, ApplicationDbContext dbContext);
		void LogDeviceActivity(ApplicationDbContext dbContext, string DeviceId, string Reason, string Message);
		Task SendSwitchModeAsync(byte collarNumber, ConfigModeTypes mode, string username, ICustomWebSocketFactory wsFactory, byte[] customconfig = null,string DeviceId = null, string BaseId= null);
	}
}
