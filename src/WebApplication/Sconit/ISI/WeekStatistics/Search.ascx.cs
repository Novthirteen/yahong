using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Entity;
using com.Sconit.Web;
using NHibernate.Expression;
using com.Sconit.Entity.View;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity;

public partial class ISI_WeekStatistics_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler ExportEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DateTime now = DateTime.Now;

            this.lblStartEndDate.Text = now.AddDays(DayOfWeek.Monday - now.DayOfWeek).ToString("yyyy年MM月dd日") + "&#45;" + now.AddDays(DayOfWeek.Sunday - now.DayOfWeek + 7).ToString("yyyy年MM月dd日");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.DoSearch();
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (ExportEvent != null)
        {
            object[] param = this.CollectParam();
            if (param != null)
                ExportEvent(param, null);
        }
    }

    protected override void DoSearch()
    {

        if (SearchEvent != null)
        {
            object[] param = CollectParam();
            if (param != null)
                SearchEvent(param, null);
        }
    }

    private object[] CollectParam()
    {
        string dept = this.ddlDept.SelectedValue;
        string dept2 = this.tbDept2.Text.Trim();
        string position = ddlPosition.SelectedValue;

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskWeekStatisticsView));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(TaskWeekStatisticsView))
            .SetProjection(Projections.Count("Code"));


        if (this.tbUser.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Code", this.tbUser.Text.Trim()));
            selectCountCriteria.Add(Expression.Eq("Code", this.tbUser.Text.Trim()));
        }
        if (this.tbJobNo.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("JobNo", this.tbJobNo.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("JobNo", this.tbJobNo.Text.Trim(), MatchMode.Anywhere));
        }
        if (!string.IsNullOrEmpty(position))
        {
            selectCriteria.Add(Expression.Eq("Position", position));
            selectCountCriteria.Add(Expression.Eq("Position", position));
        }
        if (!string.IsNullOrEmpty(dept))
        {
            selectCriteria.Add(Expression.Eq("Dept", dept));
            selectCountCriteria.Add(Expression.Eq("Dept", dept));
        }

        if (!string.IsNullOrEmpty(dept2))
        {
            selectCriteria.Add(Expression.Eq("Dept2", dept2));
            selectCountCriteria.Add(Expression.Eq("Dept2", dept2));
        }
        selectCriteria.Add(Expression.Eq("IsActive", ckIsActive.Checked));
        selectCountCriteria.Add(Expression.Eq("IsActive", ckIsActive.Checked));
        return new object[] { selectCriteria, selectCountCriteria };

    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        //if (actionParameter.ContainsKey("Location"))
        //{
        //    this.tbLocation.Text = actionParameter["Location"];
        //}
        //if (actionParameter.ContainsKey("Item"))
        //{
        //    this.tbItem.Text = actionParameter["Item"];
        //}
        //if (actionParameter.ContainsKey("EffDate"))
        //{
        //    this.tbEffDate.Text = actionParameter["EffDate"];
        //}
    }
}