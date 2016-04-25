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
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Web;
using com.Sconit.Control;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Scheduling_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
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
    private Scheduling scheduling;
    protected void Page_Load(object sender, EventArgs e)
    {
       
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void FV_Scheduling_DataBound(object sender, EventArgs e)
    {
        if (IsSpecial)
        {
            this.FV_Scheduling.FindControl("isSpecialDv").Visible = true;
            this.FV_Scheduling.FindControl("dayOfWeekShiftDv").Visible = false;
            ((RequiredFieldValidator)this.FV_Scheduling.FindControl("rfvStartDate")).Enabled = true;
            ((RequiredFieldValidator)this.FV_Scheduling.FindControl("rfvEndDate")).Enabled = true;
            ((CustomValidator)this.FV_Scheduling.FindControl("cvWeek")).Enabled = false;
            ((CustomValidator)this.FV_Scheduling.FindControl("cvTaskSubType")).Enabled = false;
        }
        else
        {
            
            this.FV_Scheduling.FindControl("isSpecialDv").Visible = false;
            this.FV_Scheduling.FindControl("dayOfWeekShiftDv").Visible = true;
            ((RequiredFieldValidator)this.FV_Scheduling.FindControl("rfvStartDate")).Enabled = false;
            ((RequiredFieldValidator)this.FV_Scheduling.FindControl("rfvEndDate")).Enabled = false;
            ((CustomValidator)this.FV_Scheduling.FindControl("cvWeek")).Enabled = true;
        }
        ((Controls_TextBox)this.FV_Scheduling.FindControl("tbTaskSubType")).ServiceParameter = "string:" + this.CurrentUser.Code;
        ((Controls_TextBox)this.FV_Scheduling.FindControl("tbTaskSubType")).DataBind();
       
    }

    public void PageCleanup()
    {
        ((TextBox)(this.FV_Scheduling.FindControl("tbStartUser"))).Text = string.Empty;
        ((TextBox)(this.FV_Scheduling.FindControl("tbDesc"))).Text = string.Empty;
        ((TextBox)(this.FV_Scheduling.FindControl("tbStartDate"))).Text = string.Empty;
        ((TextBox)(this.FV_Scheduling.FindControl("tbEndDate"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_Scheduling.FindControl("tbTaskSubType"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_Scheduling.FindControl("tbShift"))).Text = string.Empty;
        ((System.Web.UI.WebControls.DropDownList)(this.FV_Scheduling.FindControl("ddlDayOfWeek"))).SelectedIndex = 0;
    }


    protected void Save_ServerValidate(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;
        string shift = ((Controls_TextBox)(this.FV_Scheduling.FindControl("tbShift"))).Text.Trim();
        string taskSubType = ((Controls_TextBox)(this.FV_Scheduling.FindControl("tbTaskSubType"))).Text.Trim();
        string dayOfWeek = ((System.Web.UI.WebControls.DropDownList)(this.FV_Scheduling.FindControl("ddlDayOfWeek"))).SelectedValue;
        string startDate = ((TextBox)(this.FV_Scheduling.FindControl("tbStartDate"))).Text.Trim();
        string endDate = ((TextBox)(this.FV_Scheduling.FindControl("tbEndDate"))).Text.Trim();
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
            case "cvWeek":
                if (string.IsNullOrEmpty(shift) && !string.IsNullOrEmpty(dayOfWeek))
                {
                    bool exists = this.TheSchedulingMgr.Exists(dayOfWeek, shift, taskSubType, this.IsSpecial);
                    if (exists)
                    {

                        args.IsValid = false;

                    }
                }
                break;
            case "cvShift":
                if (!string.IsNullOrEmpty(shift) && string.IsNullOrEmpty(dayOfWeek))
                {
                    bool exists = this.TheSchedulingMgr.Exists(dayOfWeek, shift, taskSubType, this.IsSpecial);
                    if (exists)
                    {
                        args.IsValid = false;
                    }
                }
                break;
            case "cvTaskSubType":
                if ((!string.IsNullOrEmpty(shift) && !string.IsNullOrEmpty(dayOfWeek))
                        || (string.IsNullOrEmpty(shift) && string.IsNullOrEmpty(dayOfWeek)))
                {
                    bool exists = this.TheSchedulingMgr.Exists(dayOfWeek, shift, taskSubType, this.IsSpecial);
                    if (exists)
                    {
                        args.IsValid = false;
                        if (string.IsNullOrEmpty(shift) && string.IsNullOrEmpty(dayOfWeek))
                        {
                            cv.ErrorMessage = "${ISI.Scheduling.WarningMessage.Error3}";
                        }
                        else if (!string.IsNullOrEmpty(shift) && !string.IsNullOrEmpty(dayOfWeek))
                        {
                            cv.ErrorMessage = "${ISI.Scheduling.WarningMessage.Error4}";
                        }
                    }
                }
                break;
            case "cvStartEnd":
            case "cvEndDate":
                bool isHas = this.TheSchedulingMgr.HasSpecialScheduling2(taskSubType, DateTime.Parse(startDate), DateTime.Parse(endDate));
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


    protected void ODS_Scheduling_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        scheduling = (Scheduling)e.InputParameters[0];

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

                scheduling.DayOfWeek = null;
                scheduling.Shift = null;
            }
            else
            {
                string shift = ((Controls_TextBox)(this.FV_Scheduling.FindControl("tbShift"))).Text.Trim();
                string dayOfWeek = ((System.Web.UI.WebControls.DropDownList)(this.FV_Scheduling.FindControl("ddlDayOfWeek"))).Text.Trim();
                /*
                if (this.TheSchedulingMgr.Exists(taskSubType, shift, this.IsSpecial))
                {
                    ShowWarningMessage("ISI.Scheduling.TaskSubTypeAndShiftExist", new string[] { taskSubType, shift });
                    e.Cancel = true;
                }
                 */
                scheduling.DayOfWeek = dayOfWeek;
                scheduling.Shift = shift;
                scheduling.StartDate = null;
                scheduling.EndDate = null;
            }

            scheduling.IsSpecial = this.IsSpecial;

            string taskSubType = ((Controls_TextBox)(this.FV_Scheduling.FindControl("tbTaskSubType"))).Text.Trim();

            scheduling.StartUser = ISIUtil.GetUser(scheduling.StartUser);

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

            DateTime now = DateTime.Now;
            scheduling.CreateDate = now;
            scheduling.CreateUser = this.CurrentUser.Code;
            scheduling.LastModifyDate = now;
            scheduling.LastModifyUser = this.CurrentUser.Code;
        }
    }

    protected void ODS_Scheduling_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(scheduling.Id.ToString(), e);
            ShowSuccessMessage("ISI.Scheduling.AddScheduling.Successfully");
        }
    }

}
