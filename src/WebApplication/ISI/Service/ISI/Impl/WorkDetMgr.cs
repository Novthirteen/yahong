using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class WorkDetMgr : WorkDetBaseMgr, IWorkDetMgr
    {
        public IHqlMgrE hqlMgrE { get; set; }

        #region Customized Methods


        [Transaction(TransactionMode.Requires)]
        public void DeleteWorkDet(string taskCode)
        {
            var workDetList = hqlMgrE.FindAll<WorkDet>("from WorkDet where taskCode ='" + taskCode + "'");
            this.DeleteWorkDet(workDetList);
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class WorkDetMgrE : com.Sconit.ISI.Service.Impl.WorkDetMgr, IWorkDetMgrE
    {
    }
}

#endregion Extend Class