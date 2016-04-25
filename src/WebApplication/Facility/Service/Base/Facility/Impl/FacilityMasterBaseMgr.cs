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
    public class FacilityMasterBaseMgr : SessionBase, IFacilityMasterBaseMgr
    {
        public IFacilityMasterDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityMaster(FacilityMaster entity)
        {
            entityDao.CreateFacilityMaster(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityMaster LoadFacilityMaster(String fCID)
        {
            return entityDao.LoadFacilityMaster(fCID);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityMaster> GetAllFacilityMaster()
        {
            return entityDao.GetAllFacilityMaster();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityMaster(FacilityMaster entity)
        {
            entityDao.UpdateFacilityMaster(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityMaster(String fCID)
        {
            entityDao.DeleteFacilityMaster(fCID);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityMaster(FacilityMaster entity)
        {
            entityDao.DeleteFacilityMaster(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityMaster(IList<String> pkList)
        {
            entityDao.DeleteFacilityMaster(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityMaster(IList<FacilityMaster> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }

            entityDao.DeleteFacilityMaster(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
