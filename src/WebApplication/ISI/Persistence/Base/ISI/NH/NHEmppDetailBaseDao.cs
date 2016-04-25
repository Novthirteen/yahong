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
    public class NHEmppDetailBaseDao : NHDaoBase, IEmppDetailBaseDao
    {
        public NHEmppDetailBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateEmppDetail(EmppDetail entity)
        {
            Create(entity);
        }

        public virtual IList<EmppDetail> GetAllEmppDetail()
        {
            return FindAll<EmppDetail>();
        }

        public virtual EmppDetail LoadEmppDetail(Int32 id)
        {
            return FindById<EmppDetail>(id);
        }

        public virtual void UpdateEmppDetail(EmppDetail entity)
        {
            Update(entity);
        }

        public virtual void DeleteEmppDetail(Int32 id)
        {
            string hql = @"from EmppDetail entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteEmppDetail(EmppDetail entity)
        {
            Delete(entity);
        }

        public virtual void DeleteEmppDetail(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from EmppDetail entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteEmppDetail(IList<EmppDetail> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (EmppDetail entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteEmppDetail(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
