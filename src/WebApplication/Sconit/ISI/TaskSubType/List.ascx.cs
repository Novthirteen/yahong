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
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using System.Text;


public partial class ISI_TaskSubType_List : ListModuleBase
{
    public string Action
    {
        get
        {
            return (string)ViewState["Action"];
        }
        set
        {
            ViewState["Action"] = value;
        }
    }
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
    public EventHandler EditEvent;
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[0];
            string desc = ((LinkButton)sender).CommandArgument.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[1];
            EditEvent(new string[] { code, desc }, e);
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code = ((LinkButton)sender).CommandArgument;
        try
        {
            if (TheTaskSubTypeMgr.IsRef(code))
            {
                ShowErrorMessage("ISI.TaskSubType.DeleteTaskSubType.Fail", code);
            }
            else
            {
                TheTaskSubTypeMgr.DeleteTaskSubType(code);
                ShowSuccessMessage("ISI.TaskSubType.DeleteTaskSubType.Successfully", code);
                UpdateView();
            }
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.TaskSubType.DeleteTaskSubType.Fail", code);
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TaskSubType taskSubType = (TaskSubType)e.Row.DataItem;
            /*
            Label lblAssignUser = (Label)e.Row.FindControl("lblAssignUser");
            lblAssignUser.Text = ISIUtil.ShowUser(taskSubType.AssignUser);
            Label lblStartUser = (Label)e.Row.FindControl("lblStartUser");
            lblStartUser.Text = ISIUtil.ShowUser(taskSubType.StartUser);
            Label lblAssignUpUser = (Label)e.Row.FindControl("lblAssignUpUser");
            lblAssignUpUser.Text = ISIUtil.ShowUser(taskSubType.AssignUpUser);
            Label lblStartUpUser = (Label)e.Row.FindControl("lblStartUpUser");
            lblStartUpUser.Text = ISIUtil.ShowUser(taskSubType.StartUpUser);
            Label lblCloseUpUser = (Label)e.Row.FindControl("lblCloseUpUser");
            lblCloseUpUser.Text = ISIUtil.ShowUser(taskSubType.CloseUpUser);
            
            
            e.Row.Cells[1].Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
            e.Row.Cells[1].Attributes.Add("onmouseout", "this.style.backgroundColor=e");
            e.Row.Cells[1].Attributes.Add("title", GetProcessDefinition(taskSubType));
             * */
            if (taskSubType.IsWF)
            {
                StringBuilder detail = new StringBuilder();
                IList<ProcessDefinition> pds = this.TheProcessDefinitionMgr.GetProcessDefinition(taskSubType.Code);
                if (taskSubType.IsAssignUser)
                {
                    detail.Append("${ISI.TaskSubType.IsAssignUser}" + (taskSubType.IsRemind ? "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&#40;${ISI.TaskSubType.ProcessDefinition.IsRemind}&#41;" : string.Empty) + "<br>");
                }
                if (pds != null && pds.Count > 0)
                {
                    foreach (ProcessDefinition pd in pds)
                    {
                        detail.Append(pd.Desc1 + (!string.IsNullOrEmpty(pd.UserNm) ? "：&nbsp;" + pd.UserNm : string.Empty) + (pd.IsRemind ? "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&#40;${ISI.TaskSubType.ProcessDefinition.IsRemind}&#41;" : string.Empty) + "<br>");
                    }
                }
                if (taskSubType.IsAutoAssign || !string.IsNullOrEmpty(taskSubType.StartUser))
                {
                    detail.Append("${ISI.TaskSubType.StartUser}：");
                    if (taskSubType.IsAutoAssign)
                    {
                        detail.Append("${ISI.TaskApply.SubmitUser}");
                    }
                    if (!string.IsNullOrEmpty(taskSubType.StartUser))
                    {
                        if (taskSubType.IsAutoAssign)
                        {
                            detail.Append(", ");
                        }
                        detail.Append(this.TheUserSubscriptionMgr.GetUserName(taskSubType.StartUser));
                    }
                }
                e.Row.Cells[e.Row.Cells.Count - 2].Text = detail.ToString();
            }
        }
    }

    private string GetProcessDefinition(TaskSubType taskSubType)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[" + taskSubType.Description + "] body=[<table width=100%>");
        IList<ProcessDefinition> pds = this.TheProcessDefinitionMgr.GetProcessDefinition(taskSubType.Code);
        if (taskSubType.IsAssignUser)
        {
            detail.Append("<tr><td></td><td>${ISI.TaskSubType.IsAssignUser}</td><td></td></tr>");
        }
        foreach (ProcessDefinition pd in pds)
        {
            detail.Append("<tr><td>" + pd.Desc1 + "</td><td>" + pd.UserNm + "</td><td>" + (pd.IsRemind ? "${ISI.TaskSubType.ProcessDefinition.IsRemind}" : string.Empty) + "</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }

    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        /*
        bool b = this.Action != "View" && this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN);
        for (int i = 4; i <= 10; i++)
        {
            GV_List.Columns[i].Visible = b;
        }
         */
        GV_List.Columns[GV_List.Columns.Count - 1].Visible = ModuleType != ISIConstants.ISI_TASK_TYPE_WORKFLOW && this.Action != "View" || ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW && this.Action != "View" && this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN);
    }
}
