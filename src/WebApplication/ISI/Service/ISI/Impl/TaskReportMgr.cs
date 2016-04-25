using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Entity.MasterData;
using System.Linq;
using com.Sconit.ISI.Service.Util;
using System.Net.Mail;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class TaskReportMgr : TaskReportBaseMgr, ITaskReportMgr
    {
        public ITaskSubTypeMgrE taskSubTypeMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        public ISmtpMgrE smtpMgrE { get; set; }
        public ITaskMstrMgrE taskMstrMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }
        public ICodeMasterMgrE codeMasterMgrE { get; set; }
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        #region Customized Methods

        public IList<TaskReportView> GetUserAllTaskSubType(string userCode)
        {
            IList<TaskSubType> taskSubTypeList = taskSubTypeMgrE.GetTaskSubType(string.Empty, false, userCode, false, false, false, false);

            if (taskSubTypeList == null || taskSubTypeList.Count == 0) return null;

            StringBuilder hql = new StringBuilder();
            hql.Append(@"Select tr.Id, tr.TaskSubType, tr.IsActive ");
            hql.Append(@"From TaskReport tr,User u ");
            hql.Append(@"Where tr.User = u.Code ");
            hql.Append(@"and u.Code =:UserCode and tr.TaskSubType in (:TaskSubType)");
            hql.Append(@"Order by tr.TaskSubType Asc,tr.Id Asc ");
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("UserCode", userCode);
            param.Add("TaskSubType", taskSubTypeList.Select(t => t.Code).ToArray<string>());

            IList<object[]> userSubscriptionList = this.hqlMgrE.FindAll<object[]>(hql.ToString(), param);

            IList<TaskReportView> taskReportViewList = (from t in taskSubTypeList
                                                        select new TaskReportView() { TaskSubTypeCode = t.Code, TaskSubTypeDesc = t.Desc, TaskType = t.Type }).ToList<TaskReportView>();
            if (userSubscriptionList != null && userSubscriptionList.Count > 0)
            {
                foreach (TaskReportView taskReportView in taskReportViewList)
                {
                    foreach (object[] userSubscription in userSubscriptionList)
                    {
                        if (userSubscription[1].ToString() == taskReportView.TaskSubTypeCode)
                        {
                            if (userSubscription[0] == null)
                            {
                                taskReportView.Id = null;
                            }
                            else
                            {
                                taskReportView.Id = int.Parse(userSubscription[0].ToString());
                            }
                            taskReportView.IsActive = userSubscription[2] == null ? false : bool.Parse(userSubscription[2].ToString());
                        }
                    }
                }
            }

            return taskReportViewList;
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateTaskReport(IList<TaskReportView> taskReportViewList, User user)
        {
            if (taskReportViewList == null || taskReportViewList.Count == 0) return;
            foreach (TaskReportView taskReportView in taskReportViewList)
            {
                this.UpdateTaskReport(taskReportView, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateTaskReport(TaskReportView taskReportView, User user)
        {
            DateTime now = DateTime.Now;
            TaskReport taskReport = null;
            if (taskReportView.Id.HasValue)
            {
                taskReport = this.LoadTaskReport(taskReportView.Id.Value);
            }
            else
            {
                taskReport = new TaskReport();
            }
            taskReport.LastModifyDate = now;
            taskReport.LastModifyUser = user.Code;
            taskReport.IsActive = taskReportView.IsActive;
            if (taskReportView.Id.HasValue)
            {
                this.UpdateTaskReport(taskReport);
            }
            else
            {
                taskReport.User = user.Code;
                taskReport.TaskSubType = taskReportView.TaskSubTypeCode;
                taskReport.CreateUser = user.Code;
                taskReport.CreateDate = now;
                this.CreateTaskReport(taskReport);
            }

        }

        [Transaction(TransactionMode.Requires)]
        public void Check()
        {
            StringBuilder hql = new StringBuilder();
            hql.Append(@" select tr.Id from TaskReport tr ");
            hql.Append(@" where  ");
            hql.Append(@"      not exists (select up.Permission.Code from UserPermission up where up.User.Code = tr.User and ((up.Permission.Category.Code=:TaskSubType and tr.TaskSubType = up.Permission.Code) or up.Permission.Code in (:Admin))) ");
            hql.Append(@"  and not exists (select rp.Permission.Code from RolePermission rp join rp.Role r,UserRole ur where r.Code = ur.Role.Code and ur.User.Code = tr.User and ((rp.Permission.Category.Code=:TaskSubType and tr.TaskSubType = rp.Permission.Code ) or (rp.Permission.Code in (:Admin)))) ");

            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("TaskSubType", ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
            param.Add("Admin", new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN });

            IList<int> idList = hqlMgrE.FindAll<int>(hql.ToString(), param);

            if (idList == null || idList.Count == 0) return;

            this.DeleteTaskReport(idList);

        }

        [Transaction(TransactionMode.RequiresNew)]
        public void SendReport()
        {
            try
            {
                DateTime endDate = DateTime.Now;
                //todo
                DateTime startDate = endDate.AddDays(-1);
                //startDate = endDate.AddDays(-90);
                IDictionary<string, object> param = new Dictionary<string, object>();

                param.Add("StartDate", startDate);
                param.Add("EndDate", endDate);
                //param.Add("TaskType", ISIConstants.ISI_TASK_TYPE_PRIVACY);
                param.Add("Permissions", new string[] { ISIConstants.PERMISSION_PAGE_ISI_VALUE_TASKREPORT, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN });

                var tracers = GetTracers(param);
                if (tracers != null && tracers.Count > 0)
                {
                    var tracerDetail = GetTracerDetail(param);
                    if (tracerDetail != null && tracerDetail.Count > 0)
                    {   
                        //获得用户名字
                        string users = string.Join(";", tracers.Select(t => (string)t[6]).ToArray<string>());
                        users += ISIConstants.ISI_LEVEL_SEPRATOR + string.Join(";", tracerDetail.Select(s => (string)s[9]).ToArray<string>());

                        var userList = (from subType in tracers
                                        group subType by new { Code = subType[0], FirstName = subType[1], LastName = subType[2], Email = subType[3] } into s
                                        select new UserSub
                                        {
                                            Code = (string)s.Key.Code,
                                            Name = (string)s.Key.FirstName + (string)s.Key.LastName,
                                            Email = (string)s.Key.Email
                                        }).ToList();

                        if (userList != null && userList.Count > 0)
                        {
                            var statusCodeMstrList = this.codeMasterMgrE.GetCachedCodeMaster(ISIConstants.CODE_MASTER_ISI_STATUS).ToDictionary(c => c.Value, c => c.Description);
                            IDictionary<string, UserSub> userDic = null;
                            IDictionary<string, string> colorDic = null;
                            if (!string.IsNullOrEmpty(users))
                            {
                                userDic = taskMstrMgrE.GetUser2(users);
                            }

                            foreach (var user in userList)
                            {
                                try
                                {
                                    StringBuilder mailBody = new StringBuilder();
                                    StringBuilder mailDetailBody = new StringBuilder();
                                    colorDic = new Dictionary<string, string>();
                                    string companyName = entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).Value;
                                    ISIUtil.AppendTestText(smtpMgrE.IsTestSystem(), mailBody, ISIConstants.EMAIL_SEPRATOR);
                                    StringBuilder subject = new StringBuilder();
                                    subject.Append("ISI系统任务报告");
                                    mailBody.Append("<span style='font-size:13px;'>尊敬的: " + user.Name + " 先生/女士</span><br /><br />");
                                    mailBody.Append("&nbsp;&nbsp;<span style='font-size:13px;'>您的 ISI系统任务报告。</span><br />");
                                    mailBody.Append("&nbsp;&nbsp;<span style='font-size:13px;'>时间范围: " + startDate.ToString("yyyy-MM-dd HH:mm") + " 至 " + endDate.ToString("yyyy-MM-dd HH:mm") + "</span><br /><br /><br />");

                                    //任务分类列表
                                    var subTypeList = tracers.Where(t => (string)t[0] == user.Code)
                                                        .Select(s => new
                                                        {
                                                            Code = (string)s[0],
                                                            FirstName = (string)s[1],
                                                            LastName = (string)s[2],
                                                            Email = (string)s[3],
                                                            TaskSubTypeCode = (string)s[4],
                                                            TaskSubTypeDesc = (string)s[5],
                                                            AssignUser = (string)s[6],
                                                            Count = (long)s[7]
                                                        }).ToList();
                                    if (subTypeList != null && subTypeList.Count > 0)
                                    {
                                        mailBody.Append("<span style='font-size:15px;'>总览</span><br />");
                                        mailBody.Append("<table cellspacing='0' cellpadding='4' rules='all' border='1' style='width:80%;border-collapse:collapse;font-size:12px;'>");
                                        mailBody.Append("<tr style='color:#FFFFFF;background-color:#000060;font-weight:bold;line-height:150%;'>");
                                        mailBody.Append("<th scope='col'>任务分类编号</th><th scope='col'>任务分类描述</th><th scope='col'>分派人</th><th scope='col'>更新数</th></tr>");
                                        foreach (var subType in subTypeList)
                                        {
                                            #region 汇总前半部分
                                            mailBody.Append("<tr>");
                                            mailBody.Append("<td nowrap><a style='text-decoration:none;' href='#" + subType.TaskSubTypeCode + "'>" + subType.TaskSubTypeCode + "</a></td>");
                                            mailBody.Append("<td nowrap><a style='text-decoration:none;' href='#" + subType.TaskSubTypeCode + "'>" + subType.TaskSubTypeDesc + "</a></td>");
                                            mailBody.Append("<td ><a style='text-decoration:none;' href='#" + subType.TaskSubTypeCode + "'>");
                                            StringBuilder assignUser = new StringBuilder();
                                            if (userDic != null && userDic.Count > 0 && !string.IsNullOrEmpty(subType.AssignUser))
                                            {
                                                string[] userCodes = subType.AssignUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                                                if (userCodes != null && userCodes.Length > 0)
                                                {
                                                    for (int i = 0; i < userCodes.Length; i++)
                                                    {
                                                        if (userDic.Keys.Contains(userCodes[i]))
                                                        {
                                                            if (i != 0)
                                                            {
                                                                assignUser.Append(", ");
                                                            }
                                                            assignUser.Append(userDic[userCodes[i]].Name);
                                                        }
                                                    }

                                                    mailBody.Append(assignUser.ToString());
                                                }
                                            }
                                            mailBody.Append("</a></td>");
                                            mailBody.Append("<td nowrap style='font-size:15px;");
                                            #endregion

                                            #region 明细
                                            var taskTracerList = tracerDetail.Where(t => (string)t[0] == user.Code && (string)t[1] == subType.TaskSubTypeCode).Select(s => new TaskView
                                                                                    {
                                                                                        UserCode = (string)s[0],
                                                                                        TaskSubTypeCode = (string)s[1],
                                                                                        TaskSubTypeDesc = (s[2] == null || String.IsNullOrEmpty((string)s[2])) ? string.Empty : (string)s[2],
                                                                                        Code = (string)s[3],
                                                                                        Subject = (s[4] == null || String.IsNullOrEmpty((string)s[4])) ? string.Empty : (string)s[4],
                                                                                        Desc1 = (s[5] == null || String.IsNullOrEmpty((string)s[5])) ? string.Empty : (string)s[5],
                                                                                        Desc2 = (s[6] == null || String.IsNullOrEmpty((string)s[6])) ? string.Empty : (string)s[6],
                                                                                        AssignUserNm = (s[7] == null || String.IsNullOrEmpty((string)s[7])) ? string.Empty : (string)s[7],
                                                                                        SubmitUserNm = (s[8] == null || String.IsNullOrEmpty((string)s[8])) ? string.Empty : (string)s[8],
                                                                                        StartedUser = s[9] != null ? (string)s[9] : string.Empty,
                                                                                        //SchedulingStartUser = s[9] != null ? (string)s[9] : string.Empty,
                                                                                        //AssignStartUser = s[10] != null ? (string)s[10] : string.Empty,
                                                                                        Flag = s[10] == null ? string.Empty : (string)s[10],
                                                                                        Color = s[11] == null ? string.Empty : (string)s[11],
                                                                                        StatusDesc = (s[12] == null || String.IsNullOrEmpty((string)s[12])) ? string.Empty : (string)s[12],
                                                                                        StatusDate = s[13] != null ? (DateTime)s[13] : new System.Nullable<DateTime>(),
                                                                                        CreateUserNm = s[14] != null ? (string)s[14] : string.Empty,
                                                                                        CommentCreateUserNm = s[15] != null ? (string)s[15] : string.Empty,
                                                                                        CommentCreateDate = s[16] != null ? (DateTime)s[16] : new System.Nullable<DateTime>(),
                                                                                        Comment = s[17] != null ? (string)s[17] : string.Empty,
                                                                                        ExpectedResults = !String.IsNullOrEmpty((string)s[18]) ? (string)s[18] : string.Empty,
                                                                                        Priority = (string)s[19],
                                                                                        AttachmentCount = (s[20] != null ? int.Parse(s[20].ToString()) : 0),
                                                                                        Phase = (s[21] != null ? (string)s[21] : string.Empty),
                                                                                        Seq = (s[22] != null ? (string)s[22] : string.Empty),
                                                                                        Type = (string)s[23],
                                                                                        PlanCompleteDate = s[24] != null ? (DateTime)s[24] : new System.Nullable<DateTime>(),
                                                                                        Status = (string)s[25],
                                                                                        CommentCount = (s[26] != null ? int.Parse(s[26].ToString()) : 0),
                                                                                        RefTaskCount = (s[27] != null ? int.Parse(s[27].ToString()) : 0),
                                                                                        StatusCount = (s[28] != null ? int.Parse(s[28].ToString()) : 0)
                                                                                    }).ToList();
                                            if (taskTracerList != null && taskTracerList.Count > 0)
                                            {
                                                mailDetailBody.Append("<br/>");
                                                mailDetailBody.Append("<a style='font-size:13px;' name='" + subType.TaskSubTypeCode + "'>" + subType.TaskSubTypeCode + "&nbsp;&nbsp;[" + subType.TaskSubTypeDesc + "]&nbsp;&nbsp;分派人：" + assignUser.ToString() + "</a>");
                                                ISIUtil.GetColumnHead(mailDetailBody);
                                                foreach (var taskTracer in taskTracerList)
                                                {
                                                    ISIUtil.GetReportDetailBody(mailDetailBody, taskTracer, endDate, userDic, statusCodeMstrList);

                                                    if (taskTracer.Color == ISIConstants.CODE_MASTER_ISI_FLAG_RED && !colorDic.Keys.Contains(taskTracer.TaskSubTypeCode))
                                                    {
                                                        colorDic.Add(taskTracer.TaskSubTypeCode, taskTracer.Color);
                                                    }
                                                }
                                                mailDetailBody.Append("</table><br />");
                                            }
                                            #endregion

                                            #region 汇总后半部分

                                            if (colorDic.Keys.Contains(subType.TaskSubTypeCode))
                                            {
                                                mailBody.Append("background-color:" + colorDic[subType.TaskSubTypeCode] + ";");
                                            }
                                            mailBody.Append("'><a style='text-decoration:none;' href='#" + subType.TaskSubTypeCode + "'>" + subType.Count + "</a></td>");
                                            mailBody.Append("</tr>");
                                            #endregion
                                        }
                                        mailBody.Append("</table><br /><br />");
                                    }
                                    string webAddress = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_WEBADDRESS).Value;
                                    mailBody.Append("<span style='font-size:14px;'>详细情况</span><br />");
                                    mailBody.Append(mailDetailBody.ToString() + "<br /><br /><br /><br />");
                                    mailBody.Append("<span style='font-size:13px;'>重要:本邮件只是提示,具体内容以 <a href='http://" + webAddress + "'>ISI系统</a> 为准.</span><br />");
                                    mailBody.Append("<span style='font-size:13px;'>谢谢合作!</span><br /><br />");
                                    mailBody.Append("<span style='font-size:13px;'>" + companyName + "</span><br/>");
                                    mailBody.Append("<span style='font-size:13px;'><a href='http://" + webAddress + "'>http://" + webAddress + "</a></span>");
                                    //todo
                                    //this.SmtpMgrE.AsyncSend(subject.ToString(), mailBody.ToString(), "tiansu@yfgm.com.cn", string.Empty, MailPriority.Normal);
                                    this.smtpMgrE.AsyncSend(subject.ToString(), mailBody.ToString(), user.Email, string.Empty, MailPriority.Normal);
                                }
                                catch (Exception e)
                                {
                                    log.Error(e.Message, e);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }
        [Transaction(TransactionMode.Requires)]
        private IList<object[]> GetTracers(IDictionary<string, object> param)
        {
            StringBuilder tracerSql = new StringBuilder();
            tracerSql.Append(@"select u.Code,u.FirstName,u.LastName,u.Email,ts.TaskSubTypeCode,ts.TaskSubTypeDesc,ts.TaskSubTypeAssignUser,Count(ts.TaskSubTypeCode) ");
            tracerSql.Append(@"from TaskReport tr,User u, ");
            tracerSql.Append(@"     TaskStatusView ts ");
            tracerSql.Append(
                @"where tr.TaskSubType = ts.TaskSubTypeCode and tr.IsActive = 1 and tr.User = u.Code ");
            tracerSql.Append(@"and u.Email != '' and u.Email is not null and u.IsActive = 1 ");
            tracerSql.Append(
                @"and ts.CreateDate <= :EndDate and ts.CreateDate>= :StartDate and ts.StatusDesc != '' and ts.StatusDesc is not null ");
            tracerSql.Append(@"and ");
            tracerSql.Append(@"      ( ");
            tracerSql.Append(
                @"          exists (select up.Permission.Code from UserPermission up where up.User.Code = u.Code and up.Permission.Code in (:Permissions)) ");
            tracerSql.Append(@"      or ");
            tracerSql.Append(
                @"          exists (select rp.Permission.Code from RolePermission rp join rp.Role r,UserRole ur where r.Code = ur.Role.Code and ur.User.Code = u.Code and rp.Permission.Code in (:Permissions))  ");
            tracerSql.Append(@"      ) ");
            tracerSql.Append(
                @"group by u.Code,u.FirstName,u.LastName,u.Email,ts.TaskSubTypeCode,ts.TaskSubTypeDesc,ts.TaskSubTypeAssignUser having count(1)>0 ");
            tracerSql.Append(@"order by u.Code,ts.TaskSubTypeCode ");
            IList<object[]> tracers = this.hqlMgrE.FindAll<object[]>(tracerSql.ToString(), param);
            return tracers;
        }

        [Transaction(TransactionMode.Requires)]
        private IList<object[]> GetTracerDetail(IDictionary<string, object> param)
        {
            StringBuilder tracerDetailSql = new StringBuilder();
            tracerDetailSql.Append(@"select  u.Code,ts.TaskSubTypeCode ,ts.TaskSubTypeDesc , ");
            tracerDetailSql.Append(@"        ts.TaskCode,ts.Subject,ts.Desc1,ts.Desc2,ts.AssignUserNm,ts.SubmitUserNm,ts.StartedUser, ");//t.SchedulingStartUser,t.AssignStartUser
            tracerDetailSql.Append(@"        ts.Flag,ts.Color,ts.StatusDesc,ts.CreateDate,ts.CreateUserNm,ts.CommentCreateUserNm,ts.CommentCreateDate, ");
            tracerDetailSql.Append(@"        ts.Comment,ts.ExpectedResults,ts.Priority,ts.AttachmentCount,ts.Phase,ts.Seq,ts.Type,ts.PlanCompleteDate,ts.Status, ");
            tracerDetailSql.Append(@"        ts.CommentCount,ts.RefTaskCount,ts.StatusCount ");
            tracerDetailSql.Append(@"from TaskReport tr,User u, ");
            tracerDetailSql.Append(@"     TaskStatusView ts ");
            tracerDetailSql.Append(@"where tr.TaskSubType = ts.TaskSubTypeCode and tr.IsActive = 1 and tr.User = u.Code ");
            tracerDetailSql.Append(@"and u.Email != '' and u.Email is not null and u.IsActive = 1 ");
            tracerDetailSql.Append(
                @"and ts.CreateDate <= :EndDate and ts.CreateDate>= :StartDate and ts.StatusDesc != '' and ts.StatusDesc is not null ");
            tracerDetailSql.Append(@"and ");
            tracerDetailSql.Append(@"      ( ");
            tracerDetailSql.Append(
                @"          exists (select up.Permission.Code from UserPermission up where up.User.Code = u.Code and up.Permission.Code in (:Permissions)) ");
            tracerDetailSql.Append("      or ");
            tracerDetailSql.Append(
                @"          exists (select rp.Permission.Code from RolePermission rp join rp.Role r,UserRole ur where r.Code = ur.Role.Code and ur.User.Code = u.Code and rp.Permission.Code in (:Permissions))  ");
            tracerDetailSql.Append(@"      ) ");
            tracerDetailSql.Append(@"order by u.Code,ts.TaskSubTypeCode asc,ts.Phase asc,ts.Seq asc,ts.TaskCode asc,ts.CreateDate desc ");

            IList<object[]> tracerDetail = this.hqlMgrE.FindAll<object[]>(tracerDetailSql.ToString(), param);
            return tracerDetail;
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class TaskReportMgrE : com.Sconit.ISI.Service.Impl.TaskReportMgr, ITaskReportMgrE
    {
    }
}

#endregion Extend Class