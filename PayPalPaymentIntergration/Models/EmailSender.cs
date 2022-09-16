using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalPaymentIntergration.Models
{
    public class EmailSender: IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }
		
        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

		public async Task SendEmailAsync(string email, string subject, string message)
		{
			var mimeEmail = new MimeMessage();
			mimeEmail.Subject = subject;
			mimeEmail.From.Add(new MailboxAddress("Shop", "*******@gmail.com"));
			mimeEmail.To.Add(new MailboxAddress("User", email));

			//email.Body = new TextPart("plain")
			mimeEmail.Body = new TextPart("html")
			{
				Text = message
			};

			using (var client = new SmtpClient())
			{
				client.ServerCertificateValidationCallback = (sender, certificate, certChainType, errors) => true;
				client.AuthenticationMechanisms.Remove("XOAUTH2");

				await client.ConnectAsync("smtp.gmail.com", 587);
				//await client.AuthenticateAsync(Options.SendSMTPUser, Options.SendSMTPPassword);
				await client.AuthenticateAsync("******@gmail.com", "********");
				await client.SendAsync(mimeEmail);
				await client.DisconnectAsync(true);
			}
		}
		
	}
}
