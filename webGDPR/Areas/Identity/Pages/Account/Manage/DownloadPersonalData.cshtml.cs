using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using webGDPR.Data;
using webGDPR.Infrastructure;
using webGDPR.Models;



namespace webGDPR.Areas.Identity.Pages.Account.Manage
{
	public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;
		private readonly ApplicationDbContext _context;

		public DownloadPersonalDataModel(
            UserManager<ApplicationUser> userManager,
            ILogger<DownloadPersonalDataModel> logger, 
			ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
			_context = context;
		}

		private async Task<Dictionary<string, string>> ReadClient(ApplicationUser user, string UserId)
		{
			var personalData = new Dictionary<string, string>();
			var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
							prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
			foreach (var p in personalDataProps)
			{
				personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
			}

			User client = await _context.User.AsNoTracking().FirstOrDefaultAsync(b => b.UserID == UserId);
			personalData.Add("Name", client.Name);
			personalData.Add("UserEmail", client.Email);

			//await _context.User.AsNoTracking().Where(b => b.UserID == UserId).Include(b => b.Devices).Include(b => b.Bases).ThenInclude(c => c.BaseStatus).Include(b => b.Collars).ThenInclude(c => c.CollarStatus).Include(b => b.Pets).ThenInclude(c => c.PetCollars)..ThenInclude(c => c.PetTracking).ToListAsync();
			return personalData;
		}

		private Dictionary<string, string> ListClientFiles(string UserId)
		{
			var userpath = CustomPaths.GetUserPath(UserId);
			var filenamesAndUrls = new Dictionary<string, string>();
			if (Directory.Exists(Path.GetDirectoryName(userpath)))
			{
				DirectoryInfo d = new DirectoryInfo(userpath);
				FileInfo[] Files = d.GetFiles("*.*", SearchOption.AllDirectories);
				
				foreach (FileInfo file in Files)
				{
					string filename = Path.GetFileNameWithoutExtension(file.Name);
					string fileextension = file.Extension;

					if (filenamesAndUrls.ContainsKey(filename + fileextension)) {
						int copy = 0;
						do {
							copy++;
						} while (filenamesAndUrls.ContainsKey(filename + "(" + copy + ")" + fileextension));
						filename = filename +"("+copy+")";
					}
					filename = filename + fileextension;
					filenamesAndUrls.Add(filename, file.FullName);
				}
			}
			return filenamesAndUrls;
		}
		public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

			//TODO: zip it. include all other personal data and files
			//https://github.com/StephenClearyExamples/AsyncDynamicZip/tree/core-ziparchive

			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
			Dictionary<string, string> personalData = await ReadClient(user, UserId);
			var filenamesAndUrls = ListClientFiles(UserId);
			
		return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), async (outputStream, _) =>
			{
				using (var zipArchive = new ZipArchive(new WriteOnlyStreamWrapper(outputStream), ZipArchiveMode.Create))
				{
					var userZipEntry = zipArchive.CreateEntry("personaldata.json");
					using (var userZipStream = userZipEntry.Open())
					using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData))) )
						await stream.CopyToAsync(userZipStream);

					foreach (var kvp in filenamesAndUrls)
					{
						var zipEntry = zipArchive.CreateEntry(kvp.Key);
						using (var zipStream = zipEntry.Open())
						using (var stream = System.IO.File.OpenRead(kvp.Value))
							await stream.CopyToAsync(zipStream);
					}
				}
			})
			{
				FileDownloadName = "MyZipfile.zip"
			};

			//Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.zip");
			//Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
			//return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), "application/octet-stream");
			//return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), "text/json");
        }
    }

	public class WriteOnlyStreamWrapper : Stream
	{
		private readonly Stream _stream;
		private long _position;

		public WriteOnlyStreamWrapper(Stream stream)
		{
			_stream = stream;
		}

		public override bool CanRead => false;
		public override bool CanSeek => false;
		public override bool CanWrite => true;

		public override long Position
		{
			get { return _position; }
			set
			{
				throw new NotSupportedException();
			}
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			_position += count;
			_stream.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			_position += 1;
			_stream.WriteByte(value);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			_position += count;
			return _stream.WriteAsync(buffer, offset, count, cancellationToken);
		}



		// Crap that we just have to forward.

		public override bool CanTimeout => _stream.CanTimeout;
		public override int ReadTimeout
		{
			get { return _stream.ReadTimeout; }
			set { _stream.ReadTimeout = value; }
		}
		public override int WriteTimeout
		{
			get { return _stream.WriteTimeout; }
			set { _stream.WriteTimeout = value; }
		}

		public override void Flush() => _stream.Flush();
		public override Task FlushAsync(CancellationToken cancellationToken) => _stream.FlushAsync(cancellationToken);

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				_stream.Dispose();
		}

		public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			return _stream.CopyToAsync(destination, bufferSize, cancellationToken);
		}


		// Unsupported operations.

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}

	public class FileCallbackResult : FileResult
	{
		private Func<Stream, ActionContext, Task> _callback;

		/// <summary>
		/// Creates a new <see cref="FileCallbackResult"/> instance.
		/// </summary>
		/// <param name="contentType">The Content-Type header of the response.</param>
		/// <param name="callback">The stream with the file.</param>
		public FileCallbackResult(string contentType, Func<Stream, ActionContext, Task> callback)
			: this(MediaTypeHeaderValue.Parse(contentType), callback)
		{
		}

		/// <summary>
		/// Creates a new <see cref="FileCallbackResult"/> instance.
		/// </summary>
		/// <param name="contentType">The Content-Type header of the response.</param>
		/// <param name="callback">The stream with the file.</param>
		public FileCallbackResult(MediaTypeHeaderValue contentType, Func<Stream, ActionContext, Task> callback)
			: base(contentType?.ToString())
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			Callback = callback;
		}

		/// <summary>
		/// Gets or sets the callback responsible for writing the file content to the output stream.
		/// </summary>
		public Func<Stream, ActionContext, Task> Callback
		{
			get
			{
				return _callback;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_callback = value;
			}
		}

		/// <inheritdoc />
		public override Task ExecuteResultAsync(ActionContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			var executor = new FileCallbackResultExecutor(context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>());
			return executor.ExecuteAsync(context, this);
		}

		private sealed class FileCallbackResultExecutor : FileResultExecutorBase
		{
			public FileCallbackResultExecutor(ILoggerFactory loggerFactory)
				: base(CreateLogger<FileCallbackResultExecutor>(loggerFactory))
			{
			}

			public Task ExecuteAsync(ActionContext context, FileCallbackResult result)
			{
				//Todo: find this function SetHeadersAndLog(context, result);
				return result.Callback(context.HttpContext.Response.Body, context);
			}
		}
	}

}
