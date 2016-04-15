using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Facilities.NHibernateIntegration;
using NHibernate;
using NHibernate.Type;
using com.Sconit.Entity.MRP;

//TODO: Add other using statmens here.

namespace com.Sconit.Persistence.MRP.NH
{
    public class NHMrpPlanTransactionBaseDao : NHDaoBase, IMrpPlanTransactionBaseDao
    {
        public NHMrpPlanTransactionBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateMrpPlanTransaction(MrpPlanTransaction entity)
        {
            Create(entity);
        }

        public virtual IList<MrpPlanTransaction> GetAllMrpPlanTransaction()
        {
            return FindAll<MrpPlanTransaction>();
        }

        public virtual MrpPlanTransaction LoadMrpPlanTransaction(Int32 id)
        {
            return FindById<MrpPlanTransaction>(id);
        }

        public virtual void UpdateMrpPlanTransaction(MrpPlanTransaction entity)
        {
            Update(entity);
        }

        public virtual void DeleteMrpPlanTransaction(Int32 id)
        {
            string hql = @"from MrpPlanTransaction entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteMrpPlanTransaction(MrpPlanTransaction entity)
        {
            Delete(entity);
        }

        public virtual void DeleteMrpPlanTransaction(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from MrpPlanTransaction entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteMrpPlanTransaction(IList<MrpPlanTransaction> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (MrpPlanTransaction entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteMrpPlanTransaction(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
