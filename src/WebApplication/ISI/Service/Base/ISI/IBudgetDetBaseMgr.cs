using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IBudgetDetBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateBudgetDet(BudgetDet entity);

        BudgetDet LoadBudgetDet(Int32 id);

        IList<BudgetDet> GetAllBudgetDet();
    
        void UpdateBudgetDet(BudgetDet entity);

        void DeleteBudgetDet(Int32 id);
    
        void DeleteBudgetDet(BudgetDet entity);
    
        void DeleteBudgetDet(IList<Int32> pkList);
    
        void DeleteBudgetDet(IList<BudgetDet> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
