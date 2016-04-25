using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IProcessDefinitionBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateProcessDefinition(ProcessDefinition entity);

        ProcessDefinition LoadProcessDefinition(Int32 id);

        IList<ProcessDefinition> GetAllProcessDefinition();
    
        void UpdateProcessDefinition(ProcessDefinition entity);

        void DeleteProcessDefinition(Int32 id);
    
        void DeleteProcessDefinition(ProcessDefinition entity);
    
        void DeleteProcessDefinition(IList<Int32> pkList);
    
        void DeleteProcessDefinition(IList<ProcessDefinition> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
