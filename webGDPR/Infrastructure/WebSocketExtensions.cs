using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using webGDPR.Data;
using webGDPR.Infrastructure;

namespace AgendaSignalR.Infrastructure
{
    public static class WebSocketExtensions
    {
		public static IApplicationBuilder UseCustomWebSocketManager(this IApplicationBuilder app)
		{
			return app.UseMiddleware<CustomWebSocketManager>();
		}
	}
}
