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


public partial class ISI_TaskStatus_List : ListModuleBase
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

    public EventHandler EditEvent;


    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.TaskCode))
        {
            this.GV_List.Columns[0].Visible = true;
            this.GV_List.Columns[this.GV_List.Columns.Count - 1].Visible = false;
        }
        else
        {
            this.GV_List.Columns[0].Visible = false;
            bool isISIAdmin = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN);
            //bool isTaskFlowAdmin = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASKADMIN_VALUE_TASKFLOWADMIN);
            if (isISIAdmin)
            {
                this.GV_List.Columns[this.GV_List.Columns.Count - 1].Visible = true;
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.TaskCode))
        {
            this.lgd.Visible = false;

        }
        else
        {
            this.lgd.Visible = true;
        }

        if (!IsPostBack)
        {

        }
    }
    public override void UpdateView()
    {
        if (!string.IsNullOrEmpty(this.TaskCode))
        {
            this.lgd.InnerText = "${ISI.TSK." + this.ModuleType + "}" + this.TaskCode;
        }
        this.GV_List.Execute();
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            EditEvent(code, e);
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        try
        {
            TheTaskStatusMgr.DeleteTaskStatus(int.Parse(id));
            ShowSuccessMessage("ISI.TaskStatus.DeleteTaskStatus.Successfully");
            UpdateView();

        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.TaskStatus.DeleteTaskStatus.Fail");
        }

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TaskStatus taskStatus = (TaskStatus)e.Row.DataItem;
            e.Row.Cells[1].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            if (!this.IsExport && !string.IsNullOrEmpty(taskStatus.Color))
            {
                e.Row.Cells[6].Attributes["style"] = "background-color:" + taskStatus.Color;
            }
            if (!string.IsNullOrEmpty(taskStatus.Desc))
            {
                ((Label)e.Row.FindControl("lblDesc")).Text = ISIUtil.GetHtmlBody(taskStatus.Desc);
            }
            if (!(CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_VALUE_DELETETASKSTATUS) ||
                        CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)))
            {
                e.Row.FindControl("lbtnDelete").Visible = false;
            }
        }
    }
}
