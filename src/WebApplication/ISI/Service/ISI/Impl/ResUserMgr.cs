using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Service.Ext;
using NHibernate.Expression;
using System.Linq;
using com.Sconit.Service.Ext.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class ResUserMgr : ResUserBaseMgr, IResUserMgr
    {
        public IResSopMgrE resSopMgr { get; set; }
        public ICriteriaMgrE criteriaMgr { get; set; }
        public IAttachmentDetailMgrE attachmentDetailMgr { get; set; }
        public IResRoleMgrE resRoleMgr { get; set; }
        public IResWokShopMgrE resWokShopMgr { get; set; }
        public IResMatrixLogMgrE resMatrixLogMgr { get; set; }
        public IUserMgrE userMgr { get; set; }

        #region Customized Methods
        /*
         todo 历史查询
         SELECT O.* FROM (select distinct(ResMatrixId) from ISI_ResMatrixLog where [Action]<>'Delete' and CreateDate<='2013-6-6'
            and (Director like '%,liqiuyun,%' or Director like 'liqiuyun,%' or Director like '%,liqiuyun')
            ) AS E
            CROSS APPLY (
                SELECT TOP(1)* FROM ISI_ResMatrixLog AS O1 WHERE E.ResMatrixId = O1.ResMatrixId ORDER BY O1.CreateDate DESC,O1.Id DESC
        ) AS O
         */

        [Transaction(TransactionMode.Requires)]
        public override void CreateResUser(ISI.Entity.ResUser entity)
        {
            CreateResMatrixLog(entity, "New");
            base.CreateResUser(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteResUser(ISI.Entity.ResUser entity)
        {
            CreateResMatrixLog(entity, "Delete");
            base.DeleteResUser(entity.Id);
        }

        public override void DeleteResUser(IList<int> pkList)
        {
            throw new NotImplementedException();
        }

        public override void DeleteResUser(IList<ResUser> entityList)
        {
            throw new NotImplementedException();
        }

        public override void DeleteResUser(int id)
        {
            throw new NotImplementedException();
        }

        [Transaction(TransactionMode.Requires)]
        public override void UpdateResUser(ResUser entity)
        {
            CreateResMatrixLog(entity, "Update");
            base.UpdateResUser(entity);
        }

        [Transaction(TransactionMode.Requires)]
        private void CreateResMatrixLog(ResUser resUser, string action)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ResMatrix));
            criteria.Add(Expression.Eq("Id", resUser.MatrixId));
            var resMatrix = criteriaMgr.FindAll<ResMatrix>(criteria).First();

            var workShop = resWokShopMgr.LoadResWokShop(resMatrix.WorkShop);
            var resRole = resRoleMgr.LoadResRole(resMatrix.Role);
            var attachIds = string.Empty;
            var resSop = new ResSop();
            if (resMatrix.Operate.HasValue)
            {
                resSop = resSopMgr.LoadResSop(resMatrix.WorkShop, resMatrix.Operate.Value);
                if (resSop == null)
                {
                    throw new Exception(string.Format("没有找到作业区{0},工位{1}对应的工艺流程", resMatrix.WorkShop, resMatrix.Operate));
                }
                var attachMentList = this.attachmentDetailMgr.GetResSopAttachment(resSop.Id.ToString());
                if (attachMentList != null)
                {
                    foreach (var attachMent in attachMentList)
                    {
                        if (attachIds == string.Empty)
                        {
                            attachIds = attachMent.Id.ToString();
                        }
                        else
                        {
                            attachIds += ("," + attachMent.Id.ToString());
                        }
                    }
                }
            }
            var resMatrixLog = new ResMatrixLog();
            resMatrixLog.Action = action;
            resMatrixLog.AttachmentIds = attachIds;
            resMatrixLog.CreateDate = resUser.LastModifyDate;
            resMatrixLog.CreateUser = resUser.LastModifyUser;
            resMatrixLog.Operate = resSop.Operate;
            resMatrixLog.OperateDesc = resSop.OperateDesc;
            resMatrixLog.Responsibility = resMatrix.Responsibility;
            resMatrixLog.Priority = resUser.Priority;
            resMatrixLog.Role = resMatrix.Role;
            resMatrixLog.RoleName = resRole.Name;
            resMatrixLog.RoleType = resRole.RoleType;
            resMatrixLog.UserCode = resUser.UserCode;
            resMatrixLog.UserName = resUser.UserName;
            resMatrixLog.SkillLevel = resUser.SkillLevel;
            resMatrixLog.WorkShop = resMatrix.WorkShop;
            resMatrixLog.WorkShopName = workShop.Name;
            resMatrixLog.ResMatrixId = resMatrix.Id;
            resMatrixLog.StartDate = resUser.StartDate;
            resMatrixLog.EndDate = resUser.EndDate;
            resMatrixLog.Seq = resMatrix.Seq;
            resMatrixLog.TaskSubType = resMatrix.TaskSubType;

            if (action == "Update")
            {
                //var oldResUser = criteriaMgr.this.LoadResUser(resUser.Id);
                resMatrixLog.Logs = string.Empty;
                if (resUser.OldStartDate != resUser.StartDate)
                {
                    resMatrixLog.Logs += string.Format("开始时间由{0}变更为{1}", resUser.OldStartDate.ToString("yyyy-MM-dd HH:mm"), resUser.StartDate.ToString("yyyy-MM-dd HH:mm"));
                }
                if (resUser.OldEndDate != resUser.EndDate)
                {
                    resMatrixLog.Logs += string.Format("结束时间由{0}变更为{1}", resUser.OldEndDate.ToString("yyyy-MM-dd HH:mm"), resUser.EndDate.ToString("yyyy-MM-dd HH:mm"));
                }
                if (resUser.OldPriority != resUser.Priority)
                {
                    resMatrixLog.Logs += string.Format("优先级由{0}变更为{1}", resUser.OldPriority, resUser.Priority);
                }
            }

            resMatrixLogMgr.CreateResMatrixLog(resMatrixLog);
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateEndDate(string userCode, DateTime endDate, string modifyUserCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ResUser));
            criteria.Add(Expression.Eq("UserCode", userCode));
            var List = criteriaMgr.FindAll<ResUser>(criteria);
            foreach (var resUser in List)
            {
                resUser.OldEndDate = resUser.EndDate;
                resUser.EndDate = endDate;
                resUser.LastModifyDate = DateTime.Now;
                resUser.LastModifyUser = modifyUserCode;
                this.UpdateResUser(resUser);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CloneUser(string oldUserCode, string newUserCode, string createUserCode)
        {
            var newUser = userMgr.LoadUser(newUserCode, false, false);
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ResUser));
            criteria.Add(Expression.Eq("UserCode", oldUserCode));
            var List = criteriaMgr.FindAll<ResUser>(criteria);
            foreach (var resUser in List)
            {
                var newResUser = new ResUser();
                newResUser.CreateDate = DateTime.Now;
                newResUser.CreateUser = createUserCode;
                newResUser.EndDate = resUser.EndDate;
                newResUser.LastModifyDate = DateTime.Now;
                newResUser.LastModifyUser = createUserCode;
                newResUser.MatrixId = resUser.MatrixId;
                newResUser.StartDate = resUser.StartDate;
                newResUser.UserCode = newUserCode;
                newResUser.UserName = newUser.Name;
                newResUser.Priority = resUser.Priority;
                newResUser.SkillLevel = resUser.SkillLevel;
                newResUser.NeedPatrol = resUser.NeedPatrol;
                this.CreateResUser(newResUser);
            }
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class ResUserMgrE : com.Sconit.ISI.Service.Impl.ResUserMgr, IResUserMgrE
    {
    }
}

#endregion Extend Class