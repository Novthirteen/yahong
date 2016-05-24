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
using com.Sconit.Service.Ext.MasterData;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Control;

public partial class ISI_TSK_CostList : ListModuleBase
{

    public EventHandler EditEvent;
    public IList<Cost> TheCostList
    {
        get
        {
            if (ViewState["CostList"] == null)
            {
                ViewState["CostList"] = new List<Cost>();
            }
            return (IList<Cost>)ViewState["CostList"];
        }
        set
        {
            ViewState["CostList"] = value;
        }
    }
    public bool IsAmountDetail
    {
        get
        {
            return ViewState["IsAmountDetail"] != null ? (bool)ViewState["IsAmountDetail"] : false;
        }
        set
        {
            ViewState["IsAmountDetail"] = value;
        }
    }
    public string CostCenter
    {
        get
        {
            return (string)ViewState["CostCenter"];
        }
        set
        {
            ViewState["CostCenter"] = value;
        }
    }
    public string TaskCode
    {
        get
        {
            return (string)ViewState["TaskCode"];
        }
        set
        {
            ViewState["TaskCode"] = value;
        }
    }
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
    public string PageAction
    {
        get
        {
            return (string)ViewState["PageAction"];
        }
        set
        {
            ViewState["PageAction"] = value;
        }
    }
    public bool IsCostCenter
    {
        get
        {
            return ViewState["IsCostCenter"] == null ? false : (bool)ViewState["IsCostCenter"];
        }
        set
        {
            ViewState["IsCostCenter"] = value;
        }
    }
    public string FormType
    {
        get
        {
            return (string)ViewState["FormType"];
        }
        set
        {
            ViewState["FormType"] = value;
        }
    }

    public void PageCleanup()
    {
        IsAmountDetail = false;
        PageAction = string.Empty;
        FormType = string.Empty;
        TaskCode = string.Empty;
        TheCostList = null;
        IsCostCenter = false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public override void UpdateView()
    {
        //this.GV_List.DataBind();
    }

    protected void lbtnAdd_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            int currentRow = ((GridViewRow)(((DataControlFieldCell)(((System.Web.UI.WebControls.LinkButton)(sender)).Parent)).Parent)).RowIndex;
            GridViewRow newCostDetRow = this.GV_List.Rows[currentRow];
            string costCenter = ((Controls_TextBox)newCostDetRow.FindControl("tbTaskSubType")).Text.Trim();
            string code = ((System.Web.UI.WebControls.LinkButton)sender).CommandArgument;
            Cost costDet = null;
            if (string.IsNullOrEmpty(code) || int.Parse(code) == 0)
            {
                costDet = new Cost();
            }
            else
            {
              //  costDet = this.TheCostMgr.LoadCost(int.Parse(code));
            }

            if (!string.IsNullOrEmpty(costCenter))
            {
                var taskSubType = this.TheTaskSubTypeMgr.LoadTaskSubType(costCenter);
                costDet.TaskSubType = taskSubType.Code;
                costDet.TaskSubTypeDesc = taskSubType.Desc;
            }
            else
            {
                costDet.TaskSubType = null;
                costDet.TaskSubTypeDesc = null;
            }

            string account1Code = ((Controls_TextBox)newCostDetRow.FindControl("tbAccount1")).Text.Trim();
            if (!string.IsNullOrEmpty(account1Code))
            {
             //   var account = this.TheAccountMgr.LoadAccount(account1Code);
              //  costDet.Account1 = account.Code;
             //   costDet.Account1Desc = account.Desc1;
            }
            else
            {
                costDet.Account1 = null;
                costDet.Account1Desc = null;
            }

            string account2Code = ((Controls_TextBox)newCostDetRow.FindControl("tbAccount2")).Text.Trim();
            if (!string.IsNullOrEmpty(account2Code))
            {
               // var account = this.TheAccountMgr.LoadAccount(account2Code);
              //  costDet.Account2 = account.Code;
               // costDet.Account2Desc = account.Desc1;
            }
            else
            {
                costDet.Account2 = null;
                costDet.Account2Desc = null;
            }

            string userCode = ((Controls_TextBox)newCostDetRow.FindControl("tbUserCode")).Text.Trim();
            if (!string.IsNullOrEmpty(userCode))
            {
                var user = this.TheUserMgr.LoadUser(userCode);
                costDet.UserCode = user.Code;
                costDet.UserName = user.Name;
            }
            else
            {
                costDet.UserCode = string.Empty;
                costDet.UserName = string.Empty;
            }
            string item = ((TextBox)newCostDetRow.FindControl("tbItem")).Text.Trim();
            if (!string.IsNullOrEmpty(item))
            {
                costDet.Item = item;
            }
            else
            {
                costDet.Item = string.Empty;
            }
            string uom = ((TextBox)newCostDetRow.FindControl("tbUom")).Text.Trim();
            if (!string.IsNullOrEmpty(uom))
            {
                costDet.Uom = uom;
            }
            else
            {
                costDet.Uom = string.Empty;
            }

            string startDate = ((TextBox)newCostDetRow.FindControl("tbStartDate")).Text.Trim();
            if (!string.IsNullOrEmpty(startDate))
            {
                costDet.StartDate = DateTime.Parse(startDate);
            }
            else
            {
                costDet.StartDate = null;
            }

            string endDate = ((TextBox)newCostDetRow.FindControl("tbEndDate")).Text.Trim();
            if (!string.IsNullOrEmpty(endDate))
            {
                costDet.EndDate = DateTime.Parse(endDate);
            }
            else
            {
                costDet.EndDate = null;
            }

            if (this.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2 || this.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_3)
            {
                if (costDet.StartDate.HasValue && costDet.EndDate.HasValue)
                {
                    costDet.Qty = decimal.Parse(costDet.EndDate.Value.Subtract(costDet.StartDate.Value).TotalHours.ToString());
                }
                else
                {
                    costDet.Qty = null;
                }
            }
            else
            {
                string qty = ((TextBox)newCostDetRow.FindControl("tbQty")).Text.Trim();
                if (!string.IsNullOrEmpty(qty))
                {
                    costDet.Qty = decimal.Parse(qty);
                }
                else
                {
                    costDet.Qty = null;
                }
            }

            string startAddr = ((TextBox)newCostDetRow.FindControl("tbStartAddr")).Text.Trim();
            if (!string.IsNullOrEmpty(startAddr))
            {
                costDet.StartAddr = startAddr;
            }
            else
            {
                costDet.StartAddr = string.Empty;
            }
            string endAddr = ((TextBox)newCostDetRow.FindControl("tbEndAddr")).Text.Trim();
            if (!string.IsNullOrEmpty(endAddr))
            {
                costDet.EndAddr = endAddr;
            }
            else
            {
                costDet.EndAddr = string.Empty;
            }

            TextBox tbAllowance = ((TextBox)newCostDetRow.FindControl("tbAllowance"));
            if (tbAllowance.Visible)
            {
                var allowance = tbAllowance.Text.Trim();
                if (!string.IsNullOrEmpty(allowance))
                {
                    costDet.Allowance = decimal.Parse(allowance);
                }
                else
                {
                    costDet.Allowance = null;
                }
            }
            else
            {
                costDet.Allowance = null;
            }

            TextBox tbFare = ((TextBox)newCostDetRow.FindControl("tbFare"));
            if (tbFare.Visible)
            {
                var fare = tbFare.Text.Trim();
                if (!string.IsNullOrEmpty(fare))
                {
                    costDet.Fare = decimal.Parse(fare);
                }
                else
                {
                    costDet.Fare = null;
                }
            }
            else
            {
                costDet.Fare = null;
            }

            TextBox tbQuarterage = ((TextBox)newCostDetRow.FindControl("tbQuarterage"));
            if (tbQuarterage.Visible)
            {
                var quarterage = tbQuarterage.Text.Trim();
                if (!string.IsNullOrEmpty(quarterage))
                {
                    costDet.Quarterage = decimal.Parse(quarterage);
                }
                else
                {
                    costDet.Quarterage = null;
                }
            }
            else
            {
                costDet.Quarterage = null;
            }

            TextBox tbHaulage = ((TextBox)newCostDetRow.FindControl("tbHaulage"));
            if (tbHaulage.Visible)
            {
                var haulage = tbHaulage.Text.Trim();
                if (!string.IsNullOrEmpty(haulage))
                {
                    costDet.Haulage = decimal.Parse(haulage);
                }
                else
                {
                    costDet.Haulage = null;
                }
            }
            else
            {
                costDet.Haulage = null;
            }


            costDet.NoTaxAmount = (costDet.Allowance.HasValue ? costDet.Allowance.Value : 0)
                                  + (costDet.Fare.HasValue ? costDet.Fare.Value : 0)
                                  + (costDet.Quarterage.HasValue ? costDet.Quarterage.Value : 0)
                                  + (costDet.Haulage.HasValue ? costDet.Haulage.Value : 0);



            if (this.FormType != ISIConstants.CODE_MASTER_WFS_FORMTYPE_2)
            {
                TextBox tbNoTaxAmount = ((TextBox)newCostDetRow.FindControl("tbNoTaxAmount"));
                if (tbNoTaxAmount.Visible)
                {
                    var noTaxAmount = tbNoTaxAmount.Text.Trim();
                    if (!string.IsNullOrEmpty(noTaxAmount))
                    {
                        costDet.NoTaxAmount = decimal.Parse(noTaxAmount);
                    }
                    else
                    {
                        costDet.NoTaxAmount = null;
                    }
                }
                else
                {
                    costDet.NoTaxAmount = null;
                }
            }
            
            if (IsAmountDetail && (!costDet.NoTaxAmount.HasValue || costDet.NoTaxAmount.Value == 0))
            {
                this.ShowErrorMessage("WFS.Cost.Amount.Error");
                return;
            }

            TextBox tbTaxes = ((TextBox)newCostDetRow.FindControl("tbTaxes"));
            if (tbTaxes.Visible)
            {
                var taxes = tbTaxes.Text.Trim();
                if (!string.IsNullOrEmpty(taxes))
                {
                    costDet.Taxes = decimal.Parse(taxes);
                }
                else
                {
                    costDet.Taxes = null;
                }
            }
            else
            {
                costDet.Taxes = null;
            }
            /*
            TextBox tbTotalAmount = ((TextBox)newCostDetRow.FindControl("tbTotalAmount"));
            if (tbTotalAmount.Visible)
            {
                var totalAmount = tbTotalAmount.Text.Trim();
                if (!string.IsNullOrEmpty(totalAmount))
                {
                    costDet.TotalAmount = decimal.Parse(totalAmount);
                }
                else
                {
                    costDet.TotalAmount = null;
                }
            }
            else
            {
                costDet.TotalAmount = null;
            }*/

            if (!costDet.NoTaxAmount.HasValue && !costDet.Taxes.HasValue)
            {
                costDet.TotalAmount = null;
            }
            else
            {
                costDet.TotalAmount = (costDet.NoTaxAmount.HasValue ? costDet.NoTaxAmount.Value : 0)
                                                + (costDet.Taxes.HasValue ? costDet.Taxes.Value : 0);
            }

            string extNo = ((TextBox)newCostDetRow.FindControl("tbExtNo")).Text.Trim();
            if (!string.IsNullOrEmpty(extNo))
            {
                costDet.ExtNo = extNo;
            }
            else
            {
                costDet.ExtNo = string.Empty;
            }
            string desc1 = ((TextBox)newCostDetRow.FindControl("tbDesc1")).Text.Trim();
            if (!string.IsNullOrEmpty(desc1))
            {
                costDet.Desc1 = desc1;
            }
            else
            {
                costDet.Desc1 = string.Empty;
            }

            string vehicle = ((CodeMstrDropDownList)newCostDetRow.FindControl("tbVehicle")).SelectedValue;
            if (!string.IsNullOrEmpty(vehicle))
            {
                costDet.Vehicle = vehicle;
            }
            else
            {
                costDet.Vehicle = string.Empty;
            }

            costDet.TaskCode = this.TaskCode;
            costDet.LastModifyDate = DateTime.Now;
            costDet.LastModifyUser = this.CurrentUser.Code;
            costDet.LastModifyUserNm = this.CurrentUser.Name;

            if (!string.IsNullOrEmpty(TaskCode))
            {
                if (costDet.Id == 0)
                {
                  //  this.TheCostMgr.CreateCost(costDet);
                }
                else
                {
                    this.TheCostList = this.TheCostList.Where(c => c.Id != costDet.Id).ToList();
                 //   this.TheCostMgr.UpdateCost(costDet);
                    this.ShowSuccessMessage("WFS.Cost.Update.Successfully");
                }
            }

            this.TheCostList.Add(costDet);
            InitPageParameter(TheCostList);
            string qty1 = string.Empty;
            string noTaxAmount1 = string.Empty;
            string taxes1 = string.Empty;
            string totalAmount1 = string.Empty;
            if (TheCostList != null && TheCostList.Count > 0)
            {
                qty1 = TheCostList.Where(c => c.EndDate.HasValue && c.StartDate.HasValue).Sum(c => c.EndDate.Value.Subtract(c.StartDate.Value).TotalHours).ToString("0.##");

                noTaxAmount1 = TheCostList.Where(c => c.NoTaxAmount.HasValue).Sum(c => c.NoTaxAmount.Value).ToString("0.########");
                taxes1 = TheCostList.Where(c => c.Taxes.HasValue).Sum(c => c.Taxes.Value).ToString("0.########");
                totalAmount1 = TheCostList.Where(c => c.TotalAmount.HasValue).Sum(c => c.TotalAmount.Value).ToString("0.########");
            }

            EditEvent(new string[] { noTaxAmount1 == "0" ? string.Empty : noTaxAmount1, taxes1 == "0" ? string.Empty : taxes1, totalAmount1 == "0" ? string.Empty : totalAmount1, qty1 == "0" ? string.Empty : qty1 }, e);
        }
    }
    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Cost cost = (Cost)e.Row.DataItem;
            e.Row.FindControl("lbtnDelete").Visible = !cost.IsBlankDetail;
            System.Web.UI.WebControls.LinkButton lbtnAdd = (System.Web.UI.WebControls.LinkButton)e.Row.FindControl("lbtnAdd");
            lbtnAdd.Visible = (cost.IsBlankDetail || !string.IsNullOrEmpty(this.TaskCode)) && this.PageAction != BusinessConstants.PAGE_LIST_ACTION;
            if (!cost.IsBlankDetail && !string.IsNullOrEmpty(this.TaskCode) && this.PageAction != BusinessConstants.PAGE_LIST_ACTION)
            {
                lbtnAdd.Text = "${Common.Button.Update}";
                string validationGroup = "vgAdd" + cost.Id;
                lbtnAdd.ValidationGroup = validationGroup;

                string[] validators = new string[] { "rfvStartDate", "rfvEndDate", "rfvEndAddr", "rvAllowance", "rvFare", 
                                                     "rvQuarterage","rvHaulage", "rfvNoTaxAmount","rvNoTaxAmount", "rvTaxes",
                                                     "rfvItem","rfvUom","rfvQty","rvQty","rfvStartAddr"};
                foreach (var v in validators)
                {
                    var validator = (BaseValidator)e.Row.FindControl(v);
                    validator.ValidationGroup = validationGroup;
                }

                e.Row.FindControl("lblLastModifyDate").Visible = true;
                e.Row.FindControl("ltlLastModifyDate").Visible = true;
                e.Row.FindControl("ltlLastModifyUser").Visible = true;
                e.Row.FindControl("lblLastModifyUser").Visible = true;
            }

            string[] controls = new string[] {"Item","Uom","Qty","Vehicle",
                                                "TaskSubType", "Account1", "Account2", "ExtNo", "Desc1", "NoTaxAmount", "Taxes", "TotalAmount",
                                                "UserCode", "StartAddr", "EndAddr", "StartDate", "EndDate", "Allowance", "Fare", "Quarterage","Haulage" };
            foreach (var c in controls)
            {
                var tb = e.Row.FindControl("tb" + c);
                if (tb != null)
                {
                    tb.Visible = this.PageAction != BusinessConstants.PAGE_LIST_ACTION && (cost.IsBlankDetail || !string.IsNullOrEmpty(this.TaskCode));
                    if (c == "TaskSubType")
                    {
                        var tb1 = (Controls_TextBox)tb;
                        tb1.Text = cost.TaskSubType;
                        tb.Visible = this.IsCostCenter && this.PageAction == BusinessConstants.PAGE_NEW_ACTION && (cost.IsBlankDetail || !string.IsNullOrEmpty(this.TaskCode));
                    }
                    if (c == "Account1")
                    {
                        var tb1 = (Controls_TextBox)tb;
                        tb1.ServiceParameter = "string:" + this.CostCenter + ",string:#" + (this.PageAction == BusinessConstants.PAGE_NEW_ACTION && (cost.IsBlankDetail || !string.IsNullOrEmpty(this.TaskCode)) ? "tbTaskSubType" : "hfTaskSubType");
                        tb1.Text = cost.Account1;
                        tb1.DataBind();
                        tb1.Visible = this.IsCostCenter;
                        if (!cost.IsBlankDetail && cost.IsAccount1)
                        {
                            Label ltl = (Label)e.Row.FindControl("ltl" + c);
                            if (ltl != null)
                            {
                                tb1.CssClass = "inputRequired";
                                ltl.BackColor = System.Drawing.Color.Red;
                            }
                        }
                        tb1.Visible = this.IsCostCenter && this.PageAction != BusinessConstants.PAGE_LIST_ACTION && (cost.IsBlankDetail || !string.IsNullOrEmpty(this.TaskCode));
                    }
                    if (c == "Account2")
                    {
                        var tb1 = (Controls_TextBox)tb;
                        tb1.Text = cost.Account2;
                        tb1.ServiceParameter = "string:" + this.CostCenter + ",string:#" + (this.PageAction == BusinessConstants.PAGE_NEW_ACTION && (cost.IsBlankDetail || !string.IsNullOrEmpty(this.TaskCode)) ? "tbTaskSubType" : "hfTaskSubType") + ",string:#tbAccount1";
                        tb1.DataBind();
                        tb1.Visible = this.IsCostCenter;
                        if (!cost.IsBlankDetail && cost.IsAccount2)
                        {
                            Label ltl = (Label)e.Row.FindControl("ltl" + c);
                            if (ltl != null)
                            {
                                tb1.CssClass = "inputRequired";
                                ltl.BackColor = System.Drawing.Color.Red;
                            }
                        } 
                        tb1.Visible = this.IsCostCenter && this.PageAction != BusinessConstants.PAGE_LIST_ACTION && (cost.IsBlankDetail || !string.IsNullOrEmpty(this.TaskCode));
                    }
                    if (c == "UserCode")
                    {
                        var tb1 = (Controls_TextBox)tb;
                        tb1.Text = cost.UserCode;
                    }
                }
                var lbl = e.Row.FindControl("lbl" + c);
                if (lbl != null)
                {
                    if (c == "TaskSubType")
                    {
                        lbl.Visible = !(this.PageAction == BusinessConstants.PAGE_NEW_ACTION && (cost.IsBlankDetail || !string.IsNullOrEmpty(this.TaskCode)));
                    }
                    else
                    {
                        lbl.Visible = !(this.PageAction != BusinessConstants.PAGE_LIST_ACTION && (cost.IsBlankDetail || !string.IsNullOrEmpty(this.TaskCode)));
                    }
                }
            }
            /*
            if (cost.IsBlankDetail)
            {
            }
            else
            {
            }
            */
            e.Row.FindControl("tr1").Visible = this.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_5;
            e.Row.FindControl("tr2").Visible = this.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2 || this.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_3 || this.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_4;

            if (this.IsAmountDetail)
            {
                TextBox tbNoTaxAmount = (TextBox)e.Row.FindControl("tbNoTaxAmount");
                tbNoTaxAmount.CssClass = "inputRequired";
                e.Row.FindControl("rfvNoTaxAmount").Visible = true;
            }
            else
            {
                TextBox tbNoTaxAmount = (TextBox)e.Row.FindControl("tbNoTaxAmount");
                tbNoTaxAmount.CssClass = string.Empty;
                e.Row.FindControl("rfvNoTaxAmount").Visible = false;
            }

            if (this.FormType != ISIConstants.CODE_MASTER_WFS_FORMTYPE_2)
            {
                e.Row.FindControl("ltlVehicle").Visible = false;
                e.Row.FindControl("lblVehicle").Visible = false;
                e.Row.FindControl("tbVehicle").Visible = false;

                TextBox tbStartDate = (TextBox)e.Row.FindControl("tbStartDate");
                TextBox tbEndDate = (TextBox)e.Row.FindControl("tbEndDate");
                tbStartDate.Attributes.Add("onclick", "var " + tbEndDate.ClientID + "=$dp.$('" + tbEndDate.ClientID + "');WdatePicker({startDate:'%y-%M-%d 08:00:00',dateFmt:'yyyy-MM-dd HH:mm',onpicked:function(){" + tbEndDate.ClientID + ".click();},maxDate:'#F{$dp.$D(\\'" + tbEndDate.ClientID + "\\')}' })");
                tbEndDate.Attributes.Add("onclick", "WdatePicker({doubleCalendar:true,startDate:'%y-%M-%d 16:30:00',dateFmt:'yyyy-MM-dd HH:mm',minDate:'#F{$dp.$D(\\'" + tbStartDate.ClientID + "\\')}'})");
            }
            else
            {
                CodeMstrDropDownList tbVehicle = (CodeMstrDropDownList)e.Row.FindControl("tbVehicle");
                tbVehicle.SelectedValue = cost.Vehicle;
                TextBox tbNoTaxAmount = (TextBox)e.Row.FindControl("tbNoTaxAmount");
                tbNoTaxAmount.ReadOnly = true;
                tbNoTaxAmount.CssClass = string.Empty;
                e.Row.FindControl("rfvNoTaxAmount").Visible = false;
                TextBox tbStartDate = (TextBox)e.Row.FindControl("tbStartDate");
                TextBox tbEndDate = (TextBox)e.Row.FindControl("tbEndDate");
                tbStartDate.Attributes.Add("onclick", "var " + tbEndDate.ClientID + "=$dp.$('" + tbEndDate.ClientID + "');WdatePicker({startDate:'%y-%M-%d 08:00:00',dateFmt:'yyyy-MM-dd HH:mm',onpicked:function(){" + tbEndDate.ClientID + ".click();},maxDate:'#F{$dp.$D(\\'" + tbEndDate.ClientID + "\\')}' })");
                tbEndDate.Attributes.Add("onclick", "WdatePicker({doubleCalendar:true,startDate:'%y-%M-%d 16:30:00',dateFmt:'yyyy-MM-dd HH:mm',minDate:'#F{$dp.$D(\\'" + tbStartDate.ClientID + "\\')}'})");
            }

            if (this.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_3)
            {
                e.Row.FindControl("lblStartAddr").Visible = false;
                e.Row.FindControl("lblEndAddr").Visible = false;
                e.Row.FindControl("ltlStartAddr").Visible = false;
                e.Row.FindControl("ltlEndAddr").Visible = false;
                e.Row.FindControl("tbStartAddr").Visible = false;
                e.Row.FindControl("tbEndAddr").Visible = false;
                //((TextBox)e.Row.FindControl("tbStartAddr")).CssClass = string.Empty;
                //((TextBox)e.Row.FindControl("tbEndAddr")).CssClass = string.Empty;
                e.Row.FindControl("rfvStartAddr").Visible = false;
                e.Row.FindControl("rfvEndAddr").Visible = false;
            }
            if (this.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_4)
            {
                e.Row.FindControl("ltlStartDate").Visible = false;
                e.Row.FindControl("lblStartDate").Visible = false;
                e.Row.FindControl("tbStartDate").Visible = false;
                e.Row.FindControl("rfvStartDate").Visible = false;
                ((Label)e.Row.FindControl("ltlEndDate")).Text = "${WFS.Cost.ApplicationDate}:";
                Label lblEndDate = (Label)e.Row.FindControl("lblEndDate");
                if (cost.EndDate.HasValue)
                {
                    lblEndDate.Text = cost.EndDate.Value.ToString("yyyy-MM-dd");
                }
                TextBox tbEndDate = (TextBox)e.Row.FindControl("tbEndDate");
                tbEndDate.Attributes.Add("onclick", "WdatePicker({dateFmt:'yyyy-MM-dd'})");
            }
            if (this.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_5)
            {

            }

            e.Row.FindControl("tr3").Visible = this.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2;

        }
    }

    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        //if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        //{
        this.GV_List.Columns[this.GV_List.Columns.Count - 1].Visible = this.PageAction != BusinessConstants.PAGE_LIST_ACTION;
        //}
    }
    public void InitPageParameter()
    {
        InitPageParameter(null);
    }
    public void InitPageParameter(IList<Cost> costList)
    {
        if (costList != null)
        {
            this.TheCostList = costList;
        }
        else
        {
            this.TheCostList = new List<Cost>();
        }
        IList<Cost> costDetList = new List<Cost>();
        if (TheCostList.Count > 0)
        {
            foreach (var c in TheCostList)
            {
                costDetList.Add(c);
            }
        }

        if (this.PageAction == BusinessConstants.PAGE_NEW_ACTION)
        {
            Cost cost = new Cost();
            cost.IsBlankDetail = true;
            costDetList.Add(cost);
        }

        this.GV_List.DataSource = costDetList;
        this.GV_List.DataBind();
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code = ((System.Web.UI.WebControls.LinkButton)sender).CommandArgument;
        try
        {
            int rowIndex = ((GridViewRow)(((DataControlFieldCell)(((System.Web.UI.WebControls.LinkButton)(sender)).Parent)).Parent)).RowIndex;
            this.TheCostList.RemoveAt(rowIndex);
            if (!string.IsNullOrEmpty(code))
            {
                int id = int.Parse(code);
                if (id > 0)
                {
//                    this.TheCostMgr.DeleteCost(id);
                }
            }
            InitPageParameter(this.TheCostList);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Delete.Fail", code);
        }
    }
}
