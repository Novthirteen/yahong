using System;
using System.Collections;
using System.Collections.Generic;
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
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Web;
using NHibernate.Expression;


public partial class Cost_MiscOrder_MiscOrder_NewQty : EditModuleBase
{
    public event EventHandler BackEvent;

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

    public MiscOrder MiscOrder
    {
        get
        {
            return (MiscOrder)ViewState["MiscOrder"];
        }
        set
        {
            ViewState["MiscOrder"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbMiscOrderRegion.ServiceParameter = "string:" + this.CurrentUser.Code;

        if (!this.IsPostBack)
        {
            this.MiscOrder = new MiscOrder();
            List<CodeMaster> codeMstrs = new List<CodeMaster>();
            codeMstrs.AddRange(TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_STOCK_OUT_REASON).ToList());
            codeMstrs.AddRange(TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_STOCK_IN_REASON).ToList());
            this.ddlReason.DataSource = codeMstrs;
            this.ddlReason.DataBind();

            this.ddlCostElement.DataSource = TheCostElementMgr.GetAllCostElement();
            this.ddlCostElement.DataBind();

            this.ddlCostGroup.DataSource = TheCostGroupMgr.GetAllCostGroup();
            this.ddlCostGroup.DataBind();

        }
    }

    protected void MiscOrderDetailsGV_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            MiscOrderDetail orderDetail = (MiscOrderDetail)e.Row.DataItem;
            TextBox tbCost = ((TextBox)e.Row.FindControl("tbCost"));
            tbCost.Text = "0";
            if (!orderDetail.IsBlankDetail)
            {
                HiddenField hdfCost = (HiddenField)e.Row.FindControl("hdfCost");
                hdfCost.Value = "0";
                decimal? cost = this.TheCostDetailMgr.CalculateItemUnitCost(orderDetail.Item.Code, orderDetail.MiscOrder.CostGroup,
                    orderDetail.MiscOrder.EffectiveDate.Year, orderDetail.MiscOrder.EffectiveDate.Month - 1);
                if (cost.HasValue)
                {
                    hdfCost.Value = cost.Value.ToString("0.########");
                    tbCost.Text = (cost.Value * orderDetail.Qty).ToString("0.########");
                }
            }

            if (this.MiscOrder.OrderNo != null && this.MiscOrder.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT)
            {
                ((Label)e.Row.FindControl("lblItemCode")).Visible = true;
                ((Controls_TextBox)e.Row.FindControl("tbItemCode")).Visible = false;
                ((Label)e.Row.FindControl("lblQty")).Visible = true;
                //((TextBox)e.Row.FindControl("tbQty")).Visible = false;
                ((Label)e.Row.FindControl("lblCost")).Visible = true;
                tbCost.Visible = false;

                ((LinkButton)e.Row.FindControl("lbtnAdd")).Visible = false;
                ((LinkButton)e.Row.FindControl("lbtnDelete")).Visible = false;
            }
            else
            {
                if (orderDetail.IsBlankDetail)
                {
                    Controls_TextBox tbItemCode = ((Controls_TextBox)e.Row.FindControl("tbItemCode"));
                    tbItemCode.Visible = true;
                    tbItemCode.SuggestTextBox.Attributes.Add("onchange", "GenerateItem(this);");
                    ((LinkButton)e.Row.FindControl("lbtnAdd")).Visible = true;
                    ((LinkButton)e.Row.FindControl("lbtnDelete")).Visible = false;

                    RangeValidator revCost = (RangeValidator)e.Row.FindControl("revCost");
                    revCost.Enabled = true;
                }
                else
                {
                    ((LinkButton)e.Row.FindControl("lbtnAdd")).Visible = false;
                    ((LinkButton)e.Row.FindControl("lbtnDelete")).Visible = true;
                }
            }
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        int rowIndex = ((GridViewRow)(((DataControlFieldCell)(((LinkButton)(sender)).Parent)).Parent)).RowIndex;
        MiscOrder.MiscOrderDetails.RemoveAt(rowIndex);
        BindMiscOrderDetails(true);
        this.MiscOrderDetailsGV.DataBind();
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (this.rfvEffectDate.IsValid && this.rfvLocation.IsValid && this.rfvRegion.IsValid)//&& this.rfvCostCenterCode.IsValid
        //&& this.rfvSubjectCode.IsValid && this.rfvEffectDate.IsValid && this.rfvAccountCode.IsValid)
        {
            if (this.MiscOrder.MiscOrderDetails != null && this.MiscOrder.MiscOrderDetails.Count == 0)
            {
                ShowErrorMessage("MasterData.MiscOrder.Error.NoDetails");
                return;
            }
            try
            {
                UpdateMiscOrderDetails();

                MiscOrder.Remark = this.tbRemark.Text;
                MiscOrder.Type = this.ModuleType;
                MiscOrder.Location = this.TheLocationMgr.LoadLocation(this.tbMiscOrderLocation.Text);
                MiscOrder.EffectiveDate = DateTime.Parse(this.tbMiscOrderEffectDate.Text);
                MiscOrder.ReferenceOrderNo = this.tbRefNo.Text.Trim();
                MiscOrder.Reason = this.ddlReason.SelectedValue;

                MiscOrder = TheMiscOrderMgr.SaveMiscOrder(MiscOrder, this.CurrentUser);

                ShowSuccessMessage("MasterData.MiscOrder.Value.Successfully", MiscOrder.OrderNo);

                InitPageParameter();
            }
            catch (BusinessErrorException ex)
            {
                ShowErrorMessage(ex);
            }
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    public void InitPageParameter()
    {
       
        if (this.MiscOrder.OrderNo == null)
        {
            //绑定头
            MiscOrder = new MiscOrder();
            MiscOrder.CreateUser = this.CurrentUser;
            IList<MiscOrderDetail> miscOrderDetails = new List<MiscOrderDetail>();
            MiscOrder.MiscOrderDetails = miscOrderDetails;

            this.tbMiscOrderRegion.Text = string.Empty;
            this.tbMiscOrderLocation.Text = string.Empty;
            this.tbMiscOrderEffectDate.Text = string.Empty;

            this.tbRefNo.Text = string.Empty;
            this.tbRemark.Text = string.Empty;
            this.tbMiscOrderCreateDate.Text = DateTime.Now.ToLongDateString();
            this.lbCreateUser.Text = this.CurrentUser.Code;
            this.tbMiscOrderCode.Text = string.Empty;
            this.MiscOrderDetailsGV.Columns[5].Visible = true;

            this.tvMiscOrderRegion.Visible = false;
            this.tbMiscOrderRegion.Visible = true;

            this.tvMiscOrderLocation.Visible = false;
            this.tbMiscOrderLocation.Visible = true;

            this.lbRefNo.Visible = false;
            this.tbRefNo.Visible = true;

            this.tvMiscOrderEffectDate.Visible = false;
            this.tbMiscOrderEffectDate.Visible = true;

            //this.MiscOrderDetailsGV.Columns[5].Visible = true;
            this.btnSubmit.Visible = true;

            this.btnCancel.Enabled = false;
            this.btnConfirm.Enabled = false;

            this.tbRemark.Enabled = true;
            this.ddlReason.Visible = true;
            this.lblReason.Visible = false;

            //绑定明细
            BindMiscOrderDetails(true);
        }
        else
        {
            this.tbMiscOrderCode.Text = MiscOrder.OrderNo;
            this.tbMiscOrderCreateDate.Text = MiscOrder.CreateDate.ToLongDateString();

            this.tvMiscOrderRegion.Text = MiscOrder.Location == null ? string.Empty : MiscOrder.Location.Region.Name;
            this.tvMiscOrderRegion.Visible = true;
            this.tbMiscOrderRegion.Visible = false;

            this.tvMiscOrderLocation.Text = MiscOrder.Location == null ? string.Empty : MiscOrder.Location.Name;
            this.tvMiscOrderLocation.Visible = true;
            this.tbMiscOrderLocation.Visible = false;

            this.lbRefNo.Text = MiscOrder.ReferenceOrderNo;
            this.lbRefNo.Visible = true;
            this.tbRefNo.Visible = false;

            this.tvMiscOrderEffectDate.Text = MiscOrder.EffectiveDate.ToLongDateString();
            this.tvMiscOrderEffectDate.Visible = true;
            this.tbMiscOrderEffectDate.Visible = false;
            this.lbCreateUser.Text = MiscOrder.CreateUser.Code;
            this.tbRemark.Text = MiscOrder.Remark;
            this.tbRemark.Enabled = false;
            this.ddlReason.Visible = false;
            this.lblReason.Visible = true;
            this.ddlReason.Text = MiscOrder.Reason;

            this.MiscOrderDetailsGV.Columns[5].Visible = false;

            this.btnSubmit.Visible = false;

            if (this.MiscOrder.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT)
            {
                this.tbRemark.Enabled = true;
            }

            this.lblReason.Text = GetCodeMaster(this.MiscOrder.Reason);
            //绑定明细
            BindMiscOrderDetails(false);
        }
    }

    protected void lbtnAdd_Click(object sender, EventArgs e)
    {
        int rowIndex = ((GridViewRow)(((DataControlFieldCell)(((LinkButton)(sender)).Parent)).Parent)).RowIndex;
        GridViewRow row = this.MiscOrderDetailsGV.Rows[rowIndex];
        RequiredFieldValidator rfvItemCode = (RequiredFieldValidator)row.FindControl("rfvItemCode");

        if (rfvItemCode.IsValid)
        {
            UpdateMiscOrderDetails();

            //int rowCount = this.MiscOrderDetailsGV.Rows.Count - 1;
            MiscOrderDetail miscOrderDetail = new MiscOrderDetail();
            Controls_TextBox tbItemCode = row.FindControl("tbItemCode") as Controls_TextBox;
            miscOrderDetail.Item = TheItemMgr.LoadItem(tbItemCode.Text.Trim());
            TextBox tbGridQtyTextBox = row.FindControl("tbCost") as TextBox;
            miscOrderDetail.Cost = decimal.Parse(tbGridQtyTextBox.Text.Trim());
            miscOrderDetail.IsBlankDetail = false;
            this.MiscOrder.AddMiscOrderDetail(miscOrderDetail);

            BindMiscOrderDetails(true);
        }
    }

    private void UpdateMiscOrderDetails()
    {
        int rowCount = this.MiscOrderDetailsGV.Rows.Count - 1;
        IList<MiscOrderDetail> miscOrderDetails = new List<MiscOrderDetail>();
        for (int i = 0; i < rowCount; i++)
        {
            MiscOrderDetail miscOrderDetail = this.MiscOrder.MiscOrderDetails[i];
            TextBox tbGridQtyTextBox = this.MiscOrderDetailsGV.Rows[i].FindControl("tbCost") as TextBox;
            miscOrderDetail.Cost = decimal.Parse(tbGridQtyTextBox.Text.Trim());
        }
    }

    private void BindMiscOrderDetails(bool includeBlank)
    {
        IList<MiscOrderDetail> miscOrderDetailList = new List<MiscOrderDetail>();
        foreach (MiscOrderDetail miscOrderDetail in this.MiscOrder.MiscOrderDetails)
        {
            miscOrderDetailList.Add(miscOrderDetail);
        }

        if (includeBlank)
        {
            MiscOrderDetail blankMiscOrderDetail = new MiscOrderDetail();
            blankMiscOrderDetail.Qty = 0;
            blankMiscOrderDetail.IsBlankDetail = true;
            miscOrderDetailList.Add(blankMiscOrderDetail);
        }
        this.MiscOrderDetailsGV.DataSource = miscOrderDetailList;
        this.MiscOrderDetailsGV.DataBind();
    }

    public void UpdateView()
    {
        this.MiscOrder = new MiscOrder();
        this.InitPageParameter();
    }

    public void InitPageParameter(string miscOrderNo)
    {
        MiscOrder miscOrder = TheMiscOrderMgr.ReLoadMiscOrder(miscOrderNo);
        if (miscOrder.MiscOrderDetails != null && miscOrder.MiscOrderDetails.Count > 0)
        {
            if (miscOrder.MiscOrderDetails[0].HuId != null && miscOrder.MiscOrderDetails[0].HuId != string.Empty)
            {
                this.MiscOrder = miscOrder;
                this.InitPageParameter();
            }
            else
            {
                this.MiscOrder = miscOrder;
                this.InitPageParameter();
            }
        }
    }
    private string GetCodeMaster(string codeValue)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(CodeMaster));
        selectCriteria.Add(Expression.Eq("Value", codeValue));

        IList<CodeMaster> codemstrs = TheCriteriaMgr.FindAll<CodeMaster>(selectCriteria);
        if (codemstrs != null && codemstrs.Count > 0)
        {
            return codemstrs[0].Description;
        }
        return string.Empty;
    }
}
