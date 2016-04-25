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
    public class FacilityCategoryBaseMgr : SessionBase, IFacilityCategoryBaseMgr
    {
        public IFacilityCategoryDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityCategory(FacilityCategory entity)
        {
            entityDao.CreateFacilityCategory(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityCategory LoadFacilityCategory(String code)
        {
            return entityDao.LoadFacilityCategory(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityCategory> GetAllFacilityCategory()
        {
            return entityDao.GetAllFacilityCategory();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityCategory(FacilityCategory entity)
        {
            entityDao.UpdateFacilityCategory(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityCategory(String code)
        {
            entityDao.DeleteFacilityCategory(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityCategory(FacilityCategory entity)
        {
            entityDao.DeleteFacilityCategory(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityCategory(IList<String> pkList)
        {
            entityDao.DeleteFacilityCategory(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityCategory(IList<FacilityCategory> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFacilityCategory(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
