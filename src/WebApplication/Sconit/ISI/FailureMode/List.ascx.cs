﻿using System;
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
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using System.Collections.Generic;


public partial class ISI_FailureMode_List : ListModuleBase
{
    public EventHandler EditEvent;
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            EditEvent(code, e);
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code = ((LinkButton)sender).CommandArgument;
        try
        {
            //IList<FailureMode> failureModeList = TheFailureModeMgr.GetFailureModeByParent(code);
            //if (failureModeList != null && failureModeList.Count > 0)
            if (TheFailureModeMgr.IsRef(code))
            {
                ShowErrorMessage("ISI.FailureMode.DeleteFailureMode.Ref.Fail", code);
            }
            else
            {
                TheFailureModeMgr.DeleteFailureMode(code);
                ShowSuccessMessage("ISI.FailureMode.DeleteFailureMode.Successfully", code);
                UpdateView();
            }
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.FailureMode.DeleteFailureMode.Fail", code);
        }

    }
}