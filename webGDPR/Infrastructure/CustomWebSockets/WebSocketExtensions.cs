using Microsoft.AspNetCore.Builder;
using webGDPR.Infrastructure;

namespace webGDPR.Infrastructure.CustomWebSockets
{
	public static class WebSocketExtensions
    {
		public static IApplicationBuilder UseCustomWebSocketManager(this IApplicationBuilder app)
		{
			return app.UseMiddleware<CustomWebSocketManager>();
		}
	}
}
