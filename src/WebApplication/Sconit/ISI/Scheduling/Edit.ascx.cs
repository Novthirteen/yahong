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
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Control;

using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Scheduling_Edit : EditModuleBase
{
    public event EventHandler BackEvent;
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
    protected int SchedulingId
    {
        get
        {
            return (int)ViewState["SchedulingId"];
        }
        set
        {
            ViewState["SchedulingId"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void InitPageParameter(int id)
    {
        this.SchedulingId = id;
        this.ODS_Scheduling.SelectParameters["Id"].DefaultValue = this.SchedulingId.ToString();
        this.ODS_Scheduling.DeleteParameters["Id"].DefaultValue = this.SchedulingId.ToString();
    }

    protected void FV_Scheduling_DataBound(object sender, EventArgs e)
    {
        if (SchedulingId != 0)
        {
            Scheduling scheduling = (Scheduling)((FormView)sender).DataItem;
            UpdateView(scheduling);
        }
    }

    private void UpdateView(Scheduling scheduling)
    {
        if (IsSpecial)
        {
            this.FV_Scheduling.FindControl("dayOfWeekShiftDv").Visible = false;
            this.FV_Scheduling.FindControl("isSpecialdv").Visible = true;
            ((RequiredFieldValidator)this.FV_Scheduling.FindControl("rfvStartDate")).Enabled = true;
            ((RequiredFieldValidator)this.FV_Scheduling.FindControl("rfvEndDate")).Enabled = true;
        }
        else
        {
            this.FV_Scheduling.FindControl("dayOfWeekShiftDv").Visible = true;
            this.FV_Scheduling.FindControl("isSpecialdv").Visible = false;
            ((RequiredFieldValidator)this.FV_Scheduling.FindControl("rfvStartDate")).Enabled = false;
            ((RequiredFieldValidator)this.FV_Scheduling.FindControl("rfvEndDate")).Enabled = false;
        }
        TextBox tbTaskSubType = (TextBox)this.FV_Scheduling.FindControl("tbTaskSubType");
        tbTaskSubType.Text = scheduling.TaskSubType.Code;
        /*
        Controls_TextBox tbTaskSubType = (Controls_TextBox)this.FV_Scheduling.FindControl("tbTaskSubType");
        tbTaskSubType.Text = scheduling.TaskSubType.Code;

        Controls_TextBox tbShift = (Controls_TextBox)this.FV_Scheduling.FindControl("tbShift");
        tbShift.Text = scheduling.Shift;
        */
        TextBox tbStartUser = (TextBox)this.FV_Scheduling.FindControl("tbStartUser");
        tbStartUser.Text = ISIUtil.EditUser(scheduling.StartUser);
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_Scheduling_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("ISI.Scheduling.UpdateScheduling.Successfully");
        btnBack_Click(this, e);
    }

    protected void ODS_Scheduling_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        Scheduling scheduling = (Scheduling)e.InputParameters[0];
        if (scheduling != null)
        {
            if (this.IsSpecial)
            {
                string startDate = ((TextBox)(this.FV_Scheduling.FindControl("tbStartDate"))).Text.Trim();
                string endDate = ((TextBox)(this.FV_Scheduling.FindControl("tbEndDate"))).Text.Trim();
                if (string.IsNullOrEmpty(startDate))
                {
                    scheduling.StartDate = null;
                }
                else
                {
                    scheduling.StartDate = DateTime.Parse(startDate);
                }

                if (string.IsNullOrEmpty(endDate))
                {
                    scheduling.EndDate = null;
                }
                else
                {
                    scheduling.EndDate = DateTime.Parse(endDate);
                }

                if (scheduling.StartDate.HasValue &&
                    scheduling.EndDate.HasValue &&
                    DateTime.Compare(scheduling.StartDate.Value, scheduling.EndDate.Value) >= 0)
                {
                    ShowWarningMessage("ISI.Scheduling.WarningMessage.TimeCompare");
                    e.Cancel = true;
                }
            }
            else
            {
                scheduling.StartDate = null;
                scheduling.EndDate = null;
            }

            Scheduling oldScheduling = TheSchedulingMgr.LoadScheduling(scheduling.Id);
            scheduling.CreateDate = oldScheduling.CreateDate;
            scheduling.CreateUser = oldScheduling.CreateUser;
            scheduling.IsSpecial = oldScheduling.IsSpecial;
            scheduling.TaskSubType = oldScheduling.TaskSubType;
            scheduling.Shift = oldScheduling.Shift;
            scheduling.DayOfWeek = oldScheduling.DayOfWeek;

            scheduling.StartUser = ISIUtil.GetUser(scheduling.StartUser);
            /*
            string taskSubType = ((Controls_TextBox)(this.FV_Scheduling.FindControl("tbTaskSubType"))).Text.Trim();
            string shift = ((Controls_TextBox)(this.FV_Scheduling.FindControl("tbShift"))).Text.Trim();
            if (string.IsNullOrEmpty(taskSubType))
            {
                scheduling.TaskSubType = null;
            }
            else
            {
                TaskSubType subType = new TaskSubType();
                subType.Code = taskSubType;
                scheduling.TaskSubType = subType;
            }
            scheduling.Shift = shift;
            */
            scheduling.LastModifyDate = DateTime.Now;
            scheduling.LastModifyUser = this.CurrentUser.Code;
        }

    }

    protected void ODS_Scheduling_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        Scheduling scheduling = (Scheduling)e.InputParameters[0];
        //IList<Scheduling> schedulingList = TheSchedulingMgr.GetSchedulingByParent(scheduling.Code);
        //if (schedulingList != null && schedulingList.Count > 0)
        /*
        if (TheSchedulingMgr.IsRef(scheduling.Code))
        {
            ShowErrorMessage("ISI.Scheduling.DeleteScheduling.Ref.Fail", SchedulingId.ToString());
            e.Cancel = true;
        }*/
    }
    protected void ODS_Scheduling_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("ISI.Scheduling.DeleteScheduling.Successfully");
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("ISI.Scheduling.DeleteScheduling.Fail");
            e.ExceptionHandled = true;
        }
    }

    protected void Save_ServerValidate(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;
        string startDate = ((TextBox)(this.FV_Scheduling.FindControl("tbStartDate"))).Text.Trim();
        string endDate = ((TextBox)(this.FV_Scheduling.FindControl("tbEndDate"))).Text.Trim();
        string taskSubType = ((TextBox)(this.FV_Scheduling.FindControl("tbTaskSubType"))).Text.Trim();
        switch (cv.ID)
        {
            case "cvStartUser":
                string invalidUser = TheTaskMgr.GetInvalidUser(args.Value, this.CurrentUser.Code);
                if (!string.IsNullOrEmpty(invalidUser))
                {
                    //ShowErrorMessage("ISI.TaskAddress.CodeExist", args.Value);
                    cv.ErrorMessage = this.TheLanguageMgr.TranslateMessage("ISI.Error.UserNotExist", this.CurrentUser, new string[] { invalidUser });
                    args.IsValid = false;
                }
                break;
            case "cvStartEnd":
            case "cvEndDate":
                bool isHas = this.TheSchedulingMgr.HasSpecialScheduling2(taskSubType, DateTime.Parse(startDate), DateTime.Parse(endDate), this.SchedulingId);
                if (isHas)
                {
                    cv.ErrorMessage = "${ISI.Scheduling.WarningMessage.Error5}";
                    args.IsValid = false;
                }
                break;
            default:
                break;
        }
    }

}
