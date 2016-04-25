using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using com.Sconit.Facility.Entity;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Service;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class CheckListMgr : ICheckListMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public ITaskMgrE taskMgr { get; set; }
        public ITaskSubTypeMgrE taskSubTypeMgr { get; set; }
        public IUserSubscriptionMgrE userSubscriptionMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public void CreateCheckListOrder(CheckListOrderMaster checkListOrder)
        {
            #region 保存头
            CheckListMaster checkListMaster = GetCheckListMaster(checkListOrder.CheckListCode);
            checkListOrder.CheckListName = checkListMaster.Name;
            checkListOrder.Region = checkListMaster.Region;
            checkListOrder.FacilityID = checkListMaster.FacilityID;
            checkListOrder.FacilityName = checkListMaster.FacilityName;
            checkListOrder.Description = checkListMaster.Description;
            checkListOrder.Status = com.Sconit.Entity.BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE;
            genericMgr.Create(checkListOrder);
            #endregion

            if (checkListOrder.CheckListOrderDetailList != null && checkListOrder.CheckListOrderDetailList.Count > 0)
            {
                foreach (CheckListOrderDetail d in checkListOrder.CheckListOrderDetailList)
                {
                    d.OrderNo = checkListOrder.Code;

                    d.CreateDate = checkListOrder.CreateDate;
                    d.CreateUser = checkListOrder.CreateUser;
                    d.LastModifyDate = checkListOrder.LastModifyDate;
                    d.LastModifyUser = checkListOrder.LastModifyUser;
                    genericMgr.Create(d);
                }
            }

        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateCheckListOrder(CheckListOrderMaster checkListOrder)
        {
            #region 保存头
            genericMgr.Update(checkListOrder);
            #endregion

            foreach (CheckListOrderDetail d in checkListOrder.CheckListOrderDetailList)
            {
                genericMgr.Update(d);
            }

        }


        [Transaction(TransactionMode.Requires)]
        public void DeleteCheckListOrder(string checkListCode)
        {
            #region 保存头
            CheckListOrderMaster checkListOrder = this.GetCheckListOrderMaster(checkListCode);
            if (checkListOrder.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            {
                throw new BusinessErrorException("非创建状态的巡检单不能删除");
            }
            foreach (CheckListOrderDetail d in checkListOrder.CheckListOrderDetailList)
            {
                genericMgr.Delete(d);
            }
            genericMgr.Delete(checkListOrder);
            #endregion
        }

        public CheckListMaster GetCheckListMaster(string code)
        {
            var checkListMaster = genericMgr.FindById<CheckListMaster>(code);
            var checkListDetail = genericMgr.FindAll<CheckListDetail>("from CheckListDetail where CheckListCode = ? order by Seq desc ", checkListMaster.Code) ?? new List<CheckListDetail>();
            checkListMaster.CheckListDetailList = checkListDetail.OrderBy(d => d.Seq).ToList();
            return checkListMaster;
        }

        public CheckListOrderMaster GetCheckListOrderMaster(string code)
        {
            var checkListOrderMaster = genericMgr.FindById<CheckListOrderMaster>(code);
            var checkListOrderDetail = genericMgr.FindAll<CheckListOrderDetail>("from CheckListOrderDetail where OrderNo = ? order by Seq desc ", code) ?? new List<CheckListOrderDetail>();
            checkListOrderMaster.CheckListOrderDetailList = checkListOrderDetail.OrderBy(d => d.Seq).ToList();
            return checkListOrderMaster;
        }

        public IList<CheckListMaster> GetAllCheckListMaster()
        {
            return genericMgr.FindAll<CheckListMaster>();

        }

        public void CloseCheckListMaster(string code, User user)
        {
            CheckListOrderMaster master = this.GetCheckListOrderMaster(code);
            var checkListMaster = genericMgr.FindById<CheckListMaster>(master.CheckListCode);

            master.LastModifyDate = DateTime.Now;
            master.LastModifyUser = user.Code;
            master.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;
            genericMgr.Update(master);

            // 创建ISI问题
            var abnomalDetails = master.CheckListOrderDetailList.Where(p => !p.IsNormal);
            if (!string.IsNullOrEmpty(checkListMaster.TaskSubType) && abnomalDetails.Any())
            {
                var subject = string.Format("巡检异常:{0},{1}[{2}]", master.Code, master.CheckListCode, master.CheckListName);
                var desc1 = string.Format("巡检设备名称:{0}", master.FacilityName);
                foreach (var abnomalDetail in abnomalDetails)
                {
                    StringBuilder str = new StringBuilder();
                    str.AppendLine();
                    str.AppendFormat("--异常:{0}[{1}],结果{2}", abnomalDetail.CheckListDetailCode, abnomalDetail.Description, abnomalDetail.Remark);
                    desc1 += str.ToString();
                }

                var desc2 = master.Description;
                var result = master.Remark;

                if (desc1.Length > 2000)
                {
                    desc1 = desc1.Substring(0, 1990) + "...";
                }
                if (desc2.Length > 2000)
                {
                    desc2 = desc2.Substring(0, 1990) + "...";
                }
                if (result.Length > 2000)
                {
                    result = result.Substring(0, 1990) + "...";
                }

                TaskMstr task = new TaskMstr();
                task.Priority = BusinessConstants.CODE_MASTER_ISSUE_PRIORITY_NORMAL;
                task.Subject = subject;
                task.Desc1 = desc1;
                task.Desc2 = desc2;
                task.ExpectedResults = result;

                task.TaskSubType = taskSubTypeMgr.LoadTaskSubType(checkListMaster.TaskSubType);
                task.UserName = user.Name;
                task.Email = user.Email;
                task.MobilePhone = user.MobliePhone;
                task.TaskAddress = user.Address;
                task.Type = ISIConstants.ISI_TASK_TYPE_ISSUE;
                task.IsAutoRelease = true;

                task.AssignStartUser = checkListMaster.SubUser;
                task.AssignStartUserNm = userSubscriptionMgr.GetUserName(checkListMaster.SubUser);

                //task.PatrolTime = patrolTime;
                task.PlanStartDate = DateTime.Now;
                task.PlanCompleteDate = DateTime.Now.AddDays(1);
                //task.IsNoSend = true;
                taskMgr.CreateTask(task, user);
                taskMgr.AssignTask(task.Code, user);
            }
        }
    }

}
