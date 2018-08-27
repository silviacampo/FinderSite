using System;
using System.Collections.Generic;
using System.Linq;

namespace AgendaSignalR.Infrastructure
{
	public class CustomWebSocketFactory : ICustomWebSocketFactory
	{
		List<CustomWebSocket> List;

		public CustomWebSocketFactory(){
			List = new List<CustomWebSocket>();
		}

		//when connect
		public void Add(CustomWebSocket uws)
		{
			List.Add(uws);
		}
		
		//when disconnect
		public void Remove(Guid guid) {
			List.Remove(Client(guid));
		}

		public List<CustomWebSocket> All()
		{
			return List;
		}

		public List<CustomWebSocket> Group(string username)
		{
			return List.Where(c=>c.Username == username).ToList();
		}

		public List<CustomWebSocket> Others(CustomWebSocket client)
		{
			return List.Where(c => c.Username == client.Username && c.Guid != client.Guid).ToList();
		}

		public CustomWebSocket Client(Guid guid)
		{
			return List.First(c => c.Guid == guid);
		}


	}
}
