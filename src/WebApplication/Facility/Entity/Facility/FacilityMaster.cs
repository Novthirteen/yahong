using System;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public class FacilityMaster : FacilityMasterBase
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

        public string ApplyPerson { get; set; }

        public string ApplySite { get; set; }

        public string ApplyOrg { get; set; }
        #endregion
    }
}