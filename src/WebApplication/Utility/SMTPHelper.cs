using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;

namespace com.Sconit.Utility
{
    public static class SMTPHelper
    {
        public static log4net.ILog log = log4net.LogManager.GetLogger("Log.Report");

        public static bool SendSMTPEMail(string Subject, string Body, string MailFrom, string MailTo, string SmtpServer, string MailFromPasswd, string ReplyTo)
        {
            return SendSMTPEMail(Subject, Body, MailFrom, MailTo, SmtpServer, MailFromPasswd, ReplyTo, null);
        }
        public static bool SendSMTPEMail(string Subject, string Body, string MailFrom, string MailTo, string SmtpServer, string MailFromPasswd, string ReplyTo, IList<string> files)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient(SmtpServer);
                foreach (string mailTo in MailTo.Split(';'))
                {
                    foreach (string mailto in mailTo.Split(','))
                    {
                        message.To.Add(new MailAddress(mailto));
                    }
                }
                message.Subject = Subject;
                message.Body = Body;

                string address = MailFrom;
                string displayName = string.Empty;
                if (!string.IsNullOrEmpty(ReplyTo))
                {
                    string[] mailfrom = ReplyTo.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                    displayName = mailfrom[0];
                    if (mailfrom.Length > 1)
                    {
                        address = mailfrom[1];
                    }
                    else
                    {
                        address = mailfrom[0];
                    }
                }

                message.From = new MailAddress(MailFrom, displayName, Encoding.UTF8);//address, displayName, Encoding.GetEncoding("GB2312")
                message.ReplyTo = new MailAddress(address, displayName, Encoding.UTF8);
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(MailFrom, MailFromPasswd);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                // System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment("D:\\logs\\" + filePath);
                // message.Attachments.Add(attachment);

                if (files != null && files.Count > 0)
                {
                    foreach (string file in files)
                    {
                        System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(file, MediaTypeNames.Application.Octet);

                        if (!message.Attachments.Contains(data))
                        {
                            ContentDisposition disposition = data.ContentDisposition;
                            disposition.CreationDate = System.IO.File.GetCreationTime(file);
                            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                            message.Attachments.Add(data);
                        }
                    }
                }

                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                client.Send(message);
                // message.Dispose();
                // logger.Error(Subject);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return false;
            }
        }
    }
}
