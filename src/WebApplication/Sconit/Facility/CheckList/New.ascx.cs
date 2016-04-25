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

public partial class Facility_CheckList_New : NewModuleBase
{
    public event EventHandler Back;
    public event EventHandler Create;
    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbTaskSubType.ServiceParameter = "string:" + ISIConstants.ISI_TASK_TYPE_ISSUE + ",string:" + this.CurrentUser.Code;
        if (!IsPostBack)
        {

        }
    }

    protected void btnInsert_Click(object sender, EventArgs e)
    {
        var checkListMaster = new CheckListMaster();
        checkListMaster.Code = this.tbCode.Text.Trim();
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
            checkListMaster.FacilityName = this.tbFacilityName.Text;
        }

        checkListMaster.Description = this.tbDescription.Text.Trim();
        checkListMaster.CreateUser = this.CurrentUser.Code;
        checkListMaster.CreateDate = DateTime.Now;
        checkListMaster.LastModifyDate = DateTime.Now;
        checkListMaster.LastModifyUser = this.CurrentUser.Code;
        checkListMaster.TaskSubType = this.tbTaskSubType.Text.Trim();
        checkListMaster.NeekCreateTask = this.cbNeedCreateTask.Checked;

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
        this.TheGenericMgr.Create(checkListMaster);
        ShowSuccessMessage("创建成功");
        Create(checkListMaster.Code, e);
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Back != null)
        {
            Back(this, e);
        }
    }
}