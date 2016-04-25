using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IHelpLotDetailBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateHelpLotDetail(HelpLotDetail entity);

        HelpLotDetail LoadHelpLotDetail(Int32 id);

        IList<HelpLotDetail> GetAllHelpLotDetail();
    
        void UpdateHelpLotDetail(HelpLotDetail entity);

        void DeleteHelpLotDetail(Int32 id);
    
        void DeleteHelpLotDetail(HelpLotDetail entity);
    
        void DeleteHelpLotDetail(IList<Int32> pkList);
    
        void DeleteHelpLotDetail(IList<HelpLotDetail> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
