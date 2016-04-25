using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IFilterMgr : IFilterBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IFilterMgrE : com.Sconit.ISI.Service.IFilterMgr
    {
    }
}

#endregion Extend Interface