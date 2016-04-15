using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity.MRP;
using com.Sconit.Persistence.MRP;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP.Impl
{
    [Transactional]
    public class MrpLocationLotDetailBaseMgr : SessionBase, IMrpLocationLotDetailBaseMgr
    {
        public IMrpLocationLotDetailDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateMrpLocationLotDetail(MrpLocationLotDetail entity)
        {
            entityDao.CreateMrpLocationLotDetail(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual MrpLocationLotDetail LoadMrpLocationLotDetail(Int32 id)
        {
            return entityDao.LoadMrpLocationLotDetail(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<MrpLocationLotDetail> GetAllMrpLocationLotDetail()
        {
            return entityDao.GetAllMrpLocationLotDetail();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateMrpLocationLotDetail(MrpLocationLotDetail entity)
        {
            entityDao.UpdateMrpLocationLotDetail(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMrpLocationLotDetail(Int32 id)
        {
            entityDao.DeleteMrpLocationLotDetail(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMrpLocationLotDetail(MrpLocationLotDetail entity)
        {
            entityDao.DeleteMrpLocationLotDetail(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMrpLocationLotDetail(IList<Int32> pkList)
        {
            entityDao.DeleteMrpLocationLotDetail(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMrpLocationLotDetail(IList<MrpLocationLotDetail> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteMrpLocationLotDetail(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
