using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IMouldMgr
    {
        #region Customized Methods
        IList<Mould> GetMould(string prjCode, string type);
        IList<Mould> GetMouldList(string supplierContractNo);
        IList<Mould> GetMouldList(string code, string supplierContractNo);
        void RemindBill();
        IList<Mould> GetAllMould();
        void DeleteMould(Mould mould);
        string CopyMould(string code, User user);
        void CreateMould(Mould mould);
        void DeleteMould(string code);
        void UpdateMould(Mould mould);
        Mould LoadMould(string code);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IMouldMgrE : com.Sconit.ISI.Service.IMouldMgr
    {
    }
}

#endregion Extend Interface