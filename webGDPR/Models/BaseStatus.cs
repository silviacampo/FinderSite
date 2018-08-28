using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgendaSignalR.Infrastructure;

namespace webGDPR.Models
{
    public class BaseStatus
    {
		public string BaseStatusId { get; set; }
		public string BaseId { get; set; }
		public string UserId { get; set; }

		public bool IsActive { get; set; }

		public bool IsConnected { get; set; }
		public string ConnectedTo { get; set; }

		public bool IsPlugged { get; set; }

		public bool IsNotPlugged { get { return !IsPlugged; } }

		public bool IsCharging { get; set; }
		public int Battery { get; set; }

		public bool HasBattery { get; set; }

		public bool IsMissingBattery { get { return !HasBattery; } }

		public int Radio { get; set; }

		public string RadioPercentage
		{
			get
			{
				return Radio.ToString() + "%";
			}
		}

		public DateTime CreationDate { get; set; }

	}
}
