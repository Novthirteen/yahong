using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using System.IO;

public partial class ISI_Attachment_Attachment : com.Sconit.Web.MainModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler UpdateAttacmentTitleEvent;

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
    public string AttachmentType
    {
        get
        {
            return (string)ViewState["AttachmentType"];
        }
        set
        {
            ViewState["AttachmentType"] = value;
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
    public TaskMstr task
    {
        get
        {
            return ViewState["Task"] == null ? null : (TaskMstr)ViewState["Task"];
        }
        set
        {
            ViewState["Task"] = value;
        }
    }

    public void InitPageParameter(int taskCode, string attachmentType)
    {
        InitPageParameter(taskCode.ToString(), attachmentType);
    }

    public void InitPageParameter(string taskCode, string attachmentType)
    {
        this.TaskCode = taskCode;
        this.AttachmentType = attachmentType;
        this.btnSearch_Click(null, null);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_VALUE_ADDATTACHMENT)
                    || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                    || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
            {
                this.legUpload.Visible = true;
            }
            else
            {
                this.legUpload.Visible = false;
            }
        }

    }

    protected void UploadButton_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheAttachmentDetailMgr.UploadFile(this.TaskCode, this.AttachmentType, HttpContext.Current.Request, this.CurrentUser);
            UpdateAttacmentTitleEvent(this.TaskCode, e);

            this.ShowSuccessMessage("ISI.TSK.UploadAttachment.Successfully");

            this.btnSearch_Click(null, null);
        }
        catch (System.Exception Ex)
        {
            //strStatus.Text = Ex.Message;
            this.ShowErrorMessage(Ex.Message);
        }
    }


    protected void lbtnDownLoad_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        string fileName = string.Empty;
        try
        {
            TheAttachmentDetailMgr.DownLoadFile(int.Parse(id), System.Web.HttpContext.Current.Request, Response, Server);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {

        }
    }


    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        string fileName = string.Empty;
        try
        {
            fileName = this.TheAttachmentDetailMgr.DeleteFile(int.Parse(id), System.Web.HttpContext.Current.Request);

            ShowSuccessMessage("ISI.TSK.DeleteAttachment.Successfully", fileName);
            this.btnSearch_Click(null, null);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            //TheTaskMgr.DeleteTaskMstr(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.DeleteAttachment.Fail", fileName);
        }
    }


    private void PageCleanup()
    {
        this.TaskCode = string.Empty;
        this.AttachmentType = string.Empty;
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
        this.GV_List_Attachment.DataSource = this.TheAttachmentDetailMgr.GetAttachment(this.TaskCode, this.AttachmentType);
        this.GV_List_Attachment.DataBind();

        if (task != null && this.AttachmentType == typeof(TaskMstr).FullName)
        {
            List<AttachmentDetail> attachmentDetailList = new List<AttachmentDetail>();
            IList<AttachmentDetail> attachmentList = null;
            if (!string.IsNullOrEmpty(task.ExtNo))
            {
                attachmentList = this.TheAttachmentDetailMgr.GetAttachment(task.ExtNo, "com.Sconit.Facility.Entity.MaintainPlan");
                if (attachmentList != null && attachmentList.Count > 0)
                {
                    attachmentDetailList.AddRange(attachmentList);
                }
            }
            if (task.ProjectTask != 0)
            {
                attachmentList = this.TheAttachmentDetailMgr.GetProjectAttachment(task.ProjectTask.ToString());
                if (attachmentList != null && attachmentList.Count > 0)
                {
                    attachmentDetailList.AddRange(attachmentList);
                }
            }

            attachmentList = this.TheAttachmentDetailMgr.GetTaskSubTypeAttachment(task.TaskSubType.Code);
            if (attachmentList != null && attachmentList.Count > 0)
            {
                attachmentDetailList.AddRange(attachmentList);
            }

            this.GV_ProjectTask.DataSource = attachmentDetailList;
            this.GV_ProjectTask.DataBind();

            this.isProject.Visible = attachmentDetailList != null && attachmentDetailList.Count > 0;
        }

        if ((Button)sender == this.btnExport)
        {
            this.ExportXLS(this.GV_List_Attachment);
        }
        else
        {
            UpdateAttacmentTitleEvent(this.TaskCode, e);
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.IsExport = true;
        this.btnSearch_Click(sender, e);
    }

    protected void GV_List_OnDataBinding(object sender, EventArgs e)
    {
        if (this.AttachmentType == typeof(TaskMstr).FullName)
        {
            task = this.TheTaskMstrMgr.LoadTaskMstr(this.TaskCode);
        }
    }
    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        AttachmentDetail attachment = (AttachmentDetail)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lbtnDownLoad = (LinkButton)e.Row.FindControl("lbtnDownLoad");
            if (lbtnDownLoad != null)
            {
                lbtnDownLoad.Text = attachment.FileName;
            }

            LinkButton lbtnDelete = (LinkButton)e.Row.FindControl("lbtnDelete");
            if (lbtnDelete != null)
            {
                if ((//attachment.CreateUser == this.CurrentUser.Code || 
                        this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_VALUE_DELETEATTACHMENT)
                            || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                            || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
                            || (task != null && (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE || (task.TaskSubType != null && task.TaskSubType.IsAttachment && task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)) && attachment.CreateUser == this.CurrentUser.Code))
                {
                    lbtnDelete.Visible = true;
                }
                else
                {
                    lbtnDelete.Visible = false;
                }
            }

        }
    }
}