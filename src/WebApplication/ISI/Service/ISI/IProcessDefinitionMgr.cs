using com.Sconit.ISI.Entity;
using System;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IProcessDefinitionMgr : IProcessDefinitionBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.
        IList<ProcessDefinition> GetProcessDefinition(string taskSubType);

        IList<ProcessDefinition> GetProcessDefinition(string taskSubType, string processNo);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IProcessDefinitionMgrE : com.Sconit.ISI.Service.IProcessDefinitionMgr
    {
    }
}

#endregion Extend Interface