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
                    //由于不同浏览器取出的FileName不同（有的是文件绝对路径，有的是只有文件名），故要进行处理
                    if (fileName.IndexOf('\\') > -1)
                    {
                        fileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                    }
                    else if (fileName.IndexOf('/') > -1)
                    {
                        fileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
                    }

                    string taskCode = context.Request.Params["TaskCode"];
                    string createUser = context.Request.Params["UserCode"];
                    string createUserNm = context.Request.Params["UserName"];
                    string moduleType = context.Request.Params["ModuleType"];
                    if (string.IsNullOrEmpty(moduleType))
                    {
                        moduleType = com.Sconit.ISI.Service.Util.ISIUtil.GetModuleType(taskCode);
                    }
                    string fileExtension = string.Empty;


                    fileExtension = System.IO.Path.GetExtension(fileName);

                    DateTime now = DateTime.Now;
                    AttachmentDetail attachment = new AttachmentDetail();
                    attachment.TaskCode = taskCode;
                    attachment.Size = decimal.Parse((postedFile.ContentLength / 1024.0).ToString());
                    attachment.CreateUser = createUser;
                    attachment.CreateUserNm = createUserNm;
                    attachment.CreateDate = now;
                    attachment.FileName = fileName;
                    attachment.FileExtension = fileExtension;
                    attachment.ContentType = postedFile.ContentType;
                    attachment.ModuleType = typeof(TaskMstr).FullName;
                    string alias = taskCode + now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid() + fileExtension;

                    string absolutePath = System.Web.HttpContext.Current.Request.MapPath("../../App_Data/");
                    string path = moduleType + "/" + taskCode + "/";
                    if (!System.IO.Directory.Exists(absolutePath + path))//判断是否存在
                    {
                        System.IO.Directory.CreateDirectory(absolutePath + path);//创建新路径
                    }
                    attachment.Path = path + alias;

                    postedFile.SaveAs(absolutePath + attachment.Path);

                    this.TheAttachmentDetailMgr.CreateAttachmentDetail(attachment);

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