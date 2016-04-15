using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using com.Sconit.Utility;
using com.Sconit.Entity.Procurement;
using com.Sconit.Entity.View;
using com.Sconit.Entity.MRP;
using System.Reflection;
using NHibernate.Expression;
using System.Text.RegularExpressions;

public partial class MRP_Schedule_MPS_Main : MainModuleBase
{
    private bool enableDiscon = false;
    private IList<ItemReference> itemRefs;
    private IList<Item> items;

    private static log4net.ILog log = log4net.LogManager.GetLogger("NHibernate.SQL");

    public event EventHandler lbRunMrpClickEvent;

    #region 变量
    private bool isExport = false;
    private int seq = 1;
    private int seq_Detail = 1;
    private IList<MrpPlanTransaction> mrpPlanTransactions
    {
        get;
        set;
    }
    private IList<ExpectTransitInventory> expectTransitInventories
    {
        get;
        set;
    }
    private IList<ItemDiscontinue> itemDiscontinueList
    {
        get;
        set;
    }

    private DateTime EffDate
    {
        get
        {
            return DateTime.Parse(this.ddlDate.SelectedValue.Trim());
        }
    }

    private DateTime? WinDate
    {
        get
        {
            if (this.tbScheduleTime.Text.Trim() != string.Empty && isWinTime)
            {
                return DateTime.Parse(this.tbScheduleTime.Text.Trim()).Date;
            }
            else
            {
                return null;
            }
        }
    }

    private DateTime? StartDate
    {
        get
        {
            if (this.tbScheduleTime.Text.Trim() != string.Empty && !isWinTime)
            {
                return DateTime.Parse(this.tbScheduleTime.Text.Trim()).Date;
            }
            else
            {
                return null;
            }
        }
    }


    private string flowOrLoc
    {
        get { return this.tbFlowOrLoc.Text.Trim(); }
    }

    private string itemCode
    {
        get { return this.tbItemCode.Text.Trim(); }
    }

    private bool isWinTime
    {
        get { return this.rblDateType.SelectedIndex == 1; }
    }

    private bool isFlow
    {
        get { return this.rblFlowOrLoc.SelectedIndex == 0; }
    }

    private string FlowCode
    {
        get { return (string)ViewState["FlowCode"]; }
        set { ViewState["FlowCode"] = value; }
    }

    private string FlowType
    {
        get { return (string)ViewState["FlowType"]; }
        set { ViewState["FlowType"] = value; }
    }

    private DateTime? ScheduleDate
    {
        get { return (DateTime)ViewState["ScheduleDate"]; }
        set { ViewState["ScheduleDate"] = value; }
    }

    private string PartyCode
    {
        get { return (string)ViewState["PartyCode"]; }
        set { ViewState["PartyCode"] = value; }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucMonth.isSupplier = false;
        this.tbFlowOrLoc.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:false,bool:false,bool:false,bool:true,bool:false,bool:false,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_FROM;

        if (!IsPostBack)
        {
            DetachedCriteria criteria = DetachedCriteria.For<MrpRunLog>();
            criteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("RunDate")));
            criteria.AddOrder(Order.Desc("RunDate"));
            IList<DateTime> list = TheCriteriaMgr.FindAll<DateTime>(criteria, 0, 30);

            List<string> effDate = list.Select(l => l.ToString("yyyy-MM-dd")).ToList();

            this.ddlDate.DataSource = effDate;
            this.ddlDate.DataBind();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.DoSearch((Button)sender);
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        this.fld_Search.Visible = true;
        this.fld_Group.Visible = false;
        this.DoSearch(this.btnSearch);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int columnCount = this.GV_List.Columns.Count;

        if (e.Row.RowType == DataControlRowType.Header)
        {
            for (int i = 6; i < columnCount; i++)
            {
                e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("*", "<br/>");
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ScheduleBody body = (ScheduleBody)(e.Row.DataItem);

            e.Row.Cells[0].Text = seq.ToString();
            Item item = this.items.Where(i => StringHelper.Eq(i.Code, body.Item)).SingleOrDefault();
            if (item != null)
            {
                e.Row.Cells[2].Text = item.Description;
            }
            ItemReference itemRef = this.GetItemReference(body.Item, this.PartyCode, null);
            if (itemRef != null)
            {
                e.Row.Cells[3].Text = itemRef.ReferenceCode;
            }

            string lblUom = e.Row.Cells[4].Text;
            string lblUnitCount = e.Row.Cells[5].Text;

            seq++;

            if (!isExport && this.cbDetail.Checked)
            {
                for (int i = 6; i < columnCount; i++)
                {
                    string headerText = this.GV_List.Columns[i].SortExpression;
                    string lastHeaderText = this.GV_List.Columns[i].FooterText;
                    DateTime headerTextTime = DateTime.Parse(headerText);
                    DateTime? lastHeaderTextTime = null;
                    if (lastHeaderText != string.Empty)
                    {
                        lastHeaderTextTime = DateTime.Parse(lastHeaderText);
                    }

                    string[] segment = this.GV_List.Columns[i].HeaderText.Split('*');
                    e.Row.Cells[i].Attributes.Add("title", this.GetDetail(body.Item, headerTextTime, lastHeaderTextTime, segment));
                }
            }
            else
            {
                e.Row.Cells[1].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            }
        }
    }

    protected void CV_ServerValidate(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;

        switch (cv.ID)
        {
            case "cvStartDate":
                try
                {
                    Convert.ToDateTime(args.Value);
                }
                catch (Exception)
                {
                    ShowWarningMessage("Common.Date.Error");
                    args.IsValid = false;
                }
                break;
            default:
                break;
        }
    }

    protected void rblFlowOrLoc_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (isFlow)
        {
            this.tbFlowOrLoc.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:false,bool:false,bool:false,bool:true,bool:false,bool:false,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_TO;
            this.tbFlowOrLoc.ServicePath = "FlowMgr.service";
            this.tbFlowOrLoc.ServiceMethod = "GetFlowList";
            this.tbFlowOrLoc.DescField = "Description";
        }
        else
        {
            this.tbFlowOrLoc.ServiceParameter = "string:" + this.CurrentUser.Code;
            this.tbFlowOrLoc.ServicePath = "LocationMgr.service";
            this.tbFlowOrLoc.ServiceMethod = "GetLocationByUserCode";
            this.tbFlowOrLoc.DescField = "Name";
        }
        this.tbFlowOrLoc.Text = string.Empty;
        this.tbFlowOrLoc.DataBind();
    }

    private void DoSearch(Button button)
    {
        try
        {
            log.Info("SearchStart@" + DateTime.Now);
            //this.itemRefs = TheItemReferenceMgr.GetAllItemReference();
            this.items = TheItemMgr.GetCacheAllItem();
            if (this.flowOrLoc == string.Empty)
            {
                ShowErrorMessage("MRP.Schedule.Import.CustomerSchedule.Result.SelectFlow");
                return;
            }
            else if (isFlow)
            {
                Flow flow = TheFlowMgr.LoadFlow(this.flowOrLoc);
                if (flow.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    this.PartyCode = flow.PartyTo.Code;
                }
                else
                {
                    this.PartyCode = flow.PartyFrom.Code;
                }
                SecurityHelper.CheckPermission(flow.Type, flow.PartyFrom.Code, flow.PartyTo.Code, this.CurrentUser);
            }

            DetachedCriteria criteria = DetachedCriteria.For<ExpectTransitInventory>();
            criteria.Add(Expression.Eq("EffectiveDate", this.EffDate));
            criteria.Add(Expression.Not(Expression.Eq("TransitQty", 0M)));

            expectTransitInventories = this.TheCriteriaMgr.FindAll<ExpectTransitInventory>(criteria);
            itemDiscontinueList = this.TheCriteriaMgr.FindAll<ItemDiscontinue>();


            this.GV_List.EnableViewState = true;
            if (button == this.btnSearch)
            {
                this.mrpPlanTransactions = TheMrpPlanTransactionMgr.GetMrpPlanTransactions((isFlow ? this.flowOrLoc : null), (!isFlow ? this.flowOrLoc : null), this.itemCode, this.EffDate, this.WinDate, this.StartDate);
                //this.mrpPlanTransactions = this.mrpPlanTransactions.Where(m => m.Qty > 0).ToList();
            }
            log.Info("GetMrpShipPlans@" + DateTime.Now);
            IList<MrpShipPlanView> mrpShipPlanViews = TheMrpShipPlanViewMgr.GetMrpShipPlanViews((isFlow ? this.flowOrLoc : null), (!isFlow ? this.flowOrLoc : null), this.itemCode, this.EffDate, this.WinDate, this.StartDate);

            log.Info("GetMrpShipPlanViews@" + DateTime.Now);
            criteria = DetachedCriteria.For<ExpectTransitInventoryView>();
            criteria.Add(Expression.Eq("EffectiveDate", this.EffDate));
            IList<ExpectTransitInventoryView> transitInventoryViews = this.TheCriteriaMgr.FindAll<ExpectTransitInventoryView>(criteria);

            log.Info("GetTransitInventoryViews@" + DateTime.Now);
            ScheduleView scheduleView = this.TransferMrpShipPlanViews2ScheduleView(mrpShipPlanViews, transitInventoryViews, itemDiscontinueList, this.rblFlowOrLoc.SelectedValue, this.rblDateType.SelectedValue);

            log.Info("TransferMrpShipPlanViews2ScheduleView@" + DateTime.Now);

            this.GV_List_DataBind(scheduleView);
            if (button == this.btnExport)
            {
                this.ExportXLS(this.GV_List);
                this.isExport = true;
            }
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage(ex);
        }
    }

    private void GV_List_DataBind(ScheduleView scheduleView)
    {
        log.Info("GV_List_DataBind Start@" + DateTime.Now);
        for (int i = this.GV_List.Columns.Count; i > 6; i--)
        {
            this.GV_List.Columns.RemoveAt(this.GV_List.Columns.Count - 1);
        }

        this.fld_Group.Visible = true;

        if (scheduleView == null) return;
        IList<ScheduleBody> scheduleBodys = scheduleView.ScheduleBodys;
        IList<ScheduleHead> scheduleHeads = scheduleView.ScheduleHeads;

        #region add qty column
        if (scheduleHeads != null && scheduleHeads.Count > 0)
        {
            int i = 0;
            foreach (ScheduleHead scheduleHead in scheduleHeads)
            {
                string qty = "Qty" + i.ToString();
                if (enableDiscon)
                {
                    qty = "DisplayQty" + i.ToString();
                }

                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                {
                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qty))
                    {
                        BoundField bfColumn = new BoundField();
                        bfColumn.DataField = qty;
                        bfColumn.DataFormatString = "{0:#,##0.##}";
                        bfColumn.HeaderText = isWinTime ? scheduleHead.DateTo.ToString("MM-dd") : scheduleHead.DateFrom.ToString("MM-dd");
                        bfColumn.HeaderText += "*" + scheduleHead.PeriodType + "*" + scheduleHead.SourceType;
                        bfColumn.SortExpression = isWinTime ? scheduleHead.DateTo.ToString("yyyy-MM-dd") : scheduleHead.DateFrom.ToString("yyyy-MM-dd");
                        bfColumn.FooterText = isWinTime ? (scheduleHead.LastDateTo.HasValue ? scheduleHead.LastDateTo.Value.ToString("yyyy-MM-dd") : string.Empty)
                            : (scheduleHead.LastDateFrom.HasValue ? scheduleHead.LastDateFrom.Value.ToString("yyyy-MM-dd") : string.Empty);
                        this.GV_List.Columns.Add(bfColumn);
                        break;
                    }
                }
                i++;
            }
            this.ltl_GV_List_Result.Visible = false;
        }
        else
        {
            this.ltl_GV_List_Result.Visible = true;
        }
        #endregion
        this.GV_List.DataSource = scheduleBodys;
        this.GV_List.DataBind();
        //this.btnSave.Visible = false;
        log.Info("GV_List_DataBind End@" + DateTime.Now);
    }

    private string GetDetail(string itemCode, DateTime effTime, DateTime? lastHeaderTextTime, string[] segment)
    {
        string detail = string.Empty;
        if (this.mrpPlanTransactions != null)
        {
            var q_Trans = isFlow ?
                this.mrpPlanTransactions.Where(m => StringHelper.Eq(itemCode, m.Item)
                && StringHelper.Eq(flowOrLoc, m.Flow))
                :
                this.mrpPlanTransactions.Where(m => StringHelper.Eq(itemCode, m.Item)
                && StringHelper.Eq(flowOrLoc, m.Location))
                ;

            IList<ExpectTransitInventory> expectTransitInventoryList = new List<ExpectTransitInventory>();
            IList<ExpectTransitInventory> disconExpectTransitInventoryList = new List<ExpectTransitInventory>();
            if (this.expectTransitInventories != null && expectTransitInventories.Count > 0)
            {
                if (isFlow)
                {
                    if (isWinTime)
                    {
                        var p = from inv in this.expectTransitInventories
                                where inv.Flow == this.flowOrLoc
                                && inv.Item == itemCode
                                && (!lastHeaderTextTime.HasValue || lastHeaderTextTime.Value < inv.WindowTime)
                                && inv.WindowTime <= effTime
                                select inv;

                        if (p != null && p.Count() > 0)
                        {
                            expectTransitInventoryList = p.ToList();
                        }

                        if (itemDiscontinueList != null && itemDiscontinueList.Count() > 0)
                        {
                            var r = from discon in itemDiscontinueList
                                    join inv in this.expectTransitInventories
                                    on discon.DiscontinueItem.Code equals inv.Item
                                    where inv.Flow == this.flowOrLoc
                                    && discon.Item.Code == itemCode
                                    && inv.WindowTime <= effTime
                                    && (!lastHeaderTextTime.HasValue || lastHeaderTextTime.Value < inv.WindowTime)
                                     && discon.StartDate <= inv.StartTime
                                    && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                    select inv;

                            if (r != null && r.Count() >= 0)
                            {
                                disconExpectTransitInventoryList = r.ToList();
                            }
                        }
                    }
                    else
                    {
                        var p = from inv in this.expectTransitInventories
                                where inv.Flow == this.flowOrLoc
                                && inv.Item == itemCode
                                && (!lastHeaderTextTime.HasValue || lastHeaderTextTime.Value < inv.StartTime)
                                && inv.StartTime <= effTime
                                select inv;

                        if (p != null && p.Count() > 0)
                        {
                            expectTransitInventoryList = p.ToList();
                        }

                        if (itemDiscontinueList != null && itemDiscontinueList.Count() > 0)
                        {
                            var r = from discon in itemDiscontinueList
                                    join inv in this.expectTransitInventories
                                    on discon.DiscontinueItem.Code equals inv.Item
                                    where inv.Flow == this.flowOrLoc
                                    && discon.Item.Code == itemCode
                                    && inv.StartTime <= effTime
                                    && (!lastHeaderTextTime.HasValue || lastHeaderTextTime.Value < inv.StartTime)
                                    && discon.StartDate <= inv.StartTime
                                    && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                    select inv;

                            if (r != null && r.Count() >= 0)
                            {
                                disconExpectTransitInventoryList = r.ToList();
                            }
                        }
                    }
                }
                else
                {
                    if (isWinTime)
                    {
                        var p = from inv in this.expectTransitInventories
                                where inv.Location == this.flowOrLoc
                                && inv.Item == itemCode
                                && (!lastHeaderTextTime.HasValue || lastHeaderTextTime.Value < inv.WindowTime)
                                && inv.WindowTime <= effTime
                                select inv;

                        if (p != null && p.Count() > 0)
                        {
                            expectTransitInventoryList = p.ToList();
                        }

                        if (itemDiscontinueList != null && itemDiscontinueList.Count() > 0)
                        {
                            var r = from discon in itemDiscontinueList
                                    join inv in this.expectTransitInventories
                                    on discon.DiscontinueItem.Code equals inv.Item
                                    where inv.Location == this.flowOrLoc
                                    && discon.Item.Code == itemCode
                                    && inv.WindowTime <= effTime
                                    && (!lastHeaderTextTime.HasValue || lastHeaderTextTime.Value < inv.WindowTime)
                                     && discon.StartDate <= inv.StartTime
                                    && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                    select inv;

                            if (r != null && r.Count() >= 0)
                            {
                                disconExpectTransitInventoryList = r.ToList();
                            }
                        }
                    }
                    else
                    {
                        var p = from inv in this.expectTransitInventories
                                where inv.Location == this.flowOrLoc
                                && inv.Item == itemCode
                                && (!lastHeaderTextTime.HasValue || lastHeaderTextTime.Value < inv.StartTime)
                                && inv.StartTime <= effTime
                                select inv;

                        if (p != null && p.Count() > 0)
                        {
                            expectTransitInventoryList = p.ToList();
                        }

                        if (itemDiscontinueList != null && itemDiscontinueList.Count() > 0)
                        {
                            var r = from discon in itemDiscontinueList
                                    join inv in this.expectTransitInventories
                                    on discon.DiscontinueItem.Code equals inv.Item
                                    where inv.Location == this.flowOrLoc
                                    && discon.Item.Code == itemCode
                                    && inv.StartTime <= effTime
                                    && (!lastHeaderTextTime.HasValue || lastHeaderTextTime.Value < inv.StartTime)
                                    && discon.StartDate <= inv.StartTime
                                    && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                    select inv;

                            if (r != null && r.Count() >= 0)
                            {
                                disconExpectTransitInventoryList = r.ToList();
                            }
                        }
                    }
                }
            }

            if (isWinTime)
            {
                q_Trans = q_Trans.Where(m => m.WindowTime.Date == effTime.Date && m.PeriodType == segment[1] && m.SourceType == segment[2]);
            }
            else
            {
                q_Trans = q_Trans.Where(m => m.StartTime.Date == effTime.Date && m.PeriodType == segment[1] && m.SourceType == segment[2]);
            }

            if (q_Trans.Count() > 0 || expectTransitInventoryList.Count > 0 || disconExpectTransitInventoryList.Count > 0)
            {
                detail += "cssbody=[obbd] cssheader=[obhd] header=[${MRP.Schedule.Detail}] body=[<table width=100%>";

                if (q_Trans.Count() > 0)
                {
                    //detail += "<tr><td></td><td>数量</td><td>开始时间</td><td>到货时间</td><td>时间类型</td><td>需求类型</td></tr>";
                }
                foreach (MrpPlanTransaction trans in q_Trans)
                {
                    string Qty = trans.Qty.ToString("#,##0.##");
                    string startTime = trans.StartTime.ToString("MM-dd");
                    string winTime = trans.WindowTime.ToString("MM-dd");
                    string periodType = trans.PeriodType;
                    string sourceType = trans.SourceType;
                    if (trans.SourceType == BusinessConstants.CODE_MASTER_MRP_SOURCE_TYPE_VALUE_ORDER)
                    {
                        sourceType = trans.Reference;
                    }
                    //string locationFrom = mrpShipPlan.LocationFrom == null ? string.Empty : mrpShipPlan.LocationFrom;
                    //string locationTo = mrpShipPlan.LocationTo == null ? string.Empty : mrpShipPlan.LocationTo;

                    detail += "<tr><td>${MRP.Schedule.Demand}</td><td>" + Qty + "</td><td>" + startTime + "</td><td>" + winTime + "</td><td>"
                        + periodType + "</td><td>" + sourceType + "</td></tr>";
                }

                if (q_Trans.Count() > 0)
                {
                    //detail += "<tr><td></td><td>数量</td><td>开始时间</td><td>到货时间</td><td colspan='2'>订单号</td></tr>";
                }
                foreach (ExpectTransitInventory expectTransitInventory in expectTransitInventoryList)
                {
                    string orderNo = expectTransitInventory.OrderNo;
                    string Qty = expectTransitInventory.TransitQty.ToString("#,##0.##");
                    string startTime = expectTransitInventory.StartTime.ToString("MM-dd");
                    string winTime = expectTransitInventory.WindowTime.ToString("MM-dd");
                    //string locationFrom = mrpShipPlan.LocationFrom == null ? string.Empty : mrpShipPlan.LocationFrom;
                    //string locationTo = mrpShipPlan.LocationTo == null ? string.Empty : mrpShipPlan.LocationTo;

                    detail += "<tr><td>订单-</td><td>" + Qty + "</td><td>" + startTime + "</td><td>" + winTime + "</td><td colspan='2'>" + orderNo + "</td></tr>";
                }

                foreach (ExpectTransitInventory expectTransitInventory in disconExpectTransitInventoryList)
                {
                    string item = expectTransitInventory.Item;
                    string orderNo = expectTransitInventory.OrderNo;
                    string Qty = expectTransitInventory.TransitQty.ToString("#,##0.##");
                    string startTime = expectTransitInventory.StartTime.ToString("MM-dd");
                    string winTime = expectTransitInventory.WindowTime.ToString("MM-dd");
                    //string locationFrom = mrpShipPlan.LocationFrom == null ? string.Empty : mrpShipPlan.LocationFrom;
                    //string locationTo = mrpShipPlan.LocationTo == null ? string.Empty : mrpShipPlan.LocationTo;

                    detail += "<tr><td>${MRP.Schedule.Discon}</td><td>" + Qty + "</td><td>" + startTime + "</td><td>" + winTime + "</td><td>" + orderNo + "</td><td>" + item + "</td></tr>";
                }
                detail += "</table>]";
            }
        }
        return detail;
    }

    private ItemReference GetItemReference(string itemCode, string partyCode1, string partyCode2)
    {   return null;
        var q = itemRefs.Where(i => i.Item.Code == itemCode && i.Party != null);
        if (q.Count() == 1)
        {
            return q.First();
        }
        else if (q.Count() > 1)
        {
            var q1 = q.Where(i => i.Party.Code == partyCode1);
            if (q1.Count() > 0)
            {
                return q1.First();
            }
            else
            {
                var q2 = q.Where(i => i.Party.Code == partyCode2);
                if (q2.Count() > 0)
                {
                    return q2.First();
                }
            }
        }
        q = itemRefs.Where(i => i.Item.Code == itemCode && i.Party == null);
        if (q.Count() > 0)
        {
            return q.First();
        }
        return null;
    }

    public ScheduleView TransferMrpShipPlanViews2ScheduleView(IList<MrpShipPlanView> mrpShipPlanViews,
        IList<ExpectTransitInventoryView> expectTransitInventoryViews,
        IList<ItemDiscontinue> itemDiscontinueList,
        string locOrFlow, string winOrStartTime)
    {
        if (mrpShipPlanViews == null || mrpShipPlanViews.Count == 0)
        {
            return null;
        }
        #region 头
        List<ScheduleHead> scheduleHeads = new List<ScheduleHead>();

        if (locOrFlow == "Flow")
        {
            if (winOrStartTime == "WindowTime")
            {
                scheduleHeads = (from det in mrpShipPlanViews
                                 group det by new { det.Flow, det.FlowType, det.WindowTime, det.PeriodType, det.SourceType } into result
                                 select new ScheduleHead
                                 {
                                     Flow = result.Key.Flow,
                                     Type = result.Key.FlowType,
                                     DateTo = result.Key.WindowTime,
                                     PeriodType = result.Key.PeriodType,
                                     SourceType = result.Key.SourceType
                                 }).ToList();
            }
            else
            {
                scheduleHeads = (from det in mrpShipPlanViews
                                 group det by new { det.Flow, det.FlowType, det.StartTime, det.PeriodType, det.SourceType } into result
                                 select new ScheduleHead
                                 {
                                     Flow = result.Key.Flow,
                                     Type = result.Key.FlowType,
                                     DateFrom = result.Key.StartTime,
                                     PeriodType = result.Key.PeriodType,
                                     SourceType = result.Key.SourceType
                                 }).ToList();
            }
        }
        else if (locOrFlow == "Location")
        {
            if (winOrStartTime == "WindowTime")
            {
                scheduleHeads = (from det in mrpShipPlanViews
                                 group det by new { det.Location, det.WindowTime, det.PeriodType, det.SourceType } into result
                                 select new ScheduleHead
                                 {
                                     Location = result.Key.Location,
                                     Type = "Location",
                                     DateTo = result.Key.WindowTime,
                                     PeriodType = result.Key.PeriodType,
                                     SourceType = result.Key.SourceType
                                 }).ToList();
            }
            else
            {
                scheduleHeads = (from det in mrpShipPlanViews
                                 group det by new { det.Location, det.StartTime, det.PeriodType, det.SourceType } into result
                                 select new ScheduleHead
                                 {
                                     Location = result.Key.Location,
                                     Type = "Location",
                                     DateFrom = result.Key.StartTime,
                                     PeriodType = result.Key.PeriodType,
                                     SourceType = result.Key.SourceType
                                 }).ToList();
            }
        }
        else
        {
            throw new TechnicalException(locOrFlow);
        }

        if (winOrStartTime == "WindowTime")
        {
            scheduleHeads = scheduleHeads.OrderBy(c => c.DateTo).Take(40).ToList();
        }
        else
        {
            scheduleHeads = scheduleHeads.OrderBy(c => c.DateFrom).Take(40).ToList();
        }
        #endregion

        #region 明细
        List<ScheduleBody> scheduleBodys =
            (from det in mrpShipPlanViews
             group det by new { det.Item, det.ItemDescription, det.ItemReference, det.Uom, det.UnitCount }
                 into result
                 select new ScheduleBody
                 {
                     Item = result.Key.Item,
                     ItemDescription = result.Key.ItemDescription,
                     ItemReference = result.Key.ItemReference,
                     Uom = result.Key.Uom,
                     UnitCount = result.Key.UnitCount,
                 }).ToList();
        #endregion

        #region 赋值
        foreach (ScheduleBody scheduleBody in scheduleBodys)
        {
            int i = 0;
            DateTime? lastDate = null;
            ScheduleHead lastScheduleHead = null;


            var itemDisconList = itemDiscontinueList == null ? null :
                    from discon in itemDiscontinueList
                    where discon.Item.Code == scheduleBody.Item
                    select discon;


            foreach (ScheduleHead scheduleHead in scheduleHeads)
            {
                string qty = "Qty" + i.ToString();
                string actQty = "ActQty" + i.ToString();
                string disconActQty = "DisconActQty" + i.ToString();

                if (locOrFlow == "Location")
                {
                    if (winOrStartTime == "WindowTime")
                    {
                        var p = from plan in mrpShipPlanViews
                                where plan.Location == scheduleHead.Location
                                && plan.Item == scheduleBody.Item
                                && plan.WindowTime == scheduleHead.DateTo
                                && plan.SourceType == scheduleHead.SourceType
                                && plan.PeriodType == scheduleHead.PeriodType
                                select plan;

                        if (p != null && p.Count() >= 0)
                        {
                            PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                            foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                            {
                                if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qty))
                                {
                                    pi.SetValue(scheduleBody, p.Sum(pp => pp.Qty), null);
                                    break;
                                }
                            }
                        }

                        var q = from inv in expectTransitInventoryViews
                                where inv.Location == scheduleHead.Location
                                && inv.Item == scheduleBody.Item
                                && inv.WindowTime <= scheduleHead.DateTo
                                && (!lastDate.HasValue || inv.WindowTime > lastDate.Value)
                                select inv;

                        if (q != null && q.Count() >= 0)
                        {
                            PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                            foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                            {
                                if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), actQty))
                                {
                                    pi.SetValue(scheduleBody, q.Sum(qq => qq.TransitQty), null);
                                    break;
                                }
                            }
                        }

                        if (itemDisconList != null && itemDisconList.Count() > 0)
                        {
                            var r = from discon in itemDisconList
                                    join inv in expectTransitInventoryViews
                                    on discon.DiscontinueItem.Code equals inv.Item
                                    where inv.Location == scheduleHead.Location
                                    && inv.WindowTime <= scheduleHead.DateTo
                                    && (!lastDate.HasValue || inv.WindowTime > lastDate.Value)
                                    && discon.StartDate <= inv.StartTime
                                    && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                    select inv;

                            if (r != null && r.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), disconActQty))
                                    {
                                        pi.SetValue(scheduleBody, r.Sum(rr => rr.TransitQty), null);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (winOrStartTime == "StartTime")
                    {
                        var p = from plan in mrpShipPlanViews
                                where plan.Location == scheduleHead.Location
                                && plan.Item == scheduleBody.Item
                                && plan.StartTime == scheduleHead.DateFrom
                                && plan.SourceType == scheduleHead.SourceType
                                && plan.PeriodType == scheduleHead.PeriodType
                                select plan;

                        if (p != null && p.Count() >= 0)
                        {
                            PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                            foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                            {
                                if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qty))
                                {
                                    pi.SetValue(scheduleBody, p.Sum(pp => pp.Qty), null);
                                    break;
                                }
                            }
                        }

                        var q = from inv in expectTransitInventoryViews
                                where inv.Location == scheduleHead.Location
                                && inv.Item == scheduleBody.Item
                                && inv.StartTime <= scheduleHead.DateFrom
                                && (!lastDate.HasValue || inv.StartTime > lastDate.Value)
                                select inv;

                        if (q != null && q.Count() >= 0)
                        {
                            PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                            foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                            {
                                if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), actQty))
                                {
                                    pi.SetValue(scheduleBody, q.Sum(qq => qq.TransitQty), null);
                                    break;
                                }
                            }
                        }

                        if (itemDisconList != null && itemDisconList.Count() > 0)
                        {
                            var r = from discon in itemDisconList
                                    join inv in expectTransitInventoryViews
                                    on discon.DiscontinueItem.Code equals inv.Item
                                    where inv.Location == scheduleHead.Location
                                    && inv.StartTime <= scheduleHead.DateFrom
                                    && (!lastDate.HasValue || inv.StartTime > lastDate.Value)
                                    && discon.StartDate <= inv.StartTime
                                    && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                    select inv;

                            if (r != null && r.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), disconActQty))
                                    {
                                        pi.SetValue(scheduleBody, r.Sum(rr => rr.TransitQty), null);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (locOrFlow == "Flow")
                {
                    if (winOrStartTime == "WindowTime")
                    {
                        var p = from plan in mrpShipPlanViews
                                where plan.Flow == scheduleHead.Flow
                                && plan.Item == scheduleBody.Item
                                && plan.WindowTime == scheduleHead.DateTo
                                && plan.SourceType == scheduleHead.SourceType
                                && plan.PeriodType == scheduleHead.PeriodType
                                select plan;

                        if (p != null && p.Count() >= 0)
                        {
                            PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                            foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                            {
                                if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qty))
                                {
                                    pi.SetValue(scheduleBody, p.Sum(pp => pp.Qty), null);
                                    break;
                                }
                            }
                        }

                        var q = from inv in expectTransitInventoryViews
                                where inv.Flow == scheduleHead.Flow
                                && inv.Item == scheduleBody.Item
                                && inv.WindowTime <= scheduleHead.DateTo
                                && (!lastDate.HasValue || inv.WindowTime > lastDate.Value)
                                select inv;

                        if (q != null && q.Count() >= 0)
                        {
                            PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                            foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                            {
                                if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), actQty))
                                {
                                    pi.SetValue(scheduleBody, q.Sum(qq => qq.TransitQty), null);
                                    break;
                                }
                            }
                        }

                        if (itemDisconList != null && itemDisconList.Count() > 0)
                        {
                            var r = from discon in itemDisconList
                                    join inv in expectTransitInventoryViews
                                    on discon.DiscontinueItem.Code equals inv.Item
                                    where inv.Flow == scheduleHead.Flow
                                    && inv.WindowTime <= scheduleHead.DateTo
                                    && (!lastDate.HasValue || inv.WindowTime > lastDate.Value)
                                    && discon.StartDate <= inv.StartTime
                                    && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                    select inv;

                            if (r != null && r.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), disconActQty))
                                    {
                                        pi.SetValue(scheduleBody, r.Sum(rr => rr.TransitQty), null);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (winOrStartTime == "StartTime")
                    {
                        var p = from plan in mrpShipPlanViews
                                where plan.Flow == scheduleHead.Flow
                                && plan.Item == scheduleBody.Item
                                && plan.StartTime == scheduleHead.DateFrom
                                && plan.SourceType == scheduleHead.SourceType
                                && plan.PeriodType == scheduleHead.PeriodType
                                select plan;

                        if (p != null && p.Count() >= 0)
                        {
                            PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                            foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                            {
                                if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qty))
                                {
                                    pi.SetValue(scheduleBody, p.Sum(pp => pp.Qty), null);
                                    break;
                                }
                            }
                        }

                        var q = from inv in expectTransitInventoryViews
                                where inv.Flow == scheduleHead.Flow
                                && inv.Item == scheduleBody.Item
                                && inv.StartTime <= scheduleHead.DateFrom
                                && (!lastDate.HasValue || inv.StartTime > lastDate.Value)
                                select inv;

                        if (q != null && q.Count() >= 0)
                        {
                            PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                            foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                            {
                                if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), actQty))
                                {
                                    pi.SetValue(scheduleBody, q.Sum(qq => qq.TransitQty), null);
                                    break;
                                }
                            }
                        }

                        if (itemDisconList != null && itemDisconList.Count() > 0)
                        {
                            var r = from discon in itemDisconList
                                    join inv in expectTransitInventoryViews
                                    on discon.DiscontinueItem.Code equals inv.Item
                                    where inv.Flow == scheduleHead.Flow
                                    && inv.StartTime <= scheduleHead.DateFrom
                                    && (!lastDate.HasValue || inv.StartTime > lastDate.Value)
                                    && discon.StartDate <= inv.StartTime
                                    && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                    select inv;

                            if (r != null && r.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), disconActQty))
                                    {
                                        pi.SetValue(scheduleBody, r.Sum(rr => rr.TransitQty), null);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                i++;
                if (winOrStartTime == "WindowTime")
                {
                    lastDate = scheduleHead.DateTo;
                }
                else if (winOrStartTime == "StartTime")
                {
                    lastDate = scheduleHead.DateFrom;
                }
                else
                {
                    throw new TechnicalException(winOrStartTime);
                }

                if (lastScheduleHead != null)
                {
                    scheduleHead.LastDateTo = lastScheduleHead.DateTo;
                    scheduleHead.LastDateFrom = lastScheduleHead.DateFrom;
                }
                lastScheduleHead = scheduleHead;
            }
        }
        #endregion

        #region 过滤全部是0的
        scheduleBodys = scheduleBodys.Where(s => s.TotalQty > 0).ToList();
        #endregion

        ScheduleView scheduleView = new ScheduleView();
        scheduleView.ScheduleHeads = scheduleHeads;
        scheduleView.ScheduleBodys = scheduleBodys;
        return scheduleView;
    }

    protected void lbn1_Click(object sender, EventArgs e)
    {
        this.ucMonth.Visible = false;
        this.fld_Search.Visible = true;
        this.fld_Group.Visible = true;
        this.tab_1.Attributes["class"] = "ajax__tab_active";
        this.tab_3.Attributes["class"] = "ajax__tab_inactive";
    }


    protected void lbn3_Click(object sender, EventArgs e)
    {
        this.ucMonth.Visible = true;
        this.fld_Search.Visible = false;
        this.fld_Group.Visible = false;
        this.tab_1.Attributes["class"] = "ajax__tab_inactive";
        this.tab_3.Attributes["class"] = "ajax__tab_active";
    }
}

