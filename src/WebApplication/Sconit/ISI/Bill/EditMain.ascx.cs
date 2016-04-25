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
using NHibernate.Expression;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity;

public partial class ISI_Bill_EditMain : MainModuleBase
{
    public event EventHandler BackEvent;
    public string PSIType
    {
        get
        {
            return (string)ViewState["PSIType"];
        }
        set
        {
            ViewState["PSIType"] = value;
        }
    }
    protected string MouldCode
    {
        get
        {
            return (string)ViewState["MouldCode"];
        }
        set
        {
            ViewState["MouldCode"] = value;
        }
    }

    public void InitPageParameter(string code)
    {
        this.MouldCode = code;
        this.ucEdit.InitPageParameter(MouldCode);
        this.ucAttachment.InitPageParameter(MouldCode, typeof(Mould).FullName);
        this.ucTabNavigator.UpdateView(MouldCode);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucTabNavigator.lbMouldClickEvent += new System.EventHandler(this.TabMouldClick_Render);
        this.ucTabNavigator.lbMouldAttachmentClickEvent += new System.EventHandler(this.TabMouldAttachmentClick_Render);
        this.ucAttachment.UpdateAttacmentTitleEvent += new System.EventHandler(this.UpdateAttacmentTitle_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucEdit.DetailNewEvent += new System.EventHandler(this.DetailNew_Render);
        this.ucEdit.DetailEditEvent += new System.EventHandler(this.DetailEdit_Render);
        this.ucDetailNew.BackEvent += new System.EventHandler(this.TabMouldClick_Render);
        this.ucDetailNew.CreateEvent += new System.EventHandler(this.DetailEdit_Render);
        this.ucDetailEdit.BackEvent += new System.EventHandler(this.TabMouldClick_Render);
        this.ucAttachment.BackEvent += new System.EventHandler(this.EditBack_Render);

        if (!IsPostBack)
        {
            this.ucTabNavigator.PSIType = this.PSIType;
            this.ucDetailNew.PSIType = this.PSIType;
            this.ucDetailEdit.PSIType = this.PSIType;
            this.ucEdit.PSIType = this.PSIType;
        }
    }

    protected void DetailEdit_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetailNew.Visible = false;
        this.ucDetailEdit.Visible = true;
        this.ucAttachment.Visible = false;
        this.ucDetailEdit.InitPageParameter(int.Parse(sender.ToString()), this.MouldCode);
    }

    protected void DetailNew_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetailNew.Visible = true;
        this.ucDetailEdit.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucDetailNew.InitPageParameter(sender.ToString(), this.MouldCode);
    }

    protected void TabMouldClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucDetailNew.Visible = false;
        this.ucDetailEdit.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucEdit.UpdateView();
    }


    protected void TabMouldAttachmentClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetailNew.Visible = false;
        this.ucDetailEdit.Visible = false;
        this.ucAttachment.Visible = true;
        this.ucAttachment.InitPageParameter(this.MouldCode, typeof(Mould).FullName);
    }

    protected void UpdateAttacmentTitle_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateAttachmentTitle(this.MouldCode);
    }

    protected void EditBack_Render(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

}
