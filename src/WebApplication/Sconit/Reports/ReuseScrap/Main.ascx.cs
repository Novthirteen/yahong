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
using com.Sconit.Utility;
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
            string sql = @"select loctrans.item as 物料代码,item.desc1 as 描述1,item.desc2 as 描述2,loctrans.Uom as 单位,
                            cast(sum(qty) as numeric(12,2)) as 数量 from ordermstr
                            left join loctrans on loctrans.orderno = ordermstr.orderno
                            left join item on item.code = loctrans.item
                            where ordermstr.flow =@p1 and loctrans.transtype = @p2
                            and loctrans.createdate>=@p3 and loctrans.createdate<@p4
                            and  ordermstr.SubType='RWO'
                            group by loctrans.item,item.desc1,item.desc2,loctrans.Uom";

            string sql1 = @"";

            SqlParameter[] sqlParam = new SqlParameter[4];

            //sqlParam[0] = new SqlParameter("@p0", itemCode);
            sqlParam[0] = new SqlParameter("@p1", flowCode);
            sqlParam[1] = new SqlParameter("@p2", "ISS-WO");//原材料
            sqlParam[2] = new SqlParameter("@p3", startTime);
            sqlParam[3] = new SqlParameter("@p4", endTime);

            //产出的回用
            DataSet dataSetRm = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);
            List<ReuseScrap> reuseScrapRms = IListHelper.DataTableToList<ReuseScrap>(dataSetRm.Tables[0]);

            //投入的成品
            sqlParam[1] = new SqlParameter("@p2", "RCT-WO");//成品
            DataSet dataSetFg = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);
            List<ReuseScrap> reuseScrapFgs = IListHelper.DataTableToList<ReuseScrap>(dataSetFg.Tables[0]);

            List<ReuseScrap> reuseScraps = new List<ReuseScrap>();
            //bom拆分
            foreach (var r in reuseScrapFgs)
            {
                IList<BomDetail> bomDetailList = TheBomDetailMgr.GetFlatBomDetail(r.物料代码, DateTime.Now);

                if (bomDetailList != null)
                {
                    var a = from b in bomDetailList
                            select new ReuseScrap
                            {
                                单位 = b.Uom.Code,
                                描述1 = b.Item.Desc1,
                                描述2 = b.Item.Desc2,
                                数量 = b.RateQty * r.数量,
                                物料代码 = b.Item.Code
                            };
                    reuseScraps.AddRange(a.ToList());
                }
            }

            //汇总
            List<ReuseScrap> groupReuseScraps = (from g in reuseScraps
                                                 group g by new { g.物料代码, g.描述1, g.描述2, g.单位 } into result
                                                 select new ReuseScrap
                                                 {
                                                     单位 = result.Key.单位,
                                                     描述1 = result.Key.描述1,
                                                     描述2 = result.Key.描述2,
                                                     数量 = reuseScraps.Where(r => r.物料代码 == result.Key.物料代码).Sum(r => r.数量),
                                                     物料代码 = result.Key.物料代码
                                                 }).ToList();


            //报废=汇总-回用的
            foreach (var r in groupReuseScraps)
            {
                r.数量 = r.数量 + reuseScrapRms.Where(s => s.物料代码 == r.物料代码).Sum(s => s.数量);
            }

            groupReuseScraps = groupReuseScraps.Where(s => s.数量 != 0).ToList();

            this.GV_List.DataSource = groupReuseScraps;
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
            e.Row.Cells[5].Text = decimal.Parse(e.Row.Cells[5].Text).ToString("0.########");
        }
    }

    class ReuseScrap
    {
        public string 物料代码 { get; set; }
        public string 描述1 { get; set; }
        public string 描述2 { get; set; }
        public string 单位 { get; set; }
        public decimal 数量 { get; set; }
    }

}
