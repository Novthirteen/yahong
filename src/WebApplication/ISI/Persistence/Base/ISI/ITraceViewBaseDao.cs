using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface ITraceViewBaseDao
    {
        #region Method Created By CodeSmith

        void CreateTraceView(TraceView entity);

        TraceView LoadTraceView(Int32 id, String type);
  
        IList<TraceView> GetAllTraceView();
  
        void UpdateTraceView(TraceView entity);
        
        void DeleteTraceView(Int32 id, String type);
    
        
    
        void DeleteTraceView(TraceView entity);
    
        void DeleteTraceView(IList<TraceView> entityList);    
        #endregion Method Created By CodeSmith
    }
}
