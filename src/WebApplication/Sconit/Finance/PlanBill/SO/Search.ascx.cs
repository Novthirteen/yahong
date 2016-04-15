using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity.MasterData;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Entity.Exception;

public partial class Finance_PlanBill_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler ConfirmEvent;
    public event EventHandler MatchClick;

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



    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.PageCleanUp();
        }
        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code;
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
        if (actionParameter.ContainsKey("ReceiptNo"))
        {
            this.tbReceiptNo.Text = actionParameter["ReceiptNo"];
        }
        if (actionParameter.ContainsKey("ItemCode"))
        {
            this.tbItemCode.Text = actionParameter["ItemCode"];
        }
        if (actionParameter.ContainsKey("ExternalReceiptNo"))
        {
            this.tbExtReceiptNo.Text = actionParameter["ExternalReceiptNo"];
        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }
    protected override void DoSearch()
    {
        if (SearchEvent != null)
        {
            SearchEvent(GetCriteria(), null);
        }
    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        if (ConfirmEvent != null)
        {
            ConfirmEvent(sender, e);
        }
    }

    protected void btnAutoMatch_Click(object sender, EventArgs e)
    {
        if (MatchClick != null)
        {
            MatchClick(GetCriteria(), null);
        }
    }

    private void PageCleanUp()
    {
        this.tbStartDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
        this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        this.tbPartyCode.Text = string.Empty;
        this.tbReceiptNo.Text = string.Empty;
        this.tbItemCode.Text = string.Empty;

    }

    private DetachedCriteria GetCriteria()
    {
        string partyCode = this.tbPartyCode.Text != string.Empty ? this.tbPartyCode.Text.Trim() : string.Empty;
        string ReceiptNo = this.tbReceiptNo.Text != string.Empty ? this.tbReceiptNo.Text.Trim() : string.Empty;
        string startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        string endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
        string itemCode = this.tbItemCode.Text != string.Empty ? this.tbItemCode.Text.Trim() : string.Empty;
        string externalReceiptNo = this.tbExtReceiptNo.Text != string.Empty ? this.tbExtReceiptNo.Text.Trim() : string.Empty;
        string ipNo = this.tbIpNo.Text.Trim();

        #region DetachedCriteria查询

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(PlannedBill));
        selectCriteria.Add(Expression.Eq("TransactionType", this.ModuleType));
        selectCriteria.Add(Expression.Or(Expression.IsNull("ActingQty"), Expression.GtProperty("PlannedQty", "ActingQty")));

        if (partyCode != string.Empty)
        {
            selectCriteria.CreateAlias("BillAddress", "ba");
            selectCriteria.Add(Expression.Eq("ba.Party.Code", partyCode));
        }
        if (ReceiptNo != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("ReceiptNo", ReceiptNo));
        }
        if (startDate != string.Empty)
        {
            selectCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(startDate)));
        }
        if (endDate != string.Empty)
        {
            selectCriteria.Add(Expression.Le("CreateDate", DateTime.Parse(endDate).AddDays(1)));
        }
        if (itemCode != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Item.Code", itemCode));
        }
        if (externalReceiptNo != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("ExternalReceiptNo", externalReceiptNo));
        }
        if (ipNo != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("IpNo", ipNo));
        }
        #endregion

        return selectCriteria;
    }


    protected void btnRecalculate_Click(object sender, EventArgs e)
    {
        try
        {
            string partyCode = this.tbPartyCode.Text.Trim();
            string startDate = this.tbStartDate.Text.Trim();
            string endDate = this.tbEndDate.Text.Trim();
            string itemCode = this.tbItemCode.Text.Trim();
            string flowCode = this.tbFlow.Text.Trim();

            if (startDate == string.Empty || endDate == string.Empty)
            {
                ShowErrorMessage("Import.Time.Error.Empty");
                return;
            }

            DateTime startTime = DateTime.Parse(startDate);
            DateTime endTime = DateTime.Parse(endDate).AddDays(1);

            this.ThePlannedBillMgr.RecalculatePrice(this.CurrentUser, this.ModuleType, partyCode, flowCode, startTime, endTime, itemCode);
            ShowSuccessMessage("MasterData.ActingBill.Recalculate.Successfully");
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

}
