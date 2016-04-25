using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IApplyBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateApply(Apply entity);

        Apply LoadApply(String code);

        IList<Apply> GetAllApply();
    
        void UpdateApply(Apply entity);

        void DeleteApply(String code);
    
        void DeleteApply(Apply entity);
    
        void DeleteApply(IList<String> pkList);
    
        void DeleteApply(IList<Apply> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
