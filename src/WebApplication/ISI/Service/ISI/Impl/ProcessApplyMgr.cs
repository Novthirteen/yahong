using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using System.Linq;
using com.Sconit.Entity.Exception;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class ProcessApplyMgr : ProcessApplyBaseMgr, IProcessApplyMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        //TODO: Add other methods here.
        [Transaction(TransactionMode.Unspecified)]
        public IList<ProcessApply> GetProcessApply(string taskSubType)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ProcessApply));
            criteria.Add(Expression.Eq("TaskSubType", taskSubType));
            criteria.AddOrder(Order.Asc("Seq"));
            IList<ProcessApply> processApplyList = this.criteriaMgrE.FindAll<ProcessApply>(criteria);
            return processApplyList;
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<ProcessApply> GetProcessApply(string taskSubType, string apply)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ProcessApply));
            criteria.Add(Expression.Eq("TaskSubType", taskSubType));
            criteria.Add(Expression.Eq("Apply", apply));
            criteria.AddOrder(Order.Asc("Seq"));
            IList<ProcessApply> processApplyList = this.criteriaMgrE.FindAll<ProcessApply>(criteria);
            return processApplyList;
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateProcessApply(IDictionary<string, ProcessApply> processApplyDic)
        {

            IList<ProcessApply> processApplyList = processApplyDic.Values.ToList();

            UpdateProcessApply(processApplyList);
        }

        public void UpdateProcessApply(IList<ProcessApply> processApplyList)
        {
            //处理分组
            var groupList = processApplyList.Where(p => p.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || p.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO).GroupBy(p => new { p.Type, p.Seq }).ToList();
            if (groupList != null && groupList.Count > 0)
            {
                foreach (var group in groupList)
                {
                    var checkRadioList = processApplyList.Where(p => p.Type == group.Key.Type && p.Seq == group.Key.Seq).ToList();
                    var groupDescList = checkRadioList.Where(p => !string.IsNullOrEmpty(p.UOMDesc1) && !string.IsNullOrEmpty(p.UOMDesc2)).ToList();
                    if (groupDescList == null || groupDescList.Count == 0)
                    {
                        groupDescList = checkRadioList.Where(p => !string.IsNullOrEmpty(p.UOMDesc1)).ToList();

                        if (groupDescList == null || groupDescList.Count == 0)
                        {
                            throw new BusinessErrorException("ISI.TaskSubType.Apply.Group.IsNotEmpty");
                        }
                    }
                    if (groupDescList != null && groupDescList.Count > 0)
                    {
                        //bool isRow = processApplyList.Where(p => p.Type == group.Key.Type && p.Seq == group.Key.Seq && p.IsRow.HasValue && p.IsRow.Value).Count() > 1;
                        //bool isVertical = processApplyList.Where(p => p.Type == group.Key.Type && p.Seq == group.Key.Seq && p.IsVertical.HasValue && p.IsVertical.Value).Count() > 1;
                        foreach (var p in checkRadioList)
                        {
                            p.UOMDesc1 = groupDescList[0].UOMDesc1;
                            p.UOMDesc2 = groupDescList[0].UOMDesc2;
                            p.IsRow = groupDescList[0].IsRow;
                            //p.Required = groupDescList[0].Required;
                            p.IsVertical = groupDescList[0].IsVertical;
                            p.FontSize = groupDescList[0].FontSize;
                            p.Color = groupDescList[0].Color;
                            p.Align = groupDescList[0].Align;
                        }
                    }
                }
            }

            foreach (var processApply in processApplyList)
            {
                if (processApply.Id != 0)
                {
                    this.UpdateProcessApply(processApply);
                }
                else
                {
                    this.CreateProcessApply(processApply);
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
    public partial class ProcessApplyMgrE : com.Sconit.ISI.Service.Impl.ProcessApplyMgr, IProcessApplyMgrE
    {
    }
}

#endregion Extend Class