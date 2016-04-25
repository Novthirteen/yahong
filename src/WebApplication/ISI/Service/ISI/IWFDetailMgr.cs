using System;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IWFDetailMgr : IWFDetailBaseMgr
    {
        #region Customized Methods
        void CreateWFDetail(string taskCode, string status, string content, DateTime now, User user);
        void CreateWFDetail(string taskCode, string status, DateTime now, User user);
        void CreateWFDetail(string taskCode, string status, int? level, int? preLevel, DateTime now, User user);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IWFDetailMgrE : com.Sconit.ISI.Service.IWFDetailMgr
    {
    }
}

#endregion Extend Interface