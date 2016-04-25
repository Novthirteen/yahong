using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface ICheckupBaseDao
    {
        #region Method Created By CodeSmith

        void CreateCheckup(Checkup entity);

        Checkup LoadCheckup(Int32 id);
  
        IList<Checkup> GetAllCheckup();
  
        void UpdateCheckup(Checkup entity);
        
        void DeleteCheckup(Int32 id);
    
        void DeleteCheckup(Checkup entity);
    
        void DeleteCheckup(IList<Int32> pkList);
    
        void DeleteCheckup(IList<Checkup> entityList);    
        #endregion Method Created By CodeSmith
    }
}
