﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Expression;
using com.Sconit.Entity.View;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;

namespace com.Sconit.Utility
{
    /// <summary>
    /// 封装常用查询条件
    /// </summary>
    public class CriteriaHelper
    {
        #region Criteria: Like
        #region Item
        public static void SetItemCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetItemCriteria(criteria, propertyName, criteriaParam.Item);
        }


        public static void SetItemCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam, MatchMode matchMode)
        {
            SetItemCriteria(criteria, propertyName, criteriaParam.Item, matchMode);
        }
        public static void SetItemCriteria(DetachedCriteria criteria, string propertyName, string item)
        {
            SetItemCriteria(criteria, propertyName, item, MatchMode.Start);//default
        }
        public static void SetItemCriteria(DetachedCriteria criteria, string propertyName, string item, MatchMode matchMode)
        {
            if (item != null && item.Trim() != string.Empty)
            {
                criteria.Add(Expression.Like(propertyName, item, matchMode));
            }
        }
        #endregion

        #region ItemDesc
        public static void SetItemDescCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetItemDescCriteria(criteria, propertyName, criteriaParam.ItemDesc);
        }
        public static void SetItemDescCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam, MatchMode matchMode)
        {
            SetItemDescCriteria(criteria, propertyName, criteriaParam.ItemDesc, matchMode);
        }
        public static void SetItemDescCriteria(DetachedCriteria criteria, string propertyName, string itemDesc)
        {
            SetItemDescCriteria(criteria, propertyName, itemDesc, MatchMode.Anywhere);//default
        }
        public static void SetItemDescCriteria(DetachedCriteria criteria, string propertyName, string itemDesc, MatchMode matchMode)
        {
            if (itemDesc != null && itemDesc.Trim() != string.Empty)
            {
                criteria.Add(Expression.Like(propertyName, itemDesc, matchMode));
            }
        }
        #endregion

        #region LotNo
        public static void SetLotNoCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetItemCriteria(criteria, propertyName, criteriaParam.LotNo);
        }
        public static void SetLotNoCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam, MatchMode matchMode)
        {
            SetItemCriteria(criteria, propertyName, criteriaParam.LotNo, matchMode);
        }
        public static void SetLotNoCriteria(DetachedCriteria criteria, string propertyName, string lotNo)
        {
            SetItemCriteria(criteria, propertyName, lotNo, MatchMode.Anywhere);//default
        }
        public static void SetLotNoCriteria(DetachedCriteria criteria, string propertyName, string lotNo, MatchMode matchMode)
        {
            if (lotNo != null && lotNo.Trim() != string.Empty)
            {
                criteria.Add(Expression.Like(propertyName, lotNo, matchMode));
            }
        }
        #endregion


        #region TransactionType
        public static void SetTransactionTypeCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetTransactionTypeCriteria(criteria, propertyName, criteriaParam.TransactionType);
        }
        public static void SetTransactionTypeCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam, MatchMode matchMode)
        {
            SetTransactionTypeCriteria(criteria, propertyName, criteriaParam.TransactionType, matchMode);
        }
        public static void SetTransactionTypeCriteria(DetachedCriteria criteria, string propertyName, string transactionType)
        {
            SetTransactionTypeCriteria(criteria, propertyName, transactionType, MatchMode.Start);//default
        }
        public static void SetTransactionTypeCriteria(DetachedCriteria criteria, string propertyName, string transactionType, MatchMode matchMode)
        {
            if (transactionType != null && transactionType.Trim() != string.Empty)
            {
                criteria.Add(Expression.Like(propertyName, transactionType, matchMode));
            }
        }
        #endregion

        #region OrderNo
        public static void SetOrderNoCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam, MatchMode matchMode)
        {
            SetOrderNoCriteria(criteria, propertyName, criteriaParam.OrderNo, matchMode);
        }
        public static void SetOrderNoCriteria(DetachedCriteria criteria, string propertyName, string orderNo, MatchMode matchMode)
        {
            if (orderNo != null && orderNo.Trim() != string.Empty)
            {
                criteria.Add(Expression.Like(propertyName, orderNo, matchMode));
            }
        }
        #endregion

        #endregion

        #region Criteria: In
        public static void SetInCriteria<T>(DetachedCriteria criteria, string propertyName, List<T> list)
        {
            if (list != null && list.Count > 0)
            {
                if (list.Count == 1)
                {
                    criteria.Add(Expression.Eq(propertyName, list[0]));
                }
                else
                {
                    criteria.Add(Expression.InG<T>(propertyName, list));
                }
            }
        }

        public static void SetPartyCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetPartyCriteria(criteria, propertyName, criteriaParam.Party);
        }
        public static void SetPartyCriteria(DetachedCriteria criteria, string propertyName, string[] party)
        {
            if (party != null && party.Length > 0)
            {
                if (party.Length == 1)
                {
                    criteria.Add(Expression.Eq(propertyName, party[0]));
                }
                else
                {
                    criteria.Add(Expression.In(propertyName, party));
                }
            }
        }

        public static void SetFlowCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetFlowCriteria(criteria, propertyName, criteriaParam.Flow);
        }
        public static void SetFlowCriteria(DetachedCriteria criteria, string propertyName, string[] flow)
        {
            if (flow != null && flow.Length > 0)
            {
                if (flow.Length == 1)
                {
                    criteria.Add(Expression.Eq(propertyName, flow[0]));
                }
                else
                {
                    criteria.Add(Expression.In(propertyName, flow));
                }
            }
        }

        public static void SetLocationCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetLocationCriteria(criteria, propertyName, criteriaParam.Location);
        }
        public static void SetLocationCriteria(DetachedCriteria criteria, string propertyName, string[] location)
        {
            if (location != null && location.Length > 0)
            {
                if (location.Length == 1)
                {
                    criteria.Add(Expression.Eq(propertyName, location[0]));
                }
                else
                {
                    criteria.Add(Expression.In(propertyName, location));
                }
            }
        }
        #endregion

        public static void SetStartDateCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetStartDateCriteria(criteria, propertyName, criteriaParam.StartDate);
        }
        public static void SetStartDateCriteria(DetachedCriteria criteria, string propertyName, DateTime? startDate)
        {
            if (startDate.HasValue)
            {
                criteria.Add(Expression.Ge(propertyName, startDate.Value));
            }
        }

        public static void SetEndDateCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetEndDateCriteria(criteria, propertyName, criteriaParam.EndDate);
        }
        public static void SetEndDateCriteria(DetachedCriteria criteria, string propertyName, DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                criteria.Add(Expression.Lt(propertyName, endDate.Value.AddDays(1)));
            }
        }

        public static void SetEndTimeCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            if (criteriaParam.EndDate.HasValue)
            {
                criteria.Add(Expression.Lt(propertyName, criteriaParam.EndDate.Value));
            }
        }
        
        public static void SetShiftCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetShiftCriteria(criteria, propertyName, criteriaParam.Shift);
        }
        public static void SetShiftCriteria(DetachedCriteria criteria, string propertyName, string shift)
        {
            if (shift != null && shift.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq(propertyName, shift));
            }
        }

        public static void SetStorageBinCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetStorageBinCriteria(criteria, propertyName, criteriaParam.BinCode);
        }
        public static void SetStorageBinCriteria(DetachedCriteria criteria, string propertyName, string binCode)
        {
            if (binCode != null && binCode.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq(propertyName, binCode));
            }
        }

        public static void SetItemCategoryCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetItemCategoryCriteria(criteria, propertyName, criteriaParam.ItemCategory);
        }
        public static void SetItemCategoryCriteria(DetachedCriteria criteria, string propertyName, string itemCategory)
        {
            if (itemCategory != null && itemCategory.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq(propertyName, itemCategory));
            }
        }

        public static void SetCostGroupCriteria(DetachedCriteria criteria, string propertyName, CriteriaParam criteriaParam)
        {
            SetCostGroupCriteria(criteria, propertyName, criteriaParam.CostGroup);
        }
        public static void SetCostGroupCriteria(DetachedCriteria criteria, string propertyName, string costGroup)
        {
            if (costGroup != null && costGroup.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq(propertyName, costGroup));
            }
        }

        public static object[] CollectMasterParam(IDictionary<string, string> dic, List<string> statusList, List<string> typeList, bool newItem)
        {
            string orderNo = (dic == null || !dic.ContainsKey("OrderNo")) ? null : dic["OrderNo"];
            string refOrderNo = (dic == null || !dic.ContainsKey("RefOrderNo")) ? null : dic["RefOrderNo"];
            string extOrderNo = (dic == null || !dic.ContainsKey("ExtOrderNo")) ? null : dic["ExtOrderNo"];
            string flow = (dic == null || !dic.ContainsKey("Flow")) ? null : dic["Flow"];
            string partyFrom = (dic == null || !dic.ContainsKey("PartyFrom")) ? null : dic["PartyFrom"];
            string partyTo = (dic == null || !dic.ContainsKey("PartyTo")) ? null : dic["PartyTo"];
            string moduleType = (dic == null || !dic.ContainsKey("ModuleType")) ? null : dic["ModuleType"];
            string locationFrom = (dic == null || !dic.ContainsKey("LocationFrom")) ? null : dic["LocationFrom"];
            string locationTo = (dic == null || !dic.ContainsKey("LocationTo")) ? null : dic["LocationTo"];
            string moduleSubType = (dic == null || !dic.ContainsKey("ModuleSubType")) ? null : dic["ModuleSubType"];
            string priority = (dic == null || !dic.ContainsKey("Priority")) ? null : dic["Priority"];
            string createUser = (dic == null || !dic.ContainsKey("CreateUser")) ? null : dic["CreateUser"];
            string startDate = (dic == null || !dic.ContainsKey("StartDate")) ? null : dic["StartDate"];
            string endDate = (dic == null || !dic.ContainsKey("EndDate")) ? null : dic["EndDate"];
            string currentUser = (dic == null || !dic.ContainsKey("CurrentUser")) ? null : dic["CurrentUser"];

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(OrderHead));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(OrderHead))
                .SetProjection(Projections.Count("OrderNo"));
            IDictionary<string, string> alias = new Dictionary<string, string>();
            selectCriteria.CreateAlias("PartyFrom", "pf");
            selectCriteria.CreateAlias("PartyTo", "pt");
            selectCountCriteria.CreateAlias("PartyFrom", "pf");
            selectCountCriteria.CreateAlias("PartyTo", "pt");
            alias.Add("PartyFrom", "pf");
            alias.Add("PartyTo", "pt");

            #region 设置订单Type查询条件
            selectCriteria.Add(Expression.In("Type", typeList));
            selectCountCriteria.Add(Expression.In("Type", typeList));
            #endregion

            #region 设置订单SubType查询条件
            if (moduleSubType != null && moduleSubType != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("SubType", moduleSubType));
                selectCountCriteria.Add(Expression.Eq("SubType", moduleSubType));
            }
            #endregion

            selectCriteria.Add(Expression.Eq("IsNewItem", newItem));
            selectCountCriteria.Add(Expression.Eq("IsNewItem", newItem));

            if (orderNo != null && orderNo != string.Empty)
            {
                selectCriteria.Add(Expression.Like("OrderNo", orderNo, MatchMode.End));
                selectCountCriteria.Add(Expression.Like("OrderNo", orderNo, MatchMode.End));
            }
            if (extOrderNo != null && extOrderNo != string.Empty)
            {
                selectCriteria.Add(Expression.Like("ExternalOrderNo", extOrderNo, MatchMode.End));
                selectCountCriteria.Add(Expression.Like("ExternalOrderNo", extOrderNo, MatchMode.End));
            }
            if (refOrderNo != null && refOrderNo != string.Empty)
            {
                selectCriteria.Add(Expression.Like("ReferenceOrderNo", refOrderNo, MatchMode.End));
                selectCountCriteria.Add(Expression.Like("ReferenceOrderNo", refOrderNo, MatchMode.End));
            }

            if (priority != null && priority != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Priority", priority));
                selectCountCriteria.Add(Expression.Eq("Priority", priority));
            }

            #region partyFrom
            if (partyFrom != null && partyFrom != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("pf.Code", partyFrom));
                selectCountCriteria.Add(Expression.Eq("pf.Code", partyFrom));
            }
            //else if (moduleType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
            //{
            //    #region partyFrom
            //    SecurityHelper.SetPartyFromSearchCriteria(
            //        selectCriteria, selectCountCriteria, partyFrom, moduleType, currentUser);
            //    #endregion
            //}
            #endregion

            #region partyTo
            if (partyTo != null && partyTo != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("pt.Code", partyTo));
                selectCountCriteria.Add(Expression.Eq("pt.Code", partyTo));
            }
            //else if (moduleType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
            //{
            //    #region partyTo
            //    SecurityHelper.SetPartyToSearchCriteria(
            //        selectCriteria, selectCountCriteria, partyTo, moduleType, currentUser);
            //    #endregion
            //}
            #endregion

            #region 权限验证
            SecurityHelper.SetPartySearchCriteriaMstr(selectCriteria, selectCountCriteria, moduleType, currentUser);
            #endregion

            if (locationFrom != null && locationFrom != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("LocationFrom.Code", locationFrom));
                selectCountCriteria.Add(Expression.Eq("LocationFrom.Code", locationFrom));
            }

            if (locationTo != null && locationTo != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("LocationTo.Code", locationTo));
                selectCountCriteria.Add(Expression.Eq("LocationTo.Code", locationTo));
            }

            if (flow != null && flow != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Flow", flow));
                selectCountCriteria.Add(Expression.Eq("Flow", flow));
            }

            if (createUser != null && createUser != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("CreateUser.Code", createUser));
                selectCountCriteria.Add(Expression.Eq("CreateUser.Code", createUser));
            }

            #region date
            if (startDate != null && startDate != string.Empty)
            {
                selectCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(startDate)));
                selectCountCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(startDate)));
            }
            if (endDate != null && endDate != string.Empty)
            {
                selectCriteria.Add(Expression.Lt("CreateDate", DateTime.Parse(endDate).AddDays(1)));
                selectCountCriteria.Add(Expression.Lt("CreateDate", DateTime.Parse(endDate).AddDays(1)));
            }
            #endregion

            #region status
            if (statusList != null && statusList.Count > 0)
            {
                selectCriteria.Add(Expression.In("Status", statusList));
                selectCountCriteria.Add(Expression.In("Status", statusList));
            }
            #endregion
            return new object[] { selectCriteria, selectCountCriteria, alias, true };
        }

        public static object[] CollectDetailParam(IDictionary<string, string> dic, List<string> statusList, List<string> typeList, bool newItem)
        {
            string orderNo = (dic == null || !dic.ContainsKey("OrderNo")) ? null : dic["OrderNo"];
            string refOrderNo = (dic == null || !dic.ContainsKey("RefOrderNo")) ? null : dic["RefOrderNo"];
            string extOrderNo = (dic == null || !dic.ContainsKey("ExtOrderNo")) ? null : dic["ExtOrderNo"];
            string flow = (dic == null || !dic.ContainsKey("Flow")) ? null : dic["Flow"];
            string partyFrom = (dic == null || !dic.ContainsKey("PartyFrom")) ? null : dic["PartyFrom"];
            string partyTo = (dic == null || !dic.ContainsKey("PartyTo")) ? null : dic["PartyTo"];
            string moduleType = (dic == null || !dic.ContainsKey("ModuleType")) ? null : dic["ModuleType"];
            string locationFrom = (dic == null || !dic.ContainsKey("LocationFrom")) ? null : dic["LocationFrom"];
            string locationTo = (dic == null || !dic.ContainsKey("LocationTo")) ? null : dic["LocationTo"];
            string moduleSubType = (dic == null || !dic.ContainsKey("ModuleSubType")) ? null : dic["ModuleSubType"];
            string priority = (dic == null || !dic.ContainsKey("Priority")) ? null : dic["Priority"];
            string createUser = (dic == null || !dic.ContainsKey("CreateUser")) ? null : dic["CreateUser"];
            string startDate = (dic == null || !dic.ContainsKey("StartDate")) ? null : dic["StartDate"];
            string endDate = (dic == null || !dic.ContainsKey("EndDate")) ? null : dic["EndDate"];
            string currentUser = (dic == null || !dic.ContainsKey("CurrentUser")) ? null : dic["CurrentUser"];
            string item = (dic == null || !dic.ContainsKey("Item")) ? null : dic["Item"];

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(OrderDetail));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(OrderDetail))
                .SetProjection(Projections.Count("Id"));
            IDictionary<string, string> alias = new Dictionary<string, string>();
            selectCriteria.CreateAlias("OrderHead", "o");
            selectCriteria.CreateAlias("o.PartyFrom", "pf");
            selectCriteria.CreateAlias("o.PartyTo", "pt");
            selectCountCriteria.CreateAlias("OrderHead", "o");
            selectCountCriteria.CreateAlias("o.PartyFrom", "pf");
            selectCountCriteria.CreateAlias("o.PartyTo", "pt");
            alias.Add("PartyFrom", "pf");
            alias.Add("PartyTo", "pt");
            alias.Add("OrderHead", "o");

            #region 设置订单Type查询条件
            selectCriteria.Add(Expression.In("o.Type", typeList));
            selectCountCriteria.Add(Expression.In("o.Type", typeList));
            #endregion

            #region 设置订单SubType查询条件
            if (moduleSubType != null && moduleSubType != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("o.SubType", moduleSubType));
                selectCountCriteria.Add(Expression.Eq("o.SubType", moduleSubType));
            }
            #endregion

            selectCriteria.Add(Expression.Eq("o.IsNewItem", newItem));
            selectCountCriteria.Add(Expression.Eq("o.IsNewItem", newItem));

            if (orderNo != null && orderNo != string.Empty)
            {
                selectCriteria.Add(Expression.Like("o.OrderNo", orderNo, MatchMode.End));
                selectCountCriteria.Add(Expression.Like("o.OrderNo", orderNo, MatchMode.End));
            }
            if (extOrderNo != null && extOrderNo != string.Empty)
            {
                selectCriteria.Add(Expression.Like("o.ExternalOrderNo", extOrderNo, MatchMode.End));
                selectCountCriteria.Add(Expression.Like("o.ExternalOrderNo", extOrderNo, MatchMode.End));
            }
            if (refOrderNo != null && refOrderNo != string.Empty)
            {
                selectCriteria.Add(Expression.Like("o.ReferenceOrderNo", refOrderNo, MatchMode.End));
                selectCountCriteria.Add(Expression.Like("o.ReferenceOrderNo", refOrderNo, MatchMode.End));
            }

            if (priority != null && priority != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("o.Priority", priority));
                selectCountCriteria.Add(Expression.Eq("o.Priority", priority));
            }
            if (item != null && item != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Item.Code", item));
                selectCountCriteria.Add(Expression.Eq("Item.Code", item));
            }
            #region partyFrom
            if (partyFrom != null && partyFrom != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("pf.Code", partyFrom));
                selectCountCriteria.Add(Expression.Eq("pf.Code", partyFrom));
            }
            //else if (moduleType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
            //{
            //    #region partyFrom
            //    SecurityHelper.SetPartyFromSearchCriteria(
            //        selectCriteria, selectCountCriteria, partyFrom, moduleType, currentUser);
            //    #endregion
            //}
            #endregion

            #region partyTo
            if (partyTo != null && partyTo != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("pt.Code", partyTo));
                selectCountCriteria.Add(Expression.Eq("pt.Code", partyTo));
            }
            //else if (moduleType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
            //{
            //    #region partyTo
            //    SecurityHelper.SetPartyToSearchCriteria(
            //        selectCriteria, selectCountCriteria, partyTo, moduleType, currentUser);
            //    #endregion
            //}
            #endregion

            #region 权限验证
            SecurityHelper.SetPartySearchCriteriaDet(selectCriteria, selectCountCriteria, moduleType, currentUser);
            #endregion

            if (locationFrom != null && locationFrom != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("o.LocationFrom.Code", locationFrom));
                selectCountCriteria.Add(Expression.Eq("o.LocationFrom.Code", locationFrom));
            }

            if (locationTo != null && locationTo != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("o.LocationTo.Code", locationTo));
                selectCountCriteria.Add(Expression.Eq("o.LocationTo.Code", locationTo));
            }

            if (flow != null && flow != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("o.Flow", flow));
                selectCountCriteria.Add(Expression.Eq("o.Flow", flow));
            }

            if (createUser != null && createUser != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("o.CreateUser.Code", createUser));
                selectCountCriteria.Add(Expression.Eq("o.CreateUser.Code", createUser));
            }

            #region date
            if (startDate != null && startDate != string.Empty)
            {
                selectCriteria.Add(Expression.Ge("o.CreateDate", DateTime.Parse(startDate)));
                selectCountCriteria.Add(Expression.Ge("o.CreateDate", DateTime.Parse(startDate)));
            }
            if (endDate != null && endDate != string.Empty)
            {
                selectCriteria.Add(Expression.Lt("o.CreateDate", DateTime.Parse(endDate).AddDays(1)));
                selectCountCriteria.Add(Expression.Lt("o.CreateDate", DateTime.Parse(endDate).AddDays(1)));
            }
            #endregion

            #region status
            if (statusList != null && statusList.Count > 0)
            {
                selectCriteria.Add(Expression.In("o.Status", statusList));
                selectCountCriteria.Add(Expression.In("o.Status", statusList));
            }
            #endregion
            return new object[] { selectCriteria, selectCountCriteria, alias, false };
        }
    }
}
