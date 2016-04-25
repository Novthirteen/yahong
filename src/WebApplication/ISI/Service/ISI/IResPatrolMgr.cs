using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IResPatrolMgr : IResPatrolBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IResPatrolMgrE : com.Sconit.ISI.Service.IResPatrolMgr
    {
    }
}

#endregion Extend Interface