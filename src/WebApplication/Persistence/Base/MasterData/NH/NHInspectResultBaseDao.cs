using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Facilities.NHibernateIntegration;
using NHibernate;
using NHibernate.Type;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statmens here.

namespace com.Sconit.Persistence.MasterData.NH
{
    public class NHInspectResultBaseDao : NHDaoBase, IInspectResultBaseDao
    {
        public NHInspectResultBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateInspectResult(InspectResult entity)
        {
            Create(entity);
        }

        public virtual IList<InspectResult> GetAllInspectResult()
        {
            return FindAll<InspectResult>();
        }

        public virtual InspectResult LoadInspectResult(Int32 id)
        {
            return FindById<InspectResult>(id);
        }

        public virtual void UpdateInspectResult(InspectResult entity)
        {
            Update(entity);
        }

        public virtual void DeleteInspectResult(Int32 id)
        {
            string hql = @"from InspectResult entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteInspectResult(InspectResult entity)
        {
            Delete(entity);
        }

        public virtual void DeleteInspectResult(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from InspectResult entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteInspectResult(IList<InspectResult> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (InspectResult entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteInspectResult(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
