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

public partial class ISI_TSK_ProcessInstance : com.Sconit.Web.MainModuleBase
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

    public int? CurrentLevel
    {
        get
        {
            if (ViewState["CurrentLevel"] == null)
            {
                return null;
            }
            else
            {
                return int.Parse(ViewState["CurrentLevel"].ToString());
            }
        }
        set
        {
            ViewState["CurrentLevel"] = value;
        }
    }

    public void InitPageParameter(string taskCode, string moduleType)
    {
        this.TaskCode = taskCode;
        this.ModuleType = moduleType;
        if (!string.IsNullOrEmpty(TaskCode))
        {
            this.lgd.InnerText = "${ISI.TSK." + this.ModuleType + "}" + this.TaskCode;
            var level = TheHqlMgr.FindAll<object>("select t.Level from TaskMstr t where t.Code ='" + taskCode + "'")[0];
            if (level == null)
            {
                this.CurrentLevel = null;
            }
            else
            {
                this.CurrentLevel = int.Parse(level.ToString());
            }
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ProcessInstance));
            criteria.Add(Expression.Eq("TaskCode", this.TaskCode));
            criteria.AddOrder(Order.Desc("Level"));
            this.GV_List_Detail.DataSource = TheCriteriaMgr.FindAll<ProcessInstance>(criteria, 0, 500);
            this.GV_List_Detail.DataBind();

            this.GV_List_Process.DataSource = TheHqlMgr.FindAll<WFDetail>("from WFDetail where TaskCode ='" + taskCode + "' order by Id Desc ");
            this.GV_List_Process.DataBind();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {


    }
    private void PageCleanup()
    {
        this.TaskCode = null;
        this.CurrentLevel = null;
        this.ModuleType = string.Empty;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }

    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        this.GV_List_Detail.Columns[GV_List_Detail.Columns.Count - 1].Visible = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ProcessInstance processInstance = (ProcessInstance)e.Row.DataItem;
            e.Row.Cells[3].Text = "${ISI.Status." + processInstance.Status + "}";
            var level = processInstance.Level / ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL * ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
            e.Row.Cells[4].Text = "${" + level + "}" + (level != processInstance.Level ? "&nbsp;" + (processInstance.Level / ISIConstants.CODE_MASTER_WF_COUNTERSIGN_LEVEL_INTERVAL).ToString().Substring(1) : string.Empty);

            if (CurrentLevel.HasValue && processInstance.Level == CurrentLevel.Value && !processInstance.ProcessDate.HasValue)
            {
                e.Row.ForeColor = System.Drawing.Color.Blue;
            }

            Label lblUserCode = (Label)e.Row.FindControl("lblUserCode");
            Controls_TextBox tbUserCode = (Controls_TextBox)e.Row.FindControl("tbUserCode");
            CheckBox cbIsParallel = (CheckBox)e.Row.FindControl("cbIsParallel");
            CheckBox cbATicket = (CheckBox)e.Row.FindControl("cbATicket");
            CheckBox cbIsRemind = (CheckBox)e.Row.FindControl("cbIsRemind");
            CheckBox cbIsCtrl = (CheckBox)e.Row.FindControl("cbIsCtrl");
            CheckBox cbIsAccountCtrl = (CheckBox)e.Row.FindControl("cbIsAccountCtrl");
            Label lblQty = (Label)e.Row.FindControl("lblQty");
            TextBox tbQty = (TextBox)e.Row.FindControl("tbQty");
            Label lblDesc1 = (Label)e.Row.FindControl("lblDesc1");
            TextBox tbDesc1 = (TextBox)e.Row.FindControl("tbDesc1");
            RequiredFieldValidator rfvDesc1 = (RequiredFieldValidator)e.Row.FindControl("rfvDesc1");
            if (CurrentLevel.HasValue && CurrentLevel.Value <= processInstance.Level && this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN) && processInstance.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE && processInstance.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
            {
                lblUserCode.Visible = false;
                tbUserCode.Visible = true;
                tbUserCode.Text = processInstance.UserCode;
                lblQty.Visible = false;
                tbQty.Visible = true;
                cbIsParallel.Enabled = true;
                cbATicket.Enabled = true;
                cbIsRemind.Enabled = true;
                cbIsCtrl.Enabled = true;
                cbIsAccountCtrl.Enabled = true;

                lblDesc1.Visible = false;
                tbDesc1.Visible = true;
                rfvDesc1.Visible = true;
            }
            else
            {
                lblUserCode.Visible = true;
                tbUserCode.Visible = false;
                lblQty.Visible = true;
                tbQty.Visible = false;
                cbIsParallel.Enabled = false;
                cbATicket.Enabled = false;
                cbIsRemind.Enabled = false;
                cbIsCtrl.Enabled = false;
                cbIsAccountCtrl.Enabled = false;

                lblDesc1.Visible = true;
                tbDesc1.Visible = false;
                rfvDesc1.Visible = false;
                if (this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN))
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = string.Empty;
                }
            }
        }
    }
    protected void lbtnUpdate_Click(object sender, EventArgs e)
    {
        string code = ((System.Web.UI.WebControls.LinkButton)sender).CommandArgument;

        try
        {
            int id = int.Parse(code);
            var pi = this.TheProcessInstanceMgr.LoadProcessInstance(id);
            IList<ProcessInstance> processInstanceList = new List<ProcessInstance>();
            bool isUpdate = false;
            for (int i = this.GV_List_Detail.Rows.Count - 1; i >= 0; i--)
            {
                GridViewRow row = this.GV_List_Detail.Rows[i];
                int idr = int.Parse(((HiddenField)row.FindControl("hfId")).Value);
                int level = int.Parse(((HiddenField)row.FindControl("hfLevel")).Value);
                var userName = ((HiddenField)row.FindControl("hfUserNm")).Value;
                var userCode = ((HiddenField)row.FindControl("hfUserCode")).Value;
                ProcessInstance processInstance = new ProcessInstance();

                if (id == idr)
                {
                    pi.UserCode = ((Controls_TextBox)row.FindControl("tbUserCode")).Text.Trim();
                    if (!string.IsNullOrEmpty(pi.UserCode))
                    {
                        pi.UserNm = this.TheUserMgr.LoadUser(pi.UserCode).Name;
                    }
                    else
                    {
                        pi.UserNm = string.Empty;
                    }

                    if (pi.UserCode != userCode)
                    {
                        isUpdate = true;
                    }

                    pi.IsParallel = ((CheckBox)row.FindControl("cbIsParallel")).Checked;
                    pi.ATicket = ((CheckBox)row.FindControl("cbATicket")).Checked;
                    pi.IsRemind = ((CheckBox)row.FindControl("cbIsRemind")).Checked;
                    pi.IsCtrl = ((CheckBox)row.FindControl("cbIsCtrl")).Checked;
                    pi.IsAccountCtrl = ((CheckBox)row.FindControl("cbIsAccountCtrl")).Checked;
                    if (((TextBox)row.FindControl("tbQty")).Text.Trim() != string.Empty)
                    {
                        pi.Qty = decimal.Parse(((TextBox)row.FindControl("tbQty")).Text.Trim());
                    }
                    else
                    {
                        pi.Qty = null;
                    }
                    pi.Desc1 = ((TextBox)row.FindControl("tbDesc1")).Text.Trim();

                    pi.ProcessDate = DateTime.Now;
                    pi.ProcessUser = this.CurrentUser.Code;
                    pi.ProcessUserNm = this.CurrentUser.Name;
                    this.TheProcessInstanceMgr.UpdateProcessInstance(pi);
                    processInstance.Status = pi.Status;
                    processInstance.UserCode = pi.UserCode;
                    processInstance.UserNm = pi.UserNm;
                }
                else
                {
                    var status = ((HiddenField)row.FindControl("hfStatus")).Value;
                    processInstance.Status = status;
                    processInstance.UserCode = userCode;
                    processInstance.UserNm = userName;
                }

                processInstance.Id = idr;
                processInstance.Level = level;
                processInstanceList.Add(processInstance);
            }

            if (isUpdate)
            {
                var task = this.TheTaskMstrMgr.CheckAndLoadTaskMstr(TaskCode);
                this.TheWorkflowMgr.SetApproval(task, processInstanceList);
                this.TheWorkflowMgr.SetCurrentApprovalUser(task, processInstanceList);

                task.LastModifyDate = DateTime.Now;
                task.LastModifyUser = this.CurrentUser.Code;
                task.LastModifyUserNm = this.CurrentUser.Name;
                this.TheTaskMstrMgr.UpdateTaskMstr(task);
            }

            InitPageParameter(this.TaskCode, this.ModuleType);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Delete.Fail", code);
        }
    }
    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code = ((System.Web.UI.WebControls.LinkButton)sender).CommandArgument;
        try
        {
            this.TheProcessInstanceMgr.DeleteProcessInstance(int.Parse(code));
            InitPageParameter(this.TaskCode, this.ModuleType);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Delete.Fail", code);
        }
    }
    protected void GV_List_Process_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            WFDetail wfDetail = (WFDetail)e.Row.DataItem;

            e.Row.Cells[5].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");

            if (!string.IsNullOrEmpty(wfDetail.Status))
            {
                e.Row.Cells[0].Text = "${ISI.Status." + wfDetail.Status + "}";
            }
            if (wfDetail.PreLevel.HasValue && wfDetail.PreLevel.Value > 0)
            {
                var level = wfDetail.PreLevel / ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL * ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                e.Row.Cells[1].Text = "${" + level + "}" + (level != wfDetail.PreLevel ? "&nbsp;" + (wfDetail.PreLevel / ISIConstants.CODE_MASTER_WF_COUNTERSIGN_LEVEL_INTERVAL).ToString().Substring(1) : string.Empty);
            }
            if (wfDetail.Level.HasValue && wfDetail.Level.Value > 0)
            {
                var level = wfDetail.Level / ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL * ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                e.Row.Cells[2].Text = "${" + level + "}" + (level != wfDetail.Level ? "&nbsp;" + (wfDetail.Level / ISIConstants.CODE_MASTER_WF_COUNTERSIGN_LEVEL_INTERVAL).ToString().Substring(1) : string.Empty);
            }
        }
    }
}