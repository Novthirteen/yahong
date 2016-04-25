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
    public class NHWorkDetBaseDao : NHDaoBase, IWorkDetBaseDao
    {
        public NHWorkDetBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateWorkDet(WorkDet entity)
        {
            Create(entity);
        }

        public virtual IList<WorkDet> GetAllWorkDet()
        {
            return FindAll<WorkDet>();
        }

        public virtual WorkDet LoadWorkDet(Int32 iD)
        {
            return FindById<WorkDet>(iD);
        }

        public virtual void UpdateWorkDet(WorkDet entity)
        {
            Update(entity);
        }

        public virtual void DeleteWorkDet(Int32 iD)
        {
            string hql = @"from WorkDet entity where entity.Id = ?";
            Delete(hql, new object[] { iD }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteWorkDet(WorkDet entity)
        {
            Delete(entity);
        }

        public virtual void DeleteWorkDet(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from WorkDet entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteWorkDet(IList<WorkDet> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (WorkDet entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteWorkDet(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
