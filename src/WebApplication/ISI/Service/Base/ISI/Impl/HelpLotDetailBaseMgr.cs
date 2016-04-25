using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class HelpLotDetailBaseMgr : SessionBase, IHelpLotDetailBaseMgr
    {
        public IHelpLotDetailDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateHelpLotDetail(HelpLotDetail entity)
        {
            entityDao.CreateHelpLotDetail(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual HelpLotDetail LoadHelpLotDetail(Int32 id)
        {
            return entityDao.LoadHelpLotDetail(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<HelpLotDetail> GetAllHelpLotDetail()
        {
            return entityDao.GetAllHelpLotDetail();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateHelpLotDetail(HelpLotDetail entity)
        {
            entityDao.UpdateHelpLotDetail(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteHelpLotDetail(Int32 id)
        {
            entityDao.DeleteHelpLotDetail(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteHelpLotDetail(HelpLotDetail entity)
        {
            entityDao.DeleteHelpLotDetail(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteHelpLotDetail(IList<Int32> pkList)
        {
            entityDao.DeleteHelpLotDetail(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteHelpLotDetail(IList<HelpLotDetail> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteHelpLotDetail(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
