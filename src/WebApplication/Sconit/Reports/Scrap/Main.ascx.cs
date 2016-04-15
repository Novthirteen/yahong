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

public partial class Reports_Scrap_Main : MainModuleBase
{
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

        DetachedCriteria criteria = DetachedCriteria.For(typeof(OrderLocationTransaction));
        criteria.CreateAlias("OrderDetail", "od");
        criteria.CreateAlias("od.OrderHead", "oh");
        criteria.Add(Expression.Eq("IOType", BusinessConstants.IO_TYPE_OUT));
        criteria.Add(Expression.Eq("oh.SubType", BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_ADJ));
        criteria.Add(Expression.Eq("oh.Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION));
        criteria.Add(Expression.Eq("oh.Priority", BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL));
        criteria.Add(Expression.IsNotNull("AccumulateQty"));
        criteria.Add(Expression.Gt("AccumulateQty", 0M));

        if (startTime.HasValue)
        {
            criteria.Add(Expression.Ge("oh.CreateDate", startTime));
        }
        if (endTime.HasValue)
        {
            criteria.Add(Expression.Le("oh.CreateDate", endTime));
        }
        if (itemCode != string.Empty)
        {
            criteria.Add(Expression.Eq("Item.Code", itemCode));
        }
        if (flowCode != string.Empty)
        {
            criteria.Add(Expression.Eq("oh.Flow", flowCode));
        }

        if (this.rblListFormat.SelectedIndex == 0)
        {
            criteria.SetProjection(Projections.ProjectionList()
                .Add(Projections.Sum("AccumulateQty").As("AccumulateQty"))
                .Add(Projections.GroupProperty("Location").As("Location"))
                .Add(Projections.GroupProperty("Item").As("Item"))
                );

            criteria.SetResultTransformer(Transformers.AliasToBean(typeof(OrderLocationTransaction)));
            this.GV_List_Group.DataSource = TheCriteriaMgr.FindAll<OrderLocationTransaction>(criteria, 0, 500);
            this.GV_List_Group.DataBind();
            this.GV_List_Group.Visible = true;
            this.Gv_List_Detail.Visible = false;
            if ((Button)sender == this.btnExport)
            {
                this.ExportXLS(this.GV_List_Group);
            }
        }
        else
        {
            this.Gv_List_Detail.DataSource = TheCriteriaMgr.FindAll<OrderLocationTransaction>(criteria, 0, 500);
            this.Gv_List_Detail.DataBind();
            this.GV_List_Group.Visible = false;
            this.Gv_List_Detail.Visible = true;
            if ((Button)sender == this.btnExport)
            {
                this.ExportXLS(this.Gv_List_Detail);
            }
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
