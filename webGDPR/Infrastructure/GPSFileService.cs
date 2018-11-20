using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Infrastructure
{
    public class GPSFileService : HostedService
	{
		public GPSFileService()
		{

		}

		protected override async Task ExecuteAsync(System.Threading.CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				TimeSpan timeBetween = DateTime.Today.AddDays(1).AddHours(1) - DateTime.Now;

				System.Timers.Timer t = new System.Timers.Timer();
				t.Elapsed += T_Elapsed;
				t.Interval = 1000 * timeBetween.TotalSeconds;
				t.Start();
			}
		}

		private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			//go and grab a new file
		}
	}
}
