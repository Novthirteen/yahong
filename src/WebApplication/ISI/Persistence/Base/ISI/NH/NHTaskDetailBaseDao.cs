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
    public class NHTaskDetailBaseDao : NHDaoBase, ITaskDetailBaseDao
    {
        public NHTaskDetailBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTaskDetail(TaskDetail entity)
        {
            Create(entity);
        }

        public virtual IList<TaskDetail> GetAllTaskDetail()
        {
            return GetAllTaskDetail(false);
        }

        public virtual IList<TaskDetail> GetAllTaskDetail(bool includeInactive)
        {
            string hql = @"from TaskDetail entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<TaskDetail> result = FindAllWithCustomQuery<TaskDetail>(hql);
            return result;
        }

        public virtual TaskDetail LoadTaskDetail(Int32 id)
        {
            return FindById<TaskDetail>(id);
        }

        public virtual void UpdateTaskDetail(TaskDetail entity)
        {
            Update(entity);
        }

        public virtual void DeleteTaskDetail(Int32 id)
        {
            string hql = @"from TaskDetail entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteTaskDetail(TaskDetail entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTaskDetail(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from TaskDetail entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteTaskDetail(IList<TaskDetail> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (TaskDetail entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteTaskDetail(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
