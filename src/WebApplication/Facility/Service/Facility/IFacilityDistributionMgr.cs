using System;
using com.Sconit.Facility.Entity;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityDistributionMgr : IFacilityDistributionBaseMgr
    {
        #region Customized Methods

        IList<FacilityDistribution> GetFacilityDistributionList(string purchaseContractCode);

        IList<FacilityDistribution> GetFacilityDistributionList(int? id, string purchaseContractCode);

        IList<FacilityDistribution> GetFacilityDistributionSupplier();

        IList<FacilityDistribution> GetFacilityDistributionCustomer();

        void CopyFacilityDistribution(FacilityDistribution facilityDistribution, User user);

        IList<FacilityDistribution> GetFacilityDistributionListByDistribution(string distributionContractCode);

        IList<FacilityDistribution> GetFacilityDistributionListByCode(string code);
        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityDistributionMgrE : com.Sconit.Facility.Service.IFacilityDistributionMgr
    {
    }
}

#endregion Extend Interface