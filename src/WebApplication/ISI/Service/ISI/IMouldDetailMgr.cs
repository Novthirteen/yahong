using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IMouldDetailMgr
    {
        #region Customized Methods
        IList<MouldDetail> GetMouldDetail(string code);
        void DeleteMouldDetail(MouldDetail mouldDetail);
        MouldDetail LoadMouldDetail(int id);
        void CreateMouldDetail(MouldDetail mouldDetail);
        void UpdateMouldDetail(MouldDetail mouldDetail);
        void DeleteMouldDetail(int id);
        IList<MouldDetail> GetAllMouldDetail();
        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IMouldDetailMgrE : com.Sconit.ISI.Service.IMouldDetailMgr
    {
    }
}

#endregion Extend Interface