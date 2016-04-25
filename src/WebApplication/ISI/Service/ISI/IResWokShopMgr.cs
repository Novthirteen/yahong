using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IResWokShopMgr : IResWokShopBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IResWokShopMgrE : com.Sconit.ISI.Service.IResWokShopMgr
    {
    }
}

#endregion Extend Interface