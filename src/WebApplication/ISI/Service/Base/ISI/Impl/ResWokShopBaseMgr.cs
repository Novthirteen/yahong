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
    public class ResWokShopBaseMgr : SessionBase, IResWokShopBaseMgr
    {
        public IResWokShopDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateResWokShop(ResWokShop entity)
        {
            entityDao.CreateResWokShop(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ResWokShop LoadResWokShop(String code)
        {
            return entityDao.LoadResWokShop(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ResWokShop> GetAllResWokShop()
        {
            return entityDao.GetAllResWokShop(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ResWokShop> GetAllResWokShop(bool includeInactive)
        {
            return entityDao.GetAllResWokShop(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateResWokShop(ResWokShop entity)
        {
            entityDao.UpdateResWokShop(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResWokShop(String code)
        {
            entityDao.DeleteResWokShop(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResWokShop(ResWokShop entity)
        {
            entityDao.DeleteResWokShop(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResWokShop(IList<String> pkList)
        {
            entityDao.DeleteResWokShop(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResWokShop(IList<ResWokShop> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteResWokShop(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
