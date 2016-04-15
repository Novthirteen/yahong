using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;

public partial class MRP_ShiftPlan_Import_Import : ModuleBase
{
    public event EventHandler ImportEvent;
    public event EventHandler BtnBackClick;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.tbDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.tbSettleTime.Text = DateTime.Today.ToString("yyyy-MM-dd HH:mm");
            this.ucShift.Date = DateTime.Today;
        }
        this.tbRegion.ServiceParameter = "string:" + this.CurrentUser.Code;
    }

    public void Create(object sender)
    {
        try
        {
            IList<OrderHead> orderHeadList = (IList<OrderHead>)sender;
            if (orderHeadList != null && orderHeadList.Count > 0)
            {
                //string shiftCode = this.ucShift.ShiftCode;
                //Shift shift = TheShiftMgr.LoadShift(shiftCode);
                //foreach (var item in orderHeadList)
                //{
                //    item.Shift = shift;
                //}
                orderHeadList = orderHeadList.Where(o => o.OrderDetails != null && o.OrderDetails.Count > 0).ToList();
                foreach (OrderHead oh in orderHeadList)
                {
                    oh.OrderDetails = oh.OrderDetails.OrderBy(o => o.Sequence).ThenBy(o => o.Item.Code).ToList();
                    if (this.tbSettleTime.Text.Trim() != string.Empty)
                    {
                        try
                        {
                            oh.SettleTime = DateTime.Parse(this.tbSettleTime.Text.Trim());
                        }
                        catch (Exception)
                        {
                            ShowErrorMessage("Import.SettleTime.Error.Empty");
                            return;
                        }
                    }
                }
                TheOrderMgr.CreateOrder(orderHeadList, this.CurrentUser.Code);
                ShowSuccessMessage("Common.Business.Result.Insert.Successfully");
            }
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage(ex);
        }
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        this.Import();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BtnBackClick != null)
        {
            BtnBackClick(null, null);
        }
    }

    protected void tbRegion_TextChanged(object sender, EventArgs e)
    {
        this.BindShift();
    }

    protected void tbDate_TextChanged(object sender, EventArgs e)
    {
        this.BindShift();
    }

    private void BindShift()
    {
        this.ucShift.BindList(DateTime.Parse(this.tbDate.Text), this.tbRegion.Text.Trim(), true);
    }

    private void Import()
    {
        try
        {
            string region = this.tbRegion.Text.Trim() != string.Empty ? this.tbRegion.Text.Trim() : string.Empty;
            string flowCode = this.tbFlow.Text.Trim() != string.Empty ? this.tbFlow.Text.Trim() : string.Empty;
            DateTime date = DateTime.Parse(this.tbDate.Text);
            string shiftCode = this.ucShift.ShiftCode;

            IList<ShiftPlanSchedule> spsList = TheImportMgr.ReadPSModelFromXls(fileUpload.PostedFile.InputStream, this.CurrentUser, region, flowCode, date, shiftCode);
            //TheShiftPlanScheduleMgr.SaveShiftPlanSchedule(spsList, this.CurrentUser);
            IList<OrderHead> ohList = TheOrderMgr.ConvertShiftPlanScheduleToOrders(spsList, this.cbCreateOpt.Checked);
            if (ImportEvent != null)
            {
                ImportEvent(new object[] { ohList }, null);
            }
            ShowSuccessMessage("Import.Result.Successfully");
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage(ex);
        }
    }
}
