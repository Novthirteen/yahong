using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IAttachmentDetailBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateAttachmentDetail(AttachmentDetail entity);

        AttachmentDetail LoadAttachmentDetail(Int32 id);

        IList<AttachmentDetail> GetAllAttachmentDetail();
    
        void UpdateAttachmentDetail(AttachmentDetail entity);

        void DeleteAttachmentDetail(Int32 id);
    
        void DeleteAttachmentDetail(AttachmentDetail entity);

        void DeleteAttachmentDetail(IList<Int32> pkList);
    
        void DeleteAttachmentDetail(IList<AttachmentDetail> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
