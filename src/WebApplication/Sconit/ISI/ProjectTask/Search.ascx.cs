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
using com.Sconit.Web;
using com.Sconit.Utility;
using com.Sconit.Entity;
using NHibernate.Transform;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using Geekees.Common.Controls;
using com.Sconit.Entity.MasterData;
using System.Text;
using NHibernate.SqlCommand;

public partial class ISI_ProjectTask_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    private IDictionary<string, string> parameter = new Dictionary<string, string>();
     
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            
            
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (SearchEvent != null)
        {
            DoSearch();
        }
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        this.parameter = actionParameter;
        if (actionParameter.ContainsKey("ProjectType"))
        {
            this.ddlProjectType.Text = actionParameter["ProjectType"];
        }
        if (actionParameter.ContainsKey("Subject"))
        {
            this.tbSubject.Text = actionParameter["Subject"];
        }
        if (actionParameter.ContainsKey("Desc"))
        {
            this.tbDesc.Text = actionParameter["Desc"];
        }
        if (actionParameter.ContainsKey("ProjectSubType"))
        {
            this.ddlProjectSubType.Text = actionParameter["ProjectSubType"];
        }
        if (actionParameter.ContainsKey("Phase"))
        {
            this.ddlPhase.Text = actionParameter["Phase"];
        }
        if (actionParameter.ContainsKey("Seq"))
        {
            this.tbSeq.Text = actionParameter["Seq"];
        }
    }

    protected override void DoSearch()
    {
        string seq = this.tbSeq.Text.Trim();
        string desc = this.tbDesc.Text.Trim();
        string subject = this.tbSubject.Text.Trim();
        string projectType = this.ddlProjectType.SelectedValue;
        string projectSubType = this.ddlProjectSubType.SelectedValue;
        string phase = this.ddlPhase.SelectedValue;

        #region DetachedCriteria

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(ProjectTask));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(ProjectTask)).SetProjection(Projections.Count("Id"));
            
        selectCriteria.Add(Expression.Eq("IsActive", true));
        selectCountCriteria.Add(Expression.Eq("IsActive", true));

        if (!string.IsNullOrEmpty(subject))
        {
            selectCriteria.Add(Expression.Like("Subject", subject, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Subject", subject, MatchMode.Anywhere));
        }
        if (!string.IsNullOrEmpty(desc))
        {
            selectCriteria.Add(Expression.Like("Desc", desc, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Desc", desc, MatchMode.Anywhere));
        }
        if (!string.IsNullOrEmpty(seq))
        {
            selectCriteria.Add(Expression.Like("Seq", seq, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Seq", seq, MatchMode.Anywhere));
        }
        if (!string.IsNullOrEmpty(projectSubType))
        {
            selectCriteria.Add(Expression.Eq("ProjectSubType", projectSubType));
            selectCountCriteria.Add(Expression.Eq("ProjectSubType", projectSubType));
        }
        if (!string.IsNullOrEmpty(phase))
        {
            selectCriteria.Add(Expression.Eq("Phase", phase));
            selectCountCriteria.Add(Expression.Eq("Phase", phase));
        }
        if (!string.IsNullOrEmpty(projectType))
        {
            selectCriteria.Add(Expression.Eq("ProjectType", projectType));
            selectCountCriteria.Add(Expression.Eq("ProjectType", projectType));
        }
        selectCriteria.AddOrder(Order.Asc("ProjectType"));
        selectCriteria.AddOrder(Order.Asc("ProjectSubType"));
        selectCriteria.AddOrder(Order.Asc("Phase"));
        selectCriteria.AddOrder(Order.Asc("Seq"));

        SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
        #endregion

    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }
 
}
