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
    public class FacilityMaintainPlanBaseMgr : SessionBase, IFacilityMaintainPlanBaseMgr
    {
        public IFacilityMaintainPlanDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityMaintainPlan(FacilityMaintainPlan entity)
        {
            entityDao.CreateFacilityMaintainPlan(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityMaintainPlan LoadFacilityMaintainPlan(Int32 id)
        {
            return entityDao.LoadFacilityMaintainPlan(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityMaintainPlan> GetAllFacilityMaintainPlan()
        {
            return entityDao.GetAllFacilityMaintainPlan();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityMaintainPlan(FacilityMaintainPlan entity)
        {
            entityDao.UpdateFacilityMaintainPlan(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityMaintainPlan(Int32 id)
        {
            entityDao.DeleteFacilityMaintainPlan(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityMaintainPlan(FacilityMaintainPlan entity)
        {
            entityDao.DeleteFacilityMaintainPlan(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityMaintainPlan(IList<Int32> pkList)
        {
            entityDao.DeleteFacilityMaintainPlan(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityMaintainPlan(IList<FacilityMaintainPlan> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFacilityMaintainPlan(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
