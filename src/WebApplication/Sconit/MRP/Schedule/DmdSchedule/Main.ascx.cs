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

public partial class MRP_Schedule_DmdSchedule_Main : MainModuleBase
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

    private bool isSupplier
    {
        get
        {
            if (this.ModuleParameter.ContainsKey("IsSupplier"))
            {
                return bool.Parse(this.ModuleParameter["IsSupplier"]);
            }
            return false;
        }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (isSupplier)
        {
            this.tbFlowOrLoc.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:true,bool:false,bool:true,bool:true,bool:true,bool:false,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_FROM;
            this.cbDetail.Visible = false;
            //TheFlowMgr.GetFlowList(
        }
        else
        {
            this.tbFlowOrLoc.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:true,bool:true,bool:true,bool:true,bool:true,bool:true,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_FROM;
        }
        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:true,bool:true,bool:true,bool:true,bool:true,bool:true,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_TO;

        this.tbWinTime.Attributes.Add("onclick", "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm',lang:'" + this.CurrentUser.UserLanguage + "'})");
        this.tbWinTime.Attributes["onchange"] += "setStartTime();";
        this.cbIsUrgent.Attributes["onchange"] += "setStartTime();";
        if (!IsPostBack)
        {
            this.ucShift.Date = DateTime.Today;

            DetachedCriteria criteria = DetachedCriteria.For<MrpRunLog>();
            criteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("RunDate")));
            criteria.AddOrder(Order.Desc("RunDate"));
            IList<DateTime> list = TheCriteriaMgr.FindAll<DateTime>(criteria, 0, 30);
            if (this.CurrentUser.Code == "4026" || this.CurrentUser.Code == "su")
            {
                list = TheCriteriaMgr.FindAll<DateTime>(criteria, 0, 300);
            }

            List<string> effDate = list.Select(l => l.ToString("yyyy-MM-dd")).ToList();

            this.ddlDate.DataSource = effDate;
            this.ddlDate.DataBind();
        }

        this.GV_Order.Columns[7].Visible = enableDiscon;
        this.GV_Order.Columns[8].Visible = enableDiscon;

        if (isSupplier)
        {
            this.rblFlowOrLoc.Items[1].Enabled = false;
            this.rblListFormat.Visible = false;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.DoSearch((Button)sender);
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        this.fld_Search.Visible = true;
        this.div_OrderDetail.Visible = false;
        this.div_MRP_Detail.Visible = false;
        this.fld_Group.Visible = false;
        this.DoSearch(this.btnSearch);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            foreach (GridViewRow gvr in this.GV_Detail.Rows)
            {
                HiddenField hdfId = (HiddenField)gvr.FindControl("hdfId");
                int id = int.Parse(hdfId.Value);

                TextBox tbQty = (TextBox)gvr.FindControl("tbQty");
                decimal qty = decimal.Parse(tbQty.Text.Trim());

                foreach (MrpPlanTransaction mrpPlanTransaction in this.mrpPlanTransactions)
                {
                    if (id == mrpPlanTransaction.Id)
                    {
                        mrpPlanTransaction.Qty = qty;
                        break;
                    }
                }
            }
            ShowSuccessMessage("${MRP.Schedule.Update.CustomerSchedule.Result.Successfully}");
            //TheMrpShipPlanMgr.UpdateMrpShipPlan(this.mrpShipPlans);
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage("MRP.Schedule.Create.CustomerSchedule.Result.Successfully");
        }
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.tbFlow.Text == string.Empty)
            {
                ShowErrorMessage("${MRP.Schedule.Import.CustomerSchedule.Result.SelectFlow}");
                return;
            }
            Flow flow = TheFlowMgr.CheckAndLoadFlow(this.tbFlow.Text);

            OrderHead orderHead = this.TheOrderMgr.TransferFlow2Order(flow);

            foreach (GridViewRow row in this.GV_Order.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    string item = row.Cells[1].Text;
                    string uom = row.Cells[4].Text;
                    string qtyStr = ((TextBox)row.Cells[9].FindControl("tbQty")).Text;
                    decimal? qty = null;
                    try
                    {
                        qty = decimal.Parse(qtyStr);
                    }
                    catch (Exception)
                    {
                        this.ShowErrorMessage("${MasterData.MiscOrder.WarningMessage.InputQtyFormat.Error}");
                        return;
                    }

                    if (qty.HasValue && qty > 0)
                    {
                        OrderDetail orderDetail = (from det in orderHead.OrderDetails
                                                   where det.Item.Code == item
                                                   select det).FirstOrDefault();

                        if (orderDetail != null)
                        {
                            orderDetail.OrderedQty = qty.Value;

                            if (orderDetail.Uom.Code != uom)
                            {
                                orderDetail.OrderedQty = this.TheUomConversionMgr.ConvertUomQty(item, uom, orderDetail.OrderedQty, orderDetail.Uom.Code);
                            }
                        }
                    }
                }
            }

            IList<OrderDetail> resultOrderDetailList = new List<OrderDetail>();

            if (orderHead != null && orderHead.OrderDetails != null && orderHead.OrderDetails.Count > 0)
            {
                foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                {
                    if (orderDetail.OrderedQty != 0)
                    {
                        if (orderDetail.Item.Type == BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_K)
                        {
                            IList<Item> newItemList = new List<Item>(); //填充套件子件
                            decimal? convertRate = null;
                            IList<ItemKit> itemKitList = null;

                            var maxSequence = orderHead.OrderDetails.Max(o => o.Sequence);
                            itemKitList = this.TheItemKitMgr.GetChildItemKit(orderDetail.Item);
                            for (int i = 0; i < itemKitList.Count; i++)
                            {
                                Item item = itemKitList[i].ChildItem;
                                if (!convertRate.HasValue)
                                {
                                    if (itemKitList[i].ParentItem.Uom.Code != orderDetail.Item.Uom.Code)
                                    {
                                        convertRate = this.TheUomConversionMgr.ConvertUomQty(orderDetail.Item, orderDetail.Item.Uom, 1, itemKitList[i].ParentItem.Uom);
                                    }
                                    else
                                    {
                                        convertRate = 1;
                                    }
                                }
                                OrderDetail newOrderDetail = new OrderDetail();

                                newOrderDetail.OrderHead = orderDetail.OrderHead;
                                newOrderDetail.Sequence = maxSequence + (i + 1);
                                newOrderDetail.IsBlankDetail = false;
                                newOrderDetail.Item = item;

                                newOrderDetail.Uom = item.Uom;
                                newOrderDetail.UnitCount = orderDetail.Item.UnitCount * itemKitList[i].Qty * convertRate.Value;
                                newOrderDetail.OrderedQty = orderDetail.OrderedQty * itemKitList[i].Qty * convertRate.Value;
                                newOrderDetail.PackageType = orderDetail.PackageType;

                                #region 价格字段
                                if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
                                {
                                    if (orderDetail.PriceList != null && orderDetail.PriceList.Code != string.Empty)
                                    {
                                        newOrderDetail.PriceList = ThePriceListMgr.LoadPriceList(orderDetail.PriceList.Code);
                                        if (newOrderDetail.PriceList != null)
                                        {
                                            PriceListDetail priceListDetail = this.ThePriceListDetailMgr.GetLastestPriceListDetail(newOrderDetail.PriceList, item, DateTime.Now, newOrderDetail.OrderHead.Currency, item.Uom);
                                            newOrderDetail.IsProvisionalEstimate = priceListDetail == null ? true : priceListDetail.IsProvisionalEstimate;
                                            if (priceListDetail != null)
                                            {
                                                newOrderDetail.UnitPrice = priceListDetail.UnitPrice;
                                                newOrderDetail.TaxCode = priceListDetail.TaxCode;
                                                newOrderDetail.IsIncludeTax = priceListDetail.IsIncludeTax;
                                            }
                                        }
                                    }
                                }
                                #endregion
                                resultOrderDetailList.Add(newOrderDetail);
                            }
                        }
                        else
                        {
                            resultOrderDetailList.Add(orderDetail);
                        }
                    }
                }
            }
            if (resultOrderDetailList.Count == 0)
            {
                this.ShowErrorMessage("MasterData.Order.OrderHead.OrderDetail.Required");
                return;
            }
            else
            {
                DateTime winTime = this.tbWinTime.Text.Trim() == string.Empty ? DateTime.Now : DateTime.Parse(this.tbWinTime.Text);
                DateTime startTime = winTime;
                if (this.tbSettleTime.Text.Trim() != string.Empty)
                {
                    orderHead.SettleTime = DateTime.Parse(this.tbSettleTime.Text);
                }

                if (this.tbStartTime.Text != string.Empty)
                {
                    startTime = DateTime.Parse(this.tbStartTime.Text.Trim());
                }
                else
                {
                    double leadTime = this.hfLeadTime.Value == string.Empty ? 0 : double.Parse(this.hfLeadTime.Value);
                    double emTime = this.hfEmTime.Value == string.Empty ? 0 : double.Parse(this.hfEmTime.Value);
                    double lTime = this.cbIsUrgent.Checked ? emTime : leadTime;
                    startTime = winTime.AddHours(0 - lTime);
                }
                if (orderHead.Type == BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION)
                {
                    if (this.ucShift.ShiftCode == string.Empty)
                    {
                        ShowErrorMessage("MasterData.Order.Shift.Empty");
                        return;
                    }
                    orderHead.Shift = TheShiftMgr.LoadShift(this.ucShift.ShiftCode);
                }
                orderHead.OrderDetails = resultOrderDetailList;
                orderHead.WindowTime = winTime;
                orderHead.StartTime = startTime;
                orderHead.IsAutoRelease = this.cbReleaseOrder.Checked;

                if (this.cbIsUrgent.Checked)
                {
                    orderHead.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_URGENT;
                }
                else
                {
                    orderHead.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
                }
                if (this.tbRefOrderNo.Text.Trim() != string.Empty)
                {
                    orderHead.ReferenceOrderNo = this.tbRefOrderNo.Text.Trim();
                }
                if (this.tbExtOrderNo.Text.Trim() != string.Empty)
                {
                    orderHead.ExternalOrderNo = this.tbExtOrderNo.Text.Trim();
                }
            }

            TheOrderMgr.CreateOrder(orderHead, this.CurrentUser);
            if (this.cbPrintOrder.Checked && false)//不要打印
            {
                IList<OrderDetail> orderDetails = orderHead.OrderDetails;
                IList<object> list = new List<object>();
                list.Add(orderHead);
                list.Add(orderDetails);

                IList<OrderLocationTransaction> orderLocationTransactions = TheOrderLocationTransactionMgr.GetOrderLocationTransaction(orderHead.OrderNo);
                list.Add(orderLocationTransactions);
                string printUrl = TheReportMgr.WriteToFile(orderHead.OrderTemplate, list);
                Page.ClientScript.RegisterStartupScript(GetType(), "method", " <script language='javascript' type='text/javascript'>PrintOrder('" + printUrl + "'); </script>");
            }
            this.ShowSuccessMessage("MasterData.Order.OrderHead.AddOrder.Successfully", orderHead.OrderNo);

            //跳转到相应的订单查询一面
            string url = null;
            if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
            {

                url = "Main.aspx?mid=Order.OrderHead.Production__mp--ModuleType-Production_ModuleSubType-Nml_StatusGroupId-4__act--ListAction";
            }
            else if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT
                || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING
                || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_CUSTOMERGOODS)
            {
                url = "Main.aspx?mid=Order.OrderHead.Procurement__mp--ModuleType-Procurement_ModuleSubType-Nml_StatusGroupId-4__act--ListAction";

            }
            else if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION
                || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER)
            {
                url = "Main.aspx?mid=Order.OrderHead.Distribution__mp--ModuleType-Distribution_ModuleSubType-Nml_StatusGroupId-4__act--ListAction";
            }
            else
            {
                return;
            }

            Page.ClientScript.RegisterStartupScript(GetType(), "method",
                " <script language='javascript' type='text/javascript'>timedMsg('" + url + "'); </script>");

        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
        catch (Exception)
        {


        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int columnCount = this.GV_List.Columns.Count;
        if (e.Row.RowType == DataControlRowType.Header && isSupplier)
        {
            for (int i = 6; i < columnCount; i++)
            {
                ((LinkButton)(e.Row.Cells[i].Controls[0])).Enabled = false;
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ScheduleBody body = (ScheduleBody)(e.Row.DataItem);

            e.Row.Cells[0].Text = seq.ToString();
            //LinkButton lbnItem = e.Row.Cells[1].Controls[0] as LinkButton;
            Item item = this.items.Where(i => StringHelper.Eq(i.Code, body.Item)).SingleOrDefault();
            if (item != null)
            {
                e.Row.Cells[2].Text = item.Description;
            }
            //e.Row.Cells[3].Text = this.TheItemReferenceMgr.GetItemReferenceByItem(body.Item, this.PartyCode, null);
            ItemReference itemRef = this.GetItemReference(body.Item, this.PartyCode, null);
            if (itemRef != null)
            {
                e.Row.Cells[3].Text = itemRef.ReferenceCode;
            }

            string lblUom = e.Row.Cells[4].Text;
            string lblUnitCount = e.Row.Cells[5].Text;

            seq++;
            //lbnItem.Text = body.Item;

            if (!isExport && !isSupplier && this.cbDetail.Checked)
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
                    e.Row.Cells[i].Attributes.Add("title", this.GetDetail(body.Item, headerTextTime, lastHeaderTextTime));
                }
            }
            else
            {
                e.Row.Cells[1].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            }
        }
    }

    protected void GV_Order_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ScheduleBody body = (com.Sconit.Entity.MRP.ScheduleBody)(e.Row.DataItem);

            e.Row.Cells[0].Text = seq.ToString();
            e.Row.Cells[2].Text = this.TheItemMgr.LoadItem(body.Item).Description;
            //e.Row.Cells[3].Text = this.TheItemReferenceMgr.GetItemReferenceByItem(body.Item, this.PartyCode, null);
            ((TextBox)e.Row.Cells[9].FindControl("tbQty")).Text = body.Qty0 > body.DisconActQty0 ? (body.Qty0 - body.DisconActQty0).ToString("#.##") : "0";
            seq++;
        }
    }

    protected void GV_Detail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ((Label)e.Row.FindControl("lblSequence")).Text = seq_Detail.ToString();
            seq_Detail++;

            e.Row.Cells[2].Text = this.TheItemMgr.LoadItem(e.Row.Cells[1].Text).Description;
            //e.Row.Cells[3].Text = this.TheItemReferenceMgr.GetItemReferenceByItem(e.Row.Cells[1].Text, this.PartyCode, null);

            if (isExport || true)
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

    //protected void ColItemCode_Click(object sender, EventArgs e)
    //{
    //    this.tbItemCode.Text = this.GV_List.SelectedDataKey.Value.ToString();
    //    this.rblListFormat.SelectedIndex = 1;
    //    this.btnSearch_Click(this.btnSearch, e);
    //}

    protected void GV_List_Sorting(Object sender, GridViewSortEventArgs e)
    {
        if (isSupplier)
        {
            return;
        }
        DataControlFieldCollection dcfc = ((GridView)sender).Columns;
        for (int i = 6; i < dcfc.Count; i++)
        {
            DataControlField dcf = dcfc[i];
            if (dcf.SortExpression == e.SortExpression)
            {
                string qty = "Qty" + (i - 6).ToString();
                string actQty = "ActQty" + (i - 6).ToString();
                string disconActQty = "DisconActQty" + (i - 6).ToString();

                this.ScheduleDate = DateTime.Parse(e.SortExpression);
                //todo wintime or starttime
                this.hfLastScheduleTime.Value = dcf.FooterText;
                DateTime? lastScheduleDate = null;
                if (this.hfLastScheduleTime.Value != string.Empty)
                {
                    lastScheduleDate = DateTime.Parse(this.hfLastScheduleTime.Value);
                }

                IList<MrpShipPlanView> mrpShipPlanViews = TheMrpShipPlanViewMgr.GetMrpShipPlanViews((isFlow ? this.flowOrLoc : null), (!isFlow ? this.flowOrLoc : null), this.itemCode, this.EffDate, null, null);

                DetachedCriteria criteria = DetachedCriteria.For<ExpectTransitInventoryView>();
                criteria.Add(Expression.Eq("EffectiveDate", this.EffDate));
                IList<ExpectTransitInventoryView> transitInventoryViews = this.TheCriteriaMgr.FindAll<ExpectTransitInventoryView>(criteria);

                itemDiscontinueList = this.TheCriteriaMgr.FindAll<ItemDiscontinue>();
                ScheduleView scheduleView = TheMrpShipPlanViewMgr.TransferMrpShipPlanViews2ScheduleView(mrpShipPlanViews, transitInventoryViews, itemDiscontinueList, this.rblFlowOrLoc.SelectedValue, this.rblDateType.SelectedValue);

                foreach (ScheduleBody body in scheduleView.ScheduleBodys)
                {
                    PropertyInfo qtyProp = typeof(ScheduleBody).GetProperty(qty);
                    PropertyInfo actQtyProp = typeof(ScheduleBody).GetProperty(actQty);
                    PropertyInfo disconActQtyProp = typeof(ScheduleBody).GetProperty(disconActQty);

                    body.Qty0 = (decimal)qtyProp.GetValue(body, null);
                    body.ActQty0 = (decimal)actQtyProp.GetValue(body, null);
                    body.DisconActQty0 = (decimal)disconActQtyProp.GetValue(body, null);
                }

                this.GV_Order.DataSource = scheduleView.ScheduleBodys;
                this.GV_Order.DataBind();
                this.fld_Search.Visible = false;
                this.div_OrderDetail.Visible = true;
                this.div_MRP_Detail.Visible = false;
                this.fld_Group.Visible = false;
                if (isFlow)
                {
                    this.tbFlow.Text = this.flowOrLoc;
                    dotbFlow_TextChanged(this.flowOrLoc);
                }
                //this.ucShift.Date = DateTime.Today;
                break;
            }
        }
    }

    protected void CustomersGridView_Sorted(Object sender, EventArgs e)
    {
        // Display the sort expression and sort direction.
    }

    protected void rblFlowOrLoc_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (isFlow)
        {
            this.tbFlowOrLoc.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:true,bool:true,bool:true,bool:true,bool:true,bool:true,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_TO;
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
            // this.itemRefs = TheItemReferenceMgr.GetAllItemReference();
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

            if (this.rblListFormat.SelectedIndex == 1)
            {
                this.GV_List.EnableViewState = false;
                this.GV_Detail.EnableViewState = true;
                this.GV_Order.EnableViewState = false;

                this.mrpPlanTransactions = TheMrpPlanTransactionMgr.GetMrpPlanTransactions((isFlow ? this.flowOrLoc : null), (!isFlow ? this.flowOrLoc : null), this.itemCode, this.EffDate, this.WinDate, this.StartDate);

                this.GV_Detail_DataBind(expectTransitInventories);
                if (button == this.btnExport)
                {
                    this.ExportXLS(this.GV_Detail);
                    this.isExport = true;
                }
            }
            else
            {
                if (false)
                {
                    this.fld.Visible = true;
                    this.fld_Group.Visible = false;
                }
                else
                {
                    this.fld.Visible = false;
                    this.GV_List.EnableViewState = true;
                    this.GV_Detail.EnableViewState = false;
                    this.GV_Order.EnableViewState = false;
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
                    ScheduleView scheduleView = TheMrpShipPlanViewMgr.TransferMrpShipPlanViews2ScheduleView(mrpShipPlanViews, transitInventoryViews, itemDiscontinueList, this.rblFlowOrLoc.SelectedValue, this.rblDateType.SelectedValue);

                    log.Info("TransferMrpShipPlanViews2ScheduleView@" + DateTime.Now);

                    this.GV_List_DataBind(scheduleView);
                    if (button == this.btnExport)
                    {
                        this.ExportXLS(this.GV_List);
                        this.isExport = true;
                    }
                }
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

        this.div_MRP_Detail.Visible = false;
        this.div_OrderDetail.Visible = false;
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
                        bfColumn.SortExpression = isWinTime ? scheduleHead.DateTo.ToString("yyyy-MM-dd") : scheduleHead.DateFrom.ToString("yyyy-MM-dd");
                        bfColumn.FooterText = isWinTime ? (scheduleHead.LastDateTo.HasValue ? scheduleHead.LastDateTo.Value.ToString("yyyy-MM-dd") : string.Empty) : (scheduleHead.LastDateFrom.HasValue ? scheduleHead.LastDateFrom.Value.ToString("yyyy-MM-dd") : string.Empty);
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

    private void GV_Detail_DataBind(IList<ExpectTransitInventory> transitList)
    {
        if (mrpPlanTransactions == null || mrpPlanTransactions.Count == 0)
        {
            this.ltl_MRP_List_Result.Visible = true;
            //this.btnSave.Visible = false;
        }
        else
        {
            if (mrpPlanTransactions.Count > 5000)
            {
                mrpPlanTransactions = mrpPlanTransactions.Take(5000).ToList();
                ShowWarningMessage("Common.Export.Warning.GreatThan5000", mrpPlanTransactions.Count.ToString());
            }
            this.ltl_MRP_List_Result.Visible = false;
            //this.btnSave.Visible = true;
        }
        this.GV_Detail.DataSource = mrpPlanTransactions;
        this.GV_Detail.DataBind();

        this.div_MRP_Detail.Visible = true;
        this.div_OrderDetail.Visible = false;
        this.fld_Group.Visible = false;
    }

    private string GetDetail(string itemCode, DateTime effTime, DateTime? lastHeaderTextTime)
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

            //IList<Tran
            //if (isFlow)
            //{
            //    var q = from inv in transitInventoryViews
            //            where inv.Location == scheduleHead.Location
            //            && inv.Item == scheduleBody.Item
            //            && inv.WindowTime <= scheduleHead.DateTo
            //            && lastDate.HasValue && inv.WindowTime > lastDate.Value
            //            select inv;
            //}
            //else
            //{
            //}
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
                q_Trans = q_Trans.Where(m => m.WindowTime.Date == effTime.Date);
            }
            else
            {
                q_Trans = q_Trans.Where(m => m.StartTime.Date == effTime.Date);
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

                    detail += "<tr><td>Order-</td><td>" + Qty + "</td><td>" + startTime + "</td><td>" + winTime + "</td><td colspan='2'>" + orderNo + "</td></tr>";
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

    protected void tbFlow_TextChanged(Object sender, EventArgs e)
    {
        dotbFlow_TextChanged(tbFlow.Text);
    }

    private void dotbFlow_TextChanged(string flowCode)
    {
        try
        {
            Flow currentFlow = TheFlowMgr.LoadFlow(flowCode, false);
            if (currentFlow != null)
            {
                this.FlowCode = currentFlow.Code;
                this.FlowType = currentFlow.Type;

                this.cbReleaseOrder.Checked = currentFlow.IsAutoRelease;
                this.cbPrintOrder.Checked = currentFlow.NeedPrintOrder;
                if (this.ScheduleDate.HasValue)
                {
                    if (isWinTime)
                    {
                        DateTime winTime = FlowHelper.GetWinTime(currentFlow, this.ScheduleDate.Value);
                        this.tbWinTime.Text = winTime.ToString("yyyy-MM-dd HH:mm");
                        double leadTime = currentFlow.LeadTime.HasValue ? (double)currentFlow.LeadTime.Value : 0;
                        this.tbStartTime.Text = winTime.AddHours(-leadTime).ToString("yyyy-MM-dd HH:mm");
                    }
                    else
                    {
                        double leadTime = currentFlow.LeadTime.HasValue ? (double)currentFlow.LeadTime.Value : 0;
                        DateTime winTime = FlowHelper.GetWinTime(currentFlow, this.ScheduleDate.Value.AddHours(leadTime));
                        this.tbWinTime.Text = winTime.ToString("yyyy-MM-dd HH:mm");
                        this.tbStartTime.Text = winTime.AddHours(-leadTime).ToString("yyyy-MM-dd HH:mm");
                    }
                }

                this.hfLeadTime.Value = currentFlow.LeadTime.ToString();
                this.hfEmTime.Value = currentFlow.EmTime.ToString();

                //  InitDetailParamater(orderHead);

                if (currentFlow.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                {
                    this.ltlShift.Text = "${MasterData.WorkCalendar.Shift}:";
                    this.ltlShift.Visible = true;
                    this.ucShift.Visible = true;
                    //this.tbScheduleTime.Visible = false;
                    this.BindShift(currentFlow);
                }
                else if (!enableDiscon && (currentFlow.Type == BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION
                    || currentFlow.Type == BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_SUBCONCTRACTING
                    || currentFlow.Type == BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PROCUREMENT))
                {
                    this.ltlShift.Visible = true;
                    this.ltlShift.Text = "${MasterData.Order.OrderHead.SettleTime}:";
                    this.ucShift.Visible = false;
                    this.tbSettleTime.Visible = true;
                    //this.tbScheduleTime.Visible = true;
                    this.tbSettleTime.Text = this.tbWinTime.Text;
                }
                else
                {
                    this.ltlShift.Visible = false;
                    this.ucShift.Visible = false;
                    this.tbSettleTime.Visible = false;
                }
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private void BindShift(Flow currentFlow)
    {
        string regionCode = currentFlow != null ? currentFlow.PartyFrom.Code : string.Empty;
        DateTime dateTime = this.tbStartTime.Text.Trim() == string.Empty ? DateTime.Today : DateTime.Parse(this.tbStartTime.Text);
        this.ucShift.BindList(dateTime, regionCode);
    }

    protected void tbWinTime_TextChanged(object sender, EventArgs e)
    {
        if (this.FlowType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
        {
            Flow currentFlow = TheFlowMgr.LoadFlow(this.FlowCode, false);
            this.BindShift(currentFlow);
        }
    }


    private ItemReference GetItemReference(string itemCode, string partyCode1, string partyCode2)
    {
        return null;
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


}

