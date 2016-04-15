using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity.Procurement;
using NHibernate.Expression;
using com.Sconit.Entity.MRP;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Entity;

public partial class MRP_Schedule_DmdScheduleRouting_Main : MainModuleBase
{
    private SupplyChain supplyChain;
    private IList<SupplyChainDetail> supplyChainDetailList;
    private IList<ExpectTransitInventory> expectTransitInventories { get; set; }
    private IList<ItemDiscontinue> itemDiscontinueList { get; set; }

    private DateTime EffDate
    {
        get
        {
            return DateTime.Parse(this.ddlDate.SelectedValue.Trim());
        }
    }

    private bool isWinTime
    {
        get { return this.rblDateType.SelectedIndex == 1; }
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

    private string flowCode
    {
        get { return this.tbFlow.Text.Trim(); }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code;
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
        string itemCode = this.tbItemCode.Text.Trim();
        Flow flow = TheFlowMgr.LoadFlow(this.flowCode);
        if (flow == null)
        {
            ShowErrorMessage("Common.Business.Warn.FlowInvalid");
            return;
        }
        SecurityHelper.CheckPermission(flow.Type, flow.PartyFrom.Code, flow.PartyTo.Code, this.CurrentUser);

        if (TheItemMgr.LoadItem(itemCode) == null)
        {
            ShowErrorMessage("Common.Business.Warn.ItemInvalid");
            return;
        }

        if (this.tbScheduleTime.Text.Trim() == String.Empty)
        {
            ShowErrorMessage("Import.Time.Error.Empty");
            return;
        }

        DetachedCriteria criteria = DetachedCriteria.For<ExpectTransitInventory>();
        criteria.Add(Expression.Eq("EffectiveDate", this.EffDate));

        expectTransitInventories = this.TheCriteriaMgr.FindAll<ExpectTransitInventory>(criteria);
        itemDiscontinueList = this.TheCriteriaMgr.FindAll<ItemDiscontinue>();

        IList<SupplyChain> supplyChains = TheSupplyChainMgr.GenerateSupplyChainUp(flowCode, itemCode);
        if (supplyChains == null || supplyChains.Count == 0)
        {
            return;
        }
        supplyChain = supplyChains[0];
        supplyChainDetailList = supplyChain.SupplyChainDetails;

        if (supplyChainDetailList != null && supplyChainDetailList.Count > 0)
        {
            SupplyChainDetail supplyChainDetail = supplyChainDetailList[0];
            com.Sconit.Control.MyOrgNode RootNode = GetOrgNode(supplyChainDetail);
            if (RootNode != null)
            {
                createChildNode(RootNode, supplyChainDetail.Id);
                OrgChartTreeView.Node = RootNode;
            }
        }
        this.fld.Visible = true;
    }

    private void createChildNode(com.Sconit.Control.MyOrgNode parentnode, int Id)
    {
        foreach (SupplyChainDetail supplyChainDetail in supplyChainDetailList)
        {
            if (supplyChainDetail.ParentId == Id)
            {
                com.Sconit.Control.MyOrgNode subOrgNode = GetOrgNode(supplyChainDetail);
                if (subOrgNode != null)
                {
                    parentnode.Nodes.Add(subOrgNode);
                    createChildNode(subOrgNode, supplyChainDetail.Id);
                }
            }
        }
    }

    private com.Sconit.Control.MyOrgNode GetOrgNode(SupplyChainDetail supplyChainDetail)
    {
        string locationCode = supplyChainDetail.LocationTo == null ? string.Empty : supplyChainDetail.LocationTo.Code;
        string locationName = supplyChainDetail.LocationTo == null ? string.Empty : supplyChainDetail.LocationTo.Name;
        string itemCode = supplyChainDetail.FlowDetail.Item.Code;
        string flowCode = supplyChainDetail.Flow.Code;
        DateTime effDate = DateTime.Parse(this.tbScheduleTime.Text.Trim()).AddHours(-supplyChainDetail.LeadTime);

        com.Sconit.Control.MyOrgNode orgNode = new com.Sconit.Control.MyOrgNode();
        orgNode.Code = supplyChainDetail.Flow.PartyTo.Code;
        orgNode.Code = orgNode.Code + "  " + locationCode;
        orgNode.CodeTooltip = "Party:" + supplyChainDetail.Flow.PartyTo.Name + " Loc:" + locationName;
        orgNode.Memo1 = itemCode;
        orgNode.Memo1Tooltip = supplyChainDetail.FlowDetail.Item.Description;
        orgNode.Name = supplyChainDetail.Flow.Description;
        orgNode.NameTooltip = supplyChainDetail.Flow.Code;
        decimal safeQty = 0;
        decimal currentQty = 0;
        if (supplyChainDetail.LocationTo != null)
        {
            MrpLocationLotDetail loc = TheMrpLocationLotDetailMgr.LoadMrpLocationLotDetail(locationCode, itemCode, this.EffDate);

            if (loc != null)
            {
                safeQty = loc.SafeQty;
                currentQty = loc.StaticQty;
                orgNode.Memo2 = "S:" + safeQty.ToString("0.####") + " C:" + currentQty.ToString("0.####");
            }
        }
        orgNode.Title = GetDetail(itemCode, locationCode, flowCode, effDate, safeQty, currentQty);
        return orgNode;
    }

    private string GetDetail(string itemCode, string LocationCode, string flowCode, DateTime effDate, decimal safeQty, decimal currentQty)
    {
        DetachedCriteria criteria = DetachedCriteria.For(typeof(MrpPlanTransaction));
        criteria.Add(Expression.Eq("Item", itemCode));
        if (LocationCode != null && LocationCode != string.Empty)
        {
            criteria.Add(Expression.Eq("Location", LocationCode));
        }
        if (flowCode != null && flowCode != string.Empty)
        {
            criteria.Add(Expression.Eq("Flow", flowCode));
        }
        criteria.Add(Expression.Eq("EffectiveDate", this.EffDate));
        criteria.Add(Expression.Not(Expression.Eq("Qty", 0M)));
        //if (isWinTime)
        //{
        //    criteria.Add(Expression.Le("WindowTime", effDate));
        //}
        //else
        //{
        //    criteria.Add(Expression.Le("StartTime", effDate));
        //}
        IList<MrpPlanTransaction> mrpPlanTransactions = this.TheCriteriaMgr.FindAll<MrpPlanTransaction>(criteria);

        mrpPlanTransactions = mrpPlanTransactions.OrderBy(m => m.WindowTime).ThenBy(m => m.Item).ThenBy(m => m.Id).ToList();

        string detail = string.Empty;
        if (mrpPlanTransactions != null)
        {
            IList<ExpectTransitInventory> expectTransitInventoryList = new List<ExpectTransitInventory>();
            IList<ExpectTransitInventory> disconExpectTransitInventoryList = new List<ExpectTransitInventory>();
            if (this.expectTransitInventories != null && expectTransitInventories.Count > 0)
            {
                if (isWinTime)
                {
                    var p = from inv in this.expectTransitInventories
                            where inv.Flow == this.flowCode
                            && inv.Location == LocationCode
                            && inv.Item == itemCode
                            //&& inv.WindowTime <= effDate
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
                                where inv.Flow == this.flowCode
                                && discon.Item.Code == itemCode
                                && inv.WindowTime <= effDate
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
                            where inv.Flow == this.flowCode
                            && inv.Location == LocationCode
                            && inv.Item == itemCode
                            //&& inv.StartTime <= effDate
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
                                where inv.Flow == this.flowCode
                                && discon.Item.Code == itemCode
                                && inv.StartTime <= effDate
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

            if (mrpPlanTransactions.Count() > 0 || expectTransitInventoryList.Count > 0 || disconExpectTransitInventoryList.Count > 0)
            {
                decimal totalQty = mrpPlanTransactions.Sum(m => m.Qty) + disconExpectTransitInventoryList.Sum(d => d.TransitQty);
                detail += "cssbody=[obbd] cssheader=[obhd] header=[净需求: " + totalQty.ToString("#,##0.##")
                    + " 安全库存:" + safeQty.ToString("#,##0.##") + " 当前库存:" + currentQty.ToString("#,##0.##")
                    + "] body=[<table width=100%>";

                foreach (MrpPlanTransaction mrpPlanTransaction in mrpPlanTransactions)
                {
                    string Qty = mrpPlanTransaction.Qty.ToString("#,##0.##");
                    string startTime = mrpPlanTransaction.StartTime.ToString("MM-dd");
                    string winTime = mrpPlanTransaction.WindowTime.ToString("MM-dd");
                    string periodType = mrpPlanTransaction.PeriodType;
                    string sourceType = mrpPlanTransaction.SourceType;
                    if (sourceType== BusinessConstants.CODE_MASTER_MRP_SOURCE_TYPE_VALUE_ORDER)
                    {
                        sourceType = mrpPlanTransaction.Reference;
                    }

                    detail += "<tr><td>${MRP.Schedule.Demand}</td><td>" + Qty + "</td><td>" + startTime + "</td><td>" + winTime + "</td><td>"
                        + periodType + "</td><td>" + sourceType + "</td></tr>";
                }

                foreach (ExpectTransitInventory expectTransitInventory in expectTransitInventoryList)
                {
                    string orderNo = expectTransitInventory.OrderNo;
                    string Qty = expectTransitInventory.TransitQty.ToString("#,##0.##");
                    string startTime = expectTransitInventory.StartTime.ToString("MM-dd");
                    string winTime = expectTransitInventory.WindowTime.ToString("MM-dd");

                    detail += "<tr><td>订单-</td><td>" + Qty + "</td><td>" + startTime + "</td><td>" + winTime + "</td><td colspan=2>" + orderNo
                        + "</td></tr>";
                }

                foreach (ExpectTransitInventory expectTransitInventory in disconExpectTransitInventoryList)
                {
                    string item = expectTransitInventory.Item;
                    string orderNo = expectTransitInventory.OrderNo;
                    string Qty = expectTransitInventory.TransitQty.ToString("#,##0.##");
                    string startTime = expectTransitInventory.StartTime.ToString("MM-dd");
                    string winTime = expectTransitInventory.WindowTime.ToString("MM-dd");

                    detail += "<tr><td>${MRP.Schedule.Discon}</td><td>" + Qty + "</td><td>" + startTime + "</td><td>" + winTime + "</td><td>" + orderNo
                        + "</td><td>" + item + "</td></tr>";
                }
                detail += "</table>]";
            }
        }
        return detail;
    }
}
