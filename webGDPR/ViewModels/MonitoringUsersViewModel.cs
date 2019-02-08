using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webGDPR.Infrastructure;
using webGDPR.Infrastructure.CustomWebSockets;
using webGDPR.Models;

namespace webGDPR.ViewModels
{
	public class MonitoringUsersViewModel
	{
		public PaginatedList<User> Users { get; set; }

		public string CurrentFilter { get; set; }
	}
}
