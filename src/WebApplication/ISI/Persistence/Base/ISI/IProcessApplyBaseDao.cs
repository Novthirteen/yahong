using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IProcessApplyBaseDao
    {
        #region Method Created By CodeSmith

        void CreateProcessApply(ProcessApply entity);

        ProcessApply LoadProcessApply(Int32 id);
  
        IList<ProcessApply> GetAllProcessApply();
  
        void UpdateProcessApply(ProcessApply entity);
        
        void DeleteProcessApply(Int32 id);
    
        void DeleteProcessApply(ProcessApply entity);
    
        void DeleteProcessApply(IList<Int32> pkList);
    
        void DeleteProcessApply(IList<ProcessApply> entityList);    
        #endregion Method Created By CodeSmith
    }
}
