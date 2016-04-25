using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IPositionMgr : IPositionBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IPositionMgrE : com.Sconit.ISI.Service.IPositionMgr
    {
    }
}

#endregion Extend Interface