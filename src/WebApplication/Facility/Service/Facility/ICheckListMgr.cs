using com.Sconit.Entity.MasterData;
using com.Sconit.Facility.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Facility.Service
{
    public interface ICheckListMgr
    {
        void CreateCheckListOrder(CheckListOrderMaster checkListOrder);

        IList<CheckListMaster> GetAllCheckListMaster();

        void UpdateCheckListOrder(CheckListOrderMaster checkListOrder);

        CheckListMaster GetCheckListMaster(string code);

        CheckListOrderMaster GetCheckListOrderMaster(string code);

        void DeleteCheckListOrder(string checkListCode);
        void CloseCheckListMaster(string code, User user);
    }
}
