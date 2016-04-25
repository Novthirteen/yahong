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
using System.Collections.Generic;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using Geekees.Common.Controls;
using com.Sconit.Entity.Exception;
using System.Text;

public partial class ISI_Approve_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler CloseEvent;
    public event EventHandler ApproveEvent;
    public event EventHandler ExportEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GenerateTree();

            ddlType.DataSource = this.GetType();
            ddlType.SelectedIndex = 1;
            ddlType.DataBind();

            DateTime dt = DateTime.Now;
            this.tbStartDate.Text = string.Empty;
            this.tbEndDate.Text = dt.ToString("yyyy-MM-dd");
        }
    }
    private IList<CodeMaster> GetType()
    {
        IList<CodeMaster> typeList = new List<CodeMaster>();
        typeList.Add(new CodeMaster());
        typeList.Add(GetType(ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECTTYPE_VALUE_CADRE));
        typeList.Add(GetType(ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECTTYPE_VALUE_EMPLOYEE));

        return typeList;
    }

    private CodeMaster GetType(string type)
    {
        return TheCodeMasterMgr.GetCachedCodeMaster(ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECTTYPE, type);
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (ExportEvent != null)
        {
            object[] param = this.CollectParam(false);
            if (param != null)
            {
                this.IsExport = true;
                ExportEvent(param, null);
            }
        }
    }

    protected void btnExportSummary_Click(object sender, EventArgs e)
    {
        if (ExportEvent != null)
        {
            object[] param = this.CollectParam(true);
            if (param != null)
            {
                this.IsExport = true;
                ExportEvent(param, null);
            }
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

        if (actionParameter.ContainsKey("CheckupProject"))
        {
            this.tbCheckupProject.Text = actionParameter["CheckupProject"];
        }
        if (actionParameter.ContainsKey("CheckupUser"))
        {
            this.tbCheckupUser.Text = actionParameter["CheckupUser"];
        }
        if (actionParameter.ContainsKey("CreateUser"))
        {
            this.tbCreateUser.Text = actionParameter["CreateUser"];
        }
        if (actionParameter.ContainsKey("StartDate"))
        {
            this.tbStartDate.Text = actionParameter["StartDate"];
        }
        if (actionParameter.ContainsKey("EndDate"))
        {
            this.tbEndDate.Text = actionParameter["EndDate"];
        }
    }

    protected override void DoSearch()
    {
        if (SearchEvent != null)
        {
            object[] param = CollectParam(null);
            if (param != null)
                SearchEvent(param, null);
        }
    }

    public object[] CollectParam(bool? isSummary)
    {
        #region org
        IList<string> orgList = new List<string>();
        List<ASTreeViewNode> orgNodes = this.astvMyTreeOrg.GetCheckedNodes();
        foreach (ASTreeViewNode node in orgNodes)
        {
            orgList.Add(node.NodeValue);
        }
        #endregion

        string checkupProject = this.tbCheckupProject.Text.Trim();
        string checkupUser = tbCheckupUser.Text.Trim();
        string createUser = this.tbCreateUser.Text.Trim();
        string type = this.ddlType.SelectedValue;
        DateTime? startTime = null;
        if (this.tbStartDate.Text.Trim() != string.Empty)
        {
            startTime = DateTime.Parse(this.tbStartDate.Text.Trim());
        }
        DateTime? endTime = null;
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            endTime = (DateTime.Parse(this.tbEndDate.Text.Trim())).AddDays(1).AddMilliseconds(-1);
        }

        #region status
        IList<string> statusList = new List<string>();
        List<ASTreeViewNode> nodes = this.astvMyTree.GetCheckedNodes();
        foreach (ASTreeViewNode node in nodes)
        {
            statusList.Add(node.NodeValue);
        }

        if (statusList.Count == 0)
        {
            statusList.Add(ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT);
            statusList.Add(ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL);
            statusList.Add(ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE);
        }

        #endregion
        return new object[] { orgList, checkupProject, checkupUser, createUser, type, startTime, endTime, statusList, isSummary };

    }

    protected void btnCloseRemind_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheCheckupMgr.CloseRemind(this.CurrentUser);
            this.ShowSuccessMessage("ISI.Checkup.CloseRemind.Successfully");
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    protected void btnPublish_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheCheckupMgr.Publish(this.CurrentUser);
            this.ShowSuccessMessage("ISI.Checkup.Publish.Successfully");
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
            if (ApproveEvent != null)
            {
                ApproveEvent(null, null);
                DoSearch();
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        try
        {
            if (CloseEvent != null)
            {
                CloseEvent(null, null);
                DoSearch();
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    private void GenerateTree()
    {
        IList<CodeMaster> statusList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS);
        foreach (CodeMaster status in statusList)
        {
            if (status.Value == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT || status.Value == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL
                    || status.Value == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE)
            {
                this.astvMyTree.RootNode.AppendChild(new ASTreeViewLinkNode(this.TheLanguageMgr.TranslateMessage("ISI.Status." + status.Value, CurrentUser), status.Value, string.Empty));
            }
        }
        this.astvMyTree.RootNode.ChildNodes[0].CheckedState = ASTreeViewCheckboxState.Checked;
        //this.astvMyTree.RootNode.ChildNodes[1].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser);

        IList<CodeMaster> orgList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_ISI_ORG);
        foreach (CodeMaster org in orgList)
        {
            this.astvMyTreeOrg.RootNode.AppendChild(new ASTreeViewLinkNode(org.Value, org.Value, string.Empty));
        }
        /*
        this.astvMyTreeOrg.RootNode.ChildNodes[0].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.RootNode.ChildNodes[1].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.RootNode.ChildNodes[2].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.RootNode.ChildNodes[3].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.RootNode.ChildNodes[4].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.RootNode.ChildNodes[5].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.RootNode.ChildNodes[6].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.RootNode.ChildNodes[7].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.RootNode.ChildNodes[8].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.RootNode.ChildNodes[9].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.RootNode.ChildNodes[10].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTreeOrg.InitialDropdownText = this.astvMyTreeOrg.RootNode.ChildNodes[0].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[1].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[2].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[3].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[4].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[5].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[6].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[7].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[8].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[9].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[10].NodeValue;
        this.astvMyTreeOrg.DropdownText = this.astvMyTreeOrg.RootNode.ChildNodes[0].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[1].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[2].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[3].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[4].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[5].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[6].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[7].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[8].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[9].NodeValue + "," + this.astvMyTreeOrg.RootNode.ChildNodes[10].NodeValue;
        */
    }
}
