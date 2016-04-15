using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;
using NHibernate.Expression;
using com.Sconit.Entity.View;
using com.Sconit.Entity;
using System.Data.SqlClient;
using System.Data;
public partial class Cost_Report_InvIOB_Main : MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            fld_Gv_List.Visible = false;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string sql = @"select item.code as 物料代码,item.desc1 as 描述1, item.desc2 as 描述2,item.uom as 单位,
                            item.category as 产品类, cast(cost as numeric(12,2))  as 成本,cast(Qty as numeric(12,2)) as 期初数量,
                            cast(QtyIn as numeric(12,2)) as 入库数量,
                            0 as 出库数量,cast(QtyEnd as numeric(12,2)) as 期末数量
                            from costinviob
                            left join item on item.code = costinviob.item
                            where costinviob.financeyear =@p0 and financemonth=@p1 ";

            if ((Button)sender == this.btnExport)
            {
                sql = @"select item.code as 物料代码,item.desc1 as 描述1, item.desc2 as 描述2,item.uom as 单位,
                            item.category as 产品类, cast(cost as numeric(12,2))  as 成本,cast(Qty as numeric(12,2)) as 期初数量,
                            cast(QtyIn as numeric(12,2)) as 入库数量,
                            0 as 出库数量,cast(QtyEnd as numeric(12,2)) as 期末数量
                            from costinviob
                            left join item on item.code = costinviob.item
                            where costinviob.financeyear =@p0 and financemonth=@p1 ";
            }
            if (this.tbFinanceYear.Text.Trim() == string.Empty)
            {
                ShowErrorMessage("Cost.FinanceCalendar.Year.Empty");
                return;
            }
            //if (this.tbItemCategory.Text.Trim() == string.Empty)
            //{
            //    ShowErrorMessage("MasterData.Item.Category.Empty");
            //    return;
            //}

            DateTime f = DateTime.Parse(this.tbFinanceYear.Text);
            int year = f.Year;
            int month = f.Month;
            FinanceCalendar financeCalendar = TheFinanceCalendarMgr.GetFinanceCalendar(year, month);
            if (financeCalendar == null)
            {
                ShowErrorMessage("会计期间不存在");
                return;
            }

            SqlParameter[] sqlParam = new SqlParameter[3];
            sqlParam[0] = new SqlParameter("@p0", year);
            sqlParam[1] = new SqlParameter("@p1", month);

            if (tbItemCategory.Text.Trim() != string.Empty)
            {
                sql += " and item.category=@p2 ";
                sqlParam[2] = new SqlParameter("@p2", this.tbItemCategory.Text.Trim());
            }
            else
            {
                sql += " order by item.category ";
            }

            DataSet dataSet = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);

            this.GV_List.DataSource = dataSet;
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

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //e.Row.Cells[1].Attributes.Add("style", "vnd.ms-excel.numberformat:@");

            double startQty = double.Parse(e.Row.Cells[7].Text == "&nbsp;" ? "0" : e.Row.Cells[7].Text);
            double inQty = double.Parse(e.Row.Cells[8].Text == "&nbsp;" ? "0" : e.Row.Cells[8].Text);
            double outQty = double.Parse(e.Row.Cells[9].Text == "&nbsp;" ? "0" : e.Row.Cells[9].Text);
            double endQty = double.Parse(e.Row.Cells[10].Text == "&nbsp;" ? "0" : e.Row.Cells[10].Text);

            e.Row.Cells[9].Text = (-(startQty + inQty - endQty)).ToString("0.##");
        }
    }


}
