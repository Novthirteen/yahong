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
    public class NHProcessInstanceBaseDao : NHDaoBase, IProcessInstanceBaseDao
    {
        public NHProcessInstanceBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateProcessInstance(ProcessInstance entity)
        {
            Create(entity);
        }

        public virtual IList<ProcessInstance> GetAllProcessInstance()
        {
            return FindAll<ProcessInstance>();
        }

        public virtual ProcessInstance LoadProcessInstance(Int32 id)
        {
            return FindById<ProcessInstance>(id);
        }

        public virtual void UpdateProcessInstance(ProcessInstance entity)
        {
            Update(entity);
        }

        public virtual void DeleteProcessInstance(Int32 id)
        {
            string hql = @"from ProcessInstance entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteProcessInstance(ProcessInstance entity)
        {
            Delete(entity);
        }

        public virtual void DeleteProcessInstance(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from ProcessInstance entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteProcessInstance(IList<ProcessInstance> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (ProcessInstance entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteProcessInstance(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
