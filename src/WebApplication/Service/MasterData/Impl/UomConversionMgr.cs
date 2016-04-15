using com.Sconit.Service.Ext.MasterData;
using System.Collections;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using com.Sconit.Persistence.MasterData;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.Utility;
using System.Linq;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class UomConversionMgr : UomConversionBaseMgr, IUomConversionMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }


        #region Customized Methods

        [Transaction(TransactionMode.Unspecified)]
        public decimal ConvertUomQty(string itemCode, Uom sourceUom, decimal sourceQty, Uom targetUom)
        {
            return ConvertUomQty(itemCode, sourceUom.Code, sourceQty, targetUom.Code);
        }

        //[Transaction(TransactionMode.Unspecified)]
        //public decimal ConvertUomQty(string itemCode, string sourceUomCode, decimal sourceQty, string targetUomCode)
        //{
        //    if (sourceUomCode == targetUomCode)
        //    {
        //        return sourceQty;
        //    }

        //    UomConversion uomConversion = this.LoadUomConversion(itemCode, sourceUomCode, targetUomCode);
        //    if (uomConversion != null)
        //    {
        //        return (sourceQty * uomConversion.BaseQty / uomConversion.AlterQty);
        //    }
        //    else
        //    {
        //        uomConversion = this.LoadUomConversion(itemCode, targetUomCode, sourceUomCode);
        //        if (uomConversion != null)
        //        {
        //            return (sourceQty * uomConversion.AlterQty / uomConversion.BaseQty);
        //        }
        //        else
        //        {
        //            uomConversion = this.LoadUomConversion(null, sourceUomCode, targetUomCode);
        //            if (uomConversion != null)
        //            {
        //                return (sourceQty * uomConversion.BaseQty / uomConversion.AlterQty);
        //            }
        //            else
        //            {
        //                uomConversion = this.LoadUomConversion(null, targetUomCode, sourceUomCode);
        //                if (uomConversion != null)
        //                {
        //                    return (sourceQty * uomConversion.AlterQty / uomConversion.BaseQty);
        //                }
        //                else
        //                {
        //                    throw new BusinessErrorException("UomConversion.Error.NotFound", itemCode, sourceUomCode, targetUomCode);
        //                }
        //            }

        //        }
        //    }
        //}

        [Transaction(TransactionMode.Unspecified)]
        public decimal ConvertUomQty(Item item, Uom sourceUom, decimal sourceQty, Uom targetUom)
        {
            return ConvertUomQty(item.Code, sourceUom, sourceQty, targetUom);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList GetUomConversion(string itemCode, string altUom, string baseUom)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(UomConversion));
            if (itemCode != string.Empty && itemCode != null)
                criteria.Add(Expression.Like("Item.Code", itemCode, MatchMode.Anywhere));
            if (altUom != string.Empty && altUom != null)
                criteria.Add(Expression.Like("AlterUom.Code", altUom, MatchMode.Anywhere));
            if (baseUom != string.Empty && baseUom != null)
                criteria.Add(Expression.Like("BaseUom.Code", baseUom, MatchMode.Anywhere));
            return criteriaMgrE.FindAll(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<UomConversion> GetUomConversion(string itemCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(UomConversion));
            criteria.Add(Expression.Eq("Item.Code", itemCode));
            return criteriaMgrE.FindAll<UomConversion>(criteria);

        }

        [Transaction(TransactionMode.Unspecified)]
        public override UomConversion LoadUomConversion(string itemCode, string alterUomCode, string baseUomCode)
        {
            if (itemCode != null && itemCode != string.Empty)
            {
                return base.LoadUomConversion(itemCode, alterUomCode, baseUomCode);
            }
            else
            {
                DetachedCriteria criteria = DetachedCriteria.For(typeof(UomConversion));
                criteria.Add(Expression.IsNull("Item"));
                if (alterUomCode != string.Empty && alterUomCode != null)
                    criteria.Add(Expression.Eq("AlterUom.Code", alterUomCode));
                if (baseUomCode != string.Empty && baseUomCode != null)
                    criteria.Add(Expression.Eq("BaseUom.Code", baseUomCode));
                IList<UomConversion> uomConversionList = criteriaMgrE.FindAll<UomConversion>(criteria);
                if (uomConversionList != null && uomConversionList.Count > 0)
                {
                    return uomConversionList[0];
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 仅支持无限级单位转换
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="sourceUomCode"></param>
        /// <param name="sourceQty"></param>
        /// <param name="targetUomCode"></param>
        /// <returns></returns>
        public decimal ConvertUomQty(string itemCode, string sourceUomCode, decimal sourceQty, string targetUomCode)
        {
            if (itemCode == null || sourceUomCode == null || targetUomCode == null)
            {
                throw new BusinessErrorException("UomConversion Error:itemCode Or sourceUomCode Or targetUomCode is null");
            }

            if (sourceUomCode == targetUomCode || sourceQty == 0)
            {
                return sourceQty;
            }

            DetachedCriteria criteria = DetachedCriteria.For(typeof(UomConversion));
            criteria.Add(Expression.Or(Expression.IsNull("Item"), Expression.Eq("Item.Code", itemCode)));

            IList<UomConversion> unGroupUomConversionList = criteriaMgrE.FindAll<UomConversion>(criteria);
            if (unGroupUomConversionList != null)
            {
                List<UomConversion> uomConversionList = unGroupUomConversionList.Where(u => u.Item != null).ToList();
                foreach (UomConversion y in unGroupUomConversionList)
                {
                    if (uomConversionList.Where(x => (StringHelper.Eq(x.AlterUom.Code, y.AlterUom.Code) && StringHelper.Eq(x.BaseUom.Code, y.BaseUom.Code))
                        || (StringHelper.Eq(x.AlterUom.Code, y.BaseUom.Code) && StringHelper.Eq(x.BaseUom.Code, y.AlterUom.Code))).Count() == 0)
                    {
                        uomConversionList.Add(y);
                    }
                }
                foreach (UomConversion u in uomConversionList)
                {
                    //顺
                    if (StringHelper.Eq(u.BaseUom.Code, sourceUomCode))
                    {
                        u.Qty = sourceQty * u.AlterQty / u.BaseQty;
                        u.IsAsc = true;
                        if (StringHelper.Eq(u.AlterUom.Code, targetUomCode))
                        {
                            return u.Qty.Value;
                        }
                    }
                    //反
                    else if (StringHelper.Eq(u.AlterUom.Code, sourceUomCode))
                    {
                        u.Qty = sourceQty * u.BaseQty / u.AlterQty;
                        u.IsAsc = false;
                        if (StringHelper.Eq(u.BaseUom.Code, targetUomCode))
                        {
                            return u.Qty.Value;
                        }
                    }
                }

                for (int i = 1; i < uomConversionList.Count; i++)
                {
                    foreach (UomConversion uomConversion1 in uomConversionList)
                    {
                        if (uomConversion1.Qty.HasValue)
                        {
                            foreach (UomConversion uomConversion2 in uomConversionList)
                            {
                                //顺
                                if (uomConversion1.IsAsc)
                                {
                                    //顺
                                    if (StringHelper.Eq(uomConversion2.BaseUom.Code, uomConversion1.AlterUom.Code) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.AlterQty / uomConversion2.BaseQty;
                                        uomConversion2.IsAsc = true;
                                        if (StringHelper.Eq(uomConversion2.AlterUom.Code, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                    //反
                                    else if (StringHelper.Eq(uomConversion2.AlterUom.Code, uomConversion1.AlterUom.Code) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.BaseQty / uomConversion2.AlterQty;
                                        uomConversion2.IsAsc = false;
                                        if (StringHelper.Eq(uomConversion2.BaseUom.Code, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                }
                                //反
                                else
                                {
                                    //顺
                                    if (StringHelper.Eq(uomConversion2.BaseUom.Code, uomConversion1.BaseUom.Code) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.AlterQty / uomConversion2.BaseQty;
                                        uomConversion2.IsAsc = true;
                                        if (StringHelper.Eq(uomConversion2.AlterUom.Code, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                    //反
                                    else if (StringHelper.Eq(uomConversion2.AlterUom.Code, uomConversion1.BaseUom.Code) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.BaseQty / uomConversion2.AlterQty;
                                        uomConversion2.IsAsc = false;
                                        if (StringHelper.Eq(uomConversion2.BaseUom.Code, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            throw new BusinessErrorException("UomConversion.Error.NotFound", itemCode, sourceUomCode, targetUomCode);
        }

        public decimal ConvertUomQtyInvToOrder(FlowDetail flowDetail, decimal invQty)
        {
            return this.ConvertUomQty(flowDetail.Item, flowDetail.Item.Uom, invQty, flowDetail.Uom);
        }

        #endregion Customized Methods
    }

    class UomConversionComparer : IEqualityComparer<UomConversion>
    {
        public bool Equals(UomConversion x, UomConversion y)
        {
            return (StringHelper.Eq(x.AlterUom.Code, y.AlterUom.Code) && StringHelper.Eq(x.BaseUom.Code, y.BaseUom.Code))
                || (StringHelper.Eq(x.AlterUom.Code, y.BaseUom.Code) && StringHelper.Eq(x.BaseUom.Code, y.AlterUom.Code));
        }

        public int GetHashCode(UomConversion obj)
        {
            string hCode = obj.AlterUom + "|" + obj.BaseUom;
            return hCode.GetHashCode();
        }
    }
}


#region Extend Class


namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class UomConversionMgrE : com.Sconit.Service.MasterData.Impl.UomConversionMgr, IUomConversionMgrE
    {

    }
}
#endregion
