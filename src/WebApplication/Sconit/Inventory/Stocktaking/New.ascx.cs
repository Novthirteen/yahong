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
using com.Sconit.Entity;
using com.Sconit.Service.MasterData;
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;

public partial class Inventory_Stocktaking_New : NewModuleBase
{
    public event EventHandler SaveEvent;
    public event EventHandler BackEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.tbEffDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            this.tbStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
        this.tbRegion.ServiceParameter = "string:" + this.CurrentUser.Code;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string region = this.tbRegion.Text.Trim();
        string location = this.tbLocation.Text.Trim();
        string effectiveDate = this.tbEffDate.Text.Trim();
        DateTime effDate = DateTime.Now;
        DateTime startDate = DateTime.Now;
        if (region == string.Empty)
        {
            ShowErrorMessage("Common.Business.Error.RegionInvalid");
            return;
        }
        else if (location == string.Empty)
        {
            ShowErrorMessage("Common.Business.Error.LocationInvalid");
            return;
        }

        try
        {
            effDate = Convert.ToDateTime(effectiveDate);
            startDate = DateTime.Parse(this.tbStartDate.Text.Trim());
        }
        catch (Exception)
        {
            ShowErrorMessage("Common.Business.Error.DateInvalid");
            return;
        }

        CycleCount cycleCount = new CycleCount();
        cycleCount.Location = TheLocationMgr.LoadLocation(location);
        cycleCount.EffectiveDate = effDate;
        cycleCount.StartDate = startDate;
        cycleCount.Type = this.ddlType.SelectedValue;
        cycleCount.IsScanHu = this.ddlPhyCntRule.SelectedValue == BusinessConstants.CODE_MASTER_PHYCNTRULE_HU ? true : false;
        cycleCount.PhyCntGroupBy = this.ddlPhyCntGroupBy.SelectedValue;
        cycleCount.IsPlotRatio = this.ddlPhyCntRule.SelectedValue == BusinessConstants.CODE_MASTER_PHYCNTRULE_CONCENTRATION ? true : false;
        TheCycleCountMgr.CreateCycleCount(cycleCount, this.CurrentUser);
        ShowSuccessMessage("Common.Business.Result.Insert.Successfully");

        if (SaveEvent != null)
        {
            SaveEvent(cycleCount.Code, e);
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    public void UpdateView()
    {
        //this.tbEffDate.Text = string.Empty;
        this.tbRegion.Text = string.Empty;
        this.tbLocation.Text = string.Empty;
        this.ddlType.SelectedIndex = 0;
        this.ddlPhyCntRule.SelectedIndex = 0;
    }

}
