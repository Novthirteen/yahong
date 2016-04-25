using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;
using NHibernate.Expression;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.by liqiuyun
public partial class Modules_ISI_ResMatrix_Edit : EditModuleBase
{
    public event EventHandler Back;
    //Get the logger
    private static ILog log = LogManager.GetLogger("ISI");

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
        
    }

    private CodeMaster GetTimePeriodType(string statusValue)
    {
        return TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE, statusValue);
    }

    public void InitPageParameter(object Code)
    {
        this.ODS_ResMatrix.SelectParameters["Id"].DefaultValue = Code.ToString();
        this.ODS_ResMatrix.DeleteParameters["Id"].DefaultValue = Code.ToString();
        this.FV_ResMatrix.DataBind();

    }

    protected void FV_ResMatrix_DataBound(object sender, EventArgs e)
    {
        ResMatrix dataItem = (ResMatrix)this.FV_ResMatrix.DataItem;
        if (dataItem != null)
        {
            Controls_TextBox tbWorkShop = (Controls_TextBox)this.FV_ResMatrix.FindControl("tbWorkShop");
            tbWorkShop.Text = dataItem.WorkShop;
            Controls_TextBox tbRole = (Controls_TextBox)this.FV_ResMatrix.FindControl("tbRole");
            tbRole.Text = dataItem.Role;
            Controls_TextBox tbTaskSubType = (Controls_TextBox)this.FV_ResMatrix.FindControl("tbTaskSubType");
            tbTaskSubType.Text = dataItem.TaskSubType;
            //tbTaskSubType.ServiceParameter = "string:" + ISIConstants.ISI_TASK_TYPE_PLAN + ",string:" + this.CurrentUser.Code;
            var ddlTimePeriodType = (com.Sconit.Control.CodeMstrDropDownList)this.FV_ResMatrix.FindControl("ddlTimePeriodType");
            ddlTimePeriodType.SelectedValue = dataItem.TimePeriodType;

            //DropDownList ddlTimePeriodType = (DropDownList)this.FV_ResMatrix.FindControl("ddlTimePeriodType");
            //IList<CodeMaster> statusGroupList = new List<CodeMaster>();
            //statusGroupList.Add(GetTimePeriodType(BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_DAY));
            //statusGroupList.Add(GetTimePeriodType(BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_WEEK));
            //statusGroupList.Add(GetTimePeriodType(BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_MONTH));
            //statusGroupList.Add(GetTimePeriodType(BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_YEAR));
            //ddlTimePeriodType.DataSource = statusGroupList;
            //ddlTimePeriodType.DataBind();
            //if (!string.IsNullOrEmpty(dataItem.TimePeriodType))
            //{
            //    var list = TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE);

            //    ddlTimePeriodType.SelectedValue = dataItem.TimePeriodType;
            //    ddlTimePeriodType.Text = (list.FirstOrDefault(p => p.Value == dataItem.TimePeriodType) ?? new CodeMaster()).Description;
            //}
            ReBindGvList();
        }
    }

    private void ReBindGvList()
    {
        int id = int.Parse(this.ODS_ResMatrix.SelectParameters["Id"].DefaultValue);
        DetachedCriteria criteria = DetachedCriteria.For(typeof(ResUser));
        criteria.Add(Expression.Eq("MatrixId", id));

        var list = TheCriteriaMgr.FindAll<ResUser>(criteria);
        var resUser = new ResUser();
        resUser.StartDate = DateTime.Now.Date;
        resUser.EndDate = DateTime.Parse("2099-1-1");
        resUser.NeedPatrol = true;
        list.Add(resUser);

        GridView gv_List = (GridView)this.FV_ResMatrix.FindControl("GV_List");
        gv_List.DataSource = list;
        gv_List.DataBind();
    }


    protected void ODS_ResMatrix_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        ResMatrix dataItem = (ResMatrix)e.InputParameters[0];
        dataItem.LastModifyDate = DateTime.Now;
        dataItem.LastModifyUser = this.CurrentUser.Code;
        Controls_TextBox tbWorkShop = (Controls_TextBox)this.FV_ResMatrix.FindControl("tbWorkShop");
        dataItem.WorkShop = tbWorkShop.Text.Trim();
        Controls_TextBox tbRole = (Controls_TextBox)this.FV_ResMatrix.FindControl("tbRole");
        dataItem.Role = tbRole.Text.Trim();

        Controls_TextBox tbTaskSubType = (Controls_TextBox)this.FV_ResMatrix.FindControl("tbTaskSubType");
        dataItem.TaskSubType = tbTaskSubType.Text.Trim();
        //DropDownList ddlPriority = (DropDownList)this.FV_ResMatrix.FindControl("ddlPriority");
        //dataItem.Priority = ddlPriority.SelectedValue;

        DropDownList ddlTimePeriodType = (DropDownList)this.FV_ResMatrix.FindControl("ddlTimePeriodType");
        dataItem.TimePeriodType = ddlTimePeriodType.SelectedValue;
    }

    protected void ODS_ResMatrix_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {

        if (e.Exception != null)
        {
            ShowErrorMessage("Common.Business.Result.Update.Failed.Reason", e.Exception.InnerException.Message);
            e.ExceptionHandled = true;
        }
        else
        {
            Back(sender, e);
            ShowSuccessMessage("Common.Business.Result.Update.Successfully");
        }
    }

    protected void ODS_ResMatrix_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            ShowErrorMessage("Common.Business.Result.Delete.Failed");
            e.ExceptionHandled = true;
        }
        else
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Common.Business.Result.Delete.Successfully");
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Back != null)
        {
            this.Visible = false;
            Back(sender, e);
        }
    }

    protected void lbtnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton lb = (LinkButton)sender;
            //获取传过来的commentID
            int comId = int.Parse(lb.CommandArgument);

            GridView gv_List = (GridView)this.FV_ResMatrix.FindControl("GV_List");
            int rowIndex = ((GridViewRow)(((DataControlFieldCell)(((LinkButton)(sender)).Parent)).Parent)).RowIndex;
            GridViewRow newRow = gv_List.Rows[rowIndex];
            TextBox tbStartDate = (TextBox)newRow.FindControl("tbStartDate");
            TextBox tbEndDate = (TextBox)newRow.FindControl("tbEndDate");
            var ddlSkillLevel = (com.Sconit.Control.CodeMstrDropDownList)newRow.FindControl("ddlSkillLevel");
            DropDownList ddlPriority = (DropDownList)newRow.FindControl("ddlPriority");
            CheckBox cbNeedPatrol = (CheckBox)newRow.FindControl("cbNeedPatrol");
            if (comId == 0)
            {
                Controls_TextBox tbUserCode = (Controls_TextBox)newRow.FindControl("tbUserCode");

                int id = int.Parse(this.ODS_ResMatrix.SelectParameters["Id"].DefaultValue);
                DetachedCriteria criteria = DetachedCriteria.For(typeof(ResUser));
                criteria.Add(Expression.Eq("MatrixId", id));
                var list = TheCriteriaMgr.FindAll<ResUser>(criteria);
                if (list.Where(p => p.UserCode == tbUserCode.Text.Trim()).Count() > 0)
                {
                    ShowErrorMessage("ISI.ResUser.UserCode.Duplicate");
                }

                var newResUser = new ResUser();
                newResUser.UserCode = tbUserCode.Text.Trim();
                var user = this.TheUserMgr.LoadUser(newResUser.UserCode, false, false);
                newResUser.UserName = user.Name;
                newResUser.StartDate = DateTime.Parse(tbStartDate.Text.Trim());
                newResUser.EndDate = DateTime.Parse(tbEndDate.Text.Trim());
                newResUser.MatrixId = int.Parse(this.ODS_ResMatrix.SelectParameters["Id"].DefaultValue);
                newResUser.CreateDate = DateTime.Now;
                newResUser.CreateUser = this.CurrentUser.Code;
                newResUser.LastModifyDate = DateTime.Now;
                newResUser.LastModifyUser = this.CurrentUser.Code;
                newResUser.SkillLevel = ddlSkillLevel.SelectedValue;
                newResUser.Priority = ddlPriority.SelectedValue;
                newResUser.NeedPatrol = cbNeedPatrol.Checked;
                this.TheResUserMgr.CreateResUser(newResUser);
            }
            else
            {
                var resUser = this.TheResUserMgr.LoadResUser(comId);
                resUser.OldStartDate = resUser.StartDate;
                resUser.OldEndDate = resUser.EndDate;
                resUser.OldPriority = resUser.Priority;
                resUser.StartDate = DateTime.Parse(tbStartDate.Text.Trim());
                resUser.EndDate = DateTime.Parse(tbEndDate.Text.Trim());
                resUser.NeedPatrol = cbNeedPatrol.Checked;

                resUser.LastModifyDate = DateTime.Now;
                resUser.LastModifyUser = this.CurrentUser.Code;
                resUser.SkillLevel = ddlSkillLevel.SelectedValue;
                resUser.Priority = ddlPriority.SelectedValue;
                this.TheResUserMgr.UpdateResUser(resUser);
            }
            this.FV_ResMatrix.DataBind();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Common.Business.Result.Insert.Failed");
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton lb = (LinkButton)sender;
            string comId = lb.CommandArgument;
            var resUser = this.TheResUserMgr.LoadResUser(int.Parse(comId));
            resUser.LastModifyUser = this.CurrentUser.Code;
            resUser.LastModifyDate = DateTime.Now;
            this.TheResUserMgr.DeleteResUser(resUser);

            this.FV_ResMatrix.DataBind();
            ShowSuccessMessage("Common.Business.Result.Delete.Successfully");
        }
        catch (Exception)
        {
            ShowErrorMessage("Common.Business.Result.Delete.Failed");
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Controls_TextBox tbUserCode = (Controls_TextBox)e.Row.FindControl("tbUserCode");
        Label lblUserCode = (Label)e.Row.FindControl("lblUserCode");
        //RequiredFieldValidator rfvUserCode = (RequiredFieldValidator)e.Row.FindControl("rfvUserCode");
        TextBox tbStartDate = (TextBox)e.Row.FindControl("tbStartDate");
        CheckBox cbNeedPatrol = (CheckBox)e.Row.FindControl("cbNeedPatrol");
        TextBox tbEndDate = (TextBox)e.Row.FindControl("tbEndDate");
        LinkButton lbtnAdd = (LinkButton)e.Row.FindControl("lbtnAdd");
        LinkButton lbtnDelete = (LinkButton)e.Row.FindControl("lbtnDelete");


        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var ddlSkillLevel = (com.Sconit.Control.CodeMstrDropDownList)e.Row.FindControl("ddlSkillLevel");
            DropDownList ddlPriority = (DropDownList)e.Row.FindControl("ddlPriority");

            ResUser resUser = (ResUser)e.Row.DataItem;
            if (resUser.Id == 0)
            {
                tbUserCode.Visible = true;
                //rfvUserCode.Enabled = true;
                lblUserCode.Visible = false;
                lbtnAdd.Text = "${Common.Button.New}";
                lbtnDelete.Visible = false;
            }
            else
            {
                tbUserCode.Visible = false;
                //rfvUserCode.Enabled = false;
                lblUserCode.Visible = true;
                lbtnAdd.Text = "${Common.Button.Update}";
                lbtnDelete.Visible = true;
            }

            ddlSkillLevel.SelectedValue = resUser.SkillLevel;
            ddlPriority.SelectedValue = resUser.Priority;
            cbNeedPatrol.Checked = resUser.NeedPatrol;

            tbStartDate.Text = resUser.StartDate.ToString("yyyy-MM-dd HH:mm");
            tbEndDate.Text = resUser.EndDate.ToString("yyyy-MM-dd HH:mm");
        }
    }
}