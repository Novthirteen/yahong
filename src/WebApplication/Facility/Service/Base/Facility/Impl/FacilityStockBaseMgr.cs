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
    public class FacilityStockBaseMgr : SessionBase, IFacilityStockBaseMgr
    {
        public IFacilityStockDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityStock(FacilityStock entity)
        {
            entityDao.CreateFacilityStock(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityStock LoadFacilityStock(Int32 id)
        {
            return entityDao.LoadFacilityStock(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityStock> GetAllFacilityStock()
        {
            return entityDao.GetAllFacilityStock();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityStock(FacilityStock entity)
        {
            entityDao.UpdateFacilityStock(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStock(Int32 id)
        {
            entityDao.DeleteFacilityStock(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStock(FacilityStock entity)
        {
            entityDao.DeleteFacilityStock(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStock(IList<Int32> pkList)
        {
            entityDao.DeleteFacilityStock(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityStock(IList<FacilityStock> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFacilityStock(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
