using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.Exception;

public partial class ISI_UserSubscription_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        InitPageParameter();
        if (!IsPostBack)
        {

        }
    }

    public void InitPageParameter()
    {
        this.ODS_GV_UserSubscription.SelectParameters["userCode"].DefaultValue = this.CurrentUser.Code;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            IList<UserSubView> userSubViewList = this.PopulateData();
            this.TheUserSubscriptionMgr.UpdateUserSubscription(userSubViewList, this.CurrentUser);
            this.GV_UserSubscription.DataBind();
            this.ShowSuccessMessage("ISI.UserSubscription.Successfully");
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private IList<UserSubView> PopulateData()
    {
        if (this.GV_UserSubscription.Rows != null && this.GV_UserSubscription.Rows.Count > 0)
        {
            IList<UserSubView> userSubViewList = new List<UserSubView>();
            foreach (GridViewRow row in this.GV_UserSubscription.Rows)
            {
                CheckBox cbIsEmail = row.FindControl("cbIsEmail") as CheckBox;
                CheckBox cbIsSMS = row.FindControl("cbIsSMS") as CheckBox;
                TextBox tbEmail = row.FindControl("tbEmail") as TextBox;
                TextBox tbMobilePhone = row.FindControl("tbMobilePhone") as TextBox;
                UserSubView userSubView = new UserSubView();
                HiddenField hfId = row.FindControl("hfId") as HiddenField;
                if (!string.IsNullOrEmpty(hfId.Value.Trim()) & hfId.Value.Trim() != "null")
                {
                    userSubView.Id = int.Parse(hfId.Value.Trim());
                }
                else
                {
                    userSubView.Id = null;
                }

                userSubView.TaskSubTypeCode = row.Cells[1].Text.Trim();

                userSubView.Email = tbEmail.Text.Trim();
                if (string.IsNullOrEmpty(userSubView.Email))
                {
                    userSubView.IsEmail = false;
                }
                else
                {
                    userSubView.IsEmail = cbIsEmail.Checked;
                }

                userSubView.MobilePhone = tbMobilePhone.Text.Trim();
                if (string.IsNullOrEmpty(userSubView.MobilePhone))
                {
                    userSubView.IsSMS = false;
                }
                else
                {
                    userSubView.IsSMS = cbIsSMS.Checked;
                }
                userSubViewList.Add(userSubView);
            }
            return userSubViewList;
        }

        return null;
    }

    protected void FV_List_DataBound(object sender, EventArgs e)
    {
        if ((((System.Web.UI.WebControls.GridView)(sender))).PageCount == 0)
        {
            btnSave.Visible = false;
        }
        else
        {
            btnSave.Visible = true;
        }
    }
    protected void FV_List_OnDataBinding(object sender, EventArgs e)
    {

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            UserSubView userSubView = (UserSubView)e.Row.DataItem;
            if (userSubView.MobilePhone == null || userSubView.MobilePhone.Trim() == string.Empty || !ISIUtil.IsValidMobilePhone(userSubView.MobilePhone.Trim()))
            {
                TextBox tbMobilePhone = (TextBox)e.Row.FindControl("tbMobilePhone");
                tbMobilePhone.Text = string.Empty;
            }
            if (userSubView.Email == null || userSubView.Email.Trim() == string.Empty || !ISIUtil.IsValidEmail(userSubView.Email.Trim()))
            {
                TextBox tbEmail = (TextBox)e.Row.FindControl("tbEmail");
                tbEmail.Text = string.Empty;
            }
        }
    }

    protected void ODS_GV_UserSubscription_OnUpdating(object source, ObjectDataSourceMethodEventArgs e)
    {
        ///this.GV_UserSubscription.EditIndex
        //UserSubView userSubView = (UserSubView)e.InputParameters[0];
        /*
        if (userSubView != null)
        {
            userSubView.IsEmail = ((CheckBox)GV_UserSubscription.Rows[this.GV_UserSubscription.EditIndex].Cells[4].Controls[1]).Checked;
            userSubView.IsSMS = ((CheckBox)GV_UserSubscription.Rows[this.GV_UserSubscription.EditIndex].Cells[6].Controls[1]).Checked;
        }
        */
        //userSubView.UserCode = this.CurrentUser.Code;

    }

    protected void GV_UserSubscription_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        string email = ((TextBox)GV_UserSubscription.Rows[e.RowIndex].Cells[5].Controls[1]).Text.ToString().Trim();
        string mobilePhone = ((TextBox)GV_UserSubscription.Rows[e.RowIndex].Cells[7].Controls[1]).Text.ToString().Trim();

        if (!string.IsNullOrEmpty(email) && !ISIUtil.IsValidEmail(email))
        {
            e.Cancel = true;
            ShowErrorMessage("ISI.Error.MailIsInvalid", email);
        }
        else if (!string.IsNullOrEmpty(mobilePhone) && !ISIUtil.IsValidMobilePhone(mobilePhone))
        {
            e.Cancel = true;
            ShowErrorMessage("ISI.Error.MobilePhoneIsInvalid", mobilePhone);
        }
        else
        {
            string Code = GV_UserSubscription.Rows[e.RowIndex].Cells[1].Text;
            ShowSuccessMessage("ISI.UserSubscription.Update.Successfully", Code);
        }
    }
}