using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface ISummaryBaseDao
    {
        #region Method Created By CodeSmith

        void CreateSummary(Summary entity);

        Summary LoadSummary(String code);
  
        IList<Summary> GetAllSummary();
  
        void UpdateSummary(Summary entity);
        
        void DeleteSummary(String code);
    
        void DeleteSummary(Summary entity);
    
        void DeleteSummary(IList<String> pkList);
    
        void DeleteSummary(IList<Summary> entityList);    
        #endregion Method Created By CodeSmith
    }
}
