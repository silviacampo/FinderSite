namespace webGDPR.Hubs
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.SignalR;
	using webGDPR.Data;

	public class BroadcastHub : Hub
	{
		UserManager<ApplicationUser> _userManager;

		public BroadcastHub(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public override async Task OnConnectedAsync()
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, _userManager.GetUserName(this.Context.User));
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, _userManager.GetUserName(this.Context.User));
			await base.OnDisconnectedAsync(exception);
		}
	}
}
