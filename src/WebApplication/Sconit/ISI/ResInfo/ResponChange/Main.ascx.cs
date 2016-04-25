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

public partial class ISI_ResInfo_ResponChange_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string tbStartDate = this.tbStartDate.Text.Trim();
        string tbEndDate = this.tbEndDate.Text.Trim();
        string tbUserCode = this.tbUserCode.Text.Trim();

        string sql = string.Empty;
        SqlParameter[] sqlParam = new SqlParameter[3];
        sqlParam[0] = new SqlParameter("@StartDate", tbStartDate);
        sqlParam[1] = new SqlParameter("@EndDate", tbEndDate);
        sqlParam[2] = new SqlParameter("@UserCode", tbUserCode);

        DataSet dataSetInv = TheSqlHelperMgr.GetDatasetByStoredProcedure("usp_Query_Res_GetResponChange1", sqlParam);
        List<Res> responsibilityList = IListHelper.DataTableToList<Res>(dataSetInv.Tables[0]);

        this.GV_List.DataSource = responsibilityList;
        this.GV_List.DataBind();
        this.fs.Visible = true;
        if ((Button)sender == this.btnExport)
        {
            this.Export(this.GV_List, "application/ms-excel", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".xls");
        }
    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Res responsibility = (Res)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Text = responsibility.UserCode + "[" + responsibility.UserName + "]";
            if (!string.IsNullOrEmpty(responsibility.WorkShopName))
            {
                e.Row.Cells[2].Text = responsibility.WorkShop + "[" + responsibility.WorkShopName + "]";
            }
            if (responsibility.Operate > 0 && responsibility.OperateDesc != string.Empty)
            {
                e.Row.Cells[3].Text = responsibility.Operate + "[" + responsibility.OperateDesc + "]";
            }
            else
            {
                e.Row.Cells[3].Text = string.Empty;
            }
            if (responsibility.Mark == "变更")
            {
                e.Row.Cells[4].Text = DiffMatchPatchHelper.DiffPrettyHtml(responsibility.Responsibility, responsibility.OldResponsibility);
            }
            e.Row.Cells[4].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
        }
        else
        {
            //e.Row.Cells[4].Style.Add("min-width", "45px");
            //e.Row.Cells[5].Style.Add("min-width", "30px");
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
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string WorkShop { get; set; }
        public string WorkShopName { get; set; }
        public int? Operate { get; set; }
        public string OperateDesc { get; set; }
        public string Mark { get; set; }
        public DateTime EndDate { get; set; }
        public string Responsibility { get; set; }
        public string OldResponsibility { get; set; }
    }
}