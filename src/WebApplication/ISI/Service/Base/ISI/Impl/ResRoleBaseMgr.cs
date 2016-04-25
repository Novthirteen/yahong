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
    public class ResRoleBaseMgr : SessionBase, IResRoleBaseMgr
    {
        public IResRoleDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateResRole(ResRole entity)
        {
            entityDao.CreateResRole(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ResRole LoadResRole(String code)
        {
            return entityDao.LoadResRole(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ResRole> GetAllResRole()
        {
            return entityDao.GetAllResRole(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ResRole> GetAllResRole(bool includeInactive)
        {
            return entityDao.GetAllResRole(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateResRole(ResRole entity)
        {
            entityDao.UpdateResRole(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResRole(String code)
        {
            entityDao.DeleteResRole(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResRole(ResRole entity)
        {
            entityDao.DeleteResRole(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResRole(IList<String> pkList)
        {
            entityDao.DeleteResRole(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResRole(IList<ResRole> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteResRole(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
