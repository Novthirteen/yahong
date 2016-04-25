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
    public class NHResMatrixBaseDao : NHDaoBase, IResMatrixBaseDao
    {
        public NHResMatrixBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateResMatrix(ResMatrix entity)
        {
            Create(entity);
        }

        public virtual IList<ResMatrix> GetAllResMatrix()
        {
            return FindAll<ResMatrix>();
        }

        public virtual ResMatrix LoadResMatrix(Int32 id)
        {
            return FindById<ResMatrix>(id);
        }

        public virtual void UpdateResMatrix(ResMatrix entity)
        {
            Update(entity);
        }

        public virtual void DeleteResMatrix(Int32 id)
        {
            string hql = @"from ResMatrix entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteResMatrix(ResMatrix entity)
        {
            Delete(entity);
        }

        public virtual void DeleteResMatrix(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from ResMatrix entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteResMatrix(IList<ResMatrix> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (ResMatrix entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteResMatrix(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
