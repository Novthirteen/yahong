using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Facilities.NHibernateIntegration;
using NHibernate;
using NHibernate.Type;
using com.Sconit.ISI.Entity;
using com.Sconit.Persistence;

//TODO: Add other using statmens here.

namespace com.Sconit.ISI.Persistence.NH
{
    public class NHTaskStatusViewBaseDao : NHDaoBase, ITaskStatusViewBaseDao
    {
        public NHTaskStatusViewBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTaskStatusView(TaskStatusView entity)
        {
            Create(entity);
        }

        public virtual IList<TaskStatusView> GetAllTaskStatusView()
        {
            return FindAll<TaskStatusView>();
        }

        public virtual TaskStatusView LoadTaskStatusView(String taskCode)
        {
            return FindById<TaskStatusView>(taskCode);
        }

        public virtual void UpdateTaskStatusView(TaskStatusView entity)
        {
            Update(entity);
        }

        public virtual void DeleteTaskStatusView(String taskCode)
        {
            string hql = @"from TaskStatusView entity where entity.TaskCode = ?";
            Delete(hql, new object[] { taskCode }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteTaskStatusView(TaskStatusView entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTaskStatusView(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from TaskStatusView entity where entity.TaskCode in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteTaskStatusView(IList<TaskStatusView> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (TaskStatusView entity in entityList)
            {
                pkList.Add(entity.TaskCode);
            }

            DeleteTaskStatusView(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
