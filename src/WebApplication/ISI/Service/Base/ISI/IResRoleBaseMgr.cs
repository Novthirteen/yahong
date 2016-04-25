using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IResRoleBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateResRole(ResRole entity);

        ResRole LoadResRole(String code);

        IList<ResRole> GetAllResRole();
    
        IList<ResRole> GetAllResRole(bool includeInactive);
      
        void UpdateResRole(ResRole entity);

        void DeleteResRole(String code);
    
        void DeleteResRole(ResRole entity);
    
        void DeleteResRole(IList<String> pkList);
    
        void DeleteResRole(IList<ResRole> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
