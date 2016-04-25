using com.Sconit.Entity.Exception;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service;
using com.Sconit.ISI.Entity;
public partial class ISI_TSK_Approve : System.Web.UI.Page
{
    private ITaskMgrE TheTaskMgr
    {
        get { return ServiceLocator.GetService<ITaskMgrE>("TaskMgr.service"); }
    }

    private ILanguageMgrE TheLanguageMgr
    {
        get { return ServiceLocator.GetService<ILanguageMgrE>("LanguageMgr.service"); }
    }
    private IUserMgrE TheUserMgr
    {
        get { return ServiceLocator.GetService<IUserMgrE>("UserMgr.service"); }
    }
    private IGenericMgr TheGenericMgr
    {
        get { return ServiceLocator.GetService<IGenericMgr>("GenericMgr.service"); }
    }
    public string Type
    {
        get
        {
            return (string)ViewState["Type"];
        }
        set
        {
            ViewState["Type"] = value;
        }
    }
    public string TaskCode
    {
        get
        {
            return (string)ViewState["TaskCode"];
        }
        set
        {
            ViewState["TaskCode"] = value;
        }
    }
    public string UserCode
    {
        get
        {
            return (string)ViewState["UserCode"];
        }
        set
        {
            ViewState["UserCode"] = value;
        }
    }
    public string GUID
    {
        get
        {
            return (string)ViewState["GUID"];
        }
        set
        {
            ViewState["GUID"] = value;
        }
    }
    public User ApproveUser
    {
        get
        {
            return ViewState["ApproveUser"] != null ? (User)ViewState["ApproveUser"] : null;
        }
        set
        {
            ViewState["ApproveUser"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
             GUID = Request.Params["GUID"];
             if (string.IsNullOrEmpty(GUID))
            {
                Response.Write(TheLanguageMgr.TranslateMessage("ISI.Warning.IllegalOperation", "su"));
                return;
            }
             var fastTrack = this.TheGenericMgr.FindById<FastTrack>(GUID);
             if (fastTrack == null)
             {
                 Response.Write(TheLanguageMgr.TranslateMessage("ISI.Warning.IllegalOperation", "su"));
                 return;
             }
             Type = Request.Params["Type"];
             TaskCode = fastTrack.PK;
             UserCode = fastTrack.UserCode;

            ApproveUser = null;
            btnApprove.Visible = false;
            if (!string.IsNullOrEmpty(TaskCode)
                       && !string.IsNullOrEmpty(UserCode)
                       && !string.IsNullOrEmpty(Type))
            {
                ApproveUser = TheUserMgr.CheckAndLoadUser(UserCode);
                if (ApproveUser != null)
                {
                    btnApprove.Visible = true;
                    if (Type == "1")
                    {
                        btnApprove.Text = TheLanguageMgr.TranslateMessage("ISI.TSK.Button.Approve", ApproveUser);
                    }
                    //退回
                    else if (Type == "2")
                    {
                        btnApprove.Text = TheLanguageMgr.TranslateMessage("ISI.TSK.Button.Return", ApproveUser);
                    }//争议
                    else if (Type == "3")
                    {
                        btnApprove.Text = TheLanguageMgr.TranslateMessage("ISI.TSK.Button.Dispute", ApproveUser);
                    }//不批准
                    else if (Type == "4")
                    {
                        btnApprove.Text = TheLanguageMgr.TranslateMessage("ISI.TSK.Button.Refuse", ApproveUser);
                    }
                    else
                    {
                        btnApprove.Visible = false;
                    }
                    return;
                }
            }

            this.Response.Redirect("about:blank");
        }
        catch (BusinessErrorException ex)
        {
            this.Response.Redirect("about:blank");
        }
    }


    protected void btnApprove_Click(object sender, EventArgs e)
    {
        if (ApproveUser != null && !string.IsNullOrEmpty(Type))
        {
            try
            {
                string msg = tbApprove.Text.Trim();
               
                //批准
                if (Type == "1")
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "邮件快速批准";
                    }
                    TheTaskMgr.ProcessByEmail(TaskCode, ISIConstants.ISI_LEVEL_APPROVE, tbApprove.Text.Trim(), ApproveUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN), ApproveUser);
                    Response.Write(TheLanguageMgr.TranslateMessage("ISI.TSK.Approve.Successfully", ApproveUser, new string[] { TaskCode }));
                }
                //退回
                else if (Type == "2")
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "邮件快速退回";
                    }
                    TheTaskMgr.ProcessByEmail(TaskCode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN, tbApprove.Text.Trim(), ApproveUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN), ApproveUser);
                    Response.Write(TheLanguageMgr.TranslateMessage("ISI.TSK.Return.Successfully", ApproveUser, new string[] { TaskCode }));

                }//争议
                else if (Type == "3")
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "邮件快速批准（争议）";
                    }
                    TheTaskMgr.ProcessByEmail(TaskCode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE, tbApprove.Text.Trim(), ApproveUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN), ApproveUser);
                    Response.Write(TheLanguageMgr.TranslateMessage("ISI.TSK.Dispute.Successfully", ApproveUser, new string[] { TaskCode }));

                }//不批准
                else if (Type == "4")
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "邮件快速不批准";
                    }
                    TheTaskMgr.ProcessByEmail(TaskCode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE, tbApprove.Text.Trim(), ApproveUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN), ApproveUser);
                    Response.Write(TheLanguageMgr.TranslateMessage("ISI.TSK.Refuse.Successfully", ApproveUser, new string[] { TaskCode }));
                }
                this.tbApprove.Visible = false;
                this.btnApprove.Visible = false;
            }
            catch (com.Sconit.Entity.Exception.BusinessErrorException ex)
            {
                Response.Write(ex.Message);
            }
            catch (Exception ee)
            {
                Response.Write(ee.Message);
            }
        }
    }
}