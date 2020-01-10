﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SoccerAPI.Services.IService
{
    public interface IAwsEmailService
    {
        Task<HttpStatusCode> SendEmailAsync(IEnumerable<string> to, string subject, 
            string body, bool isHtmlBody = true, List<string> cc = null, List<string> bcc = null);

        Task<HttpStatusCode> SendEmailWithAttachmentAsync(IEnumerable<string> to, string subject,
            string body, bool isHtmlBody = true,string fileAttachmentPath = null, List<string> cc = null,List<string> bcc=null);

        Task<HttpStatusCode> SendEmailWithAttachmentAsync(IEnumerable<string> to, string subject, 
            string body, bool isHtmlBody = true,string fileName = null, Stream fileAttachmentStream = null, List<string> cc = null,
            List<string> bcc = null);
    }
}
