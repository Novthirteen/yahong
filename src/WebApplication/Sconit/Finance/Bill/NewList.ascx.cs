using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;

public partial class Finance_Bill_NewList : ListModuleBase
{
    public string ModuleType
    {
        get { return (string)ViewState["ModuleType"]; }
        set { ViewState["ModuleType"] = value; }
    }

    private int DecimalLength
    {
        get { return (int)ViewState["DecimalLength"]; }
        set { ViewState["DecimalLength"] = value; }
    }

    public void BindDataSource(IList<ActingBill> actingBillList)
    {
        if (actingBillList == null)
        {
            actingBillList = new List<ActingBill>();
        }
        this.GV_List.DataSource = actingBillList;
        this.UpdateView();
        this.GV_List.Columns[19].Visible = false;
        this.GV_List.Columns[20].Visible = false;
    }

    public IList<ActingBill> PopulateSelectedData()
    {
        return PopulateSelectedData(false);
    }

    private IList<ActingBill> PopulateSelectedData(bool checkedAll)
    {
        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {
            IList<ActingBill> actingBillList = new List<ActingBill>();

            foreach (GridViewRow row in this.GV_List.Rows)
            {
                TextBox tbAmount = row.FindControl("tbAmount") as TextBox;
                CheckBox checkBoxGroup = row.FindControl("CheckBoxGroup") as CheckBox;
                if (checkBoxGroup.Checked || checkedAll)
                {
                    HiddenField hfId = row.FindControl("hfId") as HiddenField;
                    TextBox tbQty = row.FindControl("tbQty") as TextBox;
                    TextBox tbDiscount = row.FindControl("tbDiscount") as TextBox;
                    HiddenField hfUnitPrice = row.FindControl("hfUnitPrice") as HiddenField;

                    int id = int.Parse(hfId.Value);
                    decimal unitprice = decimal.Parse(hfUnitPrice.Value);
                    ActingBill actingBill = TheActingBillMgr.LoadActingBill(id);
                    actingBill.CurrentBillQty = decimal.Parse(tbQty.Text);
                    actingBill.CurrentBillAmount = decimal.Parse(tbAmount.Text);
                    decimal discount = unitprice * actingBill.CurrentBillQty - actingBill.CurrentBillAmount;
                    actingBill.CurrentDiscount = discount;
                    actingBillList.Add(actingBill);
                }
            }
            return actingBillList;
        }
        return null;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (this.ModuleType == BusinessConstants.BILL_TRANS_TYPE_PO)
            {
                //this.GV_List.Columns[3].Visible = false;
            }
            else if (this.ModuleType == BusinessConstants.BILL_TRANS_TYPE_SO)
            {
                this.GV_List.Columns[1].HeaderText = "${MasterData.ActingBill.Customer}";
                this.GV_List.Columns[2].Visible = false;
            }

            EntityPreference entityPreference = TheEntityPreferenceMgr.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_AMOUNT_DECIMAL_LENGTH);
            DecimalLength = int.Parse(entityPreference.Value);
        }
    }

    public override void UpdateView()
    {
        this.GV_List.DataBind();
        if (this.GV_List.DataSource != null)
        {
            this.lblNoRecordFound.Visible = false;

        }
        else
        {
            this.lblNoRecordFound.Visible = true;
        }
        this.GV_List.Columns[0].Visible = true;
        this.GV_List.Columns[16].Visible = false;
        this.GV_List.Columns[17].Visible = false;
        this.GV_List.Columns[18].Visible = false;
    }

    public void ExportXLS()
    {
        this.GV_List.Columns[0].Visible = false;
        this.GV_List.Columns[16].Visible = true;
        this.GV_List.Columns[17].Visible = true;
        this.GV_List.Columns[18].Visible = true;
        this.ExportXLS(GV_List, "Bill.xls");
    }

    protected void GV_List_Sorting(Object sender, GridViewSortEventArgs e)
    {
        IList<ActingBill> actingBills = PopulateSelectedData(true);
        DataControlFieldCollection dcfc = ((GridView)sender).Columns;
        for (int i = 1; i < 5; i++)
        {
            DataControlField dcf = dcfc[i];
            if (dcf.SortExpression == e.SortExpression)
            {
                if (i == 1)//party
                {
                    actingBills = actingBills.OrderBy(a => a.BillAddress.Party.Code).ToList();
                }
                else if (i == 2)//ReceiptNo
                {
                    actingBills = actingBills.OrderBy(a => a.ReceiptNo).ToList();
                }
                else if (i == 3)//ExternalReceiptNo
                {
                    actingBills = actingBills.OrderBy(a => a.ExternalReceiptNo).ToList();
                }
                else if (i == 4)//ItemCode
                {
                    actingBills = actingBills.OrderBy(a => a.Item.Code).ToList();
                }
                else
                {
                    return;
                }
            }
        }
        this.BindDataSource(actingBills);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ActingBill actingBill = (ActingBill)e.Row.DataItem;

            decimal billAmount = actingBill.BillAmount;
            decimal unitPrice = actingBill.UnitPrice;

            decimal remailQty = actingBill.BillQty - actingBill.BilledQty;
            decimal remailAmount = actingBill.CurrentBillQty * actingBill.UnitPrice;//billAmount - actingBill.BilledAmount;
            decimal discount = unitPrice * remailQty - remailAmount;

            TextBox tbQty = e.Row.FindControl("tbQty") as TextBox;
            TextBox tbDiscountRate = e.Row.FindControl("tbDiscountRate") as TextBox;
            TextBox tbDiscount = e.Row.FindControl("tbDiscount") as TextBox;
            TextBox tbAmount = e.Row.FindControl("tbAmount") as TextBox;
            Literal ltlAmount = e.Row.FindControl("ltlAmount") as Literal;
            Literal ltlQty = e.Row.FindControl("ltlQty") as Literal;
            Label lblIsProvisionalEstimate = e.Row.FindControl("lblIsProvisionalEstimate") as Label;

            if (actingBill.IsProvisionalEstimate)
            {
                lblIsProvisionalEstimate.Text = "暂估";
            }
            if (actingBill.BilledQty == actingBill.BillQty)
            {
                lblIsProvisionalEstimate.Text = "补开";
            }

            if (IsExport)
            {
                tbQty.Visible = false;
                tbDiscountRate.Visible = false;
                tbDiscount.Visible = false;
                tbAmount.Visible = false;
                ltlQty.Visible = true;
                ltlAmount.Visible = true;

                ltlQty.Text = remailQty.ToString("0.########");
                ltlAmount.Text = remailAmount.ToString("0.########");
                e.Row.Cells[3].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
                e.Row.Cells[4].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
                e.Row.Cells[6].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            }
            else
            {
                tbQty.Visible = true;
                tbDiscountRate.Visible = true;
                tbDiscount.Visible = true;
                tbAmount.Visible = true;
                ltlQty.Visible = false;
                ltlAmount.Visible = false;

                //tbQty.Text = remailQty.ToString("0.########");
                if (unitPrice != 0 && remailQty != 0)
                {
                    tbDiscountRate.Text = (Math.Round(discount / (unitPrice * remailQty), this.DecimalLength, MidpointRounding.AwayFromZero) * 100).ToString("F2");
                }
                tbDiscount.Text = discount.ToString("0.########");
                tbAmount.Text = remailAmount.ToString("0.########");
                tbAmount.Attributes["oldValue"] = tbAmount.Text;
            }
        }
    }


    public void BindDataSourceNotMatch(IList<ActingBill> actingBillList)
    {
        if (actingBillList == null)
        {
            actingBillList = new List<ActingBill>();
        }
        this.GV_NotMatch.DataSource = actingBillList;
        this.GV_NotMatch.DataBind();
        //this.UpdateView();
        this.GV_List.Columns[19].Visible = true;
        this.GV_List.Columns[20].Visible = true;
    }
}
