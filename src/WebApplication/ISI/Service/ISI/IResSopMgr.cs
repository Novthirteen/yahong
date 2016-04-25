using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IResSopMgr : IResSopBaseMgr
    {
        #region Customized Methods
        ResSop LoadResSop(string workShop, int operate);

        List<ResSop> GetResSopList(string workShop);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IResSopMgrE : com.Sconit.ISI.Service.IResSopMgr
    {
    }
}

#endregion Extend Interface