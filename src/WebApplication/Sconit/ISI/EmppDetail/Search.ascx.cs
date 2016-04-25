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

public partial class ISI_EmppDetail_Search : SearchModuleBase
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
        
        if (actionParameter.ContainsKey("Content"))
        {
            this.tbContent.Text = actionParameter["Content"];
        }
        if (actionParameter.ContainsKey("DestID"))
        {
            this.tbDestID.Text = actionParameter["DestID"];
        }
        if (actionParameter.ContainsKey("MsgID"))
        {
            this.tbMsgID.Text = actionParameter["MsgID"];
        }
    }

    protected override void DoSearch()
    {
        
        string content = this.tbContent.Text.Trim();
        string destID = this.tbDestID.Text.Trim();
        string msgID = this.tbMsgID.Text.Trim();
        string startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        string endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;

        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(EmppDetail));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(EmppDetail)).SetProjection(Projections.Count("Id"));
            
            if (content != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Content", content, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Content", content, MatchMode.Anywhere));
            }
            if (destID != string.Empty)
            {
                selectCriteria.Add(Expression.Like("DestID", destID, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("DestID", destID, MatchMode.Anywhere));
            }
            if (msgID != string.Empty)
            {
                selectCriteria.Add(Expression.Like("MsgID", msgID, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("MsgID", msgID, MatchMode.Anywhere));
            }

            if (startDate != string.Empty)
            {
                selectCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(startDate)));
                selectCountCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(startDate)));
            }
            if (endDate != string.Empty)
            {
                selectCriteria.Add(Expression.Lt("CreateDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
                selectCountCriteria.Add(Expression.Lt("CreateDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
            }

            selectCriteria.Add(Expression.Not(Expression.Eq("EventHandler", ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_MESSAGERECEIVEDINTERFACE)));
            selectCountCriteria.Add(Expression.Not(Expression.Eq("EventHandler", ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_MESSAGERECEIVEDINTERFACE)));

            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
            #endregion
        }
    }


}
