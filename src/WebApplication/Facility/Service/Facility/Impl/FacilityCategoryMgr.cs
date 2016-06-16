using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;
using com.Sconit.Facility.Entity;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityCategoryMgr : FacilityCategoryBaseMgr, IFacilityCategoryMgr
    {
        #region Customized Methods
        public ICriteriaMgrE criteriaMgrE { get; set; }


        public IList<FacilityCategory> GetAllMouldCategory()
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityCategory));
            criteria.Add(Expression.Eq("ParentCategory", "YH_MJ"));
            return criteriaMgrE.FindAll<FacilityCategory>(criteria);
        }


        public IList<FacilityCategory> GetAllEquipmentCategory()
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityCategory));
            criteria.Add(Expression.Eq("ParentCategory", "YH_SB"));
            return criteriaMgrE.FindAll<FacilityCategory>(criteria);
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityCategoryMgrE : com.Sconit.Facility.Service.Impl.FacilityCategoryMgr, IFacilityCategoryMgrE
    {
    }
}

#endregion Extend Class