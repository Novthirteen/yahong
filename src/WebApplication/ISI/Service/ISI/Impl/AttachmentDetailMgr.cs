using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using NHibernate;
using System.Linq;
using System.IO;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.Exception;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.MasterData;
using System.Web;
using com.Sconit.ISI.Service.Util;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class AttachmentDetailMgr : AttachmentDetailBaseMgr, IAttachmentDetailMgr
    {

        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }

        #region Customized Methods

        [Transaction(TransactionMode.Unspecified)]
        public int GetAttachmentCount(string key)
        {
            return GetAttachmentCount(key, null);
        }

        [Transaction(TransactionMode.Unspecified)]
        public int GetTaskAttachmentCount(string key)
        {
            return GetAttachmentCount(key, typeof(TaskMstr).FullName);
        }

        [Transaction(TransactionMode.Unspecified)]
        public int GetProjectAttachmentCount(string key)
        {
            return GetAttachmentCount(key, typeof(ProjectTask).FullName);
        }

        [Transaction(TransactionMode.Unspecified)]
        public int GetTaskSubTypeAttachmentCount(string key)
        {
            return GetAttachmentCount(key, typeof(TaskSubType).FullName);
        }
        [Transaction(TransactionMode.Unspecified)]
        public int GetMaintainPlanAttachmentCount(string key)
        {
            return GetAttachmentCount(key, "com.Sconit.Facility.Entity.MaintainPlan");
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetAttachment(string taskCode, int firstRow, int maxRows)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(AttachmentDetail));
            criteria.Add(Expression.Eq("TaskCode", taskCode));
            ICriteria c = criteria.GetExecutableCriteria(this.daoBase.GetSession());

            criteria.AddOrder(Order.Desc("CreateDate"));
            c.SetFirstResult(firstRow);
            c.SetMaxResults(maxRows);
            //c.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(AttachmentDetail)));

            return c.List<AttachmentDetail>();
        }

        [Transaction(TransactionMode.Unspecified)]
        public IDictionary<string, IList<object>> GetAttachmentDetail(IList<string> taskCodeList, DateTime monday, DateTime lastMonday, DateTime lastLastMonday)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(AttachmentDetail));
            criteria.Add(Expression.In("TaskCode", taskCodeList.ToArray()));
            ICriteria c = criteria.GetExecutableCriteria(this.daoBase.GetSession());
            criteria.AddOrder(Order.Desc("CreateDate"));
            IList<AttachmentDetail> attachmentDetailList = c.List<AttachmentDetail>();
            IDictionary<string, IList<object>> attachmentDetailDic = new Dictionary<string, IList<object>>();
            if (attachmentDetailList != null && attachmentDetailList.Count > 0)
            {
                foreach (var code in taskCodeList)
                {
                    var thisAttachmentDetailList = attachmentDetailList.Where(t => t.TaskCode == code).ToList();
                    if (thisAttachmentDetailList != null && thisAttachmentDetailList.Count > 0)
                    {
                        int count = thisAttachmentDetailList.Count;

                        var thisMondayAttachmentDetailList = thisAttachmentDetailList.Where(t => t.CreateDate >= monday).ToList();

                        IList<IList<AttachmentDetail>> attachmentDetailListList = new List<IList<AttachmentDetail>>();
                        if (thisMondayAttachmentDetailList != null && thisMondayAttachmentDetailList.Count > 0)
                        {
                            attachmentDetailListList.Add(thisMondayAttachmentDetailList);
                        }

                        int count1 = 0;
                        if (count >= 10)
                        {
                            count1 = 10;
                        }
                        else
                        {
                            count1 = count;
                        }

                        if (attachmentDetailListList.Count == 0)
                        {
                            attachmentDetailListList.Add(thisAttachmentDetailList.Take(count1).ToList());
                            attachmentDetailListList.Add(thisAttachmentDetailList.Take(count1).ToList());
                        }
                        else if (attachmentDetailListList.Count == 1)
                        {
                            attachmentDetailListList.Add(thisAttachmentDetailList.Take(count1).ToList());
                        }

                        IList<object> objectList = new List<object>();
                        objectList.Add(attachmentDetailListList);

                        //������
                        if (thisMondayAttachmentDetailList != null)
                        {
                            objectList.Add(thisMondayAttachmentDetailList.Count);
                        }
                        else
                        {
                            objectList.Add(0);
                        }

                        //����
                        objectList.Add(count);

                        attachmentDetailDic.Add(code, objectList);
                    }
                }
            }
            return attachmentDetailDic;
        }

        [Transaction(TransactionMode.Unspecified)]
        public int GetAttachmentCount(string key, string type)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(AttachmentDetail));
            criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Id")));
            criteria.Add(Expression.Eq("TaskCode", key));
            if (type != null)
            {
                criteria.Add(Expression.Eq("ModuleType", type));
            }
            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0)
            {
                return count[0];
            }
            return 0;
        }


        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetAttachment(string key)
        {
            return GetAttachment(key, null);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetTaskAttachment(string key)
        {
            return GetAttachment(key, typeof(TaskMstr).FullName);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetTaskSubTypeAttachment(string key)
        {
            return GetAttachment(key, typeof(TaskSubType).FullName);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetProjectAttachment(string key)
        {
            return GetAttachment(key, typeof(ProjectTask).FullName);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetFacilityAttachment(string key)
        {
            return GetAttachment(key, "com.Sconit.Facility.Entity.FacilityTrans");
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetResSopAttachment(string key)
        {
            return GetAttachment(key, typeof(ResSop).FullName);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetAttachment(string key, string type)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(AttachmentDetail));
            criteria.Add(Expression.Eq("TaskCode", key));
            if (type != null)
            {
                criteria.Add(Expression.Eq("ModuleType", type));
            }
            criteria.AddOrder(Order.Desc("CreateDate"));
            return criteriaMgrE.FindAll<AttachmentDetail>(criteria, 0, 500);
        }

        //  �����ļ���
        [Transaction(TransactionMode.Unspecified)]
        public void DownLoadFile(int id, HttpRequest request, HttpResponse Response, HttpServerUtility server)
        {
            AttachmentDetail attachment = this.LoadAttachmentDetail(id);
            if (attachment != null)
            {
                string absolutePath = request.MapPath("App_Data/");
                // �����ļ�������·��
                //string Url = "File\\" + FullFileName;
                // �����ļ�������·��
                string fullPath = absolutePath + attachment.Path;// HttpContext.Current.Server.MapPath(Url);
                // ��ʼ��FileInfo���ʵ������Ϊ�ļ�·���İ�װ
                FileInfo fi = new FileInfo(fullPath);
                // �ж��ļ��Ƿ����
                if (fi.Exists)
                {
                    // ���ļ����浽����
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + server.UrlEncode(attachment.FileName));
                    Response.AddHeader("Content-Length", fi.Length.ToString());
                    Response.ContentType = attachment.ContentType;
                    Response.Filter.Close();
                    Response.WriteFile(fi.FullName);
                    Response.End();
                }
            }
        }

        // ɾ���ļ���
        [Transaction(TransactionMode.Requires)]
        public string DeleteFile(int id, HttpRequest request)
        {
            AttachmentDetail attachment = this.LoadAttachmentDetail(id);
            if (attachment == null) return string.Empty;
            string fileName = attachment.FileName;

            string fullFileName = attachment.Path;

            string absolutePath = request.MapPath("App_Data/");

            // �����ļ�������·��
            //string Url = "File\\" + fullFileName;
            // �����ļ�������·��
            string FullPath = absolutePath + fullFileName;// HttpContext.Current.Server.MapPath(Url);
            // ȥ���ļ���ֻ������
            File.SetAttributes(FullPath, FileAttributes.Normal);
            // ��ʼ��FileInfo���ʵ������Ϊ�ļ�·���İ�װ
            FileInfo FI = new FileInfo(FullPath);
            // �ж��ļ��Ƿ����
            if (FI.Exists)
            {
                FI.Delete();
            }
            this.DeleteAttachmentDetail(attachment);

            return fileName;
        }

        [Transaction(TransactionMode.Requires)]
        public void UploadTaskFile(string code, HttpRequest request, User user)
        {
            UploadFile(code, typeof(TaskMstr).FullName, request, false, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void UploadTaskSubTypeFile(string code, HttpRequest request, User user)
        {
            UploadFile(code, typeof(TaskSubType).FullName, request, true, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void UploadProjectFile(string code, HttpRequest request, User user)
        {
            UploadFile(code, typeof(ProjectTask).FullName, request, true, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void UploadFile(string code, string type, HttpRequest request, User user)
        {
            if (type == typeof(TaskMstr).FullName)
            {
                UploadFile(code, type, request, false, user);
            }
            if (type == typeof(ProjectTask).FullName)
            {
                UploadFile(code, type, request, true, user);
            }
            if (type == typeof(TaskSubType).FullName)
            {
                UploadFile(code, type, request, true, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void UploadFile(string code, string type, HttpRequest request, bool isTemplates, User user)
        {
            ///'����File��Ԫ��
            HttpFileCollection files = request.Files;

            for (int iFile = 0; iFile < files.Count; iFile++)
            {
                ///'����ļ���չ����
                HttpPostedFile postedFile = files[iFile];
                string absolutePath = request.MapPath("App_Data/");
                UploadFile(code, type, absolutePath, isTemplates, user, postedFile);
            }
        }
        [Transaction(TransactionMode.Requires)]
        public void UploadFile(string code, string type, string absolutePath, bool isTemplates, User user, HttpPostedFile postedFile)
        {
            string fileName = string.Empty;
            string fileExtension = string.Empty;
            int contentLength = 0;
            fileName = System.IO.Path.GetFileName(postedFile.FileName);
            if (!string.IsNullOrEmpty(fileName))
            {
                //���ڲ�ͬ�����ȡ����FileName��ͬ���е����ļ�����·�����е���ֻ���ļ���������Ҫ���д���
                if (fileName.IndexOf('\\') > -1)
                {
                    fileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                }
                else if (fileName.IndexOf('/') > -1)
                {
                    fileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
                }

                fileExtension = System.IO.Path.GetExtension(fileName);
                //contentLength = postedFile.ContentLength;

                contentLength = int.Parse(this.entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ISI_CONTENTLENGTH).Value);
                if (postedFile.ContentLength > (contentLength * 1024 * 1024))
                {
                    throw new BusinessErrorException("ISI.Attachment.FilesCantBeMoreThan", new string[] { contentLength.ToString() });
                }

                string[] fileExtensions = this.entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ISI_FILEEXTENSION).Value.ToLower().Split(ISIConstants.ISI_FILE_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                if (!fileExtensions.Contains(fileExtension.ToLower().Substring(1)))
                {
                    throw new BusinessErrorException("ISI.Attachment.NotAllowedToUpload", new string[] { fileExtension.Substring(1) });
                }

                //strMsg.Append("�ϴ����ļ����ͣ�" + postedFile.ContentType.ToString() + "<br>");
                //strMsg.Append("�ͻ����ļ���ַ��" + postedFile.FileName + "<br>");
                //strMsg.Append("�ϴ��ļ����ļ�����" + fileName + "<br>");
                //strMsg.Append("�ϴ��ļ�����չ����" + fileExtension + "<br><hr>");
                ///'�ɸ�����չ���ֵĲ�ͬ���浽��ͬ���ļ���
                ///ע�⣺����Ҫ�޸�����ļ��е�����д��Ȩ�ޡ�

                DateTime now = DateTime.Now;
                AttachmentDetail attachment = new AttachmentDetail();
                attachment.TaskCode = code;
                attachment.Size = decimal.Parse((postedFile.ContentLength / 1024.0).ToString());
                attachment.CreateUser = user.Code;
                attachment.CreateUserNm = user.Name;
                attachment.CreateDate = now;
                attachment.FileName = fileName;
                attachment.FileExtension = fileExtension;
                attachment.ModuleType = type;
                attachment.ContentType = postedFile.ContentType;
                string alias = ISIUtil.GetAlias(type, code, now, fileExtension);

                int y = now.Year;
                int m = now.Month;
                int d = now.Day;
                string path = ISIUtil.GetPath(now, isTemplates);

                if (!Directory.Exists(absolutePath + path))//�ж��Ƿ����
                {
                    Directory.CreateDirectory(absolutePath + path);//������·��
                }

                attachment.Path = path + alias;

                postedFile.SaveAs(absolutePath + attachment.Path);

                this.CreateAttachmentDetail(attachment);
            }
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class AttachmentDetailMgrE : com.Sconit.ISI.Service.Impl.AttachmentDetailMgr, IAttachmentDetailMgrE
    {
    }
}

#endregion Extend Class