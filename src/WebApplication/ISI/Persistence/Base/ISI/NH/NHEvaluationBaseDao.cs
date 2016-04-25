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
    public class NHEvaluationBaseDao : NHDaoBase, IEvaluationBaseDao
    {
        public NHEvaluationBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateEvaluation(Evaluation entity)
        {
            Create(entity);
        }

        public virtual IList<Evaluation> GetAllEvaluation()
        {
            return GetAllEvaluation(false);
        }

        public virtual IList<Evaluation> GetAllEvaluation(bool includeInactive)
        {
            string hql = @"from Evaluation entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<Evaluation> result = FindAllWithCustomQuery<Evaluation>(hql);
            return result;
        }

        public virtual Evaluation LoadEvaluation(String userCode)
        {
            return FindById<Evaluation>(userCode);
        }

        public virtual void UpdateEvaluation(Evaluation entity)
        {
            Update(entity);
        }

        public virtual void DeleteEvaluation(String userCode)
        {
            string hql = @"from Evaluation entity where entity.UserCode = ?";
            Delete(hql, new object[] { userCode }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteEvaluation(Evaluation entity)
        {
            Delete(entity);
        }

        public virtual void DeleteEvaluation(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from Evaluation entity where entity.UserCode in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteEvaluation(IList<Evaluation> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (Evaluation entity in entityList)
            {
                pkList.Add(entity.UserCode);
            }

            DeleteEvaluation(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
