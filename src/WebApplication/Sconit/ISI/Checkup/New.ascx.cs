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
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Web;
using com.Sconit.Control;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Checkup_New : NewModuleBase
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
    public event EventHandler CreateEvent;
    public event EventHandler NewEvent;
    private Checkup checkup;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void FV_Checkup_OnDataBinding(object sender, EventArgs e)
    {

    }

    public void PageCleanup()
    {
        this.IsAutoRelease.Checked = this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP);
        tbAmount.Text = string.Empty;
        tbCheckupDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        tbContent.Text = string.Empty;
        tbCheckupUser.Text = string.Empty;
        tbCheckupProject.Text = string.Empty;
        tbCheckupProject.ServiceParameter = "string:#tbCheckupUser,DateTime:#tbCheckupDate,string:" + this.ModuleType;
        tbCheckupProject.DataBind();
        fsApprove.Visible = this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP);
        this.tbAuditInstructions.Text = string.Empty;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (this.rfvContent.IsValid && this.rfvCheckupDate.IsValid && this.rfvCheckupProject.IsValid && this.rvAmount.IsValid && this.rfvCheckupUser.IsValid)
        {
            string checkupProjectCode = tbCheckupProject.Text.Trim();
            DateTime checkupDate = DateTime.Parse(tbCheckupDate.Text.Trim());
            string userCodeNames = tbCheckupUser.Text.Trim();
            string content = tbContent.Text.Trim();
            string amountStr = tbAmount.Text.Trim();
            bool isAutoRelease = this.IsAutoRelease.Checked;
            string auditInstructions = this.tbAuditInstructions.Text.Trim();
            decimal? amount = null;
            if (amountStr != string.Empty)
            {
                amount = decimal.Parse(amountStr);
            }
            string[] users = ISIUtil.GetUserSplit(userCodeNames);
            string[] userCodes = users[0].Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);

            Checkup checkup = this.TheCheckupMgr.CreateCheckup(this.ModuleType, checkupProjectCode, checkupDate, userCodes, content, amount, isAutoRelease, auditInstructions, this.CurrentUser);
            if (userCodes.Length > 1)
            {
                BackEvent(this, e);
                ShowSuccessMessage("ISI.Checkup.AddCheckups.Successfully", users[1].Replace(",", "、"));
            }
            else
            {
                CreateEvent(checkup.Id.ToString(), e);
                ShowSuccessMessage("ISI.Checkup.AddCheckup.Successfully");
            }
        }
    }
}
