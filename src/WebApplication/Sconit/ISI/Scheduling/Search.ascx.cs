using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity;
using NHibernate.SqlCommand;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using System.Text;

public partial class ISI_Scheduling_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    public bool IsSpecial
    {
        get
        {
            return (bool)ViewState["IsSpecial"];
        }
        set
        {
            ViewState["IsSpecial"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.isSpecial.Visible = !this.IsSpecial;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    public void UpdateView()
    {
        DoSearch();
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

        if (actionParameter.ContainsKey("TaskSubType"))
        {
            this.tbTaskSubType.Text = actionParameter["TaskSubType"];
        }
        if (actionParameter.ContainsKey("DayOfWeek"))
        {
            this.ddlDayOfWeek.SelectedValue = actionParameter["DayOfWeek"];
        }
        if (actionParameter.ContainsKey("Shift"))
        {
            this.tbShift.Text = actionParameter["Shift"];
        }
        if (actionParameter.ContainsKey("StartUser"))
        {
            this.tbStartUser.Text = actionParameter["StartUser"];
        }
    }

    protected override void DoSearch()
    {
        string taskSubType = this.tbTaskSubType.Text.Trim();
        string dayOfWeek = this.ddlDayOfWeek.SelectedValue;
        string shift = this.tbShift.Text.Trim();
        string startUser = this.tbStartUser.Text.Trim();

        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(Scheduling));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(Scheduling)).SetProjection(Projections.Count("Id"));

            //selectCountCriteria.CreateCriteria("TaskSubType", "tst", JoinType.InnerJoin);
            if (!string.IsNullOrEmpty(shift))
            {
                selectCriteria.Add(Expression.Eq("Shift", shift));
                selectCountCriteria.Add(Expression.Eq("Shift", shift));
            }
            if (!string.IsNullOrEmpty(dayOfWeek))
            {
                selectCriteria.Add(Expression.Eq("DayOfWeek", dayOfWeek));
                selectCountCriteria.Add(Expression.Eq("DayOfWeek", dayOfWeek));
            }

            if (!string.IsNullOrEmpty(taskSubType))
            {
                selectCriteria.Add(Expression.Eq("TaskSubType.Code", taskSubType));
                selectCountCriteria.Add(Expression.Eq("TaskSubType.Code", taskSubType));
            }
            selectCriteria.Add(Expression.Eq("IsSpecial", IsSpecial));
            selectCountCriteria.Add(Expression.Eq("IsSpecial", IsSpecial));
            if (!string.IsNullOrEmpty(startUser))
            {
                selectCriteria.Add(Expression.Or(Expression.Or(Expression.Like("StartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                ,
                                                                Expression.Like("StartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                                ,
                                                 Expression.Or(Expression.Like("StartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                ,
                                                                Expression.Like("StartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))));

                selectCountCriteria.Add(Expression.Or(Expression.Or(Expression.Like("StartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                    ,
                                                                    Expression.Like("StartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                                    ,
                                                     Expression.Or(Expression.Like("StartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                    ,
                                                                    Expression.Like("StartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))));

            }

            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
            #endregion
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }
}
