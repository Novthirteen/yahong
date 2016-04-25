using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class EvaluationBaseMgr : SessionBase, IEvaluationBaseMgr
    {
        public IEvaluationDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateEvaluation(Evaluation entity)
        {
            entityDao.CreateEvaluation(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual Evaluation LoadEvaluation(String userCode)
        {
            return entityDao.LoadEvaluation(userCode);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Evaluation> GetAllEvaluation()
        {
            return entityDao.GetAllEvaluation(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Evaluation> GetAllEvaluation(bool includeInactive)
        {
            return entityDao.GetAllEvaluation(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateEvaluation(Evaluation entity)
        {
            entityDao.UpdateEvaluation(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteEvaluation(String userCode)
        {
            entityDao.DeleteEvaluation(userCode);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteEvaluation(Evaluation entity)
        {
            entityDao.DeleteEvaluation(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteEvaluation(IList<String> pkList)
        {
            entityDao.DeleteEvaluation(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteEvaluation(IList<Evaluation> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteEvaluation(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
