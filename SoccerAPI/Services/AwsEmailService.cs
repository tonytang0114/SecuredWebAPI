using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using SoccerAPI.Services.IService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SoccerAPI.Services
{
    public class AwsEmailService : IAwsEmailService
    {
        private readonly IConfiguration _config;
        private readonly IAmazonSimpleEmailService _emailService;
        private readonly ILogger _logger;

        public AwsEmailService(IConfiguration config, IAmazonSimpleEmailService emailService, ILogger<AwsEmailService> logger)
        {
            _config = config;
            _emailService = emailService;
            _logger = logger;
        }

        public Task<HttpStatusCode> SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtmlBody = true, List<string> cc = null, List<string> bcc = null)
        {
            var emailMessage = BuildEmailHeaders(_config.GetValue<string>("AwsEmailServiceOptions:Sender"), to, cc, bcc, subject);
            var emailBody = BuildEmailBody(body, isHtmlBody);
            emailMessage.Body = emailBody.ToMessageBody();
            return SendEmailAsync(emailMessage);
        }

        public Task<HttpStatusCode> SendEmailWithAttachmentAsync(IEnumerable<string> to, string subject, string body, bool isHtmlBody = true, string fileAttachmentPath = null, List<string> cc = null, List<string> bcc = null)
        {
            throw new NotImplementedException();
        }

        public Task<HttpStatusCode> SendEmailWithAttachmentAsync(IEnumerable<string> to, string subject, string body, bool isHtmlBody = true, string fileName = null, Stream fileAttachmentStream = null, List<string> cc = null, List<string> bcc = null)
        {
            throw new NotImplementedException();
        }

        private static MimeMessage BuildEmailHeaders(string from, IEnumerable<string> to, IReadOnlyCollection<string> cc,IReadOnlyCollection<string> bcc, string subject)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(string.Empty, from));
            foreach (var recipient in to)
            {
                message.To.Add(new MailboxAddress(string.Empty, recipient));
            }
            if (cc != null && cc.Any())
            {
                foreach (var recipient in cc)
                {
                    message.Cc.Add(new MailboxAddress(string.Empty, recipient));
                }
            }
            if (bcc != null && bcc.Any())
            {
                foreach (var recipient in bcc)
                {
                    message.Bcc.Add(new MailboxAddress(string.Empty, recipient));
                }
            }
            message.Subject = subject;
            return message;
        }

        private static BodyBuilder BuildEmailBody(string body, bool isHtmlBody = true)
        {
            var bodyBuilder = new BodyBuilder();
            if (isHtmlBody)
            {
                bodyBuilder.HtmlBody = $@"<html>
                                     <head>
                                         <title>SES Email</title>
                                     </head>
                                     <body>
                                         {body}
                                     </body>
                                 </html>";
            }
            else
            {
                bodyBuilder.TextBody = body;
            }
            return bodyBuilder;
        }

        private async Task<HttpStatusCode> SendEmailAsync(MimeMessage message)
        {
            using (var memoryStream = new MemoryStream())
            {
                await message.WriteToAsync(memoryStream);

                var sendRequest = new SendRawEmailRequest { RawMessage = new RawMessage(memoryStream) };

                var response = await _emailService.SendRawEmailAsync(sendRequest);
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation($"The email with message Id {response.MessageId} sent successfully to {message.To} on {DateTime.UtcNow:O}");
                }
                else
                {
                    _logger.LogError($"Failed to send email with message Id {response.MessageId} to {message.To} on {DateTime.UtcNow:O} due to {response.HttpStatusCode}.");
                }
                return response.HttpStatusCode;
            }
        }
    }
}
