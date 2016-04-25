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
    public class NHProcessDefinitionBaseDao : NHDaoBase, IProcessDefinitionBaseDao
    {
        public NHProcessDefinitionBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateProcessDefinition(ProcessDefinition entity)
        {
            Create(entity);
        }

        public virtual IList<ProcessDefinition> GetAllProcessDefinition()
        {
            return FindAll<ProcessDefinition>();
        }

        public virtual ProcessDefinition LoadProcessDefinition(Int32 id)
        {
            return FindById<ProcessDefinition>(id);
        }

        public virtual void UpdateProcessDefinition(ProcessDefinition entity)
        {
            Update(entity);
        }

        public virtual void DeleteProcessDefinition(Int32 id)
        {
            string hql = @"from ProcessDefinition entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteProcessDefinition(ProcessDefinition entity)
        {
            Delete(entity);
        }

        public virtual void DeleteProcessDefinition(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from ProcessDefinition entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteProcessDefinition(IList<ProcessDefinition> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (ProcessDefinition entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteProcessDefinition(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
