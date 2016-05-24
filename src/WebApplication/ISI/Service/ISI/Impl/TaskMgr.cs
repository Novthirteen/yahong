using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Service.Ext;
using System.Linq;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Persistence;
using System.Net.Mail;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.MasterData;
using NHibernate.Expression;
using NHibernate;
using com.Sconit.Entity.Exception;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using com.Sconit.Entity;
using com.Sconit.Service.Ext;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class TaskMgr : ITaskMgr
    {
        #region 变量
        public IWorkDetMgrE workDetMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        public ISqlHelperMgrE sqlHelperMgrE { get; set; }
        public ILanguageMgrE languageMgrE { get; set; }
        public ITaskMstrMgrE taskMstrMgrE { get; set; }
        public IUserRoleMgrE userRoleMgrE { get; set; }
        public ITaskStatusMgrE taskStatusMgrE { get; set; }
        public IWFDetailMgrE wfDetailMgrE { get; set; }
        public ITaskApplyMgrE taskApplyMgrE { get; set; }
        public IProcessInstanceMgrE processInstanceMgrE { get; set; }
        public IWorkflowMgrE workflowMgrE { get; set; }
        public IAttachmentDetailMgrE attachmentDetailMgr { get; set; }
        public ICommentDetailMgrE commentDetailMgrE { get; set; }
        public INumberControlMgrE numberControlMgrE { get; set; }
        public ISmtpMgrE smtpMgrE { get; set; }
        public ITaskDetailMgrE taskDetailMgrE { get; set; }
        public ITaskSubTypeMgrE taskSubTypeMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public ICodeMasterMgrE codeMasterMgrE { get; set; }
        public IUserSubscriptionMgrE userSubscriptionMgrE { get; set; }
        public ISchedulingMgrE schedulingMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }

        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        #endregion

        /// <summary>
        /// 控制是否有操作权限
        /// </summary>
        /// <param name="task"></param>
        /// <param name="isISIAdmin"></param>
        /// <param name="isTaskFlowAdmin"></param>
        /// <param name="isCloser"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Unspecified)]
        public bool HasPermission(TaskMstr task, bool isISIAdmin, bool isTaskFlowAdmin, bool isCloser, string currentUser)
        {
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
            {
                if (isISIAdmin || task.CreateUser == currentUser)
                {
                    return true;
                }
            }

            if (isISIAdmin || isTaskFlowAdmin) return true;

            TaskSubType taskSubType = task.TaskSubType;
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
            {
                //手工分派的执行人
                //排班表的执行人
                //执行上报人
                return HasPermissionByProcess(task.Status, task.StartedUser, taskSubType.StartUpUser, task.CreateUser, task.SubmitUser, isISIAdmin, isTaskFlowAdmin, currentUser);
            }
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
            {
                //手工分派的执行人
                //排班表的执行人
                //执行上报人
                if (ISIUtil.Contains(task.StartedUser, currentUser)
                         || ISIUtil.Contains(taskSubType.StartUpUser, currentUser))
                {
                    return true;
                }
            }
            #region  完成状态
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
            {
                return HasPermissionByComplete(task.Status, task.Type,
                                                        task.CreateUser, task.SubmitUser, task.StartedUser,
                                                        taskSubType.ECUser, taskSubType.AssignUser, taskSubType.AssignUpUser,
                                                        taskSubType.CloseUpUser, taskSubType.CloseUpLevel,
                                                        isISIAdmin, isISIAdmin, isCloser, currentUser);
            }
            #endregion

            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE
                            || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL)
            {
                return HasPermissionByClose(task.Status, task.Type, task.IsWF, task.CreateUser, task.SubmitUser, isISIAdmin, isCloser, currentUser);
            }

            return false;
        }

        /// <summary>
        /// 是否有附件权限
        /// </summary>
        /// <param name="startedUser"></param>
        /// <param name="assignUser"></param>
        /// <param name="assignUpUser"></param>
        /// <param name="isISIAdmin"></param>
        /// <param name="isTaskFlowAdmin"></param>
        /// <param name="isViewer"></param>
        /// <param name="isAssigner"></param>
        /// <param name="isCloser"></param>
        /// <param name="isDeleteAttachment"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Unspecified)]
        public bool HasAttachmentPermission(string startedUser, string assignUser, string assignUpUser, bool isISIAdmin, bool isTaskFlowAdmin, bool isViewer, bool isAssigner, bool isCloser, bool isDeleteAttachment, string currentUser)
        {
            //执行人
            //分派人
            //分派上报人
            //全局权限
            //全局分派
            //全局观察
            //DeleteAttachment
            return (!string.IsNullOrEmpty(startedUser) && ISIUtil.Contains(startedUser, currentUser))
                            || !string.IsNullOrEmpty(assignUser) && ISIUtil.Contains(startedUser, currentUser)
                            || !string.IsNullOrEmpty(assignUpUser) && ISIUtil.Contains(startedUser, currentUser)
                            || isISIAdmin || isTaskFlowAdmin
                            || isViewer || isAssigner || isCloser || isDeleteAttachment;
        }

        /// <summary>
        /// 是否有审批权限
        /// </summary>
        /// <param name="status"></param>
        /// <param name="taskCode"></param>
        /// <param name="level"></param>
        /// <param name="isWFAdmin"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Unspecified)]
        public bool HasProcessPermission(string status, string taskCode, int? level, bool isWFAdmin, string currentUser)
        {
            return ProcessPermission(status, taskCode, level, isWFAdmin, currentUser).IsApprove;
        }

        /// <summary>
        /// 获取前一步的权限
        /// </summary>
        /// <param name="status"></param>
        /// <param name="taskCode"></param>
        /// <param name="preLevel"></param>
        /// <param name="level"></param>
        /// <param name="isWFAdmin"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Unspecified)]
        public WFPermission PreProcessPermission(string status, string taskCode, int? preLevel, int? level, bool isWFAdmin, string currentUser)
        {
            if (!level.HasValue || !preLevel.HasValue || level == preLevel)
            {
                return new WFPermission() { IsApprove = false, IsCtrl = false };
            }
            else
            {
                return ProcessPermission(status, taskCode, preLevel, isWFAdmin, currentUser);
            }
        }

        /// <summary>
        /// 获取权限组
        /// </summary>
        /// <param name="status"></param>
        /// <param name="taskCode"></param>
        /// <param name="level"></param>
        /// <param name="isWFAdmin"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Unspecified)]
        public WFPermission ProcessPermission(string status, string taskCode, int? level, bool isWFAdmin, string currentUser)
        {
            if ((status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT || status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE || status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE) && level.HasValue)
            {
                //var processInstanceList = hqlMgrE.FindAll<ProcessInstance>("from ProcessInstance where TaskCode='" + taskCode + "' and Level =" + level + " and UserCode = '" + currentUser + "' and Status ='" + ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED + "'  order by id asc ");
                var processInstanceList = hqlMgrE.FindAll<ProcessInstance>("from ProcessInstance where TaskCode='" + taskCode + "' and Level =" + level + "  order by id asc ");
                //and p.UserCode ='" + currentUser + "'
                if (processInstanceList != null && processInstanceList.Count > 0 && (processInstanceList.Where(p => p.UserCode == currentUser).Count() > 0 || isWFAdmin))
                {
                    return new WFPermission() { IsApprove = processInstanceList.Where(p => p.IsApprove).Count() > 0, IsCtrl = processInstanceList.Where(p => p.IsCtrl || string.IsNullOrEmpty(p.UserCode)).Count() > 0, Desc1 = processInstanceList[0].Desc1 };
                }
            }
            return new WFPermission() { IsApprove = false, IsCtrl = false };
        }

        [Transaction(TransactionMode.Unspecified)]
        public bool HasPermissionByClose(string status, string type, bool isWF, string createUser, string submitUser, bool isISIAdmin, bool isCloser, string currentUser)
        {
            if (!isWF && (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE
                    || status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL))
            {
                if (isISIAdmin
                            || createUser == currentUser
                            || submitUser == currentUser
                            || (type != ISIConstants.ISI_TASK_TYPE_PRIVACY && isCloser))
                {
                    return true;
                }
            }
            return false;
        }

        [Transaction(TransactionMode.Unspecified)]
        public bool HasPermissionByComplete(string status, string type,
                                                string createUser, string submitUser, string startedUser,
                                                string ecUser, string assignUser, string assignUpUser,
                                                string closeUpUser, string[] closeUpLevel,
                                                bool isISIAdmin, bool isTaskFlowAdmin, bool isCloser, string currentUser)
        {
            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
            {

                if (isISIAdmin || isTaskFlowAdmin) return true;

                //全局关闭人
                if (type != ISIConstants.ISI_TASK_TYPE_PRIVACY && isCloser)
                {
                    return true;
                }

                //项目、项目问题由分派人、分派上报人和关闭上报人关闭
                //工程更改 由工程更改负责人、分派人、分派上报人和关闭上报人关闭
                if (type == ISIConstants.ISI_TASK_TYPE_PROJECT || type == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE
                        || type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
                {
                    if (ISIUtil.Contains(assignUser, currentUser)
                            || ISIUtil.Contains(assignUpUser, currentUser)
                            || ISIUtil.Contains(closeUpUser, currentUser))
                    {
                        return true;
                    }
                    else if (type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE && ISIUtil.Contains(ecUser, currentUser))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //问题、快速响应由提交人、创建人、关闭上报人关闭
                else if (type == ISIConstants.ISI_TASK_TYPE_ISSUE || type == ISIConstants.ISI_TASK_TYPE_RESPONSE)
                {
                    if (createUser == currentUser
                        || submitUser == currentUser
                        || ISIUtil.Contains(closeUpUser, currentUser))
                    {
                        return true;
                    }
                    //问题，非提交人和创建人不能关闭
                    else
                    {
                        return false;
                    }
                }
                else if (ISIUtil.Contains(closeUpUser, currentUser))
                {
                    //分级
                    if (ISIUtil.Contains(closeUpUser, createUser))
                    {
                        string[] closeUpLevelT = closeUpLevel.Where(t => !string.IsNullOrEmpty(t)).ToArray();
                        if (closeUpLevelT == null || closeUpLevelT.Length == 0)
                        {
                            return false;
                        }
                        else if (closeUpLevelT.Length == 1)
                        {
                            return true;
                        }
                        else if (closeUpLevelT.Length > 1)
                        {
                            return true;
                            //不考虑多级
                            /*
                            for (int i = 0; i < closeUpLevelT.Length; i++)
                            {
                                string user = closeUpLevelT[i];
                                if (ISIUtil.Contains(user, task.CreateUser))
                                {
                                    for (int j = i + 1; j < closeUpLevelT.Length; j++)
                                    {
                                        string user2 = closeUpLevelT[j];
                                        if (ISIUtil.Contains(user2, currentUser))
                                        {
                                            return true;
                                        }
                                    }
                                    break;
                                }
                            }
                            */
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                //创建人不是分派人,分派人和分派上报人可以关闭
                else if ((!ISIUtil.Contains(assignUser, startedUser) || String.IsNullOrEmpty(ISIUtil.EditUser(closeUpUser)))
                                && (ISIUtil.Contains(assignUser, currentUser) || ISIUtil.Contains(assignUpUser, currentUser)))
                {
                    return true;
                }
            }
            return false;
        }

        [Transaction(TransactionMode.Unspecified)]
        public bool HasPermissionByProcess(string status, string startedUser, string startUpUser, string createUser, string submitUser, bool isISIAdmin, bool isTaskFlowAdmin, string currentUser)
        {
            //手工分派的执行人
            //排班表的执行人
            //执行上报人
            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN || status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
            {
                if (isISIAdmin || isTaskFlowAdmin) return true;

                if (ISIUtil.Contains(startedUser, currentUser) || ISIUtil.Contains(startUpUser, currentUser) || createUser == currentUser || submitUser == currentUser)
                {
                    return true;
                }
            }
            return false;
        }

        //控制是否有分派权限
        [Transaction(TransactionMode.Unspecified)]
        public bool HasAssignPermission(TaskMstr task, bool isISIAdmin, bool isTaskFlowAdmin, bool isAssigner, string currentUser)
        {
            TaskSubType taskSubType = task.TaskSubType;

            return HasAssignPermission(task.CreateUser, task.SubmitUser, task.Status, task.Type, isISIAdmin, isTaskFlowAdmin, isAssigner, currentUser, taskSubType.ECUser, taskSubType.AssignUser, taskSubType.AssignUpUser, taskSubType.IsAutoAssign, task.IsWF);
        }

        [Transaction(TransactionMode.Unspecified)]
        public bool HasAssignPermission(string createUser, string submitUser, string status, string type,
                                                bool isISIAdmin, bool isTaskFlowAdmin, bool isAssigner, string currentUser, string ecUser,
                                                string assignUser, string assignUpUser, bool isAutoAssign, bool isWF)
        {
            if (isISIAdmin || isTaskFlowAdmin || (type != ISIConstants.ISI_TASK_TYPE_PRIVACY && isAssigner)) return true;

            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE
                    || status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE
                    || status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE
                    || status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
            {
                return false;
            }

            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT
                        || status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN
                        || status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS
                        || status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE)
            {
                if (ISIUtil.Contains(assignUser, currentUser)//分派人
                            || ISIUtil.Contains(assignUpUser, currentUser)//分派上报人
                            || ISIUtil.Contains(ecUser, currentUser)//工程更改负责人
                    //自动分派的分类，提交人（创建人）具有分配的权限
                            || (!isWF && isAutoAssign && (createUser == currentUser || submitUser == currentUser)))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 返回标准格式的用户代码与姓名，兼容角色
        /// </summary>
        /// <param name="assignStartUser">页面上执行人的输入</param>
        /// <returns>返回标准格式的用户代码与姓名</returns>
        [Transaction(TransactionMode.Unspecified)]
        public string[] GetUserCodeName(string assignStartUser)
        {
            string[] userCodeName = ISIUtil.GetUserSplit(assignStartUser);

            string[] roleCodes = userCodeName[0].Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Where(u => u.StartsWith("Role_")).Select(u => u.Replace("Role_", string.Empty)).ToArray();

            IDictionary<string, string> userRoleDic = userRoleMgrE.GetUsersByRoleCode(roleCodes);
            foreach (var role in roleCodes)
            {
                int pos1 = assignStartUser.IndexOf("Role_" + role + "[");
                int pos2 = assignStartUser.IndexOf("]", pos1);
                if (userRoleDic.Keys.Contains(role))
                {
                    assignStartUser = assignStartUser.Replace(assignStartUser.Substring(pos1, pos2 - pos1 + 1), userRoleDic[role]);
                }
                else
                {
                    assignStartUser = assignStartUser.Replace(assignStartUser.Substring(pos1, pos2 - pos1), string.Empty);
                }
            }
            if (userRoleDic != null && userRoleDic.Count > 0)
            {
                userCodeName = ISIUtil.GetUserSplit(assignStartUser);
            }

            return userCodeName;
        }

        [Transaction(TransactionMode.Unspecified)]
        public string GetInvalidUser(string users, string userCode)
        {
            if (string.IsNullOrEmpty(users))
            {
                return string.Empty;
            }

            string[] userArr = users.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            if (userArr == null || userArr.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder userHql = new StringBuilder();
            userHql.Append(@"select u.Code from User u where u.Code in (:UserCodeArray) ");//IsActive = 1 and
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("UserCodeArray", userArr);
            IList<string> userList = this.hqlMgrE.FindAll<string>(userHql.ToString(), param);
            if (userList != null && userList.Count > 0)
            {
                userArr = userArr.Except<string>(userList).ToArray<string>();
            }
            if (userArr == null || userArr.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder invalidUser = new StringBuilder();


            for (int i = 0; i < userArr.Length; i++)
            {
                if (userArr.Length > 1 && i == userArr.Length - 1)
                {
                    invalidUser.Append(languageMgrE.TranslateMessage("ISI.And", userCode));
                }
                else if (i != 0)
                {
                    invalidUser.Append("、");
                }
                invalidUser.Append(userArr[i]);
            }

            return invalidUser.ToString();

        }

        [Transaction(TransactionMode.Requires)]
        public void AssignTask(string taskCode, string backYards, string taskSubTypeCode, string[] assignStartUser, DateTime planStartDate, DateTime planCompleteDate, string desc2, string expectedResults, User user)
        {
            TaskMstr task = this.taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);

            //检查状态
            if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT
                    && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN
                    && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS
                    && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE)
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenAssign", task.Status, task.Code);
            }

            DateTime nowDate = DateTime.Now;
            string oldStatus = task.Status;

            bool isSendRemind = false;
            bool isUserSubSend = false;

            if (!string.IsNullOrEmpty(taskSubTypeCode) && task.TaskSubType.Code != taskSubTypeCode)
            {
                TaskSubType taskSubType = taskSubTypeMgrE.LoadTaskSubType(taskSubTypeCode);
                task.TaskSubType = taskSubType;
                /*
                if (task.FailureMode != null && task.FailureMode.TaskSubType.Code != task.TaskSubType.Code)
                {
                    task.FailureMode = null;
                }
                */
                this.ClearScheduling(task);
                HandleAssign(task, nowDate, user);

                //订阅提醒
                if (user.Code != ISIUtil.EditUser(task.StartedUser))
                {
                    isUserSubSend = true;
                }
            }
            //手工分派时候提醒
            else if (assignStartUser != null && assignStartUser.Length == 2)
            {
                var assignStartUserCode = ISIUtil.GetUser(assignStartUser[0]);
                if (task.AssignStartUser != assignStartUserCode)
                {
                    if (task.AssignStartUser != assignStartUserCode && user.Code != ISIUtil.EditUser(assignStartUserCode))
                    {
                        isSendRemind = true;
                    }
                    task.AssignStartUser = assignStartUserCode;
                    task.AssignStartUserNm = assignStartUser[1];
                    task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN;
                }
            }
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
            {
                task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN;
                isSendRemind = true;
            }
            this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, nowDate, user);
            task.BackYards = backYards;
            task.Desc2 = desc2;
            task.ExpectedResults = expectedResults;
            task.PlanStartDate = planStartDate;
            task.PlanCompleteDate = planCompleteDate;

            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
            {
                task.AssignDate = nowDate;
                task.AssignUser = user.Code;
                task.AssignUserNm = user.Name.Trim();

                task.StartDate = null;
                task.StartUser = string.Empty;
                task.StartUserNm = string.Empty;
            }

            task.LastModifyDate = nowDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name.Trim();
            taskMstrMgrE.UpdateTaskMstr(task);

            if (task.TaskSubType.IsAutoStart)
            {
                this.ConfirmTask(taskCode, user);
            }

            if (!(task.TaskSubType.IsAutoStart && task.TaskSubType.IsAutoStatus))
            {
                List<UserSub> userSubList = new List<UserSub>();
                if (isUserSubSend)
                {
                    //订阅提醒
                    IList<UserSub> userSubListT = userSubscriptionMgrE.SubmitUserSub(task, user);

                    if (userSubListT != null && userSubListT.Count > 0)
                    {
                        userSubList.AddRange(userSubListT);
                    }
                }

                if (isSendRemind)
                {
                    //分派提醒
                    IList<UserSub> userSubListT = userSubscriptionMgrE.GenerateUserSub(task, task.StartedUser, false, user);
                    if (userSubListT != null && userSubListT.Count > 0)
                    {
                        userSubList.AddRange(userSubListT);
                    }
                }
                if (userSubList != null && userSubList.Count > 0)
                {
                    userSubscriptionMgrE.Remind(task, userSubList, user);
                }
            }
        }
        /// <summary>
        /// 仅适用于提交状态的任务，计划完成时间、计划结束时间
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="user"></param>
        [Transaction(TransactionMode.Requires)]
        public string AssignTask(string taskCode, User user)
        {
            TaskMstr task = this.taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);

            //检查状态
            if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenAssign", task.Status, task.Code);
            }

            DateTime nowDate = DateTime.Now;
            string oldStatus = task.Status;

            bool isSendRemind = false;

            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
            {
                task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN;
                isSendRemind = true;
            }
            this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, nowDate, user);

            if (string.IsNullOrEmpty(task.StartedUser))
            {
                task.AssignStartUser = ISIUtil.GetUser(task.SubmitUser);
                task.AssignStartUserNm = task.SubmitUserNm;
            }
            if (!task.PlanStartDate.HasValue)
            {
                task.PlanStartDate = nowDate;

            }
            if (!task.PlanCompleteDate.HasValue)
            {
                task.PlanCompleteDate = this.GetPlanCompleteDate(task.TaskSubType, task.PlanStartDate.Value);
            }

            task.AssignDate = nowDate;
            task.AssignUser = user.Code;
            task.AssignUserNm = user.Name.Trim();

            task.LastModifyDate = nowDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name.Trim();
            taskMstrMgrE.UpdateTaskMstr(task);

            if (task.TaskSubType.IsAutoStart)
            {
                this.ConfirmTask(taskCode, user);
            }

            if (!(task.TaskSubType.IsAutoStart && task.TaskSubType.IsAutoStatus))
            {
                List<UserSub> userSubList = new List<UserSub>();

                if (isSendRemind)
                {
                    //分派提醒
                    IList<UserSub> userSubListT = userSubscriptionMgrE.GenerateUserSub(task, task.StartedUser, false, user);
                    if (userSubListT != null && userSubListT.Count > 0)
                    {
                        userSubList.AddRange(userSubListT);
                    }
                }
                if (userSubList != null && userSubList.Count > 0)
                {
                    userSubscriptionMgrE.Remind(task, userSubList, user);
                }
            }

            return task.Status;
        }


        /*
        [Transaction(TransactionMode.Requires)]
        public void ReassignTask(string taskCode, string assignStartUser, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);

            //检查状态
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE
                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT
                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL
                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE)
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenReassign", task.Status, task.Code);
            }

            DateTime nowDate = DateTime.Now;

            this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REASSIGN, nowDate, user);
            task.AssignStartUser = assignStartUser;
            task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REASSIGN;
            task.ReassignDate = nowDate;
            task.ReassignUser = user;
            task.LastModifyDate = nowDate;
            task.LastModifyUser = user;

            taskMstrMgrE.UpdateTaskMstr(task);

            //重新分派提醒
            IList<UserSub> userSubList = userSubscriptionMgrE.GenerateUserSub(task, task.AssignStartUser);
            Remind(task, userSubList, user);
        }
*/
        [Transaction(TransactionMode.Requires)]
        public void CompleteTask(string taskCode, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);
            this.CompleteTask(task, user);
        }
        [Transaction(TransactionMode.Requires)]
        public void CompleteTask(TaskMstr task, User user)
        {
            //检查状态
            if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenComplete", task.Status, task.Code);
            }

            DateTime nowDate = DateTime.Now;

            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI4;
            task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE;
            this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, nowDate, user);
            task.CompleteDate = nowDate;
            task.CompleteUser = user.Code;
            task.CompleteUserNm = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name.Trim();
            taskMstrMgrE.UpdateTaskMstr(task);

            if (task.TaskSubType.IsAutoClose)
            {
                this.CloseTask(task.Code, user);
            }
            else if (!task.IsCompleteNoRemind)
            {
                //提醒关闭
                IList<UserSub> userSubList = userSubscriptionMgrE.GenerateUserSub(task, task.CreateUser + ISIConstants.ISI_USER_SEPRATOR + task.SubmitUser + ISIConstants.ISI_USER_SEPRATOR + task.AssignUser, false, user);
                userSubscriptionMgrE.Remind(task, userSubList, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CompleteTask(string taskCode, string desc2, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);

            //检查状态
            if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenComplete", task.Status, task.Code);
            }

            DateTime nowDate = DateTime.Now;

            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI4;
            task.Desc2 = desc2;
            task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE;
            task.CompleteDate = nowDate;
            task.CompleteUser = user.Code;
            task.CompleteUserNm = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name.Trim();
            this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, nowDate, user);


            taskMstrMgrE.UpdateTaskMstr(task);

            if (!task.IsCompleteNoRemind)
            {
                //提醒关闭
                IList<UserSub> userSubList = userSubscriptionMgrE.GenerateUserSub(task, task.CreateUser + ISIConstants.ISI_USER_SEPRATOR + task.SubmitUser + ISIConstants.ISI_USER_SEPRATOR + task.AssignUser, false, user);
                userSubscriptionMgrE.Remind(task, userSubList, user);
            }
            if (task.TaskSubType.IsAutoClose)
            {
                this.CloseTask(taskCode, user);
            }
        }
        /// <summary>
        /// 任务开始动作
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="user"></param>
        [Transaction(TransactionMode.Requires)]
        public void ConfirmTask(string taskCode, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);
            ConfirmTask(task, user);
        }

        /// <summary>
        /// 任务开始动作
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="user"></param>
        [Transaction(TransactionMode.Requires)]
        public void ConfirmTask(TaskMstr task, User user)
        {
            //检查状态
            if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenConfirm", task.Status, task.Code);
            }

            DateTime nowDate = DateTime.Now;

            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI2;

            task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS;
            task.StartDate = nowDate;
            task.StartUser = user.Code;
            task.StartUserNm = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name.Trim();
            this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, nowDate, user);
            taskMstrMgrE.UpdateTaskMstr(task);

            if (task.TaskSubType.IsAutoStatus)
            {
                this.CompleteTask(task, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void ConfirmTask(string taskCode, DateTime planStartDate, DateTime planCompleteDate, string desc2, string expectedResults, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);

            //检查状态
            if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenConfirm", task.Status, task.Code);
            }

            DateTime nowDate = DateTime.Now;



            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI2;

            task.Desc2 = desc2;
            task.ExpectedResults = expectedResults;
            task.PlanStartDate = planStartDate;
            task.PlanCompleteDate = planCompleteDate;
            task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS;
            task.StartDate = nowDate;
            task.StartUser = user.Code;
            task.StartUserNm = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name.Trim();
            this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, nowDate, user);
            taskMstrMgrE.UpdateTaskMstr(task);

            if (task.TaskSubType.IsAutoStatus)
            {
                this.CompleteTask(taskCode, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseTask(string taskCode, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);

            //检查状态
            if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenClose", task.Status, task.Code);
            }

            DateTime nowDate = DateTime.Now;

            task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE;
            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI5;
            task.Color = string.Empty;

            task.CloseDate = nowDate;
            task.CloseUser = user.Code;
            task.CloseUserNm = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name.Trim();
            this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, nowDate, user);
            taskMstrMgrE.UpdateTaskMstr(task);
        }

        [Transaction(TransactionMode.Requires)]
        public string RejectTask(string taskCode, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);

            //检查状态
            if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenReject", task.Status, task.Code);
            }

            DateTime nowDate = DateTime.Now;

            task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS;
            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI3;
            //task.Color = string.Empty;

            task.StartDate = nowDate;
            task.StartUser = user.Code;
            task.StartUserNm = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name.Trim();

            task.RejectDate = nowDate;
            task.RejectUser = user.Code;
            task.RejectUserNm = user.Name.Trim();

            //task.CompleteDate = null;
            //task.CompleteUser = string.Empty;
            //task.CompleteUserNm = string.Empty;
            this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, nowDate, user);
            taskMstrMgrE.UpdateTaskMstr(task);

            return task.Status;
        }

        [Transaction(TransactionMode.Requires)]
        public string OpenTask(string taskCode, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);

            //检查状态
            if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE
                    && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL)
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenOpen", task.Status, task.Code);
            }

            DateTime nowDate = DateTime.Now;
            //取消变为提交状态
            string targetStatus = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT;
            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI2;
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE)
            {
                //关闭变为执行中状态
                targetStatus = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS;
                task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI2;
                task.CompleteDate = null;
                task.CompleteUser = string.Empty;
                task.CompleteUserNm = string.Empty;
                task.CloseDate = null;
                task.CloseUser = string.Empty;
                task.CloseUserNm = string.Empty;
            }
            else
            {
                task.CancelDate = null;
                task.CancelUser = string.Empty;
                task.CancelUserNm = string.Empty;
            }
            task.Status = targetStatus;

            //task.Color = string.Empty;

            task.OpenDate = nowDate;
            task.OpenUser = user.Code;
            task.OpenUserNm = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name.Trim();
            this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, nowDate, user);
            taskMstrMgrE.UpdateTaskMstr(task);

            return task.Status;
        }

        [Transaction(TransactionMode.Requires)]
        public int BatchTask(IList<string> taskList, string create, bool isCancl, bool isComplete, string complete, bool isOpen, User user)
        {
            int count = 0;
            DateTime now = DateTime.Now;
            foreach (var taskCode in taskList)
            {
                var task = this.taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);
                if (create != string.Empty && task.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CREATE)
                {
                    if (create == "IsDelete")
                    {
                        this.DeleteTask(taskCode, user);
                    }
                    else
                    {
                        this.SubmitTask(taskCode, user);
                    }
                    count++;
                    continue;
                }
                if (isCancl && (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN))
                {
                    this.CancelTask(taskCode, user);
                    count++;
                    continue;
                }
                if (isComplete && task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
                {
                    this.CompleteTask(taskCode, user);
                    count++;
                    continue;
                }
                if (complete != string.Empty && task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                {
                    if (complete == "IsClose")
                    {
                        this.CloseTask(taskCode, user);
                    }
                    else
                    {
                        this.RejectTask(taskCode, user);
                    }
                    count++;
                    continue;
                }
                if (isOpen && task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE)
                {
                    this.OpenTask(taskCode, user);
                    count++;
                    continue;
                }
            }
            return count;
        }

        [Transaction(TransactionMode.Requires)]
        public int BatchTask(string type, IList<TaskMstr> taskList, string taskSubTypeCode, string projectSubType, User user)
        {
            if (taskList != null && taskList.Count > 0)
            {
                TaskSubType taskSubType = this.taskSubTypeMgrE.LoadTaskSubType(taskSubTypeCode);

                if (type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)// && taskSubType.IsEC
                {
                    //20140919 徐娟需求，工程更改允许重复导入
                    //taskSubType.IsEC = false;
                }
                else
                {

                    if (type == ISIConstants.ISI_TASK_TYPE_PROJECT
                               && projectSubType == ISIConstants.CODE_MASTER_ISI_PROJECTSUBTYPE_QUOTE
                               && taskSubType.IsQuote == true)
                    {
                        taskSubType.IsQuote = false;
                    }
                    else if (type == ISIConstants.ISI_TASK_TYPE_PROJECT
                                && projectSubType == ISIConstants.CODE_MASTER_ISI_PROJECTSUBTYPE_INITIATION
                                && taskSubType.IsInitiation == true)
                    {
                        taskSubType.IsInitiation = false;
                    }
                    else
                    {
                        throw new BusinessErrorException("ISI.TSK.Batch.NoImportProjectTask");
                    }

                    taskSubType.LastModifyDate = DateTime.Now;
                    taskSubType.LastModifyUser = user.Code;
                    taskSubTypeMgrE.UpdateTaskSubType(taskSubType);
                }

                foreach (var task in taskList)
                {
                    task.Priority = ISIConstants.CODE_MASTER_ISI_PRIORITY_NORMAL;
                    task.TaskSubType = taskSubType;
                    task.IsNoSend = true;
                    this.CreateTask(task, user);
                }
                return taskList.Count;
            }
            else
            {
                return 0;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public int BatchDeleteTask(IList<string> taskList, User user)
        {
            int count = 0;
            foreach (var task in taskList)
            {
                this.DeleteTask(task, user);
                count++;
            }
            return count;
        }

        [Transaction(TransactionMode.Requires)]
        public int BatchRejectTask(IList<string> taskList, User user)
        {
            int count = 0;
            foreach (var task in taskList)
            {
                this.RejectTask(task, user);
                count++;
            }
            return count;
        }
        [Transaction(TransactionMode.Requires)]
        public int BatchCloseTask(IList<string> taskList, User user)
        {
            int count = 0;
            foreach (var task in taskList)
            {
                this.CloseTask(task, user);
                count++;
            }
            return count;
        }
        [Transaction(TransactionMode.Requires)]
        public int BatchOpenTask(IList<string> taskList, User user)
        {
            int count = 0;
            foreach (var task in taskList)
            {
                this.OpenTask(task, user);
                count++;
            }
            return count;
        }
        [Transaction(TransactionMode.Requires)]
        public int BatchCompleteTask(IList<string> taskList, User user)
        {
            int count = 0;
            foreach (var task in taskList)
            {
                this.CompleteTask(task, user);
                count++;
            }
            return count;
        }
        [Transaction(TransactionMode.Requires)]
        public int BatchSubmitTask(IList<string> taskList, User user)
        {
            int count = 0;
            foreach (var task in taskList)
            {
                this.SubmitTask(task, user);
                count++;
            }
            return count;
        }
        [Transaction(TransactionMode.Requires)]
        public int BatchCancelTask(IList<string> taskList, User user)
        {
            int count = 0;
            foreach (var task in taskList)
            {
                this.CancelTask(task, user);
                count++;
            }
            return count;
        }

        [Transaction(TransactionMode.Requires)]
        public int BatchReplaceTask(IList<TaskMstr> taskList, string oldUserCode, string newUserCode, User user)
        {
            int count = 0;
            DateTime now = DateTime.Now;
            User oldUser = this.userMgrE.CheckAndLoadUser(oldUserCode);
            User newUser = this.userMgrE.CheckAndLoadUser(newUserCode);
            foreach (var task in taskList)
            {
                if (!string.IsNullOrEmpty(task.StartedUser))
                {
                    string assignStartUser = task.StartedUser;

                    int p1 = assignStartUser.IndexOf(oldUserCode);
                    if (p1 != -1)
                    {
                        bool isContainsNewUser = false;
                        for (int i = 1; i <= 4; i++)
                        {
                            if (assignStartUser.Contains(ISIUtil.FormatUser(newUserCode, i)))
                            {
                                isContainsNewUser = true;
                                break;
                            }
                        }

                        for (int i = 1; i <= 4; i++)
                        {
                            //已经包含了目标用户
                            if (isContainsNewUser)
                            {
                                string newAssignStartUser = assignStartUser.Replace(ISIUtil.FormatUser(oldUserCode, i), ISIUtil.FormatUser(string.Empty, i));
                                newAssignStartUser = newAssignStartUser.Replace("|,", "|").Replace(",|", "|").Replace(",,", ",");
                                if (newAssignStartUser != assignStartUser)
                                {
                                    task.AssignStartUser = newAssignStartUser;
                                    if (!string.IsNullOrEmpty(task.AssignStartUserNm))
                                    {
                                        task.AssignStartUserNm = task.AssignStartUserNm.Replace(oldUser.Name + ", ", string.Empty).Replace(", " + oldUser.Name, string.Empty);
                                    }
                                    else
                                    {
                                        task.AssignStartUserNm = userSubscriptionMgrE.GetUserName(task.AssignStartUser);
                                    }
                                    task.LastModifyDate = now;
                                    task.LastModifyUser = user.Code;
                                    task.LastModifyUserNm = user.Name.Trim();
                                    this.taskMstrMgrE.UpdateTaskMstr(task);
                                    count++;
                                    break;
                                }
                            }
                            else if (assignStartUser.Contains(ISIUtil.FormatUser(oldUserCode, i)))
                            {
                                string newAssignStartUser = assignStartUser.Replace(ISIUtil.FormatUser(oldUserCode, i), ISIUtil.FormatUser(newUserCode, i));
                                if (newAssignStartUser != assignStartUser)
                                {
                                    task.AssignStartUser = newAssignStartUser;
                                    if (!string.IsNullOrEmpty(task.AssignStartUserNm))
                                    {
                                        task.AssignStartUserNm = task.AssignStartUserNm.Replace(oldUser.Name, newUser.Name);
                                    }
                                    else
                                    {
                                        task.AssignStartUserNm = userSubscriptionMgrE.GetUserName(task.AssignStartUser);
                                    }
                                    task.LastModifyDate = now;
                                    task.LastModifyUser = user.Code;
                                    task.LastModifyUserNm = user.Name.Trim();
                                    this.taskMstrMgrE.UpdateTaskMstr(task);
                                    count++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return count;
        }

        [Transaction(TransactionMode.Requires)]
        public string CancelTask(string code, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(code);
            string srcStatus = task.Status;

            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT
                        || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN
                        || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
            {
                DateTime nowDate = DateTime.Now;
                task.Level = ISIConstants.CODE_MASTER_WFS_LEVEL_DEFAULT;
                task.PreLevel = ISIConstants.CODE_MASTER_WFS_LEVEL_DEFAULT;
                task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL;
                task.CancelDate = nowDate;
                task.CancelUser = user.Code;
                task.CancelUserNm = user.Name.Trim();
                this.wfDetailMgrE.CreateWFDetail(task.Code, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL, task.Level, task.PreLevel, nowDate, user);

                task.LastModifyDate = nowDate;
                task.LastModifyUser = user.Code;
                task.LastModifyUserNm = user.Name.Trim();
                taskMstrMgrE.UpdateTaskMstr(task);
            }
            else
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenCancel", task.Status, code);
            }
            return task.Status;
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteTask(string code, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(code);

            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
            {
                IList<TaskDetail> taskDetails = task.TaskDetails;
                if (taskDetails != null && taskDetails.Count > 0)
                {
                    this.taskDetailMgrE.DeleteTaskDetail(taskDetails);
                }

                //删除状态
                DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskStatus));
                criteria.Add(Expression.Eq("TaskCode", code));
                IList<TaskStatus> taskStatusList = this.criteriaMgrE.FindAll<TaskStatus>(criteria);
                if (taskStatusList != null && taskStatusList.Count > 0)
                {
                    this.taskStatusMgrE.DeleteTaskStatus(taskStatusList);
                }

                //流程
                criteria = DetachedCriteria.For(typeof(WFDetail));
                criteria.Add(Expression.Eq("TaskCode", code));
                IList<WFDetail> wfDetailList = this.criteriaMgrE.FindAll<WFDetail>(criteria);
                if (wfDetailList != null && wfDetailList.Count > 0)
                {
                    this.wfDetailMgrE.DeleteWFDetail(wfDetailList);
                }

                //删除评论
                criteria = DetachedCriteria.For(typeof(CommentDetail));
                criteria.Add(Expression.Eq("TaskCode", code));
                IList<CommentDetail> commentDetailList = this.criteriaMgrE.FindAll<CommentDetail>(criteria);
                if (commentDetailList != null && commentDetailList.Count > 0)
                {
                    this.commentDetailMgrE.DeleteCommentDetail(commentDetailList);
                }

                if (task.IsWF)
                {
                    criteria = DetachedCriteria.For(typeof(ProcessInstance));
                    criteria.Add(Expression.Eq("TaskCode", code));
                    IList<ProcessInstance> processInstanceList = this.criteriaMgrE.FindAll<ProcessInstance>(criteria);
                    if (processInstanceList != null && processInstanceList.Count > 0)
                    {
                        this.processInstanceMgrE.DeleteProcessInstance(processInstanceList);
                    }
                }

                if (task.IsApply)
                {
                    criteria = DetachedCriteria.For(typeof(TaskApply));
                    criteria.Add(Expression.Eq("TaskCode", code));
                    IList<TaskApply> taskApplyList = this.criteriaMgrE.FindAll<TaskApply>(criteria);
                    if (taskApplyList != null && taskApplyList.Count > 0)
                    {
                        this.taskApplyMgrE.DeleteTaskApply(taskApplyList);
                    }
                }


                //暂不删除附件


                //this.wfDetailMgrE.CreateWFDetail(code, task.Status, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_DELETE, DateTime.Now, user);

                //删除TaskMstr
                taskMstrMgrE.DeleteTaskMstr(code);
            }
            else
            {
                throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenDelete", task.Status, code);
            }
        }
        [Transaction(TransactionMode.Requires)]
        public TaskMstr ProcessByEmail(string taskCode, string wfsStatus, string approveDesc, bool isiAdmin, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);
            return ProcessNew(task, wfsStatus, approveDesc, string.Empty, null, isiAdmin, true, user);
        }

        [Transaction(TransactionMode.Requires)]
        public TaskMstr ProcessNew(string taskCode, string wfsStatus, string approveDesc, string color, bool isiAdmin, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);
            return ProcessNew(task, wfsStatus, approveDesc, color, null, isiAdmin, false, user);

        }

        [Transaction(TransactionMode.Requires)]
        public TaskMstr ProcessNew(string taskCode, string wfsStatus, string approveDesc, string color, IList<object> countersignList, bool isiAdmin, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);
            return ProcessNew(task, wfsStatus, approveDesc, color, countersignList, isiAdmin, false, user);
        }

        [Transaction(TransactionMode.Requires)]
        public TaskMstr ProcessNew(TaskMstr task, string wfsStatus, string approveDesc, string color, IList<object> countersignList, bool isiAdmin, bool isEmail, User user)
        {
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT
                        || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE
                        || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE)
            {
                if (task.IsWF && task.Status != wfsStatus)
                {
                    DateTime now = DateTime.Now;
                    workflowMgrE.ProcessNew(task, wfsStatus, approveDesc, color, countersignList, isiAdmin, now, isEmail, user);
                    if (task.IsUpdate)
                    {
                        if (task.Level == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE && wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE)
                        {
                            //自动分派
                            HandleAssign(task, now, user);

                            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
                            {
                                if (!(task.TaskSubType.IsAutoStart && task.TaskSubType.IsAutoComplete))
                                {
                                    var userSubList = userSubscriptionMgrE.SubmitUserSub(task, user);
                                    userSubscriptionMgrE.Remind(task, userSubList, user);
                                }
                                if (task.TaskSubType.IsAutoStart)
                                {
                                    this.ConfirmTask(task, user);
                                }
                            }
                        }
                        this.taskMstrMgrE.UpdateTaskMstr(task);
                    }
                    else if (isEmail)
                    {
                        throw new BusinessErrorException("ISI.WF.Error.Approveed", task.Code, task.Status);
                    }
                }
            }
            else
            {
                throw new BusinessErrorException("ISI.WF.Error.StatusErrorWhenApprove", task.Status, task.Code);
            }
            return task;
        }

        [Transaction(TransactionMode.Requires)]
        public TaskMstr SubmitTask(string code, string userCode)
        {
            User user = this.userMgrE.CheckAndLoadUser(userCode);
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(code);
            return this.SubmitTask(task, user);
        }

        [Transaction(TransactionMode.Requires)]
        public TaskMstr SubmitTask(string code, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(code);
            return this.SubmitTask(task, user);
        }


        [Transaction(TransactionMode.Requires)]
        public TaskMstr SubmitTask(TaskMstr task, User user)
        {
            try
            {
                if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
                {
                    throw new BusinessErrorException("ISI.Error." + task.Type + "StatusErrorWhenSubmit", task.Status, task.Code);
                }
                bool isProcess = false;
                DateTime now = DateTime.Now;

                if (task.TaskSubType.IsRemoveForm)
                {
                    var taskApplyList = hqlMgrE.FindAll<TaskApply>("from TaskApply where TaskCode='" + task.Code + "' and Qty is null and (Value ='' or Value is null) and DateValue is null and (Checked = 0 or Checked is null) ");
                    if (taskApplyList != null && taskApplyList.Count > 0)
                    {
                        this.taskApplyMgrE.DeleteTaskApply(taskApplyList);
                    }
                }

                if (!task.IsAutoRelease || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
                {
                    taskMstrMgrE.UpdateApplay(task, now, user);

                    if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
                    {
                        isProcess = true;
                        //task.Level = this.Process(task, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE, null, user);
                        task.Level = ISIConstants.CODE_MASTER_WFS_LEVEL3;
                        bool isWFAdmin = user.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN);
                        workflowMgrE.ProcessNew(task, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE, string.Empty, string.Empty, null, isWFAdmin, now, false, user);
                    }
                }

                task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT;
                task.SubmitDate = now;
                task.SubmitUser = user.Code;
                task.SubmitUserNm = user.Name.Trim();
                task.LastModifyDate = now;
                task.LastModifyUser = user.Code;
                task.LastModifyUserNm = user.Name.Trim();

                this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, now, user);

                IList<UserSub> userSubList = null;

                if (task.IsWF)
                {
                    if (!isProcess)
                    {
                        string assignUser = string.Empty;
                        if (task.TaskSubType.IsAssignUser && !string.IsNullOrEmpty(user.CostCenter))
                        {
                            assignUser = hqlMgrE.FindAll<string>("select tst.AssignUser from TaskSubType tst where tst.Code='" + user.CostCenter + "'").FirstOrDefault();
                        }
                        workflowMgrE.StartProcessInstance(task, assignUser, now, user);
                    }
                    if (task.Level > ISIConstants.CODE_MASTER_WFS_LEVEL3)
                    {
                        task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE;
                        task.InApproveDate = now;
                        task.InApproveUser = user.Code;
                        task.InApproveUserNm = user.Name;
                    }
                    if (task.Level == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
                    {
                        task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE;
                        task.PreLevel = ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE;
                        task.ApproveDate = now;
                        task.ApproveUser = user.Code;
                        task.ApproveUserNm = user.Name;
                    }
                    taskMstrMgrE.UpdateTaskMstr(task);
                    userSubList = userSubscriptionMgrE.SubmitUserSub(task, user);
                }

                if (!task.IsWF || task.IsWF && task.IsTrace && task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE)
                {
                    HandleAssign(task, now, user);

                    taskMstrMgrE.UpdateTaskMstr(task);

                    if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
                    {
                        if (!(task.TaskSubType.IsAutoStart && task.TaskSubType.IsAutoComplete))
                        {
                            userSubList = userSubscriptionMgrE.SubmitUserSub(task, user);
                        }
                        if (task.TaskSubType.IsAutoStart)
                        {
                            this.ConfirmTask(task.Code, user);
                        }
                    }
                    else if ((!task.IsNoSend
                                    && !ISIUtil.Contains(task.TaskSubType.AssignUser, user.Code)//分派人
                                    && !ISIUtil.Contains(task.TaskSubType.AssignUpUser, user.Code)
                        //&& (task.TaskSubType.Parent != null && !ISIUtil.Contains(task.TaskSubType.Parent.AssignUser, user.Code)
                        //                    && !ISIUtil.Contains(task.TaskSubType.Parent.AssignUpUser, user.Code))
                                    && !user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                        ))
                    {
                        if (task.Type != ISIConstants.ISI_TASK_TYPE_PRIVACY
                                    && user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN)
                                    && user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)) return task;

                        userSubList = userSubscriptionMgrE.SubmitUserSub(task, user);

                        #region 处理发送用户

                        if (userSubList == null || userSubList.Count == 0)
                        {
                            log.Warn("Code" + task.Code + ",Not found user subscription!");
                            return task;
                        }

                        #endregion
                    }
                }

                userSubscriptionMgrE.Remind(task, userSubList, user);
            }
            catch (Exception e)
            {
                log.Error("Code=" + task.Code + ",e=" + e.Message, e);
            }
            return task;
        }



        /*
        [Transaction(TransactionMode.Requires)]
        public int? StartProcessInstance(string taskCode, DateTime effDate, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);
            task.Level = workflowMgrE.StartProcessInstance(task, effDate, user);
            if (task.Level == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE && (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUSPEND || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE))
            {
                task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE;
            }
            else if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
            {
                task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE;
            }

            task.LastModifyDate = effDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name;

            this.taskMstrMgrE.UpdateTaskMstr(task);

            return task.Level;
        }
        */
        [Transaction(TransactionMode.Requires)]
        public void HelpTask(string taskCode, string helpUser, bool isRemindCreateUser,
                                                            bool isRemindAssignUser,
                                                            bool isRemindStartUser,
                                                            bool isRemindCommentUser,
                                                            bool isRemindAdmin, string helpContent, User user)
        {
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(taskCode);
            #region 获取用户

            StringBuilder users = new StringBuilder();

            if (!string.IsNullOrEmpty(helpUser))
            {
                users.Append(ISIConstants.ISI_USER_SEPRATOR);
                users.Append(helpUser);
            }

            if (isRemindCreateUser && !string.IsNullOrEmpty(task.SubmitUser))
            {
                users.Append(task.CreateUser);
                users.Append(ISIConstants.ISI_USER_SEPRATOR);
                users.Append(task.SubmitUser);
            }
            /*if (!string.IsNullOrEmpty(task.StartUser))
            {
                users.Append(ISIConstants.ISI_USER_SEPRATOR);
                users.Append(task.StartUser);
            }*/
            if (isRemindStartUser && !string.IsNullOrEmpty(task.StartedUser))
            {
                users.Append(ISIConstants.ISI_USER_SEPRATOR);
                users.Append(task.StartedUser);
            }
            if (isRemindAssignUser)
            {
                if (!string.IsNullOrEmpty(task.AssignUser))
                {
                    users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    users.Append(task.AssignUser);
                }
                else if (!string.IsNullOrEmpty(task.TaskSubType.AssignUser))
                {
                    users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    users.Append(task.TaskSubType.AssignUser);
                }
            }
            if (isRemindCommentUser)
            {
                var taskStatusList = this.taskStatusMgrE.GetTaskStatus(task.Code);
                if (taskStatusList != null && taskStatusList.Count > 0)
                {
                    task.TaskStatus = taskStatusList[0];

                    string statusUsers = string.Join(";", taskStatusList.Select(t => t.LastModifyUser).Distinct().ToArray<string>());
                    if (users.Length != 0)
                    {
                        users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    }
                    users.Append(statusUsers);
                }
            }

            IList<UserSub> userSubList = userSubscriptionMgrE.GenerateUserSub(task, users.ToString(), false, user);

            if (isRemindAdmin)
            {
                //查询所有管理员和流程管理员            
                IList<UserSub> userAdminSubList = userSubscriptionMgrE.GenerateUserSub(task.TaskSubType.Code, user);

                if (userSubList == null)
                {
                    userSubList = userAdminSubList;
                }
                else
                {
                    foreach (var admin in userAdminSubList)
                    {
                        if (!userSubList.Select(u => u.Code).Contains(admin.Code))
                        {
                            userSubList.Add(admin);
                        }
                    }
                }
            }

            #endregion

            if (userSubList != null && userSubList.Count > 0)
            {
                //发短信和邮件给他们
                userSubscriptionMgrE.Remind(task, userSubList, helpContent, user);
            }
        }

        private void HandleAssign(TaskMstr task, DateTime now, User user)
        {
            if (string.IsNullOrEmpty(task.AssignStartUser))
            {
                IList<SchedulingView> schedulingViewList = schedulingMgrE.GetScheduling2(now.Date, now.Date, task.TaskSubType.Code, string.Empty);

                if (schedulingViewList != null && schedulingViewList.Count > 0)
                {
                    SchedulingView schedulingView = schedulingViewList.Where(s => s.StartTime <= now && now <= s.EndTime).FirstOrDefault();
                    if (schedulingView != null)
                    {
                        if (schedulingView.Id.HasValue)
                        {
                            //排班表有执行人
                            task.Scheduling = schedulingView.Id;
                            task.SchedulingStartUser = schedulingView.StartUser;
                            task.SchedulingShift = schedulingView.ShiftCode;
                            task.SchedulingShiftTime = schedulingView.StartTime.ToString("yyyy-MM-dd HH:mm") + " " + schedulingView.EndTime.ToString("yyyy-MM-dd HH:mm");
                        }
                        else
                        {
                            task.Scheduling = null;

                            task.AssignStartUser = schedulingView.StartUser;
                            if (schedulingView.IsAutoAssign && !ISIUtil.Contains(task.AssignStartUser, task.SubmitUser))
                            {
                                string startUser = string.Empty;
                                string startUserNm = string.Empty;
                                if (!string.IsNullOrEmpty(task.AssignStartUser))
                                {
                                    startUser = ISIConstants.ISI_USER_SEPRATOR + ISIUtil.EditUser(task.AssignStartUser);
                                    startUserNm = ISIConstants.ISI_USER_SEPRATOR + this.userSubscriptionMgrE.GetUserName(task.AssignStartUser);
                                }
                                task.AssignStartUser = ISIConstants.ISI_LEVEL_SEPRATOR + task.SubmitUser + startUser + ISIConstants.ISI_LEVEL_SEPRATOR;
                                task.AssignStartUserNm = task.SubmitUserNm + startUserNm;
                            }
                            else if (!string.IsNullOrEmpty(task.AssignStartUser) && string.IsNullOrEmpty(task.AssignStartUserNm))
                            {
                                task.AssignStartUserNm = this.userSubscriptionMgrE.GetUserName(schedulingView.StartUser);
                            }
                            task.SchedulingShift = schedulingView.ShiftCode;
                            task.SchedulingShiftTime = schedulingView.StartTime.ToString("yyyy-MM-dd HH:mm") + " " + schedulingView.EndTime.ToString("yyyy-MM-dd HH:mm");
                        }
                    }
                    else
                    {
                        ClearScheduling(task);
                    }
                }
                else
                {
                    ClearScheduling(task);
                }

                if (!string.IsNullOrEmpty(task.SchedulingStartUser) || !string.IsNullOrEmpty(task.AssignStartUser))
                {
                    //userCodes = !string.IsNullOrEmpty(task.SchedulingStartUser) ? task.SchedulingStartUser : task.AssignStartUser;
                    task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN;

                    this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, now, user);

                    task.AssignDate = now;
                    task.AssignUser = user.Code;
                    task.AssignUserNm = user.Name.Trim();

                    task.StartDate = null;
                    task.StartUser = string.Empty;
                    task.StartUserNm = string.Empty;

                    //工作流 wf
                    //workflowMgrE.StartProcessInstance(task, now, user);

                    //if (!task.TaskSubType.IsAutoAssign)
                    //{
                    task.PlanStartDate = now;
                    task.PlanCompleteDate = this.GetPlanCompleteDate(task.TaskSubType, task.PlanStartDate.Value);
                    //}
                }
            }
        }



        private void ClearScheduling(TaskMstr task)
        {
            //没有排班表，按照任务分类执行
            task.Scheduling = null;
            task.SchedulingStartUser = null;
            task.SchedulingShift = null;
            task.SchedulingShiftTime = null;
            task.AssignStartUser = null;
            task.AssignStartUserNm = null;
        }



        protected void RemindStatus(TaskMstr task, TaskStatus taskStatus)
        {
            #region 获取用户列表

            StringBuilder users = new StringBuilder();
            if (taskStatus.IsRemindCreateUser)
            {
                users.Append(task.CreateUser);
                if (!string.IsNullOrEmpty(task.SubmitUser))
                {
                    users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    users.Append(task.SubmitUser);
                }
            }
            if (taskStatus.IsRemindAssignUser)
            {
                if (!string.IsNullOrEmpty(task.AssignUser))
                {
                    if (users.Length != 0)
                    {
                        users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    }
                    users.Append(task.AssignUser);
                }
                else if (!string.IsNullOrEmpty(task.TaskSubType.AssignUser))
                {
                    if (users.Length != 0)
                    {
                        users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    }
                    users.Append(task.TaskSubType.AssignUser);
                }
            }
            if (taskStatus.IsRemindStartUser && !string.IsNullOrEmpty(task.StartedUser))
            {
                if (users.Length != 0)
                {
                    users.Append(ISIConstants.ISI_USER_SEPRATOR);
                }
                users.Append(task.StartedUser);
            }
            if (taskStatus.IsRemindCommentUser)
            {
                //所有评论人
                var commentList = commentDetailMgrE.GetComment(task.Code);
                if (commentList != null && commentList.Count > 0)
                {
                    string commentUsers = string.Join(";", commentList.Select(t => t.CreateUser).Distinct().ToArray<string>());
                    if (users.Length != 0)
                    {
                        users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    }
                    users.Append(commentUsers);

                    task.CommentDetail = commentList[0];
                }
            }
            #endregion

            User operationUser = new User();
            operationUser.Code = taskStatus.LastModifyUser;
            operationUser.FirstName = taskStatus.LastModifyUserNm;
            IList<UserSub> userSubList = userSubscriptionMgrE.GenerateUserSub(task, users.ToString(), false, operationUser);

            task.TaskStatus = taskStatus;
            userSubscriptionMgrE.Remind(task, ISIConstants.ISI_LEVEL_STATUS, userSubList, operationUser);
        }


        [Transaction(TransactionMode.Unspecified)]
        public DateTime GetShiftStartTime(DateTime date, string shift)
        {
            string[] shiftTime = shift.Split('-');
            if (shiftTime.Length == 0)
                return date.Date;
            else
                return this.AssembleActualTime(date, shiftTime[0]);
        }

        [Transaction(TransactionMode.Unspecified)]
        public DateTime GetShiftEndTime(DateTime date, string shift)
        {
            string[] shiftTime = shift.Split('-');
            if (shiftTime.Length == 0)
            {
                return date.Date.AddDays(1);
            }
            else
            {
                DateTime shiftStart = this.GetShiftStartTime(date, shift);
                DateTime shiftEnd = this.AssembleActualTime(date, shiftTime[shiftTime.Length - 1]);
                if (DateTime.Compare(shiftStart, shiftEnd) < 0)
                {
                    return shiftEnd;
                }
                else
                {
                    //跨夜晚班
                    return shiftEnd.AddDays(1);
                }
            }
        }

        private DateTime AssembleActualTime(DateTime date, string time)
        {
            DateTime actualTime = date;
            try
            {
                actualTime = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " " + time);
            }
            catch (Exception)
            { }

            return actualTime;
        }

        private DateTime GetPlanCompleteDate(TaskSubType taskSubType, DateTime planStartDate)
        {
            DateTime planEndDate = planStartDate;
            if (taskSubType.AssignUpTime.HasValue)
            {
                planEndDate = planEndDate.AddMinutes(double.Parse(taskSubType.AssignUpTime.Value.ToString()));
            }
            else
            {
                EntityPreference entityPreference = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_ASSIGN_UP_TIME);
                if (entityPreference != null && !string.IsNullOrEmpty(entityPreference.Value))
                {
                    planEndDate = planEndDate.AddMinutes(double.Parse(entityPreference.Value));
                }
            }

            if (taskSubType.StartUpTime.HasValue)
            {
                planEndDate = planEndDate.AddMinutes(double.Parse(taskSubType.StartUpTime.Value.ToString()));
            }
            else
            {
                EntityPreference entityPreference = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_START_UP_TIME);
                if (entityPreference != null && !string.IsNullOrEmpty(entityPreference.Value))
                {
                    planEndDate = planEndDate.AddMinutes(double.Parse(entityPreference.Value));
                }
            }
            if (taskSubType.CloseUpTime.HasValue)
            {
                planEndDate = planEndDate.AddMinutes(double.Parse(taskSubType.CloseUpTime.Value.ToString()));
            }
            else
            {
                EntityPreference entityPreference = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_CLOSE_UP_TIME);
                if (entityPreference != null && !string.IsNullOrEmpty(entityPreference.Value))
                {
                    planEndDate = planEndDate.AddMinutes(double.Parse(entityPreference.Value));
                }
            }
            return planEndDate;
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void PlanRemind()
        {
            try
            {
                string mailTo = this.FindEmailByPermission(new string[] { ISIConstants.PERMISSION_PAGE_ISI_VALUE_PLANREMIND });
                string subject = "工作计划更新提醒";
                StringBuilder body = new StringBuilder();

                body.Append("<span style='font-size:15px;'>您好!</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("&nbsp;&nbsp;<span style='font-size:15px;'>请于周四登陆ISI系统上报和更新工作计划。</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("<span style='font-size:15px;'>周五部门经理登陆ISI上报和更新，同时要对自己部门员工的工作计划进行分派和评论。</span>");

                Remind(subject, body, mailTo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void FiveSRemind()
        {
            try
            {
                string mailTo = this.FindEmailByPermission(new string[] { ISIConstants.PERMISSION_PAGE_ISI_VALUE_5SREMIND });

                string subject = "定期每周五5S大检查";
                StringBuilder body = new StringBuilder();
                body.Append("<span style='font-size:15px;'>您好:</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("&nbsp;&nbsp;<span style='font-size:15px;'></span>");
                body.Append("定期每周五7:45分将对办公区域进行5S大检查,请各位员工按照5S标准,");
                body.Append("做好各自区域的5S工作,特别是下班后办公椅的归位，也请相关部门注意一些无人区域的清理。");

                Remind(subject, body, mailTo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }
        //不建议使用
        public string FindEmail()
        {
            return FindEmail(null);
        }

        public string FindEmail(string[] userCodes)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("select u.Email from User u where u.IsActive = 1 and u.Email is not null and u.Email != :Empty ");
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Empty", string.Empty);
            if (userCodes != null && userCodes.Length > 0)
            {
                hql.Append(" and u.Code in (:UserCodeArray)");
                param.Add("UserCodeArray", userCodes);
            }
            IList<object> emails = hqlMgrE.FindAll<object>(hql.ToString(), param);

            if (emails == null || emails.Count == 0) return string.Empty;
            StringBuilder toEmail = new StringBuilder();
            foreach (object emailObj in emails)
            {
                string email = (string)emailObj;
                if (ISIUtil.IsValidEmail(email))
                {
                    if (toEmail.Length != 0)
                    {
                        toEmail.Append(";");
                    }
                    toEmail.Append(email);
                }
            }
            return toEmail.ToString();
        }
        //不建议使用
        [Transaction(TransactionMode.Requires)]
        public void Remind(string subject, StringBuilder body)
        {
            try
            {
                string mailTo = this.FindEmail();
                //mailTo="tiansu@yfgm.com.cn";
                if (string.IsNullOrEmpty(mailTo)) return;
                this.Remind(subject, body, mailTo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Remind(string subject, StringBuilder body, string mailTo)
        {
            try
            {
                if (string.IsNullOrEmpty(mailTo)) return;
                string companyName = entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).Value;
                ISIUtil.AppendTestText(companyName, body, ISIConstants.EMAIL_SEPRATOR);

                string webAddress = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_WEBADDRESS).Value;
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("<span style='font-size:15px;'>" + companyName + "</span><br/>");
                body.Append("<span style='font-size:15px;'><a href='http://" + webAddress + "'>http://" + webAddress + "</a></span>");
                MailPriority mailPriority = MailPriority.Normal;
                string replyTo = string.Empty;
                smtpMgrE.AsyncSend(subject, body.ToString(), mailTo, replyTo, mailPriority);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void SendUp()
        {
            try
            {
                //没有分派的
                //没有确认的
                //没有关闭的
                IList<TaskMstr> taskList = this.GetUpTask();
                string assignUpMinuteDefault = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_ASSIGN_UP_TIME).Value;
                string startUpMinuteDefault = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_START_UP_TIME).Value;
                string closeUpMinuteDefault = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_CLOSE_UP_TIME).Value;

                User operationUser = userMgrE.GetMonitorUser();

                foreach (TaskMstr task in taskList)
                {
                    if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT && task.SubmitDate.HasValue)
                    {
                        string[] assignUpLevel = task.TaskSubType.AssignUpLevel;
                        double assignUpMinute = GetUpTime(task.TaskSubType.AssignUpTime, assignUpMinuteDefault);
                        if (assignUpLevel != null && assignUpLevel.Length > 0)
                        {
                            Remind(task, assignUpMinute, task.SubmitDate.Value, assignUpLevel, operationUser);
                        }
                    }
                    else if ((task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN && task.AssignDate.HasValue))
                    //|| (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REASSIGN && task.ReassignDate.HasValue)
                    {
                        string[] startUpLevel = task.TaskSubType.StartUpLevel;
                        double startUpMinute = GetUpTime(task.TaskSubType.StartUpTime, startUpMinuteDefault);
                        if (startUpLevel != null && startUpLevel.Length > 0)
                        {
                            //if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
                            //{
                            Remind(task, startUpMinute, task.AssignDate.Value, startUpLevel, operationUser);
                            /*}
                           
                           if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REASSIGN)
                           {
                               Remind(task, startUpMinute, task.ReassignDate.Value, startUpLevel, operationUser);
                           }*/
                        }
                    }
                    else if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE && task.CompleteDate.HasValue)
                    {
                        string[] closeUpLevel = task.TaskSubType.CloseUpLevel;
                        double closeUpMinute = GetUpTime(task.TaskSubType.CloseUpTime, closeUpMinuteDefault);
                        if (closeUpLevel != null && closeUpLevel.Length > 0)
                        {
                            Remind(task, closeUpMinute, task.CompleteDate.Value, closeUpLevel, operationUser);
                        }
                    }
                }

                //暂时不处理失败的
                //处理发送失败的
                //SendFail(user);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        private void Remind(TaskMstr task, double minutes, DateTime date, string[] levels, User operationUser)
        {
            int count = levels.Length;
            IList<string> levelList = levels.Reverse<string>().ToList<string>();
            foreach (string userCodes in levelList)
            {
                if (string.IsNullOrEmpty(userCodes)) continue;

                IList<UserSub> userSubs = userSubscriptionMgrE.GenerateUserSub(task, userCodes, false, operationUser);
                if (userSubs != null && userSubs.Count > 0)
                {
                    if (date.AddMinutes(count * minutes) < DateTime.Now)
                    {
                        //是否发送过
                        if (!taskDetailMgrE.IsSended(task.Code, task.TaskSubType.Code, task.Status, count.ToString()))
                        {
                            userSubscriptionMgrE.Remind(task, count.ToString(), minutes, userSubs, operationUser);
                        }
                        break;
                    }
                }
                count--;
            }
        }

        private double GetUpTime(decimal? taskSubTypeUpMinute, string defaultUpMinute)
        {
            if (taskSubTypeUpMinute.HasValue && taskSubTypeUpMinute.Value > 0)
            {
                return double.Parse(taskSubTypeUpMinute.Value.ToString());
            }
            else
            {
                return double.Parse(defaultUpMinute);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public string CreateTaskStatus(TaskStatus taskStatus)
        {
            return CreateTaskStatus(taskStatus, false);
            /*
            TaskMstr task = this.taskMstrMgrE.CheckAndLoadTaskMstr(taskStatus.TaskCode);

            bool isComplete = task.TaskSubType.IsAutoComplete;

            this.taskStatusMgrE.CreateTaskStatus(taskStatus);

            if (taskStatus.IsCurrentStatus
                        || taskStatus.IsRemindAssignUser
                        || taskStatus.IsRemindCommentUser
                        || taskStatus.IsRemindCreateUser
                        || taskStatus.IsRemindStartUser
                        || isComplete)
            {
                if (taskStatus.IsCurrentStatus || isComplete)
                {
                    if (isComplete)
                    {
                        User user = new User();
                        user.Code = taskStatus.LastModifyUser;
                        user.FirstName = taskStatus.LastModifyUserNm;
                        task.IsCompleteNoRemind = true;
                        CompleteTask(task, user);
                    }
                    taskMstrMgrE.UpdateTaskStatus(taskStatus, task);
                }

                this.RemindStatus(task, taskStatus);
            }
             * */
        }

        [Transaction(TransactionMode.Requires)]
        public string CreateTaskStatus(TaskStatus taskStatus, bool isComplete)
        {
            TaskMstr task = this.taskMstrMgrE.CheckAndLoadTaskMstr(taskStatus.TaskCode);
            //自动开始此任务
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
            {
                this.ConfirmTask(task.Code, new User() { Code = task.LastModifyUser, FirstName = task.LastModifyUserNm });
            }
            taskStatus.Type = ISIConstants.CODE_MASTER_ISI_MSG_TYPE_STATUS;
            this.taskStatusMgrE.CreateTaskStatus(taskStatus);

            if (taskStatus.IsCurrentStatus
                        || taskStatus.IsRemindAssignUser
                        || taskStatus.IsRemindCommentUser
                        || taskStatus.IsRemindCreateUser
                        || taskStatus.IsRemindStartUser
                        || isComplete)
            {

                if (taskStatus.IsCurrentStatus || isComplete)
                {
                    if (isComplete && task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
                    {
                        User user = new User();
                        user.Code = taskStatus.LastModifyUser;
                        user.FirstName = taskStatus.LastModifyUserNm;
                        task.IsCompleteNoRemind = true;
                        CompleteTask(task, user);
                    }
                    taskMstrMgrE.UpdateTaskStatus(taskStatus, task);
                }

                this.RemindStatus(task, taskStatus);
            }
            return task.Status;
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateTaskStatus(TaskStatus taskStatus)
        {
            this.taskStatusMgrE.UpdateTaskStatus(taskStatus);

            if (taskStatus.IsCurrentStatus
                        || taskStatus.IsRemindAssignUser
                        || taskStatus.IsRemindCommentUser
                        || taskStatus.IsRemindCreateUser
                        || taskStatus.IsRemindStartUser)
            {
                TaskMstr task = this.taskMstrMgrE.CheckAndLoadTaskMstr(taskStatus.TaskCode);

                if (taskStatus.IsCurrentStatus)
                {
                    taskMstrMgrE.UpdateTaskStatus(taskStatus, task);
                }

                this.RemindStatus(task, taskStatus);
            }
        }


        [Transaction(TransactionMode.Requires)]
        public void CreateCommentDetail(CommentDetail commentDetail, string userCode, string userName)
        {
            DateTime now = DateTime.Now;
            commentDetail.CreateUser = userCode;
            commentDetail.CreateDate = now;
            commentDetail.CreateUserNm = userName;
            commentDetail.LastModifyUser = userCode;
            commentDetail.LastModifyDate = now;
            commentDetail.LastModifyUserNm = userName;
            commentDetail.Type = ISIConstants.CODE_MASTER_ISI_MSG_TYPE_COMMENT;
            this.commentDetailMgrE.CreateCommentDetail(commentDetail);

            #region 获取用户
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(commentDetail.TaskCode);

            StringBuilder users = new StringBuilder();
            users.Append(task.CreateUser);

            if (!string.IsNullOrEmpty(task.SubmitUser))
            {
                users.Append(ISIConstants.ISI_USER_SEPRATOR);
                users.Append(task.SubmitUser);
            }
            /*if (!string.IsNullOrEmpty(task.StartUser))
            {
                users.Append(ISIConstants.ISI_USER_SEPRATOR);
                users.Append(task.StartUser);
            }*/
            if (!string.IsNullOrEmpty(task.StartedUser))
            {
                users.Append(ISIConstants.ISI_USER_SEPRATOR);
                users.Append(task.StartedUser);
            }
            if (!string.IsNullOrEmpty(task.AssignUser))
            {
                users.Append(ISIConstants.ISI_USER_SEPRATOR);
                users.Append(task.AssignUser);
            }
            else if (!string.IsNullOrEmpty(task.TaskSubType.AssignUser))
            {
                users.Append(ISIConstants.ISI_USER_SEPRATOR);
                users.Append(task.TaskSubType.AssignUser);
            }
            var taskStatusList = this.taskStatusMgrE.GetTaskStatus(task.Code);
            if (taskStatusList != null && taskStatusList.Count > 0)
            {
                task.TaskStatus = taskStatusList[0];

                string statusUsers = string.Join(";", taskStatusList.Select(t => t.LastModifyUser).Distinct().ToArray<string>());
                if (users.Length != 0)
                {
                    users.Append(ISIConstants.ISI_USER_SEPRATOR);
                }
                users.Append(statusUsers);
            }

            User operationUser = new User();
            operationUser.Code = userCode;
            operationUser.FirstName = userName;

            IList<UserSub> userSubList = userSubscriptionMgrE.GenerateUserSub(task, users.ToString(), false, operationUser);

            #endregion

            task.CommentDetail = commentDetail;
            userSubscriptionMgrE.Remind(task, ISIConstants.ISI_LEVEL_COMMENT, userSubList, operationUser);

        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskMstr> GetUpTask()
        {
            StringBuilder hql = new StringBuilder();
            hql.Append(@" select t from TaskMstr t join t.TaskSubType tst ");
            hql.Append(@" where (tst.IsAssignUp = 1 and t.Status=:SubmitStatus) ");
            hql.Append(@" or    (tst.IsStartUp = 1 and t.Status=:AssignStatus) ");
            hql.Append(@" or    (tst.IsCloseUp = 1 and t.Status=:CompleteStatus) ");
            hql.Append(@" order by t.Status asc,t.CreateDate asc ");

            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("SubmitStatus", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT);
            param.Add("AssignStatus", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN);
            //param.Add("ReassignStatus", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REASSIGN);
            //param.Add("StartStatus", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS);
            param.Add("CompleteStatus", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE);

            return this.hqlMgrE.FindAll<TaskMstr>(hql.ToString(), param);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateTask(TaskMstr taskMstr, User user)
        {
            DateTime dateTimeNow = DateTime.Now;

            #region 创建TaskMstr

            #region 单号
            if (string.IsNullOrEmpty(taskMstr.Code) || string.IsNullOrEmpty(taskMstr.Type))
            {
                if (taskMstr.Type.Length == 3)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(taskMstr.Type);
                }
                if (taskMstr.Type == ISIConstants.ISI_TASK_TYPE_PLAN)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_PLAN);
                }
                if (taskMstr.Type == ISIConstants.ISI_TASK_TYPE_ISSUE)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_ISSUE);
                }
                if (taskMstr.Type == ISIConstants.ISI_TASK_TYPE_IMPROVE)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_IMPROVE);
                }
                if (taskMstr.Type == ISIConstants.ISI_TASK_TYPE_CHANGE)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_CHANGE);
                }
                if (taskMstr.Type == ISIConstants.ISI_TASK_TYPE_PRIVACY)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_PRIVACY);
                }
                if (taskMstr.Type == ISIConstants.ISI_TASK_TYPE_RESPONSE)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_RESPONSE);
                }
                if (taskMstr.Type == ISIConstants.ISI_TASK_TYPE_PROJECT)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_PROJECT);
                }
                if (taskMstr.Type == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_PROJECT_ISSUE);
                }
                if (taskMstr.Type == ISIConstants.ISI_TASK_TYPE_AUDIT)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_AUDIT);
                }
                if (taskMstr.Type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_ENGINEERING_CHANGE);
                }
                if (string.IsNullOrEmpty(taskMstr.Code))
                {
                    taskMstr.Code = numberControlMgrE.GenerateNumber(ISIConstants.CODE_PREFIX_ISI);
                }

                if (string.IsNullOrEmpty(taskMstr.Type))
                {
                    taskMstr.Type = ISIConstants.ISI_TASK_TYPE_PLAN;
                }
            }
            #endregion

            taskMstr.Template = taskMstr.TaskSubType.Template;
            taskMstr.IsApply = taskMstr.TaskSubType.IsApply;
            taskMstr.IsWF = taskMstr.TaskSubType.IsWF;
            taskMstr.IsTrace = taskMstr.TaskSubType.IsTrace;
            taskMstr.CreateUser = user.Code;
            taskMstr.CreateUserNm = user.Name.Trim();
            taskMstr.CreateDate = dateTimeNow;
            taskMstr.LastModifyUser = user.Code;
            taskMstr.LastModifyUserNm = user.Name.Trim();
            taskMstr.LastModifyDate = dateTimeNow;
            taskMstr.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE;
            taskMstr.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI1;
            taskMstr.Color = ISIConstants.CODE_MASTER_ISI_FLAG_GREEN;
            taskMstrMgrE.CreateTaskMstr(taskMstr);

            workflowMgrE.Create(taskMstr, dateTimeNow, user);

            this.wfDetailMgrE.CreateWFDetail(taskMstr.Code, taskMstr.Status, taskMstr.Level, taskMstr.PreLevel, dateTimeNow, user);

            if (taskMstr.IsAutoRelease)
            {
                this.SubmitTask(taskMstr, user);
            }
            #endregion
        }

        [Transaction(TransactionMode.Unspecified)]
        public int CountTask(string action, User CurrentUser)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskMstr));
            criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Code")));

            criteria.CreateAlias("TaskSubType", "tst");
            /*
            if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                        && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                        && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN)
                        && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE)
                        && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN))
            {
                DetachedCriteria[] tstCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(CurrentUser.Code,
                                ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
                criteria.Add(
                    Expression.Or(
                        Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                        Subqueries.PropertyIn("tst.Code", tstCrieteria[1])));
            }
            */
            if (!string.IsNullOrEmpty(action))
            {
                criteria.Add(Expression.Eq("Status", action));

                if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                        && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
                {
                    if (action == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE || action == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
                    {
                        criteria.Add(Expression.Eq("CreateUser", CurrentUser.Code));
                    }
                    if (action == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
                    {
                        if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE))
                        {
                            #region 非观察者
                            criteria.Add(Expression.Or(Expression.Or(Expression.Eq("tst.AssignUpUser", CurrentUser.Code),
                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                             Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Eq("tst.AssignUser", CurrentUser.Code))))),
                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                                                        ));
                            #endregion
                        }
                        else
                        {
                            #region 观察者
                            criteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                        Expression.Or(Expression.Or(Expression.Eq("tst.AssignUpUser", CurrentUser.Code),
                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                                 Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Eq("tst.AssignUser", CurrentUser.Code))))),
                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                                                               ))));
                            #endregion
                        }
                    }
                    if (action == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN
                            || action == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
                    {
                        if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                                    && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)
                                    && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE))
                        {
                            #region 非观察者
                            //找执行人
                            criteria.Add(Expression.Or(Expression.And(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)), Expression.IsNotNull("AssignStartUser")),
                                                                             Expression.Or(Expression.Eq("AssignStartUser", CurrentUser.Code),
                                                                                           Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                         Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                      Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                         ))))),
                                                            Expression.Or(Expression.And(Expression.Or(Expression.Eq("AssignStartUser", string.Empty), Expression.IsNull("AssignStartUser")),
                                                                                         Expression.Or(Expression.Eq("SchedulingStartUser", CurrentUser.Code),
                                                                                           Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                         Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                      Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                                                      ))))),
                                                                          Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Eq("tst.StartUpUser", CurrentUser.Code))))),
                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                )));
                            #endregion
                        }
                        else
                        {
                            #region 观察者
                            criteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                Expression.Or(Expression.And(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)), Expression.IsNotNull("AssignStartUser")),
                                                                                     Expression.Or(Expression.Eq("AssignStartUser", CurrentUser.Code),
                                                                                                   Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                 Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                              Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                                 ))))),
                                                                    Expression.Or(Expression.And(Expression.Or(Expression.Eq("AssignStartUser", string.Empty), Expression.IsNull("AssignStartUser")),
                                                                                                 Expression.Or(Expression.Eq("SchedulingStartUser", CurrentUser.Code),
                                                                                                   Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                 Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                              Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                                                              ))))),
                                                                                  Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Eq("tst.StartUpUser", CurrentUser.Code))))),
                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                )))));
                            #endregion
                        }
                    }
                    if (action == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                    {

                        if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE))
                        {
                            #region 非观察者
                            criteria.Add(Expression.Or(Expression.And(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE }), Expression.Or(Expression.Eq("CreateUser", CurrentUser.Code), Expression.Eq("SubmitUser", CurrentUser.Code))),
                                            Expression.And(Expression.Not(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE })),
                                                            Expression.Or(Expression.Or(Expression.Eq("tst.CloseUpUser", CurrentUser.Code),
                                                                                                 Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                    Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),

                                                                            Expression.And(Expression.Or(Expression.Not(Expression.Eq("CreateUser", CurrentUser.Code)), Expression.Or(Expression.IsNull("tst.CloseUpUser"), Expression.Eq("tst.CloseUpUser", string.Empty))),
                                                                                     Expression.Or(Expression.Or(Expression.Eq("tst.AssignUser", CurrentUser.Code),
                                                                                                                 Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                                                                   Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                  Expression.Eq("tst.AssignUpUser", CurrentUser.Code))))),
                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                                                       ))))));

                            #endregion
                        }
                        else
                        {
                            #region 观察者
                            criteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                Expression.Or(Expression.And(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE }), Expression.Or(Expression.Eq("CreateUser", CurrentUser.Code), Expression.Eq("SubmitUser", CurrentUser.Code))),
                                                    Expression.And(Expression.Not(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE })),
                                                                    Expression.Or(Expression.Or(Expression.Eq("tst.CloseUpUser", CurrentUser.Code),
                                                                                                         Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),

                                                                                    Expression.And(Expression.Or(Expression.Not(Expression.Eq("CreateUser", CurrentUser.Code)), Expression.Or(Expression.IsNull("tst.CloseUpUser"), Expression.Eq("tst.CloseUpUser", string.Empty))),
                                                                                             Expression.Or(Expression.Or(Expression.Eq("tst.AssignUser", CurrentUser.Code),
                                                                                                                         Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                                                                           Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                          Expression.Eq("tst.AssignUpUser", CurrentUser.Code))))),
                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                         ))))))));
                            #endregion
                        }
                    }
                }
            }
            else if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                        && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
            {
                if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE))
                {
                    criteria.Add(Expression.Or(Expression.Eq("CreateUser", CurrentUser.Code),
                                                 Expression.Or(Expression.Eq("SubmitUser", CurrentUser.Code),
                                                               Expression.Or(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)),
                                                                                            Expression.And(Expression.IsNotNull("AssignStartUser"),
                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Eq("AssignStartUser", CurrentUser.Code))))))),
                                                                            Expression.Or(Expression.And(Expression.Or(Expression.IsNull("AssignStartUser"), Expression.Eq("AssignStartUser", string.Empty)),
                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Eq("SchedulingStartUser", CurrentUser.Code)))))),
                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Eq("tst.AssignUpUser", CurrentUser.Code))))),
                                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Eq("tst.StartUpUser", CurrentUser.Code))))),
                                                                                                                        Expression.Or(Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Eq("tst.CloseUpUser", CurrentUser.Code))))),
                                                                                                                                        Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                      Expression.Eq("tst.AssignUser", CurrentUser.Code))))),
                                                                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                    Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                                                                                                                                ))))))));




                }
                else
                {
                    criteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                Expression.Or(Expression.Eq("CreateUser", CurrentUser.Code),
                                                                     Expression.Or(Expression.Eq("SubmitUser", CurrentUser.Code),
                                                                                   Expression.Or(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)),
                                                                                                                Expression.And(Expression.IsNotNull("AssignStartUser"),
                                                                                                                            Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Eq("AssignStartUser", CurrentUser.Code))))))),
                                                                                                Expression.Or(Expression.And(Expression.Or(Expression.IsNull("AssignStartUser"), Expression.Eq("AssignStartUser", string.Empty)),
                                                                                                                                            Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                    Expression.Eq("SchedulingStartUser", CurrentUser.Code)))))),
                                                                                                                Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Eq("tst.AssignUpUser", CurrentUser.Code))))),
                                                                                                                                Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                    Expression.Eq("tst.StartUpUser", CurrentUser.Code))))),
                                                                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                            Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                    Expression.Eq("tst.CloseUpUser", CurrentUser.Code))))),
                                                                                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                          Expression.Eq("tst.AssignUser", CurrentUser.Code))))),
                                                                                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                                        Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                ))))))))));
                }
            }

            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0)
            {
                return count[0];
            }

            return 0;
        }

        //任务逾期提醒项目经理
        [Transaction(TransactionMode.RequiresNew)]
        public void CompleteRemind()
        {
            try
            {
                DateTime now = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                sql.Append(@"select t.Code, t.Address as TaskAddress, t.Type, Subject, Desc1, Desc2, Status, ");
                sql.Append(@"       t.Priority, t.StartDate, t.BackYards, t.Flag, t.Color, ");
                sql.Append(@"       t.PlanStartDate, t.PlanCompleteDate, t.ExpectedResults, t.UserName, t.Email, ");
                sql.Append(@"       t.MobilePhone, t.Scheduling, t.SchedulingStartUser, t.SchedulingShift, t.SchedulingShiftTime, ");
                sql.Append(@"       t.AssignStartUser, t.CreateUserNm, t.CreateUser, t.CreateDate, t.SubmitUserNm, t.SubmitUser, ");
                sql.Append(@"       t.SubmitDate,  t.AssignUserNm, t.AssignUser, ");
                sql.Append(@"       t.AssignDate, t.StartUserNm, t.StartUser, t.CompleteUserNm, t.CompleteUser, t.CompleteDate, ");
                sql.Append(@"       t.Seq, t.Phase, ");
                sql.Append(@"       tst.Code as TaskSubTypeCode,tst.Desc_ as TaskSubTypeDesc, FailureMode as FailureModeCode,tst.AssignUser as TaskSubTypeAssignUser, ");
                sql.Append(@"       t.SchedulingStartUser ");
                sql.Append(@"from ISI_TaskMstr t join ISI_TaskSubType tst on t.TaskSubType=tst.Code  ");
                sql.Append(@"where tst.IsActive=1 and tst.IsCompleteUp=1 ");
                sql.Append(@"and t.Status in ('" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN + "','" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS + "') ");
                sql.Append(@"and tst.AssignUser is not null and tst.AssignUser != '' ");
                sql.Append(@"and DATEADD(MINUTE,isnull(tst.CompleteUpTime,0),t.PlanCompleteDate) <= getdate() ");
                sql.Append(@"and not exists(select 1 from ISI_TaskDet td where td.TaskCode=t.Code and td.Level_='" + ISIConstants.ISI_LEVEL_COMPLETE + "')");
                sql.Append(@"order by t.CreateDate asc ");

                DataSet taskDS = sqlHelperMgrE.GetDatasetBySql(sql.ToString());

                var taskList = IListHelper.DataTableToList<TaskMstr>(taskDS.Tables[0]);

                if (taskList != null && taskList.Count > 0)
                {
                    string users = string.Join(";", taskList.Select(t => t.TaskSubTypeAssignUser).Distinct().ToArray<string>());

                    if (string.IsNullOrEmpty(users)) return;

                    var userDic = this.taskMstrMgrE.GetUser2(users);
                    if (userDic == null || userDic.Count == 0) return;

                    foreach (var task in taskList)
                    {
                        var userSubList = new List<UserSub>();
                        string[] assignUser = (task.TaskSubTypeAssignUser + task.StartedUser).Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var u in assignUser)
                        {
                            if (userDic.Keys.Contains(u) && !userSubList.Contains(userDic[u]))
                            {
                                userSubList.Add(userDic[u]);
                            }
                        }
                        if (userSubList.Count > 0)
                        {
                            userSubscriptionMgrE.Remind(task, ISIConstants.ISI_LEVEL_COMPLETE, userSubList, userMgrE.GetMonitorUser());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        public string FindUserNameByPermission(string[] permissionCodes)
        {
            if (permissionCodes == null || permissionCodes.Length == 0) return string.Empty;
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PermissionCodes", permissionCodes);

            StringBuilder userSql = new StringBuilder();
            userSql.Append(@"select u.Code,u.FirstName,u.LastName ");
            userSql.Append(@"from User u ");
            userSql.Append(@"where ");
            userSql.Append(@"      u.IsActive = 1 ");
            userSql.Append(@"and ");
            userSql.Append(@"      (");
            userSql.Append(@"          exists (select up.Permission.Code from UserPermission up where up.User.Code = u.Code and up.Permission.Code in (:PermissionCodes)) ");
            userSql.Append(@"      or ");
            userSql.Append(@"          exists (select rp.Permission.Code from RolePermission rp join rp.Role r,UserRole ur where r.Code = ur.Role.Code and ur.User.Code = u.Code and rp.Permission.Code in (:PermissionCodes))  ");
            userSql.Append(@"      )");

            userSql.Append(@"order by u.Code ");
            IList<object[]> users = this.hqlMgrE.FindAll<object[]>(userSql.ToString(), param);

            if (users == null || users.Count == 0) return string.Empty;

            string userNames = string.Join("、", users.Select(u => (string)u[1] + (string)u[2] + "[" + (string)u[0] + "]").ToArray<string>());
            return userNames;
        }

        [Transaction(TransactionMode.Unspecified)]
        public string FindEmailByPermission(string[] permissionCodes)
        {
            if (permissionCodes == null || permissionCodes.Length == 0) return string.Empty;
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PermissionCodes", permissionCodes);

            StringBuilder userSql = new StringBuilder();
            userSql.Append(@"select u.Email ");
            userSql.Append(@"from User u ");
            userSql.Append(@"where ");
            userSql.Append(@"      u.Email != '' and u.Email is not null and u.IsActive = 1 ");
            userSql.Append(@"and ");
            userSql.Append(@"      (");
            userSql.Append(@"          exists (select up.Permission.Code from UserPermission up where up.User.Code = u.Code and up.Permission.Code in (:PermissionCodes)) ");
            userSql.Append(@"      or ");
            userSql.Append(@"          exists (select rp.Permission.Code from RolePermission rp join rp.Role r,UserRole ur where r.Code = ur.Role.Code and ur.User.Code = u.Code and rp.Permission.Code in (:PermissionCodes))  ");
            userSql.Append(@"      )");

            userSql.Append(@"order by u.Code ");
            IList<object> emails = this.hqlMgrE.FindAll<object>(userSql.ToString(), param);

            if (emails == null || emails.Count == 0) return string.Empty;

            string mailList = string.Join(";", emails.Select(u => (string)u).ToArray<string>());
            return mailList;
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void SendTaskSubTypeReport()
        {
            try
            {
                StringBuilder hql = new StringBuilder();
                hql.Append(@"select  ts.TaskSubTypeAssignUser,ts.TaskSubTypeCode,ts.TaskSubTypeDesc, ");
                hql.Append(@"        ts.TaskCode,ts.Subject,ts.Desc1,ts.Desc2,ts.AssignUserNm,ts.SubmitUserNm,ts.StartedUser, ");//t.SchedulingStartUser,t.AssignStartUser
                hql.Append(@"        ts.Flag,ts.Color,ts.StatusDesc,ts.CreateDate,ts.CreateUserNm,ts.CommentCreateUserNm, ");
                hql.Append(@"        ts.CommentCreateDate,ts.Comment,ts.ExpectedResults,ts.Priority,ts.AttachmentCount,ts.Phase,ts.Seq,ts.Type,ts.PlanCompleteDate,ts.Status, ");
                hql.Append(@"        ts.CommentCount,ts.RefTaskCount,ts.StatusCount,c.Description ");
                hql.Append(@"from TaskStatusView ts,CodeMaster c ");
                hql.Append(@"where c.Code= :Type and c.Value = ts.Type  ");
                hql.Append(@"and ts.IsReport = 1 and ts.TaskSubTypeAssignUser is not null and ts.TaskSubTypeAssignUser != '' ");

                hql.Append(@"and ( ");
                hql.Append(@"        (ts.CreateDate <= :EndDate and ts.CreateDate>= :StartDate and ts.StatusDesc != '' and ts.StatusDesc is not null) ");
                hql.Append(@"    or  (ts.Status in (:Status) and ts.PlanCompleteDate is not null and ts.PlanCompleteDate <= :Now ) ");
                hql.Append(@") ");
                //todo
                //hql.Append(" and tst.Code='RGYK' ");
                hql.Append(@"order by ts.TaskSubTypeCode asc,ts.Phase asc,ts.Seq asc,ts.TaskCode asc ");

                DateTime endDate = DateTime.Now;
                //todo
                DateTime startDate = endDate.AddDays(-1);
                IDictionary<string, object> param = new Dictionary<string, object>();
                param.Add("Type", ISIConstants.CODE_MASTER_ISI_TYPE);
                param.Add("StartDate", startDate);
                param.Add("EndDate", endDate);
                param.Add("Status", new string[] { ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS });
                param.Add("Now", endDate);

                var taskStatusViewList = this.hqlMgrE.FindAll<object[]>(hql.ToString(), param);
                if (taskStatusViewList != null && taskStatusViewList.Count > 0)
                {
                    var taskSubTypeList =
                        taskStatusViewList.Select(t => new { AssignUser = (string)t[0], Code = (string)t[1], Desc = (string)t[2], Type = (string)t[23] }).Distinct();

                    //获得用户名字
                    string users = string.Join(";", taskSubTypeList.Select(t => t.AssignUser).ToArray<string>());
                    users += ISIConstants.ISI_LEVEL_SEPRATOR + string.Join(";", taskStatusViewList.Select(s => (string)s[9]).ToArray<string>());

                    if (string.IsNullOrEmpty(users)) return;

                    var userDic = this.taskMstrMgrE.GetUser2(users);
                    if (userDic == null || userDic.Count == 0) return;

                    var entityPreferenceList = entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[]
                        {
                            BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME,
                            ISIConstants.ENTITY_PREFERENCE_WEBADDRESS
                        });
                    var statusCodeMstrList = this.codeMasterMgrE.GetCachedCodeMaster(ISIConstants.CODE_MASTER_ISI_STATUS).ToDictionary(c => c.Value, c => c.Description);

                    foreach (var taskSutType in taskSubTypeList)
                    {
                        var taskList = taskStatusViewList.Where(t => (string)t[1] == taskSutType.Code).Select(s => new TaskView
                                                                                   {
                                                                                       AssignUser = (string)s[0],
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
                                                                                       StatusCount = (s[28] != null ? int.Parse(s[28].ToString()) : 0),
                                                                                       TaskType = (string)s[29]
                                                                                   }).ToList();

                        string[] userCodes = new string[] { };
                        StringBuilder assignUser = new StringBuilder();
                        if (userDic != null && userDic.Count > 0 && !string.IsNullOrEmpty(taskSutType.AssignUser))
                        {
                            userCodes = taskSutType.AssignUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
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
                            }
                        }

                        StringBuilder updateTaskListBody = new StringBuilder();
                        updateTaskListBody.Append("<span style='font-size:13px;' >" + taskSutType.Code + "&nbsp;&nbsp;[" + taskSutType.Desc +
                                "]&nbsp;&nbsp;分派人：" + assignUser.ToString() + "</span>");
                        ISIUtil.GetColumnHead(updateTaskListBody);
                        StringBuilder expectListBody = new StringBuilder();
                        expectListBody.Append("<span style='font-size:13px;' >逾期任务列表</span>");

                        ISIUtil.GetColumnHead(expectListBody);
                        foreach (var task in taskList)
                        {
                            //处理输出
                            //是否是最新更新
                            if (task.StatusDate.HasValue)
                            {
                                if (task.StatusDate.Value >= startDate && task.StatusDate.Value <= endDate)
                                {
                                    ISIUtil.GetReportDetailBody(updateTaskListBody, task, endDate, userDic, statusCodeMstrList);
                                }
                            }

                            if (task.PlanCompleteDate.HasValue && (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN))
                            {
                                if (task.PlanCompleteDate.Value <= endDate)
                                {
                                    ISIUtil.GetReportDetailBody(expectListBody, task, endDate, userDic, statusCodeMstrList);
                                }
                            }
                        }
                        updateTaskListBody.Append("</table><br />");
                        expectListBody.Append("</table><br />");

                        foreach (var userCode in userCodes)
                        {
                            if (userDic.Keys.Contains(userCode))
                            {
                                string subject = string.Empty;
                                if (taskSutType.Type == ISIConstants.ISI_TASK_TYPE_PROJECT)
                                {
                                    subject = "ISI项目报告";
                                }
                                else
                                {
                                    subject = "ISI任务分类报告";
                                }
                                StringBuilder mailBody = new StringBuilder();
                                string companyName =
                                    entityPreferenceList.Where(
                                        e => e.Code == BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME)
                                                        .FirstOrDefault()
                                                        .Value;

                                ISIUtil.AppendTestText(companyName, mailBody, ISIConstants.EMAIL_SEPRATOR);

                                mailBody.Append("<span style='font-size:13px;'>尊敬的: " + userDic[userCode].Name + " 先生/女士</span><br /><br />");
                                mailBody.Append("&nbsp;&nbsp;<span style='font-size:13px;'>您的 " + subject + "。</span><br />");
                                mailBody.Append("&nbsp;&nbsp;<span style='font-size:13px;'>时间范围: " + startDate.ToString("yyyy-MM-dd HH:mm") + " 至 " + endDate.ToString("yyyy-MM-dd HH:mm") + "</span><br /><br /><br />");

                                string webAddress = entityPreferenceList.Where(
                                        e => e.Code == ISIConstants.ENTITY_PREFERENCE_WEBADDRESS)
                                                        .FirstOrDefault()
                                                        .Value;

                                mailBody.Append("<span style='font-size:14px;'>详细情况</span><br />");

                                mailBody.Append(updateTaskListBody.ToString());
                                mailBody.Append(expectListBody.ToString());
                                mailBody.Append("<br /><br /><br /><br />");
                                mailBody.Append("<span style='font-size:13px;'>重要:本邮件只是提示,具体内容以 <a href='http://" + webAddress + "'>ISI系统</a> 为准.</span><br />");
                                mailBody.Append("<span style='font-size:13px;'>谢谢合作!</span><br /><br />");
                                mailBody.Append("<span style='font-size:13px;'>" + companyName + "</span><br/>");
                                mailBody.Append("<span style='font-size:13px;'><a href='http://" + webAddress + "'>http://" + webAddress + "</a></span>");
                                //todo
                                this.smtpMgrE.AsyncSend(subject, mailBody.ToString(), userDic[userCode].Email, string.Empty, MailPriority.Normal);
                                //this.SmtpMgrE.AsyncSend(subject, mailBody.ToString(), "tiansu@yfgm.com.cn", string.Empty, MailPriority.Normal);
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

        //提醒执行人快到完成时间了
        [Transaction(TransactionMode.RequiresNew)]
        public void StartPercentRemind()
        {
            try
            {
                DateTime now = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                sql.Append(@"select t.Code, t.Address as TaskAddress, t.Type, Subject, Desc1, Desc2, Status, ");
                sql.Append(@"       t.Priority, t.StartDate, t.BackYards, t.Flag, t.Color, ");
                sql.Append(@"       t.PlanStartDate, t.PlanCompleteDate, t.ExpectedResults, t.UserName, t.Email, ");
                sql.Append(@"       t.MobilePhone, t.Scheduling, t.SchedulingStartUser, t.SchedulingShift, t.SchedulingShiftTime, ");
                sql.Append(@"       t.AssignStartUser, t.CreateUserNm, t.CreateUser, t.CreateDate, t.SubmitUserNm, t.SubmitUser, ");
                sql.Append(@"       t.SubmitDate,  t.AssignUserNm, t.AssignUser, ");
                sql.Append(@"       t.AssignDate, t.StartUserNm, t.StartUser, t.CompleteUserNm, t.CompleteUser, t.CompleteDate, ");
                sql.Append(@"       t.Seq, t.Phase, tst.StartPercent, ");
                sql.Append(@"       tst.Code as TaskSubTypeCode,tst.Desc_ as TaskSubTypeDesc, FailureMode as FailureModeCode,tst.AssignUser as TaskSubTypeAssignUser ");
                sql.Append(@"from ISI_TaskMstr t join ISI_TaskSubType tst on t.TaskSubType=tst.Code  ");
                sql.Append(@"where tst.IsActive=1 and tst.IsStart=1 and tst.StartPercent is not null ");
                sql.Append(@"and t.Status in ('" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN + "','" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS + "') ");
                sql.Append(@"and DATEADD(MINUTE, datediff(MINUTE,t.PlanStartDate,t.PlanCompleteDate) * tst.StartPercent,t.PlanStartDate) <= getdate() ");
                sql.Append(@"and not exists(select 1 from ISI_TaskDet td where td.TaskCode=t.Code and td.Level_ in ('" + ISIConstants.ISI_LEVEL_STARTPERCENT + "','" + ISIConstants.ISI_LEVEL_COMPLETE + "')) ");
                sql.Append(@"order by t.CreateDate asc ");

                DataSet taskDS = sqlHelperMgrE.GetDatasetBySql(sql.ToString());

                var taskList = IListHelper.DataTableToList<TaskMstr>(taskDS.Tables[0]);

                if (taskList != null && taskList.Count > 0)
                {
                    string users = string.Join(";", taskList.Select(t => t.StartedUser).Distinct().ToArray<string>());

                    if (string.IsNullOrEmpty(users)) return;

                    var userDic = this.taskMstrMgrE.GetUser2(users);
                    if (userDic == null || userDic.Count == 0) return;

                    foreach (var task in taskList)
                    {
                        string[] startedUser = task.StartedUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                        var userSubList = new List<UserSub>();
                        foreach (var u in startedUser)
                        {
                            if (userDic.Keys.Contains(u))
                            {
                                userSubList.Add(userDic[u]);
                            }
                        }
                        userSubscriptionMgrE.Remind(task, ISIConstants.ISI_LEVEL_STARTPERCENT, userSubList, userMgrE.GetMonitorUser());
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }


        //提醒执行人开始执行
        [Transaction(TransactionMode.RequiresNew)]
        public void OpenRemind()
        {
            try
            {
                DateTime now = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                sql.Append(@"select t.Code, t.Address as TaskAddress, t.Type, Subject, Desc1, Desc2, Status, ");
                sql.Append(@"       t.Priority, t.StartDate, t.BackYards, t.Flag, t.Color, ");
                sql.Append(@"       t.PlanStartDate, t.PlanCompleteDate, t.ExpectedResults, t.UserName, t.Email, ");
                sql.Append(@"       t.MobilePhone, t.Scheduling, t.SchedulingStartUser, t.SchedulingShift, t.SchedulingShiftTime, ");
                sql.Append(@"       t.AssignStartUser, t.CreateUserNm, t.CreateUser, t.CreateDate, t.SubmitUserNm, t.SubmitUser, ");
                sql.Append(@"       t.SubmitDate,  t.AssignUserNm, t.AssignUser, ");
                sql.Append(@"       t.AssignDate, t.StartUserNm, t.StartUser, t.CompleteUserNm, t.CompleteUser, t.CompleteDate, ");
                sql.Append(@"       t.Seq, t.Phase, tst.StartPercent, ");
                sql.Append(@"       tst.Code as TaskSubTypeCode,tst.Desc_ as TaskSubTypeDesc, FailureMode as FailureModeCode,tst.AssignUser as TaskSubTypeAssignUser ");
                sql.Append(@"from ISI_TaskMstr t join ISI_TaskSubType tst on t.TaskSubType=tst.Code  ");
                sql.Append(@"where tst.IsActive=1 and tst.IsOpen=1 and t.PlanStartDate is not null ");
                sql.Append(@"and t.Status = '" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN + "' ");
                sql.Append(@"and DATEADD(MINUTE,-isnull(tst.OpenTime,0),t.PlanStartDate) <= getdate() ");
                sql.Append(@"and not exists(select 1 from ISI_TaskDet td where td.TaskCode=t.Code and td.Level_ in ('" + ISIConstants.ISI_LEVEL_OPEN + "','" + ISIConstants.ISI_LEVEL_STARTPERCENT + "','" + ISIConstants.ISI_LEVEL_COMPLETE + "')) ");
                sql.Append(@"order by t.CreateDate asc ");

                DataSet taskDS = sqlHelperMgrE.GetDatasetBySql(sql.ToString());

                var taskList = IListHelper.DataTableToList<TaskMstr>(taskDS.Tables[0]);

                if (taskList != null && taskList.Count > 0)
                {
                    string users = string.Join(";", taskList.Select(t => t.StartedUser).Distinct().ToArray<string>());

                    if (string.IsNullOrEmpty(users)) return;

                    var userDic = this.taskMstrMgrE.GetUser2(users);
                    if (userDic == null || userDic.Count == 0) return;

                    foreach (var task in taskList)
                    {
                        string[] startedUser = task.StartedUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                        var userSubList = new List<UserSub>();
                        foreach (var u in startedUser)
                        {
                            if (userDic.Keys.Contains(u))
                            {
                                userSubList.Add(userDic[u]);
                            }
                        }
                        userSubscriptionMgrE.Remind(task, ISIConstants.ISI_LEVEL_OPEN, userSubList, userMgrE.GetMonitorUser());
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

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class TaskMgrE : com.Sconit.ISI.Service.Impl.TaskMgr, ITaskMgrE
    {
    }
}

#endregion Extend Class