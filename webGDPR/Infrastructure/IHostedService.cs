using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Infrastructure
{
	public interface IHostedService
	{
		//
		// Summary:
		//     Triggered when the application host is ready to start the service.
		Task StartAsync(System.Threading.CancellationToken cancellationToken);
		//
		// Summary:
		//     Triggered when the application host is performing a graceful shutdown.
		Task StopAsync(System.Threading.CancellationToken cancellationToken);
	}
}
