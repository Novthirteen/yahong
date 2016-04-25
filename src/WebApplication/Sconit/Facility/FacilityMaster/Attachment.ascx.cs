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

public partial class Facility_FacilityMaster_Attachment : com.Sconit.Web.MainModuleBase
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
        this.lgd.InnerText =this.TaskCode;
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
        ///'遍历File表单元素
        HttpFileCollection files = HttpContext.Current.Request.Files;

        /// '状态信息
        System.Text.StringBuilder strMsg = new System.Text.StringBuilder();
        strMsg.Append("上传的文件分别是：<hr color='red'/>");
        try
        {
            for (int iFile = 0; iFile < files.Count; iFile++)
            {
                ///'检查文件扩展名字
                HttpPostedFile postedFile = files[iFile];
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                int contentLength = 0;
                fileName = System.IO.Path.GetFileName(postedFile.FileName);
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileExtension = System.IO.Path.GetExtension(fileName);

                    contentLength = int.Parse(this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ISI_CONTENTLENGTH).Value);
                    if (postedFile.ContentLength > (contentLength * 1024 * 1024))
                    {
                        this.ShowWarningMessage("ISI.Attachment.FilesCantBeMoreThan", new string[] { contentLength.ToString() });
                        return;
                    }

                    string[] fileExtensions = this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ISI_FILEEXTENSION).Value.ToLower().Split(ISIConstants.ISI_FILE_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (!fileExtensions.Contains(fileExtension.ToLower().Substring(1)))
                    {
                        this.ShowWarningMessage("ISI.Attachment.NotAllowedToUpload", new string[] { fileExtension.Substring(1) });
                        return;
                    }

                    strMsg.Append("上传的文件类型：" + postedFile.ContentType.ToString() + "<br>");
                    strMsg.Append("客户端文件地址：" + postedFile.FileName + "<br>");
                    strMsg.Append("上传文件的文件名：" + fileName + "<br>");
                    strMsg.Append("上传文件的扩展名：" + fileExtension + "<br><hr>");
                    ///'可根据扩展名字的不同保存到不同的文件夹
                    ///注意：可能要修改你的文件夹的匿名写入权限。

                    DateTime now = DateTime.Now;
                    AttachmentDetail attachment = new AttachmentDetail();
                    attachment.TaskCode = this.TaskCode;
                    attachment.Size = decimal.Parse((postedFile.ContentLength / 1024.0).ToString());
                    attachment.CreateUser = this.CurrentUser.Code;
                    attachment.CreateUserNm = this.CurrentUser.Name;
                    attachment.CreateDate = now;
                    attachment.FileName = fileName;
                    attachment.FileExtension = fileExtension;
                    attachment.ContentType = postedFile.ContentType;
                    attachment.ModuleType = typeof(FacilityMaster).FullName;
                    string alias = this.TaskCode + now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid() + fileExtension;
                    
                    string absolutePath = System.Web.HttpContext.Current.Request.MapPath("App_Data/");
                    string path = this.ModuleType + "/" + this.TaskCode + "/";
                    if (!Directory.Exists(absolutePath + path))//判断是否存在
                    {
                        Directory.CreateDirectory(absolutePath + path);//创建新路径
                    }
                    attachment.Path = path + alias;

                    postedFile.SaveAs(absolutePath + attachment.Path);

                    this.TheAttachmentDetailMgr.CreateAttachmentDetail(attachment);

                    UpdateAttacmentTitleEvent(sender, e);

                    this.ShowSuccessMessage("ISI.TSK.UploadAttachment.Successfully", fileName);
                }
            }
            //strStatus.Text = strMsg.ToString();

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
            AttachmentDetail attachment = this.TheAttachmentDetailMgr.LoadAttachmentDetail(int.Parse(id));

            this.DownLoadFile(attachment);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {

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

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        string fileName = string.Empty;
        try
        {
            AttachmentDetail attachment = this.TheAttachmentDetailMgr.LoadAttachmentDetail(int.Parse(id));

            fileName = attachment.FileName;

            DeleteFile(attachment.Path);

            this.TheAttachmentDetailMgr.DeleteAttachmentDetail(attachment);
            UpdateAttacmentTitleEvent(sender, e);
            ShowSuccessMessage("ISI.TSK.DeleteAttachment.Successfully", fileName);
            this.btnSearch_Click(null, null);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            //TheTaskMgr.DeleteTaskMstr(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.DeleteAttachment.Fail", fileName);
        }
    }


    // 删除文件类
    public void DeleteFile(string FullFileName)
    {
        string absolutePath = System.Web.HttpContext.Current.Request.MapPath("App_Data/");

        // 保存文件的虚拟路径
        //string Url = "File\\" + FullFileName;
        // 保存文件的物理路径
        string FullPath = absolutePath + FullFileName;// HttpContext.Current.Server.MapPath(Url);
        // 去除文件的只读属性
        File.SetAttributes(FullPath, FileAttributes.Normal);
        // 初始化FileInfo类的实例，作为文件路径的包装
        FileInfo FI = new FileInfo(FullPath);
        // 判断文件是否存在
        if (FI.Exists)
        {
            FI.Delete();
        }
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
        this.GV_List_Attachment.DataSource = this.TheFacilityMasterMgr.GetFacilityMasterAttachment(this.TaskCode);
        this.GV_List_Attachment.DataBind();


        if ((Button)sender == this.btnExport)
        {
            this.ExportXLS(this.GV_List_Attachment);
        }
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

            //LinkButton lbtnDelete = (LinkButton)e.Row.FindControl("lbtnDelete");
            //if (lbtnDelete != null)
            //{
            //    if (//attachment.CreateUser == this.CurrentUser.Code || 
            //            this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_VALUE_DELETEATTACHMENT)
            //                || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
            //                || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
            //    {
            //        lbtnDelete.Visible = true;
            //    }
            //    else
            //    {
            //        lbtnDelete.Visible = false;
            //    }
            //}

        }
    }
}