using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace com.Sconit.ISI.Service
{
    public interface ISmtpMgr
    {
        bool IsTestSystem();
        void AsyncSend(string subject, string body, string mailTo, string replyTo, MailPriority priority);
        void AsyncSend2(string subject, string body, string mailTo, string replyTo, MailPriority priority, IList<string> files);
        void Send(string PermissionCode, string subject, string body, string replyTo, IList<string> files);
        void Send(string subject, string body, string mailTo, string replyTo, MailPriority priority);
        void Send(string PermissionCode, string subject, string body, string replyTo);
        void Send(string subject, string body, string mailTo, string replyTo, MailPriority priority, IList<string> files);
        void AsyncSend3(bool isApprove, string subject, string body, string mailTo, string replyTo, MailPriority priority);
        void AsyncSend3(bool isApprove, string subject, string body, string mailTo, string replyTo, MailPriority priority, IList<string[]> files);
    }
}

#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ISmtpMgrE : com.Sconit.ISI.Service.ISmtpMgr
    {
    }
}

#endregion Extend Interface
