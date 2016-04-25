using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using NHibernate.Expression;
using com.Sconit.Entity.Exception;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class ProjectTaskMgr : ProjectTaskBaseMgr, IProjectTaskMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public ITaskSubTypeMgrE taskSubTypeE { get; set; }
        #region Customized Methods
        [Transaction(TransactionMode.Requires)]
        public IList<ProjectTask> GetProjectTask(string taskSubTypeCode, string type)
        {
            TaskSubType taskSubType = taskSubTypeE.LoadTaskSubType(taskSubTypeCode);
            if (type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
            {
                if (taskSubType.IsEC)
                {
                    DetachedCriteria criteria = DetachedCriteria.For(typeof(ProjectTask));
                    criteria.Add(Expression.Eq("ProjectType", taskSubType.ECType));

                    criteria.Add(Expression.Eq("IsActive", true));
                    criteria.AddOrder(Order.Asc("Phase"));
                    criteria.AddOrder(Order.Asc("Seq"));
                    criteria.AddOrder(Order.Asc("CreateDate"));
                    IList<ProjectTask> projectTaskList = this.criteriaMgrE.FindAll<ProjectTask>(criteria);
                    return projectTaskList;
                }
            }
            else if (!string.IsNullOrEmpty(taskSubType.ProjectType))
            {
                if (taskSubType.IsQuote || taskSubType.IsInitiation)
                {
                    DetachedCriteria criteria = DetachedCriteria.For(typeof(ProjectTask));

                    criteria.Add(Expression.Eq("ProjectType", taskSubType.ProjectType));
                    if (taskSubType.IsQuote)
                    {
                        criteria.Add(Expression.Eq("ProjectSubType", ISIConstants.CODE_MASTER_ISI_PROJECTSUBTYPE_QUOTE));
                    }
                    else
                    {
                        criteria.Add(Expression.Eq("ProjectSubType", ISIConstants.CODE_MASTER_ISI_PROJECTSUBTYPE_INITIATION));
                    }

                    criteria.Add(Expression.Eq("IsActive", true));
                    criteria.AddOrder(Order.Asc("Phase"));
                    criteria.AddOrder(Order.Asc("Seq"));
                    criteria.AddOrder(Order.Asc("CreateDate"));
                    IList<ProjectTask> projectTaskList = this.criteriaMgrE.FindAll<ProjectTask>(criteria);
                    return projectTaskList;
                }
            }
            return null;
        }

        [Transaction(TransactionMode.Unspecified)]
        public ProjectTask CheckAndLoadProjectTask(int id)
        {
            ProjectTask task = this.LoadProjectTask(id);
            if (task != null)
            {
                return task;
            }
            else
            {
                throw new BusinessErrorException("ISI.Error.TaskNotExist");
            }
        }
        [Transaction(TransactionMode.Requires)]
        public void DeleteProjectTask(int id, User user)
        {
            //ProjectTask task = this.CheckAndLoadProjectTask(id);

            //暂不删除附件

            //删除ProjectTask
            this.DeleteProjectTask(id);

        }


        [Transaction(TransactionMode.Requires)]
        public void CreateProjectTask(ProjectTask task, User user)
        {
            DateTime dateTimeNow = DateTime.Now;
            #region 创建TaskMstr

            task.TaskType = ISIConstants.ISI_TASK_TYPE_PROJECT;
            task.CreateUser = user.Code;
            task.CreateUserNm = user.Name;
            task.CreateDate = dateTimeNow;
            task.LastModifyUser = user.Code;
            task.LastModifyUserNm = user.Name;
            task.LastModifyDate = dateTimeNow;
            this.CreateProjectTask(task);

            #endregion
        }


        [Transaction(TransactionMode.Requires)]
        public void UpdateProjectTask(ProjectTask task, User user)
        {
            ProjectTask oldTask = this.CheckAndLoadProjectTask(task.Id);

            DateTime now = DateTime.Now;
            oldTask.IsActive = task.IsActive;
            oldTask.Phase = task.Phase;
            oldTask.ProjectSubType = task.ProjectSubType;
            oldTask.ProjectType = task.ProjectType;
            oldTask.Subject = task.Subject;
            oldTask.Phase = task.Phase;
            oldTask.Seq = task.Seq;
            oldTask.Desc = task.Desc;
            oldTask.ExpectedResults = task.ExpectedResults;
            oldTask.LastModifyDate = now;
            oldTask.LastModifyUser = user.Code;
            oldTask.LastModifyUserNm = user.Name;
            this.UpdateProjectTask(oldTask);
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class ProjectTaskMgrE : com.Sconit.ISI.Service.Impl.ProjectTaskMgr, IProjectTaskMgrE
    {
    }
}

#endregion Extend Class