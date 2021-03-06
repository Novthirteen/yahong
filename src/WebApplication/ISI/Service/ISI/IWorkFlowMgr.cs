using System;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using System.Text;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IWorkflowMgr
    {
        void Create(TaskMstr task, DateTime effDate, User user);
        //int? StartProcessInstance(TaskMstr task, string assignUser, DateTime effDate, User user);
        void StartProcessInstance(TaskMstr task, string assignUser, DateTime effDate, User user);

        void ProcessNew(TaskMstr task, string wfsStatus, string approveDesc, string color, IList<object> countersignList, bool isiAdmin, DateTime effDate, bool isEmail, User user);
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IWorkflowMgrE : com.Sconit.ISI.Service.IWorkflowMgr
    {
    }
}

#endregion Extend Interface