using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;

public partial class Order_OrderHead_OrderImport_Import2 : ModuleBase
{
    public event EventHandler ImportEvent;
    public event EventHandler BtnBackClick;

    private string companyCode
    {
        get { return (string)ViewState["companyCode"]; }
        set { ViewState["companyCode"] = value; }
    }

    public string ModuleType
    {
        get { return (string)ViewState["ModuleType"]; }
        set { ViewState["ModuleType"] = value; }
    }

    private DateTime StartDate
    {
        get { return this.tbStartDate.Text == string.Empty ? DateTime.Now : DateTime.Parse(this.tbStartDate.Text); }
        set { this.tbStartDate.Text = value.ToString("yyyy-MM-dd"); }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.tbStartDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            this.tbSettleTime.Text = DateTime.Today.ToString("yyyy-MM-dd");
            if (this.ModuleType == BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION)
            {
                this.rblDateType.SelectedIndex = 1;
            }
        }
        this.InitialUI();
    }

    public void Create(object sender)
    {
        try
        {
            IList<OrderHead> orderHeadList = (IList<OrderHead>)sender;
            foreach (OrderHead orderHead in orderHeadList)
            {
                orderHead.SettleTime = DateTime.Parse(this.tbSettleTime.Text.Trim());
            }
            if (orderHeadList != null && orderHeadList.Count > 0)
            {
                TheOrderMgr.CreateOrder(orderHeadList, this.CurrentUser.Code);
                ShowSuccessMessage("Common.Business.Result.Insert.Successfully");
            }
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage(ex);
        }
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        this.Import();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BtnBackClick != null)
        {
            BtnBackClick(null, null);
        }
    }

    private void Import()
    {
        try
        {
            if (this.tbSettleTime.Text.Trim() == string.Empty)
            {
                ShowErrorMessage("Import.SettleTime.Error.Empty");
                return;
            }
            if (this.tbStartDate.Text.Trim() == string.Empty)
            {
                ShowErrorMessage("Import.Time.Error.Empty");
                return;
            }

            bool isWinTime = this.rblDateType.SelectedIndex == 0;
            string flowCode = this.tbFlow.Text.Trim();
            string timePeriodType = BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_DAY;

            IList<FlowPlan> flowPlanList = new List<FlowPlan>();

            if (this.rblListFormat.SelectedIndex == 2)
            {
                flowPlanList = TheImportMgr.ReadShipSchedulePanaFromXls(fileUpload.PostedFile.InputStream, this.CurrentUser, this.StartDate, this.StartDate, flowCode, this.cbItemRef.Checked);
                flowPlanList = flowPlanList.Where(f => f.PlanQty > 0).ToList();
            }
            else if (this.rblListFormat.SelectedIndex == 1)
            {
                flowPlanList = TheImportMgr.ReadScheduleFromXls(fileUpload.PostedFile.InputStream, this.CurrentUser, this.ModuleType, flowCode, this.cbItemRef.Checked, this.StartDate);
            }
            else if (this.rblListFormat.SelectedIndex == 3)
            {
                flowPlanList = TheImportMgr.ReadNewPanaOrderFromCSV(fileUpload.PostedFile.InputStream, this.CurrentUser, flowCode, this.cbItemRef.Checked, this.StartDate, this.StartDate);
            }
            else
            {
                flowPlanList = TheImportMgr.ReadShipScheduleYFKFromXls(fileUpload.PostedFile.InputStream, this.CurrentUser, this.ModuleType, flowCode, timePeriodType, this.StartDate);
            }

            IList<OrderHead> ohList = new List<OrderHead>();
            if (this.rblListFormat.SelectedIndex == 2 || this.rblListFormat.SelectedIndex == 3)
            {
                ohList = TheOrderMgr.ConvertFlowPlanToOrdersPana(flowPlanList);
            }
            else
            {
                ohList = TheOrderMgr.ConvertFlowPlanToOrders(flowPlanList, isWinTime);
            }

            ohList = ohList.Where(o => o.OrderDetails != null && o.OrderDetails.Count > 0).ToList();
            foreach (OrderHead oh in ohList)
            {
                oh.OrderDetails = oh.OrderDetails.OrderBy(o => o.Sequence).ThenBy(o => o.Item.Code).ToList();
                if (this.tbSettleTime.Text.Trim() != string.Empty)
                {
                    try
                    {
                        oh.SettleTime = DateTime.Parse(this.tbSettleTime.Text.Trim());
                    }
                    catch (Exception)
                    {
                        ShowErrorMessage("Import.SettleTime.Error.Empty");
                        return;
                    }
                }
            }

            if (ImportEvent != null)
            {
                ImportEvent(new object[] { ohList }, null);
            }
            ShowSuccessMessage("Import.Result.Successfully");
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage(ex);
        }
    }

    private void InitialUI()
    {
        this.tbStartDate.Attributes["onchange"] += "setStartTime();";
        if (ModuleType == BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION)
        {
            this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:false,bool:true,bool:true,bool:false,bool:false,bool:false,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_FROM;

            this.hlTemplate1.NavigateUrl = "~/Reports/Templates/ImportTemplates/DistributionOrder1.xls";
            this.hlTemplate2.NavigateUrl = "~/Reports/Templates/ImportTemplates/DistributionOrder2.xls";
            this.hlTemplate3.NavigateUrl = "~/Reports/Templates/ImportTemplates/DistributionOrder3.xls";
            this.hlTemplate3.Visible = true;
            this.rblListFormat.Items[2].Enabled = true;
        }
        else if (ModuleType == BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PROCUREMENT)
        {
            this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:true,bool:false,bool:true,bool:false,bool:true,bool:true,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_TO;

            this.hlTemplate1.NavigateUrl = "~/Reports/Templates/ImportTemplates/ProcurementOrder1.xls";
            this.hlTemplate2.NavigateUrl = "~/Reports/Templates/ImportTemplates/ProcurementOrder2.xls";
            this.hlTemplate3.Visible = false;
            this.rblListFormat.Items[2].Enabled = false;
        }
        else //todo
        {
            this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:false,bool:true,bool:true,bool:false,bool:false,bool:false,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_FROM;
            this.hlTemplate1.Enabled = false;
            this.hlTemplate2.Enabled = false;
            this.hlTemplate3.Visible = false;
            this.rblListFormat.Visible = false;
        }
    }

    protected void rblListFormat_IndexChanged(object sender, EventArgs e)
    {
        if (this.rblListFormat.SelectedIndex == 3)
        {
            this.cbItemRef.Checked = true;
            //this.tbEndDate.Visible = true;
        }
        else
        {
            this.cbItemRef.Checked = false;
            //this.tbEndDate.Visible = true;
        }

        //if (this.rblListFormat.SelectedIndex == 2)
        //{
        //    this.ltlEndDate.Visible = true;
        //    //this.tbEndDate.Visible = true;
        //}
        //else
        //{
        //    this.ltlEndDate.Visible = false;
        //    //this.tbEndDate.Visible = false;
        //}
    }
}
