using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.ISI.Entity.Util;
using System.Linq;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class TaskSubTypeMgr : TaskSubTypeBaseMgr, ITaskSubTypeMgr
    {
        private static IList<TaskSubType> cachedAllTaskSubType;
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IPermissionMgrE permissionMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        #region Customized Methods

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetCacheAllTaskSubType()
        {
            if (cachedAllTaskSubType == null)
            {
                cachedAllTaskSubType = this.GetAllTaskSubType();
            }
            else
            {
                //检查TaskSubType大小是否发生变化
                DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskSubType));
                criteria.Add(Expression.Eq("IsActive", true));
                criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Code")));
                IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);

                if (count[0] != cachedAllTaskSubType.Count)
                {
                    cachedAllTaskSubType = GetAllTaskSubType();
                }
            }

            if (cachedAllTaskSubType != null && cachedAllTaskSubType.Count > 0)
            {
                foreach (TaskSubType taskSubType in cachedAllTaskSubType)
                {
                    taskSubType.Name = GetName(taskSubType);
                }
            }

            return cachedAllTaskSubType;
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteTaskSubType(String code)
        {
            permissionMgrE.DeletePermission(code);
            base.DeleteTaskSubType(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public override IList<TaskSubType> GetAllTaskSubType()
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskSubType));
            criteria.Add(Expression.Eq("IsActive", true));
            criteria.AddOrder(Order.Desc("Type"));
            criteria.AddOrder(Order.Asc("Seq"));
            criteria.AddOrder(Order.Asc("Code"));
            return this.criteriaMgrE.FindAll<TaskSubType>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetCostCenter()
        {
            return GetCostCenter(null);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetCostCenter(bool? isActive)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskSubType));
            if (isActive.HasValue)
            {
                criteria.Add(Expression.Eq("IsActive", isActive.Value));
            }
            criteria.Add(Expression.Eq("IsCost", true));
            criteria.AddOrder(Order.Desc("IsActive"));
            criteria.AddOrder(Order.Asc("Type"));
            criteria.AddOrder(Order.Asc("Seq"));
            criteria.AddOrder(Order.Asc("Code"));
            return this.criteriaMgrE.FindAll<TaskSubType>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetTaskSubType(string userCode)
        {
            return GetTaskSubType(null, userCode, false);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetTaskSubType(string userCode, bool includePrivacy)
        {
            return GetTaskSubType(null, userCode, false, includePrivacy);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetPublicTaskSubType(string type, string userCode)
        {
            return GetTaskSubType(type, userCode, false, true);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetTaskSubType(string type, string userCode, bool onlyPublic)
        {
            return GetTaskSubType(type, userCode, false, onlyPublic);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetTaskSubType(string type, string userCode)
        {
            return GetTaskSubType(type, userCode, false, false);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetTaskSubTypeList(string type)
        {
            return this.GetTaskSubType(type, true, string.Empty, false, true, false, false);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetTaskSubType(string type, string userCode, bool includeInactive, bool onlyPublic)
        {
            return this.GetTaskSubType(type, false, userCode, includeInactive, true, onlyPublic, false);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetProjectTaskSubType()
        {
            DetachedCriteria criteria = DetachedCriteria.For<TaskSubType>();
            criteria.Add(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT));
            criteria.AddOrder(Order.Desc("IsActive"));
            criteria.AddOrder(Order.Asc("Seq"));
            criteria.AddOrder(Order.Asc("Code"));
            IList<TaskSubType> taskSubTypeList = criteriaMgrE.FindAll<TaskSubType>(criteria);
            return taskSubTypeList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetProjectTaskSubType(string type, string userCode, bool isProjectImportCheck)
        {
            return this.GetTaskSubType(type, false, userCode, false, false, false, isProjectImportCheck);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetTaskSubType(string type, bool onlyType, string userCode, bool includeInactive, bool includePrivacy, bool onlyPublic, bool? isProjectImportCheck)
        {
            DetachedCriteria criteria = DetachedCriteria.For<TaskSubType>();
            if (!includeInactive)
            {
                criteria.Add(Expression.Eq("IsActive", true));
            }
            if (onlyPublic)
            {
                criteria.Add(Expression.Eq("IsPublic", onlyPublic));
            }
            else if (type == ISIConstants.ISI_TASK_TYPE_PROJECT || type == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
            {
                criteria.Add(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT));
                if (isProjectImportCheck.HasValue && isProjectImportCheck.Value)
                {
                    criteria.Add(Expression.Or(Expression.Eq("IsQuote", isProjectImportCheck.Value), Expression.Eq("IsInitiation", isProjectImportCheck.Value)));
                    //必须项目经理
                    if (!string.IsNullOrEmpty(userCode))
                    {
                        criteria.Add(Expression.And(Expression.IsNotNull("AssignUser"),
                                            Expression.Or(Expression.Like("AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + userCode + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                        Expression.Or(Expression.Like("AssignUser", ISIConstants.ISI_USER_SEPRATOR + userCode + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                    Expression.Or(Expression.Like("AssignUser", ISIConstants.ISI_USER_SEPRATOR + userCode + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                    Expression.Or(Expression.Like("AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + userCode + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                Expression.Eq("AssignUser", userCode)))))));
                    }
                }
            }
            else if (type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
            {
                criteria.Add(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT));
                if (isProjectImportCheck.HasValue && isProjectImportCheck.Value)
                {
                    criteria.Add(Expression.Eq("IsEC", isProjectImportCheck.Value));
                    //必须工程更改负责人经理
                    if (!string.IsNullOrEmpty(userCode))
                    {
                        criteria.Add(Expression.And(Expression.IsNotNull("ECUser"),
                                            Expression.Or(Expression.Like("ECUser", ISIConstants.ISI_LEVEL_SEPRATOR + userCode + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                        Expression.Or(Expression.Like("ECUser", ISIConstants.ISI_USER_SEPRATOR + userCode + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                    Expression.Or(Expression.Like("ECUser", ISIConstants.ISI_USER_SEPRATOR + userCode + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                    Expression.Or(Expression.Like("ECUser", ISIConstants.ISI_LEVEL_SEPRATOR + userCode + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                Expression.Eq("ECUser", userCode)))))));
                    }
                }
            }
            else if (type == ISIConstants.ISI_TASK_TYPE_WORKFLOW)
            {
                criteria.Add(Expression.Eq("Type", type));
            }
            else if (!string.IsNullOrEmpty(type))
            {
                if (!onlyType)
                {
                    criteria.Add(Expression.Or(Expression.Eq("Type", type), Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_GENERAL)));
                }
                else
                {
                    criteria.Add(Expression.Eq("Type", type));
                }
            }
            else
            {
                criteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT)));
            }

            if (!includePrivacy)
            {
                criteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)));
            }

            if (!string.IsNullOrEmpty(userCode))
            {
                /*
                User user = userMgrE.LoadUser(userCode, false, true);
                if (!user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
						&& !user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE)
						&& !user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)
                        && !user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                        && !user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
                {
                    DetachedCriteria[] pCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(userCode);

                    criteria.Add(
                        Expression.Or(
                            Subqueries.PropertyIn("Code", pCrieteria[0]),
                            Subqueries.PropertyIn("Code", pCrieteria[1])));
                }*/
            }

            criteria.AddOrder(Order.Desc("Type"));
            criteria.AddOrder(Order.Asc("Seq"));
            criteria.AddOrder(Order.Asc("Code"));
            IList<TaskSubType> taskSubTypeList = criteriaMgrE.FindAll<TaskSubType>(criteria);

            if (taskSubTypeList != null && taskSubTypeList.Count > 0)
            {
                foreach (TaskSubType taskSubType in taskSubTypeList)
                {
                    taskSubType.Name = GetName(taskSubType);
                }
            }
            return taskSubTypeList;
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetWFSTaskSubType()
        {
            return GetWFSTaskSubType(string.Empty, string.Empty, string.Empty);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetWFSTaskSubType(string type, string userCode)
        {
            return GetWFSTaskSubType(type, userCode, string.Empty);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetCostCenter2()
        {
            DetachedCriteria criteria = DetachedCriteria.For<TaskSubType>();
            criteria.Add(Expression.Or(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT), Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_GENERAL)));
            criteria.AddOrder(Order.Desc("IsActive"));
            criteria.AddOrder(Order.Asc("Type"));
            criteria.AddOrder(Order.Asc("Seq"));
            criteria.AddOrder(Order.Asc("Code"));
            IList<TaskSubType> taskSubTypeList = criteriaMgrE.FindAll<TaskSubType>(criteria);

            if (taskSubTypeList != null && taskSubTypeList.Count > 0)
            {
                foreach (TaskSubType taskSubType in taskSubTypeList)
                {
                    taskSubType.Name = GetName(taskSubType);
                }
            }
            return taskSubTypeList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetWFSTaskSubType(string type, string userCode, string taskSubTypeCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For<TaskSubType>();
            criteria.Add(Expression.Eq("IsWF", true));

            if (!string.IsNullOrEmpty(type))
            {
                criteria.Add(Expression.Eq("Type", type));
            }
            if (!string.IsNullOrEmpty(taskSubTypeCode))
            {
                criteria.Add(Expression.Not(Expression.Eq("Code", taskSubTypeCode)));
            }

            criteria.AddOrder(Order.Desc("Type"));
            criteria.AddOrder(Order.Asc("Seq"));
            criteria.AddOrder(Order.Asc("Code"));
            IList<TaskSubType> taskSubTypeList = criteriaMgrE.FindAll<TaskSubType>(criteria);

            if (taskSubTypeList != null && taskSubTypeList.Count > 0)
            {
                foreach (TaskSubType taskSubType in taskSubTypeList)
                {
                    taskSubType.Name = GetName(taskSubType);
                }
            }
            return taskSubTypeList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public string GetName(TaskSubType taskSubType)
        {
            string result = string.Empty;
            if (taskSubType.Parent == null)
            {
                return taskSubType.Desc;
            }
            else
            {
                result = GetName(taskSubType.Parent) + "|" + taskSubType.Desc;
            }

            return result;
        }

        [Transaction(TransactionMode.Unspecified)]
        public bool IsRef(string code)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskSubType)).SetProjection(Projections.CountDistinct("Code"));
            criteria.Add(Expression.Eq("Parent.Code", code));
            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }
            //排班表
            criteria = DetachedCriteria.For(typeof(UserSubscription)).SetProjection(Projections.CountDistinct("Id"));
            criteria.Add(Expression.Eq("TaskSubType", code));
            count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }

            //失效模式
            criteria = DetachedCriteria.For(typeof(FailureMode)).SetProjection(Projections.CountDistinct("Code"));
            criteria.Add(Expression.Eq("TaskSubType.Code", code));
            count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }

            //任务
            criteria = DetachedCriteria.For(typeof(TaskMstr)).SetProjection(Projections.CountDistinct("Code"));
            criteria.Add(Expression.Eq("TaskSubType.Code", code));
            count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }

            //查权限
            criteria = DetachedCriteria.For(typeof(UserPermission)).SetProjection(Projections.CountDistinct("Id"));
            criteria.CreateAlias("Permission", "pm");
            criteria.Add(Expression.Eq("pm.Code", code));
            count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }

            criteria = DetachedCriteria.For(typeof(RolePermission)).SetProjection(Projections.CountDistinct("Id"));
            criteria.CreateAlias("Permission", "pm");
            criteria.Add(Expression.Eq("pm.Code", code));
            count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }

            return false;
        }


        [Transaction(TransactionMode.Unspecified)]
        public bool ExistsCode(string code)
        {
            if (this.LoadTaskSubType(code) != null)
            {
                return true;
            }

            DetachedCriteria criteria = DetachedCriteria.For(typeof(Permission)).SetProjection(Projections.CountDistinct("Code"));
            criteria.Add(Expression.Eq("Code", code));
            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }

            return false;
        }

        [Transaction(TransactionMode.Unspecified)]
        public string GetCostCenter(string taskCode, string costCenter)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskSubType)).SetProjection(Projections.Distinct(Projections.ProjectionList()
                            .Add(Projections.Property("AssignUser").As("AssignUser"))));
            DetachedCriteria subCriteria = DetachedCriteria.For(typeof(Cost));
            subCriteria.Add(Expression.Eq("TaskCode", taskCode));
            subCriteria.Add(Expression.Not(Expression.Eq("TaskSubType", costCenter)));
            subCriteria.Add(Expression.Not(Expression.Eq("TaskSubType", string.Empty)));
            subCriteria.Add(Expression.IsNotNull("TaskSubType"));
            subCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("TaskSubType")));
            criteria.Add(Subqueries.PropertyIn("Code", subCriteria));
            var assignUserList = this.criteriaMgrE.FindAll<object>(criteria);
            string costCenterUser = string.Join(";", assignUserList.Select(t => t.ToString()).Distinct().ToArray<string>());
            return costCenterUser;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskSubType> GetTaskSubTypeNotCode(string code)
        {
            TaskSubType taskSubType = this.LoadTaskSubType(code);

            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskSubType));
            criteria.Add(Expression.Or(Expression.Eq("Type", taskSubType.Type), Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_GENERAL)));
            criteria.Add(Expression.Not(Expression.In("Code", (ICollection)GetChildrenCode(taskSubType))));
            criteria.AddOrder(Order.Desc("Type"));
            criteria.AddOrder(Order.Asc("Seq"));
            criteria.AddOrder(Order.Asc("Code"));
            return this.criteriaMgrE.FindAll<TaskSubType>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<string> GetChildrenCode(TaskSubType taskSubType)
        {
            IList<string> taskSubTypeList = new List<string>();

            IList<TaskSubType> children = GetChildren(taskSubType);
            if (children != null && children.Count > 0)
            {
                foreach (TaskSubType task in children)
                {
                    taskSubTypeList.Add(taskSubType.Code);
                }
            }

            return taskSubTypeList;
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<TaskSubType> GetChildren(TaskSubType taskSubType)
        {
            IList<TaskSubType> taskList = new List<TaskSubType>();
            taskList.Add(taskSubType);
            IList<TaskSubType> children = taskSubType.Children;
            if (children != null && children.Count > 0)
            {
                foreach (TaskSubType child in children)
                {
                    IList<TaskSubType> taskChildren = GetChildren(child);
                    foreach (TaskSubType task in taskChildren)
                    {
                        taskList.Add(task);
                    }
                }
            }
            return taskList;
        }

        [Transaction(TransactionMode.Requires)]
        public override void CreateTaskSubType(TaskSubType taskSubType)
        {
            try
            {
                Permission pm = new Permission();
                pm.Code = taskSubType.Code;
                pm.Description = this.GetPermissioinDescription(taskSubType);
                PermissionCategory pmCategory = this.criteriaMgrE.FindById<PermissionCategory>(ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
                pm.Category = pmCategory;
                permissionMgrE.CreatePermission(pm);

                base.CreateTaskSubType(taskSubType);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public override void UpdateTaskSubType(TaskSubType taskSubType)
        {
            try
            {
                Permission permission = permissionMgrE.GetPermission(taskSubType.Code);
                if (permission != null)
                {
                    permission.Description = this.GetPermissioinDescription(taskSubType);
                    permissionMgrE.UpdatePermission(permission);
                }
                else
                {
                    Permission pm = new Permission();
                    pm.Code = taskSubType.Code;
                    pm.Description = this.GetPermissioinDescription(taskSubType);
                    PermissionCategory pmCategory = this.criteriaMgrE.FindById<PermissionCategory>(ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
                    pm.Category = pmCategory;
                    permissionMgrE.CreatePermission(pm);
                }
                base.UpdateTaskSubType(taskSubType);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public IList<User> GetUser(string taskSubType)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append(@" select u from User u ");
            hql.Append(@" where u.IsActive =1 and ");
            hql.Append(@" ( exists ( ");
            hql.Append(@"           select up.id from UserPermission up join up.User upu join up.Permission pm join pm.Category pmc ");
            hql.Append(@"               where pmc.Type=:PMCType1 ");
            hql.Append(@"               and pmc.Code=:PMCCode1 ");
            hql.Append(@"               and pm.Code=:TaskSubType1 ");
            hql.Append(@"               and upu.Code=u.Code ");
            hql.Append(@"       ) ");
            hql.Append(@" or  ");
            hql.Append(@" exists ( ");
            hql.Append(@"           select ur.id from UserRole ur join ur.User uru join ur.Role r,RolePermission rp join rp.Role rpr join rp.Permission pm join pm.Category pmc ");
            hql.Append(@"               where pmc.Type=:PMCType2  ");
            hql.Append(@"               and pmc.Code=:PMCCode2 ");
            hql.Append(@"               and r.Code=rpr.Code ");
            hql.Append(@"               and pm.Code=:TaskSubType2 ");
            hql.Append(@"               and uru.Code=u.Code ");
            hql.Append(@"        ) ");
            hql.Append(@" ) ");
            hql.Append(@" order by u.Code ");
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PMCType1", ISIConstants.CODE_MASTER_PERMISSION_CATEGORY_TYPE_VALUE_ISI);
            param.Add("PMCCode1", ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
            param.Add("TaskSubType1", taskSubType);
            param.Add("PMCType2", ISIConstants.CODE_MASTER_PERMISSION_CATEGORY_TYPE_VALUE_ISI);
            param.Add("PMCCode2", ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
            param.Add("TaskSubType2", taskSubType);
            return hqlMgrE.FindAll<User>(hql.ToString(), param);
        }

        [Transaction(TransactionMode.Unspecified)]
        private string GetPermissioinDescription(TaskSubType taskSubType)
        {
            return taskSubType.Type + " -- " + this.GetName(taskSubType);
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class TaskSubTypeMgrE : com.Sconit.ISI.Service.Impl.TaskSubTypeMgr, ITaskSubTypeMgrE
    {
    }
}

#endregion Extend Class