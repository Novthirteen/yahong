using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IFailureModeBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateFailureMode(FailureMode entity);

        FailureMode LoadFailureMode(String code);

        IList<FailureMode> GetAllFailureMode();
    
        IList<FailureMode> GetAllFailureMode(bool includeInactive);
      
        void UpdateFailureMode(FailureMode entity);

        void DeleteFailureMode(String code);
    
        void DeleteFailureMode(FailureMode entity);
    
        void DeleteFailureMode(IList<String> pkList);
    
        void DeleteFailureMode(IList<FailureMode> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
