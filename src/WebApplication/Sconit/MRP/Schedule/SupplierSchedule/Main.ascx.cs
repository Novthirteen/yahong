using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using com.Sconit.Web;
using NHibernate.Expression;
using com.Sconit.Entity.Batch;
using com.Sconit.Entity;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using System.Data.SqlClient;
using com.Sconit.Utility;
using System.Reflection;
using com.Sconit.Entity.MRP;
using NHibernate.SqlCommand;
using com.Sconit.Service.MasterData.Impl;

public partial class MRP_Schedule_SupplierSchedule_Main : MainModuleBase
{
    public bool isSupplier
    {
        get { return ViewState["isSupplier"] == null ? true : (bool)ViewState["isSupplier"]; }
        set { ViewState["isSupplier"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (isSupplier)
        {
            this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:true,bool:false,bool:false,bool:false,bool:false,bool:false,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_FROM;

            // TheFlowMgr.GetFlowList(
        }
        else
        {
            this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:false,bool:false,bool:false,bool:true,bool:false,bool:false,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_FROM;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            Flow flow = TheFlowMgr.CheckAndLoadFlow(this.tbFlow.Text.Trim());

            SecurityHelper.CheckPermission(flow.Type, flow.PartyFrom.Code, flow.PartyTo.Code, this.CurrentUser);

            if (this.tbFinanceYear.Text.Trim() == string.Empty)
            {
                ShowErrorMessage("Cost.FinanceCalendar.Year.Empty");
                return;
            }

            DateTime fy = DateTime.Parse(this.tbFinanceYear.Text);
            int year = fy.Year;
            int month = fy.Month;
            FinanceCalendar financeCalendar = TheFinanceCalendarMgr.GetFinanceCalendar(year, month);
            if (financeCalendar == null)
            {
                ShowErrorMessage("会计期间不存在");
                return;
            }


            #region BomTree
            string sql = @"select bomlevel as BomLevel, fg as Fg,fgdesc as FgDesc,item as Item,itemdesc as ItemDesc,
                     accumqty as AccumQty,uom as UomCode,FgUom
                     from bomtree where itemcategorycode like '1%' ";
            if (!isSupplier)
            {
                sql = @"select bomlevel as BomLevel, fg as Fg,fgdesc as FgDesc,item as Item,itemdesc as ItemDesc,
                     accumqty as AccumQty,uom as UomCode,FgUom
                     from bomtree where itemcategorycode like '2%' ";
            }
            SqlParameter[] sqlParam = new SqlParameter[1];
            DataSet ds = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);

            List<BomTree> bomTrees = IListHelper.DataTableToList<BomTree>(ds.Tables[0]);
            #endregion bomTree

            #region 查询并缓存所有FlowDetail
            DetachedCriteria criteria = DetachedCriteria.For<FlowDetail>();
            criteria.CreateAlias("Flow", "f");
            criteria.CreateAlias("Item", "i");
            criteria.CreateAlias("i.Uom", "iu");
            criteria.CreateAlias("Uom", "u");
            criteria.CreateAlias("i.Location", "il", JoinType.LeftOuterJoin);
            criteria.CreateAlias("i.Bom", "ib", JoinType.LeftOuterJoin);
            criteria.CreateAlias("i.Routing", "ir", JoinType.LeftOuterJoin);
            criteria.CreateAlias("LocationFrom", "lf", JoinType.LeftOuterJoin);
            criteria.CreateAlias("LocationTo", "lt", JoinType.LeftOuterJoin);
            criteria.CreateAlias("f.LocationFrom", "flf", JoinType.LeftOuterJoin);
            criteria.CreateAlias("f.LocationTo", "flt", JoinType.LeftOuterJoin);
            criteria.CreateAlias("Bom", "b", JoinType.LeftOuterJoin);
            criteria.CreateAlias("Routing", "r", JoinType.LeftOuterJoin);
            criteria.CreateAlias("f.Routing", "fr", JoinType.LeftOuterJoin);

            criteria.SetProjection(Projections.ProjectionList()
                .Add(Projections.GroupProperty("f.Code").As("Flow"))
                .Add(Projections.GroupProperty("f.Type").As("FlowType"))
                .Add(Projections.GroupProperty("i.Code").As("Item"))
                .Add(Projections.GroupProperty("lf.Code").As("LocationFrom"))
                .Add(Projections.GroupProperty("lt.Code").As("LocationTo"))
                .Add(Projections.GroupProperty("flf.Code").As("FlowLocationFrom"))
                .Add(Projections.GroupProperty("flt.Code").As("FlowLocationTo"))
                .Add(Projections.GroupProperty("MRPWeight").As("MRPWeight"))
                .Add(Projections.GroupProperty("b.Code").As("Bom"))
                .Add(Projections.GroupProperty("r.Code").As("Routing"))
                .Add(Projections.GroupProperty("fr.Code").As("FlowRouting"))
                .Add(Projections.GroupProperty("iu.Code").As("ItemUom"))
                .Add(Projections.GroupProperty("u.Code").As("Uom"))
                .Add(Projections.GroupProperty("f.LeadTime").As("LeadTime"))
                .Add(Projections.GroupProperty("ib.Code").As("ItemBom"))
                .Add(Projections.GroupProperty("ir.Code").As("ItemRouting"))
                .Add(Projections.GroupProperty("il.Code").As("ItemLocation"))
                .Add(Projections.GroupProperty("UnitCount").As("UnitCount"))
                .Add(Projections.GroupProperty("i.Desc1").As("ItemDesc1"))
                .Add(Projections.GroupProperty("i.Desc2").As("ItemDesc2"))
                .Add(Projections.GroupProperty("Id").As("Id"))
                );

            criteria.Add(Expression.Eq("f.IsActive", true));
            if (isSupplier)
            {
                criteria.Add(Expression.Eq("f.Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT));
            }
            else
            {
                criteria.Add(Expression.Eq("f.Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION));
            }
            criteria.Add(Expression.Gt("MRPWeight", 0));
            criteria.Add(Expression.Eq("f.IsMRP", true));
            criteria.Add(Expression.Eq("Flow", this.tbFlow.Text.Trim()));

            IList<object[]> flowDetailList = TheCriteriaMgr.FindAll<object[]>(criteria);

            var targetFlowDetailList = from fd in flowDetailList
                                       select new FlowDetailSnapShot
                                       {
                                           Flow = (string)fd[0],
                                           FlowType = (string)fd[1],
                                           Item = (string)fd[2],
                                           LocationFrom = fd[3] != null ? (string)fd[3] : fd[5] != null ? (string)fd[5] : (string)fd[16],
                                           LocationTo = fd[4] != null ? (string)fd[4] : (string)fd[6],
                                           MRPWeight = (int)fd[7],
                                           Bom = (string)fd[1] != BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION ? null : fd[8] != null ? (string)fd[8] : fd[14] != null ? (string)fd[14] : (string)fd[2],  //FlowDetail --> Item.Bom --> Item.Code
                                           Routing = (string)fd[1] != BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION ? null : fd[9] != null ? (string)fd[9] : fd[10] != null ? (string)fd[10] : fd[15] != null ? (string)fd[15] : null, //FlowDetail --> Flow --> Item.Routing
                                           BaseUom = (string)fd[11],
                                           Uom = (string)fd[12],
                                           LeadTime = fd[13] != null ? (decimal)fd[13] : 0,
                                           UnitCount = (decimal)fd[17],
                                           ItemDescription = ((fd[18] != null ? fd[18] : string.Empty) + ((fd[19] != null) ? "[" + fd[19] + "]" : string.Empty)),
                                           Id = (int)fd[20]
                                       };

            IList<FlowDetailSnapShot> flowDetailSnapShotList = new List<FlowDetailSnapShot>();
            if (targetFlowDetailList != null && targetFlowDetailList.Count() > 0)
            {
                flowDetailSnapShotList = targetFlowDetailList.ToList();
            }

            #region 处理引用
            if (flowDetailSnapShotList != null && flowDetailSnapShotList.Count > 0)
            {
                criteria = DetachedCriteria.For<Flow>();

                criteria.CreateAlias("LocationFrom", "flf", JoinType.LeftOuterJoin);
                criteria.CreateAlias("LocationTo", "flt", JoinType.LeftOuterJoin);
                criteria.CreateAlias("Routing", "fr", JoinType.LeftOuterJoin);

                criteria.SetProjection(Projections.ProjectionList()
                    .Add(Projections.GroupProperty("Code").As("Flow"))
                    .Add(Projections.GroupProperty("Type").As("FlowType"))
                    .Add(Projections.GroupProperty("ReferenceFlow").As("ReferenceFlow"))
                    .Add(Projections.GroupProperty("flf.Code").As("FlowLocationFrom"))
                    .Add(Projections.GroupProperty("flt.Code").As("FlowLocationTo"))
                    .Add(Projections.GroupProperty("fr.Code").As("FlowRouting"))
                    );

                criteria.Add(Expression.Eq("IsActive", true));
                criteria.Add(Expression.IsNotNull("ReferenceFlow"));
                criteria.Add(Expression.Eq("IsMRP", true));
                criteria.Add(Expression.Not(Expression.Eq("Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)));
                criteria.Add(Expression.Not(Expression.Eq("Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING)));

                IList<object[]> refFlowList = TheCriteriaMgr.FindAll<object[]>(criteria);

                if (refFlowList != null && refFlowList.Count > 0)
                {
                    foreach (object[] refFlow in refFlowList)
                    {
                        var refFlowDetailList = from fd in flowDetailSnapShotList
                                                where string.Compare(fd.Flow, (string)refFlow[2]) == 0
                                                select fd;

                        if (refFlowDetailList != null && refFlowDetailList.Count() > 0)
                        {
                            IListHelper.AddRange(flowDetailSnapShotList, (from fd in refFlowDetailList
                                                                          select new FlowDetailSnapShot
                                                                          {
                                                                              Flow = (string)refFlow[0],
                                                                              FlowType = (string)refFlow[1],
                                                                              Item = fd.Item,
                                                                              LocationFrom = (string)refFlow[3],
                                                                              LocationTo = (string)refFlow[4],
                                                                              MRPWeight = fd.MRPWeight,
                                                                              Bom = fd.Bom,
                                                                              Routing = (string)refFlow[5],
                                                                              BaseUom = fd.BaseUom,
                                                                              Uom = fd.Uom,
                                                                              LeadTime = fd.LeadTime,
                                                                              UnitCount = fd.UnitCount,
                                                                              ItemDescription = fd.ItemDescription
                                                                          }).ToList());
                        }
                    }
                }
            }
            #endregion

            #endregion

            #region CustScheduledet

            criteria = DetachedCriteria.For(typeof(CustomerSchedule));
            criteria.Add(Expression.Eq("FinanceCalendar", this.tbFinanceYear.Text));
            criteria.Add(Expression.Eq("Flow", "PANAC1"));//只是松下的需求，写死
            criteria.Add(Expression.Eq("Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT));
            criteria.AddOrder(Order.Desc("Id"));
            IList<CustomerSchedule> customerSchedules = TheCriteriaMgr.FindAll<CustomerSchedule>(criteria, 0, 1);

            if (customerSchedules == null || customerSchedules.Count == 0 || customerSchedules[0] == null)
            {
                ShowWarningMessage("没有需求");
                return;
            }
            else
            {
                sqlParam[0] = new SqlParameter("@p0", customerSchedules[0].Id);
            }

            sql = @"select Item,Uom,sum(qty) as Qty from CustScheduledet where scheduleid=@p0 group by Item,Uom";
            ds = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);
            List<Plan> plans = IListHelper.DataTableToList<Plan>(ds.Tables[0]);
            #endregion

            #region 需求展开
            foreach (Plan plan in plans)
            {
                if (!isSupplier)
                {
                    var q = flowDetailSnapShotList.Where(f => f.Item == plan.Item);
                    if (q != null && q.Count() > 0)
                    {
                        var qFirst = q.First();
                        //转成订单单位
                        qFirst.Qty += ConvertUomQty(qFirst.Item, plan.Uom, plan.Qty, qFirst.Uom);
                        if (q.Count() > 1)
                        {
                            //log warning
                        }
                    }
                    else
                    {
                        //log error
                    }
                }

                foreach (BomTree bomTree in bomTrees)
                {
                    if (plan.Item == bomTree.Fg)
                    {
                        //转成Bom单位 todo
                        decimal sourceQty = TheUomConversionMgr.ConvertUomQty(plan.Item, plan.Uom, plan.Qty, bomTree.FgUom);
                        var q = flowDetailSnapShotList.Where(f => f.Item == bomTree.Item);

                        if (q != null && q.Count() > 0)
                        {
                            var qFirst = q.First();
                            //转成订单单位
                            qFirst.Qty += ConvertUomQty(qFirst.Item, bomTree.UomCode, bomTree.AccumQty * sourceQty, qFirst.Uom);
                            if (q.Count() > 1)
                            {
                                //log warning
                            }
                        }
                        else
                        {
                            //log error
                        }
                    }
                }
            }

            //清空 SupplierSchedule
            //TheSqlHelperMgr.ExecuteSql(" truncate table MRP_SupplierSchedule", null);

            //sql = "insert MRP_SupplierSchedule values(@p0,@p1,@p2,@p3,@p4,@p5)";
            //SqlParameter[] sqlParam = new SqlParameter[13];

            flowDetailSnapShotList = flowDetailSnapShotList.Where(fd => fd.Qty > 0).ToList();

            DateTime startTime = TheCustomerScheduleDetailMgr.GetCustomerScheduleDetailDateTime(customerSchedules[0].Id, true).Value;
            DateTime endTime = TheCustomerScheduleDetailMgr.GetCustomerScheduleDetailDateTime(customerSchedules[0].Id, false).Value;

            foreach (FlowDetailSnapShot fd in flowDetailSnapShotList)
            {
                //DateTime dateTime = DateTime.Parse("2011-8-25");

                #region 已收
                criteria = DetachedCriteria.For(typeof(LocationTransaction));
                criteria.Add(Expression.Ge("EffectiveDate", startTime));//选定日期期末库存
                criteria.Add(Expression.Lt("EffectiveDate", endTime));//选定日期期末库存
                //criteria.Add(Expression.Eq("Location", fd.LocationTo));
                criteria.Add(Expression.Eq("Item", fd.Item));
                if (isSupplier)
                {
                    criteria.Add(Expression.Eq("TransactionType", BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_RCT_PO));
                }
                else
                {
                    criteria.Add(Expression.Eq("TransactionType", BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_RCT_WO));
                }
                criteria.SetProjection(Projections.Sum("Qty"));

                IList result = TheCriteriaMgr.FindAll(criteria);
                decimal receivedQty = 0;
                if (result != null && result.Count > 0 && result[0] != null)
                {
                    receivedQty = (decimal)result[0];
                }
                #endregion

                fd.ReceivedQty = receivedQty;

                //sqlParam[0] = new SqlParameter("@p0", fd.Flow);
                //sqlParam[1] = new SqlParameter("@p1", fd.Item);
                //sqlParam[2] = new SqlParameter("@p2", fd.ItemDescription);
                //sqlParam[3] = new SqlParameter("@p3", fd.Uom);
                //sqlParam[4] = new SqlParameter("@p4", fd.Qty);
                //sqlParam[5] = new SqlParameter("@p5", receivedQty);

                //TheSqlHelperMgr.Create(sql, sqlParam);
            }
            this.GV_List.DataSource = flowDetailSnapShotList;
            this.GV_List.DataBind();
            #endregion
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }

    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FlowDetailSnapShot body = (FlowDetailSnapShot)(e.Row.DataItem);
            e.Row.Cells[4].Text = this.TheItemReferenceMgr.GetItemReferenceByItem(body.Item, null, null);
        }
    }

    /// <summary>
    /// 支持无限级单位转换
    /// </summary>
    private decimal ConvertUomQty(string itemCode, string sourceUomCode, decimal sourceQty, string targetUomCode)
    {
        if (itemCode == null || sourceUomCode == null || targetUomCode == null)
        {
            //throw new BusinessErrorException("UomConversion Error:itemCode Or sourceUomCode Or targetUomCode is null");
            //log.Error("UomConversion Error:itemCode Or sourceUomCode Or targetUomCode is null");
            return sourceQty;
        }

        if (sourceUomCode == targetUomCode || sourceQty == 0)
        {
            return sourceQty;
        }

        DetachedCriteria criteria = DetachedCriteria.For(typeof(UomConversion));
        criteria.Add(Expression.Or(Expression.IsNull("Item"), Expression.Eq("Item.Code", itemCode)));

        IList<UomConversion> unGroupUomConversionList = TheCriteriaMgr.FindAll<UomConversion>(criteria);
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
        //throw new BusinessErrorException("UomConversion.Error.NotFound", itemCode, sourceUomCode, targetUomCode);
        //log.Error("UomConversion.Error.NotFound,itemCode:" + itemCode + ",sourceUomCode:" + sourceUomCode + ",targetUomCode:" + targetUomCode);
        return sourceQty;
    }

    class Plan
    {
        public string Item { get; set; }
        public string Uom { get; set; }
        public decimal Qty { get; set; }
    }

    class BomTree
    {
        public int BomLevel { get; set; }
        public string Fg { get; set; }
        public string FgDesc { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public decimal AccumQty { get; set; }
        public string UomCode { get; set; }
        public string FgUom { get; set; }
    }

    class FlowDetailSnapShot
    {
        public string Flow { get; set; }
        public string FlowType { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public string Item { get; set; }
        public int MRPWeight { get; set; }
        public string Bom { get; set; }
        public string Routing { get; set; }
        public string BaseUom { get; set; }
        public string Uom { get; set; }
        public decimal LeadTime { get; set; }
        public decimal UnitCount { get; set; }
        public string ItemDescription { get; set; }
        public int Id { get; set; }
        public decimal Qty { get; set; }
        public decimal ReceivedQty { get; set; }
    }
}
