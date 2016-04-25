using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_TSK_Detail : com.Sconit.Web.MainModuleBase
{
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

    public void InitPageParameter(string taskCode)
    {
        this.TaskCode = taskCode;
        if (!string.IsNullOrEmpty(TaskCode))
        {
            this.lgd.InnerText = "${ISI.TSK." + this.ModuleType + "}" + this.TaskCode;

            this.btnSearch_Click(null, null);
        }
        else
        {
            DateTime now = DateTime.Now;
            this.tbStartDate.Text = now.AddDays(-7).ToString("yyyy-MM-dd HH:mm");
            this.tbEndDate.Text = now.ToString("yyyy-MM-dd HH:mm");
            this.lblTaskCode.Visible = true;
            this.tbTaskCode.Visible = true;
            this.lgd.Visible = false;
            this.fs.Visible = false;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {


    }

    private void PageCleanup()
    {
        this.TaskCode = null;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string status = this.ddlStatus.SelectedValue;
        string mobilePhone = this.tbMobilePhone.Text.Trim();
        string email = this.tbEmail.Text.Trim();
        string emailStatus = this.ddlEmailStatus.SelectedValue;
        string smsStatus = this.ddlSMSStatus.SelectedValue;


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

        DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskDetail));
        if (!string.IsNullOrEmpty(TaskCode))
        {
            criteria.Add(Expression.Eq("TaskCode", this.TaskCode));
        }
        else
        {
            if (!string.IsNullOrEmpty(this.tbTaskCode.Text.Trim()))
            {
                criteria.Add(Expression.Eq("TaskCode", this.tbTaskCode.Text.Trim()));
            }

            if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                && !this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN)
                && !this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW))
            {
                criteria.Add(Subqueries.PropertyIn("TaskCode", ISIUtil.GetTaskPermissionCriteria(this.CurrentUser.Code, this.tbTaskCode.Text.Trim())));
            }
        }

        if (startTime.HasValue)
        {
            criteria.Add(Expression.Ge("CreateDate", startTime));
        }
        if (endTime.HasValue)
        {
            criteria.Add(Expression.Le("CreateDate", endTime));
        }
        if (status != string.Empty)
        {
            criteria.Add(Expression.Eq("Status", status));
        }
        if (mobilePhone != string.Empty)
        {
            criteria.Add(Expression.Eq("MobilePhone", mobilePhone));
        }
        if (email != string.Empty)
        {
            criteria.Add(Expression.Eq("Email", email));
        }
        if (emailStatus != string.Empty)
        {
            criteria.Add(Expression.Eq("EmailStatus", emailStatus));
        }
        if (smsStatus != string.Empty)
        {
            criteria.Add(Expression.Eq("SMSStatus", smsStatus));
        }
        criteria.AddOrder(Order.Desc("CreateDate"));
        this.GV_List_Detail.DataSource = TheCriteriaMgr.FindAll<TaskDetail>(criteria, 0, 500);
        this.GV_List_Detail.DataBind();

        if ((Button)sender == this.btnExport)
        {
            this.ExportXLS(this.GV_List_Detail);
        }
        else
        {
            this.fs.Visible = true;
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.btnSearch_Click(sender, e);
    }


    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.TaskCode))
        {
            this.GV_List_Detail.Columns[0].Visible = true;
            this.GV_List_Detail.Columns[1].Visible = true;
        }
        else
        {
            this.GV_List_Detail.Columns[0].Visible = false;
            this.GV_List_Detail.Columns[1].Visible = false;
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TaskDetail taskDetail = (TaskDetail)e.Row.DataItem;

            Label lblStatus = ((Label)(e.Row.FindControl("lblStatus")));
            lblStatus.Text = "${ISI.Status." + taskDetail.Status + "}";

            if (taskDetail.Level == ISIConstants.ISI_LEVEL_BASE)
            {
                e.Row.Cells[2].Text = "${ISI.Remind.0}";
            }
            else if (taskDetail.Level == ISIConstants.ISI_LEVEL_HELP)
            {
                e.Row.Cells[2].Text = "${ISI.Remind.Help}";
            }
            else if (taskDetail.Level == ISIConstants.ISI_LEVEL_COMMENT)
            {
                e.Row.Cells[2].Text = "${ISI.Remind.Comment}";
            }
            else if (taskDetail.Level == ISIConstants.ISI_LEVEL_STATUS)
            {
                e.Row.Cells[2].Text = "${ISI.Remind.Status}";
            }
            else if (taskDetail.Level == ISIConstants.ISI_LEVEL_STARTPERCENT)
            {
                e.Row.Cells[2].Text = "${ISI.Remind.Schedule}";
            }
            else if (taskDetail.Level == ISIConstants.ISI_LEVEL_COMPLETE)
            {
                e.Row.Cells[2].Text = "${ISI.Remind.OverDue}";
            }
            else if (taskDetail.Level == ISIConstants.ISI_LEVEL_OPEN)
            {
                e.Row.Cells[2].Text = "${ISI.Remind.Open}";
            }
            else if (taskDetail.Level == ISIConstants.ISI_LEVEL_APPROVE)
            {
                e.Row.Cells[2].Text = "${ISI.Remind.Approve}";
            }
            else
            {
                e.Row.Cells[2].Text = TheLanguageMgr.TranslateMessage("ISI.Remind.Up", this.CurrentUser, new string[] { taskDetail.Level });
            }
        }
    }
}