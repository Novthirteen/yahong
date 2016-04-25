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
    public class NHTraceViewBaseDao : NHDaoBase, ITraceViewBaseDao
    {
        public NHTraceViewBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTraceView(TraceView entity)
        {
            Create(entity);
        }

        public virtual IList<TraceView> GetAllTraceView()
        {
            return FindAll<TraceView>();
        }

        public virtual TraceView LoadTraceView(Int32 id, String type)
        {
            string hql = @"from TraceView entity where entity.Id = ? and entity.Type = ?";
            IList<TraceView> result = FindAllWithCustomQuery<TraceView>(hql, new object[] { id, type }, new IType[] { NHibernateUtil.Int32, NHibernateUtil.String });
            if (result != null && result.Count > 0)
            {
                return result[0];
            }
            else
            {
                return null;
            }
        }

        public virtual void UpdateTraceView(TraceView entity)
        {
            Update(entity);
        }

        public virtual void DeleteTraceView(Int32 id, String type)
        {
            string hql = @"from TraceView entity where entity.Id = ? and entity.Type = ?";
            Delete(hql, new object[] { id, type }, new IType[] { NHibernateUtil.Int32, NHibernateUtil.String });
        }


        public virtual void DeleteTraceView(TraceView entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTraceView(IList<TraceView> entityList)
        {
            foreach (TraceView entity in entityList)
            {
                DeleteTraceView(entity);
            }
        }


        #endregion Method Created By CodeSmith
    }
}
