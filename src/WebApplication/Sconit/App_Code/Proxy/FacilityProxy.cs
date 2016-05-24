using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Persistence;
using NHibernate;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.Procurement;
using com.Sconit.Service.Ext.Distribution;
using com.Sconit.Entity.Procurement;
using com.Sconit.Entity.Distribution;
using com.Sconit.Entity.Batch;
using com.Sconit.Service.Ext.Batch;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Facility.Service.Ext;
using com.Sconit.Facility.Entity;

/// <summary>
/// Summary description for ManagerProxy
/// </summary>
namespace com.Sconit.Web
{

    public class FacilityMasterMgrProxy
    {
        private IFacilityMasterMgrE FacilityMasterMgr
        {
            get
            {
                return ServiceLocator.GetService<IFacilityMasterMgrE>("FacilityMasterMgr.service");
            }
        }

        public FacilityMasterMgrProxy()
        {
        }

        public void CreateFacilityMaster(FacilityMaster facilityMaster)
        {
            FacilityMasterMgr.CreateFacilityMaster(facilityMaster);
        }

        public FacilityMaster LoadFacilityMaster(string fcId)
        {
            return FacilityMasterMgr.LoadFacilityMaster(fcId);
        }

        public void UpdateFacilityMaster(FacilityMaster facilityMaster)
        {
            FacilityMasterMgr.UpdateFacilityMaster(facilityMaster);
        }

        public void DeleteFacilityMaster(FacilityMaster facilityMaster)
        {
            FacilityMasterMgr.DeleteFacilityMaster(facilityMaster);
        }

        public IList<FacilityMaster> GetAllFacilityMaster()
        {
            return FacilityMasterMgr.GetAllFacilityMaster();
        }

        public void UpdateFacilityMasterMaintain(FacilityMaster facilityMaster)
        {
            FacilityMasterMgr.UpdateFacilityMasterMaintain(facilityMaster);
        }

        public FacilityTrans LoadFacilityMaintain(string fcId)
        {
            return FacilityMasterMgr.LoadFacilityMaintain(fcId);
        }

        public FacilityTrans LoadFacilityFix(string fcId)
        {
            return FacilityMasterMgr.LoadFacilityFix(fcId);
        }

        public FacilityTrans LoadFacilityInspect(string fcId)
        {
            return FacilityMasterMgr.LoadFacilityInspect(fcId);
        }

        public FacilityTrans LoadFacilityEnvelop(string fcId)
        {
            return FacilityMasterMgr.LoadFacilityEnvelop(fcId);
        }
    }

    public class FacilityCategoryMgrProxy
    {
        private IFacilityCategoryMgrE FacilityCategoryMgr
        {
            get
            {
                return ServiceLocator.GetService<IFacilityCategoryMgrE>("FacilityCategoryMgr.service");
            }
        }

        public FacilityCategoryMgrProxy()
        {
        }

        public void CreateFacilityCategory(FacilityCategory facilityCategory)
        {
            FacilityCategoryMgr.CreateFacilityCategory(facilityCategory);
        }

        public FacilityCategory LoadFacilityCategory(string code)
        {
            return FacilityCategoryMgr.LoadFacilityCategory(code);
        }

        public void UpdateFacilityCategory(FacilityCategory facilityCategory)
        {
            FacilityCategoryMgr.UpdateFacilityCategory(facilityCategory);
        }

        public void DeleteFacilityCategory(FacilityCategory facilityCategory)
        {
            FacilityCategoryMgr.DeleteFacilityCategory(facilityCategory);
        }

        public IList<FacilityCategory> GetAllFacilityCategory()
        {
            return FacilityCategoryMgr.GetAllFacilityCategory();
        }
    }

    public class MaintainPlanMgrProxy
    {
        private IMaintainPlanMgrE MaintainPlanMgr
        {
            get
            {
                return ServiceLocator.GetService<IMaintainPlanMgrE>("MaintainPlanMgr.service");
            }
        }

        public MaintainPlanMgrProxy()
        {
        }

        public void CreateMaintainPlan(MaintainPlan maintainPlan)
        {
            MaintainPlanMgr.CreateMaintainPlan(maintainPlan);
        }

        public MaintainPlan LoadMaintainPlan(string code)
        {
            return MaintainPlanMgr.LoadMaintainPlan(code);
        }

        public void UpdateMaintainPlan(MaintainPlan maintainPlan)
        {
            MaintainPlanMgr.UpdateMaintainPlan(maintainPlan);
        }

        public void DeleteMaintainPlan(MaintainPlan maintainPlan)
        {
            MaintainPlanMgr.DeleteMaintainPlan(maintainPlan);
        }

        public IList<MaintainPlan> GetAllMaintainPlan()
        {
            return MaintainPlanMgr.GetAllMaintainPlan();
        }
    }

    public class FacilityMaintainPlanMgrProxy
    {
        private IFacilityMaintainPlanMgrE FacilityMaintainPlanMgr
        {
            get
            {
                return ServiceLocator.GetService<IFacilityMaintainPlanMgrE>("FacilityMaintainPlanMgr.service");
            }
        }

        public FacilityMaintainPlanMgrProxy()
        {
        }

        public void CreateMaintainPlan(FacilityMaintainPlan facilitymaintainPlan)
        {
            FacilityMaintainPlanMgr.CreateFacilityMaintainPlan(facilitymaintainPlan);
        }

        public FacilityMaintainPlan LoadFacilityMaintainPlan(int id)
        {
            return FacilityMaintainPlanMgr.LoadFacilityMaintainPlan(id);
        }

        public void UpdateFacilityMaintainPlan(FacilityMaintainPlan facilitymaintainPlan)
        {
            FacilityMaintainPlanMgr.UpdateFacilityMaintainPlan(facilitymaintainPlan);
        }

        public void DeleteFacilityMaintainPlan(FacilityMaintainPlan facilitymaintainPlan)
        {
            FacilityMaintainPlanMgr.DeleteFacilityMaintainPlan(facilitymaintainPlan);
        }

        public IList<FacilityMaintainPlan> GetAllFacilityMaintainPlan()
        {
            return FacilityMaintainPlanMgr.GetAllFacilityMaintainPlan();
        }
    }

    public class FacilityTransMgrProxy
    {
        private IFacilityTransMgrE  FacilityTransMgr
        {
            get
            {
                return ServiceLocator.GetService<IFacilityTransMgrE>("FacilityTransMgr.service");
            }
        }

        public FacilityTransMgrProxy()
        {
        }

        public void CreateFacilityTransMgr(FacilityTrans facilityTrans)
        {
            FacilityTransMgr.CreateFacilityTrans(facilityTrans);
        }

        public FacilityTrans LoadFacilityTrans(int id)
        {
            return FacilityTransMgr.LoadFacilityTrans(id);
        }

        public void UpdateFacilityTrans(FacilityTrans facilityTrans)
        {
            FacilityTransMgr.UpdateFacilityTrans(facilityTrans);
        }

        public void DeleteFacilityTrans(FacilityTrans facilityTrans)
        {
            FacilityTransMgr.DeleteFacilityTrans(facilityTrans);
        }

        public IList<FacilityTrans> GetAllFacilityTrans()
        {
            return FacilityTransMgr.GetAllFacilityTrans();
        }

    }


    public class FacilityItemMgrProxy
    {
        private IFacilityItemMgrE FacilityItemMgr
        {
            get
            {
                return ServiceLocator.GetService<IFacilityItemMgrE>("FacilityItemMgr.service");
            }
        }

        public FacilityItemMgrProxy()
        {
        }

        public void CreateFacilityItem(FacilityItem facilityItem)
        {
            FacilityItemMgr.CreateFacilityItem(facilityItem);
        }

        public FacilityItem LoadFacilityItem(int id)
        {
            return FacilityItemMgr.LoadFacilityItem(id);
        }

        public void UpdateFacilityItem(FacilityItem facilityItem)
        {
            FacilityItemMgr.UpdateFacilityItem(facilityItem);
        }

        public void DeleteFacilityItem(FacilityItem facilityItem)
        {
            FacilityItemMgr.DeleteFacilityItem(facilityItem);
        }

        public IList<FacilityItem> GetAllFacilityItem()
        {
            return FacilityItemMgr.GetAllFacilityItem();
        }

    }

    public class FacilityDistributionMgrProxy
    {
        private IFacilityDistributionMgrE FacilityDistributionMgr
        {
            get
            {
                return ServiceLocator.GetService<IFacilityDistributionMgrE>("FacilityDistributionMgr.service");
            }
        }

        public FacilityDistributionMgrProxy()
        {
        }

        public void CreateFacilityDistribution(FacilityDistribution facilityDistribution)
        {
            FacilityDistributionMgr.CreateFacilityDistribution(facilityDistribution);
        }

        public FacilityDistribution LoadFacilityDistribution(int id)
        {
            return FacilityDistributionMgr.LoadFacilityDistribution(id);
        }

        public void UpdateFacilityDistribution(FacilityDistribution facilityDistribution)
        {
            FacilityDistributionMgr.UpdateFacilityDistribution(facilityDistribution);
        }

        public void DeleteFacilityDistribution(FacilityDistribution facilityDistribution)
        {
            FacilityDistributionMgr.DeleteFacilityDistribution(facilityDistribution);
        }

        public IList<FacilityDistribution> GetAllFacilityDistribution()
        {
            return FacilityDistributionMgr.GetAllFacilityDistribution();
        }

    }

    public class FacilityDistributionDetailMgrProxy
    {
        private IFacilityDistributionDetailMgrE FacilityDistributionDetailMgr
        {
            get
            {
                return ServiceLocator.GetService<IFacilityDistributionDetailMgrE>("FacilityDistributionDetailMgr.service");
            }
        }

        public FacilityDistributionDetailMgrProxy()
        {
        }

        public void CreateFacilityDistributionDetail(FacilityDistributionDetail facilityDistributionDetail)
        {
            FacilityDistributionDetailMgr.CreateFacilityDistributionDetail(facilityDistributionDetail);
        }

        public FacilityDistributionDetail LoadFacilityDistributionDetail(int id)
        {
            return FacilityDistributionDetailMgr.LoadFacilityDistributionDetail(id);
        }

        public void UpdateFacilityDistributionDetail(FacilityDistributionDetail facilityDistributionDetail)
        {
            FacilityDistributionDetailMgr.UpdateFacilityDistributionDetail(facilityDistributionDetail);
        }

        public void DeleteFacilityDistributionDetail(FacilityDistributionDetail facilityDistributionDetail)
        {
            FacilityDistributionDetailMgr.DeleteFacilityDistributionDetail(facilityDistributionDetail);
        }

        public IList<FacilityDistributionDetail> GetAllFacilityDistributionDetail()
        {
            return FacilityDistributionDetailMgr.GetAllFacilityDistributionDetail();
        }

    }

    public class FacilityAllocateMgrProxy
    {
        private IFacilityAllocateMgrE FacilityAllocateMgr
        {
            get
            {
                return ServiceLocator.GetService<IFacilityAllocateMgrE>("FacilityAllocateMgr.service");
            }
        }

        public FacilityAllocateMgrProxy()
        {
        }

        public void CreateFacilityAllocate(FacilityAllocate facilityAllocate)
        {
            FacilityAllocateMgr.CreateFacilityAllocate(facilityAllocate);
        }

        public FacilityAllocate LoadFacilityAllocate(int id)
        {
            return FacilityAllocateMgr.LoadFacilityAllocate(id);
        }

        public void UpdateFacilityAllocate(FacilityAllocate facilityAllocate)
        {
            FacilityAllocateMgr.UpdateFacilityAllocate(facilityAllocate);
        }

        public void DeleteFacilityAllocate(FacilityAllocate facilityAllocate)
        {
            FacilityAllocateMgr.DeleteFacilityAllocate(facilityAllocate);
        }

        public IList<FacilityAllocate> GetAllFacilityAllocate()
        {
            return FacilityAllocateMgr.GetAllFacilityAllocate();
        }

    }
}