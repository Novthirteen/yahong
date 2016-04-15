using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using NHibernate.Transform;
using System.Data.SqlClient;

public partial class Reports_RMScrap_Main : MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code;
        if (!IsPostBack)
        {
            this.tbStartDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm");
            this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string itemCode = this.tbItem.Text.Trim();
        string flowCode = this.tbFlow.Text.Trim();
        DateTime? startTime = null;
        if (this.tbStartDate.Text.Trim() != string.Empty)
        {
            startTime = DateTime.Parse(this.tbStartDate.Text.Trim());
        }
        DateTime? endTime = null;
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            endTime = DateTime.Parse(this.tbEndDate.Text.Trim());
        }

        try
        {
            string sql = @"select m.orderno as 报废单,m.reforderno as 不合格品处理单,d.item as 物料号,i.desc1 as 描述1,i.desc2 as 描述2,
                            i.uom as 单位,cast(d.qty as numeric(12,2)) as 数量,m.location as 库位,m.projectcode as '生产线',m.createuser as 用户,
                            m.createdate as 时间 from MiscOrderdet d
                            left join MiscOrdermstr m on d.Orderno = m.orderno
                            left join item i on d.item = i.code 
                            where reason ='StockOutReason1' and m.type='Gi'
                            and (m.ProjectCode is null 
                            ";

            SqlParameter[] sqlParam = new SqlParameter[4];

            sqlParam[0] = new SqlParameter("@p1", flowCode);
            sqlParam[1] = new SqlParameter("@p2", startTime);
            sqlParam[2] = new SqlParameter("@p3", endTime);
            sqlParam[3] = new SqlParameter("@p4", itemCode);

            if (!string.IsNullOrEmpty(flowCode))
            {
                sql += " or m.projectcode = @p1)";
            }
            else
            {
                sql += " )";
            }
            if (!string.IsNullOrEmpty(itemCode))
            {
                sql += " and d.item = @p4 ";
            }
            if (startTime.HasValue)
            {
                sql += " and m.createdate > @p2 ";
            }
            if (endTime.HasValue)
            {
                sql += " and m.createdate< @p3 ";
            }

            //投入的成品
            this.GV_List.DataSource = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);
            this.GV_List.DataBind();
            this.fld_Gv_List.Visible = true;
            if ((Button)sender == this.btnExport)
            {
                this.ExportXLS(this.GV_List);
            }

        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.btnSearch_Click(sender, e);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[3].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
    }
}
