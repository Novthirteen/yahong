using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;


//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class ProcessDefinitionMgr : ProcessDefinitionBaseMgr, IProcessDefinitionMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        //TODO: Add other methods here.
        [Transaction(TransactionMode.Unspecified)]
        public IList<ProcessDefinition> GetProcessDefinition(string taskSubType)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ProcessDefinition));
            criteria.Add(Expression.Eq("TaskSubType", taskSubType));
            criteria.AddOrder(Order.Asc("Seq"));
            IList<ProcessDefinition> processDefinitionList = this.criteriaMgrE.FindAll<ProcessDefinition>(criteria);
            return processDefinitionList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<ProcessDefinition> GetProcessDefinition(string taskSubType, string processNo)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ProcessDefinition));
            criteria.Add(Expression.Eq("TaskSubType", processNo));
            criteria.AddOrder(Order.Asc("Seq"));
            IList<ProcessDefinition> processDefinitionList = this.criteriaMgrE.FindAll<ProcessDefinition>(criteria);
            if (processDefinitionList == null || processDefinitionList.Count == 0)
            {
                criteria = DetachedCriteria.For(typeof(ProcessDefinition));
                criteria.Add(Expression.Eq("TaskSubType", taskSubType));
                criteria.AddOrder(Order.Asc("Seq"));
                processDefinitionList = this.criteriaMgrE.FindAll<ProcessDefinition>(criteria);
            }
            return processDefinitionList;
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class ProcessDefinitionMgrE : com.Sconit.ISI.Service.Impl.ProcessDefinitionMgr, IProcessDefinitionMgrE
    {
    }
}

#endregion Extend Class