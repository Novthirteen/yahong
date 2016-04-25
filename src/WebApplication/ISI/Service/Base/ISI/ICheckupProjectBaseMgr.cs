using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ICheckupProjectBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateCheckupProject(CheckupProject entity);

        CheckupProject LoadCheckupProject(String code);

        IList<CheckupProject> GetAllCheckupProject();
    
        IList<CheckupProject> GetAllCheckupProject(bool includeInactive);
      
        void UpdateCheckupProject(CheckupProject entity);

        void DeleteCheckupProject(String code);
    
        void DeleteCheckupProject(CheckupProject entity);
    
        void DeleteCheckupProject(IList<String> pkList);
    
        void DeleteCheckupProject(IList<CheckupProject> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
