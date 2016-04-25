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
    public class AttachmentDetailBaseMgr : SessionBase, IAttachmentDetailBaseMgr
    {
        public IAttachmentDetailDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateAttachmentDetail(AttachmentDetail entity)
        {
            entityDao.CreateAttachmentDetail(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual AttachmentDetail LoadAttachmentDetail(Int32 id)
        {
            return entityDao.LoadAttachmentDetail(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<AttachmentDetail> GetAllAttachmentDetail()
        {
            return entityDao.GetAllAttachmentDetail();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateAttachmentDetail(AttachmentDetail entity)
        {
            entityDao.UpdateAttachmentDetail(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteAttachmentDetail(Int32 id)
        {
            entityDao.DeleteAttachmentDetail(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteAttachmentDetail(AttachmentDetail entity)
        {
            entityDao.DeleteAttachmentDetail(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteAttachmentDetail(IList<Int32> pkList)
        {
            entityDao.DeleteAttachmentDetail(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteAttachmentDetail(IList<AttachmentDetail> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteAttachmentDetail(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
