
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Service.Batch;
using com.Sconit.ISI.Entity;

using System.IO;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Service.Util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Ionic.Zip;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Service.Ext;
using com.Sconit.Entity;
using System.Net.Mail;

namespace com.Sconit.ISI.Service.Batch.Job
{
    [Transactional]
    public class StatisticsRepJob : IJob
    {
        public IUserMgrE uesrMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }
        public ISmtpMgrE smtpMgrE { get; set; }
        public ISqlHelperMgrE sqlHelperMgrE { get; set; }
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            DateTime startDate = ISIUtil.GetMondayDate();
            DateTime endDate = DateTime.Now;
            try
            {
                IList<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@StartDate", startDate));
                sqlParam.Add(new SqlParameter("@EndDate", endDate));
                DataSet userDS = this.sqlHelperMgrE.GetDatasetByStoredProcedure("USP_Rep_Comment", sqlParam.ToArray<SqlParameter>());
                var userList = IListHelper.DataTableToList<User>(userDS.Tables[0]);
                if (userList != null && userList.Count > 0)
                {
                    string subject = "周评论统计";
                    string separator = ISIConstants.EMAIL_SEPRATOR;

                    string mailTo = uesrMgrE.FindEmailByPermission(new string[] { ISIConstants.PERMISSION_PAGE_VALUE_ISISTATISTICS });

                    if (mailTo.Length == 0) return;
                    StringBuilder body = new StringBuilder();
                    string companyName = entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).Value;
                    ISIUtil.AppendTestText(smtpMgrE.IsTestSystem(), body, separator);
                    string webAddress = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_WEBADDRESS).Value;
                    body.Append("您好:");
                    body.Append(separator);
                    body.Append(separator);
                    body.Append(startDate.ToString("yyyy年MM月dd日 HH点mm分"));
                    body.Append(" 至 ");
                    body.Append(endDate.ToString("yyyy年MM月dd日 HH点mm分"));
                    body.Append("周评论统计: ");
                    body.Append(separator);
                    body.Append(separator);
                    body.Append("<table cellspacing='0' cellpadding='4' rules='all' border='1' style='border-collapse:collapse;font-size:12px;'>");
                    body.Append("<tr style='color:#FFFFFF;background-color:#000000;font-weight:bold;line-height:150%;'>");
                    body.Append("<th scope='col'>用户代码</th><th scope='col'>用户名</th>");
                    if (companyName.Contains("Jiehua"))
                    {
                        body.Append("<th scope='col'>杰康评论数</th><th scope='col'>本部评论数</th></tr>");
                        foreach (User user in userList)
                        {
                            body.Append("<tr><td>" + user.Code + "</td><td>" + user.Name + "</td><td>" + user.Count1 + "</td><td>" + user.Count2 + "</td></tr>");
                        }
                    }
                    else
                    {
                        body.Append("<th scope='col'>评论数</th></tr>");
                        foreach (User user in userList)
                        {
                            body.Append("<tr><td>" + user.Code + "</td><td>" + user.Name + "</td><td>" + user.Count1 + "</td></tr>");
                        }
                    }
                    body.Append("</table>");
                    body.Append(separator);
                    body.Append("重要: 实时查询请关注本公司网站（考核管理 > 信息 > 员工任务统计）。");
                    body.Append(separator);
                    body.Append("谢谢合作!");
                    body.Append(separator);
                    body.Append(separator);
                    body.Append(separator);
                    body.Append("<span style='font-size:15px;'>" + companyName + "</span><br/>");
                    body.Append("<span style='font-size:15px;'><a href='http://" + webAddress + "'>http://" + webAddress + "</a></span>");
                    MailPriority mailPriority = MailPriority.Normal;
                    string replyTo = string.Empty;
                    smtpMgrE.AsyncSend(subject, body.ToString(), mailTo, replyTo, mailPriority);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        class User
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public int Count1 { get; set; }
            public int Count2 { get; set; }
        }

    }
}



#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class StatisticsRepJob : com.Sconit.ISI.Service.Batch.Job.StatisticsRepJob
    {
        public StatisticsRepJob()
        {
        }
    }
}

#endregion
