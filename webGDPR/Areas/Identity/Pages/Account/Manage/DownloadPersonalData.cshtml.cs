using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using webGDPR.Data;
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

		private async Task<User> ReadClient(ApplicationUser user)
		{
			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;

			var userpath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\user\\{UserId}");
			if (Directory.Exists(Path.GetDirectoryName(userpath)))
			{
				
			}

			await _context.SaveChangesAsync();
			User client = new User(); // await _context.User.AsNoTracking().Where(b => b.UserID == UserId).Include(b => b.Devices).Include(b => b.Bases).ThenInclude(c => c.BaseStatus).Include(b => b.Collars).ThenInclude(c => c.CollarStatus).Include(b => b.Pets).ThenInclude(c => c.PetCollars)..ThenInclude(c => c.PetTracking).ToListAsync();
			return client;
		}

		public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }
			//TODO: zip it. include all other personal data and files
			//https://github.com/StephenClearyExamples/AsyncDynamicZip/tree/core-ziparchive
			string clientInfo = JsonConvert.SerializeObject(ReadClient(user));

			var filenamesAndUrls = new Dictionary<string, string>
			{
				{ "README.md", "https://raw.githubusercontent.com/StephenClearyExamples/AsyncDynamicZip/master/README.md" },
				{ ".gitignore", "https://raw.githubusercontent.com/StephenClearyExamples/AsyncDynamicZip/master/.gitignore" },
			};
			HttpClient Client = new HttpClient();
			
		return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), async (outputStream, _) =>
			{
				using (var zipArchive = new ZipArchive(new WriteOnlyStreamWrapper(outputStream), ZipArchiveMode.Create))
				{
					foreach (var kvp in filenamesAndUrls)
					{
						var zipEntry = zipArchive.CreateEntry(kvp.Key);
						using (var zipStream = zipEntry.Open())
						using (var stream = await Client.GetStreamAsync(kvp.Value))
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
