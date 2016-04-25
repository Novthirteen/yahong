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
    public class FacilityTransBaseMgr : SessionBase, IFacilityTransBaseMgr
    {
        public IFacilityTransDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFacilityTrans(FacilityTrans entity)
        {
            entityDao.CreateFacilityTrans(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FacilityTrans LoadFacilityTrans(Int32 id)
        {
            return entityDao.LoadFacilityTrans(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FacilityTrans> GetAllFacilityTrans()
        {
            return entityDao.GetAllFacilityTrans();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFacilityTrans(FacilityTrans entity)
        {
            entityDao.UpdateFacilityTrans(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityTrans(Int32 id)
        {
            entityDao.DeleteFacilityTrans(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityTrans(FacilityTrans entity)
        {
            entityDao.DeleteFacilityTrans(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityTrans(IList<Int32> pkList)
        {
            entityDao.DeleteFacilityTrans(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFacilityTrans(IList<FacilityTrans> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFacilityTrans(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
