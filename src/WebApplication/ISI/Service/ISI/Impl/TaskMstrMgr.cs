using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.Exception;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Service.Ext;
using System.Linq;
using com.Sconit.Service.Ext.Hql;
using System.Net.Mail;
using NHibernate.Expression;
using NHibernate.Type;
using NHibernate.Transform;
using NHibernate.SqlCommand;
using NHibernate;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Utility;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class TaskMstrMgr : TaskMstrBaseMgr, ITaskMstrMgr
    {
        public IHqlMgrE hqlMgrE { get; set; }
        public ITaskApplyMgrE taskApplyMgrE { get; set; }
        public IWFDetailMgrE wfDetailMgrE { get; set; }
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        #region Customized Methods
        [Transaction(TransactionMode.Unspecified)]
        public TaskMstr CheckAndLoadTaskMstr(string taskCode)
        {
            TaskMstr task = this.LoadTaskMstr(taskCode);
            if (task != null)
            {
                return task;
            }
            else
            {
                throw new BusinessErrorException("ISI.Error.TaskCodeNotExist", taskCode);
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        public TaskMstr CheckAndLoadTaskMstr(string taskCode, bool includeTaskDetail)
        {
            TaskMstr task = this.CheckAndLoadTaskMstr(taskCode);

            if (includeTaskDetail)
            {
                if (task.TaskDetails != null && task.TaskDetails.Count > 0)
                {
                }
            }
            return task;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Task> GetRefTask(string taskCode, int firstRow, int maxRows)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskMstr));
            criteria.Add(Expression.Eq("BackYards", taskCode));
            ICriteria c = criteria.GetExecutableCriteria(this.daoBase.GetSession());
            ProjectionList list = Projections.ProjectionList().Create();
            list.Add(Projections.Distinct(Projections.Property("Code")));
            list.Add(Projections.GroupProperty("Code"), "Code");
            list.Add(Projections.GroupProperty("Subject"), "Subject");
            list.Add(Projections.GroupProperty("Desc1"), "Desc1");
            list.Add(Projections.GroupProperty("Desc2"), "Desc2");
            list.Add(Projections.GroupProperty("CreateDate"), "CreateDate");
            criteria.SetProjection(list);
            criteria.AddOrder(Order.Desc("CreateDate"));
            c.SetFirstResult(firstRow);
            c.SetMaxResults(maxRows);
            c.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Task)));

            return c.List<Task>();
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateTaskStatus(TaskStatus taskStatus, TaskMstr task)
        {
            if (!task.IsWF)
            {
                task.Flag = taskStatus.Flag;
                task.Color = taskStatus.Color;
            }
            task.LastModifyDate = taskStatus.LastModifyDate;
            task.LastModifyUser = taskStatus.LastModifyUser;
            task.LastModifyUserNm = taskStatus.LastModifyUserNm;
            this.UpdateTaskMstr(task);
        }

        [Transaction(TransactionMode.Requires)]
        public override void UpdateTaskMstr(TaskMstr entity)
        {
            UpdateFirstUser(entity);
            base.UpdateTaskMstr(entity);
            this.CreateTaskApply(entity);
        }

        private static void UpdateFirstUser(TaskMstr entity)
        {
            if (!string.IsNullOrEmpty(entity.StartedUser))
            {
                entity.FirstUser = entity.StartedUser.Substring(1, entity.StartedUser.IndexOfAny(new char[] { ',', '|' }, 1) - 1);
                if (entity.AssignStartUserNm.Contains(','))
                {
                    entity.FirstUserNm = entity.AssignStartUserNm.Substring(0, entity.AssignStartUserNm.IndexOfAny(new char[] { ',' }));
                }
                else
                {
                    entity.FirstUserNm = entity.AssignStartUserNm;
                }
            }
            else
            {
                entity.FirstUser = null;
                entity.FirstUserNm = null;
            }
        }


        [Transaction(TransactionMode.Requires)]
        public override void CreateTaskMstr(TaskMstr entity)
        {
            UpdateFirstUser(entity);
            base.CreateTaskMstr(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public TaskMstr LoadTaskMstr(string code, bool includeTaskDetail)
        {
            TaskMstr task = this.LoadTaskMstr(code);

            if (includeTaskDetail)
            {
                if (task.TaskDetails != null && task.TaskDetails.Count > 0)
                {
                }
            }

            return task;
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateTaskMstr(string code, string wiki, User user)
        {
            TaskMstr task = this.CheckAndLoadTaskMstr(code);
            task.Wiki = wiki;
            task.LastModifyDate = DateTime.Now;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name;
            this.UpdateTaskMstr(task);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateTaskMstr(TaskMstr task, User user)
        {
            TaskMstr oldTask = this.CheckAndLoadTaskMstr(task.Code);

            if (oldTask.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE
                && oldTask.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT
                && oldTask.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN
                && oldTask.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS
                && oldTask.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN
                && !(oldTask.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE && oldTask.TaskSubType.IsCostCenter))
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenModify", oldTask.Status,
                                                 oldTask.Code);
            }
            else
            {
                DateTime now = DateTime.Now;

                //更新预算归结todo
                if (oldTask.Account1 != task.Account1 || oldTask.Account2 != task.Account2)
                {

                }

                oldTask.Account1 = task.Account1;
                oldTask.Account1Desc = task.Account1Desc;
                oldTask.Account2 = task.Account2;
                oldTask.Account2Desc = task.Account2Desc;
                oldTask.Voucher = task.Voucher;
                oldTask.PayeeCode = task.PayeeCode;
                oldTask.PayeeName = task.PayeeName;
                oldTask.TravelType = task.TravelType;
                oldTask.Taxes = task.Taxes;
                oldTask.TotalAmount = task.TotalAmount;
                oldTask.Qty = task.Qty;

                oldTask.CostCenterCode = task.CostCenterCode;
                oldTask.CostCenterDesc = task.CostCenterDesc;
                oldTask.WorkHoursUser = task.WorkHoursUser;
                oldTask.WorkHoursUserNm = task.WorkHoursUserNm;
                oldTask.PlanAmount = task.PlanAmount;
                oldTask.Amount = task.Amount;

                oldTask.Priority = task.Priority;
                oldTask.BackYards = task.BackYards;
                oldTask.TaskAddress = task.TaskAddress;
                oldTask.Subject = task.Subject;
                oldTask.Desc1 = task.Desc1;
                if (task.TaskSubType != null && oldTask.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL
                            && oldTask.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE
                            && oldTask.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE)
                //oldTask.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE)
                {
                    oldTask.TaskSubType = task.TaskSubType;
                }
                oldTask.FailureMode = task.FailureMode;
                oldTask.Phase = task.Phase;
                oldTask.Seq = task.Seq;
                oldTask.UserName = task.UserName;
                oldTask.Email = task.Email;
                oldTask.MobilePhone = task.MobilePhone;

                oldTask.ExpectedResults = task.ExpectedResults;
                oldTask.Desc2 = task.Desc2;
                oldTask.PlanStartDate = task.PlanStartDate;
                oldTask.PlanCompleteDate = task.PlanCompleteDate;

                if (!string.IsNullOrEmpty(task.AssignStartUser))
                {
                    oldTask.AssignStartUser = task.AssignStartUser;
                    oldTask.AssignStartUserNm = task.AssignStartUserNm;
                }

                oldTask.LastModifyDate = now;
                oldTask.LastModifyUser = user.Code;
                oldTask.LastModifyUserNm = user.Name;
                this.UpdateTaskMstr(oldTask);

                if (oldTask.IsApply)
                {
                    oldTask.TaskApplyList = task.TaskApplyList;
                    this.UpdateApplay(oldTask, now, user);
                }

                //创建和提交状态更新不跟踪
                if (oldTask.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE
                        && oldTask.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
                {
                    this.wfDetailMgrE.CreateWFDetail(oldTask.Code, oldTask.Status, oldTask.Level, oldTask.PreLevel, now, user);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        protected void CreateTaskApply(TaskMstr task)
        {
            if (task.IsApply && task.TaskApplyList != null && task.TaskApplyList.Count > 0)
            {
                //申请项
                foreach (var taskApply in task.TaskApplyList)
                {
                    taskApply.TaskCode = task.Code;
                    taskApplyMgrE.CreateTaskApply(taskApply);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateApplay(TaskMstr task, DateTime effDate, User user)
        {
            if (task.IsApply)
            {
                //var oldTaskApplyList = taskApplyMgrE.GetTaskApply(task.Code);
                //UpdateApplay(task, oldTaskApplyList, effDate, user);
                if (task.TaskApplyList == null || task.TaskApplyList.Count == 0)
                {
                    return;
                }
                foreach (var taskApply in task.TaskApplyList)
                {
                    taskApplyMgrE.UpdateTaskApply(taskApply);
                }
            }
        }
        /// <summary>
        /// 废弃
        /// </summary>
        /// <param name="task"></param>
        /// <param name="oldTaskApplyList"></param>
        /// <param name="effDate"></param>
        /// <param name="user"></param>
        [Transaction(TransactionMode.Requires)]
        protected void UpdateApplay(TaskMstr task, IList<TaskApply> oldTaskApplyList, DateTime effDate, User user)
        {
            if (task.IsApply)
            {
                /*
                if (task.TaskApplyList == null || task.TaskApplyList.Count == 0)
                {
                    this.taskApplyMgrE.DeleteTaskApply(oldTaskApplyList);
                }
                else
                {
                 */
                foreach (var taskApply in task.TaskApplyList)
                {
                    if (oldTaskApplyList == null || oldTaskApplyList.Count == 0)
                    {
                        taskApply.TaskCode = task.Code;
                        taskApplyMgrE.CreateTaskApply(taskApply);
                    }
                    else
                    {
                        taskApplyMgrE.UpdateTaskApply(taskApply);
                    }
                }
                //}
            }
        }


        [Transaction(TransactionMode.Unspecified)]
        public IDictionary<string, string> GetUser(string userCodes)
        {
            if (string.IsNullOrEmpty(userCodes)) return new Dictionary<string, string>();

            var users = userCodes.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            if (users == null || users.Count() == 0) return new Dictionary<string, string>();
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("UserCode", users);
            IList<object[]> userNameObj = hqlMgrE.FindAll<object[]>("select u.FirstName,u.LastName,u.Code from User u where u.Code in (:UserCode) ", param);

            return userNameObj.Where(u => (u[0] != null || u[1] != null) && u[2] != null).ToDictionary(t => (string)t[2], t => ((t[0] != null && !string.IsNullOrEmpty((string)(t[0])) ? (string)(t[0]) + " " : string.Empty) + (t[1] != null && !string.IsNullOrEmpty((string)(t[1])) ? (string)(t[1]) + " " : string.Empty)));
        }


        [Transaction(TransactionMode.Requires)]
        public IDictionary<string, UserSub> GetUser2(string userCode)
        {
            var userList = GetUser3(userCode);
            if (userList == null || userList.Count == 0) return null;
            return userList.ToDictionary(u => (string)u[0], u => new UserSub() { Code = (string)u[0], Name = (u[1] != null ? (string)u[1] : string.Empty) + (u[2] != null ? (string)u[2] : string.Empty), Email = (string)u[3], IsEmail = true });
        }

        [Transaction(TransactionMode.Requires)]
        public IList<object[]> GetUser3(string userCode)
        {
            string[] userCodes = userCode.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            if (userCodes == null || userCodes.Length == 0) return null;
            StringBuilder userSql = new StringBuilder();
            userSql.Append("select u.Code,u.FirstName,u.LastName,u.Email ");
            userSql.Append("from User u ");
            userSql.Append("where u.Email != '' and u.Email is not null and u.IsActive = 1 ");
            userSql.Append(" and ( ");

            for (int i = 0; i < userCodes.Length; i++)
            {
                var code = userCodes[i];
                if (i != 0) userSql.Append(" or ");
                userSql.Append(" u.Code = '" + code + "' ");
            }
            userSql.Append(" ) ");
            userSql.Append("order by u.Code ");
            //IDictionary<string, object> param = new Dictionary<string, object>();
            //param.Add("UserCode", userCodes);
            return this.hqlMgrE.FindAll<object[]>(userSql.ToString());

        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class TaskMstrMgrE : com.Sconit.ISI.Service.Impl.TaskMstrMgr, ITaskMstrMgrE
    {
    }
}

#endregion Extend Class