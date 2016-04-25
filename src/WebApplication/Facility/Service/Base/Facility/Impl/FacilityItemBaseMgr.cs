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
    public class FacilityItemBaseMgr : SessionBase, IFacilityItemBaseMgr
    {
        public IFacilityItemDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityItem(FacilityItem entity)
        {
            entityDao.CreateFacilityItem(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityItem LoadFacilityItem(Int32 id)
        {
            return entityDao.LoadFacilityItem(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityItem> GetAllFacilityItem()
        {
            return entityDao.GetAllFacilityItem(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityItem> GetAllFacilityItem(bool includeInactive)
        {
            return entityDao.GetAllFacilityItem(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityItem(FacilityItem entity)
        {
            entityDao.UpdateFacilityItem(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityItem(Int32 id)
        {
            entityDao.DeleteFacilityItem(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityItem(FacilityItem entity)
        {
            entityDao.DeleteFacilityItem(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityItem(IList<Int32> pkList)
        {
            entityDao.DeleteFacilityItem(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityItem(IList<FacilityItem> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFacilityItem(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
