using System;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IEmppDetailMgr : IEmppDetailBaseMgr
    {
        #region Customized Methods

        IList<EmppDetail> GetEmppDetailByDestID(string DestID);

        IList<EmppDetail> GetEmppDetail(string MsgID);

        IList<EmppDetail> GetEmppDetail(string MsgID, int SeqID);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IEmppDetailMgrE : com.Sconit.ISI.Service.IEmppDetailMgr
    {
    }
}

#endregion Extend Interface