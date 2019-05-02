namespace webGDPR.Hubs
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.SignalR;
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using webGDPR.Data;

	public class ChatHub : Hub
	{
		UserManager<ApplicationUser> _userManager;

		public ChatHub(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task EchoMessage(string user, string message)
		{
			await Clients.Caller.SendAsync("ReceiveMessage", _userManager.GetUserName(this.Context.User), message);
		}

		public async Task RespondMessage(string message, string ConnectionId)
		{
			await Clients.Client(ConnectionId).SendAsync("ReceiveMessage", _userManager.GetUserName(this.Context.User), message);
		}

		public void SendMessage(string message)
		{
			var chatUser = ChatHandler.ConnectedUsers.Find(c => c.User == Context.User);
			if (chatUser.Messages == null) {
				chatUser.Messages = new HashSet<ChatMessage>();
			}
			chatUser.Messages.Add(new ChatMessage { Incoming = true, Message = message, Time = DateTime.Now });
			//await Clients.All.SendAsync("ReceiveMessage", _userManager.GetUserName(this.Context.User), message);
		}

		public override Task OnConnectedAsync()
		{
			ChatHandler.ConnectedUsers.Add(new ChatUser { ConnectedId = Context.ConnectionId, User = Context.User });
			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			ChatHandler.ConnectedUsers.Remove(ChatHandler.ConnectedUsers.Find(c=>c.ConnectedId == Context.ConnectionId));
			return base.OnDisconnectedAsync(exception);
		}
	}

	public static class ChatHandler
	{
		//The HashSet<T> class provides high-performance set operations. A set is a collection that contains no duplicate elements, and whose elements are in no particular order.
		public static List<ChatUser> ConnectedUsers = new List<ChatUser>();
	}

	public class ChatUser {
		public string ConnectedId { get; set; }
		public System.Security.Claims.ClaimsPrincipal User { get; set; }
		public HashSet<ChatMessage> Messages { get; set; }
}

	public class ChatMessage
	{
		public bool Incoming { get; set; }
		public string Message { get; set; }
		public DateTime Time { get; set; }
	}
}
