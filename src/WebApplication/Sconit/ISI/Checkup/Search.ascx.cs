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

public partial class ISI_Checkup_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
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
        tbCheckupProject.ServiceParameter = "string:" + this.ModuleType;
        if (!IsPostBack)
        {
            GenerateTree();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        if (actionParameter.ContainsKey("Department"))
        {
            this.ddlDepartment.SelectedValue = actionParameter["Department"];
        }
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
        if (actionParameter.ContainsKey("SummaryCode"))
        {
            this.tbSummaryCode.Text = actionParameter["SummaryCode"];
        }
        
    }

    protected override void DoSearch()
    {
        string department = this.ddlDepartment.SelectedValue;
        string checkupProject = this.tbCheckupProject.Text.Trim();
        string checkupUser = tbCheckupUser.Text.Trim();
        string createUser = this.tbCreateUser.Text.Trim();
        string summaryCode = this.tbSummaryCode.Text.Trim();
        
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

        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(Checkup));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(Checkup)).SetProjection(Projections.Count("Id"));

            #region status
            IList<string> statusList = new List<string>();
            List<ASTreeViewNode> nodes = this.astvMyTree.GetCheckedNodes();
            foreach (ASTreeViewNode node in nodes)
            {
                statusList.Add(node.NodeValue);
            }

            #endregion

            if (statusList != null && statusList.Count > 0)
            {
                selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
                selectCountCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
            }
            

            if (!string.IsNullOrEmpty(checkupProject))
            {
                selectCriteria.Add(Expression.Eq("CheckupProject.Code", checkupProject));
                selectCountCriteria.Add(Expression.Eq("CheckupProject.Code", checkupProject));
            }

            if (!string.IsNullOrEmpty(department))
            {
                selectCriteria.Add(Expression.Like("Department", department, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Department", department, MatchMode.Anywhere));
            }
            if (!string.IsNullOrEmpty(summaryCode))
            {
                selectCriteria.Add(Expression.Eq("SummaryCode", summaryCode));
                selectCountCriteria.Add(Expression.Eq("SummaryCode", summaryCode));
            }
            if (!string.IsNullOrEmpty(createUser))
            {
                selectCriteria.Add(Expression.Eq("CreateUser", createUser));
                selectCountCriteria.Add(Expression.Eq("CreateUser", createUser));
            }
            if (!string.IsNullOrEmpty(createUser))
            {
                selectCriteria.Add(Expression.Eq("CreateUser", createUser));
                selectCountCriteria.Add(Expression.Eq("CreateUser", createUser));
            }
            if (!string.IsNullOrEmpty(checkupUser))
            {
                selectCriteria.Add(Expression.Eq("CheckupUser", checkupUser));
                selectCountCriteria.Add(Expression.Eq("CheckupUser", checkupUser));
            }
            if (startTime.HasValue)
            {
                selectCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Ge("CreateDate", startTime)));
                selectCountCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Ge("CreateDate", startTime)));
            }
            if (endTime.HasValue)
            {
                selectCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Le("CreateDate", endTime)));
                selectCountCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Le("CreateDate", endTime)));
            }

            if (!this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP)
                    && !this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_CLOSECHECKUP)
                    && !this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_CREATECHECKUP))
            {
                selectCriteria.Add(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT, ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL, ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE }));
                selectCountCriteria.Add(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT, ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL, ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE }));
            }

            selectCriteria.Add(Expression.Eq("Type", this.ModuleType));
            selectCountCriteria.Add(Expression.Eq("Type", this.ModuleType));

            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
            #endregion
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }

    private void GenerateTree()
    {
        IList<CodeMaster> statusList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS);
        foreach (CodeMaster status in statusList)
        {
            this.astvMyTree.RootNode.AppendChild(new ASTreeViewLinkNode(this.TheLanguageMgr.TranslateMessage("ISI.Status." + status.Value, CurrentUser), status.Value, string.Empty));
        }
        this.astvMyTree.RootNode.ChildNodes[0].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[1].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[3].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser);
    }
}
