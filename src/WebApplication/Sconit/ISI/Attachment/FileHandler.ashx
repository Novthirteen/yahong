<%@ WebHandler Language="C#" Class="FileHandler" %>

using System;
using System.Web;
using com.Sconit.ISI.Entity;

public class FileHandler : IHttpHandler
{
    protected com.Sconit.ISI.Service.Ext.IAttachmentDetailMgrE TheAttachmentDetailMgr { get { return GetService<com.Sconit.ISI.Service.Ext.IAttachmentDetailMgrE>("AttachmentDetailMgr.service"); } }

    protected T GetService<T>(string serviceName)
    {
        return com.Sconit.Utility.ServiceLocator.GetService<T>(serviceName);
    }

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            context.Response.ContentType = "text/plain";

            HttpPostedFile postedFile = context.Request.Files["Filedata"];

            if (postedFile != null)
            {
                string fileName = postedFile.FileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    string taskCode = context.Request.Params["TaskCode"];
                    string createUser = context.Request.Params["UserCode"];
                    string createUserNm = context.Request.Params["UserName"];
                    string moduleType = context.Request.Params["ModuleType"];
                    if (string.IsNullOrEmpty(moduleType))
                    {
                        moduleType = typeof(TaskMstr).FullName;
                    }
                    string absolutePath = System.Web.HttpContext.Current.Request.MapPath("../../App_Data/");

                    this.TheAttachmentDetailMgr.UploadFile(taskCode, moduleType, absolutePath, false, new com.Sconit.Entity.MasterData.User { Code = createUser, FirstName = createUserNm }, postedFile);
                }

                //下面这句代码缺少的话，上传成功后上传队列的显示不会自动消失
                context.Response.Write("1");
                //context.Response.Write("OK");
            }
        }
        catch
        {
            context.Response.Write("0");
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