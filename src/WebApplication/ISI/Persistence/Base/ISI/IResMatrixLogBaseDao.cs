using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IResMatrixLogBaseDao
    {
        #region Method Created By CodeSmith

        void CreateResMatrixLog(ResMatrixLog entity);

        ResMatrixLog LoadResMatrixLog(Int32 id);
  
        IList<ResMatrixLog> GetAllResMatrixLog();
  
        void UpdateResMatrixLog(ResMatrixLog entity);
        
        void DeleteResMatrixLog(Int32 id);
    
        void DeleteResMatrixLog(ResMatrixLog entity);
    
        void DeleteResMatrixLog(IList<Int32> pkList);
    
        void DeleteResMatrixLog(IList<ResMatrixLog> entityList);    
        #endregion Method Created By CodeSmith
    }
}
