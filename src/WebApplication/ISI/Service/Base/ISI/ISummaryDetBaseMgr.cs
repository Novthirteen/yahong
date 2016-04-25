using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ISummaryDetBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateSummaryDet(SummaryDet entity);

        SummaryDet LoadSummaryDet(Int32 id);

        IList<SummaryDet> GetAllSummaryDet();
    
        void UpdateSummaryDet(SummaryDet entity);

        void DeleteSummaryDet(Int32 id);
    
        void DeleteSummaryDet(SummaryDet entity);
    
        void DeleteSummaryDet(IList<Int32> pkList);
    
        void DeleteSummaryDet(IList<SummaryDet> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
