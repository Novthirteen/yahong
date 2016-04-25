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
    public class NHTaskStatusBaseDao : NHDaoBase, ITaskStatusBaseDao
    {
        public NHTaskStatusBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTaskStatus(TaskStatus entity)
        {
            Create(entity);
        }

        public virtual IList<TaskStatus> GetAllTaskStatus()
        {
            return FindAll<TaskStatus>();
        }

        public virtual TaskStatus LoadTaskStatus(Int32 id)
        {
            return FindById<TaskStatus>(id);
        }

        public virtual void UpdateTaskStatus(TaskStatus entity)
        {
            Update(entity);
        }

        public virtual void DeleteTaskStatus(Int32 id)
        {
            string hql = @"from TaskStatus entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteTaskStatus(TaskStatus entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTaskStatus(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from TaskStatus entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteTaskStatus(IList<TaskStatus> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (TaskStatus entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteTaskStatus(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
