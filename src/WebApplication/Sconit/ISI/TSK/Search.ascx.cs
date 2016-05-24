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
using com.Sconit.Web;
using com.Sconit.Utility;
using com.Sconit.Entity;
using NHibernate.Transform;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using Geekees.Common.Controls;
using com.Sconit.Entity.MasterData;
using System.Text;
using NHibernate.SqlCommand;

public partial class ISI_TSK_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    public event EventHandler BatchEvent;
    private IDictionary<string, string> parameter = new Dictionary<string, string>();

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
        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            this.btnBatch.Visible = true;
        }

        if (this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
        {
            this.btnNew.FunctionId = string.Empty;
            this.btnBatch.FunctionId = string.Empty;
        }
        else
        {
            this.btnNew.FunctionId = "Create" + this.ModuleType;
            this.btnBatch.FunctionId = "CreateBatch" + this.ModuleType;
        }
        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            tbTaskSubType.ServiceMethod = "GetProjectTaskSubType";
            this.tbTaskSubType.ServiceParameter = "string:" + this.ModuleType + ",string:" + this.CurrentUser.Code + ",bool:false";
        }
        else
        {
            tbTaskSubType.ServiceParameter = "string:" + this.ModuleType + ",string:" + this.CurrentUser.Code;
        }

        tbTaskSubType.DataBind();

        if (!IsPostBack)
        {
            GenerateTree();
            if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
            {
                //this.isProject.Visible = true;
                this.lblSeq.Visible = true;
                this.tbSeq.Visible = true;
                this.ddlPhase.Visible = true;
                this.lblTaskSubType.Text = "${ISI.TSK.Project}:";
                if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
                {
                    this.lblBackYards.Text = "${ISI.TSK.RefTask}:";
                    this.lblSubject.Text = "${ISI.TSK.PrjIss.Subject}:";
                    this.lblDesc1.Text = "${ISI.TSK.PrjIss.Desc1}:";
                }
            }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (SearchEvent != null)
        {
            DoSearch();
        }
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        this.parameter = actionParameter;
        if (actionParameter.ContainsKey("Code"))
        {
            this.tbCode.Text = actionParameter["Code"];
        }
        if (actionParameter.ContainsKey("Subject"))
        {
            this.tbSubject.Text = actionParameter["Subject"];
        }
        if (actionParameter.ContainsKey("Desc1"))
        {
            this.tbDesc1.Text = actionParameter["Desc1"];
        }
        if (actionParameter.ContainsKey("TaskSubType"))
        {
            this.tbTaskSubType.Text = actionParameter["TaskSubType"];
        }
        if (actionParameter.ContainsKey("TaskAddress"))
        {
            this.tbTaskAddress.Text = actionParameter["TaskAddress"];
        }
        if (actionParameter.ContainsKey("Email"))
        {
            this.tbEmail.Text = actionParameter["Email"];
        }
        if (actionParameter.ContainsKey("MobilePhone"))
        {
            this.tbMobilePhone.Text = actionParameter["MobilePhone"];
        }
        if (actionParameter.ContainsKey("Priority"))
        {
            this.ddlPriority.SelectedValue = actionParameter["Priority"];
        }
        if (actionParameter.ContainsKey("BackYards"))
        {
            this.tbBackYards.Text = actionParameter["BackYards"];
        }
        if (actionParameter.ContainsKey("Phase"))
        {
            this.ddlPhase.SelectedValue = actionParameter["Phase"];
        }
        if (actionParameter.ContainsKey("Seq"))
        {
            this.tbSeq.Text = actionParameter["Seq"];
        }
    }

    protected override void DoSearch()
    {
        string code = this.tbCode.Text.Trim();
        string backYards = this.tbBackYards.Text.Trim();
        string desc1 = this.tbDesc1.Text.Trim();
        string taskSubType = this.tbTaskSubType.Text.Trim();
        string taskAddress = this.tbTaskAddress.Text.Trim();
        //string status = this.ddlStatus.SelectedValue;
        string mobilePhone = this.tbMobilePhone.Text.Trim();
        string email = this.tbEmail.Text.Trim();
        string subject = this.tbSubject.Text.Trim();
        string priority = this.ddlPriority.SelectedValue;

        #region status
        IList<string> statusList = new List<string>();
        List<ASTreeViewNode> nodes = this.astvMyTree.GetCheckedNodes();
        foreach (ASTreeViewNode node in nodes)
        {
            statusList.Add(node.NodeValue);
        }

        #endregion

        #region DetachedCriteria

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskMstr))
                        .SetProjection(Projections.Distinct(Projections.ProjectionList()
                            .Add(Projections.Property("Code").As("Code"))
                            .Add(Projections.Property("BackYards").As("BackYards"))
                            .Add(Projections.Property("TaskAddress").As("TaskAddress"))
                            .Add(Projections.Property("TaskSubType").As("TaskSubType"))
                            .Add(Projections.Property("Subject").As("Subject"))
                            .Add(Projections.Property("Phase").As("Phase"))
                            .Add(Projections.Property("Seq").As("Seq"))
                            .Add(Projections.Property("Status").As("Status"))
                            .Add(Projections.Property("Priority").As("Priority"))
                            .Add(Projections.Property("Flag").As("Flag"))
                            .Add(Projections.Property("Color").As("Color"))
                            .Add(Projections.Property("CreateUserNm").As("CreateUserNm"))
                            .Add(Projections.Property("AssignStartUser").As("AssignStartUser"))
                            .Add(Projections.Property("SchedulingStartUser").As("SchedulingStartUser"))
                            ));

        selectCriteria.CreateAlias("TaskSubType", "tst");

        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(TaskMstr))
            .SetProjection(Projections.CountDistinct("Code"));


        selectCountCriteria.CreateAlias("TaskSubType", "tst");

        if (!string.IsNullOrEmpty(code))
        {
            selectCriteria.Add(Expression.Like("Code", code, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Code", code, MatchMode.Anywhere));
        }

        if (!string.IsNullOrEmpty(backYards))
        {
            selectCriteria.Add(Expression.Like("BackYards", backYards, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("BackYards", backYards, MatchMode.Anywhere));
        }

        if (!string.IsNullOrEmpty(email))
        {
            selectCriteria.Add(Expression.Like("Email", email, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Email", email, MatchMode.Anywhere));
        }

        if (!string.IsNullOrEmpty(mobilePhone))
        {
            selectCriteria.Add(Expression.Like("MobilePhone", mobilePhone, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("MobilePhone", mobilePhone, MatchMode.Anywhere));
        }

        if (!string.IsNullOrEmpty(desc1))
        {
            selectCriteria.Add(Expression.Like("Desc1", desc1, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Desc1", desc1, MatchMode.Anywhere));
        }
        if (!string.IsNullOrEmpty(taskSubType))
        {
            selectCriteria.Add(Expression.Eq("tst.Code", taskSubType));
            selectCountCriteria.Add(Expression.Eq("tst.Code", taskSubType));
        }

        if (!string.IsNullOrEmpty(taskAddress))
        {
            selectCriteria.Add(Expression.Like("TaskAddress", taskAddress, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("TaskAddress", taskAddress, MatchMode.Anywhere));
        }

        if (!string.IsNullOrEmpty(subject))
        {
            selectCriteria.Add(Expression.Like("Subject", subject, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Subject", subject, MatchMode.Anywhere));
        }

        if (statusList != null && statusList.Count > 0)
        {
            selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
            selectCountCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
        }

        if (!string.IsNullOrEmpty(priority))
        {
            selectCriteria.Add(Expression.Eq("Priority", priority));
            selectCountCriteria.Add(Expression.Eq("Priority", priority));
        }

        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            string phase = this.ddlPhase.SelectedValue;
            string seq = this.tbSeq.Text;

            if (!string.IsNullOrEmpty(phase))
            {
                selectCriteria.Add(Expression.Eq("Phase", phase));
                selectCountCriteria.Add(Expression.Eq("Phase", phase));
            }
            if (!string.IsNullOrEmpty(seq))
            {
                selectCriteria.Add(Expression.Eq("Seq", seq));
                selectCountCriteria.Add(Expression.Eq("Seq", seq));
            }
        }

        #region 权限的过滤

        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PRIVACY ||
                !(this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                    || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN)
                    || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)))
        {
            /*
            DetachedCriteria[] tstCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(this.CurrentUser.Code,
                            ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);

            //创建人
            selectCriteria.Add(
                    Expression.Or(
                        Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                        Subqueries.PropertyIn("tst.Code", tstCrieteria[1])));

            selectCountCriteria.Add(
                    Expression.Or(
                        Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                        Subqueries.PropertyIn("tst.Code", tstCrieteria[1])));
            */

            string[] propertyNames = new string[] { "CreateUser", string.Empty, "AssignStartUser", "SchedulingStartUser", "tst.AssignUpUser", "tst.StartUpUser", "tst.CloseUpUser", "tst.AssignUser", "tst.ViewUser", "tst.ECUser", "Type", "ApprovalUser", "tst.Code" };

            ISIUtil.SetNoVierUserCriteria(selectCriteria, this.CurrentUser, propertyNames);
            ISIUtil.SetNoVierUserCriteria(selectCountCriteria, this.CurrentUser, propertyNames);

        }

        //selectCriteria.SetResultTransformer(new NHibernate.Transform.DistinctRootEntityResultTransformer());
        //selectCountCriteria.SetResultTransformer(NHibernate.CriteriaUtil.DistinctRootEntity);
        #endregion

        selectCriteria.Add(Expression.Eq("Type", this.ModuleType));
        selectCountCriteria.Add(Expression.Eq("Type", this.ModuleType));

        selectCriteria.SetResultTransformer(Transformers.AliasToBean(typeof(TaskMstr)));
        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            selectCriteria.AddOrder(Order.Asc("TaskSubType.Code"));
            selectCriteria.AddOrder(Order.Asc("Phase"));
            selectCriteria.AddOrder(Order.Asc("Seq"));
        }

        SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
        #endregion

    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }
    protected void btnBatch_Click(object sender, EventArgs e)
    {
        BatchEvent(sender, e);
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
        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser);
    }
}
