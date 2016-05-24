using com.Sconit.Service.Ext.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MasterData;
using Castle.Services.Transaction;
using com.Sconit.Utility;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity;
using com.Sconit.Service.Report;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Ext;
using NPOI.SS.UserModel;
using com.Sconit.ISI.Service.Util;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Service.Ext.Hql;
using System.Linq;

namespace com.Sconit.ISI.Service.Report.Impl
{
    [Transactional]
    public class RepCCBXMgr : ReportBaseMgr
    {
        public override string reportTemplateFolder { get; set; }

        public ITaskMstrMgrE taskMstrMgrE { get; set; }
        public ILanguageMgrE languageMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        public ITaskApplyMgrE taskApplyMgrE { get; set; }
        public RepCCBXMgr()
        {

        }
        public override void CopyPageValues(int pageIndex)
        {

        }

        /**
         * 填充报表
         * 
         * Param list [0]OrderHead
         * Param list [0]IList<OrderDetail>           
         */
        [Transaction(TransactionMode.Requires)]
        public override bool FillValues(String templateFileName, IList<object> list)
        {
            try
            {
                if (list == null || list.Count < 1) return false;

                TaskMstr task = (TaskMstr)(list[0]);
                if (task == null || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE
                        || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN
                        || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE
                        || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL
                        || !task.TaskSubType.IsPrint
                        || !task.IsApply)
                {
                    return false;
                }

                this.init(templateFileName);

                int pageIndex = 1;
                int rowIndex = 1;

                var taskApplyList = taskApplyMgrE.GetTaskApply(task.Code);
                var typeList = taskApplyList.Where(ta => ta.UOMDesc1 == "类型").ToList();
                if (typeList != null && typeList.Count > 0)
                {
                    StringBuilder type = new StringBuilder();
                    foreach (var apply in typeList)
                    {
                        if (type.Length != 0)
                        {
                            type.Append("、");
                        }
                        type.Append(apply.Desc1);
                        taskApplyList.Remove(apply);
                    }
                    this.SetRowCell(pageIndex, 1, 1, type.ToString());
                }
                this.SetRowCell(pageIndex, 1, 3, task.SubmitDate.Value.ToString("yyyy-MM-dd"));
                this.SetRowCell(pageIndex, 1, 6, task.Code);
                if (task.CostCenter != null)
                {
                    this.SetRowCell(pageIndex, 2, 1, task.CostCenter.Description);
                }
                this.SetRowCell(pageIndex, 2, 3, task.SubmitUserNm);
                this.SetRowCell(pageIndex, 2, 6, languageMgrE.ProcessLanguage("${ISI.Status." + task.Status + "}", BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN));

                rowIndex = 4;
                //描述与申请项
                StringBuilder desc = new StringBuilder();
                if (!string.IsNullOrEmpty(task.Desc1))
                {
                    desc.Append(task.Desc1.Trim());
                }
                if (!string.IsNullOrEmpty(task.Desc2))
                {
                    if (desc.Length > 0)
                    {
                        desc.Append(ISIConstants.TEXT_SEPRATOR);
                    }
                    desc.Append(task.Desc2.Trim());
                }

                if (!string.IsNullOrEmpty(task.WorkHoursUserNm))
                {
                    if (desc.Length > 0)
                    {
                        desc.Append(ISIConstants.TEXT_SEPRATOR);
                    }
                    desc.Append("申请人" + task.WorkHoursUserNm);
                    desc.Append(task.WorkHoursUserNm.Trim());
                }
                if (!string.IsNullOrEmpty(task.ExpectedResults))
                {
                    if (desc.Length > 0)
                    {
                        desc.Append(ISIConstants.TEXT_SEPRATOR);
                    }
                    desc.Append(task.ExpectedResults.Trim());
                }

                this.SetRowCell(pageIndex, 4, 0, desc.ToString());

                typeList = taskApplyList.Where(ta => ta.UOMDesc1 == "类型").OrderBy(ta => ta.Seq).ToList();
                if (typeList != null && typeList.Count > 0)
                {
                    StringBuilder type = new StringBuilder();
                    foreach (var apply in typeList)
                    {
                        if (type.Length != 0)
                        {
                            type.Append("、");
                        }
                        type.Append(apply.Desc1);
                        taskApplyList.Remove(apply);
                    }
                    this.SetRowCell(pageIndex, 1, 1, type.ToString());
                }
                rowIndex = 7;
                var dateApplyList = taskApplyList.Where(ta => ta.Apply == ISIConstants.APPLY_DATE).OrderBy(ta => ta.Seq).ToList();
                if (dateApplyList != null && dateApplyList.Count > 0)
                {
                    for (int i = 0; i < dateApplyList.Count; i += 2)
                    {
                        StringBuilder date = new StringBuilder();
                        TaskApply apply1 = dateApplyList[i];
                        TaskApply apply2 = null;
                        if (dateApplyList.Count  > i + 1)
                        {
                            apply2 = dateApplyList[i + 1];
                        }
                        date.Append(apply1.DateValue.Value.ToString("yyyy-MM-dd"));
                        if (apply2 != null)
                        {
                            date.Append("  " + apply2.DateValue.Value.ToString("yyyy-MM-dd"));
                            taskApplyList.Remove(apply2);
                        }
                        this.SetRowCell(pageIndex, rowIndex++, 0, date.ToString());
                        taskApplyList.Remove(apply1);
                    }
                }

                rowIndex = 7;
                var shipFromApplyList = taskApplyList.Where(ta => ta.Apply == ISIConstants.APPLY_SHIPFROM).ToList();
                if (shipFromApplyList != null && shipFromApplyList.Count > 0)
                {
                    for (int i = 0; i < shipFromApplyList.Count; i++)
                    {
                        TaskApply apply = shipFromApplyList[i];
                        this.SetRowCell(pageIndex, rowIndex++, 1, apply.Value);
                        taskApplyList.Remove(apply);
                    }
                }

                rowIndex = 7;
                var shipToApplyList = taskApplyList.Where(ta => ta.Apply == ISIConstants.APPLY_SHIPTO).ToList();
                if (shipToApplyList != null && shipToApplyList.Count > 0)
                {
                    for (int i = 0; i < shipToApplyList.Count; i++)
                    {
                        TaskApply apply = shipToApplyList[i];
                        this.SetRowCell(pageIndex, rowIndex++, 2, apply.Value);
                        taskApplyList.Remove(apply);
                    }
                }

                rowIndex = 7;
                var amount2List = taskApplyList.Where(ta => ta.Desc1 == "住宿费").OrderBy(ta => ta.Seq).ToList();
                if (amount2List != null && amount2List.Count > 0)
                {
                    for (int i = 0; i < amount2List.Count; i++)
                    {
                        TaskApply apply = amount2List[i];
                        if (apply.Qty.HasValue)
                        {
                            this.SetRowCell(pageIndex, rowIndex++, 3, apply.Qty.Value.ToString("0.########"));
                        }
                        taskApplyList.Remove(apply);
                    }

                    this.SetRowCell(pageIndex, 11, 3, amount2List.Sum(a => a.Qty).Value.ToString("0.########"));
                }

                rowIndex = 7;
                amount2List = taskApplyList.Where(ta => ta.Desc1 == "陆地交通费").OrderBy(ta => ta.Seq).ToList();
                if (amount2List != null && amount2List.Count > 0)
                {
                    for (int i = 0; i < amount2List.Count; i++)
                    {
                        TaskApply apply = amount2List[i];
                        if (apply.Qty.HasValue)
                        {
                            this.SetRowCell(pageIndex, rowIndex++, 4, apply.Qty.Value.ToString("0.########"));
                        }
                        taskApplyList.Remove(apply);
                    }
                    this.SetRowCell(pageIndex, 11, 4, amount2List.Sum(a => a.Qty).Value.ToString("0.########"));
                }

                rowIndex = 7;
                amount2List = taskApplyList.Where(ta => ta.Desc1 == "包干费").OrderBy(ta => ta.Seq).ToList();
                if (amount2List != null && amount2List.Count > 0)
                {
                    for (int i = 0; i < amount2List.Count; i++)
                    {
                        TaskApply apply = amount2List[i];
                        if (apply.Qty.HasValue)
                        {
                            this.SetRowCell(pageIndex, rowIndex++, 5, apply.Qty.Value.ToString("0.########"));
                        }
                        taskApplyList.Remove(apply);
                    }
                    this.SetRowCell(pageIndex, 11, 5, amount2List.Sum(a => a.Qty).Value.ToString("0.########"));
                }

                rowIndex = 7;
                amount2List = taskApplyList.Where(ta => ta.Desc1 == "飞机票/火车票").OrderBy(ta => ta.Seq).ToList();
                if (amount2List != null && amount2List.Count > 0)
                {
                    for (int i = 0; i < amount2List.Count; i++)
                    {
                        TaskApply apply = amount2List[i];
                        if (apply.Qty.HasValue)
                        {
                            this.SetRowCell(pageIndex, rowIndex++, 6, apply.Qty.Value.ToString("0.########"));
                        }
                        taskApplyList.Remove(apply);
                    }
                    this.SetRowCell(pageIndex, 11, 6, amount2List.Sum(a => a.Qty).Value.ToString("0.########"));
                }
                //总金额
                var amountApply = taskApplyList.Where(ta => ta.Apply == ISIConstants.APPLY_AMOUNT).FirstOrDefault();
                if (amountApply != null && amountApply.Qty.HasValue)
                {
                    this.SetRowCell(pageIndex, 12, 3, amountApply.GetValue());
                    taskApplyList.Remove(amountApply);
                }

                rowIndex = 14;
                if (task.IsWF)
                {
                    //批示
                    StringBuilder hql = new StringBuilder("from TraceView where TaskCode ='" + task.Code + "' ");
                    hql.Append(" and Type = '" + ISIConstants.CODE_MASTER_ISI_MSG_TYPE_APPROVE + "'");
                    hql.Append("order by LastModifyDate desc ");
                    IList<TraceView> traceViewList = hqlMgrE.FindAll<TraceView>(hql.ToString());

                    if (traceViewList != null && traceViewList.Count > 0)
                    {
                        desc = new StringBuilder();
                        foreach (var traceView in traceViewList)
                        {
                            if (desc.Length > 0)
                            {
                                desc.Append(ISIConstants.TEXT_SEPRATOR);
                            }
                            desc.Append(traceView.LastModifyUserNm);
                            desc.Append("(");
                            desc.Append(traceView.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));
                            desc.Append("): ");
                            desc.Append(traceView.Desc);
                        }
                        this.SetRowCell(pageIndex, 14, 0, desc.ToString());
                    }
                }

                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public override IList<object> GetDataList(string code)
        {
            IList<object> list = new List<object>();
            TaskMstr task = taskMstrMgrE.CheckAndLoadTaskMstr(code, false);
            list.Add(task);
            return list;
        }

    }
}

#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Report.Impl
{
    [Transactional]
    public partial class RepCCBXMgrE : com.Sconit.ISI.Service.Report.Impl.RepCCBXMgr, IReportBaseMgrE
    {


    }
}

#endregion
