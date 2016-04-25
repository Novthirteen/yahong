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
    public class FacilityStockDetailBaseMgr : SessionBase, IFacilityStockDetailBaseMgr
    {
        public IFacilityStockDetailDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityStockDetail(FacilityStockDetail entity)
        {
            entityDao.CreateFacilityStockDetail(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityStockDetail LoadFacilityStockDetail(Int32 id)
        {
            return entityDao.LoadFacilityStockDetail(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityStockDetail> GetAllFacilityStockDetail()
        {
            return entityDao.GetAllFacilityStockDetail();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityStockDetail(FacilityStockDetail entity)
        {
            entityDao.UpdateFacilityStockDetail(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStockDetail(Int32 id)
        {
            entityDao.DeleteFacilityStockDetail(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStockDetail(FacilityStockDetail entity)
        {
            entityDao.DeleteFacilityStockDetail(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStockDetail(IList<Int32> pkList)
        {
            entityDao.DeleteFacilityStockDetail(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStockDetail(IList<FacilityStockDetail> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFacilityStockDetail(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
