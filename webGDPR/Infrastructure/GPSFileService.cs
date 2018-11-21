using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using webGDPR.Data;
using webGDPR.Infrastructure.CustomWebSockets;

namespace webGDPR.Infrastructure
{
	public class GPSFileService : HostedService
    {
        public const string url = "http://offline-live1.services.u-blox.com/GetOfflineData.ashx?token=Tdw1rYjjLES8m8cObGyfiA;gnss=gps,glo;alm=gps,glo;period=1;resolution=2";

		public const string localUrl = "/gps/mgaoffline.ubx";

		ICustomWebSocketMessageHandler _webSocketMessageHandler;
		ICustomWebSocketFactory _wsFactory;
		ApplicationDbContext _dbContext;

		public GPSFileService(ICustomWebSocketMessageHandler webSocketMessageHandler, ICustomWebSocketFactory wsFactory)
        {
			_webSocketMessageHandler = webSocketMessageHandler;
			_wsFactory = wsFactory;
			//_dbContext =  new ApplicationDbContext( new Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext>(){ UseMySql(
			//		Configuration.GetConnectionString("DefaultConnection")));
		}
        System.Timers.Timer t;

        protected override async Task ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
			await Task.Delay(TimeSpan.FromMinutes(5));
			await DownloadAndSave();
			while (!cancellationToken.IsCancellationRequested)
            {
                TimeSpan timeBetween = DateTime.Today.AddDays(1).AddHours(1) - DateTime.Now;
                t = new System.Timers.Timer();
                t.Elapsed += T_Elapsed;
                t.Interval = 1000 * timeBetween.TotalSeconds;
                t.Start();
			}
        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            t.Stop();
            Task.Run(async () =>
            {
                while (true)
                {
                    await DownloadAndSave();
                    await Task.Delay(TimeSpan.FromDays(1));
                }
            });


        }

        private async Task DownloadAndSave()
        {
            var htmlpath = Path.Combine(CustomPaths.GetGPSFilesPath(), "mgaoffline.ubx");
            if (!Directory.Exists(Path.GetDirectoryName(htmlpath)))
                Directory.CreateDirectory(Path.GetDirectoryName(htmlpath));

            if (File.Exists((htmlpath)))
            {
                File.Delete(htmlpath);
            }
            byte[] filebytes = await DownloadFile(url);
            await File.WriteAllBytesAsync(htmlpath, filebytes);
			//notify devices
			await _webSocketMessageHandler.SendGPSFile(localUrl, _wsFactory, _dbContext);

		}

        private async Task<byte[]> DownloadFile(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    using (var result = await client.GetAsync(url))
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            return await result.Content.ReadAsByteArrayAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
					//todo
                    var test = ex.Message;
                }
            }
            return null;
        }
    }
}
