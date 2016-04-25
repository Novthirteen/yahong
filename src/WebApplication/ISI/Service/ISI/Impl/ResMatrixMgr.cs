using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Service;
using com.Sconit.Service.Ext;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using NHibernate.Expression;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class ResMatrixMgr : ResMatrixBaseMgr, IResMatrixMgr
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger("Application");

        public IAttachmentDetailMgrE attachmentDetailMgr { get; set; }
        public IResSopMgrE resSopMgr { get; set; }
        public IResRoleMgrE resRoleMgr { get; set; }
        public IResWokShopMgrE resWokShopMgr { get; set; }
        public IResMatrixLogMgrE resMatrixLogMgr { get; set; }
        public ICriteriaMgrE criteriaMgr { get; set; }
        public IResPatrolMgrE resPatrolMgr { get; set; }
        public ITaskMgrE taskMgr { get; set; }
        public ITaskSubTypeMgrE taskSubTypeMgr { get; set; }
        public IUserMgrE userMgr { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgr { get; set; }
        public ISmtpMgrE smtpMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        public ISqlHelperMgrE sqlHelperMgr { get; set; }

        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public override void CreateResMatrix(ResMatrix entity)
        {
            entity.Responsibility = entity.Responsibility.Trim();
            entity.NextPatrolTime = DateTime.Now;
            if (entity.Operate.HasValue)
            {
                var resSop = resSopMgr.LoadResSop(entity.WorkShop, entity.Operate.Value);
                if (resSop == null)
                {
                    throw new Exception(string.Format("没有找到作业区{0}工位{1}对应的工艺流程", entity.WorkShop, entity.Operate));
                }
            }

            CreateResMatrixLog(entity, "New");
            base.CreateResMatrix(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public override ResMatrix LoadResMatrix(int id)
        {
            var resMatrix = base.LoadResMatrix(id);
            resMatrix.OldOperate = resMatrix.Operate;
            //resMatrix.OldPriority = resMatrix.Priority;
            resMatrix.OldRole = resMatrix.Role;
            resMatrix.OldWorkShop = resMatrix.WorkShop;
            resMatrix.OldResponsibility = resMatrix.Responsibility;
            return resMatrix;
        }

        private void CreateResMatrixLog(ResMatrix resMatrix, string action)
        {
            return;

        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteResMatrix(ResMatrix entity)
        {
            CreateResMatrixLog(entity, "Delete");
            base.DeleteResMatrix(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteResMatrix(IList<ResMatrix> entityList)
        {
            foreach (var entity in entityList)
            {
                CreateResMatrixLog(entity, "Delete");
            }
            base.DeleteResMatrix(entityList);
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteResMatrix(IList<int> pkList)
        {
            foreach (var id in pkList)
            {
                var entity = this.LoadResMatrix(id);
                CreateResMatrixLog(entity, "Delete");
            }
            base.DeleteResMatrix(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteResMatrix(int id)
        {
            var entity = this.LoadResMatrix(id);
            CreateResMatrixLog(entity, "Delete");
            base.DeleteResMatrix(id);
        }

        [Transaction(TransactionMode.Requires)]
        public override void UpdateResMatrix(ResMatrix entity)
        {
            entity.Responsibility = entity.Responsibility.Trim();
            var resRole = resRoleMgr.LoadResRole(entity.Role);
            var workShop = resWokShopMgr.LoadResWokShop(entity.WorkShop);
            var resSop = new ResSop();
            var attachIds = string.Empty;
            if (entity.Operate.HasValue)
            {
                resSop = resSopMgr.LoadResSop(entity.WorkShop, entity.Operate.Value);
                if (resSop == null)
                {
                    throw new Exception(string.Format("没有找到作业区{0}工位{1}对应的工艺流程", entity.WorkShop, entity.Operate));
                }

                var attachMentList = this.attachmentDetailMgr.GetResSopAttachment(resSop.Id.ToString());
                if (attachMentList != null)
                {
                    foreach (var attachMent in attachMentList)
                    {
                        if (attachIds == string.Empty)
                        {
                            attachIds = attachMent.Id.ToString();
                        }
                        else
                        {
                            attachIds += ("," + attachMent.Id.ToString());
                        }
                    }
                }
            }

            string logs = string.Empty;
            //var oldResMatrix = this.LoadResMatrix(entity.Id);
            if (entity.OldWorkShop != entity.WorkShop)
            {
                logs += string.Format("工作区由{0}变更为{1}", entity.OldWorkShop, entity.WorkShop);
            }
            if (entity.OldOperate != entity.Operate)
            {
                logs += string.Format("工位由{0}变更为{1}", entity.OldOperate, entity.Operate);
            }
            if (entity.OldRole != entity.Role)
            {
                logs += string.Format("岗位由{0}变更为{1}", entity.OldRole, entity.Role);
            }

            //if (entity.OldResponsibility != entity.Responsibility)
            //{
            //    StringBuilder str = new StringBuilder();
            //    str.AppendLine("职责由");
            //    str.AppendLine(entity.OldResponsibility);
            //    str.AppendLine("变更为");
            //    str.AppendLine(entity.Responsibility);
            //    logs += str.ToString();
            //}
            if (logs != string.Empty)
            {
                DetachedCriteria criteria = DetachedCriteria.For(typeof(ResUser));
                criteria.Add(Expression.Eq("MatrixId", entity.Id));
                var resUserList = criteriaMgr.FindAll<ResUser>(criteria);
                if (resUserList != null && resUserList.Count > 0)
                {
                    foreach (var resUser in resUserList)
                    {
                        var resMatrixLog = new ResMatrixLog();
                        resMatrixLog.Action = "Update";
                        resMatrixLog.AttachmentIds = attachIds;
                        resMatrixLog.CreateDate = entity.LastModifyDate;
                        resMatrixLog.CreateUser = entity.LastModifyUser;
                        resMatrixLog.Operate = entity.Operate;
                        resMatrixLog.OperateDesc = resSop.OperateDesc;
                        resMatrixLog.Responsibility = entity.Responsibility;
                        resMatrixLog.Priority = resUser.Priority;
                        resMatrixLog.Role = entity.Role;
                        resMatrixLog.RoleName = resRole.Name;
                        resMatrixLog.RoleType = resRole.RoleType;
                        resMatrixLog.SkillLevel = resUser.SkillLevel;
                        resMatrixLog.WorkShop = entity.WorkShop;
                        resMatrixLog.WorkShopName = workShop.Name;
                        resMatrixLog.ResMatrixId = entity.Id;
                        resMatrixLog.UserCode = resUser.UserCode;
                        resMatrixLog.UserName = resUser.UserName;
                        resMatrixLog.StartDate = resUser.StartDate;
                        resMatrixLog.EndDate = resUser.EndDate;
                        resMatrixLog.Logs = logs;
                        resMatrixLog.Seq = entity.Seq;
                        resMatrixLog.OldResponsibility = entity.OldResponsibility;

                        resMatrixLogMgr.CreateResMatrixLog(resMatrixLog);
                    }
                }
            }

            base.UpdateResMatrix(entity);
        }
        [Transaction(TransactionMode.Requires)]
        public void CreateTask(User user)
        {
            CreateTask(user, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateTask(User user, DateTime currentDateTime)
        {
            #region 获取数据
            DataTable userTable = sqlHelperMgr.GetDatasetBySql(
                    @"select b.WorkShop as WorkShop,a.USR_Code as UserCode,a.USR_FirstName+' '+isnull(a.USR_LastName,'') as UserName
                    from ACC_User a with(nolock),ISI_ResMatrix b with(nolock),ISI_ResUser c with(nolock)
                    where a.USR_Code = c.UserCode and b.Id= c.MatrixId and a.IsActive= 1
                    and c.EndDate>GETDATE() and b.NeedPatrol=1 and c.NeedPatrol=1
                    group by b.WorkShop,a.USR_Code,a.USR_FirstName,a.USR_LastName
                    order by b.WorkShop desc ").Tables[0];
            var workShopUserDic = IListHelper.DataTableToList<WorkShopUser>(userTable)
                .GroupBy(p => p.WorkShop).ToDictionary(d => d.Key, d => d.ToList());

            var resTaskDetTable = sqlHelperMgr.GetDatasetBySql(
                    @"select * from
                    (select *, ROW_NUMBER() over(partition by TaskCode order by Id desc) as rowNum
                    from ISI_ResTaskDet with(nolock)
                    ) ranked where ranked.rowNum <= 1 ").Tables[0];
            var resTaskDetDic = IListHelper.DataTableToList<ResTaskDet>(resTaskDetTable).ToDictionary(d => d.TaskCode, d => d);

            var criteria = DetachedCriteria.For(typeof(ResUser));
            criteria.Add(Expression.Le("StartDate", currentDateTime));
            criteria.Add(Expression.Gt("EndDate", currentDateTime));
            criteria.Add(Expression.Eq("NeedPatrol", true));
            var resUserDic = (criteriaMgr.FindAll<ResUser>(criteria) ?? new List<ResUser>())
                .GroupBy(p => p.MatrixId, (k, g) => new
                {
                    k,
                    List = g.ToList()
                }).ToDictionary(d => d.k, d => d.List);

            //工艺流程 =>附件
            var resSopList = resSopMgr.GetAllResSop() ?? new List<ResSop>();
            //岗位类型
            var resRoleDic = (resRoleMgr.GetAllResRole() ?? new List<ResRole>()).Where(p => p.IsActive)
                .ToDictionary(d => d.Code, d => d);
            //方阵设置
            var resMatrixList = (this.GetAllResMatrix() ?? new List<ResMatrix>()).Where(p => p.NeedPatrol).ToList();

            //作业区
            var workShopDic = (this.resWokShopMgr.GetAllResWokShop() ?? new List<ResWokShop>())
                .Where(p => p.IsActive)
                .ToDictionary(d => d.Code, d => d);

            var resTaskList = this.genericMgr.FindAll<ResTask>("select a from ResTask a,User b where a.UserCode=b.Code and b.IsActive=1 order by TaskCode desc")
                ?? new List<ResTask>();

            #endregion

            #region 获取巡查用户对象列表
            List<PatrolUser> patrolUserList = new List<PatrolUser>();

            //月 每月1日  /年 每年的10月1日
            foreach (var resMatrix in resMatrixList)
            {
                resMatrix.TaskSubType = string.IsNullOrEmpty(resMatrix.TaskSubType) ? "ZRFZ" : resMatrix.TaskSubType;
                if (resMatrix.NextPatrolTime < currentDateTime || true)
                {
                    DateTime nextWindowTime = currentDateTime;
                    if (resMatrix.TimePeriodType == BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_YEAR)
                    {
                        nextWindowTime = DateTime.Parse(currentDateTime.ToString("yyyy-10-01"));
                    }
                    else if (resMatrix.TimePeriodType == BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_MONTH)
                    {
                        nextWindowTime = DateTime.Parse(currentDateTime.AddMonths(1).ToString("yyyy-MM-01"));
                    }
                    else
                    {
                        resMatrix.TimePeriodType = BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_WEEK;
                        nextWindowTime = DateTimeHelper.GetWeekStart(currentDateTime).AddDays(7);
                    }

                    resMatrix.NextPatrolTime = nextWindowTime;
                    base.UpdateResMatrix(resMatrix);
                    if (!workShopDic.ContainsKey(resMatrix.WorkShop))
                    {
                        continue;
                    }

                    var resUserList = resUserDic.ValueOrDefault(resMatrix.Id) ?? new List<ResUser>();
                    foreach (var resUser in resUserList)
                    {
                        PatrolUser patrolUser = new PatrolUser();
                        patrolUser.UserCode = resUser.UserCode;
                        patrolUser.WindowTime = currentDateTime;
                        patrolUser.ResMatrix = resMatrix;
                        patrolUser.ResRole = resRoleDic.ValueOrDefault(resMatrix.Role);
                        if (patrolUser.ResRole == null)
                        {
                            continue;
                        }
                        patrolUser.ResSopList = resSopList.Where(p => p.WorkShop == resMatrix.WorkShop
                            && (p.Operate == resMatrix.Operate || !resMatrix.Operate.HasValue)
                            && workShopDic.ContainsKey(p.WorkShop)).ToList();
                        foreach (var resSop in patrolUser.ResSopList)
                        {
                            resSop.ResWokShop = workShopDic.ValueOrDefault(resSop.WorkShop);
                        }
                        patrolUserList.Add(patrolUser);
                    }
                }
            }
            #endregion

            #region 不分组
            var singlePatolUserList = patrolUserList.Where(p => p.ResMatrix.IsIndependentTask)
                .OrderBy(p => p.ResMatrix.TimePeriodType).ThenBy(p => p.ResMatrix.Seq).ThenBy(p => p.ResMatrix.Responsibility).ToList();
            foreach (var patrolUser in singlePatolUserList)
            {
                StringBuilder str = new StringBuilder();
                //巡查的内容
                var resWokShop = workShopDic[patrolUser.ResMatrix.WorkShop];
                str.AppendLine(string.Format("区域:{0}-{1}",
                    patrolUser.ResMatrix.TimePeriodType, resWokShop.CodeName, patrolUser.ResMatrix.Operate, patrolUser.ResRole.CodeName));
                str.AppendLine(patrolUser.ResMatrix.Responsibility.Trim());

                var windowTime = patrolUser.ResMatrix.NextPatrolTime;

                List<WorkShopUser> userList = new List<WorkShopUser>();
                var currentUser = workShopUserDic.SelectMany(p => p.Value).FirstOrDefault(p => string.Equals(p.UserCode, patrolUser.UserCode, StringComparison.OrdinalIgnoreCase));
                if (currentUser == null)
                {
                    log.WarnFormat("用户{0}被禁用,职责没有被禁用", patrolUser.UserCode);
                    continue;
                }
                userList.Add(currentUser);
                GetResUserList(0, workShopDic, resWokShop, workShopUserDic, ref userList);

                var resTask = resTaskList.FirstOrDefault(p => p.ResId == patrolUser.ResMatrix.Id && p.UserCode == patrolUser.UserCode);

                CreateNewTask(user, str.ToString(), patrolUser.WindowTime, windowTime, str.ToString(),
                    patrolUser.ResMatrix.TimePeriodType, patrolUser.ResMatrix.TaskSubType, patrolUser.ResMatrix.Id,
                    patrolUser.ResRole, resTask, userList, resTaskDetDic);
            }
            #endregion

            #region 分组:时间类型/岗位/人员/分类/窗口时间
            var groupPatrolUserList = patrolUserList.Where(p => !p.ResMatrix.IsIndependentTask)
                .GroupBy(p => new { p.ResMatrix.TimePeriodType, p.ResRole, p.UserCode, p.ResMatrix.TaskSubType, p.WindowTime });
            foreach (var groupPatrolUser in groupPatrolUserList)
            {
                StringBuilder str = new StringBuilder();
                StringBuilder fullstr = new StringBuilder();
                var resMatrixs = groupPatrolUser.Select(p => p.ResMatrix).OrderBy(p => p.Seq).ToList();

                var groupResMatrixs = resMatrixs.GroupBy(p => new { p.WorkShop, p.Operate });
                int maxLength = (int)(2000 / resMatrixs.Count);

                var gKey = groupPatrolUser.Key;
                var windowTime = groupPatrolUser.First().ResMatrix.NextPatrolTime;
                var resTask = resTaskList.FirstOrDefault(p =>
                    p.TimePeriodType == gKey.TimePeriodType && string.Equals(p.ResRole, gKey.ResRole.Code, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(p.UserCode, gKey.UserCode, StringComparison.OrdinalIgnoreCase) && p.TaskSubType == gKey.TaskSubType
                );

                List<WorkShopUser> userList = new List<WorkShopUser>();

                var currentUser = workShopUserDic.SelectMany(p => p.Value).FirstOrDefault(p => string.Equals(p.UserCode, gKey.UserCode, StringComparison.OrdinalIgnoreCase));
                if (currentUser == null)
                {
                    log.WarnFormat("用户{0}被禁用,职责没有被禁用", gKey.UserCode);
                    continue;
                }
                userList.Add(currentUser);
                foreach (var groupResMatrix in groupResMatrixs)
                {
                    int i = 0;
                    var resWokShop = workShopDic[groupResMatrix.Key.WorkShop];
                    str.AppendLine(string.Format("区域:{0}-{1}", resWokShop.CodeName, groupResMatrix.Key.Operate));
                    fullstr.AppendLine(string.Format("区域:{0}-{1}", resWokShop.CodeName, groupResMatrix.Key.Operate));
                    foreach (var resMatrix in groupResMatrix)
                    {
                        i++;
                        string contentLines = string.Format("{0}.{1}", i, resMatrix.Responsibility.Trim());
                        fullstr.AppendLine(contentLines);
                        if (contentLines.Length > maxLength)
                        {
                            contentLines = contentLines.Substring(0, maxLength) + "...";
                        }
                        str.AppendLine(contentLines);
                    }
                    GetResUserList(0, workShopDic, resWokShop, workShopUserDic, ref userList);
                }
                CreateNewTask(user, str.ToString(), gKey.WindowTime, windowTime, fullstr.ToString(),
                    gKey.TimePeriodType, gKey.TaskSubType, 0, gKey.ResRole, resTask, userList, resTaskDetDic);
            }
            #endregion
        }

        private void GetResUserList(int count, Dictionary<string, ResWokShop> resWokShopDic, ResWokShop resWokShop,
           Dictionary<string, List<WorkShopUser>> workShopUserDic, ref List<WorkShopUser> userCodeList)
        {
            count++;
            if (resWokShop != null && !string.IsNullOrEmpty(resWokShop.ParentCode) && count < 10)
            {
                var parentuser = workShopUserDic.ValueOrDefault(resWokShop.ParentCode);
                if (parentuser != null)
                {
                    userCodeList.AddRange(parentuser);
                    userCodeList = userCodeList.Distinct().ToList();
                }
                var parentResWokShop = resWokShopDic.ValueOrDefault(resWokShop.ParentCode);
                GetResUserList(count, resWokShopDic, parentResWokShop, workShopUserDic, ref userCodeList);
            }
        }

        private void CreateNewTask(User user, string desc1, DateTime startTime, DateTime windowTime,
            string fullDesc, string timePeriodType, string taskSubType, int resId, ResRole resRole, ResTask resTask,
            List<WorkShopUser> userList, Dictionary<string, ResTaskDet> resTaskDetDic)
        {
            try
            {
                userList = userList.Where(p => p != null).Take(30).ToList();
                if (!userList.Any())
                {
                    log.ErrorFormat("desc1:{0},没有对应的人员", desc1);
                    return;
                }
                string subject = string.Format("({0})责任方阵巡查自动任务,岗位{1}", timePeriodType, resRole.CodeName);
                string userCodes = ISIConstants.ISI_LEVEL_SEPRATOR
                    + string.Join(ISIConstants.ISI_USER_SEPRATOR, userList.Select(p => p.UserCode.ToLower()).Distinct().ToArray())
                    + ISIConstants.ISI_LEVEL_SEPRATOR;
                string userNames = string.Join(ISIConstants.ISI_USER_SEPRATOR, userList.Select(p => p.UserName).ToArray());

                if (resTask == null)
                {
                    TaskMstr task = new TaskMstr();
                    task.Priority = BusinessConstants.CODE_MASTER_ISSUE_PRIORITY_NORMAL;
                    //task.BackYards = taskMstr != null ? taskMstr.Code : null;
                    task.Desc1 = desc1;

                    if (task.Desc1.Length > 2000)
                    {
                        task.Desc1 = task.Desc1.Substring(0, 1990) + "...";
                    }

                    task.Subject = subject;
                    taskSubType = string.IsNullOrEmpty(taskSubType) ? "ZRFZ" : taskSubType;
                    task.TaskSubType = taskSubTypeMgr.LoadTaskSubType(taskSubType);
                    task.UserName = user.Name;
                    task.Email = user.Email;
                    task.MobilePhone = user.MobliePhone;
                    task.TaskAddress = user.Address;
                    task.Type = ISIConstants.ISI_TASK_TYPE_RESMATRIX;
                    task.IsAutoRelease = true;

                    task.AssignStartUser = userCodes;
                    task.AssignStartUserNm = userNames;

                    //task.PatrolTime = patrolTime;
                    task.PlanStartDate = startTime;
                    task.PlanCompleteDate = windowTime;
                    //task.IsNoSend = true;
                    taskMgr.CreateTask(task, user);
                    taskMgr.AssignTask(task.Code, user);

                    resTask = new ResTask();
                    resTask.CreateDate = DateTime.Now;
                    resTask.LastModifyDate = DateTime.Now;
                    resTask.ResRole = resRole.Code;
                    resTask.TaskCode = task.Code;
                    resTask.TaskSubType = taskSubType;
                    resTask.TimePeriodType = timePeriodType;
                    resTask.UserCode = userList.First().UserCode;
                    resTask.ResId = resId;
                    this.genericMgr.Create(resTask);

                    var resTaskDet = new ResTaskDet();
                    resTaskDet.CreateDate = DateTime.Now;
                    resTaskDet.Subject = subject;
                    resTaskDet.Desc1 = fullDesc;
                    resTaskDet.TaskCode = task.Code;
                    this.genericMgr.Create(resTaskDet);
                }
                else
                {
                    var taskMstr = this.genericMgr.FindById<TaskMstr>(resTask.TaskCode);
                    taskMstr.Desc1 = desc1;
                    taskMstr.Subject = subject;
                    taskMstr.PlanStartDate = startTime;
                    taskMstr.PlanCompleteDate = windowTime;
                    taskMstr.LastModifyDate = DateTime.Now;
                    taskMstr.Color = "green";
                    taskMstr.AssignStartUser = userCodes;
                    taskMstr.AssignStartUserNm = userNames;

                    this.genericMgr.Update(taskMstr);
                    if (taskMstr.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                    {
                        taskMgr.RejectTask(resTask.TaskCode, user);
                    }
                    else if (taskMstr.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE)
                    {
                        taskMgr.OpenTask(resTask.TaskCode, user);
                    }

                    //记录日志
                    var resTaskDet = resTaskDetDic.ValueOrDefault(taskMstr.Code);
                    if (resTaskDet != null && resTaskDet.Desc1 == fullDesc)
                    {
                        //nothing to do
                    }
                    else
                    {
                        var newResTaskDet = new ResTaskDet();
                        newResTaskDet.Subject = subject;
                        newResTaskDet.Desc1 = fullDesc;
                        newResTaskDet.CreateDate = DateTime.Now;
                        newResTaskDet.TaskCode = resTask.TaskCode;
                        this.genericMgr.Create(newResTaskDet);
                    }
                }
            }
            catch (Exception e)
            {
                //log
                int _resId = resId;
                throw e;
            }
        }

        public void SendResChangeLog()
        {
            var startDate = DateTime.Now.Date.AddDays(-6);
            var endDate = DateTime.Now.Date.AddDays(1);

            DetachedCriteria criteria = DetachedCriteria.For(typeof(ResMatrixLog));
            criteria.Add(Expression.Ge("CreateDate", startDate));
            criteria.Add(Expression.Lt("CreateDate", endDate));
            criteria.AddOrder(Order.Asc("UserCode"));
            var resMatrixLogList = criteriaMgr.FindAll<ResMatrixLog>(criteria) ?? new List<ResMatrixLog>();
            var listBody = new StringBuilder();

            #region 全部日志
            var entityPre = entityPreferenceMgr.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_ISI_RESSUBSCRIBE);
            if (entityPre != null)
            {
                var userCodeList = entityPre.Value.Split(',');
                if (resMatrixLogList.Count > 0)
                {
                    GetListBody(resMatrixLogList, listBody);
                }
                else
                {
                    listBody.Append("此时间段无职责变更.");
                }
                foreach (var userCode in userCodeList)
                {
                    sendUserMail(startDate, endDate, listBody, userCode);
                }
            }
            #endregion

            #region 分用户日志
            if (resMatrixLogList.Count > 0)
            {
                var groupResMatrixLogList = resMatrixLogList.GroupBy(p => p.UserCode);
                foreach (var groupResMatrixLog in groupResMatrixLogList)
                {
                    listBody = new StringBuilder();
                    GetListBody(groupResMatrixLog.ToList(), listBody);
                    sendUserMail(startDate, endDate, listBody, groupResMatrixLog.Key);
                }
            }
            #endregion
        }

        private void GetListBody(IList<ResMatrixLog> resMatrixLogList, StringBuilder listBody)
        {
            var updateResMatrixLogList = resMatrixLogList.Where(p => p.Action == "Update").ToList();
            if (updateResMatrixLogList.Count() > 0)
            {
                listBody.Append("<br /><span style='font-size:14px;'>变更的职责</span><br />");
                GetTable(updateResMatrixLogList, listBody);
            }

            var newResMatrixLogList = resMatrixLogList.Where(p => p.Action == "New").ToList();
            if (newResMatrixLogList.Count() > 0)
            {
                listBody.Append("<br /><span style='font-size:14px;'>新增的职责</span><br />");
                GetTable(newResMatrixLogList, listBody);
            }

            var deleteResMatrixLogList = resMatrixLogList.Where(p => p.Action == "New").ToList();
            if (deleteResMatrixLogList.Count() > 0)
            {
                listBody.Append("<br /><span style='font-size:14px;'>删除的职责</span><br />");
                GetTable(deleteResMatrixLogList, listBody);
            }
        }

        private void sendUserMail(DateTime startDate, DateTime endDate, StringBuilder listBody, string userCode)
        {
            var user = this.userMgr.LoadUser(userCode);

            if (user.IsActive && !string.IsNullOrEmpty(user.Email))
            {
                string subject = "责任方阵职责变化日志";
                StringBuilder mailBody = new StringBuilder();
                string companyName = entityPreferenceMgr.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).Value;
                ISIUtil.AppendTestText(smtpMgr.IsTestSystem(), mailBody, ISIConstants.EMAIL_SEPRATOR);

                string gender = "先生/女士";
                if (user.Gender == "M")
                {
                    gender = "先生";
                }
                else if (user.Gender == "M")
                {
                    gender = "女士";
                }

                mailBody.Append(string.Format("<span style='font-size:13px;'>尊敬的{0}{1}:</span><br /><br />", user.Name, gender));
                mailBody.Append("&nbsp;&nbsp;<span style='font-size:13px;'>您的 " + subject + "。</span><br />");
                mailBody.Append("&nbsp;&nbsp;<span style='font-size:13px;'>操作的时间范围: " + startDate.ToString("yyyy-MM-dd") + " 至 " + endDate.ToString("yyyy-MM-dd") + "</span><br /><br /><br />");

                string webAddress = entityPreferenceMgr.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_WEBADDRESS).Value;

                mailBody.Append("<span style='font-size:14px;'>详细情况</span><br />");

                mailBody.Append(listBody.ToString());

                mailBody.Append("<br /><br /><br /><br />");
                mailBody.Append("请知悉!<br /><br />");
                mailBody.Append("<span style='font-size:13px;'>重要:本邮件只是提示,具体内容以 <a href='http://" + webAddress + "'>ISI系统</a> 为准.</span><br />");
                mailBody.Append("<span style='font-size:13px;'>谢谢合作!</span><br /><br />");
                mailBody.Append("<span style='font-size:13px;'>" + companyName + "</span><br/>");
                mailBody.Append("<span style='font-size:13px;'><a href='http://" + webAddress + "'>http://" + webAddress + "</a></span>");
                //todo
                this.smtpMgr.AsyncSend(subject, mailBody.ToString(), user.Email, string.Empty, MailPriority.Normal);
            }
        }

        private void GetTable(IList<ResMatrixLog> resMatrixLogList, StringBuilder listBody)
        {
            listBody.Append("<table cellspacing='0' cellpadding='4' rules='all' border='1' style='width:100%;border-collapse:collapse;font-size:12px;'>");
            listBody.Append("<tr nowrap style='color:#FFFFFF;background-color:#000060;font-weight:bold;line-height:150%;'>");
            listBody.Append("<th nowrap scope='col'>作业区</th>");
            listBody.Append("<th nowrap scope='col'>工位</th>");
            listBody.Append("<th nowrap scope='col'>岗位</th>");
            listBody.Append("<th nowrap scope='col'>优先级</th>");
            listBody.Append("<th nowrap scope='col'>人员</th>");
            listBody.Append("<th nowrap scope='col'>开始时间</th>");
            listBody.Append("<th nowrap scope='col'>结束时间</th>");
            listBody.Append("<th nowrap scope='col'>职责</th>");
            listBody.Append("<th nowrap scope='col'>日志</th>");
            listBody.Append("</tr>");
            foreach (var resMatrixLog in resMatrixLogList)
            {
                listBody.Append("<tr><td>");
                listBody.Append(resMatrixLog.WorkShopCodeName);
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.OperateCodeName);
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.RoleCodeName);
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.Priority);
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.UserCodeName);
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.StartDate.ToString("yyyy-MM-dd HH:mm"));
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.EndDate.ToString("yyyy-MM-dd HH:mm"));
                listBody.Append("</td><td style='word-break:break-all;word-wrap:break-word;white-space:pre-wrap;'>");

                listBody.Append(DiffMatchPatchHelper.DiffPrettyHtml(resMatrixLog.OldResponsibility, resMatrixLog.Responsibility));
                listBody.Append("</td><td style='word-break:break-all;word-wrap:break-word;white-space:pre-wrap;'>");
                listBody.Append(resMatrixLog.Logs);
                listBody.Append("</td></tr>");
            }
            listBody.Append("</table>");
        }

        #endregion Customized Methods

        class PatrolUser
        {
            public string UserCode { get; set; }
            public DateTime WindowTime { get; set; }
            //public DateTime NextWindowTime { get; set; }
            public ResRole ResRole { get; set; }
            public ResMatrix ResMatrix { get; set; }
            //public ResPatrol ResPatrol { get; set; }
            public List<ResSop> ResSopList { get; set; }
        }

        class WorkShopUser
        {
            public string WorkShop { get; set; }
            public string UserCode { get; set; }
            public string UserName { get; set; }
        }
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class ResMatrixMgrE : com.Sconit.ISI.Service.Impl.ResMatrixMgr, IResMatrixMgrE
    {
    }
}

#endregion Extend Class