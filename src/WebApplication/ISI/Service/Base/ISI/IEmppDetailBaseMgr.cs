using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IEmppDetailBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateEmppDetail(EmppDetail entity);

        EmppDetail LoadEmppDetail(Int32 id);

        IList<EmppDetail> GetAllEmppDetail();
    
        void UpdateEmppDetail(EmppDetail entity);

        void DeleteEmppDetail(Int32 id);
    
        void DeleteEmppDetail(EmppDetail entity);
    
        void DeleteEmppDetail(IList<Int32> pkList);
    
        void DeleteEmppDetail(IList<EmppDetail> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
