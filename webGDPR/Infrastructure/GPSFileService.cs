using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
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

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		ICustomWebSocketMessageHandler _webSocketMessageHandler;
		private readonly ICustomWebSocketFactory _wsFactory;
		private readonly ApplicationDbContext _dbContext;

		public GPSFileService(ICustomWebSocketMessageHandler webSocketMessageHandler, ICustomWebSocketFactory wsFactory)
		{
			_webSocketMessageHandler = webSocketMessageHandler;
			_wsFactory = wsFactory;
			//_dbContext =  new ApplicationDbContext( new Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext>(){ UseMySql(
			//		Configuration.GetConnectionString("DefaultConnection")));
		}
		System.Timers.Timer t;
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				t = new System.Timers.Timer();
				t.Elapsed += T_Elapsed;
				TimeSpan timeBetween = DateTime.Today.AddDays(1).AddHours(1) - DateTime.Now;
				t.Interval = 1000 * timeBetween.TotalSeconds;
				t.Start();
				log.Info($"GPSFileService - Wait for {t.Interval} seconds");
			}
		}

		private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			t.Stop();
			Task.Run(async () =>
			{
				await DownloadAndSave();
			});
				TimeSpan timeBetween = DateTime.Today.AddDays(1).AddHours(1) - DateTime.Now;
			t.Interval = 1000 * timeBetween.TotalSeconds;
				t.Start();
				log.Info($"GPSFileService - Wait for {t.Interval} seconds");
		}

		private async Task DownloadAndSave()
		{
			log.Info("GPSFileService - Download Start");
			var date = DateTime.Today.ToString("yy_MM_dd");
			var filename = $"mgaoffline-{date}.ubx";
			var gpsephemerispath = CustomPaths.GetGPSEphemerisPath();
			var path = Path.Combine(gpsephemerispath, filename);
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			else
			{
				ClearFolder(gpsephemerispath);
			}

			if (File.Exists((path)))
			{
				File.Delete(path);
			}
			byte[] filebytes = await DownloadFile(url);
			log.Info("GPSFileService - Writing File");
			await File.WriteAllBytesAsync(path, filebytes);
			log.Info("GPSFileService - Send Message to devices");
			await _webSocketMessageHandler.SendDownloadFile(new List<string> { CustomPaths.GetDownloadURL(0, filename) }, _wsFactory, _dbContext);
			log.Info("GPSFileService - Download Finished");

		}

		private void ClearFolder(string FolderName)
		{
			DirectoryInfo dir = new DirectoryInfo(FolderName);

			foreach (FileInfo fi in dir.GetFiles())
			{
				fi.Delete();
			}

			foreach (DirectoryInfo di in dir.GetDirectories())
			{
				ClearFolder(di.FullName);
				di.Delete();
			}
		}

		private async Task<byte[]> DownloadFile(string url)
		{
			using (var client = new HttpClient())
			{
				try
				{
					log.Info("GPSFileService - Calling Remote Server");
					using (var result = await client.GetAsync(url))
					{
						if (result.IsSuccessStatusCode)
						{
							log.Info("GPSFileService - Reading File");
							return await result.Content.ReadAsByteArrayAsync();
						}
					}
				}
				catch (Exception ex)
				{					
					log.Error($"GPSFileService - Error: {ex.Message}");
				}
			}
			return null;
		}
	}
}
