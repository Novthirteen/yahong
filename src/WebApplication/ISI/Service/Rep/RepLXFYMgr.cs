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

namespace com.Sconit.ISI.Service.Report.Impl
{
    [Transactional]
    public class RepLXFYMgr : ReportBaseMgr
    {
        public override string reportTemplateFolder { get; set; }

        public ITaskMstrMgrE taskMstrMgrE { get; set; }
        public ILanguageMgrE languageMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        public ITaskApplyMgrE taskApplyMgrE { get; set; }
        public RepLXFYMgr()
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

                this.SetRowCell(pageIndex, rowIndex, 1, task.SubmitUserNm);
                if (task.CostCenter != null)
                {
                    this.SetRowCell(pageIndex, rowIndex, 3, task.CostCenter.Description);
                }
                this.SetRowCell(pageIndex, rowIndex++, 5, task.SubmitDate.Value.ToString("yyyy-MM-dd"));
                this.SetRowCell(pageIndex, rowIndex, 1, task.Code);
                this.SetRowCell(pageIndex, rowIndex, 3, languageMgrE.ProcessLanguage("${ISI.Status." + task.Status + "}", BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN));
                rowIndex++;
                var taskApplyList = taskApplyMgrE.GetTaskApply(task.Code);

                //附件张数
                var apply = taskApplyList.Where(ta => ta.Desc1 == "附件张数").FirstOrDefault();
                if (apply != null && apply.Qty.HasValue)
                {
                    this.SetRowCell(pageIndex, rowIndex, 2, apply.Desc1);
                    this.SetRowCell(pageIndex, rowIndex, 3, apply.Qty.Value.ToString("0.########"));
                    taskApplyList.Remove(apply);
                }

                //金额
                apply = taskApplyList.Where(ta => ta.Apply == ISIConstants.APPLY_AMOUNT).FirstOrDefault();
                if (apply != null && apply.Qty.HasValue)
                {
                    this.SetRowCell(pageIndex, rowIndex, 1, apply.Desc1);
                    this.SetRowCell(pageIndex, rowIndex + 1, 1, apply.GetValue());
                    taskApplyList.Remove(apply);
                }

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

                this.SetRowCell(pageIndex, 6, 0, desc.ToString());

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
                        this.SetRowCell(pageIndex, 8, 0, desc.ToString());
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
    public partial class RepLXFYMgrE : com.Sconit.ISI.Service.Report.Impl.RepLXFYMgr, IReportBaseMgrE
    {


    }
}

#endregion
