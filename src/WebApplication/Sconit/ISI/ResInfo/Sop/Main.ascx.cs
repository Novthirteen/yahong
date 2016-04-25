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

public partial class ISI_ResInfo_Sop_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DateTime now = DateTime.Now;
            //this.tbStartDate.Text = now.AddDays(-7).ToString("yyyy-MM-dd");
            //this.tbEndDate.Text = now.ToString("yyyy-MM-dd");
            //btnSearch_Click(null, null);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(ResSop));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(ResSop))
        .SetProjection(Projections.ProjectionList()
        .Add(Projections.Count("WorkShop")));
        string workshop = this.tbWorkShop.Text.Trim();
        if (workshop != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("WorkShop", workshop));
            selectCountCriteria.Add(Expression.Eq("WorkShop", workshop));
        }
        string operate = this.tbOperate.Text.Trim();
        if (operate != string.Empty)
        {
            int op = 0;
            int.TryParse(operate, out op);
            selectCriteria.Add(Expression.Eq("Operate", op));
            selectCountCriteria.Add(Expression.Eq("Operate", op));
        }
        selectCriteria.AddOrder(Order.Asc("WorkShop"));
        selectCriteria.AddOrder(Order.Asc("Operate"));
        this.SetSearchCriteria(selectCriteria, selectCountCriteria);
        this.UpdateView();
        this.fs.Visible = true;
    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
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
        ResSop resSop = (ResSop)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (resSop != null)
            {
                e.Row.Cells[1].Text = this.TheResWokShopMgr.LoadResWokShop(resSop.WorkShop).CodeName;
                //e.Row.Cells[2].Text = resSop.Operate + "[" + resSop.OperateDesc + "]";
            }

            IList<AttachmentDetail> attachMentList = this.TheAttachmentDetailMgr.GetResSopAttachment(resSop.Id.ToString());
            GridView gvListAttachment = (GridView)e.Row.FindControl("GV_List_Attachment");
            gvListAttachment.DataSource = attachMentList;
            gvListAttachment.DataBind();
        }
    }

    protected void GV_List_Attachment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        AttachmentDetail attachmentDetail = (AttachmentDetail)e.Row.DataItem;
        LinkButton lbtnDownLoad = (LinkButton)e.Row.FindControl("lbtnDownLoad");
        if (lbtnDownLoad != null)
        {
            lbtnDownLoad.Text = attachmentDetail.FileName;
        }
    }
}