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

public partial class Reports_WorkHours_Main : MainModuleBase
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
        string itemCategory = this.tbItemCategory.Text.Trim();
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
            string sql = @"select loctrans.item as 物料,item.category as 产品类,item.desc1 as 描述1,item.desc2 as 描述2,
                            loctrans.uom as 单位,loctrans.loc as 库位,routingdet.runtime as 单位耗时分钟,sum(qty) as 总入库数,
                            routingdet.runtime * sum(qty)/60 as 总耗时小时 from loctrans
                            left join routingdet on routingdet.routing= loctrans.item
                            left join item on item.code = loctrans.item
                            left join location on location.code = loctrans.loc
                            left join OrderMstr on OrderMstr.OrderNo = LocTrans.OrderNo
                            where transtype ='RCT-WO' and location.type='NML'                          
                            ";

            SqlParameter[] sqlParam = new SqlParameter[5];

            sqlParam[0] = new SqlParameter("@p1", itemCategory);
            sqlParam[1] = new SqlParameter("@p2", startTime);
            sqlParam[2] = new SqlParameter("@p3", endTime);
            sqlParam[3] = new SqlParameter("@p4", itemCode);
            sqlParam[4] = new SqlParameter("@p5", flowCode);

            if (!string.IsNullOrEmpty(itemCategory))
            {
                sql += "  and item.category = @p1 ";
            }
            if (!string.IsNullOrEmpty(itemCode))
            {
                sql += " and loctrans.item = @p4 ";
            }
            if (startTime.HasValue)
            {
                sql += " and loctrans.CreateDate > @p2 ";
            }
            if (endTime.HasValue)
            {
                sql += " and loctrans.CreateDate < @p3 ";
            }
            if (!string.IsNullOrEmpty(flowCode))
            {
                sql += "  and OrderMstr.Flow = @p5 ";
            }

            sql += @"group by loctrans.item,item.category,item.desc1,item.desc2,loctrans.uom,loctrans.loc,routingdet.runtime
                     order by loctrans.loc";

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
