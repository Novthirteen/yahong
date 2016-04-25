using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IResPatrolBaseDao
    {
        #region Method Created By CodeSmith

        void CreateResPatrol(ResPatrol entity);

        ResPatrol LoadResPatrol(Int32 id);
  
        IList<ResPatrol> GetAllResPatrol();
  
        void UpdateResPatrol(ResPatrol entity);
        
        void DeleteResPatrol(Int32 id);
    
        void DeleteResPatrol(ResPatrol entity);
    
        void DeleteResPatrol(IList<Int32> pkList);
    
        void DeleteResPatrol(IList<ResPatrol> entityList);    
        #endregion Method Created By CodeSmith
    }
}
