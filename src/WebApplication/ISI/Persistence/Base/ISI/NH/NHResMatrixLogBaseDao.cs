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
    public class NHResMatrixLogBaseDao : NHDaoBase, IResMatrixLogBaseDao
    {
        public NHResMatrixLogBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateResMatrixLog(ResMatrixLog entity)
        {
            Create(entity);
        }

        public virtual IList<ResMatrixLog> GetAllResMatrixLog()
        {
            return FindAll<ResMatrixLog>();
        }

        public virtual ResMatrixLog LoadResMatrixLog(Int32 id)
        {
            return FindById<ResMatrixLog>(id);
        }

        public virtual void UpdateResMatrixLog(ResMatrixLog entity)
        {
            Update(entity);
        }

        public virtual void DeleteResMatrixLog(Int32 id)
        {
            string hql = @"from ResMatrixLog entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteResMatrixLog(ResMatrixLog entity)
        {
            Delete(entity);
        }

        public virtual void DeleteResMatrixLog(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from ResMatrixLog entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteResMatrixLog(IList<ResMatrixLog> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (ResMatrixLog entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteResMatrixLog(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
