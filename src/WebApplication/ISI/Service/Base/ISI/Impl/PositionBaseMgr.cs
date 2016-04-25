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
    public class PositionBaseMgr : SessionBase, IPositionBaseMgr
    {
        public IPositionDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreatePosition(Position entity)
        {
            entityDao.CreatePosition(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual Position LoadPosition(String position)
        {
            return entityDao.LoadPosition(position);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Position> GetAllPosition()
        {
            return entityDao.GetAllPosition();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdatePosition(Position entity)
        {
            entityDao.UpdatePosition(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeletePosition(String position)
        {
            entityDao.DeletePosition(position);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeletePosition(Position entity)
        {
            entityDao.DeletePosition(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeletePosition(IList<String> pkList)
        {
            entityDao.DeletePosition(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeletePosition(IList<Position> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeletePosition(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
