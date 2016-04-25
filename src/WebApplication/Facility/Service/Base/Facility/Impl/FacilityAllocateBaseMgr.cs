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
    public class FacilityAllocateBaseMgr : SessionBase, IFacilityAllocateBaseMgr
    {
        public IFacilityAllocateDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityAllocate(FacilityAllocate entity)
        {
            entityDao.CreateFacilityAllocate(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityAllocate LoadFacilityAllocate(Int32 id)
        {
            return entityDao.LoadFacilityAllocate(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityAllocate> GetAllFacilityAllocate()
        {
            return entityDao.GetAllFacilityAllocate(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityAllocate> GetAllFacilityAllocate(bool includeInactive)
        {
            return entityDao.GetAllFacilityAllocate(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityAllocate(FacilityAllocate entity)
        {
            entityDao.UpdateFacilityAllocate(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityAllocate(Int32 id)
        {
            entityDao.DeleteFacilityAllocate(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityAllocate(FacilityAllocate entity)
        {
            entityDao.DeleteFacilityAllocate(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityAllocate(IList<Int32> pkList)
        {
            entityDao.DeleteFacilityAllocate(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityAllocate(IList<FacilityAllocate> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFacilityAllocate(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
