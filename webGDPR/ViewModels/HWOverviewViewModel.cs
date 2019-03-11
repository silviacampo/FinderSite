using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webGDPR.Models;

namespace webGDPR.ViewModels
{
	public class HWOverviewViewModel
	{
		public User User { get; set; }

		public double[] AvgDistance { get; set; }

		public double AvgDistanceDay { get; set; }

		public double[] MediumAvgDistance { get; set; }

		public double MediumAvgDistanceDay { get; set; }

		public List<Tuple<PetTrackingInfo, string>> PointVisited { get; internal set; }

		public Device MostConnectedToDevice { get; set;}

		public Base MostConnectedToBase { get; set; }

		public List<BaseStats> BaseStats { get; set; }

		public List<CollarStats> CollarStats { get; set; }

		public List<PointServiceLevel> PointsServiceLevel { get; set; }
	}

	public class BaseStats
	{
		public Base Base { get; set; }
		public TimeSpan DisconnectedTimeSpan { get; set; }
		public List<Tuple<string, TimeSpan>> ConnectedToTimeSpan { get; set; }
		public TimeSpan[] RadioTimeSpan { get; set; }
		public double AvgRadio { get; set; }
		public TimeSpan IsChargingTimeSpan { get; set; }
		public bool BatteriesChargingMore75percent { get; set; }
		public TimeSpan PluginTimSpan { get; internal set; }

		public double AvgConnected { get; set; }
		public double AvgPlugIn { get; set; }
	}

	public class CollarStats
	{
		public Collar Collar { get; set; }
		public TimeSpan DisconnectedTimeSpan { get; set; }
		public List<Tuple<string, TimeSpan>> ConnectedToTimeSpan { get; set; }
		public TimeSpan[] RadioTimeSpan { get; set; }
		public double AvgRadio { get; set; }
		public TimeSpan GPSDisconnectedTimeSpan { get; set; }
		public double BatteryMinus25Minutes { get; set; }
		public TimeSpan[] BatteryTimeSpan { get; set; }
		public double AvgBattery { get; set; }

		public double AvgConnected { get; set; }
		public double AvgGPSConnected { get; set; }
	}

	public class PointServiceLevel
	{
		public bool GPS { get; set; }
		public int Radio { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
}
