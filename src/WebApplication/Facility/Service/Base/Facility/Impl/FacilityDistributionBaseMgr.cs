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
    public class FacilityDistributionBaseMgr : SessionBase, IFacilityDistributionBaseMgr
    {
        public IFacilityDistributionDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityDistribution(FacilityDistribution entity)
        {
            entityDao.CreateFacilityDistribution(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityDistribution LoadFacilityDistribution(Int32 id)
        {
            return entityDao.LoadFacilityDistribution(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityDistribution> GetAllFacilityDistribution()
        {
            return entityDao.GetAllFacilityDistribution();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityDistribution(FacilityDistribution entity)
        {
            entityDao.UpdateFacilityDistribution(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityDistribution(Int32 id)
        {
            entityDao.DeleteFacilityDistribution(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityDistribution(FacilityDistribution entity)
        {
            entityDao.DeleteFacilityDistribution(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityDistribution(IList<Int32> pkList)
        {
            entityDao.DeleteFacilityDistribution(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityDistribution(IList<FacilityDistribution> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFacilityDistribution(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
