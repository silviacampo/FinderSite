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

	}
}
