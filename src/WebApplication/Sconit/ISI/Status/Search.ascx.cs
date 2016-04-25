using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Entity;
using com.Sconit.Web;
using NHibernate.Expression;
using com.Sconit.Entity.View;
using com.Sconit.Utility;
using com.Sconit.Entity.MasterData;
using Geekees.Common.Controls;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using NHibernate.Transform;
using System.Collections;

public partial class ISI_Status_Search : SearchModuleBase
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
    public EventHandler NewEvent;
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
    public bool IsToDoList
    {
        get
        {
            return ViewState["IsToDoList"] != null ? (bool)ViewState["IsToDoList"] : false;
        }
        set
        {
            ViewState["IsToDoList"] = value;
        }
    }

    public event EventHandler SearchEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT)
        {
            tbTaskSubType.ServiceMethod = "GetProjectTaskSubType";
            this.tbTaskSubType.ServiceParameter = "string:" + ISIConstants.ISI_TASK_TYPE_PROJECT + ",string:" + this.CurrentUser.Code + ",bool:false";
        }
        else if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW)
        {
            tbTaskSubType.ServiceMethod = "GetWFSTaskSubType";
            tbTaskSubType.ServiceParameter = "string:#ddlType,string:" + this.CurrentUser.Code;
            ckIsWF.Visible = false;

            cbExcludeAssignUser.Text = "${ISI.Status.CurrentApprovalUser}";
            cbExcludeAssignUser.ToolTip = string.Empty;
            ckApprove.Visible = false;
        }
        else
        {
            tbTaskSubType.ServiceParameter = "string:#ddlType,string:" + this.CurrentUser.Code;
        }
        tbTaskSubType.DataBind();
        if (!IsPostBack)
        {
            lblType.Visible = this.CurrentUser.Code != "tiansu";
            cbExcludeType.Visible = this.CurrentUser.Code == "tiansu";

            lblTaskSubType.Visible = this.CurrentUser.Code != "tiansu";
            cbExclude.Visible = this.CurrentUser.Code == "tiansu";

            lblPhase.Visible = this.CurrentUser.Code != "tiansu";
            cbPhase.Visible = this.CurrentUser.Code == "tiansu";

            lblStatus.Visible = this.CurrentUser.Code != "tiansu";
            cbStatus.Visible = this.CurrentUser.Code == "tiansu";


            lblStartDate.Visible = this.CurrentUser.Code != "tiansu";
            cbIsStatus.Visible = this.CurrentUser.Code == "tiansu";


            lblCreateUser.Visible = this.CurrentUser.Code != "tiansu";
            cbExcludeSubmitUser.Visible = this.CurrentUser.Code == "tiansu";

            lblAssignUser.Visible = this.ModuleType != ISIConstants.ISI_TASK_TYPE_WORKFLOW && this.CurrentUser.Code != "tiansu";
            cbExcludeAssignUser.Visible = this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW || this.CurrentUser.Code == "tiansu";

            this.btnNew.FunctionId = "Create" + this.ModuleType;
            this.ddlType.Items.RemoveAt(1);

            GenerateTree();

            if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT)
            {
                this.cbPhase.Text = "${ISI.TSK.Phase}|${ISI.TSK.Seq}:";
                this.lblPhase.Text = "${ISI.TSK.Phase}|${ISI.TSK.Seq}:";
                //this.isProject.Visible = true;
                this.tbSeq.Visible = true;
                this.ddlPhase.Visible = true;
                this.cbExclude.Text = "${ISI.TSK.Project}:";
                this.lblTaskSubType.Text = "${ISI.TSK.Project}:";
                this.rabOrderBy.Visible = true;
                this.lblOrderBy.Visible = true;
                //this.ltlType.Visible = false;
                //this.ddlType.Visible = false;
                int count = this.ddlType.Items.Count - 3;
                for (int i = 0; i < count - 1; i++)
                {
                    this.ddlType.Items.RemoveAt(1);
                }

                this.ddlType.SelectedIndex = 1;
            }
            else
            {
                this.rabOrderBy.Visible = false;
                this.lblOrderBy.Visible = false;
                this.ddlType.Items.RemoveAt(this.ddlType.Items.Count - 1);
                this.ddlType.Items.RemoveAt(this.ddlType.Items.Count - 1);
                this.ddlType.Items.RemoveAt(this.ddlType.Items.Count - 1);
                if (this.ModuleType != ISIConstants.ISI_TASK_TYPE_WORKFLOW)
                {
                    this.ddlType.Items.RemoveAt(this.ddlType.Items.Count - 1);
                }
                else
                {
                    cbExcludeAssignUser.Checked = true;
                }
                this.cbPhase.Text = "${ISI.Status.Org}:";
                this.lblPhase.Text = "${ISI.Status.Org}:";
                this.astvMyTreeOrg.Visible = true;
            }

            if (this.IsToDoList)
            {
                DoSearch();
            }
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        if (NewEvent != null)
        {
            NewEvent(sender, e);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.DoSearch();
    }

    protected override void DoSearch()
    {
        if (SearchEvent != null)
        {
            object[] param = CollectParam();
            if (param != null)
                SearchEvent(param, null);
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        IList<object> objList = new List<object>();
        IList<TaskStatusView> taskViews = this.TheCriteriaMgr.FindAll<TaskStatusView>((DetachedCriteria)(CollectParam()[0]));
        if (taskViews != null && taskViews.Count > 0)
        {
            objList.Add(taskViews);
            //objList.Add("从 " + this.tbStartDate.Text.Trim() + " 到 " + this.tbEndDate.Text.Trim());
            objList.Add(string.Empty);
            objList.Add(this.tbTaskSubType.Text.Trim());
            objList.Add(this.ddlType.SelectedValue);
            TheReportMgr.WriteToClient("Task.xls", objList, "Task.xls");
        }
    }

    private object[] CollectParam()
    {
        //string taskAddress = this.tbTaskAddress.Text.Trim();

        string type = this.ddlType.SelectedValue;
        string taskSubType = this.tbTaskSubType.Text.Trim();
        //string taskCode = this.tbTaskCode.Text.Trim();
        //string backYards = this.tbBackYards.Text.Trim();
        string assignUser = this.tbAssignUser.Text.Trim();
        string startUser = this.tbStartUser.Text.Trim();
        string createUser = this.tbCreateUser.Text.Trim();
        string desc = this.tbDesc.Text.Trim();
        string commentUser = this.tbCommentUser.Text.Trim();
        string flag = this.ddlFlag.SelectedValue;
        string color = this.ddlColor.SelectedValue;
        string priority = this.ddlPriority.SelectedValue;
        bool isOverdue = this.ckIsOverdue.Checked;
        bool isOverdue2 = this.ckIsOverdue2.Checked;
        bool isMine = this.ckIsMine.Checked;

        bool isFocus = this.ckIsFocus.Checked;
        bool isStatus = this.cbIsStatus.Checked;
        bool isWF = this.ckIsWF.Checked;
        bool isTrace = this.ckIsTrace.Checked;
        string startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        string endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
        DateTime now = DateTime.Now;
        DateTime monday = ISIUtil.GetMondayDate();

        #region status
        IList<string> statusList = new List<string>();
        if (IsToDoList)
        {
            statusList.Add(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT);
            statusList.Add(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE);
            statusList.Add(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN);
            statusList.Add(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE);
            statusList.Add(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN);
            statusList.Add(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS);
        }
        else
        {
            List<ASTreeViewNode> nodes = this.astvMyTree.GetCheckedNodes();
            foreach (ASTreeViewNode node in nodes)
            {
                statusList.Add(node.NodeValue);
            }
        }
        #endregion

        #region DetachedCriteria

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskStatusView));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(TaskStatusView))
            .SetProjection(Projections.CountDistinct("TaskCode"));

        if (isOverdue)
        {
            selectCriteria.Add(Expression.And(Expression.IsNotNull("PlanCompleteDate"), Expression.Or(Expression.And(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN }), Expression.Le("PlanCompleteDate", now))
                                , Expression.And(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE }), Expression.LeProperty("PlanCompleteDate", "CompleteDate")))));
            selectCountCriteria.Add(Expression.And(Expression.IsNotNull("PlanCompleteDate"), Expression.Or(Expression.And(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN }), Expression.Le("PlanCompleteDate", now))
                                , Expression.And(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE }), Expression.LeProperty("PlanCompleteDate", "CompleteDate")))));
        }

        if (isOverdue2)
        {
            DateTime preMonth = DateTime.Now.AddDays(-14);
            selectCriteria.Add(Expression.And(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS }), Expression.Le("StatusDate", preMonth)));
            selectCountCriteria.Add(Expression.And(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS }), Expression.Le("StatusDate", preMonth)));
        }

        if (isFocus)
        {
            selectCriteria.Add(Expression.Or(Expression.Eq("FocusUser", this.CurrentUser.Code),
                                        Expression.Or(Expression.Like("FocusUser", this.CurrentUser.Code + ",", MatchMode.Anywhere),
                                                        Expression.Or(Expression.Like("FocusUser", "," + this.CurrentUser.Code + ",", MatchMode.Anywhere),
                                                                        Expression.Like("FocusUser", "," + this.CurrentUser.Code, MatchMode.Anywhere)))));
            selectCountCriteria.Add(Expression.Or(Expression.Eq("FocusUser", this.CurrentUser.Code),
                                        Expression.Or(Expression.Like("FocusUser", this.CurrentUser.Code + ",", MatchMode.Anywhere),
                                                        Expression.Or(Expression.Like("FocusUser", "," + this.CurrentUser.Code + ",", MatchMode.Anywhere),
                                                                        Expression.Like("FocusUser", "," + this.CurrentUser.Code, MatchMode.Anywhere)))));


            selectCriteria.Add(Expression.Or(Expression.Eq("FocusUser", this.CurrentUser.Code),
                                                        Expression.Or(Expression.Like("FocusUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                       Expression.Or(Expression.Like("FocusUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                     Expression.Or(Expression.Like("FocusUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                   Expression.Like("FocusUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))));
        }
        if (isWF)
        {
            selectCriteria.Add(Expression.Eq("IsWF", true));
            selectCountCriteria.Add(Expression.Eq("IsWF", true));
        }
        if (ckUltimate.Checked)
        {
            selectCriteria.Add(Expression.Eq("Level", ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE));
            selectCountCriteria.Add(Expression.Eq("Level", ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE));
        }
        if (isTrace)
        {
            selectCriteria.Add(Expression.Eq("IsTrace", true));
            selectCountCriteria.Add(Expression.Eq("IsTrace", true));
        }
        if (!string.IsNullOrEmpty(this.TaskCode))
        {
            selectCriteria.Add(Expression.Eq("BackYards", this.TaskCode));
            selectCountCriteria.Add(Expression.Eq("BackYards", this.TaskCode));
        }
        if (!string.IsNullOrEmpty(desc))
        {
            selectCriteria.Add(Expression.Or(Expression.Like("TaskCode", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("BackYards", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("Desc1", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("Desc2", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("Subject", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("SupplierCode", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("SupplierName", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("ExpectedResults", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("FailureMode", desc, MatchMode.Anywhere), Expression.Like("FailureModeDesc", desc, MatchMode.Anywhere)))))))))));
            selectCountCriteria.Add(Expression.Or(Expression.Like("TaskCode", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("BackYards", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("Desc1", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("Desc2", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("Subject", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("SupplierCode", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("SupplierName", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("ExpectedResults", desc, MatchMode.Anywhere), Expression.Or(Expression.Like("FailureMode", desc, MatchMode.Anywhere), Expression.Like("FailureModeDesc", desc, MatchMode.Anywhere)))))))))));
        }
        if (!string.IsNullOrEmpty(flag))
        {
            selectCriteria.Add(Expression.Eq("Flag", flag));
            selectCountCriteria.Add(Expression.Eq("Flag", flag));
        }
        if (!string.IsNullOrEmpty(color))
        {
            selectCriteria.Add(Expression.Eq("Color", color));
            selectCountCriteria.Add(Expression.Eq("Color", color));
        }
        if (!string.IsNullOrEmpty(priority))
        {
            selectCriteria.Add(Expression.Eq("Priority", priority));
            selectCountCriteria.Add(Expression.Eq("Priority", priority));
        }

        if (!string.IsNullOrEmpty(commentUser))
        {
            //selectCriteria.Add(Expression.Like("CommentCreateUser", commentUser, MatchMode.Anywhere));
            //selectCountCriteria.Add(Expression.Like("CommentCreateUser", commentUser, MatchMode.Anywhere));

            DetachedCriteria commentSubCriteria = DetachedCriteria.For<CommentDetail>();
            commentSubCriteria.Add(Expression.Eq("CreateUser", commentUser));
            commentSubCriteria.Add(Expression.Ge("CreateDate", monday));
            commentSubCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("TaskCode")));
            selectCriteria.Add(Subqueries.PropertyIn("TaskCode", commentSubCriteria));
            selectCountCriteria.Add(Subqueries.PropertyIn("TaskCode", commentSubCriteria));
        }

        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT)
        {
            //PLN000001578中提出需要修改
            //2013-03-12谢总提出修改，项目跟踪要包含项目问题
            if (string.IsNullOrEmpty(type) || (!string.IsNullOrEmpty(type) && cbExcludeType.Checked))
            {
                selectCriteria.Add(Expression.Or(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT), Expression.Or(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE), Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE))));
                selectCountCriteria.Add(Expression.Or(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT), Expression.Or(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE), Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE))));
            }

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
        else if (!this.IsToDoList)
        {
            #region org
            IList<string> orgList = new List<string>();
            List<ASTreeViewNode> orgNodes = this.astvMyTreeOrg.GetCheckedNodes();
            foreach (ASTreeViewNode node in orgNodes)
            {
                orgList.Add(node.NodeValue);
            }
            #endregion

            if (orgList != null && orgList.Count > 0)
            {
                if (cbPhase.Checked)
                {
                    selectCriteria.Add(Expression.Not(Expression.In("Org", orgList.ToArray<string>())));
                    selectCountCriteria.Add(Expression.Not(Expression.In("Org", orgList.ToArray<string>())));
                }
                else
                {
                    selectCriteria.Add(Expression.In("Org", orgList.ToArray<string>()));
                    selectCountCriteria.Add(Expression.In("Org", orgList.ToArray<string>()));
                }
            }
            if (string.IsNullOrEmpty(type) || (!string.IsNullOrEmpty(type) && cbExcludeType.Checked))
            {
                selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT)));
                selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)));
                selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)));
                selectCountCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT)));
                selectCountCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)));
                selectCountCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)));
            }
        }

        if (!string.IsNullOrEmpty(type))
        {
            if (cbExcludeType.Checked)
            {
                selectCriteria.Add(Expression.Not(Expression.Eq("Type", type)));
                selectCountCriteria.Add(Expression.Not(Expression.Eq("Type", type)));
            }
            else
            {
                selectCriteria.Add(Expression.Eq("Type", type));
                selectCountCriteria.Add(Expression.Eq("Type", type));
            }
        }

        if (taskSubType != string.Empty)
        {
            if (cbExclude.Checked)
            {
                selectCriteria.Add(Expression.Not(Expression.Eq("TaskSubTypeCode", taskSubType)));
                selectCountCriteria.Add(Expression.Not(Expression.Eq("TaskSubTypeCode", taskSubType)));
            }
            else
            {
                selectCriteria.Add(Expression.Eq("TaskSubTypeCode", taskSubType));
                selectCountCriteria.Add(Expression.Eq("TaskSubTypeCode", taskSubType));
            }
        }

        if (statusList != null && statusList.Count > 0)
        {
            if (cbStatus.Checked)
            {
                selectCriteria.Add(Expression.Not(Expression.In("Status", statusList.ToArray<string>())));
                selectCountCriteria.Add(Expression.Not(Expression.In("Status", statusList.ToArray<string>())));
            }
            else
            {
                selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
                selectCountCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
            }
        }
        if (!string.IsNullOrEmpty(assignUser))
        {

            if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW)
            {
                if (cbExcludeAssignUser.Checked)
                {
                    //当前审批人
                    selectCriteria.Add(Expression.Or(Expression.Eq("CurrentApprovalUser", assignUser),
                                                                                         Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + assignUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Eq("CurrentApprovalUser", assignUser),
                                                                                                                      Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + assignUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_USER_SEPRATOR + assignUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                  Expression.Like("CurrentApprovalUser", ISIConstants.ISI_USER_SEPRATOR + assignUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)))))));

                    selectCountCriteria.Add(Expression.Or(Expression.Eq("CurrentApprovalUser", assignUser),
                                                                                         Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + assignUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Eq("CurrentApprovalUser", assignUser),
                                                                                                                      Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + assignUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_USER_SEPRATOR + assignUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                  Expression.Like("CurrentApprovalUser", ISIConstants.ISI_USER_SEPRATOR + assignUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)))))));


                }
                else
                {
                    //审批列表
                    selectCriteria.Add(
                            Expression.And(Expression.Eq("IsWF", true),
                                           Expression.Or(Expression.Eq("ApprovalUser", assignUser),
                                                         Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + assignUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("ApprovalUser", assignUser),
                                                                                      Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + assignUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + assignUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + assignUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))));
                    selectCountCriteria.Add(
                                Expression.And(Expression.Eq("IsWF", true),
                                               Expression.Or(Expression.Eq("ApprovalUser", assignUser),
                                                             Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + assignUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                            Expression.Or(Expression.Eq("ApprovalUser", assignUser),
                                                                                          Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + assignUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + assignUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                      Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + assignUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))));


                }
            }
            else
            {
                if (cbExcludeAssignUser.Checked)
                {
                    selectCriteria.Add(Expression.Not(Expression.Eq("AssignUser", assignUser)));
                    selectCountCriteria.Add(Expression.Not(Expression.Eq("AssignUser", assignUser)));
                }
                else
                {
                    selectCriteria.Add(Expression.Eq("AssignUser", assignUser));
                    selectCountCriteria.Add(Expression.Eq("AssignUser", assignUser));
                }
            }
        }
        if (!isStatus)
        {
            if (!string.IsNullOrEmpty(startDate))
            {
                selectCriteria.Add(Expression.Ge("SubmitDate", DateTime.Parse(startDate)));
                selectCountCriteria.Add(Expression.Ge("SubmitDate", DateTime.Parse(startDate)));
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                selectCriteria.Add(Expression.Lt("SubmitDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
                selectCountCriteria.Add(Expression.Lt("SubmitDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
            }
        }else
        {
            if (!string.IsNullOrEmpty(startDate))
            {
                selectCriteria.Add(Expression.Le("SubmitDate", DateTime.Parse(startDate)));
                selectCountCriteria.Add(Expression.Le("SubmitDate", DateTime.Parse(startDate)));
                selectCriteria.Add(Expression.Ge("StatusDate", DateTime.Parse(startDate)));
                selectCountCriteria.Add(Expression.Ge("StatusDate", DateTime.Parse(startDate)));                
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                selectCriteria.Add(Expression.Le("SubmitDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
                selectCountCriteria.Add(Expression.Le("SubmitDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
            }
        }
        if (!string.IsNullOrEmpty(startUser))
        {
            if (ckApprove.Checked)
            {
                selectCriteria.Add(
                            Expression.And(Expression.Eq("IsWF", true),
                                           Expression.Or(Expression.Eq("ApprovalUser", startUser),
                                                         Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("ApprovalUser", startUser),
                                                                                      Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))));
                selectCountCriteria.Add(
                            Expression.And(Expression.Eq("IsWF", true),
                                           Expression.Or(Expression.Eq("ApprovalUser", startUser),
                                                         Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("ApprovalUser", startUser),
                                                                                      Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))));


            }
            else if (!ckFirst.Checked)
            {
                #region 不控制是否第一责任人
                selectCriteria.Add(
                        Expression.Or(
                            Expression.And(Expression.Eq("IsWF", true),
                                           Expression.Or(Expression.Eq("ApprovalUser", startUser),
                                                         Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("ApprovalUser", startUser),
                                                                                      Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),
                    Expression.Or(
                            Expression.And(Expression.Or(Expression.IsNull("SchedulingStartUser"), Expression.Eq("SchedulingStartUser", string.Empty)),
                                           Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                         Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                                                      Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),
                            Expression.And(Expression.And(Expression.IsNotNull("SchedulingStartUser"), Expression.Not(Expression.Eq("SchedulingStartUser", string.Empty))),
                                           Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                         Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                                                      Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))))));
                selectCountCriteria.Add(
                        Expression.Or(
                            Expression.And(Expression.Eq("IsWF", true),
                                           Expression.Or(Expression.Eq("ApprovalUser", startUser),
                                                         Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("ApprovalUser", startUser),
                                                                                      Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("ApprovalUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),
                    Expression.Or(
                            Expression.And(Expression.Or(Expression.IsNull("SchedulingStartUser"), Expression.Eq("SchedulingStartUser", string.Empty)),
                                           Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                         Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                                                      Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),
                            Expression.And(Expression.And(Expression.IsNotNull("SchedulingStartUser"), Expression.Not(Expression.Eq("SchedulingStartUser", string.Empty))),
                                           Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                         Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                                                      Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))))));
                #endregion
            }
            else
            {
                #region 只查找第一责任人
                selectCriteria.Add(Expression.Eq("FirstUser", startUser));
                selectCountCriteria.Add(Expression.Eq("FirstUser", startUser));
                #endregion
            }
        }

        if (!string.IsNullOrEmpty(createUser))
        {
            if (cbExcludeSubmitUser.Checked)
            {
                selectCriteria.Add(Expression.Not(Expression.Eq("CreateUser", createUser)));
                selectCountCriteria.Add(Expression.Not(Expression.Eq("CreateUser", createUser)));
            }
            else
            {
                selectCriteria.Add(Expression.Eq("CreateUser", createUser));
                selectCountCriteria.Add(Expression.Eq("CreateUser", createUser));
            }
        }
        //selectCriteria.Add(Expression.Eq("IsWF", this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW));
        //selectCountCriteria.Add(Expression.Eq("IsWF", this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW));
        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW)
        {
            selectCriteria.Add(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_WORKFLOW));
            selectCountCriteria.Add(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_WORKFLOW));
        }
        else
        {
            selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_WORKFLOW)));
            selectCountCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_WORKFLOW)));
        }

        #region 权限的过滤
        if (this.IsToDoList)
        {
            selectCriteria.Add(Expression.Or(
                                Expression.And(Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN),
                                                Expression.Or(Expression.Eq("SubmitUser", this.CurrentUser.Code), Expression.Eq("CreateUser", this.CurrentUser.Code))),
                //当前审批人
                                    Expression.Or(Expression.And(Expression.Or(Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT), Expression.Or(Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE), Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE))),

                                                                            Expression.Or(Expression.Eq("CurrentApprovalUser", this.CurrentUser.Code),
                                                                                         Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Eq("CurrentApprovalUser", this.CurrentUser.Code),
                                                                                                                      Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                  Expression.Like("CurrentApprovalUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),


                                                        //执行人(未控制第一执行人)
                                                        Expression.And(Expression.Or(Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN), Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS))
                                                        ,
                                                        Expression.Or(
                                                            Expression.And(Expression.Or(Expression.IsNull("SchedulingStartUser"), Expression.Eq("SchedulingStartUser", string.Empty)),
                                                                           Expression.Or(Expression.Eq("AssignStartUser", this.CurrentUser.Code),
                                                                                         Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Eq("AssignStartUser", this.CurrentUser.Code),
                                                                                                                      Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                  Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),
                                                            Expression.And(Expression.And(Expression.IsNotNull("SchedulingStartUser"), Expression.Not(Expression.Eq("SchedulingStartUser", string.Empty))),
                                                                           Expression.Or(Expression.Eq("SchedulingStartUser", this.CurrentUser.Code),
                                                                                         Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Eq("SchedulingStartUser", this.CurrentUser.Code),
                                                                                                                      Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                  Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))))))));


            selectCountCriteria.Add(Expression.Or(
                                    Expression.And(Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN),
                                                    Expression.Or(Expression.Eq("SubmitUser", this.CurrentUser.Code), Expression.Eq("CreateUser", this.CurrentUser.Code))),
                //当前审批人
                                        Expression.Or(Expression.And(Expression.Or(Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT), Expression.Or(Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE), Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE))),

                                                                                Expression.Or(Expression.Eq("CurrentApprovalUser", this.CurrentUser.Code),
                                                                                             Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Eq("CurrentApprovalUser", this.CurrentUser.Code),
                                                                                                                          Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("CurrentApprovalUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                      Expression.Like("CurrentApprovalUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),


                                                            //执行人(未控制第一执行人)
                                                            Expression.And(Expression.Or(Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN), Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS))
                                                            ,
                                                            Expression.Or(
                                                                Expression.And(Expression.Or(Expression.IsNull("SchedulingStartUser"), Expression.Eq("SchedulingStartUser", string.Empty)),
                                                                               Expression.Or(Expression.Eq("AssignStartUser", this.CurrentUser.Code),
                                                                                             Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Eq("AssignStartUser", this.CurrentUser.Code),
                                                                                                                          Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                      Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),
                                                                Expression.And(Expression.And(Expression.IsNotNull("SchedulingStartUser"), Expression.Not(Expression.Eq("SchedulingStartUser", string.Empty))),
                                                                               Expression.Or(Expression.Eq("SchedulingStartUser", this.CurrentUser.Code),
                                                                                             Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Eq("SchedulingStartUser", this.CurrentUser.Code),
                                                                                                                          Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                      Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))))))));



        }
        else if (isMine || (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
           && !this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN)))
        {
            if (isMine || !(CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                    || CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE)
                    || CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)))
            {
                #region 非观察人

                ISIUtil.SetNoVierUserCriteria(selectCriteria, this.CurrentUser);
                ISIUtil.SetNoVierUserCriteria(selectCountCriteria, this.CurrentUser);

                #endregion
            }
            else
            {
                #region 观察人

                string[] propertyNames = new string[] { "Type", "CreateUser", string.Empty, "AssignStartUser", "SchedulingStartUser", "AssignUpUser", "StartUpUser", "CloseUpUser", "TaskSubTypeAssignUser", "ViewUser", "ECUser", "TaskSubTypeCode" };
                ISIUtil.SetVierUserCriteria(selectCriteria, this.CurrentUser.Code, propertyNames);
                ISIUtil.SetVierUserCriteria(selectCountCriteria, this.CurrentUser.Code, propertyNames);

                #endregion
            }
        }

        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW)
        {
            selectCriteria.Add(Expression.Eq("IsWF", true));
            selectCountCriteria.Add(Expression.Eq("IsWF", true));
        }

        #endregion

        if (rabOrderBy.SelectedValue == "PlanStartDate")
        {
            selectCriteria.AddOrder(Order.Asc("PlanStartDate"));
        }
        else if (rabOrderBy.SelectedValue == "TaskSubTypeCodePhaseSeq" && (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE))
        {
            selectCriteria.AddOrder(Order.Asc("TaskSubTypeCode"));
            selectCriteria.AddOrder(Order.Asc("Phase"));
            selectCriteria.AddOrder(Order.Asc("Seq"));
        }

        //selectCriteria.AddOrder(Order.Desc("CreateDate"));

        #endregion

        return new object[] { selectCriteria, selectCountCriteria, type, this.cbHighlight.Checked, desc, isMine, isFocus, isStatus && (!string.IsNullOrEmpty(startDate) || !string.IsNullOrEmpty(endDate)), startDate, endDate };

    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        //if (actionParameter.ContainsKey("Location"))
        //{
        //    this.tbLocation.Text = actionParameter["Location"];
        //}
        //if (actionParameter.ContainsKey("Item"))
        //{
        //    this.tbItem.Text = actionParameter["Item"];
        //}
        //if (actionParameter.ContainsKey("EffDate"))
        //{
        //    this.tbEffDate.Text = actionParameter["EffDate"];
        //}
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

        //this.astvMyTree.RootNode.ChildNodes[7].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[8].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[9].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[10].CheckedState = ASTreeViewCheckboxState.Checked;

        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[8].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[9].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[10].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[8].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[9].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[10].NodeValue, CurrentUser);

        IList<CodeMaster> orgList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_ISI_ORG);
        foreach (CodeMaster org in orgList)
        {
            this.astvMyTreeOrg.RootNode.AppendChild(new ASTreeViewLinkNode(org.Value, org.Value, string.Empty));
        }

    }
}

