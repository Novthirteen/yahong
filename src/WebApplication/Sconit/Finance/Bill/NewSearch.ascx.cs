using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using NHibernate.Expression;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;

public partial class Finance_Bill_NewSearch : SearchModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    public string ModuleType
    {
        get
        {
            return (string)ViewState["ModuleType"];
        }
        set
        {
            ViewState["ModuleType"] = value;
        }
    }

    private string billNo
    {
        get
        {
            return (string)ViewState["billNo"];
        }
        set
        {
            ViewState["billNo"] = value;
        }
    }

    public void InitPageParameter(bool isPopup, Bill bill)
    {
        if (isPopup)
        {
            this.billNo = bill.BillNo;
            this.tbPartyCode.Visible = false;
            this.ltlParty.Text = bill.BillAddress.Party.Name;
            this.ltlParty.Visible = true;
            this.IsRelease.Visible = false;
            this.btnConfirm.Visible = false;
            this.btnBack.Visible = false;
            this.btnAddDetail.Visible = true;
            this.btnClose.Visible = true;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.ucNewList.ModuleType = this.ModuleType;
            this.PageCleanUp();
            string companyCode = TheEntityPreferenceMgr.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANY_CODE).Value;
            if (companyCode != "ChunShen")
            {
                this.cbZS.Visible = false;
                this.cbGS.Visible = false;
            }
        }

        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code;
        if (this.ModuleType == BusinessConstants.BILL_TRANS_TYPE_SO)
        {
            this.ltlPartyCode.Text = "${MasterData.ActingBill.Customer}:";
            this.ltlReceiver.Text = "${MasterData.ActingBill.ExternalReceiptNo}:";

            this.tbPartyCode.ServicePath = "CustomerMgr.service";
            this.tbPartyCode.ServiceMethod = "GetAllCustomer";
            this.tbBillAddress.ServiceParameter = "bool:false";
            this.tbFlow.ServiceMethod = "GetDistributionFlow";
        }
        else
        {
            this.tbBillAddress.ServiceParameter = "bool:true";
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        this.ucNewList.IsExport = false;
        if (btn == this.btnExport)
        {
            this.ucNewList.IsExport = true;
            DoSearch();
            this.ucNewList.ExportXLS();
        }
        else
        {
            DoSearch();
        }
    }

    protected override void DoSearch()
    {
        string partyCode = this.tbPartyCode.Text != string.Empty ? this.tbPartyCode.Text.Trim() : string.Empty;
        string receiver = this.tbReceiver.Text != string.Empty ? this.tbReceiver.Text.Trim() : string.Empty;
        string startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        string endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
        string itemCode = this.tbItemCode.Text != string.Empty ? this.tbItemCode.Text.Trim() : string.Empty;
        string currency = this.tbCurrency.Text != string.Empty ? this.tbCurrency.Text.Trim() : string.Empty;
        string flowCode = this.tbFlow.Text != string.Empty ? this.tbFlow.Text.Trim() : null;
        string billAddress = this.tbBillAddress.Text != string.Empty ? this.tbBillAddress.Text.Trim() : null;
        bool isOrderByItem = this.cbOrderByItem.Checked;

        DateTime? effDateFrom = null;
        if (startDate != string.Empty)
        {
            effDateFrom = DateTime.Parse(startDate);
        }

        DateTime? effDateTo = null;
        if (endDate != string.Empty)
        {
            effDateTo = DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1);
        }

        bool needRecalculate = bool.Parse(TheEntityPreferenceMgr.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_RECALCULATE_WHEN_BILL).Value);
        bool isCreateTime = (this.rblDateType.SelectedIndex == 0);
        if (needRecalculate)
        {
            IList<ActingBill> allactingBillList = TheActingBillMgr.GetActingBill(partyCode, receiver, effDateFrom, effDateTo, itemCode, currency, this.ModuleType, this.billNo, null, flowCode, billAddress, isCreateTime);
            TheActingBillMgr.RecalculatePrice(allactingBillList, this.CurrentUser);
        }
        IList<ActingBill> actingBillList = TheActingBillMgr.GetActingBill(partyCode, receiver, effDateFrom, effDateTo, itemCode, currency, this.ModuleType, this.billNo, null, flowCode, billAddress, isCreateTime);
        //actingBillList = actingBillList.OrderBy(b => b.ExternalReceiptNo).ToList();
        if (isOrderByItem)
        {
            actingBillList = actingBillList.OrderBy(b => b.Item.Code).ToList();
        }
        if (actingBillList != null)
        {
            foreach (ActingBill bill in actingBillList)
            {
                bill.CurrentBillQty = bill.BillQty - bill.BilledQty;
            }
        }
        this.ucNewList.BindDataSource(actingBillList != null && actingBillList.Count > 0 ? actingBillList : null);
        this.ucNewList.Visible = true;
    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        try
        {
            IList<ActingBill> actingBillList = this.ucNewList.PopulateSelectedData();
            IList<Bill> billList = TheBillMgr.CreateBill(actingBillList, this.CurrentUser);
            this.ShowSuccessMessage("MasterData.Bill.CreateSuccessfully", billList[0].BillNo);

            if (this.IsRelease.Checked)
            {
                TheBillMgr.ReleaseBill(billList[0].BillNo, this.CurrentUser);
                this.ShowSuccessMessage("MasterData.Bill.ReleaseSuccessfully", billList[0].BillNo);
            }
            this.PageCleanUp();
            CreateEvent(billList[0].BillNo, null);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        BackEvent(this, null);
    }

    protected void btnAddDetail_Click(object sender, EventArgs e)
    {
        try
        {
            IList<ActingBill> actingBillList = this.ucNewList.PopulateSelectedData();
            this.TheBillMgr.AddBillDetail(this.billNo, actingBillList, this.CurrentUser);
            this.ShowSuccessMessage("MasterData.Bill.AddBillDetailSuccessfully");
            BackEvent(this, null);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnNamedQuery_Click(object sender, EventArgs e)
    {
        IDictionary<string, string> actionParameter = new Dictionary<string, string>();
        if (this.tbStartDate.Text != string.Empty)
        {
            actionParameter.Add("StartDate", this.tbStartDate.Text);
        }
        if (this.tbEndDate.Text != string.Empty)
        {
            actionParameter.Add("EndDate", this.tbEndDate.Text);
        }
        if (this.tbPartyCode.Text != string.Empty)
        {
            actionParameter.Add("PartyCode", this.tbPartyCode.Text);
        }
        if (this.tbReceiver.Text != string.Empty)
        {
            actionParameter.Add("Receiver", this.tbReceiver.Text);
        }
        if (this.tbItemCode.Text != string.Empty)
        {
            actionParameter.Add("ItemCode", this.tbItemCode.Text);
        }
        if (this.tbCurrency.Text != string.Empty)
        {
            actionParameter.Add("Currency", this.tbCurrency.Text);
        }

        //this.SaveNamedQuery(this.tbNamedQuery.Text, actionParameter);
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        if (actionParameter.ContainsKey("StartDate"))
        {
            this.tbStartDate.Text = actionParameter["StartDate"];
        }
        if (actionParameter.ContainsKey("EndDate"))
        {
            this.tbEndDate.Text = actionParameter["EndDate"];
        }
        if (actionParameter.ContainsKey("PartyCode"))
        {
            this.tbPartyCode.Text = actionParameter["PartyCode"];
        }
        if (actionParameter.ContainsKey("Receiver"))
        {
            this.tbReceiver.Text = actionParameter["Receiver"];
        }
        if (actionParameter.ContainsKey("ItemCode"))
        {
            this.tbItemCode.Text = actionParameter["ItemCode"];
        }
        if (actionParameter.ContainsKey("Currency"))
        {
            this.tbCurrency.Text = actionParameter["Currency"];
        }
    }

    private void PageCleanUp()
    {
        this.tbStartDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
        this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        this.tbPartyCode.Text = string.Empty;
        this.tbReceiver.Text = string.Empty;
        this.tbItemCode.Text = string.Empty;
        this.tbCurrency.Text = string.Empty;

        this.ucNewList.Visible = false;
    }


    protected void btnImport_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.tbPartyCode.Text))
        {
            ShowErrorMessage("客户或供应商不能为空");
            return;
        }
        IList<ActingBill> actingBills = TheImportMgr.ReadActingBillFromXls1(fileUpload.PostedFile.InputStream);

        string partyCode = this.tbPartyCode.Text != string.Empty ? this.tbPartyCode.Text.Trim() : string.Empty;
        string receiver = this.tbReceiver.Text != string.Empty ? this.tbReceiver.Text.Trim() : string.Empty;
        string startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        string endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
        string itemCode = this.tbItemCode.Text != string.Empty ? this.tbItemCode.Text.Trim() : string.Empty;
        string currency = this.tbCurrency.Text != string.Empty ? this.tbCurrency.Text.Trim() : string.Empty;
        string flowCode = this.tbFlow.Text != string.Empty ? this.tbFlow.Text.Trim() : null;
        string billAddress = this.tbBillAddress.Text != string.Empty ? this.tbBillAddress.Text.Trim() : null;
        bool isOrderByItem = this.cbOrderByItem.Checked;

        DateTime? effDateFrom = null;
        if (startDate != string.Empty)
        {
            effDateFrom = DateTime.Parse(startDate);
        }

        DateTime? effDateTo = null;
        if (endDate != string.Empty)
        {
            effDateTo = DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1);
        }
        bool isCreateTime = (this.rblDateType.SelectedIndex == 0);
        IList<ActingBill> actingBillList = TheActingBillMgr.GetActingBill(partyCode, receiver, effDateFrom, effDateTo, itemCode, currency, this.ModuleType, this.billNo, null, flowCode, billAddress, isCreateTime);

        if (actingBillList == null)
        {
            ShowErrorMessage("没有待结算明细");
            return;
        }

        IList<ActingBill> notMatchBill = new List<ActingBill>();
        foreach (ActingBill actingBill1 in actingBills)
        {
            if (string.IsNullOrEmpty(actingBill1.ErrorMessage))
            {
                foreach (ActingBill actingBill2 in actingBillList)
                {
                    if (actingBill1.CurrentBillQty > 0 && actingBill2.Qty > actingBill2.CurrentBillQty)
                    {
                        if (actingBill1.Item.Code == actingBill2.Item.Code)
                        {
                            if (!string.IsNullOrEmpty(actingBill1.ReceiptNo))
                            {
                                if (actingBill1.ReceiptNo != actingBill2.ReceiptNo)
                                {
                                    continue;
                                }
                            }
                            if (!string.IsNullOrEmpty(actingBill1.ExternalReceiptNo))
                            {
                                if (actingBill1.ExternalReceiptNo != actingBill2.ExternalReceiptNo)
                                {
                                    continue;
                                }
                            }
                            actingBill2.WarningMessage = actingBill2.WarningMessage == null ? string.Empty : actingBill2.WarningMessage;
                            if (actingBill1.UnitPrice != actingBill2.UnitPrice)
                            {
                                actingBill2.WarningMessage = "单价不一致";
                            }
                            if (actingBill1.EffectiveDate != actingBill2.EffectiveDate)
                            {
                                actingBill2.WarningMessage += " 日期不一致";
                            }

                            if (actingBill1.CurrentBillQty <= actingBill2.Qty)
                            {
                                actingBill2.CurrentBillQty += actingBill1.CurrentBillQty;
                                actingBill1.CurrentBillQty = 0;
                                actingBill2.RowIndex = actingBill2.RowIndex + " " + actingBill1.RowIndex;
                            }
                            else
                            {
                                actingBill2.CurrentBillQty += actingBill2.Qty;
                                actingBill1.CurrentBillQty = actingBill1.CurrentBillQty - actingBill2.Qty;
                                actingBill2.RowIndex = actingBill2.RowIndex + " " + actingBill1.RowIndex;
                            }
                        }
                    }
                }

                if (actingBill1.CurrentBillQty > 0)
                {
                    notMatchBill.Add(actingBill1);
                }
            }
            else
            {
                actingBill1.ErrorMessage = "未能匹配";
                notMatchBill.Add(actingBill1);
            }
        }

        IList<ActingBill> newActingBill = actingBillList.Where(a => a.CurrentBillQty > 0).ToList();
        this.ucNewList.BindDataSource(newActingBill != null && newActingBill.Count > 0 ? newActingBill : null);
        this.ucNewList.BindDataSourceNotMatch(notMatchBill != null && notMatchBill.Count > 0 ? notMatchBill : null);

        this.ucNewList.Visible = true;
    }

}
