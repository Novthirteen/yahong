using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IWorkDetBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateWorkDet(WorkDet entity);

        WorkDet LoadWorkDet(Int32 iD);

        IList<WorkDet> GetAllWorkDet();
    
        void UpdateWorkDet(WorkDet entity);

        void DeleteWorkDet(Int32 iD);
    
        void DeleteWorkDet(WorkDet entity);
    
        void DeleteWorkDet(IList<Int32> pkList);
    
        void DeleteWorkDet(IList<WorkDet> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
