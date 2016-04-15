using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity.MasterData;
using com.Sconit.Persistence.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class InspectComfirmResultBaseMgr : SessionBase, IInspectComfirmResultBaseMgr
    {
        public IInspectComfirmResultDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateInspectComfirmResult(InspectComfirmResult entity)
        {
            entityDao.CreateInspectComfirmResult(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual InspectComfirmResult LoadInspectComfirmResult(Int32 id)
        {
            return entityDao.LoadInspectComfirmResult(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<InspectComfirmResult> GetAllInspectComfirmResult()
        {
            return entityDao.GetAllInspectComfirmResult();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateInspectComfirmResult(InspectComfirmResult entity)
        {
            entityDao.UpdateInspectComfirmResult(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteInspectComfirmResult(Int32 id)
        {
            entityDao.DeleteInspectComfirmResult(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteInspectComfirmResult(InspectComfirmResult entity)
        {
            entityDao.DeleteInspectComfirmResult(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteInspectComfirmResult(IList<Int32> pkList)
        {
            entityDao.DeleteInspectComfirmResult(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteInspectComfirmResult(IList<InspectComfirmResult> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteInspectComfirmResult(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
