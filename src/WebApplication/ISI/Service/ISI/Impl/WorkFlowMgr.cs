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
    public class WorkflowMgr : IWorkflowMgr
    {
        public IHqlMgrE hqlMgrE { get; set; }
        public IWorkDetMgrE workDetMgrE { get; set; }
        public IWFDetailMgrE wfDetailMgrE { get; set; }
        public ITaskApplyMgrE taskApplyMgrE { get; set; }
        public IApplyMgrE applyMgrE { get; set; }
        public IUserSubscriptionMgrE userSubscriptionMgrE { get; set; }
        public IProcessInstanceMgrE processInstanceMgrE { get; set; }
        public IProcessApplyMgrE processApplyMgrE { get; set; }
        public IProcessDefinitionMgrE processDefinitionMgrE { get; set; }
        public ITaskStatusMgrE taskStatusMgrE { get; set; }

        [Transaction(TransactionMode.Requires)]
        public decimal? GenWorkHours(string taskCode, FailureMode type, string status, int level, string submitUser, string submitUserName, DateTime submitDate, string workHoursUser, string workHoursUserNm, DateTime effDate, User user)
        {
            return GenWorkHours(taskCode, type, status, level, null, submitUser, submitUserName, submitDate, workHoursUser, workHoursUserNm, effDate, user);
        }
        [Transaction(TransactionMode.Requires)]
        public decimal? GenWorkHours(string taskCode, FailureMode type, string status, int level, IList<TaskApply> uomApplyList, string submitUser, string submitUserName, DateTime submitDate, string workHoursUser, string workHoursUserNm, DateTime effDate, User user)
        {
            decimal? workHours = null;
            if (uomApplyList == null || uomApplyList.Count == 0)
            {
                uomApplyList = taskApplyMgrE.GetTaskApply(taskCode);
            }
            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE && level == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE && uomApplyList.Where(u => u.UOM == ISIConstants.WORKHOUR_UOM).Count() > 0)
            {
                var applyHour = uomApplyList.Where(u => u.UOM == ISIConstants.WORKHOUR_UOM).FirstOrDefault();
                if (applyHour.IsUser.HasValue && applyHour.IsUser.Value)
                {
                    if (!string.IsNullOrEmpty(workHoursUser))
                    {
                        string[] workHoursUserCode = workHoursUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                        string[] workHoursUserName = workHoursUserNm.Split(ISIConstants.ISI_USERNAME_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                        workHours = 0;
                        for (int i = 0; i < workHoursUserCode.Length; i++)
                        {
                            workHours += CreateWorkDet(taskCode, type, submitUser, submitUserName, submitDate, effDate, user, applyHour, workHoursUserCode[i], workHoursUserName[i]);
                        }
                    }
                }
                else
                {
                    workHours = 0;
                    workHours += CreateWorkDet(taskCode, type, submitUser, submitUserName, submitDate, effDate, user, applyHour, user.Code, user.Name);
                }
            }
            return workHours;
        }
        [Transaction(TransactionMode.Requires)]
        private decimal CreateWorkDet(string taskCode, FailureMode type, string submitUser, string submitUserName, DateTime submitDate, DateTime effDate, User user, TaskApply applyHour, string userCode, string userName)
        {
            decimal workHours = 0;
            if (applyHour.Qty.HasValue || !string.IsNullOrEmpty(applyHour.Value))
            {
                WorkDet workDet = new WorkDet();
                workDet.TaskApplyId = applyHour.Id;
                workDet.Type = type.Code;
                workDet.TaskCode = taskCode;
                workDet.TaskSubType = applyHour.TaskSubType;
                workDet.Apply = applyHour.Apply;
                workDet.UserCode = userCode;
                workDet.UserNm = userName;
                workDet.UOM = applyHour.UOM;
                workDet.Qty = applyHour.Qty.HasValue ? applyHour.Qty.Value : decimal.Parse(applyHour.Value);
                workDet.CreateDate = submitDate;
                workDet.CreateUser = submitUser;
                workDet.CreateUserNm = submitUserName;
                workDet.ApproveDate = effDate;
                workDet.ApproveUser = user.Code;
                workDet.ApproveUserNm = user.Name;
                workHours = workDet.Qty;
                workDetMgrE.CreateWorkDet(workDet);
            }
            return workHours;
        }
        [Transaction(TransactionMode.Requires)]
        public void SetApproval(TaskMstr task, IList<ProcessInstance> newProcessInstanceList)
        {
            SetApproval(task, newProcessInstanceList, false);
        }
        [Transaction(TransactionMode.Requires)]
        public void SetApproval(TaskMstr task, IList<ProcessInstance> newProcessInstanceList, bool isUpdate)
        {
            int lastLevel = newProcessInstanceList.Max(p => p.Level);
            StringBuilder userCode = new StringBuilder("|");
            StringBuilder userName = new StringBuilder();
            StringBuilder userLevel = new StringBuilder();

            foreach (var p in newProcessInstanceList)
            {
                if (p.Level == lastLevel)
                {
                    p.Level = ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE;
                }
                if (!string.IsNullOrEmpty(p.UserCode) && p.Level >= ISIConstants.CODE_MASTER_WFS_LEVEL3)
                {
                    if (userCode.Length > 1)
                    {
                        userCode.Append(",");
                        userName.Append(", ");
                        userLevel.Append(",");
                    }
                    userCode.Append(p.UserCode);
                    userName.Append(p.UserNm);
                    userLevel.Append(p.Level);
                }
                if (isUpdate)
                {
                    processInstanceMgrE.CreateProcessInstance(p);
                }
            }
            userCode.Append("|");
            task.ApprovalUser = userCode.ToString();
            task.ApprovalUserNm = userName.ToString();
            task.ApprovalLevel = userLevel.ToString();
        }

        [Transaction(TransactionMode.Requires)]
        public void SetApproval(TaskMstr task)
        {
            var processInstanceList = hqlMgrE.FindAll<ProcessInstance>("from ProcessInstance where UserCode !='' and UserCode is not null and TaskCode='" + task.Code + "' and Level >=" + ISIConstants.CODE_MASTER_WFS_LEVEL3 + " order by Level asc ");
            StringBuilder userCode = new StringBuilder();
            StringBuilder userName = new StringBuilder();
            StringBuilder userLevel = new StringBuilder();
            if (processInstanceList != null && processInstanceList.Count >= 0)
            {
                userCode.Append("|");
                for (int i = 0; i < processInstanceList.Count; i++)
                {
                    var p = processInstanceList[i];
                    /*if (userCode.ToString() == p.UserCode
                                || userCode.ToString().EndsWith("," + p.UserCode + "|")
                                || userCode.ToString().StartsWith("|" + p.UserCode + ",")
                                || userCode.ToString().IndexOf("," + p.UserCode + ",") != -1
                                || (i == 1 && userCode.ToString() == ("|" + p.UserCode.ToString())))
                    {
                        continue;
                    }
                    */
                    if (i != 0)
                    {
                        userCode.Append(",");
                        userName.Append(", ");
                        userLevel.Append(",");
                    }
                    userCode.Append(p.UserCode);
                    userName.Append(p.UserNm);
                    userLevel.Append(p.Level);
                }
                userCode.Append("|");
            }
            task.ApprovalUser = userCode.ToString();
            task.ApprovalUserNm = userName.ToString();
            task.ApprovalLevel = userLevel.ToString();
        }

        [Transaction(TransactionMode.Requires)]
        public void StartProcessInstance(TaskMstr task, string assignUser, string costCenterUser, bool isRemind, DateTime effDate, User user)
        {
            List<ProcessInstance> newProcessInstanceList = new List<ProcessInstance>();
            if (task.IsWF && task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
            {
                int level = ISIConstants.CODE_MASTER_WFS_LEVEL2;
                int nextLevel = ISIConstants.CODE_MASTER_WFS_LEVEL3;

                //加入释放人
                newProcessInstanceList.Add(AddProcessInstance(task.Code, task.TaskSubType.Code, task.SubmitUser, task.SubmitUserNm, user, effDate));

                var processDefinitionList = processDefinitionMgrE.GetProcessDefinition(task.TaskSubType.Code, task.TaskSubType.ProcessNo);
                if (processDefinitionList == null || processDefinitionList.Count == 0)
                {
                    //task.Qty = GenWorkHours(task.Code, task.FailureMode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE, ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE, task.TaskApplyList, task.SubmitUser, task.SubmitUserNm, task.SubmitDate.Value, task.WorkHoursUser, task.WorkHoursUserNm, effDate, user);
                    task.Level = ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE;

                }
                else
                {

                    //流程最终审批人提交
                    /*
                    if (processDefinitionList.Last().UserCode == user.Code)
                    {
                        nextLevel = ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE;
                    }
                    */
                    if (task.TaskSubType.IsAssignUser && !string.IsNullOrEmpty(assignUser))
                    {
                        if (task.TaskSubType.IsCtrl || !ISIUtil.Contains(assignUser, processDefinitionList[0].UserCode))
                        {
                            newProcessInstanceList.AddRange(AddDeptProcessInstance(task.Code, task.TaskSubType.Code, nextLevel, assignUser, isRemind, task.TaskSubType.IsCtrl, user, effDate));

                            level = ISIConstants.CODE_MASTER_WFS_LEVEL3;
                            if (nextLevel != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE && ISIUtil.Contains(assignUser, user.Code) && !task.TaskSubType.IsCtrl)
                            {
                                //level = ISIConstants.CODE_MASTER_WFS_LEVEL4;
                                nextLevel = ISIConstants.CODE_MASTER_WFS_LEVEL4;
                            }
                        }
                    }

                    //成本中心审批
                    if (!string.IsNullOrEmpty(costCenterUser))
                    {
                        level += ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                        newProcessInstanceList.AddRange(AddCostCenterProcessInstance(task.Code, task.TaskSubType.Code, level, costCenterUser, isRemind, task.TaskSubType.IsCtrl, user, effDate));
                    }

                    //true 当前用户是流程审批人，直接审批
                    //false 流程上的用户不直接审批
                    bool isContinuous = true;
                    for (int i = 0; i < processDefinitionList.Count; i++)
                    {
                        var p = processDefinitionList[i];

                        ProcessInstance processInstance = new ProcessInstance();
                        processInstance.TaskCode = task.Code;
                        processInstance.Desc1 = p.Desc1;
                        processInstance.IsRemind = p.IsRemind;
                        processInstance.TaskSubType = p.TaskSubType;
                        processInstance.UserCode = p.UserCode;

                        if (i == 0 || p.Seq != processDefinitionList[i - 1].Seq)
                        {
                            level += ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                        }
                        else if (!p.ATicket || p.IsCtrl)
                        {
                            //此级别有多个审批人，连续中断
                            isContinuous = false;
                        }
                        processInstance.Level = level;
                        processInstance.UserNm = p.UserNm;
                        processInstance.ATicket = p.ATicket;
                        processInstance.IsOpt = p.IsOpt;
                        processInstance.IsAccountCtrl = p.IsAccountCtrl;
                        processInstance.IsCtrl = p.IsCtrl;
                        processInstance.IsApprove = p.IsApprove;
                        processInstance.IsParallel = p.IsParallel;

                        //提交人不是审批人，加条件审批
                        if (newProcessInstanceList[newProcessInstanceList.Count - 1].UserCode != user.Code)
                        {
                            processInstance.UOM = p.UOM;
                            processInstance.UOMDesc = p.UOMDesc;
                            processInstance.Qty = p.Qty;
                            processInstance.Apply = p.Apply;
                            processInstance.ApplyDesc = p.ApplyDesc;
                            processInstance.ApplyQty = p.ApplyQty;
                        }

                        processInstance.CreateDate = effDate;
                        processInstance.CreateUser = user.Code;
                        processInstance.CreateUserNm = user.Name;

                        //1. 审批结束
                        //2. 连续标志、非会签控制、当前用户是审批人
                        if (nextLevel == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE
                                 || (isContinuous && !processInstance.IsCtrl && processInstance.UserCode == user.Code))
                        {
                            processInstance.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE;
                            processInstance.ProcessDate = effDate;
                            processInstance.ProcessUser = user.Code;
                            processInstance.ProcessUserNm = user.Name;
                            if (nextLevel != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
                            {
                                nextLevel = processInstance.Level;
                            }
                        }
                        else
                        {
                            isContinuous = false;
                            processInstance.Status = ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED;
                        }

                        newProcessInstanceList.Add(processInstance);

                        if (processInstance.IsCtrl && (i == processDefinitionList.Count - 1 || !string.IsNullOrEmpty(processDefinitionList[i + 1].UserCode)))
                        {
                            ProcessInstance newProcessInstance = new ProcessInstance();
                            newProcessInstance.IsAccountCtrl = false;
                            newProcessInstance.IsCtrl = false;
                            newProcessInstance.UserCode = string.Empty;
                            newProcessInstance.UOM = string.Empty;
                            newProcessInstance.UOMDesc = string.Empty;
                            newProcessInstance.IsParallel = true;
                            newProcessInstance.Qty = null;
                            newProcessInstance.Apply = string.Empty;
                            newProcessInstance.ApplyDesc = string.Empty;
                            newProcessInstance.ApplyQty = null;
                            level += ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                            newProcessInstance.Level = level;
                            CloneHelper.CopyProperty(processInstance, newProcessInstance);
                            newProcessInstanceList.Add(newProcessInstance);
                        }
                    }

                    this.SetApproval(task, newProcessInstanceList, true);
                    /*
                    if (newProcessInstanceList.Where(p => p.Level == nextLevel && p.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED).Count() == 0)
                    {
                        var list = newProcessInstanceList.Where(p => p.Level > nextLevel && p.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED);
                        if (list == null || list.Count() == 0)
                        {
                            nextLevel = ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE;
                        }
                        else
                        {
                            nextLevel = list.Min(p => p.Level);
                        }
                    }
                    */

                    int lastLevel = newProcessInstanceList.Max(p => p.Level);
                    if (nextLevel == lastLevel)
                    {
                        nextLevel = ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE;
                        task.PreLevel = newProcessInstanceList.Where(pi => pi.Level < nextLevel).Max(pi => pi.Level);
                    }
                    else if (newProcessInstanceList.Where(p => p.Level == nextLevel && p.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED).Count() == 0
                        || nextLevel > ISIConstants.CODE_MASTER_WFS_LEVEL4 || newProcessInstanceList.Where(p => p.Level == nextLevel).Count() == 0)
                    {
                        var uomApplyList = hqlMgrE.FindAll<TaskApply>("from TaskApply where TaskCode='" + task.Code + "' and uom is not null and uom !='' ");
                        var processInstanceList = newProcessInstanceList.Where(pi => pi.Level > nextLevel && pi.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED).ToList();
                        nextLevel = GetNextLevel(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE, nextLevel, task.Qty, task.Amount, uomApplyList, processInstanceList, null);
                        task.PreLevel = newProcessInstanceList.Where(pi => pi.Level < nextLevel).Max(pi => pi.Level);
                    }
                    /*
                    else if (nextLevel > ISIConstants.CODE_MASTER_WFS_LEVEL3)
                    {
                        task.PreLevel = nextLevel - ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                    }
                     * */
                    else// if (nextLevel <= ISIConstants.CODE_MASTER_WFS_LEVEL3)
                    {
                        task.PreLevel = ISIConstants.CODE_MASTER_WFS_LEVEL_DEFAULT;
                    }

                    //task.Qty = GenWorkHours(task.Code, task.FailureMode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE, nextLevel, task.TaskApplyList, task.SubmitUser, task.SubmitUserNm, task.SubmitDate.Value, task.WorkHoursUser, task.WorkHoursUserNm, effDate, user);
                    task.Level = GetLevel(ref nextLevel, newProcessInstanceList);

                    if (nextLevel != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
                    {
                        //发送通知需要
                        //发审批通知
                        var userCodeArray = newProcessInstanceList.Where(p => p.Level == nextLevel && !string.IsNullOrEmpty(p.UserCode)).Select(p => p.UserCode).ToArray();
                        ApproveRemind(task, user, userCodeArray);
                    }
                }
            }
            else
            {
                task.Level = ISIConstants.CODE_MASTER_WFS_LEVEL_DEFAULT;
            }

            SetCurrentApprovalUser(task, newProcessInstanceList);
        }

        public void SetCurrentApprovalUser(TaskMstr task, IList<ProcessInstance> newProcessInstanceList)
        {
            if (!task.Level.HasValue || task.Level.Value < ISIConstants.CODE_MASTER_WFS_LEVEL3
                                        || task.Level.Value == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE || string.IsNullOrEmpty(task.ApprovalUser)
                                        || newProcessInstanceList == null || newProcessInstanceList.Count == 0)
            {
                task.CurrentApprovalUser = string.Empty;
                task.CurrentApprovalUserNm = string.Empty;
            }
            else
            {
                StringBuilder userCodes = new StringBuilder("|");
                StringBuilder userNames = new StringBuilder();
                foreach (var p in newProcessInstanceList)
                {
                    if (p.Level == task.Level.Value && p.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED && !string.IsNullOrEmpty(p.UserCode))
                    {
                        if (userCodes.Length > 1)
                        {
                            userCodes.Append(",");
                            userNames.Append(", ");
                        }
                        userCodes.Append(p.UserCode);
                        userNames.Append(p.UserNm);
                    }
                }
                userCodes.Append("|");
                task.CurrentApprovalUser = userCodes.ToString();
                task.CurrentApprovalUserNm = userNames.ToString();
            }
        }


        private int GetLevel(ref int nextLevel, IList<ProcessInstance> processInstanceList)
        {
            if (nextLevel != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE && processInstanceList != null && processInstanceList.Count > 0)
            {
                if (processInstanceList.Count == 2 || processInstanceList[processInstanceList.Count - 2].Level < nextLevel)
                {
                    nextLevel = ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE;
                    return ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE;
                }
            }
            return nextLevel;
        }
        /// <summary>
        /// 审批提醒
        /// </summary>
        /// <param name="task"></param>
        /// <param name="user"></param>
        /// <param name="userCodeArray"></param>
        private void ApproveRemind(TaskMstr task, User user, string[] userCodeArray)
        {
            if (userCodeArray != null && userCodeArray.Length > 0)
            {
                string userCodes = string.Join("','", userCodeArray);
                IList<UserSub> userSubList = new List<UserSub>();
                userSubscriptionMgrE.GenerateUserSub(task.Type, task.TaskSubType.Code, task.Code, user, userSubList, userCodes);

                if (userSubList != null && userSubList.Count > 0)
                {
                    foreach (var userSub in userSubList)
                    {
                        IList<UserSub> userSubList1 = new List<UserSub>();
                        userSubList1.Add(userSub);
                        userSubscriptionMgrE.Remind(task, ISIConstants.ISI_LEVEL_APPROVE, userSubList1, true, user);
                    }
                }
            }
        }


        [Transaction(TransactionMode.Requires)]
        public void ProcessNew(TaskMstr task, string wfsStatus, string approveDesc, string color, IList<object> countersignList, bool isiAdmin, DateTime effDate, bool isEmail, User user)
        {
            IList<ProcessInstance> processInstanceList = null;
            if (task.Level.HasValue && task.Level.Value != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
            {
                //判断流程是否结束
                processInstanceList = hqlMgrE.FindAll<ProcessInstance>("from ProcessInstance where TaskCode='" + task.Code + "' and Level >= " + ISIConstants.CODE_MASTER_WFS_LEVEL3 + " order by Level asc ");

                if (processInstanceList != null || processInstanceList.Count > 0)
                {
                    List<ProcessInstance> thisLevelList = processInstanceList.Where(pi => pi.Level == task.Level.Value && pi.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED).ToList();

                    List<ProcessInstance> thisUserLevelList = processInstanceList.Where(pi => pi.Level == task.Level.Value && pi.UserCode == user.Code && pi.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED).ToList();
                    bool isPermission = thisUserLevelList != null && thisUserLevelList.Count > 0;

                    IList<ProcessInstance> nextUserLevelList = null;
                    //下一级如果也是当前审批人的话自动审批，本级无会签
                    if ((isPermission || isiAdmin)
                            && (thisLevelList.Count == 1 || thisUserLevelList.Where(l => l.ATicket).Count() > 0)
                            && (countersignList == null || countersignList.Count < 3)
                            && (wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE || wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE))
                    {
                        if (thisLevelList.Where(l => l.IsCtrl).Count() > 0)
                        {
                            nextUserLevelList = processInstanceList.Where(pi => pi.Level == (task.Level.Value + ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL + ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL) && pi.UserCode == user.Code && pi.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED).ToList();
                        }
                        else
                        {
                            nextUserLevelList = processInstanceList.Where(pi => pi.Level == (task.Level.Value + ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL) && pi.UserCode == user.Code && pi.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED).ToList();
                        }
                        //下一级审批不能使会签步骤
                        if (nextUserLevelList != null && nextUserLevelList.Count > 0 && nextUserLevelList.Where(l => l.IsCtrl).Count() > 0)
                        {
                            nextUserLevelList = null;
                        }
                    }

                    IList<TaskApply> uomApplyList = null;
                    if (task.Level.Value != ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE)
                    {
                        uomApplyList = hqlMgrE.FindAll<TaskApply>("from TaskApply where TaskCode='" + task.Code + "' and uom is not null and uom !='' ");
                    }

                    int? nextLevel = task.Level;
                    bool isPrePermission = false;

                    //当前级别权限
                    if (isPermission || (isiAdmin && !isEmail))
                    {
                        //退回清空处理时间
                        ProcessReturn(task, wfsStatus, effDate, user, processInstanceList);

                        if (wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE || wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE
                            //终极审批才能不批准                
                                                    || (wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE && task.Level.Value == ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE))
                        {
                            List<ProcessInstance> levelList = isPermission ? thisUserLevelList : thisLevelList;

                            if (nextUserLevelList != null && nextUserLevelList.Count > 0)
                            {
                                levelList.AddRange(nextUserLevelList);
                            }

                            bool isCtrl = false;
                            int levelT = 0;
                            foreach (var processInstance in levelList)
                            {
                                if (processInstance.Level <= levelT)
                                {
                                    continue;
                                }

                                processInstance.Status = wfsStatus;
                                processInstance.ProcessDate = effDate;
                                processInstance.ProcessUser = user.Code;
                                processInstance.ProcessUserNm = user.Name;
                                processInstanceMgrE.UpdateProcessInstance(processInstance);

                                if (processInstance.IsCtrl)
                                {
                                    isCtrl = true;
                                }
                                if (thisLevelList.Count == 1 || processInstance.ATicket || isiAdmin || processInstance.Level >= nextLevel)
                                {
                                    var unapprovedProcessInstanceList = processInstanceList.Where(pi => pi.Level > processInstance.Level && pi.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED).ToList();

                                    nextLevel = GetNextLevel(wfsStatus, task.Level.Value, task.Qty, task.Amount, uomApplyList, unapprovedProcessInstanceList, countersignList);

                                    if (processInstance.ATicket)
                                    {
                                        levelT = processInstance.Level;
                                        //break;
                                    }
                                }
                            }

                            if (nextLevel == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
                            {
                                if (wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE)
                                {
                                    task.Qty = GenWorkHours(task.Code, task.FailureMode, wfsStatus, ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE, uomApplyList, task.SubmitUser, task.SubmitUserNm, task.SubmitDate.Value, task.WorkHoursUser, task.WorkHoursUserNm, effDate, user);
                                    SetTask(task, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE, nextLevel, processInstanceList, effDate, user);
                                }
                                else if (wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE)
                                {
                                    if (task.Level.Value == ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE)
                                    {
                                        SetTask(task, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE, nextLevel, processInstanceList, effDate, user);
                                    }
                                }
                            }
                            else
                            {
                                if (isCtrl && countersignList != null && countersignList.Count >= 3 && countersignList[0] != task.CountersignUser && nextLevel.HasValue)
                                {
                                    //会签                        
                                    var countersignProcessInstance = processInstanceList.Where(p => p.Level == nextLevel).FirstOrDefault();
                                    CreateCountersignUser(task, effDate, user, countersignProcessInstance, countersignList, processInstanceList);
                                }
                                if (wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE)
                                {
                                    SetTask(task, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE, nextLevel, processInstanceList, effDate, user);
                                }
                                else if (wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE)
                                {
                                    SetTask(task, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE, nextLevel, processInstanceList, effDate, user);
                                }
                            }
                        }
                    }
                    //level刚起步，不需要重做；level等于PreLevel 说明有其他人操作过
                    else if (!isEmail && task.Level.Value != ISIConstants.CODE_MASTER_WFS_LEVEL3 && task.PreLevel.HasValue && task.Level.Value != task.PreLevel.Value && (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE))
                    {
                        //前一级是否有权限
                        IList<ProcessInstance> preLevelList = processInstanceList.Where(pi => pi.Level == task.PreLevel.Value && pi.UserCode == user.Code && (pi.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED || pi.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE || pi.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE)).ToList();
                        isPrePermission = preLevelList != null && preLevelList.Count > 0;
                        if (isPrePermission)
                        {
                            //会签
                            if (countersignList != null && countersignList.Count >= 3 && countersignList[0] != task.CountersignUser)
                            {
                                var countersignProcessInstanceList = processInstanceList.Where(pi => pi.Level > task.PreLevel.Value && pi.Level < (task.PreLevel.Value + ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL + ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL) && pi.Status == ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED).ToList();

                                //如果之前已经指定会签人，需要将之前的会签人删除
                                if (countersignProcessInstanceList != null && countersignProcessInstanceList.Count > 1)
                                {
                                    //保留一个位置，因此Skip（1）
                                    this.processInstanceMgrE.DeleteProcessInstance(countersignProcessInstanceList.Skip(1).ToList());
                                    foreach (var countersignProcessInstance in countersignProcessInstanceList)
                                    {
                                        processInstanceList.Remove(countersignProcessInstance);
                                    }
                                }
                                //如果之前已指定会签人，此字段会有值，因此需要清空
                                countersignProcessInstanceList[0].UserCode = string.Empty;
                                //会签   
                                CreateCountersignUser(task, effDate, user, countersignProcessInstanceList[0], countersignList, processInstanceList);
                                task.Level = countersignProcessInstanceList[0].Level;
                            }
                            //退回

                            ProcessReturn(task, wfsStatus, effDate, user, processInstanceList);
                            if (wfsStatus != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
                            {
                                foreach (var preLevel in preLevelList)
                                {
                                    preLevel.Status = wfsStatus;
                                    preLevel.ProcessDate = effDate;
                                    preLevel.ProcessUser = user.Code;
                                    preLevel.ProcessUserNm = user.Name;
                                    this.processInstanceMgrE.UpdateProcessInstance(preLevel);
                                }
                                //清空后续步骤
                                IList<ProcessInstance> nextLevelList = processInstanceList.Where(pi => pi.Level > task.PreLevel.Value && pi.UserCode == user.Code && (pi.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE || pi.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE)).ToList();
                                if (nextLevelList != null && nextLevelList.Count > 0)
                                {
                                    foreach (var nextLevelProcessInstance in nextLevelList)
                                    {
                                        nextLevelProcessInstance.Status = ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED;
                                        nextLevelProcessInstance.ProcessDate = null;
                                        nextLevelProcessInstance.ProcessUser = null;
                                        nextLevelProcessInstance.ProcessUserNm = null;
                                        processInstanceMgrE.UpdateProcessInstance(nextLevelProcessInstance);
                                    }
                                }

                                SetStatus(task, wfsStatus, processInstanceList, effDate, user);
                            }
                        }
                    }



                    if (task.IsUpdate && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
                    {
                        wfDetailMgrE.CreateWFDetail(task.Code, task.Status, task.Level, task.PreLevel, effDate, user);

                        //todo 邮件通知
                        if (!string.IsNullOrEmpty(color))
                        {
                            task.Color = color;
                        }
                    }

                    this.CreateApprove(task.Code, approveDesc, task.Flag, color, effDate, user);

                    if (task.IsUpdate || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
                    {
                        ApproveRemind(task, user, processInstanceList);
                    }
                }
            }

            SetCurrentApprovalUser(task, processInstanceList);

        }

        private void ApproveRemind(TaskMstr task, User user, IList<ProcessInstance> processInstanceList)
        {
            if (processInstanceList != null && processInstanceList.Count > 0)
            {
                string[] userCodeArray = null;
                string userCodes = string.Empty;

                //发送审批，邮件审批
                if (task.Level != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE && task.Level >= ISIConstants.CODE_MASTER_WFS_LEVEL3)
                {
                    //过滤科目控制部门（财务），财务部门不邮件审批，通过后续发送通知
                    //userCodeArray = processInstanceList.Where(p => p.Level == task.Level && !string.IsNullOrEmpty(p.UserCode) && !p.IsAccountCtrl).Select(p => p.UserCode).ToArray();
                    userCodeArray = processInstanceList.Where(p => p.Level == task.Level && !string.IsNullOrEmpty(p.UserCode)).Select(p => p.UserCode).ToArray();
                    ApproveRemind(task, user, userCodeArray);
                }

                //发送通知
                IList<string> remindUserCodeList = new List<string>();
                //退回、不批准、争议 通知流程审批用户
                if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN
                        || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE
                        || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE)
                {
                    if (userCodeArray == null || userCodeArray.Length == 0)
                    {
                        remindUserCodeList = processInstanceList.Where(p => p.Level < task.Level && !string.IsNullOrEmpty(p.UserCode)).Select(p => p.UserCode).ToList();
                    }
                    else
                    {
                        remindUserCodeList = processInstanceList.Where(p => p.Level < task.Level && !string.IsNullOrEmpty(p.UserCode) && !userCodeArray.Contains(p.UserCode)).Select(p => p.UserCode).ToList();
                    }
                }
                remindUserCodeList.Add(processInstanceList[0].CreateUser);

                userCodes = string.Join("','", remindUserCodeList.ToArray());
                IList<UserSub> userSubList = new List<UserSub>();
                userSubscriptionMgrE.GenerateUserSub(task.Type, task.TaskSubType.Code, task.Code, user, userSubList, userCodes);

                userSubscriptionMgrE.Remind(task, ISIConstants.ISI_LEVEL_BASE, userSubList, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateApprove(string taskCode, string approveDesc, string flag, string color, DateTime now, User user)
        {
            if (!string.IsNullOrEmpty(approveDesc))
            {
                TaskStatus taskStatus = new TaskStatus();
                taskStatus.Desc = approveDesc;
                taskStatus.TaskCode = taskCode;
                taskStatus.Type = ISIConstants.CODE_MASTER_ISI_MSG_TYPE_APPROVE;
                taskStatus.Color = color;
                taskStatus.Flag = flag;
                taskStatus.StartDate = now;
                taskStatus.EndDate = now;
                taskStatus.CreateDate = now;
                taskStatus.CreateUser = user.Code;
                taskStatus.CreateUserNm = user.Name;
                taskStatus.LastModifyDate = now;
                taskStatus.LastModifyUser = user.Code;
                taskStatus.LastModifyUserNm = user.Name;
                this.taskStatusMgrE.CreateTaskStatus(taskStatus);
            }
        }

        private void ProcessReturn(TaskMstr task, string wfsStatus, DateTime effDate, User user, IList<ProcessInstance> processInstanceList)
        {
            if (wfsStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
            {
                if (processInstanceList != null && processInstanceList.Count >= 0)
                {
                    foreach (var processInstance in processInstanceList)
                    {
                        processInstance.Status = ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED;
                        processInstance.ProcessDate = null;
                        processInstance.ProcessUser = null;
                        processInstance.ProcessUserNm = null;
                        processInstanceMgrE.UpdateProcessInstance(processInstance);
                    }
                }
                SetTask(task, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN, ISIConstants.CODE_MASTER_WFS_LEVEL_DEFAULT, processInstanceList, effDate, user);
            }
        }

        private void SetTask(TaskMstr task, string status, int? nextLevel, IList<ProcessInstance> processInstanceList, DateTime effDate, User user)
        {
            task.PreLevel = task.Level;
            task.Level = nextLevel;
            SetStatus(task, status, processInstanceList, effDate, user);
        }
        private void SetStatus(TaskMstr task, string status, IList<ProcessInstance> processInstanceList, DateTime effDate, User user)
        {
            task.IsUpdate = true;
            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE && task.Level != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
            {
                task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE;
            }
            else
            {
                task.Status = status;
            }
            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
            {
                task.Level = ISIConstants.CODE_MASTER_WFS_LEVEL_DEFAULT;
                task.PreLevel = ISIConstants.CODE_MASTER_WFS_LEVEL_DEFAULT;
                task.ReturnDate = effDate;
                task.ReturnUser = user.Code;
                task.ReturnUserNm = user.Name;
            }
            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE)
            {
                task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI4;
                task.Color = string.Empty;
                task.ApproveDate = effDate;
                task.ApproveUser = user.Code;
                task.ApproveUserNm = user.Name;
            }
            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE)
            {
                task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI4;
                task.Color = string.Empty;
                task.RefuseDate = effDate;
                task.RefuseUser = user.Code;
                task.RefuseUserNm = user.Name;
            }
            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE)
            {
                task.InDisputeDate = effDate;
                task.InDisputeUser = user.Code;
                task.InDisputeUserNm = user.Name;
            }
            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE)
            {
                task.InApproveDate = effDate;
                task.InApproveUser = user.Code;
                task.InApproveUserNm = user.Name;
            }
            task.LastModifyDate = effDate;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name;
        }
        private void CreateCountersignUser(TaskMstr task, DateTime effDate, User user, ProcessInstance countersignProcessInstance, IList<object> countersignList, IList<ProcessInstance> processInstanceList)
        {
            if (string.IsNullOrEmpty(countersignProcessInstance.UserCode))
            {
                if (processInstanceList.Contains(countersignProcessInstance))
                {
                    processInstanceList.Remove(countersignProcessInstance);
                }
                string taskCode = task.Code;
                task.IsUpdate = true;
                if (countersignList != null && countersignList.Count >= 3)
                {
                    task.CountersignUser = countersignList[0].ToString();
                    task.CountersignUserNm = countersignList[1].ToString();
                    task.IsCountersignSerial = bool.Parse(countersignList[2].ToString());
                }
                string countersignUser = task.CountersignUser;
                string countersignUserNm = task.CountersignUserNm;
                bool isCountersignSerial = task.IsCountersignSerial.Value;

                string[] countersignUserCode = countersignUser.ToString().Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                string[] countersignUserName = countersignUserNm.ToString().Split(ISIConstants.ISI_USERNAME_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);

                int tempLevel = countersignProcessInstance.Level;
                for (int i = 0; i < countersignUserCode.Length; i++)
                {
                    ProcessInstance processInstance = null;
                    if (i == 0)
                    {
                        processInstance = countersignProcessInstance;
                        processInstance.IsCtrl = processInstance.IsCtrl || countersignUserCode[i] == user.Code;
                        processInstance.UserCode = countersignUserCode[i];
                        processInstance.UserNm = countersignUserName[i];
                        processInstance.IsParallel = !isCountersignSerial;
                        processInstance.CreateDate = effDate;
                        processInstance.CreateUser = user.Code;
                        processInstance.CreateUserNm = user.Name;
                        this.processInstanceMgrE.UpdateProcessInstance(processInstance);
                    }
                    else
                    {
                        processInstance = new ProcessInstance();
                        processInstance.TaskCode = taskCode;
                        processInstance.IsRemind = countersignProcessInstance.IsRemind;

                        processInstance.Desc1 = countersignProcessInstance.Desc1;
                        if (isCountersignSerial)
                        {
                            tempLevel += ISIConstants.CODE_MASTER_WF_COUNTERSIGN_LEVEL_INTERVAL;
                        }
                        processInstance.Level = tempLevel;
                        processInstance.TaskSubType = countersignProcessInstance.TaskSubType;
                        processInstance.UserCode = countersignUserCode[i];
                        processInstance.UserNm = countersignUserName[i];
                        processInstance.IsOpt = countersignProcessInstance.IsOpt;
                        processInstance.IsAccountCtrl = countersignProcessInstance.IsAccountCtrl;
                        processInstance.IsCtrl = countersignProcessInstance.IsCtrl || countersignUserCode[i] == user.Code;
                        processInstance.IsApprove = countersignProcessInstance.IsApprove;
                        processInstance.IsParallel = !isCountersignSerial;
                        processInstance.ATicket = countersignProcessInstance.ATicket;

                        processInstance.UOM = countersignProcessInstance.UOM;
                        processInstance.UOMDesc = countersignProcessInstance.UOMDesc;
                        processInstance.Qty = countersignProcessInstance.Qty;

                        processInstance.Apply = countersignProcessInstance.Apply;
                        processInstance.ApplyDesc = countersignProcessInstance.ApplyDesc;
                        processInstance.ApplyQty = countersignProcessInstance.ApplyQty;

                        processInstance.Status = ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED;
                        processInstance.CreateDate = effDate;
                        processInstance.CreateUser = user.Code;
                        processInstance.CreateUserNm = user.Name;
                        processInstanceMgrE.CreateProcessInstance(processInstance);
                    }
                    processInstanceList.Add(processInstance);
                }

                processInstanceList = processInstanceList.OrderBy(p => p.Level).ToList();

                SetApproval(task);
            }
        }

        private int GetNextLevel(string wfsStatus, int level, decimal? qty, decimal? amount, IList<TaskApply> uomApplyList, IList<ProcessInstance> processInstanceList, IList<object> countersignList)
        {
            if (level == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE || processInstanceList == null || processInstanceList.Count == 0)
            {
                return ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE;
            }
            int nextLevel = 0;
            if (countersignList == null || countersignList.Count < 3)
            {
                //如果下一级是会签用户为空，跳过
                var t = processInstanceList.Where(pi => !string.IsNullOrEmpty(pi.UserCode) && pi.Level > level).ToList();

                if (t == null || t.Count == 0)
                {
                    nextLevel = ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE;
                }
                else
                {
                    nextLevel = t.Min(pi => pi.Level);
                }
            }
            else
            {
                nextLevel = processInstanceList.Min(pi => pi.Level);
            }

            //金额和工时小于设定，审批完成,争议不判断条件，到下一级审批
            if (wfsStatus != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE
                    && (qty.HasValue || amount.HasValue || uomApplyList != null && uomApplyList.Count > 0))
            {
                var processInstanceList1 = processInstanceList.Where(pi => pi.Level == nextLevel && (pi.Qty.HasValue || pi.ApplyQty.HasValue));
                if (processInstanceList1 != null && processInstanceList1.Count() > 0)
                {
                    foreach (var pi in processInstanceList1)
                    {
                        if (string.IsNullOrEmpty(pi.UOM) && qty.HasValue && qty.Value > pi.Qty.Value
                                || string.IsNullOrEmpty(pi.Apply) && amount.HasValue && amount.Value > pi.ApplyQty.Value)
                        {
                            return nextLevel;
                        }

                        if (uomApplyList != null && uomApplyList.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(pi.UOM))
                            {
                                var uomApply = uomApplyList.Where(u => u.UOM == pi.UOM).ToList();
                                if (uomApply.Where(u => u.Qty > pi.Qty).Count() > 0)
                                {
                                    return nextLevel;
                                }
                            }

                            if (!string.IsNullOrEmpty(pi.Apply))
                            {
                                var uomApply = uomApplyList.Where(u => u.Apply == pi.Apply).ToList();
                                if (uomApply.Where(u => u.Qty > pi.ApplyQty).Count() > 0)
                                {
                                    return nextLevel;
                                }
                            }
                        }
                    }

                    return ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE;
                }
            }

            return nextLevel;
        }
        /// <summary>
        /// 部门审批
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="taskSubTypeCode"></param>
        /// <param name="nextLevel"></param>
        /// <param name="userList"></param>
        /// <param name="isCtrl"></param>
        /// <param name="user"></param>
        /// <param name="effDate"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Requires)]
        public IList<ProcessInstance> AddDeptProcessInstance(string taskCode, string taskSubTypeCode, int nextLevel, string userList, bool isRemind, bool isCtrl, User user, DateTime effDate)
        {
            IList<ProcessInstance> processInstanceList = new List<ProcessInstance>();
            IDictionary<string, object> param = new Dictionary<string, object>();
            string[] userCodeArr = userList.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            param.Add("UserCode", userCodeArr);
            IList<object[]> userNameObj = hqlMgrE.FindAll<object[]>("select u.Code,u.FirstName,u.LastName from User u where u.Code in (:UserCode) ", param);
            foreach (var u in userNameObj)
            {
                ProcessInstance processInstance = new ProcessInstance();
                processInstance.TaskCode = taskCode;
                processInstance.Desc1 = "部门审批";
                processInstance.Level = ISIConstants.CODE_MASTER_WFS_LEVEL3;
                processInstance.TaskSubType = taskSubTypeCode;
                processInstance.UserCode = u[0].ToString();
                processInstance.UserNm = (u[1] == null ? string.Empty : u[1].ToString()) + (u[2] == null ? string.Empty : " " + u[2].ToString());
                processInstance.IsOpt = false;
                processInstance.IsAccountCtrl = false;
                processInstance.IsCtrl = isCtrl;
                processInstance.IsApprove = true;
                processInstance.IsParallel = userNameObj.Count > 1;
                processInstance.ATicket = true;
                processInstance.UOM = null;
                processInstance.IsRemind = isRemind;
                processInstance.UOMDesc = null;
                processInstance.Qty = null;
                processInstance.Apply = null;
                processInstance.ApplyDesc = null;
                processInstance.ApplyQty = null;
                //终结审批人、部门经理提交
                if (nextLevel == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE || (u[0].ToString() == user.Code && !processInstance.IsCtrl))
                {
                    processInstance.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE;
                    processInstance.ProcessDate = effDate;
                    processInstance.ProcessUser = user.Code;
                    processInstance.ProcessUserNm = user.Name;
                }
                else
                {
                    processInstance.Status = ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED;
                }
                processInstance.CreateDate = effDate;
                processInstance.CreateUser = user.Code;
                processInstance.CreateUserNm = user.Name;
                processInstanceList.Add(processInstance);
            }
            return processInstanceList;
        }

        /// <summary>
        /// 成本中心审批
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="taskSubTypeCode"></param>
        /// <param name="nextLevel"></param>
        /// <param name="userList"></param>
        /// <param name="isCtrl"></param>
        /// <param name="user"></param>
        /// <param name="effDate"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Requires)]
        public IList<ProcessInstance> AddCostCenterProcessInstance(string taskCode, string taskSubTypeCode, int nextLevel, string userList, bool isRemind, bool isCtrl, User user, DateTime effDate)
        {
            IList<ProcessInstance> processInstanceList = new List<ProcessInstance>();
            IDictionary<string, object> param = new Dictionary<string, object>();
            string[] userCodeArr = userList.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            param.Add("UserCode", userCodeArr);
            IList<object[]> userNameObj = hqlMgrE.FindAll<object[]>("select u.Code,u.FirstName,u.LastName from User u where u.Code in (:UserCode) ", param);
            foreach (var u in userNameObj)
            {
                ProcessInstance processInstance = new ProcessInstance();
                processInstance.TaskCode = taskCode;
                processInstance.Desc1 = "成本中心审批";
                processInstance.Level = ISIConstants.CODE_MASTER_WFS_LEVEL4;
                processInstance.TaskSubType = taskSubTypeCode;
                processInstance.UserCode = u[0].ToString();
                processInstance.UserNm = (u[1] == null ? string.Empty : u[1].ToString()) + (u[2] == null ? string.Empty : " " + u[2].ToString());
                processInstance.IsOpt = false;
                processInstance.IsAccountCtrl = false;
                processInstance.IsCtrl = isCtrl;
                processInstance.IsApprove = true;
                processInstance.IsParallel = userNameObj.Count > 1;
                processInstance.ATicket = false;
                processInstance.UOM = null;
                processInstance.IsRemind = isRemind;
                processInstance.UOMDesc = null;
                processInstance.Qty = null;
                processInstance.Apply = null;
                processInstance.ApplyDesc = null;
                processInstance.ApplyQty = null;
                //终结审批人、部门经理提交
                if (nextLevel == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE || (u[0].ToString() == user.Code && !processInstance.IsCtrl))
                {
                    processInstance.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE;
                    processInstance.ProcessDate = effDate;
                    processInstance.ProcessUser = user.Code;
                    processInstance.ProcessUserNm = user.Name;
                }
                else
                {
                    processInstance.Status = ISIConstants.CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED;
                }
                processInstance.CreateDate = effDate;
                processInstance.CreateUser = user.Code;
                processInstance.CreateUserNm = user.Name;
                processInstanceList.Add(processInstance);
            }
            return processInstanceList;
        }

        [Transaction(TransactionMode.Requires)]
        private ProcessInstance AddProcessInstance(string taskCode, string taskSubTypeCode, string userCode, string userName, User user, DateTime effDate)
        {
            ProcessInstance processInstance = new ProcessInstance();
            processInstance.TaskCode = taskCode;
            processInstance.Desc1 = "提交";
            processInstance.Level = ISIConstants.CODE_MASTER_WFS_LEVEL2;
            processInstance.TaskSubType = taskSubTypeCode;
            processInstance.UserCode = userCode;
            processInstance.UserNm = userName;
            processInstance.ATicket = false;
            processInstance.IsOpt = false;
            processInstance.IsAccountCtrl = false;
            processInstance.IsCtrl = false;
            processInstance.IsRemind = false;
            processInstance.IsApprove = false;
            processInstance.IsParallel = false;
            processInstance.UOM = null;
            processInstance.UOMDesc = null;
            processInstance.Qty = null;
            processInstance.Apply = null;
            processInstance.ApplyDesc = null;
            processInstance.ApplyQty = null;
            processInstance.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT;
            processInstance.CreateDate = effDate;
            processInstance.CreateUser = user.Code;
            processInstance.CreateUserNm = user.Name;
            processInstance.ProcessDate = effDate;
            processInstance.ProcessUser = user.Code;
            processInstance.ProcessUserNm = user.Name;
            return processInstance;
        }

        [Transaction(TransactionMode.Requires)]
        public void Create(TaskMstr task, DateTime effDate, User user)
        {
            if (task.IsWF && task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE)
            {
                //加入创建人
                ProcessInstance processInstance = new ProcessInstance();
                processInstance.TaskCode = task.Code;
                processInstance.Desc1 = "创建";
                processInstance.Level = ISIConstants.CODE_MASTER_WFS_LEVEL1;
                processInstance.TaskSubType = task.TaskSubType.Code;
                processInstance.UserCode = task.CreateUser;
                processInstance.UserNm = task.CreateUserNm;
                processInstance.IsOpt = false;
                processInstance.IsAccountCtrl = false;
                processInstance.IsCtrl = false;
                processInstance.IsApprove = false;
                processInstance.IsParallel = false;
                processInstance.IsRemind = false;
                processInstance.UOM = null;
                processInstance.UOMDesc = null;
                processInstance.Qty = null;
                processInstance.Apply = null;
                processInstance.ApplyDesc = null;
                processInstance.ApplyQty = null;
                processInstance.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE;
                processInstance.CreateDate = effDate;
                processInstance.CreateUser = user.Code;
                processInstance.CreateUserNm = user.Name;
                processInstance.ProcessDate = effDate;
                processInstance.ProcessUser = user.Code;
                processInstance.ProcessUserNm = user.Name;
                processInstanceMgrE.CreateProcessInstance(processInstance);
            }
        }
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class WorkflowMgrE : com.Sconit.ISI.Service.Impl.WorkflowMgr, IWorkflowMgrE
    {
    }
}

#endregion Extend Class