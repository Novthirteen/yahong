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
using System.Text;

public partial class ISI_TaskSubType_Edit : EditModuleBase
{
    public string ModuleType
    {
        get
        {
            return (string)ViewState["ModuleType"];
        }
        set
        {
            ViewState["ModuleType"] = value;
        }
    }
    public event EventHandler BackEvent;

    public string TaskSubTypeCode
    {
        get
        {
            return (string)ViewState["TaskSubTypeCode"];
        }
        set
        {
            ViewState["TaskSubTypeCode"] = value;
        }
    }
    public string TaskSubTypeDesc
    {
        get
        {
            return (string)ViewState["TaskSubTypeDesc"];
        }
        set
        {
            ViewState["TaskSubTypeDesc"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
        {
            GenHtml();
        }
    }

    public void InitPageParameter(string code, string desc)
    {
        this.TaskSubTypeCode = code;
        TaskSubTypeDesc = desc;
        this.ODS_TaskSubType.SelectParameters["Code"].DefaultValue = this.TaskSubTypeCode;
        this.ODS_TaskSubType.DeleteParameters["Code"].DefaultValue = this.TaskSubTypeCode;
    }

    protected void FV_TaskSubType_DataBound(object sender, EventArgs e)
    {
        if (TaskSubTypeCode != null)
        {
            TaskSubType taskSubType = (TaskSubType)((FormView)sender).DataItem;
            TaskSubTypeDesc = taskSubType.Desc;
            UpdateView(taskSubType);
        }
    }

    private void UpdateView(TaskSubType taskSubType)
    {
        this.FV_TaskSubType.FindControl("fs").Visible = this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT;
        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT)
        {
            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlProjectType"))).IncludeBlankOption = false;
            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlProjectType"))).DataBind();
            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlColor"))).IncludeBlankOption = false;
            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlColor"))).DataBind();
            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlECType"))).IncludeBlankOption = false;
            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlECType"))).DataBind();
            ((Literal)this.FV_TaskSubType.FindControl("ltlAssignUser")).Text = "${ISI.TaskSubType.TeamLeader}:";
            ((Literal)this.FV_TaskSubType.FindControl("lblIsReport")).Text = "${ISI.TaskSubType.IsProjectReport}:";
        }
        Controls_TextBox tbParent = (Controls_TextBox)this.FV_TaskSubType.FindControl("tbParent");
        TextBox tbECUser = (TextBox)this.FV_TaskSubType.FindControl("tbECUser");
        TextBox tbAssignUser = (TextBox)this.FV_TaskSubType.FindControl("tbAssignUser");
        TextBox tbStartUser = (TextBox)this.FV_TaskSubType.FindControl("tbStartUser");
        TextBox tbAssignUpUser = (TextBox)this.FV_TaskSubType.FindControl("tbAssignUpUser");
        TextBox tbStartUpUser = (TextBox)this.FV_TaskSubType.FindControl("tbStartUpUser");
        TextBox tbCloseUpUser = (TextBox)this.FV_TaskSubType.FindControl("tbCloseUpUser");
        TextBox tbViewUser = (TextBox)this.FV_TaskSubType.FindControl("tbViewUser");
        CodeMstrDropDownList ddlProjectType = ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlProjectType")));
        CodeMstrDropDownList ddlColor = ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlColor")));
        CodeMstrDropDownList ddlOrg = ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlOrg")));

        if (taskSubType.Parent != null)
        {
            tbParent.Text = taskSubType.Parent.Code;
        }
        ddlProjectType.SelectedValue = taskSubType.ProjectType;
        ddlColor.SelectedValue = taskSubType.Color;
        ddlOrg.SelectedValue = taskSubType.Org;
        tbECUser.Text = ISIUtil.EditUser(taskSubType.ECUser);
        tbAssignUser.Text = ISIUtil.EditUser(taskSubType.AssignUser);
        tbViewUser.Text = ISIUtil.EditUser(taskSubType.ViewUser);
        tbStartUser.Text = ISIUtil.EditUser(taskSubType.StartUser);
        tbAssignUpUser.Text = ISIUtil.EditUser(taskSubType.AssignUpUser);
        tbStartUpUser.Text = ISIUtil.EditUser(taskSubType.StartUpUser);
        tbCloseUpUser.Text = ISIUtil.EditUser(taskSubType.CloseUpUser);
        if (taskSubType.Type == ISIConstants.ISI_TASK_TYPE_PRIVACY)
        {
            this.FV_TaskSubType.FindControl("trPublicView").Visible = false;
            this.FV_TaskSubType.FindControl("trAdmin").Visible = false;
            this.FV_TaskSubType.FindControl("trFlowAdmin").Visible = false;
            this.FV_TaskSubType.FindControl("trPublicAssign").Visible = false;
            this.FV_TaskSubType.FindControl("trPublicClose").Visible = false;
        }
        else
        {
            ((TextBox)this.FV_TaskSubType.FindControl("tbPublicView")).Text = TheTaskMgr.FindUserNameByPermission(new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW });
            ((TextBox)this.FV_TaskSubType.FindControl("tbPublicAssign")).Text = TheTaskMgr.FindUserNameByPermission(new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN });
            ((TextBox)this.FV_TaskSubType.FindControl("tbPublicClose")).Text = TheTaskMgr.FindUserNameByPermission(new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE });
            ((TextBox)this.FV_TaskSubType.FindControl("tbAdmin")).Text = TheTaskMgr.FindUserNameByPermission(new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN });

            string flowAdmin = TheTaskMgr.FindUserNameByPermission(new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN });
            if (flowAdmin.Length > 0)
            {
                ((TextBox)this.FV_TaskSubType.FindControl("tbFlowAdmin")).Text = flowAdmin;
            }
            else
            {
                this.FV_TaskSubType.FindControl("trFlowAdmin").Visible = false;
            }
        }

        GenHtml();

    }

    private void GenHtml()
    {
        if (!string.IsNullOrEmpty(this.TaskSubTypeCode))
        {
            if (this.ModuleType != ISIConstants.ISI_TASK_TYPE_GENERAL)
            {
                string type = TheTaskMgr.FindUserNameByPermission(new string[] { ISIConstants.ISI_TASK_TYPE_WORKFLOW });
                if (type.Length > 0)
                {
                    ((TextBox)this.FV_TaskSubType.FindControl("tbPublicType")).Text = type;
                }
                else
                {
                    this.FV_TaskSubType.FindControl("trPublicType").Visible = false;
                }
            }
            else
            {
                ((HtmlTableRow)this.FV_TaskSubType.FindControl("trPublicType")).Visible = false;

                HtmlTable userTable = (HtmlTable)this.FV_TaskSubType.FindControl("userTable");
                int i = 15;
                foreach (var type in ISIConstants.TaskTypeList)
                {
                    StringBuilder str = new StringBuilder();
                    str.Append(TheTaskMgr.FindUserNameByPermission(new string[] { type }));
                    if (str.Length > 0)
                    {
                        HtmlTableCell tc1 = new HtmlTableCell();
                        HtmlTableCell tc2 = new HtmlTableCell();
                        TextBox tb = new TextBox();
                        tb.Text = str.ToString();
                        tb.ReadOnly = true;
                        tb.TextMode = TextBoxMode.MultiLine;
                        tb.Height = 50;
                        tb.Width = new Unit(77, UnitType.Percentage);
                        Literal lbl = new Literal();
                        lbl.Text = "${ISI.TSK." + type + "}:";

                        tc1.Attributes.Add("class", "td01");
                        tc1.Controls.Add(lbl);
                        //tc2.Attributes.Add("class", "td02");
                        //tc2.ColSpan = 3;
                        tc2.Controls.Add(tb);
                        HtmlTableRow tr = new HtmlTableRow();
                        tr.Controls.Add(tc1);
                        tr.Controls.Add(tc2);
                        userTable.Rows.Insert(userTable.Rows.Count, tr);
                    }
                }
            }
            //((TextBox)this.FV_TaskSubType.FindControl("tbPublicType")).Text = TheTaskMgr.FindUserNameByPermission(ISIConstants.TaskTypeList.ToArray());
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_TaskSubType_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("ISI.TaskSubType.UpdateTaskSubType.Successfully", TaskSubTypeCode);
        //btnBack_Click(this, e);
    }

    protected void ODS_TaskSubType_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        TaskSubType taskSubType = (TaskSubType)e.InputParameters[0];
        if (taskSubType != null)
        {
            TaskSubType oldTaskSubType = TheTaskSubTypeMgr.LoadTaskSubType(taskSubType.Code);
            taskSubType.CreateDate = oldTaskSubType.CreateDate;
            taskSubType.CreateUser = oldTaskSubType.CreateUser;
            taskSubType.Type = oldTaskSubType.Type;
            taskSubType.IsCostCenter = oldTaskSubType.IsCostCenter;
            taskSubType.IsAssignUser = oldTaskSubType.IsAssignUser;
            taskSubType.IsWF = oldTaskSubType.IsWF;
            taskSubType.IsTrace = oldTaskSubType.IsTrace;
            taskSubType.IsCtrl = oldTaskSubType.IsCtrl;
            taskSubType.IsRemind = oldTaskSubType.IsRemind;
            taskSubType.IsApply = oldTaskSubType.IsApply;
            taskSubType.IsPrint = oldTaskSubType.IsPrint;
            taskSubType.Template = oldTaskSubType.Template;
            taskSubType.IsRemoveForm = oldTaskSubType.IsRemoveForm;
            taskSubType.ProcessNo = oldTaskSubType.ProcessNo;
            taskSubType.CostCenter = oldTaskSubType.CostCenter;
            taskSubType.IsAttachment = oldTaskSubType.IsAttachment;
            taskSubType.FormType = oldTaskSubType.FormType;
            taskSubType.IsBudget = oldTaskSubType.IsBudget;
            taskSubType.IsAmount = oldTaskSubType.IsAmount;
            taskSubType.IsAmountDetail = oldTaskSubType.IsAmountDetail;
            
            string parent = ((Controls_TextBox)(this.FV_TaskSubType.FindControl("tbParent"))).Text.Trim();
            if (!string.IsNullOrEmpty(parent))
            {
                taskSubType.Parent = this.TheTaskSubTypeMgr.LoadTaskSubType(parent);
            }

            taskSubType.ECType =
                ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlECType"))).SelectedValue;

            taskSubType.ProjectType =
                ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlProjectType"))).SelectedValue;
            taskSubType.Color =
                ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlColor"))).SelectedValue;

            taskSubType.Org =
                ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlOrg"))).SelectedValue;
            
            taskSubType.StartUser = ISIUtil.GetUser(taskSubType.StartUser);
            taskSubType.AssignUser = ISIUtil.GetUser(taskSubType.AssignUser);
            taskSubType.ECUser = ISIUtil.GetUser(taskSubType.ECUser);
            taskSubType.ViewUser = ISIUtil.GetUser(taskSubType.ViewUser);
            taskSubType.StartUpUser = ISIUtil.GetUser(taskSubType.StartUpUser);
            taskSubType.AssignUpUser = ISIUtil.GetUser(taskSubType.AssignUpUser);
            taskSubType.CloseUpUser = ISIUtil.GetUser(taskSubType.CloseUpUser);

            taskSubType.LastModifyDate = DateTime.Now;
            taskSubType.LastModifyUser = this.CurrentUser.Code;
            taskSubType.Version = oldTaskSubType.Version;

        }
    }

    protected void ODS_TaskSubType_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        TaskSubType taskSubType = (TaskSubType)e.InputParameters[0];
        if (TheTaskSubTypeMgr.IsRef(taskSubType.Code))
        {
            ShowErrorMessage("ISI.TaskSubType.DeleteTaskSubType.Fail", TaskSubTypeCode.ToString());
            e.Cancel = true;
        }
    }
    protected void ODS_TaskSubType_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("ISI.TaskSubType.DeleteTaskSubType.Successfully", TaskSubTypeCode);
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("ISI.TaskSubType.DeleteTaskSbType.Fail", TaskSubTypeCode);
            e.ExceptionHandled = true;
        }
    }

    protected void checkUser(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;

        switch (cv.ID)
        {
            case "cvStartUser":
            case "cvECUser":
            case "cvAssignUser":
            case "cvViewUser":
            case "cvStartUpUser":
            case "cvAssignUpUser":
            case "cvCloseUpUser":
                string invalidUser = TheTaskMgr.GetInvalidUser(args.Value, this.CurrentUser.Code);
                if (!string.IsNullOrEmpty(invalidUser))
                {
                    cv.ErrorMessage = this.TheLanguageMgr.TranslateMessage("ISI.Error.UserNotExist", this.CurrentUser, new string[] { invalidUser });
                    args.IsValid = false;
                }
                break;
            default:
                break;
        }
    }
}
