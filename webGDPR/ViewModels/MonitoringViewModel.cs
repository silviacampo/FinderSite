using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webGDPR.Infrastructure;
using webGDPR.Infrastructure.CustomWebSockets;
using webGDPR.Models;

namespace webGDPR.ViewModels
{
	public class MonitoringViewModel
	{
		public List<CustomWebSocket> WebSockets { get; set; }
		public List<Device> Devices { get; set; }
		public PaginatedList<DeviceLog> DeviceLogs { get; internal set; }

		public string CurrentFilter { get; set; }
	}
}
