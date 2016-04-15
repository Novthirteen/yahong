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
public partial class Report_Reuse_Main : MainModuleBase
{
    public string ModuleType
    {
        get
        {
            return this.ModuleParameter["ModuleType"];
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code;
        if (!IsPostBack)
        {
            this.tbStartDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm");
            this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //string itemCode = this.tbItem.Text.Trim();
        string flowCode = this.tbFlow.Text.Trim();
        DateTime startTime = DateTime.Now.AddDays(-30);
        if (this.tbStartDate.Text.Trim() != string.Empty)
        {
            startTime = DateTime.Parse(this.tbStartDate.Text.Trim());
        }
        else
        {
            this.tbStartDate.Text = startTime.ToString("yyyy-MM-dd HH:mm");
        }
        DateTime endTime = DateTime.Now;
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            endTime = DateTime.Parse(this.tbEndDate.Text.Trim());
        }

        if (flowCode == string.Empty)
        {
            ShowErrorMessage("路线不能为空");
            return;
        }

        try
        {
            string sql = @"select loctrans.item as 物料代码,item.desc1 as 描述1,item.desc2 as 描述2,
                            cast(sum(qty) as numeric(12,2)) as 数量 from ordermstr
                            left join loctrans on loctrans.orderno = ordermstr.orderno
                            left join item on item.code = loctrans.item
                            where ordermstr.flow =@p1 and loctrans.transtype = @p2
                            and loctrans.createdate>=@p3 and loctrans.createdate<@p4
                            and  ordermstr.SubType='RWO'
                            group by loctrans.item,item.desc1,item.desc2";



            SqlParameter[] sqlParam = new SqlParameter[4];

            //sqlParam[0] = new SqlParameter("@p0", itemCode);
            sqlParam[0] = new SqlParameter("@p1", flowCode);
            sqlParam[1] = new SqlParameter("@p2", this.rblListFormat.SelectedValue);
            sqlParam[2] = new SqlParameter("@p3", startTime);
            sqlParam[3] = new SqlParameter("@p4", endTime);

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
            e.Row.Cells[1].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            //e.Row.Cells[4].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
    }

}
