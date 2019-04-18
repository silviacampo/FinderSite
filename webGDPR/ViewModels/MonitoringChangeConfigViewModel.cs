using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webGDPR.ViewModels
{
	public class MonitoringChangeConfigViewModel
	{
		public List<SelectListItem> DevicesItems { get; set; }
		public List<SelectListItem> BasesItems { get; set; }
		public List<SelectListItem> CollarsItems { get; set; }

		public string DeviceId { get; set; }
		public string BaseId { get; set; }
		public string CollarId { get; set; }

		public int CollarNumber { get; set; }       //CollarNumber, "0" is base, 127 all, "OR" 128 (wait for next query i.e. don't go to sleep)
		public int GpsPeriod { get; set; }          //in seconds, "0"= tracking mode
		public int GpsDuration { get; set; }        //in seconds, Gps active time per period
		public int NewBandwidth { get; set; }
		public int NewSpreadFactor { get; set; }
		public int BaseTimeout { get; set; }     //in 10s of seconds, base will rollback if not all the collars have communicated back
		public int NewFrequency { get; set; }         //Optional
		public bool NoInternet { get; set; }        //0=Internet connected, 1=No Internet
	}
}
