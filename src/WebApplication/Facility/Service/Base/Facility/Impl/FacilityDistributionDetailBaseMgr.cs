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
    public class FacilityDistributionDetailBaseMgr : SessionBase, IFacilityDistributionDetailBaseMgr
    {
        public IFacilityDistributionDetailDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityDistributionDetail(FacilityDistributionDetail entity)
        {
            entityDao.CreateFacilityDistributionDetail(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityDistributionDetail LoadFacilityDistributionDetail(Int32 id)
        {
            return entityDao.LoadFacilityDistributionDetail(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityDistributionDetail> GetAllFacilityDistributionDetail()
        {
            return entityDao.GetAllFacilityDistributionDetail();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityDistributionDetail(FacilityDistributionDetail entity)
        {
            entityDao.UpdateFacilityDistributionDetail(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityDistributionDetail(Int32 id)
        {
            entityDao.DeleteFacilityDistributionDetail(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityDistributionDetail(FacilityDistributionDetail entity)
        {
            entityDao.DeleteFacilityDistributionDetail(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityDistributionDetail(IList<Int32> pkList)
        {
            if ((pkList == null) || (pkList.Count == 0))
            {
                return;
            }

            entityDao.DeleteFacilityDistributionDetail(pkList);
        }   
        #endregion Method Created By CodeSmith
    }
}
