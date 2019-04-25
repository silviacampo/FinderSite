namespace webGDPR.Hubs
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.SignalR;
	using System.Threading.Tasks;
	using webGDPR.Data;

	public class ChatHub : Hub
	{
		UserManager<ApplicationUser> _userManager;

		public ChatHub(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task SendMessage(string user, string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", _userManager.GetUserName(this.Context.User), message);
		}
	}
}
