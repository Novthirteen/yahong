using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Entity.MRP;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP
{
    public interface IMrpPlanTransactionBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateMrpPlanTransaction(MrpPlanTransaction entity);

        MrpPlanTransaction LoadMrpPlanTransaction(Int32 id);

        IList<MrpPlanTransaction> GetAllMrpPlanTransaction();
    
        void UpdateMrpPlanTransaction(MrpPlanTransaction entity);

        void DeleteMrpPlanTransaction(Int32 id);
    
        void DeleteMrpPlanTransaction(MrpPlanTransaction entity);
    
        void DeleteMrpPlanTransaction(IList<Int32> pkList);
    
        void DeleteMrpPlanTransaction(IList<MrpPlanTransaction> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
