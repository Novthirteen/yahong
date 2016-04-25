using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Facility.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Entity.Exception;

public partial class Facility_CheckListOrder_New : NewModuleBase
{
    public event EventHandler Back;
    public event EventHandler Create;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.tbCheckListCode.DataSource = this.TheGenericMgr.FindAll<CheckListMaster>();
            this.tbCheckListCode.DataBind();
        }
    }

    public void PageCleanup()
    {
        //this.tbCheckListCode.Text = string.Empty;
        this.lblCheckListDesc.Text = string.Empty;
        this.tbCheckUser.Text = this.CurrentUser.Code;
        this.tbCheckDate.Text = string.Empty;
        this.lblCheckListDesc.Text = string.Empty;
        this.lblDescription.Text = string.Empty;
        this.tbRemark.Text = string.Empty;

        this.divDetail.Visible = false;
    }

    protected void btnInsert_Click(object sender, EventArgs e)
    {
        var checkListOrder = new CheckListOrderMaster();
        checkListOrder.CheckListCode = this.tbCheckListCode.SelectedValue;
        checkListOrder.CheckDate = Convert.ToDateTime(this.tbCheckDate.Text.Trim());
        checkListOrder.CheckUser = this.tbCheckUser.Text.Trim();

        checkListOrder.CreateUser = this.CurrentUser.Code;
        checkListOrder.CreateDate = DateTime.Now;
        checkListOrder.LastModifyDate = DateTime.Now;
        checkListOrder.LastModifyUser = this.CurrentUser.Code;
        checkListOrder.Code = TheNumberControlMgr.GenerateNumber("CLT");
        checkListOrder.Remark = this.tbRemark.Text.Trim();
        checkListOrder.CheckListOrderDetailList = GetCheckListOrderDetailList(checkListOrder.CheckListCode);

        #region 判断明细有不正常的需要填写备注
        foreach (CheckListOrderDetail d in checkListOrder.CheckListOrderDetailList)
        {

            if (!d.IsNormal && string.IsNullOrEmpty(d.Remark))
            {
                ShowErrorMessage(string.Format("序号{0}的项目巡检结果异常必须填写结果", d.Seq.ToString()));
                return;
            }
            //判断必填
            if (d.IsRequired && string.IsNullOrEmpty(d.Remark))
            {
                ShowErrorMessage(string.Format("序号{0}的项目巡检结果为必填项", d.Seq.ToString()));
                return;
            }
            //判定是否在正常值范围内
            if (d.MaxValue == 0 && d.MinValue == 0)
            {
                //nothing todo
            }
            else if (!string.IsNullOrEmpty(d.Remark))
            {
                double thisValue = 0;
                if (!double.TryParse(d.Remark, out thisValue))
                {
                    ShowErrorMessage(string.Format("序号{0}的结果不是数字类型", d.Seq.ToString()));
                    return;
                }
                if (thisValue <= d.MaxValue && thisValue >= d.MinValue)
                {
                    d.IsNormal = true;
                }
                else
                {
                    d.IsNormal = false;
                }
            }
        }
        #endregion

        this.TheCheckListMgr.CreateCheckListOrder(checkListOrder);
        ShowSuccessMessage("创建成功");
        Create(checkListOrder.Code, e);
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Back != null)
        {
            Back(this, e);
        }
    }

    protected void tbCheckList_TextChanged(Object sender, EventArgs e)
    {
        try
        {

            if (!string.IsNullOrEmpty(this.tbCheckListCode.Text))
            {
                string checkListCode = this.tbCheckListCode.Text.Trim();

                CheckListMaster checkListOrder = TheCheckListMgr.GetCheckListMaster(checkListCode);
                DetailDataBind(checkListOrder);
                this.lblCheckListDesc.Text = checkListOrder.Name;
                this.lblDescription.Text = checkListOrder.Description;

            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private void DetailDataBind(CheckListMaster checkListMaster)
    {
        IList<CheckListOrderDetail> checkListOrderDetails = new List<CheckListOrderDetail>();
        foreach (CheckListDetail d in checkListMaster.CheckListDetailList)
        {
            CheckListOrderDetail cd = new CheckListOrderDetail();
            cd.CheckListCode = d.CheckListCode;
            cd.CheckListDetailCode = d.Code;
            cd.Description = d.Description;
            cd.Seq = d.Seq;
            cd.IsNormal = true;
            cd.IsRequired = d.IsRequired;
            checkListOrderDetails.Add(cd);
        }

        this.GV_List.DataSource = checkListOrderDetails;
        this.GV_List.DataBind();
        this.divDetail.Visible = true;
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckListOrderDetail d = (CheckListOrderDetail)e.Row.DataItem;
            Label lblIsRequired = (Label)e.Row.FindControl("lblIsRequired");
            if(d.IsRequired)
            {
                lblIsRequired.Text = "必填";
                lblIsRequired.ForeColor = System.Drawing.Color.Red;
                TextBox tbRemark = (TextBox)e.Row.FindControl("tbRemark");
                tbRemark.Attributes.Add("class", "inputRequired");
            }

        }
    }

    private List<CheckListOrderDetail> GetCheckListOrderDetailList(string checkListCode)
    {
        List<CheckListOrderDetail> checkListOrderDetailList = new List<CheckListOrderDetail>();

        CheckListMaster checkListOrder = TheCheckListMgr.GetCheckListMaster(checkListCode);
        for (int i = 0; i < this.GV_List.Rows.Count; i++)
        {
            var row = this.GV_List.Rows[i];

            RadioButton rbNormal = (RadioButton)row.FindControl("rbNormal");
            TextBox tbRemark = (TextBox)row.FindControl("tbRemark");
            var checkListDetail = checkListOrder.CheckListDetailList[i];


            CheckListOrderDetail cd = new CheckListOrderDetail();
            cd.CheckListCode = checkListDetail.CheckListCode;
            cd.CheckListDetailCode = checkListDetail.Code;
            cd.Description = checkListDetail.Description;
            cd.Seq = checkListDetail.Seq;
            cd.IsNormal = rbNormal.Checked;
            cd.Remark = tbRemark.Text.Trim();
            cd.IsRequired = checkListDetail.IsRequired;
            cd.MaxValue = checkListDetail.MaxValue;
            cd.MinValue = checkListDetail.MinValue;
            checkListOrderDetailList.Add(cd);


        }
        return checkListOrderDetailList;

    }

    
}