using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ICheckupRemindBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateCheckupRemind(CheckupRemind entity);

        CheckupRemind LoadCheckupRemind(Int32 id);

        IList<CheckupRemind> GetAllCheckupRemind();
    
        void UpdateCheckupRemind(CheckupRemind entity);

        void DeleteCheckupRemind(Int32 id);
    
        void DeleteCheckupRemind(CheckupRemind entity);
    
        void DeleteCheckupRemind(IList<Int32> pkList);
    
        void DeleteCheckupRemind(IList<CheckupRemind> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
