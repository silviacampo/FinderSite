using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;

namespace webGDPR.Infrastructure
{
	public class EmailSender : IEmailSender
	{
		private readonly IEmailConfiguration _emailConfiguration;
		private readonly ILogger<EmailSender> _logger;

		public EmailSender(IEmailConfiguration emailConfiguration, ILogger<EmailSender> logger)
		{
			_emailConfiguration = emailConfiguration;
			_logger = logger;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpUsername));
			message.To.Add(new MailboxAddress(email, email));
			message.Subject = subject;
			message.Body = new TextPart(TextFormat.Html)
			{
				Text = htmlMessage
			};

			using (var emailClient = new SmtpClient())
			{
				try
				{
					emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);

					emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

					emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

					await emailClient.SendAsync(message);

				}
				catch (Exception e)
				{
					_logger.LogError("CustomWebSocketManager - Listen: " + e.Message);
					throw new Exception();
				}
				finally
				{
					emailClient.Disconnect(true);
				}
			}			
		}
	}
}
