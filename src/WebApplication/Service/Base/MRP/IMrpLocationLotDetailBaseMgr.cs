using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Entity.MRP;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP
{
    public interface IMrpLocationLotDetailBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateMrpLocationLotDetail(MrpLocationLotDetail entity);

        MrpLocationLotDetail LoadMrpLocationLotDetail(Int32 id);

        IList<MrpLocationLotDetail> GetAllMrpLocationLotDetail();
    
        void UpdateMrpLocationLotDetail(MrpLocationLotDetail entity);

        void DeleteMrpLocationLotDetail(Int32 id);
    
        void DeleteMrpLocationLotDetail(MrpLocationLotDetail entity);
    
        void DeleteMrpLocationLotDetail(IList<Int32> pkList);
    
        void DeleteMrpLocationLotDetail(IList<MrpLocationLotDetail> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
