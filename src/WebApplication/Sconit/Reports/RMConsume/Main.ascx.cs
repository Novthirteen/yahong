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

public partial class Reports_RMConsume_Main : MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code;
        this.tbTransfer.ServiceParameter = "string:" + this.CurrentUser.Code;
        if (!IsPostBack)
        {
            this.tbStartDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm");
            this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string flowCode = this.tbFlow.Text.Trim();
        string transfer = this.tbTransfer.Text.Trim();
        if (flowCode==string.Empty || transfer== string.Empty)
        {
            ShowErrorMessage("请数据生产线和与之关联的移库路线");
            return;
        }


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
            string sql = @"with tr as
                            (
                            select m.OrderNo,d.Item as Rm, d.Uom as RmUom, ISNULL(d.OrderQty,0) as RmQty
                            from OrderMstr  m
                            left join OrderDet d on m.OrderNo = d.OrderNo
                            where m.Flow = @p4 and m.Status in('Submit','In-process')
                            and m.StartTime >= @p2 and m.StartTime < @p3
                            union
                            select m.OrderNo,d.Item as Rm, d.Uom as RmUom, ISNULL(d.RecQty,0) as RmQty
                            from OrderMstr  m
                            left join OrderDet d on m.OrderNo = d.OrderNo
                            where m.Flow =@p4 and m.Status in('Complete','Close')
                            and m.StartTime >= @p2 and m.StartTime < @p3 
                            )
                            select tr.Rm as 原材料,Item.Desc1 as 描述1,Item.Desc2 as 描述2,tr.RmUom as 单位,
                            SUM(RMQty) as 数量 into #temptr
                            from tr left join Item on Item.Code = tr.Rm
                            group by tr.Rm,Item.Desc1,Item.Desc2,tr.RmUom;

                            with wo as
                            (
                            select m.OrderNo,t.Item as Rm,t.Uom as RmUom,
                            ISNULL(d.OrderQty,0)*t.UnitQty as RMQty, 0 as RmScrapQty
                            from OrderMstr  m
                            left join OrderDet d on m.OrderNo = d.OrderNo
                            left join OrderLocTrans t on t.OrderDetId = d.Id
                            where m.Flow = @p1 and m.Status in('Submit','In-process')
                            and m.StartTime >= @p2 and m.StartTime < @p3 and m.SubType ='Nml' 
                            and t.IOType = 'Out'
                            union
                            select m.OrderNo,t.Item as Rm,t.Uom as RmUom,
                            ISNULL(d.RecQty,0)*t.UnitQty as RMQty,ISNULL(d.ScrapQty,0)*t.UnitQty as RmScrapQty
                             from OrderMstr  m
                            left join OrderDet d on m.OrderNo = d.OrderNo
                            left join OrderLocTrans t on t.OrderDetId = d.Id
                            where m.Flow = @p1 and m.Status in('Complete','Close')
                            and m.StartTime >= @p2 and m.StartTime < @p3 and m.SubType ='Nml' 
                            and t.IOType = 'Out'
                            )
                            select  wo.Rm as 原材料,Item.Desc1 as 描述1,Item.Desc2 as 描述2,wo.RmUom as 单位,
                            SUM(RMQty)+ SUM(RmScrapQty) as 数量 into #tempwo
                            from wo left join Item on Item.Code = wo.Rm
                            group by wo.Rm,Item.Desc1,Item.Desc2,wo.RmUom;

                            select ISNULL(a.原材料,b.原材料) as 原材料,ISNULL(a.描述1,b.描述1) as 描述1,
                            ISNULL(a.描述2,b.描述2) as 描述2,ISNULL(a.单位,b.单位) as 单位,
                            cast(ISNULL(a.数量,0) as numeric(12,2)) as 实际领用, 
                            cast(ISNULL(b.数量,0)  as numeric(12,2)) as 理论耗用
                            from #temptr a
                            full join #tempwo b on a.原材料 = b.原材料
                            ";

            SqlParameter[] sqlParam = new SqlParameter[4];

            sqlParam[0] = new SqlParameter("@p1", flowCode);
            sqlParam[1] = new SqlParameter("@p2", startTime);
            sqlParam[2] = new SqlParameter("@p3", endTime);
            sqlParam[3] = new SqlParameter("@p4", transfer);

           
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
            e.Row.Cells[1].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            //e.Row.Cells[3].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
    }

    protected void tbFinanceYear_TextChange(object sender, EventArgs e)
    {
        DateTime f = DateTime.Parse(this.tbFinanceYear1.Text);
        int year = f.Year;
        int month = f.Month;
        FinanceCalendar financeCalendar = TheFinanceCalendarMgr.GetFinanceCalendar(year, month);

        this.tbStartDate.Text = financeCalendar.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
        this.tbEndDate.Text = financeCalendar.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
