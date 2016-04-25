using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate.Expression;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Text;

public partial class ISI_PermissionTrack_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }
    protected void btnRemovePermission_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
            {
                IList<int> upIdList = new List<int>();
                IList<int> rpIdList = new List<int>();

                foreach (GridViewRow row in this.GV_List.Rows)
                {
                    HiddenField hfUpId = row.FindControl("hfUpId") as HiddenField;
                    HiddenField hfType = row.FindControl("hfType") as HiddenField;
                    CheckBox cbGroup = row.FindControl("cbGroup") as CheckBox;
                    if (cbGroup.Checked && hfUpId.Value != "0" && hfUpId.Value != "null" && !string.IsNullOrEmpty(hfUpId.Value))
                    {
                        int id = int.Parse(hfUpId.Value);
                        if (hfType.Value == "Role")
                        {
                            rpIdList.Add(id);
                        }
                        else
                        {
                            upIdList.Add(id);
                        }
                    }
                }
                if (upIdList.Count > 0)
                {
                    this.TheUserPermissionMgr.DeleteUserPermission(upIdList);
                    ShowSuccessMessage("ISI.PermissionTrack.RemovePermission.Successfully");
                }
                if (rpIdList.Count > 0)
                {
                    this.TheRolePermissionMgr.DeleteRolePermission(rpIdList);
                }
                if (upIdList.Count > 0 || rpIdList.Count > 0)
                {
                    this.btnSearch_Click(null, null);
                    ShowSuccessMessage("ISI.PermissionTrack.RemovePermission.Successfully");
                }
            }

            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("ISI.PermissionTrack.Remove.Fail");
        }
    }
    protected void btnRemoveRole_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
            {
                IList<int> urIdList = new List<int>();

                foreach (GridViewRow row in this.GV_List.Rows)
                {
                    HiddenField hfUrId = row.FindControl("hfUrId") as HiddenField;
                    CheckBox cbGroup = row.FindControl("cbGroup") as CheckBox;
                    if (cbGroup.Checked && hfUrId.Value != "0" && hfUrId.Value != "null" && !string.IsNullOrEmpty(hfUrId.Value))
                    {
                        int id = int.Parse(hfUrId.Value);
                        urIdList.Add(id);
                    }
                }
                if (urIdList.Count > 0)
                {
                    this.TheUserRoleMgr.DeleteUserRole(urIdList);
                    this.btnSearch_Click(null, null);
                    ShowSuccessMessage("ISI.PermissionTrack.RemoveRole.Successfully");
                }
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage("ISI.PermissionTrack.Remove.Fail");
        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string user = this.tbUser.Text.Trim();
            string permission = this.tbPermission.Text.Trim();
            if (!string.IsNullOrEmpty(user) || !string.IsNullOrEmpty(permission))
            {
                DataSet dataSet;
                GetDateSet(out dataSet, string.Empty);

                this.GV_List.DataSource = dataSet;
                this.GV_List.DataBind();
                this.fld_Gv_List.Visible = true;
                if ((Button)sender == this.btnExport)
                {
                    this.ExportXLS(this.GV_List);
                }
            }
            else
            {
                this.ShowWarningMessage("ISI.PermissionTrack.Warning.UserOrPermissionIsEmpty");
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }


    private void GetDateSet(out DataSet dataSet, string sortdirection)
    {
        string user = this.tbUser.Text.Trim();
        string category = this.tbCategory.Text.Trim();
        string categoryType = this.tbCategoryType.Text.Trim();
        string permission = this.tbPermission.Text.Trim();

        IList<SqlParameter> sqlParam = new List<SqlParameter>();
        sqlParam.Add(new SqlParameter("@UserCode", tbUser.Text.Trim()));
        sqlParam.Add(new SqlParameter("@CateCode", category));
        sqlParam.Add(new SqlParameter("@TypeCode", categoryType));
        sqlParam.Add(new SqlParameter("@PermissionCode", permission));

        dataSet = TheSqlHelperMgr.GetDatasetByStoredProcedure("USP_Req_PermissionTrack", sqlParam.ToArray<SqlParameter>());

    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.IsExport = true;
        this.btnSearch_Click(sender, e);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var p = ((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray;
            e.Row.Cells[2].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[3].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[6].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");

            //LinkButton lbtnRemovePermission = (LinkButton)e.Row.FindControl("lbtnRemovePermission");
            if (p[6].ToString() == "User")
            {
                e.Row.Cells[6].Text = string.Empty;
                e.Row.Cells[7].Text = string.Empty;
                e.Row.Cells[8].Text = string.Empty;
                e.Row.Cells[9].Text = string.Empty;
            }
        }
    }

    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        string user = this.tbUser.Text.Trim();
        string category = this.tbCategory.Text.Trim();
        string categoryType = this.tbCategoryType.Text.Trim();
        string permission = this.tbPermission.Text.Trim();
        /*this.GV_List.Columns[1].Visible = string.IsNullOrEmpty(permission) && string.IsNullOrEmpty(categoryType);
        this.GV_List.Columns[2].Visible = string.IsNullOrEmpty(permission) && string.IsNullOrEmpty(category);
        this.GV_List.Columns[3].Visible = string.IsNullOrEmpty(permission);
        this.GV_List.Columns[10].Visible = string.IsNullOrEmpty(user);
         * */
    }
}