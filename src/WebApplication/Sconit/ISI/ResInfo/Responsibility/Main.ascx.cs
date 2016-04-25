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
using System.Data.SqlClient;
using System.Data;

public partial class ISI_ResInfo_Responsibility_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string resUser = this.tbDirector.Text.Trim();
        string tbHistoryDate = this.tbHistoryDate.Text.Trim();

        string sql = string.Empty;
        SqlParameter[] sqlParam = new SqlParameter[2];
        sqlParam[0] = new SqlParameter("@UserCode", resUser);
        sqlParam[1] = new SqlParameter("@HistoryDate", null);
        if (!string.IsNullOrEmpty(tbHistoryDate))
        {
            DateTime historyDate = DateTime.Parse(tbHistoryDate);
            sqlParam[1] = new SqlParameter("@HistoryDate", historyDate);
        }

        DataSet dataSetInv = TheSqlHelperMgr.GetDatasetByStoredProcedure("usp_Query_Res_GetResponsibility", sqlParam);
        List<Res> responsibilityList = IListHelper.DataTableToList<Res>(dataSetInv.Tables[0]);
        var responsibilitys = responsibilityList.GroupBy(p => new { p.WorkShop, p.Operate }, (k, g) =>
        {
            Res responsibility = g.First();
            responsibility.ResList = g.GroupBy(p => p.Responsibility).Select(p => p.First()).OrderBy(p => p.Role).ThenBy(p => p.Seq).ToList();
            return responsibility;
        });

        this.GV_List.DataSource = responsibilitys;
        this.GV_List.DataBind();
        this.fs.Visible = true;
        if ((Button)sender == this.btnExport)
        {
            this.Export(this.GV_List, "application/ms-excel", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".xls");
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

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string tbHistoryDate = this.tbHistoryDate.Text.Trim();
        Res responsibility = (Res)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!string.IsNullOrEmpty(responsibility.WorkShopName))
            {
                e.Row.Cells[1].Text = responsibility.WorkShop + "[" + responsibility.WorkShopName + "]";
            }
            if (responsibility.Operate != null)
            {
                e.Row.Cells[2].Text = responsibility.Operate + "[" + responsibility.OperateDesc + "]";
            }
            IList<AttachmentDetail> attachMentList = new List<AttachmentDetail>();
            if (string.IsNullOrEmpty(tbHistoryDate))
            {
                attachMentList = this.TheAttachmentDetailMgr.GetResSopAttachment(responsibility.SopId.ToString());
            }
            else
            {
                foreach (var id in responsibility.AttachmentIds.Split(',').Where(p => !string.IsNullOrEmpty(p)).Select(p => int.Parse(p)))
                {
                    var attachMent = this.TheAttachmentDetailMgr.LoadAttachmentDetail(id);
                    attachMentList.Add(attachMent);
                }
            }
            GridView gvListAttachment = (GridView)e.Row.FindControl("GV_List_Attachment");
            gvListAttachment.DataSource = attachMentList;
            gvListAttachment.DataBind();

            GridView gvListDetail = (GridView)e.Row.FindControl("GV_List_Detail");
            gvListDetail.DataSource = responsibility.ResList;
            gvListDetail.DataBind();
        }
    }

    protected void GV_List_Detail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Res responsibility = (Res)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!string.IsNullOrEmpty(responsibility.RoleName))
            {
                e.Row.Cells[0].Text = responsibility.Role + "[" + responsibility.RoleName + "]";
            }
            if (!string.IsNullOrEmpty(responsibility.SkillLevel))
            {
                e.Row.Cells[2].Text = TheCodeMasterMgr.GetCachedCodeMaster("ISISkillLevel", responsibility.SkillLevel).Description;
            }
            e.Row.Cells[3].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
        }
        else
        {
            e.Row.Cells[0].Style.Add("min-width", "45px");
            e.Row.Cells[1].Style.Add("min-width", "30px");
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

    class Res
    {
        public Int32 ResMatrixId { get; set; }
        public string WorkShop { get; set; }
        public string WorkShopName { get; set; }
        public int? Operate { get; set; }
        public string OperateDesc { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string Priority { get; set; }
        public string SkillLevel { get; set; }
        public int SopId { get; set; }
        public string AttachmentIds { get; set; }
        public string Responsibility { get; set; }
        public int Seq { get; set; }
        public List<Res> ResList { get; set; }
    }

}