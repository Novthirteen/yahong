using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using com.Sconit.Web;
using com.Sconit.Entity;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using com.Sconit.Utility;
using NHibernate;
using NHibernate.Type;
using System.Data.SqlClient;
using System.Data;

public partial class ManageSconit_LeanEngine_Multi_Main : MainModuleBase
{


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    protected void btnRun_Click(object sender, EventArgs e)
    {
        TheMrpMgr.RunMrp(this.CurrentUser);
    }


    protected void btnTest_Click(object sender, EventArgs e)
    {
        test();
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }

    private void test()
    {
        string sql = @"select top 10 flow as 物流路线,flowtype as 路线类型,Item as 物料,Qty as 数量,Uom as 单位 from Mrp_PlanTrans entity 
                      where entity.EffDate  = @p1";
        List<SqlParameter> sqlPara = new List<SqlParameter>();
        sqlPara.Add(new SqlParameter("@p1", "2011-7-20"));
        DataSet dataSet = TheSqlHelperMgr.GetDatasetBySql(sql, sqlPara.ToArray());

        this.GV_List.DataSource = dataSet;
        this.GV_List.DataBind();
    }

    class LastActionQty
    {
        public string Flow { get; set; }
        public string Item { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitQty { get; set; }
    }
}

