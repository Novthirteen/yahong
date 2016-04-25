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
using com.Sconit.Facility.Entity;

public partial class Facility_FacilityMaster_TransAttachment : com.Sconit.Web.MainModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler UpdateTemplateTitleEvent;

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

    public void InitPageParameter(string code)
    {
        this.TaskCode = code;
        this.btnSearch_Click(null, null);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //if (this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_VALUE_ADDATTACHMENT)
            //        || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
            //        || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
            //{
            //    this.legUpload.Visible = true;
            //}
            //else
            //{
            //    this.legUpload.Visible = false;
            //}
        }

    }


    //  下载文件类
    public void DownLoadFile(AttachmentDetail attachment)
    {
        string absolutePath = System.Web.HttpContext.Current.Request.MapPath("App_Data/");
        // 保存文件的虚拟路径
        //string Url = "File\\" + FullFileName;
        // 保存文件的物理路径
        string FullPath = absolutePath + attachment.Path;// HttpContext.Current.Server.MapPath(Url);
        // 初始化FileInfo类的实例，作为文件路径的包装
        FileInfo FI = new FileInfo(FullPath);
        // 判断文件是否存在
        if (FI.Exists)
        {
            // 将文件保存到本机
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Server.UrlEncode(attachment.FileName));
            Response.AddHeader("Content-Length", FI.Length.ToString());
            Response.ContentType = attachment.ContentType;
            Response.Filter.Close();
            Response.WriteFile(FI.FullName);
            Response.End();
        }
    }


    private void PageCleanup()
    {
        this.TaskCode = null;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        this.Visible = false;
    }

    protected void lbtnDownLoad_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        string fileName = string.Empty;
        try
        {
            AttachmentDetail attachment = this.TheAttachmentDetailMgr.LoadAttachmentDetail(int.Parse(id));

            this.DownLoadFile(attachment);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {

        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.GV_List_Attachment.DataSource = this.TheFacilityMasterMgr.GetFacilityTransAttachment(this.TaskCode);
        this.GV_List_Attachment.DataBind();

    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.IsExport = true;
        this.btnSearch_Click(sender, e);
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
                if (//attachment.CreateUser == this.CurrentUser.Code || 
                        this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_VALUE_DELETEATTACHMENT)
                            || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                            || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
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