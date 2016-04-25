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
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using System.IO;

public partial class ISI_ResInfo_Attachment_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DateTime now = DateTime.Now;
            this.tbStartDate.Text = now.AddDays(-7).ToString("yyyy-MM-dd");
            this.tbEndDate.Text = now.ToString("yyyy-MM-dd");
            //btnSearch_Click(null, null);
        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string fileName = this.tbFileName.Text.Trim();
        string startDate = this.tbStartDate.Text.Trim();
        string endDate = this.tbEndDate.Text.Trim();
        string createUser = this.tbCreateUser.Text.Trim();

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(AttachmentDetail));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(AttachmentDetail)).SetProjection(Projections.Count("Id"));
        //过滤ModelType com.Sconit.ISI.Entity.ResSop
        selectCriteria.Add(Expression.Eq("ModuleType", typeof(ResSop).FullName));
        selectCountCriteria.Add(Expression.Eq("ModuleType", typeof(ResSop).FullName));

        if (!string.IsNullOrEmpty(createUser))
        {
            selectCriteria.Add(Expression.Eq("CreateUser", createUser));
            selectCountCriteria.Add(Expression.Eq("CreateUser", createUser));
        }
        if (!string.IsNullOrEmpty(fileName))
        {
            selectCriteria.Add(Expression.Like("FileName", fileName, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FileName", fileName, MatchMode.Anywhere));
        }
        if (!string.IsNullOrEmpty(startDate))
        {
            selectCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(startDate)));
            selectCountCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(startDate)));
        }
        if (!string.IsNullOrEmpty(endDate))
        {
            selectCriteria.Add(Expression.Le("CreateDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
            selectCountCriteria.Add(Expression.Le("CreateDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
        }

        selectCriteria.AddOrder(Order.Desc("CreateDate"));
        new SessionHelper(this.Page).AddUserSelectCriteria(this.TemplateControl.AppRelativeVirtualPath, selectCriteria, selectCountCriteria);
        this.GV_List_Attachment.Execute();
        if ((Button)sender == this.btnExport)
        {
            GV_List_Attachment.ExportXLS("Attachment.xls");
        }
        else
        {
            this.fs.Visible = true;
        }
        this.IsExport = false;
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.IsExport = true;
        this.btnSearch_Click(sender, e);
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

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        AttachmentDetail attachment = (AttachmentDetail)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lbtnDownLoad = (LinkButton)e.Row.FindControl("lbtnDownLoad");
            if (lbtnDownLoad != null)
            {
                lbtnDownLoad.Text = attachment.FileName;
                ResSop resSop = TheResSopMgr.LoadResSop(int.Parse(attachment.TaskCode));
                e.Row.Cells[1].Text = this.TheResWokShopMgr.LoadResWokShop(resSop.WorkShop).CodeName;
                e.Row.Cells[2].Text = string.Format("{0}[{1}]", resSop.Id, resSop.OperateDesc);
            }
        }
    }
}