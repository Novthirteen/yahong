using com.Sconit.Service.Ext.MasterData;
using System;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData
{
    public interface IItemMgr : IItemBaseMgr
    {
        #region Customized Methods

        IList<Item> GetCacheAllItem();

        IList<Item> GetPMItem();

        IList<Item> GetItem(DateTime lastModifyDate, int firstRow, int maxRows);

        IList<Item> GetItem(IList<string> itemCodeList);

        int GetItemCount(DateTime lastModifyDate);

        int GetItemCount(string itemCategoryCode);

        Item CheckAndLoadItem(string itemCode);

        void UpdateItem(Item item, User user);

        IList<Item> GetAllItem_Lit();

        #endregion Customized Methods
    }
}





#region Extend Interface





namespace com.Sconit.Service.Ext.MasterData
{
    public partial interface IItemMgrE : com.Sconit.Service.MasterData.IItemMgr
    {
        
    }
}

#endregion
