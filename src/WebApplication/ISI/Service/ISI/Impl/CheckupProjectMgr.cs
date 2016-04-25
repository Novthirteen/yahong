using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Service.Util;
using NHibernate.Expression;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class CheckupProjectMgr : CheckupProjectBaseMgr, ICheckupProjectMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods
        [Transaction(TransactionMode.Unspecified)]
        public IList<CheckupProject> GetAllCheckupProject(string type)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(CheckupProject));
            criteria.Add(Expression.Eq("IsActive", true));
            criteria.Add(Expression.In("Type", new string[] { ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECT_TYPE_GENERAL, type }));

            return this.criteriaMgrE.FindAll<CheckupProject>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<CheckupProject> GetAllCheckupProject(string checkupUser, DateTime? checkupDate, string type)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(CheckupProject));
            criteria.Add(Expression.Eq("IsActive", true));
            criteria.Add(Expression.In("Type", new string[] { ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECT_TYPE_GENERAL, type }));

            if (!string.IsNullOrEmpty(checkupUser))
            {
                DetachedCriteria subCriteria = DetachedCriteria.For(typeof(Checkup));
                subCriteria.Add(Expression.Eq("CheckupUser", checkupUser));
                if (checkupDate.HasValue)
                {
                    DateTime startTime = ISIUtil.FirstDayOfMonth(checkupDate.Value);
                    DateTime endTime = ISIUtil.LastDayOfMonth(checkupDate.Value);
                    subCriteria.Add(Expression.Ge("CheckupDate", startTime));
                    subCriteria.Add(Expression.Le("CheckupDate", endTime));
                    subCriteria.Add(Expression.In("Status", new string[]{
                                        ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CREATE,
                                        ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT,
                                        ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL,
                                        ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE}));
                }
                subCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("CheckupProject.Code")));
                criteria.Add(
                        Subqueries.PropertyNotIn("Code", subCriteria));
            }
            return this.criteriaMgrE.FindAll<CheckupProject>(criteria);

        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class CheckupProjectMgrE : com.Sconit.ISI.Service.Impl.CheckupProjectMgr, ICheckupProjectMgrE
    {
    }
}

#endregion Extend Class