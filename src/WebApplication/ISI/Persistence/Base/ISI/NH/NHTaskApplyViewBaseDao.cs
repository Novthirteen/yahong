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
    public class NHTaskApplyViewBaseDao : NHDaoBase, ITaskApplyViewBaseDao
    {
        public NHTaskApplyViewBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTaskApplyView(TaskApplyView entity)
        {
            Create(entity);
        }

        public virtual IList<TaskApplyView> GetAllTaskApplyView()
        {
            return FindAll<TaskApplyView>();
        }

        public virtual TaskApplyView LoadTaskApplyView(Int32 id)
        {
            return FindById<TaskApplyView>(id);
        }

        public virtual void UpdateTaskApplyView(TaskApplyView entity)
        {
            Update(entity);
        }

        public virtual void DeleteTaskApplyView(Int32 id)
        {
            string hql = @"from TaskApplyView entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteTaskApplyView(TaskApplyView entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTaskApplyView(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from TaskApplyView entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteTaskApplyView(IList<TaskApplyView> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (TaskApplyView entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteTaskApplyView(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
