using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;

//TODO: Add other using statements here.by liqiuyun
public partial class Modules_ISI_ResWokShop_Edit : EditModuleBase
{
    public event EventHandler Back;
	
	//Get the logger
	private static ILog log = LogManager.GetLogger("ISI");

    protected void Page_Load(object sender, EventArgs e)
    {
		//TODO: Add code for Page_Load here.
		if (!IsPostBack)
        {

        }
    }
	
	public void InitPageParameter(object Code)
    {
        this.ODS_ResWokShop.SelectParameters["Code"].DefaultValue = Code.ToString();
        this.ODS_ResWokShop.DeleteParameters["Code"].DefaultValue = Code.ToString();
        this.FV_ResWokShop.DataBind();
    }

	protected void FV_ResWokShop_DataBound(object sender, EventArgs e)
    {
         ResWokShop dataItem = (ResWokShop)this.FV_ResWokShop.DataItem;
        if (dataItem != null)
        {
            Controls_TextBox tbParentCode = (Controls_TextBox)this.FV_ResWokShop.FindControl("tbParentCode");
            tbParentCode.Text = dataItem.ParentCode;
		//  tbPartyTo.Text = dataItem.PartyTo;
		//  tbPartyTo.ServiceParameter = "string:" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION + ",string:" + this.CurrentUser.Code;
        //	tbPartyTo.DataBind();
        }
    }

    protected void ODS_ResWokShop_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        ResWokShop dataItem = (ResWokShop)e.InputParameters[0];
        Controls_TextBox tbParentCode = (Controls_TextBox)this.FV_ResWokShop.FindControl("tbParentCode");
        dataItem.ParentCode = tbParentCode.Text.Trim();
        dataItem.LastModifyDate = DateTime.Now;
        dataItem.LastModifyUser = this.CurrentUser.Code;
        //Controls_TextBox tbPartyFrom = (Controls_TextBox)this.FV_ResWokShop.FindControl("tbPartyFrom");
    }

    protected void ODS_ResWokShop_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
		if (e.Exception != null)
        {
            ShowErrorMessage("Common.Business.Result.Update.Failed");
            e.ExceptionHandled = true;
        }
        else
        {
			Back(sender, e);
			ShowSuccessMessage("Common.Business.Result.Update.Successfully");
		}
    }

    protected void ODS_ResWokShop_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
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
}