using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AgendaSignalR.Infrastructure;

namespace webGDPR.Models
{
    public class CollarStatus
    {
		public string CollarStatusId { get; set; }
		public string CollarId { get; set; }
		public string UserId { get; set; }

		public bool IsActive { get; set; }

		public bool IsConnected { get; set; }
		public string ConnectedTo { get; set; }

		[ForeignKey("ConnectedTo")]
		public Base BaseConnectedTo { get; set; }

		public bool IsGPSConnected { get; set; }

		public bool IsNotGPSConnected { get { return !IsGPSConnected; } }

		public int Battery { get; set; }
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
