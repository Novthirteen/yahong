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
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Filter_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.ddlTaskType.Items.RemoveAt(1);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        if (actionParameter.ContainsKey("TaskCode"))
        {
            this.tbTaskCode.Text = actionParameter["TaskCode"];
        }
        if (actionParameter.ContainsKey("Description"))
        {
            this.tbDesc.Text = actionParameter["Description"];
        }
        if (actionParameter.ContainsKey("UserCode"))
        {
            this.tbUserCode.Text = actionParameter["UserCode"];
        }
        if (actionParameter.ContainsKey("Email"))
        {
            this.tbEmail.Text = actionParameter["Email"];
        }
        if (actionParameter.ContainsKey("TaskSubType"))
        {
            this.tbTaskSubType.Text = actionParameter["TaskSubType"];
        }
    }

    protected override void DoSearch()
    {
        string taskCode = this.tbTaskCode.Text.Trim();
        string desc = this.tbDesc.Text.Trim();
        string userCode = this.tbUserCode.Text.Trim();
        string email = this.tbEmail.Text.Trim();
        string taskType = this.ddlTaskType.SelectedValue;
        string taskSubType = this.tbTaskSubType.Text.Trim();
        
        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(Filter));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(Filter)).SetProjection(Projections.Count("Id"));
            if (taskCode != string.Empty)
            {
                selectCriteria.Add(Expression.Like("TaskCode", taskCode,MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("TaskCode", taskCode, MatchMode.Anywhere));
            }
            if (email != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Email", email, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Email", email, MatchMode.Anywhere));
            }
            if (desc != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Description", desc, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Description", desc, MatchMode.Anywhere));
            }
            if (taskType != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("TaskType", taskType));
                selectCountCriteria.Add(Expression.Eq("TaskType", taskType));
            }
            
            if (taskSubType != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("TaskSubType", taskSubType));
                selectCountCriteria.Add(Expression.Eq("TaskSubType", taskSubType));
            }

            if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_PERMISSION_FILTER_VALUE_FILTERADMIN))
            {
                selectCriteria.Add(Expression.Or(Expression.Eq("CreateUser", this.CurrentUser.Code), Expression.Eq("LastModifyUser", this.CurrentUser.Code)));
                selectCountCriteria.Add(Expression.Or(Expression.Eq("CreateUser", this.CurrentUser.Code), Expression.Eq("LastModifyUser", this.CurrentUser.Code)));
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
