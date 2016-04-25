using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IWFDetailBaseDao
    {
        #region Method Created By CodeSmith

        void CreateWFDetail(WFDetail entity);

        WFDetail LoadWFDetail(Int32 id);
  
        IList<WFDetail> GetAllWFDetail();
  
        void UpdateWFDetail(WFDetail entity);
        
        void DeleteWFDetail(Int32 id);
    
        void DeleteWFDetail(WFDetail entity);
    
        void DeleteWFDetail(IList<Int32> pkList);
    
        void DeleteWFDetail(IList<WFDetail> entityList);    
        #endregion Method Created By CodeSmith
    }
}
