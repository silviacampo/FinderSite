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
				//do something
				await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
			}
		}
	}
}
