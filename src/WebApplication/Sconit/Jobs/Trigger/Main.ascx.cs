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


public partial class Jobs_Trigger_Main : MainModuleBase
{
    public int CurrentIndex
    {
        get
        {
            return (int)ViewState["CurrentIndex"];
        }
        set
        {
            ViewState["CurrentIndex"] = value;
        }
    }

    public string CurrentJob
    {
        get
        {
            return (string)ViewState["CurrentJob"];
        }
        set
        {
            ViewState["CurrentJob"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void GV_List_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //停止
        if (e.CommandName.Equals("StopTrigger"))
        {
            int id = int.Parse(e.CommandArgument.ToString());
            BatchTrigger batchTrigger = TheBatchTriggerMgr.LoadBatchTrigger(id);
            batchTrigger.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_PAUSE;
            TheBatchTriggerMgr.UpdateBatchTrigger(batchTrigger);
            ShowSuccessMessage("MasterData.Jobs.Trigger.StopSuccessfully", batchTrigger.BatchJobDetail.Name);
            this.DataBind();
        }
        //启动
        if (e.CommandName.Equals("StartTrigger"))
        {
            int id = int.Parse(e.CommandArgument.ToString());
            BatchTrigger batchTrigger = TheBatchTriggerMgr.LoadBatchTrigger(id);
            batchTrigger.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS;
            //int minuteOdd = DateTime.Now.Minute % batchTrigger.Interval;
            //string newDate = DateTime.Now.AddMinutes(batchTrigger.Interval - minuteOdd).ToString("yyyy-MM-dd hh:mm");
            //batchTrigger.NextFireTime = DateTime.Parse(newDate);

            TheBatchTriggerMgr.UpdateBatchTrigger(batchTrigger);
            ShowSuccessMessage("MasterData.Jobs.Trigger.StartSuccessfully", batchTrigger.BatchJobDetail.Name);
            this.DataBind();
        }
        //查看日志
        if (e.CommandName.Equals("ViewLog"))
        {
            int id = int.Parse(e.CommandArgument.ToString());
            this.ucLog.Visible = true;
            this.ucLog.InitPageParameter(id);
        }
    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            BatchTrigger batchTrigger = (BatchTrigger)e.Row.DataItem;
            // ((Label)e.Row.FindControl("lblName")).Text = batchTrigger.BatchJobDetail.Name;
            // ((Label)e.Row.FindControl("lblDescription")).Text = batchTrigger.BatchJobDetail.Description;

            if (batchTrigger.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_PAUSE)
            {

                e.Row.FindControl("lbtnStop").Visible = false;
                e.Row.FindControl("lbtnStart").Visible = true;

            }
            if (batchTrigger.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS)
            {

                e.Row.FindControl("lbtnStop").Visible = true;
                e.Row.FindControl("lbtnStart").Visible = false;

            }
        }
    }



    protected void ODS_GV_BatchTrigger_OnUpdating(object source, ObjectDataSourceMethodEventArgs e)
    {

        BatchTrigger batchTrigger = (BatchTrigger)e.InputParameters[0];
        BatchTrigger oldTrigger = TheBatchTriggerMgr.LoadBatchTrigger(batchTrigger.Id);
        batchTrigger.BatchJobDetail = oldTrigger.BatchJobDetail;
        batchTrigger.Name = oldTrigger.Name;
        batchTrigger.Description = oldTrigger.Description;

        GridViewRow row = this.GV_List.Rows[this.CurrentIndex];
        com.Sconit.Control.CodeMstrDropDownList ddlIntervalType = (com.Sconit.Control.CodeMstrDropDownList)row.FindControl("ddlIntervalType");
        if (ddlIntervalType.SelectedIndex != -1)
        {
            batchTrigger.IntervalType = ddlIntervalType.SelectedValue;
        }
        CurrentJob = oldTrigger.BatchJobDetail.Name;
    }

    protected void ODS_GV_BatchTrigger_OnUpdated(object sender, ObjectDataSourceStatusEventArgs e)
    {

        ShowSuccessMessage("MasterData.Jobs.Trigger.UpdateSuccessfully", CurrentJob);
    }

    protected void GV_BatchTrigger_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        this.CurrentIndex = e.RowIndex;
    }

    protected void GV_BatchTrigger_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
        {
            com.Sconit.Control.CodeMstrDropDownList ddlIntervalType = (com.Sconit.Control.CodeMstrDropDownList)e.Row.FindControl("ddlIntervalType");
            if (e.Row.DataItem != null)
            {
                BatchTrigger batchTrigger = (BatchTrigger)e.Row.DataItem;
                ddlIntervalType.DataBind();
                ddlIntervalType.DefaultSelectedValue = batchTrigger.IntervalType;

            }
        }
    }

    protected void GV_BatchTrigger_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridViewRow row = this.GV_List.Rows[e.NewEditIndex];
        HiddenField hfId = (HiddenField)row.FindControl("hfId");
        BatchTrigger batchTrigger = TheBatchTriggerMgr.LoadBatchTrigger(int.Parse(hfId.Value));
        if (batchTrigger.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS)
        {
            ShowErrorMessage("MasterData.Jobs.Trigger.StopFirst");
            e.Cancel = true;
        }
    }

    protected void yuejie_onclick(object sender, EventArgs e)
    {
        TheCostMgr.CloseFinanceMonth(this.CurrentUser);
    }


    protected void btnTest_Click(object sender, EventArgs e)
    {
        TheSqlHelperMgr.ExecuteSql(" truncate table bomtree", null);

        string sql = "insert bomtree values(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13)";
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(Item));
        selectCriteria.Add(Expression.Like("ItemCategory.Code", "3", MatchMode.Start));
        IList<Item> items = TheCriteriaMgr.FindAll<Item>(selectCriteria);

        SqlParameter[] sqlParam = new SqlParameter[14];

        //200002040 test

        foreach (Item item in items)
        {
            IList<BomDetail> newBomDetails = TheBomDetailMgr.GetTreeBomDetail(item.Code, DateTime.Now);
            if (newBomDetails != null)
            {
                string fgUom = newBomDetails.First(b => b.Bom.Code == item.Code).Bom.Uom.Code;

                foreach (BomDetail bomDetail in newBomDetails)
                {
                    sqlParam[0] = new SqlParameter("@p0", bomDetail.Bom.Code);
                    sqlParam[1] = new SqlParameter("@p1", bomDetail.Bom.Description);
                    sqlParam[2] = new SqlParameter("@p2", bomDetail.Item.Code);
                    sqlParam[3] = new SqlParameter("@p3", bomDetail.Item.Description);
                    sqlParam[4] = new SqlParameter("@p4", bomDetail.RateQty);
                    sqlParam[5] = new SqlParameter("@p5", bomDetail.Uom.Code);
                    sqlParam[6] = new SqlParameter("@p6", bomDetail.BomLevel);
                    sqlParam[7] = new SqlParameter("@p7", bomDetail.Item.ItemCategory.Code);
                    sqlParam[8] = new SqlParameter("@p8", bomDetail.Item.ItemCategory.Description);
                    sqlParam[9] = new SqlParameter("@p9", bomDetail.AccumQty);
                    sqlParam[10] = new SqlParameter("@p10", DateTime.Now);
                    sqlParam[11] = new SqlParameter("@p11", item.Code);
                    sqlParam[12] = new SqlParameter("@p12", item.Description);
                    sqlParam[13] = new SqlParameter("@p13", fgUom);
                    TheSqlHelperMgr.Create(sql, sqlParam);
                }
            }
        }
    }


    protected void btnTest1_Click(object sender, EventArgs e)
    {
        #region BomTree
        string sql = @"select bomlevel as BomLevel, fg as Fg,fgdesc as FgDesc,item as Item,itemdesc as ItemDesc,
                     accumqty as AccumQty,uomcode as UomCode
                     from bomtree where itemcategorycode like '1%' ";
        SqlParameter[] sqlParam = new SqlParameter[6];
        DataSet ds = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);

        List<BomTree> bomTrees = IListHelper.DataTableToList<BomTree>(ds.Tables[0]);
        #endregion bomTree

        #region 历史库存
        string hql = @"select l.Code, i.Code, sum(lld.Qty) from LocationLotDetail as lld
                    join lld.Location as l
                    join lld.Item as i
                    where not lld.Qty = 0 and l.Type = ?
                    group by l.Code, i.Code";
        IList<object[]> invList = TheHqlMgr.FindAll<object[]>(hql, BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL);
        IList<MrpLocationLotDetail> inventoryBalanceList = new List<MrpLocationLotDetail>();
        if (invList != null && invList.Count > 0)
        {
            inventoryBalanceList = (from inv in invList
                                    select new MrpLocationLotDetail
                                        {
                                            Location = (string)inv[0],
                                            Item = (string)inv[1],
                                            Qty = (decimal)inv[2],
                                            SafeQty = 0
                                        }).ToList();
        }
        #endregion

        #region 订单待收 TODO
        #endregion

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
        criteria.Add(Expression.Eq("f.Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT));
        criteria.Add(Expression.Gt("MRPWeight", 0));
        criteria.Add(Expression.Eq("f.IsMRP", true));

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
        sql = @"select Item,Uom,sum(qty) as Qty from CustScheduledet where scheduleid=213 group by Item,Uom";
        ds = TheSqlHelperMgr.GetDatasetBySql(sql, null);
        List<Plan> plans = IListHelper.DataTableToList<Plan>(ds.Tables[0]);
        #endregion

        #region 需求展开
        foreach (Plan plan in plans)
        {
            foreach (BomTree bomTree in bomTrees)
            {
                if (plan.Item == bomTree.Fg)
                {
                    //转成Bom单位 todo
                    //decimal sourceQty = TheUomConversionMgr.ConvertUomQty(q.Item,bomTree.UomCode,bomTree.AccumQty*
                    var q = flowDetailSnapShotList.Where(f => f.Item == bomTree.Item);
                    //转成订单单位
                    if (q != null && q.Count() > 0)
                    {
                        var qFirst = q.First();
                        qFirst.Qty += ConvertUomQty(qFirst.Item, bomTree.UomCode, bomTree.AccumQty * plan.Qty, qFirst.Uom);
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
        TheSqlHelperMgr.ExecuteSql(" truncate table MRP_SupplierSchedule", null);

        sql = "insert MRP_SupplierSchedule values(@p0,@p1,@p2,@p3,@p4,@p5)";
        //SqlParameter[] sqlParam = new SqlParameter[13];
        foreach (FlowDetailSnapShot fd in flowDetailSnapShotList)
        {
            if (fd.Qty > 0)
            {
                DateTime dateTime = DateTime.Parse("2011-8-25");

                #region 已收
                criteria = DetachedCriteria.For(typeof(LocationTransaction));
                criteria.Add(Expression.Gt("EffectiveDate", dateTime));//选定日期期末库存
                criteria.Add(Expression.Eq("Location", fd.LocationTo));
                criteria.Add(Expression.Eq("Item", fd.Item));
                criteria.Add(Expression.Eq("TransactionType", BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_RCT_PO));
                criteria.SetProjection(Projections.Sum("Qty"));

                IList result = TheCriteriaMgr.FindAll(criteria);
                decimal receivedQty = 0;
                if (result != null && result.Count > 0 && result[0] != null)
                {
                    receivedQty = (decimal)result[0];
                }
                #endregion

                sqlParam[0] = new SqlParameter("@p0", fd.Flow);
                sqlParam[1] = new SqlParameter("@p1", fd.Item);
                sqlParam[2] = new SqlParameter("@p2", fd.ItemDescription);
                sqlParam[3] = new SqlParameter("@p3", fd.Uom);
                sqlParam[4] = new SqlParameter("@p4", fd.Qty);
                sqlParam[5] = new SqlParameter("@p5", receivedQty);

                TheSqlHelperMgr.Create(sql, sqlParam);
            }
        }
        #endregion
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
    }

}
