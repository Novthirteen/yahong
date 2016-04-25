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
using NHibernate.Expression;
using System.Collections.Generic;
using com.Sconit.Web;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using com.Sconit.Service.MasterData;
using com.Sconit.Utility;
using com.Sconit.Facility.Entity;

public partial class Facility_FacilityStock_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.tbStartDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            IList<CodeMaster> statusList = GetStatusGroup();
            statusList.Insert(0, new CodeMaster()); //添加空选项
            this.ddlStatus.DataSource = statusList;
            this.ddlStatus.DataBind();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    protected override void DoSearch()
    {
        if (SearchEvent != null)
        {
            string startDate = this.tbStartDate.Text.Trim() != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
            string endDate = this.tbEndDate.Text.Trim() != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
            string stNo = this.tbStNo.Text.Trim() != string.Empty ? this.tbStNo.Text.Trim() : string.Empty;
            string status = this.ddlStatus.SelectedValue.Trim();

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityStockMaster));

            if (startDate != string.Empty)
            {
                selectCriteria.Add(Expression.Ge("EffDate", DateTime.Parse(startDate)));
            }
            if (endDate != string.Empty)
            {
                selectCriteria.Add(Expression.Le("EffDate", DateTime.Parse(endDate)));
            }
            if (stNo != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Code", stNo, MatchMode.Start));
            }
            if (status != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Status", status));
            }
            DetachedCriteria selectCountCriteria = CloneHelper.DeepClone<DetachedCriteria>(selectCriteria);
            selectCountCriteria.SetProjection(Projections.Count("StNo"));
            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
        }
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        if (actionParameter.ContainsKey("StartDate"))
        {
            this.tbStartDate.Text = actionParameter["StartDate"];
        }
        if (actionParameter.ContainsKey("EndDate"))
        {
            this.tbEndDate.Text = actionParameter["EndDate"];
        }
        if (actionParameter.ContainsKey("OrderNo"))
        {
            this.tbStNo.Text = actionParameter["OrderNo"];
        }
        if (actionParameter.ContainsKey("Status"))
        {
            this.ddlStatus.SelectedValue = actionParameter["Status"];
        }
    }

    private IList<CodeMaster> GetStatusGroup()
    {
        IList<CodeMaster> statusGroup = new List<CodeMaster>();
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE));
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT));
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS));
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE));
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE));

        return statusGroup;
    }

    private CodeMaster GetStatus(string statusValue)
    {
        return TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_STATUS, statusValue);
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        if (NewEvent != null)
        {
            NewEvent(sender, e);
        }
    }
}
