using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IResMatrixLogMgr : IResMatrixLogBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IResMatrixLogMgrE : com.Sconit.ISI.Service.IResMatrixLogMgr
    {
    }
}

#endregion Extend Interface