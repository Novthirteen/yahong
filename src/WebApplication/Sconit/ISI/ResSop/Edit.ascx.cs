using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;
using System.Web;
using System.IO;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.by liqiuyun
public partial class Modules_ISI_ResSop_Edit : EditModuleBase
{
    public string Id
    {
        get
        {
            if (ViewState["WorkShopCode"] == null)
            {
                return null;
            }
            else
            {
                return (string)ViewState["WorkShopCode"];
            }
        }
        set
        {
            ViewState["WorkShopCode"] = value;
        }
    }

    public event EventHandler Back;

    //Get the logger
    private static ILog log = LogManager.GetLogger("ISI");

    protected void Page_Load(object sender, EventArgs e)
    {
        //TODO: Add code for Page_Load here.
        if (!IsPostBack)
        {

        }
    }

    public void InitPageParameter(object Code)
    {
        this.ODS_ResSop.SelectParameters["Id"].DefaultValue = Code.ToString();
        this.ODS_ResSop.DeleteParameters["Id"].DefaultValue = Code.ToString();
        this.FV_ResSop.DataBind();
        this.Id = Code.ToString();
        this.btnSearch_Click(null, null);
    }

    protected void FV_ResSop_DataBound(object sender, EventArgs e)
    {
        //ResSop dataItem = (ResSop)this.FV_ResSop.DataItem;
        //if (dataItem != null)
        //{
        //  Controls_TextBox tbPartyFrom = (Controls_TextBox)this.FV_Flow.FindControl("tbPartyFrom");
        //  tbPartyTo.Text = dataItem.PartyTo;
        //  tbPartyTo.ServiceParameter = "string:" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION + ",string:" + this.CurrentUser.Code;
        //	tbPartyTo.DataBind();
        //}
    }

    protected void ODS_ResSop_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        ResSop dataItem = (ResSop)e.InputParameters[0];
        dataItem.LastModifyDate = DateTime.Now;
        dataItem.LastModifyUser = this.CurrentUser.Code;
        //Controls_TextBox tbPartyFrom = (Controls_TextBox)this.FV_ResSop.FindControl("tbPartyFrom");
    }

    protected void ODS_ResSop_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            ShowErrorMessage("Common.Business.Result.Update.Failed");
            e.ExceptionHandled = true;
        }
        else
        {
            Back(sender, e);
            ShowSuccessMessage("Common.Business.Result.Update.Successfully");
        }
    }

    protected void ODS_ResSop_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            ShowErrorMessage("Common.Business.Result.Delete.Failed");
            e.ExceptionHandled = true;
        }
        else
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Common.Business.Result.Delete.Successfully");
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Back != null)
        {
            this.Visible = false;
            Back(sender, e);
        }
    }

    //attachment

    protected void UploadButton_Click(object sender, EventArgs e)
    {
        ///'����File��Ԫ��
        HttpFileCollection files = HttpContext.Current.Request.Files;

        /// '״̬��Ϣ
        System.Text.StringBuilder strMsg = new System.Text.StringBuilder();
        strMsg.Append("�ϴ����ļ��ֱ��ǣ�<hr color='red'/>");
        try
        {
            for (int iFile = 0; iFile < files.Count; iFile++)
            {
                ///'����ļ���չ����
                HttpPostedFile postedFile = files[iFile];
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                int contentLength = 0;
                fileName = System.IO.Path.GetFileName(postedFile.FileName);
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileExtension = System.IO.Path.GetExtension(fileName);
                    //contentLength = postedFile.ContentLength;

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

                    strMsg.Append("�ϴ����ļ����ͣ�" + postedFile.ContentType.ToString() + "<br>");
                    strMsg.Append("�ͻ����ļ���ַ��" + postedFile.FileName + "<br>");
                    strMsg.Append("�ϴ��ļ����ļ�����" + fileName + "<br>");
                    strMsg.Append("�ϴ��ļ�����չ����" + fileExtension + "<br><hr>");
                    ///'�ɸ�����չ���ֵĲ�ͬ���浽��ͬ���ļ���
                    ///ע�⣺����Ҫ�޸�����ļ��е�����д��Ȩ�ޡ�

                    DateTime now = DateTime.Now;
                    AttachmentDetail attachment = new AttachmentDetail();
                    attachment.TaskCode = this.Id;
                    attachment.Size = decimal.Parse((postedFile.ContentLength / 1024.0).ToString());
                    attachment.CreateUser = this.CurrentUser.Code;
                    attachment.CreateUserNm = this.CurrentUser.Name;
                    attachment.CreateDate = now;
                    attachment.FileName = fileName;
                    attachment.FileExtension = fileExtension;
                    attachment.ModuleType = typeof(ResSop).FullName;
                    attachment.ContentType = postedFile.ContentType;
                    string alias = this.Id + "_" + now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid() + fileExtension;
                    string absolutePath = System.Web.HttpContext.Current.Request.MapPath("App_Data/");
                    string path = "Templates/" + this.Id + "/";

                    if (!Directory.Exists(absolutePath + path))//�ж��Ƿ����
                    {
                        Directory.CreateDirectory(absolutePath + path);//������·��
                    }

                    attachment.Path = path + alias;

                    postedFile.SaveAs(absolutePath + attachment.Path);

                    this.TheAttachmentDetailMgr.CreateAttachmentDetail(attachment);

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

    //  �����ļ���
    public void DownLoadFile(AttachmentDetail attachment)
    {
        string absolutePath = System.Web.HttpContext.Current.Request.MapPath("App_Data/");
        // �����ļ�������·��
        //string Url = "File\\" + FullFileName;
        // �����ļ�������·��
        string FullPath = absolutePath + attachment.Path;// HttpContext.Current.Server.MapPath(Url);
        // ��ʼ��FileInfo���ʵ������Ϊ�ļ�·���İ�װ
        FileInfo FI = new FileInfo(FullPath);
        // �ж��ļ��Ƿ����
        if (FI.Exists)
        {
            // ���ļ����浽����
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Server.UrlEncode(attachment.FileName));
            Response.AddHeader("Content-Length", FI.Length.ToString());
            Response.ContentType = attachment.ContentType;
            Response.Filter.Close();
            Response.WriteFile(FI.FullName);
            Response.Flush();
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

            //DeleteFile(attachment.Path);

            this.TheAttachmentDetailMgr.DeleteAttachmentDetail(attachment);
            ShowSuccessMessage("ISI.TSK.DeleteAttachment.Successfully", fileName);
            this.btnSearch_Click(null, null);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            //TheTaskMgr.DeleteTaskMstr(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.DeleteAttachment.Fail", fileName);
        }
    }


    // ɾ���ļ���
    //private void DeleteFile(string FullFileName)
    //{
    //    string absolutePath = System.Web.HttpContext.Current.Request.MapPath("App_Data/");

    //    // �����ļ�������·��
    //    //string Url = "File\\" + FullFileName;
    //    // �����ļ�������·��
    //    string FullPath = absolutePath + FullFileName;// HttpContext.Current.Server.MapPath(Url);
    //    // ȥ���ļ���ֻ������
    //    File.SetAttributes(FullPath, FileAttributes.Normal);
    //    // ��ʼ��FileInfo���ʵ������Ϊ�ļ�·���İ�װ
    //    FileInfo FI = new FileInfo(FullPath);
    //    // �ж��ļ��Ƿ����
    //    if (FI.Exists)
    //    {
    //        FI.Delete();
    //    }
    //}

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.GV_List_Attachment.DataSource = this.TheAttachmentDetailMgr.GetResSopAttachment(this.Id);
        this.GV_List_Attachment.DataBind();
        //if ((Button)sender == this.btnExport)
        //{
        //    this.ExportXLS(this.GV_List_Attachment);
        //}
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
                //lbtnDownLoad.Text = attachment.FileName;
            }
        }
    }

}