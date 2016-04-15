using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class ItemDiscontinueMgr : ItemDiscontinueBaseMgr, IItemDiscontinueMgr
    {
        public ICriteriaMgrE criteriaMgr { get; set; }

        #region Customized Methods

        public IList<ItemDiscontinue> GetItemDiscontinue(Item item, Bom bom, DateTime effectiveDate)
        {
            DetachedCriteria criteria = DetachedCriteria.For<ItemDiscontinue>();

            criteria.Add(Expression.Eq("Item", item));
            criteria.Add(Expression.Le("StartDate", effectiveDate));
            criteria.Add(Expression.Or(Expression.IsNull("EndDate"), Expression.Ge("EndDate", effectiveDate)));

            if (bom != null)
            {
                criteria.Add(Expression.Or(Expression.IsNull("Bom"), Expression.Eq("Bom", bom)));
            }

            criteria.AddOrder(Order.Asc("Priority"));

            return this.criteriaMgr.FindAll<ItemDiscontinue>(criteria);
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class ItemDiscontinueMgrE : com.Sconit.Service.MasterData.Impl.ItemDiscontinueMgr, IItemDiscontinueMgrE
    {
    }
}

#endregion Extend Class