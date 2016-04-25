using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using System.Web;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IAttachmentDetailMgr : IAttachmentDetailBaseMgr
    {
        #region Customized Methods

        int GetAttachmentCount(string key);
        int GetAttachmentCount(string key, string type);
        int GetTaskAttachmentCount(string key);
        int GetMaintainPlanAttachmentCount(string key);
        int GetProjectAttachmentCount(string key);

        int GetTaskSubTypeAttachmentCount(string key);
        void UploadFile(string code, string type, HttpRequest request, User user);
        void UploadTaskFile(string code, HttpRequest request, User user);
        void UploadTaskSubTypeFile(string code, HttpRequest request, User user);
        void UploadProjectFile(string code, HttpRequest request, User user);
        void UploadFile(string code, string type, HttpRequest request, bool isTemplates, User user);

        void UploadFile(string code, string type, string absolutePath, bool isTemplates, User user, HttpPostedFile postedFile);

        string DeleteFile(int id, HttpRequest request);
        void DownLoadFile(int id, HttpRequest request, HttpResponse Response, HttpServerUtility server);

        IList<AttachmentDetail> GetAttachment(string key);
        IList<AttachmentDetail> GetAttachment(string key, string type);

        IList<AttachmentDetail> GetTaskAttachment(string key);

        IList<AttachmentDetail> GetProjectAttachment(string key);

        IList<AttachmentDetail> GetResSopAttachment(string key);

        IList<AttachmentDetail> GetAttachment(string taskCode, int firstRow, int maxRows);

        IList<AttachmentDetail> GetTaskSubTypeAttachment(string key);

        IDictionary<string, IList<object>> GetAttachmentDetail(IList<string> taskCodeList, DateTime monday, DateTime lastMonday, DateTime lastLastMonday);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IAttachmentDetailMgrE : com.Sconit.ISI.Service.IAttachmentDetailMgr
    {
    }
}

#endregion Extend Interface