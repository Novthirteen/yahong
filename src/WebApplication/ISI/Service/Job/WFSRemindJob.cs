using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Service.Batch;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;

namespace com.Sconit.ISI.Service.Batch.Job
{
    [Transactional]
    public class WFSRemindJob : IJob
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");
        public IHqlMgrE hqlMgrE { get; set; }
        public ITaskMstrMgrE taskMstrMgrE { get; set; }
        public IUserSubscriptionMgrE userSubscriptionMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            SendRemind();
        }


        [Transaction(TransactionMode.RequiresNew)]
        protected virtual void SendRemind()
        {
            try
            {
                DateTime now = DateTime.Now;
                string day1 = now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                StringBuilder hql = new StringBuilder();
                hql.Append(" select p from TaskMstr t ,ProcessInstance p ");
                hql.Append(" where p.Status='" + ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED + "' ");
                hql.Append(" and t.Status in ('" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT + "','" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE + "','" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE + "') ");
                hql.Append(" and t.SubmitDate <= '" + day1 + "' and (t.InApproveDate <= '" + day1 + "' or t.InApproveDate is null) and (t.InDisputeDate <= '" + day1 + "' or t.InDisputeDate is null) ");
                hql.Append(" and p.IsRemind=1 and t.Code=p.TaskCode and t.Level=p.Level and t.Level is not null ");
                hql.Append(" and IsWF=1 and t.Level!=" + ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE);
                hql.Append(" and t.Level> " + ISIConstants.CODE_MASTER_WFS_LEVEL2);
                var processInstanceList = hqlMgrE.FindAll<ProcessInstance>(hql.ToString());
                if (processInstanceList != null && processInstanceList.Count > 0)
                {
                    DateTime day3 = now.AddDays(-3);
                    foreach (var processInstance in processInstanceList)
                    {
                        var task = taskMstrMgrE.CheckAndLoadTaskMstr(processInstance.TaskCode);
                        IList<UserSub> userSubList = new List<UserSub>();
                        userSubscriptionMgrE.GenerateUserSub(task.Type, processInstance.TaskSubType, task.Code, userMgrE.GetMonitorUser(), userSubList, processInstance.UserCode);
                        userSubscriptionMgrE.Remind(task, ISIConstants.ISI_LEVEL_APPROVE2, userSubList, true, userMgrE.GetMonitorUser());

                        if (task.SubmitDate.HasValue)
                        {
                            //超过3天向上报警
                            bool up = false;
                            DateTime date = task.SubmitDate.Value;
                            if (task.SubmitDate.Value > day3)
                            {
                                up = true;
                                date = task.SubmitDate.Value;
                            }
                            if (task.InDisputeDate.HasValue && task.InDisputeDate.Value > day3)
                            {
                                up = true;
                                if (date < task.InDisputeDate.Value) date = task.InDisputeDate.Value;
                            }
                            if (task.InApproveDate.HasValue && task.InApproveDate > day3)
                            {
                                up = true;
                                if (date < task.InApproveDate.Value) date = task.InApproveDate.Value;
                            }
                            if (!up)
                            {
                                var p = hqlMgrE.FindAll<ProcessInstance>("select p from ProcessInstance p where p.TaskCode='" + task.Code + "' and p.Level=" + ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE).FirstOrDefault();
                                userSubList = new List<UserSub>();
                                userSubscriptionMgrE.GenerateUserSub(task.Type, processInstance.TaskSubType, task.Code, userMgrE.GetMonitorUser(), userSubList, p.UserCode);
                                task.HelpContent = "审批人 " + processInstance.UserNm + " 超时审批，已经用时：" + ISIUtil.GetDiff(date, now);
                                userSubscriptionMgrE.Remind(task, ISIConstants.ISI_LEVEL_APPROVEUP, userSubList, userMgrE.GetMonitorUser());
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
    }
}



#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class WFSRemindJob : com.Sconit.ISI.Service.Batch.Job.WFSRemindJob
    {
        public WFSRemindJob()
        {
        }
    }
}

#endregion
