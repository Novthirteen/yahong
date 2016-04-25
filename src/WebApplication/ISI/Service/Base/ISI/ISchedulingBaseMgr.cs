using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ISchedulingBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateScheduling(Scheduling entity);

        Scheduling LoadScheduling(Int32 id);

        IList<Scheduling> GetAllScheduling();
    
        void UpdateScheduling(Scheduling entity);

        void DeleteScheduling(Int32 id);
    
        void DeleteScheduling(Scheduling entity);
    
        void DeleteScheduling(IList<Int32> pkList);
    
        void DeleteScheduling(IList<Scheduling> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
