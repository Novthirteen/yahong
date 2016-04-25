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
    public class NHTaskAddressBaseDao : NHDaoBase, ITaskAddressBaseDao
    {
        public NHTaskAddressBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTaskAddress(TaskAddress entity)
        {
            Create(entity);
        }

        public virtual IList<TaskAddress> GetAllTaskAddress()
        {
            return FindAll<TaskAddress>();
        }

        public virtual TaskAddress LoadTaskAddress(String code)
        {
            return FindById<TaskAddress>(code);
        }

        public virtual void UpdateTaskAddress(TaskAddress entity)
        {
            Update(entity);
        }

        public virtual void DeleteTaskAddress(String code)
        {
            string hql = @"from TaskAddress entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteTaskAddress(TaskAddress entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTaskAddress(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from TaskAddress entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteTaskAddress(IList<TaskAddress> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (TaskAddress entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteTaskAddress(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
