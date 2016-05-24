using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Castle.Services.Transaction;
using com.Sconit.ISI.Service.Util;
using System.Net.Mime;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity;

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class SmtpMgr : ISmtpMgr
    {
        public string TestSystem { get; set; }
        public string SMTPEmailHost { get; set; }
        public string SMTPEmailPasswd { get; set; }
        public string SMTPEmailAddr { get; set; }
        public string WFSSMTPEmailPasswd { get; set; }
        public string WFSSMTPEmailAddr { get; set; }

        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");
        public IUserMgrE userMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }


        [Transaction(TransactionMode.Requires)]
        public void Send(string subject, string body, string mailTo, string replyTo, MailPriority priority)
        {
            try
            {
                #region email发送
                this.Send(subject, body, SMTPEmailAddr, mailTo, SMTPEmailHost, SMTPEmailPasswd, replyTo, priority, new List<string>());
                #endregion
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Send(string subject, string body, string mailTo, string replyTo, MailPriority priority, IList<string> files)
        {
            try
            {
                #region email发送
                this.Send(subject, body, SMTPEmailAddr, mailTo, SMTPEmailHost, SMTPEmailPasswd, replyTo, priority, files);
                #endregion
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Send(bool isApprove, string subject, string body, string mailTo, string replyTo, MailPriority priority, IList<string[]> files)
        {
            try
            {
                #region email发送
                if (isApprove)
                {
                    this.Send(subject, body, WFSSMTPEmailAddr, mailTo, SMTPEmailHost, WFSSMTPEmailPasswd, replyTo, priority, files);
                }
                else
                {
                    this.Send(subject, body, SMTPEmailAddr, mailTo, SMTPEmailHost, SMTPEmailPasswd, replyTo, priority, files);
                }
                #endregion
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Send(string subject, string body, string MailFrom, string mailTo, string replyTo, MailPriority priority)
        {
            try
            {
                if (string.IsNullOrEmpty(replyTo))
                {
                    replyTo = SMTPEmailAddr;
                }
                #region email发送
                this.Send(subject, body, MailFrom, mailTo, SMTPEmailHost, SMTPEmailPasswd, replyTo, priority, new List<string>());
                #endregion
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        public string Send(string subject, string body, string MailFrom, string mailTo, string SmtpServer, string MailFromPasswd, string replyTo)
        {
            return Send(subject, body, MailFrom, mailTo, SmtpServer, MailFromPasswd, replyTo, MailPriority.Normal, new List<string>());
        }

        public string Send(string subject, string body, string MailFrom, string mailTo, string SmtpServer, string MailFromPasswd, string replyTo, MailPriority priority, IList<string> files)
        {


            return Send(subject, body, MailFrom, mailTo, SmtpServer, MailFromPasswd, replyTo, MailPriority.Normal, files == null ? null : files.Select(f => new string[] { f }).ToList());
        }

        public string Send(string subject, string body, string MailFrom, string mailTo, string SmtpServer, string MailFromPasswd, string replyTo, MailPriority priority, IList<string[]> files)
        {
            MailMessage message = null;

            if (!string.IsNullOrEmpty(TestSystem) && TestSystem.Contains('@'))
            {
                mailTo = TestSystem;
            }

            try
            {
                message = new MailMessage();
                SmtpClient client = new SmtpClient(SmtpServer);
                var MailTos = mailTo.Split(';');
                foreach (string m in MailTos)
                {
                    var mailTos = m.Split(';');
                    foreach (string mailto in mailTos)
                    {
                        if (ISIUtil.IsValidEmail(mailto))
                        {
                            if (!message.Bcc.Contains(new MailAddress(mailto)))
                            {
                                message.Bcc.Add(new MailAddress(mailto));
                            }
                        }
                    }
                }

                message.Priority = priority;//优先级
                message.Subject = subject;
                message.Body = body;

                string address = MailFrom;

                string displayName = string.Empty;
                /*
                 * 不能代替发送邮件
                if (!string.IsNullOrEmpty(replyTo))
                {
                    string[] mailfrom = replyTo.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
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
                message.From = new MailAddress(address, displayName, Encoding.UTF8);//address, displayName, Encoding.GetEncoding("GB2312")
                message.replyTo = new MailAddress(address, displayName, Encoding.UTF8);
                */
                message.From = new MailAddress(address);
                message.ReplyTo = new MailAddress(address);
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(MailFrom, MailFromPasswd);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                // System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment("D:\\logs\\" + filePath);
                // message.Attachments.Add(attachment);

                if (files != null && files.Count > 0)
                {
                    foreach (string[] file in files)
                    {
                        System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(file[0], MediaTypeNames.Application.Octet);

                        if (!message.Attachments.Contains(data))
                        {
                            ContentDisposition disposition = data.ContentDisposition;
                            disposition.CreationDate = System.IO.File.GetCreationTime(file[0]);
                            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file[0]);
                            disposition.ReadDate = System.IO.File.GetLastAccessTime(file[0]);

                            if (file.Length == 2)
                            {
                                data.Name = file[1];

                                /*data.TransferEncoding = TransferEncoding.Base64;
                                data.NameEncoding = Encoding.UTF8;
                                disposition.FileName = file[1];
                                data.ContentType.Name = file[1];
                                 * */
                            }
                            message.Attachments.Add(data);
                        }
                    }
                }

                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = System.Text.Encoding.UTF8;//正文编码
                message.IsBodyHtml = true;//设置为HTML格式  

                client.Send(message);

                // message.Dispose();
                // logger.Error(subject);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (message != null)
                {
                    message.Dispose();
                }
            }
        }

        public void Send(string PermissionCode, string subject, string body, string replyTo)
        {
            Send(PermissionCode, subject, body, replyTo, null);
        }

        public void Send(string PermissionCode, string subject, string body, string replyTo, IList<string> files)
        {
            string mailList = userMgrE.FindEmailByPermission(new string[] { PermissionCode });
            IList<EntityPreference> entityPreferenceList = this.entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[]{BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME,
                                                    ISIConstants.ENTITY_PREFERENCE_WEBADDRESS});
            string companyName = entityPreferenceList.Where(e => e.Code == BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).SingleOrDefault().Value;
            string webAddress = entityPreferenceList.Where(e => e.Code == ISIConstants.ENTITY_PREFERENCE_WEBADDRESS).SingleOrDefault().Value;
            

            if (string.IsNullOrEmpty(mailList)) return;

            StringBuilder content = new StringBuilder();
            content.Append("<p style='font-size:15px;'>");
            string separator = ISIConstants.EMAIL_SEPRATOR;

            ISIUtil.AppendTestText(companyName, content, separator);

            content.Append(separator);
            content.Append("您好");
            content.Append(separator);
            content.Append("&nbsp;&nbsp;&nbsp;&nbsp;" + body);
            content.Append(separator);

            content.Append(separator);
            content.Append(companyName + separator);
            content.Append("<a href='http://" + webAddress + "'>http://" + webAddress + "</a>");
            content.Append(separator);
            content.Append("</p>");

            this.AsyncSend2(companyName + "-" + subject, content.ToString(), mailList, replyTo, MailPriority.Normal, files);
        }


        [Transaction(TransactionMode.Requires)]
        public void AsyncSend(
                            string subject, string body, string mailTo, string replyTo, MailPriority priority)
        {
            AsyncSendEmail asyncSend = new AsyncSendEmail(this.Send);

            asyncSend.BeginInvoke(subject, body, mailTo, replyTo, priority, null, null);
        }


        [Transaction(TransactionMode.Requires)]
        public void AsyncSend2(
                            string subject, string body, string mailTo, string replyTo, MailPriority priority, IList<string> files)
        {
            AsyncSendEmail2 asyncSend = new AsyncSendEmail2(this.Send);

            asyncSend.BeginInvoke(subject, body, mailTo, replyTo, priority, files, null, null);
        }

        [Transaction(TransactionMode.Requires)]
        public void AsyncSend3(bool isApprove, string subject, string body, string mailTo, string replyTo, MailPriority priority)
        {
            AsyncSendEmail3 asyncSend = new AsyncSendEmail3(this.Send);

            asyncSend.BeginInvoke(isApprove, subject, body, mailTo, replyTo, priority, null, null, null);
        }

        [Transaction(TransactionMode.Requires)]
        public void AsyncSend3(bool isApprove, string subject, string body, string mailTo, string replyTo, MailPriority priority, IList<string[]> files)
        {
            AsyncSendEmail3 asyncSend = new AsyncSendEmail3(this.Send);

            asyncSend.BeginInvoke(isApprove, subject, body, mailTo, replyTo, priority, files, null, null);
        }

        public delegate void AsyncSendEmail(
                                string subject, string body, string mailTo, string replyTo, MailPriority priority);

        public delegate void AsyncSendEmail2(
                                string subject, string body, string mailTo, string replyTo, MailPriority priority, IList<string> files);

        public delegate void AsyncSendEmail3(
                                bool isApprove, string subject, string body, string mailTo, string replyTo, MailPriority priority, IList<string[]> files);
    }
}

#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class SmtpMgrE : com.Sconit.ISI.Service.Impl.SmtpMgr, ISmtpMgrE
    {
    }
}

#endregion Extend Class