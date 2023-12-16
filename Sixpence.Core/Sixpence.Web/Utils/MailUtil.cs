using Sixpence.Web.Config;
using Sixpence.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using Quartz.Logging;
using System.Reflection;

namespace Sixpence.Web.Utils
{
    public static class MailUtil
    {
        private static MailClient MailClient = new MailClient(MailConfig.Config.Name, MailConfig.Config.SMTP, MailConfig.Config.Password);
        public static void SendMail(string reciver, string title, string content)
        {
            MailClient.Send(reciver, title, content);
        }
    }

    public class MailClient
    {
        private string sender;
        private string smtp;
        private string password;
        private ILogger logger;
        public MailClient(string sender, string smtp, string password)
        {
            this.sender = sender;
            this.smtp = smtp;
            this.password = password;
            logger = AppContext.GetLogger<MailClient>();
        }

        public void Send(string recevier, string title, string content)
        {
            try
            {
                MailMessage message = new MailMessage();
                MailAddress fromAddr = new MailAddress(this.sender);
                message.From = fromAddr;
                message.To.Add(recevier);
                message.Subject = title;
                message.Body = content;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient(this.smtp, 25);
                client.Credentials = new NetworkCredential(this.sender, this.password);
                client.EnableSsl = true;
                client.Send(message);

                logger.LogInformation($"{this.sender}成功发送邮件给{recevier}");
            }
            catch (Exception ex)
            {
                logger.LogInformation("邮件发送失败");
                logger.LogError(ex, "邮件发送失败");
                throw new SpException("邮件发送失败，请联系管理员");
            }
        }
    }
}
