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
using com.Sconit.Entity.MasterData;
using com.Sconit.Control;

using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.Exception;

public partial class ISI_Checkup_Edit : EditModuleBase
{
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

    public event EventHandler BackEvent;

    protected string CheckupId
    {
        get
        {
            return (string)ViewState["CheckupId"];
        }
        set
        {
            ViewState["CheckupId"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void InitPageParameter(string id)
    {
        this.CheckupId = id;
        Checkup checkup = this.TheCheckupMgr.LoadCheckup(int.Parse(this.CheckupId));
        this.ModuleType = checkup.Type;
        this.ODS_Checkup.SelectParameters["Id"].DefaultValue = this.CheckupId;
    }

    protected void FV_Checkup_DataBound(object sender, EventArgs e)
    {
        if (CheckupId != null)
        {
            Checkup checkup = (Checkup)((FormView)sender).DataItem;
            UpdateView(checkup);
        }
    }

    private void UpdateView(Checkup checkup)
    {
        this.FV_Checkup.FindControl("btnClose").Visible = false;
        ((Literal)(this.FV_Checkup.FindControl("lblStatus"))).Text = "${" + "ISI.Status." + checkup.Status + "}";
        if (checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CREATE)
        {
            this.FV_Checkup.FindControl("btnCancel").Visible = false;
            this.FV_Checkup.FindControl("fsApprove").Visible = false;
            this.FV_Checkup.FindControl("btnApprove").Visible = false;

            if (checkup.CreateUser != this.CurrentUser.Code
                && !this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_CLOSECHECKUP)
                && !this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP)
                )
            {
                this.FV_Checkup.FindControl("btnSave").Visible = false;
                this.FV_Checkup.FindControl("btnSubmit").Visible = false;
                this.FV_Checkup.FindControl("btnDelete").Visible = false;
                TextBox tbContent = ((TextBox)this.FV_Checkup.FindControl("tbContent"));
                tbContent.ReadOnly = true;
                TextBox tbAmount = ((TextBox)this.FV_Checkup.FindControl("tbAmount"));
                tbAmount.ReadOnly = true;
            }

        }
        else
        {
            TextBox tbContent = ((TextBox)this.FV_Checkup.FindControl("tbContent"));
            tbContent.ReadOnly = true;
            TextBox tbAmount = ((TextBox)this.FV_Checkup.FindControl("tbAmount"));
            tbAmount.ReadOnly = true;
            TextBox tbAuditInstructions = ((TextBox)this.FV_Checkup.FindControl("tbAuditInstructions"));
            tbAuditInstructions.ReadOnly = true;
            this.FV_Checkup.FindControl("fsApprove").Visible = true;
            if (checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE
                   || checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CANCEL)
            {
                this.FV_Checkup.FindControl("btnSave").Visible = false;
                this.FV_Checkup.FindControl("btnSubmit").Visible = false;
                this.FV_Checkup.FindControl("btnCancel").Visible = false;
                this.FV_Checkup.FindControl("btnDelete").Visible = false;
                this.FV_Checkup.FindControl("btnApprove").Visible = false;
            }
            if (checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT
                || checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL)
            {
                this.FV_Checkup.FindControl("btnSave").Visible = false;
                this.FV_Checkup.FindControl("btnSubmit").Visible = false;
                this.FV_Checkup.FindControl("btnDelete").Visible = false;

                if (this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP))
                {
                    this.FV_Checkup.FindControl("btnApprove").Visible = true;
                    tbAuditInstructions.ReadOnly = false;
                    tbAmount.ReadOnly = false;
                }
                else
                {
                    this.FV_Checkup.FindControl("btnApprove").Visible = false;
                }

                if (checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL
                        && this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_CLOSECHECKUP))
                {
                    this.FV_Checkup.FindControl("btnClose").Visible = true;
                }

                if (!(this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP)
                        || (checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT && checkup.SubmitUser == this.CurrentUser.Code)))
                {
                    this.FV_Checkup.FindControl("btnCancel").Visible = false;
                }
            }

        }
    }


    protected void btnClose_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheCheckupMgr.CloseCheckup(int.Parse(this.CheckupId), this.CurrentUser);
            this.FV_Checkup.DataBind();
            ShowSuccessMessage("ISI.Checkup.Close" + this.ModuleType + ".Successfully");

        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheCheckupMgr.CancelCheckup(int.Parse(this.CheckupId), this.CurrentUser);
            this.FV_Checkup.DataBind();
            ShowSuccessMessage("ISI.Checkup.Cancel" + this.ModuleType + ".Successfully");

        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }


    protected void btnApprove_Click(object sender, EventArgs e)
    {
        try
        {
            string auditInstructions = ((TextBox)(this.FV_Checkup.FindControl("tbAuditInstructions"))).Text.Trim();
            string amountStr = ((TextBox)(this.FV_Checkup.FindControl("tbAmount"))).Text.Trim();
            decimal amount = 0;
            if (!string.IsNullOrEmpty(amountStr))
            {
                amount = decimal.Parse(amountStr);
            }
            this.TheCheckupMgr.ApproveCheckup(int.Parse(this.CheckupId), amount, auditInstructions, this.CurrentUser);
            this.FV_Checkup.DataBind();
            ShowSuccessMessage("ISI.Checkup.Approve" + this.ModuleType + ".Successfully");

        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            string content = ((TextBox)(this.FV_Checkup.FindControl("tbContent"))).Text.Trim();
            string amountStr = ((TextBox)(this.FV_Checkup.FindControl("tbAmount"))).Text.Trim();
            decimal amount = 0;
            if (!string.IsNullOrEmpty(amountStr))
            {
                amount = decimal.Parse(amountStr);
            }

            this.TheCheckupMgr.SubmitCheckup(int.Parse(this.CheckupId), amount, content, this.CurrentUser);
            this.FV_Checkup.DataBind();
            ShowSuccessMessage("ISI.Checkup.Submit" + this.ModuleType + ".Successfully");

        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_Checkup_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("ISI.Checkup.UpdateCheckup.Successfully");
        //btnBack_Click(this, e);
    }

    protected void ODS_Checkup_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        Checkup checkup = (Checkup)e.InputParameters[0];
        if (checkup != null)
        {
            Checkup oldCheckup = TheCheckupMgr.LoadCheckup(checkup.Id);
            checkup.CreateDate = oldCheckup.CreateDate;
            checkup.CreateUser = oldCheckup.CreateUser;
            checkup.CreateUserNm = oldCheckup.CreateUserNm;
            checkup.Department = oldCheckup.Department;
            checkup.Dept2 = oldCheckup.Dept2;
            checkup.JobNo = oldCheckup.JobNo;
            checkup.CheckupUserNm = oldCheckup.CheckupUserNm;
            checkup.CheckupUser = oldCheckup.CheckupUser;
            checkup.Type = oldCheckup.Type;
            checkup.Status = oldCheckup.Status;
            checkup.CheckupProject = oldCheckup.CheckupProject;
            checkup.CheckupDate = oldCheckup.CheckupDate;

            checkup.LastModifyDate = DateTime.Now;
            checkup.LastModifyUser = this.CurrentUser.Code;
            checkup.LastModifyUserNm = this.CurrentUser.Name;
        }

    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheCheckupMgr.DeleteCheckup(int.Parse(this.CheckupId));
            btnBack_Click(this, e);
            ShowSuccessMessage("ISI.Checkup.DeleteCheckup.Successfully");
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.Checkup.DeleteCheckup.Fail");
        }

    }

}
