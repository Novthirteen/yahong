using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Facility.Entity;
using com.Sconit.Facility.Persistence;
using com.Sconit.Service;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityStockMasterBaseMgr : SessionBase, IFacilityStockMasterBaseMgr
    {
        public IFacilityStockMasterDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityStockMaster(FacilityStockMaster entity)
        {
            entityDao.CreateFacilityStockMaster(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityStockMaster LoadFacilityStockMaster(String orderNo)
        {
            return entityDao.LoadFacilityStockMaster(orderNo);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityStockMaster> GetAllFacilityStockMaster()
        {
            return entityDao.GetAllFacilityStockMaster();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityStockMaster(FacilityStockMaster entity)
        {
            entityDao.UpdateFacilityStockMaster(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStockMaster(String orderNo)
        {
            entityDao.DeleteFacilityStockMaster(orderNo);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStockMaster(FacilityStockMaster entity)
        {
            entityDao.DeleteFacilityStockMaster(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStockMaster(IList<String> pkList)
        {
            entityDao.DeleteFacilityStockMaster(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStockMaster(IList<FacilityStockMaster> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFacilityStockMaster(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
