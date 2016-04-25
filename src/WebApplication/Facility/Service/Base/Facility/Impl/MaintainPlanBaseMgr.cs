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
    public class MaintainPlanBaseMgr : SessionBase, IMaintainPlanBaseMgr
    {
        public IMaintainPlanDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateMaintainPlan(MaintainPlan entity)
        {
            entityDao.CreateMaintainPlan(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual MaintainPlan LoadMaintainPlan(String code)
        {
            return entityDao.LoadMaintainPlan(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<MaintainPlan> GetAllMaintainPlan()
        {
            return entityDao.GetAllMaintainPlan();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateMaintainPlan(MaintainPlan entity)
        {
            entityDao.UpdateMaintainPlan(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMaintainPlan(String code)
        {
            entityDao.DeleteMaintainPlan(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMaintainPlan(MaintainPlan entity)
        {
            entityDao.DeleteMaintainPlan(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMaintainPlan(IList<String> pkList)
        {
            entityDao.DeleteMaintainPlan(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMaintainPlan(IList<MaintainPlan> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteMaintainPlan(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
