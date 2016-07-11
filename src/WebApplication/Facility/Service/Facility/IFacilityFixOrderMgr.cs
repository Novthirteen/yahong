using System;
using com.Sconit.Facility.Entity;
using System.Collections.Generic;
using System.IO;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.MasterData;



//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityFixOrderMgr 
    {
        #region Customized Methods

        void CreateFacilityFixOrder(FacilityFixOrder facilityFixOrder);

        void ReleaseFacilityFixOrder(string fixNo,string userCode);

        void StartFacilityFixOrder(string fixNo, string userCode);

        void CompleteFacilityFixOrder(string fixNo, string userCode);

        void CloseFacilityFixOrder(string fixNo, string userCode);

        FacilityFixOrder GetFacilityFixOrder(string fcId);
        
        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityFixOrderMgrE : com.Sconit.Facility.Service.IFacilityFixOrderMgr
    {
    }
}

#endregion Extend Interface