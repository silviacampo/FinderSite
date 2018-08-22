using System;
using System.Collections.Generic;

namespace AgendaSignalR.Infrastructure
{
	public interface ICustomWebSocketFactory
    {
		void Add(CustomWebSocket uws);
		void Remove(Guid guid);
		List<CustomWebSocket> All();
		List<CustomWebSocket> Group(string username);
		List<CustomWebSocket> Others(CustomWebSocket client);
		CustomWebSocket Client(Guid guid);
	}
}
