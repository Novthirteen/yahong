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

public partial class ISI_Batch_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler ReplaceEvent;
    public event EventHandler DeleteEvent;
    public event EventHandler CancelEvent;
    public event EventHandler CloseEvent;
    public event EventHandler RejectEvent;
    public event EventHandler CompleteEvent;
    public event EventHandler OpenEvent;
    public event EventHandler BatchEvent;
    
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
        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT)
        {
            tbTaskSubType.ServiceMethod = "GetProjectTaskSubType";
            this.tbTaskSubType.ServiceParameter = "string:" + ISIConstants.ISI_TASK_TYPE_PROJECT + ",string:" + this.CurrentUser.Code + ",bool:false";
        }
        else
        {
            tbTaskSubType.ServiceParameter = "string:#ddlType,string:" + this.CurrentUser.Code;
        }
        tbTaskSubType.DataBind();
        if (!IsPostBack)
        {
            this.ddlType.Items.RemoveAt(1);

            GenerateTree();

            if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT)
            {
                this.lblPhase.Text = "${ISI.TSK.Phase}|${ISI.TSK.Seq}:";
                //this.isProject.Visible = true;
                this.tbSeq.Visible = true;
                this.ddlPhase.Visible = true;
                this.lblTaskSubType.Text = "${ISI.TSK.Project}:";
                //this.ltlType.Visible = false;
                //this.ddlType.Visible = false;

                this.ddlType.Items.RemoveAt(1);
                this.ddlType.Items.RemoveAt(1);
                this.ddlType.Items.RemoveAt(1);
                this.ddlType.Items.RemoveAt(4);
                this.ddlType.Items.RemoveAt(4);
                this.ddlType.Items.RemoveAt(4);
                this.ddlType.Items.RemoveAt(4);

                this.ddlType.SelectedIndex = 1;
            }
            else
            {

                this.ddlType.Items.RemoveAt(4);
                this.ddlType.Items.RemoveAt(4);
                this.ddlType.Items.RemoveAt(4);
                this.lblPhase.Text = "${ISI.Status.Org}:";
                this.astvMyTreeOrg.Visible = true;
            }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }
    protected void btnComplete_Click(object sender, EventArgs e)
    {
        if (CompleteEvent != null)
        {
            CompleteEvent(null, null);

            DoSearch();
        }
    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        if (CloseEvent != null)
        {
            CloseEvent(null, null);

            DoSearch();
        }
    }
    protected void btnOpen_Click(object sender, EventArgs e)
    {
        if (OpenEvent != null)
        {
            OpenEvent(null, null);

            DoSearch();
        }
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (RejectEvent != null)
        {
            RejectEvent(null, null);

            DoSearch();
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (CancelEvent != null)
        {
            CancelEvent(null, null);

            DoSearch();
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (DeleteEvent != null)
        {
            DeleteEvent(null, null);

            DoSearch();
        }
    }

    protected void btnBatch_Click(object sender, EventArgs e)
    {

        if (BatchEvent != null &&
                    (this.rblCreate.SelectedItem.Value != string.Empty || this.cbIsCancel.Checked ||
                    this.cbIsComplete.Checked ||
                    this.rblComplete.SelectedItem.Value != string.Empty || this.cbIsOpen.Checked))
        {

            BatchEvent(new object[] {this.rblCreate.SelectedItem.Value, this.cbIsCancel.Checked ,
                    this.cbIsComplete.Checked ,
                    this.rblComplete.SelectedItem.Value , this.cbIsOpen.Checked }, null);
            
            DoSearch();
        }
    }
    protected void btnReplace_Click(object sender, EventArgs e)
    {

        if (ReplaceEvent != null && rfvStartUser1.IsValid && rfvUser.IsValid)
        {
            string srcUser = this.tbStartUser.Text.Trim();
            string targetUser = this.tbUser.Text.Trim();

            if (srcUser == targetUser)
            {
                this.ShowErrorMessage("ISI.Batch.Replace.SrcUserEqualsTargetUser");
            }
            else
            {
                ReplaceEvent(new object[] { srcUser, targetUser }, null);
            }
            DoSearch();
        }
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {


    }

    protected override void DoSearch()
    {
        if (SearchEvent != null && rfvStartUser.IsValid)
        {
            object[] param = CollectParam();
            if (param != null)
                SearchEvent(param, null);
        }
    }

    public object[] CollectParam()
    {
        string type = this.ddlType.SelectedValue;
        string taskSubType = this.tbTaskSubType.Text.Trim();
        string assignUser = this.tbAssignUser.Text.Trim();
        string startUser = this.tbStartUser.Text.Trim();

        string flag = this.ddlFlag.SelectedValue;
        string color = this.ddlColor.SelectedValue;
        string priority = this.ddlPriority.SelectedValue;
        string phase = this.ddlPhase.SelectedValue;
        string seq = this.tbSeq.Text;
        bool first = this.ckFirst.Checked;

        IList<string> orgList = new List<string>();
        List<ASTreeViewNode> orgNodes = this.astvMyTreeOrg.GetCheckedNodes();
        foreach (ASTreeViewNode node in orgNodes)
        {
            orgList.Add(node.NodeValue);
        }

        #region status
        IList<string> statusList = new List<string>();
        List<ASTreeViewNode> nodes = this.astvMyTree.GetCheckedNodes();
        foreach (ASTreeViewNode node in nodes)
        {
            statusList.Add(node.NodeValue);
        }
        #endregion

        return new object[] { type, taskSubType, assignUser, startUser, flag, color, first, priority, statusList, orgList, phase, seq };
    }
    private void GenerateTree()
    {
        IList<CodeMaster> statusList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_ISI_STATUS);
        
        foreach (CodeMaster status in statusList)
        {
            this.astvMyTree.RootNode.AppendChild(new ASTreeViewLinkNode(this.TheLanguageMgr.TranslateMessage("ISI.Status." + status.Value, CurrentUser), status.Value, string.Empty));
        }
        this.astvMyTree.RootNode.ChildNodes[1].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[3].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[4].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[5].CheckedState = ASTreeViewCheckboxState.Checked;

        this.astvMyTree.RootNode.ChildNodes[7].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[8].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[9].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[10].CheckedState = ASTreeViewCheckboxState.Checked;

        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[7].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[8].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[9].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[10].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[7].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[8].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[9].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[10].NodeValue, CurrentUser);

        IList<CodeMaster> orgList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_ISI_ORG);
        foreach (CodeMaster org in orgList)
        {
            this.astvMyTreeOrg.RootNode.AppendChild(new ASTreeViewLinkNode(org.Value, org.Value, string.Empty));
        }
    }
}
