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
using com.Sconit.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_EmppRec_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    protected void Page_Load(object sender, EventArgs e)
    {

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
        if (actionParameter.ContainsKey("Content"))
        {
            this.tbContent.Text = actionParameter["Content"];
        }
        if (actionParameter.ContainsKey("SrcID"))
        {
            this.tbSrcID.Text = actionParameter["SrcID"];
        }
        if (actionParameter.ContainsKey("MsgID"))
        {
            this.tbMsgID.Text = actionParameter["MsgID"];
        }
    }

    protected override void DoSearch()
    {
        string taskCode = this.tbTaskCode.Text.Trim();
        string content = this.tbContent.Text.Trim();
        string srcID = this.tbSrcID.Text.Trim();
        string msgID = this.tbMsgID.Text.Trim();
        string startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        string endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;

        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(EmppDetail));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(EmppDetail)).SetProjection(Projections.Count("Id"));
            if (!string.IsNullOrEmpty(taskCode))
            {
                selectCriteria.Add(Expression.Like("TaskCode", taskCode, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("TaskCode", taskCode, MatchMode.Anywhere));
            }
            if (!string.IsNullOrEmpty(content))
            {
                selectCriteria.Add(Expression.Like("Content", content, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Content", content, MatchMode.Anywhere));
            }
            if (!string.IsNullOrEmpty(srcID))
            {
                selectCriteria.Add(Expression.Like("SrcID", srcID, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("SrcID", srcID, MatchMode.Anywhere));
            }
            if (!string.IsNullOrEmpty(msgID))
            {
                selectCriteria.Add(Expression.Like("MsgID", msgID, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("MsgID", msgID, MatchMode.Anywhere));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                selectCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(startDate)));
                selectCountCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(startDate)));
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                selectCriteria.Add(Expression.Lt("CreateDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
                selectCountCriteria.Add(Expression.Lt("CreateDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
            }

            selectCriteria.Add(Expression.Eq("EventHandler", ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_MESSAGERECEIVEDINTERFACE));
            selectCountCriteria.Add(Expression.Eq("EventHandler", ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_MESSAGERECEIVEDINTERFACE));

            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
            #endregion
        }
    }


}
