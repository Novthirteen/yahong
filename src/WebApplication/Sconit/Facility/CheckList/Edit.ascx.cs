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
using System.Drawing;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.by liqiuyun
public partial class Facility_CheckList_Edit : EditModuleBase
{
    private CheckListMaster checkListMaster
    {
        get { return (CheckListMaster)ViewState["CheckListMaster"]; }
        set { ViewState["CheckListMaster"] = value; }
    }

    private String Code
    {
        get { return (string)ViewState["Code"]; }
        set { ViewState["Code"] = value; }
    }

    public event EventHandler Back;
    //Get the logger
    private static ILog log = LogManager.GetLogger("ISI");

    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbTaskSubType.ServiceParameter = "string:" + ISIConstants.ISI_TASK_TYPE_ISSUE + ",string:" + this.CurrentUser.Code;
        if (!IsPostBack)
        {

        }
    }

    public void InitPageParameter(string Code)
    {
        checkListMaster = this.TheGenericMgr.FindById<CheckListMaster>(Code);
        this.tbTaskSubType.Text = checkListMaster.TaskSubType;
        this.cbNeedCreateTask.Checked = checkListMaster.NeekCreateTask;
        this.Code = Code;
        this.lblCode.Text = checkListMaster.Code;
        this.tbName.Text = checkListMaster.Name;
        this.tbRegion.Text = checkListMaster.Region;
        this.tbDescription.Text = checkListMaster.Description;
        this.tbFacilityID.Text = checkListMaster.FacilityID;
        this.tbFacilityName.Text = checkListMaster.FacilityName;
        if (!string.IsNullOrEmpty(checkListMaster.SubUser))
        {
            string userNames = this.TheUserSubscriptionMgr.GetUserName(checkListMaster.SubUser);
            this.tbSubUser.Text = ISIUtil.GetUserMerge(checkListMaster.SubUser, userNames);
        }
        else
        {
            this.tbSubUser.Text = string.Empty;
        }
        DetailDataBind(checkListMaster);
    }

    private void DetailDataBind(CheckListMaster checkListMaster)
    {
        var details = this.TheGenericMgr.FindAll<CheckListDetail>("from CheckListDetail where CheckListCode = ? order by Seq desc ", checkListMaster.Code, 0, 25) ?? new List<CheckListDetail>();
        details = details.OrderBy(d => d.Seq).ToList();

        var newDetail = new CheckListDetail();
        newDetail.CreateDate = DateTime.Now;
        newDetail.CreateUser = this.CurrentUser.Code;
        newDetail.LastModifyDate = DateTime.Now;
        newDetail.LastModifyUser = this.CurrentUser.Code;

        newDetail.Seq = (details.LastOrDefault() ?? new CheckListDetail()).Seq + 1;
        newDetail.CheckListCode = checkListMaster.Code;
        details.Add(newDetail);

        this.GV_List.DataSource = details;
        this.GV_List.DataBind();
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

            GridView gv_List = this.GV_List;
            int rowIndex = ((GridViewRow)(((DataControlFieldCell)(((LinkButton)(sender)).Parent)).Parent)).RowIndex;
            GridViewRow newRow = gv_List.Rows[rowIndex];

            HiddenField hfSeq = (HiddenField)newRow.FindControl("hfSeq");
            TextBox tbDetailCode = (TextBox)newRow.FindControl("tbDetailCode");
            TextBox tbDetailDescription = (TextBox)newRow.FindControl("tbDetailDescription");
            TextBox tbMaxValue = (TextBox)newRow.FindControl("tbMaxValue");
            TextBox tbMinValue = (TextBox)newRow.FindControl("tbMinValue");
            CheckBox cbIsRequired = (CheckBox)newRow.FindControl("tbIsRequired");
            List<string> marks = new List<string>();

            double maxvalue = 0;
            if (!double.TryParse(tbMaxValue.Text.Trim(), out maxvalue))
            {
                ShowErrorMessage("输入的最大值不是数字");
                return;
            }

            double minvalue = 0;
            if (!double.TryParse(tbMinValue.Text.Trim(), out minvalue))
            {
                ShowErrorMessage("输入的最小值不是数字");
                return;
            }

            if (minvalue > maxvalue)
            {
                ShowErrorMessage("最大值不能小于最小值");
            }
            else
            { 
                if (comId == 0)
                {
                    var newDetail = new CheckListDetail();
                    newDetail.CreateDate = DateTime.Now;
                    newDetail.CreateUser = this.CurrentUser.Code;
                    newDetail.LastModifyDate = DateTime.Now;
                    newDetail.LastModifyUser = this.CurrentUser.Code;
                    newDetail.Seq = Convert.ToInt32(hfSeq.Value);

                    newDetail.CheckListCode = checkListMaster.Code;
                    newDetail.Code = tbDetailCode.Text.Trim();
                    newDetail.Description = tbDetailDescription.Text.Trim();
                    newDetail.MaxValue = maxvalue;
                    newDetail.MinValue = minvalue;
                    
                    newDetail.IsRequired = cbIsRequired.Checked;

                    if (marks.Count > 0)
                    {
                        ShowErrorMessage(string.Format("输入的{0}不是数字,保存失败", string.Join(",", marks.ToArray())));
                    }
                    else
                    {
                        TheGenericMgr.Create(newDetail);
                        ShowSuccessMessage("保存成功");
                    }
                }
                else
                {
                    var checkListDetail = this.TheGenericMgr.FindById<CheckListDetail>(comId);

                    checkListDetail.Code = tbDetailCode.Text.Trim();
                    checkListDetail.Description = tbDetailDescription.Text.Trim();
                    checkListDetail.MaxValue = maxvalue;
                    checkListDetail.MinValue = minvalue;
                    checkListDetail.IsRequired = cbIsRequired.Checked;

                    checkListDetail.LastModifyDate = DateTime.Now;
                    checkListDetail.LastModifyUser = this.CurrentUser.Code;
                    if (marks.Count > 0)
                    {
                        ShowErrorMessage(string.Format("输入的{0}不是数字,更新失败", string.Join(",", marks.ToArray())));
                    }
                    else
                    {
                        this.TheGenericMgr.Update(checkListDetail);
                        ShowSuccessMessage("更新成功");
                    }
                }
                DetailDataBind(checkListMaster);
            }
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
            this.TheGenericMgr.DeleteById<CheckListDetail>(int.Parse(comId));
            DetailDataBind(checkListMaster);
            ShowSuccessMessage("Common.Business.Result.Delete.Successfully");
        }
        catch (Exception)
        {
            ShowErrorMessage("Common.Business.Result.Delete.Failed");
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        LinkButton lbtnAdd = (LinkButton)e.Row.FindControl("lbtnAdd");
        LinkButton lbtnDelete = (LinkButton)e.Row.FindControl("lbtnDelete");
        CheckBox cbIsRequired = (CheckBox)e.Row.FindControl("tbIsRequired");

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckListDetail checkListDetail = (CheckListDetail)e.Row.DataItem;

            if (checkListDetail.Id == 0)
            {
                lbtnAdd.Text = "${Common.Button.New}";
                lbtnDelete.Visible = false;
                cbIsRequired.Checked = true;
            }
            else
            {
                lbtnAdd.Text = "${Common.Button.Update}"; 
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {

        checkListMaster.Name = this.tbName.Text.Trim();
        checkListMaster.Region = this.tbRegion.Text.Trim();
        if (!string.IsNullOrEmpty(this.tbFacilityID.Text.Trim()))
        {
            FacilityMasters facility = TheGenericMgr.FindById<FacilityMasters>(this.tbFacilityID.Text.Trim());
            checkListMaster.FacilityID = facility.FCID;
            checkListMaster.FacilityName = facility.Name;
        }
        else
        {
            checkListMaster.FacilityName = this.tbFacilityName.Text.Trim();
        }
        checkListMaster.Description = this.tbDescription.Text.Trim();

        checkListMaster.TaskSubType = this.tbTaskSubType.Text.Trim();
        checkListMaster.NeekCreateTask = this.cbNeedCreateTask.Checked;

        checkListMaster.LastModifyDate = DateTime.Now;
        checkListMaster.LastModifyUser = this.CurrentUser.Code;

        string subUser = tbSubUser.Text.Trim();
        if (!string.IsNullOrEmpty(subUser))
        {
            string[] userCodeName = ISIUtil.GetUserSplit(subUser);

            string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
            if (!string.IsNullOrEmpty(invalidUser))
            {
                ShowWarningMessage("ISI.Error.UserNotExist", new string[] { invalidUser });
                return;
            }

            if (userCodeName != null && userCodeName.Length == 2)
            {
                var assignStartUserCode = ISIUtil.GetUser(userCodeName[0]);
                if (checkListMaster.SubUser != assignStartUserCode)
                {
                    checkListMaster.SubUser = assignStartUserCode;
                    //  maintainPlan.AssignStartUserNm = userCodeName[1];
                }
            }

        }
        else
        {
            checkListMaster.SubUser = string.Empty;
        }



        this.TheGenericMgr.Update(checkListMaster);
        ShowSuccessMessage("保存成功");
        InitPageParameter(checkListMaster.Code);
    }


}