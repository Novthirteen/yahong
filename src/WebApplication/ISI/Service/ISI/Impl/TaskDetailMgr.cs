using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Entity.MasterData;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class TaskDetailMgr : TaskDetailBaseMgr, ITaskDetailMgr
    {
        public IHqlMgrE hqlMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public void CreateTaskDetail(TaskMstr task, string level, IList<UserSub> userSubList, bool isEmailException, bool isSMSException, User user)
        {
            DateTime now = DateTime.Now;
            foreach (UserSub userSub in userSubList)
            {
                TaskDetail taskDetail = new TaskDetail();
                taskDetail.TaskCode = task.Code;
                taskDetail.Status = task.Status;
                taskDetail.IsEmail = userSub.IsEmail;
                taskDetail.IsSMS = userSub.IsSMS;
                taskDetail.BackYards = task.BackYards;
                taskDetail.TaskSubType = task.TaskSubType != null ? task.TaskSubType.Code : task.TaskSubTypeCode;
                taskDetail.Desc1 = task.Desc1;
                taskDetail.Desc2 = task.Desc2;
                taskDetail.UserName = task.UserName;
                taskDetail.ExpectedResults = task.ExpectedResults;
                taskDetail.Flag = task.Flag;
                taskDetail.Color = task.Color;
                taskDetail.FailureMode = task.FailureMode != null ? task.FailureMode.Code : string.Empty;
                taskDetail.PlanCompleteDate = task.PlanCompleteDate;
                taskDetail.PlanStartDate = task.PlanStartDate;
                taskDetail.Subject = task.Subject;
                taskDetail.UserEmail = task.Email;
                taskDetail.UserMobilePhone = task.MobilePhone;
                taskDetail.CreateDate = now;
                taskDetail.CreateUser = user.Code;
                taskDetail.CreateUserNm = user.Name;
                taskDetail.LastModifyDate = now;
                taskDetail.LastModifyUser = user.Code;
                taskDetail.LastModifyUserNm = user.Name;
                taskDetail.Priority = task.Priority;
                if (isEmailException)
                {
                    taskDetail.EmailStatus = ISIConstants.CODE_MASTER_ISI_SEND_STATUS_FAIL;
                }
                else
                {
                    taskDetail.EmailStatus = taskDetail.IsEmail ? ISIConstants.CODE_MASTER_ISI_SEND_STATUS_SUCCESS : ISIConstants.CODE_MASTER_ISI_SEND_STATUS_NOTSEND;
                }

                if (taskDetail.IsEmail)
                {
                    taskDetail.EmailCount += 1;
                }

                if (isSMSException)
                {
                    taskDetail.SMSStatus = ISIConstants.CODE_MASTER_ISI_SEND_STATUS_FAIL;
                }
                else
                {
                    taskDetail.SMSStatus = taskDetail.IsSMS ? ISIConstants.CODE_MASTER_ISI_SEND_STATUS_SUCCESS : ISIConstants.CODE_MASTER_ISI_SEND_STATUS_NOTSEND;
                }

                if (taskDetail.IsSMS)
                {
                    taskDetail.SMSCount += 1;
                }

                taskDetail.Level = level;
                taskDetail.Email = userSub.Email;
                taskDetail.MobilePhone = userSub.MobilePhone;

                taskDetail.Receiver = userSub.Code;

                taskDetail.IsActive = true;
                this.CreateTaskDetail(taskDetail);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public bool IsSended(string taskCode, string taskSubType, string status, string level)
        {
            /*
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskDetail));
            criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Id")));
            criteria.Add(Expression.Eq("TaskCode", taskCode));
            criteria.Add(Expression.Eq("TaskSubType", taskSubType));
            criteria.Add(Expression.Eq("Level", level));
            criteria.Add(Expression.Eq("Status", status));
            IList<int> count = criteriaMgrE.FindAll<int>(criteria);
            if (count == null || count.Count == 0 || count[0] == 0) return false;
            */
            StringBuilder sql = new StringBuilder();
            sql.Append("select max(td.id) from TaskDetail td ");
            sql.Append("where td.TaskCode=:TaskCode and td.TaskSubType=:TaskSubType and td.Level=:Level and td.Status=:Status ");
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("TaskCode", taskCode);
            param.Add("TaskSubType", taskSubType);
            param.Add("Level", level);
            param.Add("Status", status);
            IList<object> maxIds = hqlMgrE.FindAll<object>(sql.ToString(), param);
            if (maxIds == null || maxIds.Count == 0 || maxIds[0] == null || maxIds[0].ToString() == "0") return false;

            sql = new StringBuilder();
            sql.Append("select distinct td.TaskSubType from TaskDetail td ");
            sql.Append("where td.TaskCode=:TaskCode and td.id >=:Id");
            param = new Dictionary<string, object>();
            param.Add("TaskCode", taskCode);
            param.Add("Id", maxIds[0]);
            IList<string> taskSubTypes = hqlMgrE.FindAll<string>(sql.ToString(), param);
            if (taskSubTypes == null || taskSubTypes.Count <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class TaskDetailMgrE : com.Sconit.ISI.Service.Impl.TaskDetailMgr, ITaskDetailMgrE
    {
    }
}

#endregion Extend Class