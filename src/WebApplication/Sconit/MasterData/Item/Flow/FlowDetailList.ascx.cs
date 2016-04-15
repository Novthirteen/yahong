using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;


public partial class Inventory_PrintHu_FlowDetailList : ModuleBase
{

    public void InitPageParameter(IList<FlowDetail> flowDetailList)
    {
        this.GV_List.DataSource = flowDetailList;
        this.GV_List.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FlowDetail flowDetail = (FlowDetail)(e.Row.DataItem);
            ((Label)e.Row.FindControl("lblLocationFrom")).Text = flowDetail.DefaultLocationFrom == null ? string.Empty : flowDetail.DefaultLocationFrom.Code;
            ((Label)e.Row.FindControl("lblLocationTo")).Text = flowDetail.DefaultLocationTo == null ? string.Empty : flowDetail.DefaultLocationTo.Code;

            ((Label)e.Row.FindControl("lblLocationFrom")).ToolTip = flowDetail.DefaultLocationFrom == null ? string.Empty : flowDetail.DefaultLocationFrom.Name;
            ((Label)e.Row.FindControl("lblLocationTo")).ToolTip = flowDetail.DefaultLocationTo == null ? string.Empty : flowDetail.DefaultLocationTo.Name;
        }
    }

}
