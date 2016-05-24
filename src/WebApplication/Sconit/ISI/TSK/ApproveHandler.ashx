<%@ WebHandler Language="C#" Class="ApproveHandler" %>

using System;
using System.Web;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;
public class ApproveHandler : IHttpHandler
{

    protected com.Sconit.Service.Ext.MasterData.ILanguageMgrE languageMgr { get { return GetService<com.Sconit.Service.Ext.MasterData.ILanguageMgrE>("LanguageMgr.service"); } }
    protected com.Sconit.Service.Ext.MasterData.IUserMgrE userMgrE { get { return GetService<com.Sconit.Service.Ext.MasterData.IUserMgrE>("UserMgr.service"); } }
    protected com.Sconit.ISI.Service.Ext.ITaskMgrE taskMgrE { get { return GetService<com.Sconit.ISI.Service.Ext.ITaskMgrE>("TaskMgr.service"); } }
    //protected com.Sconit.ISI.Service.Ext.IWorkflowMgrE workflowMgrE { get { return GetService<com.Sconit.ISI.Service.Ext.IWorkflowMgrE>("WorkflowMgr.service"); } }
    protected T GetService<T>(string serviceName)
    {
        return com.Sconit.Utility.ServiceLocator.GetService<T>(serviceName);
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        try
        {
            string taskCode = context.Request.Params["TaskCode"];
            string userCode = context.Request.Params["UserCode"];
            string userPwd = context.Request.Params["UserPwd"];
            string type = context.Request.Params["Type"];
            if (!string.IsNullOrEmpty(taskCode)
                        && !string.IsNullOrEmpty(userCode)
                        && !string.IsNullOrEmpty(userPwd)
                        && !string.IsNullOrEmpty(type))
            {
                com.Sconit.Entity.MasterData.User user = userMgrE.CheckAndLoadUser(userCode);
                try
                {
                    if (user != null && user.Password == userPwd)
                    {
                        //批准
                        if (type == "1")
                        {
                            taskMgrE.ProcessByEmail(taskCode, ISIConstants.ISI_LEVEL_APPROVE, "邮件快速批准", user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN), user);
                            context.Response.Write(languageMgr.TranslateMessage("ISI.TSK.Approve.Successfully", user, new string[] { taskCode }));
                        }
                        //退回
                        else if (type == "2")
                        {
                            taskMgrE.ProcessByEmail(taskCode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN, "邮件快速退回", user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN), user);
                            context.Response.Write(languageMgr.TranslateMessage("ISI.TSK.Return.Successfully", user, new string[] { taskCode }));

                        }//争议
                        else if (type == "3")
                        {
                            taskMgrE.ProcessByEmail(taskCode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE, "邮件快速批准（争议）", user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN), user);
                            context.Response.Write(languageMgr.TranslateMessage("ISI.TSK.Dispute.Successfully", user, new string[] { taskCode }));

                        }//不批准
                        else if (type == "4")
                        {
                            taskMgrE.ProcessByEmail(taskCode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE, "邮件快速不批准", user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN), user);
                            context.Response.Write(languageMgr.TranslateMessage("ISI.TSK.Refuse.Successfully", user, new string[] { taskCode }));
                        }
                    }
                    else
                    {
                        context.Response.Write(languageMgr.TranslateMessage("ISI.Warning.IllegalOperation", user) + "</b>");
                    }
                }
                catch (com.Sconit.Entity.Exception.BusinessErrorException e)
                {
                    context.Response.Write(languageMgr.TranslateMessage(e.Message, user, e.MessageParams));
                }
            }
        }
        catch (Exception e)
        {
            context.Response.Write(e.Message);
        }

    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}