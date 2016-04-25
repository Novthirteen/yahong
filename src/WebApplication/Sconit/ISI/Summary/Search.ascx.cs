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
using com.Sconit.ISI.Entity.Util;
using Geekees.Common.Controls;

public partial class ISI_Summary_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnExport.Visible = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE);
            GenerateTree();
            tbDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
        }
    }

    private void GenerateTree()
    {
        IList<CodeMaster> statusList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_SUMMARY_STATUS);
        foreach (CodeMaster status in statusList)
        {
            this.astvMyTree.RootNode.AppendChild(new ASTreeViewLinkNode(this.TheLanguageMgr.TranslateMessage("ISI.Status." + status.Value, CurrentUser), status.Value, string.Empty));
        }
        /*
        this.astvMyTree.RootNode.ChildNodes[0].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[1].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[3].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[4].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser);
         * */
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        string ym = this.tbDate.Text.Trim();
        if (!string.IsNullOrEmpty(ym))
        {
            var date = DateTime.Parse(ym);
            var evaluationList = this.TheEvaluationMgr.GetEvaluation(date, false);
            this.TheSummaryMgr.SummaryRemind4(date, evaluationList, this.CurrentUser);
            this.ShowSuccessMessage("ISI.Summary.ExportSummary.Successfully", ym);
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

        if (actionParameter.ContainsKey("Code"))
        {
            this.tbCode.Text = actionParameter["Code"];
        }
        if (actionParameter.ContainsKey("User"))
        {
            this.tbUser.Text = actionParameter["User"];
        }
        if (actionParameter.ContainsKey("Date"))
        {
            this.tbDate.Text = actionParameter["Date"];
        }
    }

    protected override void DoSearch()
    {
        string code = this.tbCode.Text.Trim();
        string user = this.tbUser.Text.Trim();
        string date = this.tbDate.Text.Trim();

        if (this.cbNoSubmit.Checked && String.IsNullOrEmpty(date))
        {
            this.ShowWarningMessage("ISI.Summary.Error.QueriesHaveNotBeenSubmittedPleaseSelectAMonth");
            return;
        }

        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(Summary));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(Summary)).SetProjection(Projections.Count("Code"));
            if (code != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Code", code));
                selectCountCriteria.Add(Expression.Eq("Code", code));
            }

            #region status
            IList<string> statusList = new List<string>();
            List<ASTreeViewNode> nodes = this.astvMyTree.GetCheckedNodes();
            foreach (ASTreeViewNode node in nodes)
            {
                statusList.Add(node.NodeValue);
            }

            if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) && !this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE))
            {
                selectCriteria.Add(Expression.Or(Expression.Eq("CreateUser", CurrentUser.Code), Expression.Or(Expression.Eq("SubmitUser", CurrentUser.Code), Expression.Not(Expression.Eq("Status", ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE)))));
                selectCountCriteria.Add(Expression.Or(Expression.Eq("CreateUser", CurrentUser.Code), Expression.Or(Expression.Eq("SubmitUser", CurrentUser.Code), Expression.Not(Expression.Eq("Status", ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE)))));
            }

            if (statusList != null && statusList.Count > 0)
            {
                selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
                selectCountCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
            }

            #endregion

            if (user != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("UserCode", user));
                selectCountCriteria.Add(Expression.Eq("UserCode", user));
            }
            if (date != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Date", DateTime.Parse(date)));
                selectCountCriteria.Add(Expression.Eq("Date", DateTime.Parse(date)));
            }

            SearchEvent((new object[] { selectCriteria, selectCountCriteria, this.cbNoSubmit.Checked, date }), null);

            #endregion
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }
}
