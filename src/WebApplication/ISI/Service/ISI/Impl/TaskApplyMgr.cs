using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Criteria;
using System.Linq;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using NPOI.HSSF.UserModel;
using com.Sconit.Service.Ext.Hql;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{

    [Transactional]
    public class TaskApplyMgr : TaskApplyBaseMgr, ITaskApplyMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        #region Customized Methods

        //TODO: Add other methods here.

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskApply> GetTaskApply(string taskCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskApply));
            criteria.Add(Expression.Eq("TaskCode", taskCode));
            criteria.AddOrder(Order.Asc("Seq"));
            IList<TaskApply> taskApplyList = this.criteriaMgrE.FindAll<TaskApply>(criteria);
            return taskApplyList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskApply> GetActiveTaskApply(string taskCode)
        {
            var taskApplyList = hqlMgrE.FindAll<TaskApply>("from TaskApply where TaskCode='" + taskCode + "' and (Qty is not null or (Value !='' and Value is not null) or DateValue is not null or Checked = 1) order by Seq asc ");

            return taskApplyList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskApply> GetTaskApply(IList<string> taskCodeList)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskApply));
            criteria.Add(Expression.In("TaskCode", taskCodeList.ToArray()));
            criteria.Add(Expression.Or(Expression.And(Expression.IsNotNull("Value"), Expression.Not(Expression.Eq("Value", string.Empty))),
                            Expression.Or(Expression.IsNotNull("Qty"), Expression.Or(Expression.IsNotNull("DateValue"), Expression.And(Expression.IsNotNull("Checked"), Expression.Eq("Checked", true))))));
            criteria.AddOrder(Order.Asc("Seq"));
            IList<TaskApply> taskApplyList = this.criteriaMgrE.FindAll<TaskApply>(criteria);
            return taskApplyList;
        }

        /// <summary>
        /// 邮件调用
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="taskApplyList"></param>
        public void OutputEmailApply(StringBuilder desc, IList<TaskApply> taskApplyList, bool isRemoveApply)
        {
            OutputApply(desc, taskApplyList, isRemoveApply, ISIConstants.TASKAPPLY_GEN_EMAIL, BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN);
        }


        /// <summary>
        /// 任务跟踪调用
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="taskApplyList"></param>
        /// <param name="language"></param>
        public void OutputApply(StringBuilder desc, IList<TaskApply> taskApplyList, string language)
        {
            OutputApply(desc, taskApplyList, false, ISIConstants.TASKAPPLY_GEN_HTML, language);
        }
        /// <summary>
        /// 打印调用
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="taskApplyList"></param>
        public void OutputApply(StringBuilder desc, IList<TaskApply> taskApplyList)
        {
            OutputApply(desc, taskApplyList, false, ISIConstants.TASKAPPLY_GEN_PRINT, BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN);
        }
        private void OutputApply(StringBuilder desc, IList<TaskApply> taskApplyList, bool isRemoveApply, string type, string language)
        {
            if (taskApplyList != null && taskApplyList.Count > 0)
            {
                string seprator = string.Empty;
                if (type == ISIConstants.TASKAPPLY_GEN_HTML)
                {
                    seprator = ISIConstants.EMAIL_SEPRATOR;
                }
                if (type == ISIConstants.TASKAPPLY_GEN_EMAIL)
                {
                    seprator = ISIConstants.EMAIL_SEPRATOR;
                }
                if (type == ISIConstants.TASKAPPLY_GEN_PRINT)
                {
                    seprator = ISIConstants.TEXT_SEPRATOR;
                }

                for (int i = 0; i < taskApplyList.Count; i++)
                {
                    var apply = taskApplyList[i];
                    if (i != 0 && desc.Length > 0)
                    {
                        if (type == ISIConstants.TASKAPPLY_GEN_HTML || ((taskApplyList[i - 1].IsRow.HasValue && taskApplyList[i - 1].IsRow.Value) || (apply.IsRow.HasValue && apply.IsRow.Value) || apply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || apply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO))
                        {
                            desc.Append(seprator);
                        }
                        else
                        {
                            if (type == ISIConstants.TASKAPPLY_GEN_EMAIL)
                            {
                                desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
                            }
                            else
                            {
                                desc.Append("    ");
                            }
                        }
                    }

                    StringBuilder style = new StringBuilder();
                    if (type == ISIConstants.TASKAPPLY_GEN_EMAIL)
                    {
                        if (apply.FontSize.HasValue && apply.FontSize.Value > 0)
                        {
                            style.Append("font-size: " + apply.FontSize.Value + "px;");
                        }
                        if (!string.IsNullOrEmpty(apply.Align))
                        {
                            style.Append("text-align: " + apply.Align + ";");
                        }
                    }

                    if (apply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || apply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
                    {
                        var groupDesc = apply.UOMDesc(language);

                        if (!string.IsNullOrEmpty(groupDesc))
                        {
                            desc.Append(type == ISIConstants.TASKAPPLY_GEN_HTML ? "<span style='background:#DFD3D3;'>" : string.Empty);
                            desc.Append(type == ISIConstants.TASKAPPLY_GEN_EMAIL ? "<u style='" + style.ToString() + "'>" : string.Empty);
                            desc.Append(groupDesc.Trim());
                            desc.Append(type == ISIConstants.TASKAPPLY_GEN_EMAIL ? "</u>" : string.Empty);
                            desc.Append(type == ISIConstants.TASKAPPLY_GEN_HTML ? "</span>" : string.Empty);
                            desc.Append("：");
                        }
                        var applyList = taskApplyList.Where(p => p.Type == apply.Type && p.Seq == apply.Seq).OrderBy(p => p.Seq).ToList();

                        for (int j = 0; j < applyList.Count; j++)
                        {
                            var taskApply = applyList[j];
                            if (j != 0)
                            {
                                desc.Append(type == ISIConstants.TASKAPPLY_GEN_HTML || type == ISIConstants.TASKAPPLY_GEN_EMAIL ? "" : "  ");
                            }
                            if (taskApply.Checked.HasValue && taskApply.Checked.Value)
                            {
                                desc.Append(type == ISIConstants.TASKAPPLY_GEN_HTML || type == ISIConstants.TASKAPPLY_GEN_EMAIL ? "<input type='checkbox' checked='true' disabled='true' />" : "☑");//█ 
                            }
                            else
                            {
                                desc.Append(type == ISIConstants.TASKAPPLY_GEN_HTML || type == ISIConstants.TASKAPPLY_GEN_EMAIL ? "<input type='checkbox' disabled='true' />" : "□");
                            }
                            desc.Append(taskApply.GetDesc(language).Trim());
                        }
                        i += applyList.Count - 1;
                    }
                    else
                    {
                        desc.Append(type == ISIConstants.TASKAPPLY_GEN_HTML ? "<span style='background:#DFD3D3;'>" : string.Empty);
                        desc.Append(type == ISIConstants.TASKAPPLY_GEN_EMAIL ? "<u style='" + style.ToString() + "'>" : string.Empty);
                        desc.Append(apply.GetDesc(language).Trim());
                        desc.Append(type == ISIConstants.TASKAPPLY_GEN_EMAIL ? "</u>" : string.Empty);
                        desc.Append(type == ISIConstants.TASKAPPLY_GEN_HTML ? "</span>" : string.Empty);
                        desc.Append("：");
                        if (apply.Type != ISIConstants.CODE_MASTER_WFS_TYPE_LABEL)
                        {
                            string value = apply.GetValue();
                            if (!string.IsNullOrEmpty(value))
                            {
                                desc.Append(type == ISIConstants.TASKAPPLY_GEN_EMAIL && value != ISIConstants.STRING_EMPTY && !isRemoveApply ? "<span style='background:#DFD3D3;'>" : string.Empty);
                                desc.Append(apply.GetValue().Trim());
                                desc.Append(type == ISIConstants.TASKAPPLY_GEN_EMAIL && value != ISIConstants.STRING_EMPTY && !isRemoveApply ? "</span>" : string.Empty);
                            }
                        }
                    }
                }
            }
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class TaskApplyMgrE : com.Sconit.ISI.Service.Impl.TaskApplyMgr, ITaskApplyMgrE
    {
    }
}

#endregion Extend Class