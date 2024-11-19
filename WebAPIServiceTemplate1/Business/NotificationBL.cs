using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using WebAPIServiceTemplate1.Objects.DTOs;

namespace WebAPIServiceTemplate1.Business
{
    public static class NotificationBL
    {
        private static readonly string _emailAccount = ConfigurationManager.AppSettings["EmailAccount"].ToString();
        private static readonly string _emailPassword = ConfigurationManager.AppSettings["EmailPassword"].ToString();
        private static readonly string _smtpServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();
        private static readonly string _port = ConfigurationManager.AppSettings["Port"].ToString();
        private static readonly string _Ssl = ConfigurationManager.AppSettings["UseSSL"].ToString();
        private static readonly string _bbc = ConfigurationManager.AppSettings["BBC"].ToString();
        private static readonly string _subject = ConfigurationManager.AppSettings["EmailSubject"].ToString();
        private static readonly string _emailBody = ConfigurationManager.AppSettings["EmailBody"].ToString();

        public static void SendEmail(NotificationDto notification)
        {
            string name = string.IsNullOrEmpty(notification.ClientInfo.Name) ? "Cliente" : notification.ClientInfo.Name;

            try
            {
                MailMessage mM = new MailMessage();
                mM.From = new MailAddress(_emailAccount);
                mM.To.Add(notification.ClientInfo.Email);
                mM.Bcc.Add(_bbc);
                mM.Subject = _subject;
                mM.Body = String.Format(_emailBody, name, notification.ClientInfo.Identification);
                mM.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(mM.Body, null, MediaTypeNames.Text.Html);
                mM.AlternateViews.Add(htmlView);
                Attachment adjunto = new Attachment(notification.CertificatePath);
                mM.Attachments.Add(adjunto);
                SmtpClient sC = new SmtpClient(_smtpServer);
                sC.Port = Convert.ToInt16(_port);
                sC.UseDefaultCredentials = false;
                sC.Credentials = new NetworkCredential(_emailAccount, _emailPassword);
                sC.EnableSsl = _Ssl == "true" ? true : false;

                sC.Send(mM);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}