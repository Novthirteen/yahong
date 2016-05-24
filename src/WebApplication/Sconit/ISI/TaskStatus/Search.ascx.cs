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

public partial class ISI_TaskStatus_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
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

    private void PageCleanup()
    {
        this.TaskCode = null;
        this.tbDesc.Text = string.Empty;
        this.tbEndDate.Text = string.Empty;
        this.tbStartDate.Text = string.Empty;
        this.ddlColor.SelectedIndex = -1;
        this.ddlFlag.SelectedIndex = -1;
        this.ddlType.SelectedIndex = -1;
        this.tbTaskSubType.Text = string.Empty;
        this.tbTaskCode.Text = string.Empty;
        this.tbUser.Text = string.Empty;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.ddlFlag.Items.Remove(ListItem.FromString(ISIConstants.CODE_MASTER_ISI_FLAG_DI1));
            this.ddlFlag.Items.Remove(ListItem.FromString(ISIConstants.CODE_MASTER_ISI_FLAG_DI5));
        }

        if (string.IsNullOrEmpty(this.TaskCode))
        {
            this.btnNew.Visible = false;
            this.btnBack.Visible = false;
            this.isReport.Visible = true;
            tbTaskSubType.ServiceParameter = "string:#ddlType,string:" + this.CurrentUser.Code;
            tbTaskSubType.DataBind();
        }
        else
        {
            TaskMstr task = this.TheTaskMstrMgr.LoadTaskMstr(this.TaskCode);
            if (task != null)
            {
                if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                {
                    bool isISIAdmin = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN);
                    bool isTaskFlowAdmin = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN);
                    bool isCloser = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE);

                    if (this.TheTaskMgr.HasPermission(task, isISIAdmin, isTaskFlowAdmin, isCloser, this.CurrentUser.Code)
                                    || ISIUtil.Contains(task.StartedUser, this.CurrentUser.Code)
                                    || task.CreateUser == this.CurrentUser.Code
                                    || task.SubmitUser == this.CurrentUser.Code)
                    {
                        this.btnNew.Visible = true;
                    }
                }
                else
                {
                    this.btnNew.Visible = false;
                }
            }
            this.btnBack.Visible = true;
            this.isReport.Visible = false;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        if (actionParameter.ContainsKey("TaskCode"))
        {
            this.TaskCode = actionParameter["TaskCode"];
        }
        if (actionParameter.ContainsKey("Desc"))
        {
            this.tbDesc.Text = actionParameter["Desc"];
        }
        if (actionParameter.ContainsKey("Flag"))
        {
            this.ddlFlag.SelectedValue = actionParameter["Flag"];
        }
        if (actionParameter.ContainsKey("Color"))
        {
            this.ddlColor.SelectedValue = actionParameter["Color"];
        }
    }

    protected override void DoSearch()
    {
        if (SearchEvent != null)
        {
            string desc = this.tbDesc.Text.Trim();
            string flag = this.ddlFlag.SelectedValue;
            string color = this.ddlColor.SelectedValue;
            string user = this.tbUser.Text.Trim();

            DateTime? startTime = null;
            if (this.tbStartDate.Text.Trim() != string.Empty)
            {
                startTime = DateTime.Parse(this.tbStartDate.Text.Trim());
            }
            DateTime? endTime = null;
            if (this.tbEndDate.Text.Trim() != string.Empty)
            {
                endTime = DateTime.Parse(this.tbEndDate.Text.Trim());
            }

            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskStatus));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(TaskStatus)).SetProjection(Projections.Count("Id"));


            if (!string.IsNullOrEmpty(this.TaskCode))
            {
                selectCriteria.Add(Expression.Eq("TaskCode", this.TaskCode));
                selectCountCriteria.Add(Expression.Eq("TaskCode", this.TaskCode));
            }
            else
            {
                string taskCode = this.tbTaskCode.Text.Trim();
                string taskSubType = this.tbTaskSubType.Text.Trim();
                string type = this.ddlType.SelectedValue;

                DetachedCriteria taskCriteria = DetachedCriteria.For<TaskMstr>();
                if (!string.IsNullOrEmpty(taskCode))
                {
                    selectCriteria.Add(Expression.Eq("TaskCode", taskCode));
                    selectCountCriteria.Add(Expression.Eq("TaskCode", taskCode));
                    taskCriteria.Add(Expression.Eq("Code", taskCode));
                }

                taskCriteria.CreateAlias("TaskSubType", "tst", NHibernate.SqlCommand.JoinType.InnerJoin);
                taskCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("Code")));

                if (!string.IsNullOrEmpty(type))
                {
                    taskCriteria.Add(Expression.Eq("tst.Type", type));
                }

                if (!string.IsNullOrEmpty(taskSubType))
                {
                    taskCriteria.Add(Expression.Eq("tst.Code", taskCode));
                }
                /*
                if (!(this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                        || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN)))
                {
                    DetachedCriteria[] tstCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(this.CurrentUser.Code,
                            ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
                    taskCriteria.Add(
                            Expression.Or(
                                Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                                    Subqueries.PropertyIn("tst.Code", tstCrieteria[1])
                        ));
                }
                */
                if (!string.IsNullOrEmpty(taskSubType) || !string.IsNullOrEmpty(type)
                        || (!(this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                                || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))))
                {
                    selectCriteria.Add(Subqueries.PropertyIn("TaskCode", taskCriteria));
                    selectCountCriteria.Add(Subqueries.PropertyIn("TaskCode", taskCriteria));
                }
            }

            if (!string.IsNullOrEmpty(user))
            {
                selectCriteria.Add(Expression.Eq("CreateUser", user));
                selectCountCriteria.Add(Expression.Eq("CreateUser", user));
            }
            if (desc != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Desc", desc, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Desc", desc, MatchMode.Anywhere));
            }

            if (flag != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Flag", flag));
                selectCountCriteria.Add(Expression.Eq("Flag", flag));
            }
            if (color != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Color", color));
                selectCountCriteria.Add(Expression.Eq("Color", color));
            }

            if (startTime.HasValue)
            {
                selectCriteria.Add(Expression.Ge("CreateDate", startTime));
                selectCountCriteria.Add(Expression.Ge("CreateDate", startTime));
            }
            if (endTime.HasValue)
            {
                selectCriteria.Add(Expression.Le("CreateDate", endTime));
                selectCountCriteria.Add(Expression.Le("CreateDate", endTime));
            }

            selectCriteria.AddOrder(Order.Desc("CreateDate"));
            selectCriteria.AddOrder(Order.Desc("Id"));
            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
            #endregion
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }
}
