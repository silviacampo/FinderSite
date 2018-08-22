using Microsoft.AspNetCore.Builder;
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
