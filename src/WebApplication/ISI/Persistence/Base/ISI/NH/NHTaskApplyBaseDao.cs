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
    public class NHTaskApplyBaseDao : NHDaoBase, ITaskApplyBaseDao
    {
        public NHTaskApplyBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTaskApply(TaskApply entity)
        {
            Create(entity);
        }

        public virtual IList<TaskApply> GetAllTaskApply()
        {
            return FindAll<TaskApply>();
        }

        public virtual TaskApply LoadTaskApply(Int32 id)
        {
            return FindById<TaskApply>(id);
        }

        public virtual void UpdateTaskApply(TaskApply entity)
        {
            Update(entity);
        }

        public virtual void DeleteTaskApply(Int32 id)
        {
            string hql = @"from TaskApply entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteTaskApply(TaskApply entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTaskApply(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from TaskApply entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteTaskApply(IList<TaskApply> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (TaskApply entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteTaskApply(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
