using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.ViewModels
{
    public class CollarViewModel
    {
		public string CollarId { get; set; }

		public string HWId { get; set; }

		public string Name { get; set; }
		public byte CollarNumber { get; set; }
		public byte BaseNumber { get; set; }

		public bool IsConnected { get; set; }
		public bool IsGPSConnected { get; set; }

		public bool IsNotGPSConnected { get { return !IsGPSConnected; } }

		public int Battery { get; set; }
		public int Radio { get; set; }

		public string RadioPercentage { get { return Radio.ToString() + "%"; } }

		public string Description { get; set; }

		public string UserId { get; set; }
	}
}
