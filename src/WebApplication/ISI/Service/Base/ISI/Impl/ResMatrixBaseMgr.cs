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
    public class ResMatrixBaseMgr : SessionBase, IResMatrixBaseMgr
    {
        public IResMatrixDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateResMatrix(ResMatrix entity)
        {
            entityDao.CreateResMatrix(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ResMatrix LoadResMatrix(Int32 id)
        {
            return entityDao.LoadResMatrix(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ResMatrix> GetAllResMatrix()
        {
            return entityDao.GetAllResMatrix();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateResMatrix(ResMatrix entity)
        {
            entityDao.UpdateResMatrix(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResMatrix(Int32 id)
        {
            entityDao.DeleteResMatrix(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResMatrix(ResMatrix entity)
        {
            entityDao.DeleteResMatrix(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResMatrix(IList<Int32> pkList)
        {
            entityDao.DeleteResMatrix(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResMatrix(IList<ResMatrix> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteResMatrix(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
