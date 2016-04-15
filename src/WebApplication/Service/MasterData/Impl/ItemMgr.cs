using com.Sconit.Service.Ext.MasterData;
using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Persistence.MasterData;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.Entity.Exception;
using com.Sconit.Service.Ext.Hql;
using System.Linq;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class ItemMgr : ItemBaseMgr, IItemMgr
    {
        private static IList<Item> cachedAllItem;
        public IItemKitMgrE itemKitMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        

        #region Customized Methods

        [Transaction(TransactionMode.Unspecified)]
        public IList<Item> GetCacheAllItem()
        {
            if (cachedAllItem == null)
            {
                cachedAllItem = GetAllItem();
            }
            else
            {
                //检查Item大小是否发生变化
                DetachedCriteria criteria = DetachedCriteria.For(typeof(Item));
                criteria.Add(Expression.Eq("IsActive", true));
                criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Code")));
                IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);

                if (count[0] != cachedAllItem.Count)
                {
                    cachedAllItem = GetAllItem();
                }
            }

            return cachedAllItem;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Item> GetAllItem_Lit()
        {
            string hql = "select i.Code,i.Desc1,i.Desc2 from Item as i ";
            IList<object[]> item_Lit = this.hqlMgrE.FindAll<object[]>(hql);
            IList<Item> items = (from i in item_Lit
                                 select new Item
                                 {
                                     Code=i[0].ToString(),
                                     Desc1= i[1].ToString(),
                                     Desc2 = i[2].ToString()
                                 }).ToList();
            return items;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Item> GetPMItem()
        {
            IList<Item> listItem = GetCacheAllItem();
            IList<Item> listPMItem = new List<Item>();
            foreach (Item item in listItem)
            {
                if (item.Type == BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_M || item.Type == BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_P)
                {
                    listPMItem.Add(item);
                }
            }

            return listPMItem;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Item> GetItem(DateTime lastModifyDate, int firstRow, int maxRows)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Item));
            criteria.Add(Expression.Gt("LastModifyDate", lastModifyDate));
            criteria.AddOrder(Order.Asc("LastModifyDate"));
            IList<Item> itemList = criteriaMgrE.FindAll<Item>(criteria, firstRow, maxRows);
            if (itemList.Count > 0)
            {
                return itemList;
            }
            return null;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Item> GetItem(IList<string> itemCodeList)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Item));
            if (itemCodeList.Count == 1)
            {
                criteria.Add(Expression.Eq("Code", itemCodeList[0]));
            }
            else
            {
                criteria.Add(Expression.InG<string>("Code", itemCodeList));
            }
            return criteriaMgrE.FindAll<Item>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public int GetItemCount(DateTime lastModifyDate)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Item));
            criteria.Add(Expression.Gt("LastModifyDate", lastModifyDate));
            IList<Item> itemList = criteriaMgrE.FindAll<Item>(criteria);
            return itemList.Count;
        }

        [Transaction(TransactionMode.Unspecified)]
        public int GetItemCount(string itemCategoryCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Item));
            criteria.Add(Expression.Eq("ItemCategory.Code", itemCategoryCode));
            IList<Item> itemList = criteriaMgrE.FindAll<Item>(criteria);
            return itemList.Count;
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteItem(string code)
        {
            IList<ItemKit> itemKitList = itemKitMgrE.GetChildItemKit(code, true);
            itemKitMgrE.DeleteItemKit(itemKitList);

            base.DeleteItem(code);
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteItem(Item entity)
        {
            IList<ItemKit> itemKitList = itemKitMgrE.GetChildItemKit(entity, true);
            itemKitMgrE.DeleteItemKit(itemKitList);

            base.DeleteItem(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteItem(IList<Item> entityList)
        {
            IList<ItemKit> itemKitList = new List<ItemKit>();
            foreach (Item item in entityList)
            {
                itemKitList = itemKitMgrE.GetChildItemKit(item, true);
                itemKitMgrE.DeleteItemKit(itemKitList);
            }

            base.DeleteItem(entityList);
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteItem(IList<string> pkList)
        {
            IList<ItemKit> itemKitList = new List<ItemKit>();
            foreach (string item in pkList)
            {
                itemKitList = itemKitMgrE.GetChildItemKit(item, true);
                itemKitMgrE.DeleteItemKit(itemKitList);
            }

            base.DeleteItem(pkList);
        }

        [Transaction(TransactionMode.Unspecified)]
        public Item CheckAndLoadItem(string itemCode)
        {
            Item item = this.LoadItem(itemCode);
            if (item == null)
            {
                throw new BusinessErrorException("Item.Error.ItemCodeNotExist", itemCode);
            }

            return item;
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateItem(Item item, User user)
        {
            item.LastModifyDate = DateTime.Now;
            item.LastModifyUser = user.Code;

            this.UpdateItem(item);
        }
        #endregion Customized Methods
    }
}


#region Extend Interface


namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class ItemMgrE : com.Sconit.Service.MasterData.Impl.ItemMgr, IItemMgrE
    {
        
    }
}
#endregion
