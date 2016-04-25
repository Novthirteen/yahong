using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IResMatrixBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateResMatrix(ResMatrix entity);

        ResMatrix LoadResMatrix(Int32 id);

        IList<ResMatrix> GetAllResMatrix();
    
        void UpdateResMatrix(ResMatrix entity);

        void DeleteResMatrix(Int32 id);
    
        void DeleteResMatrix(ResMatrix entity);
    
        void DeleteResMatrix(IList<Int32> pkList);
    
        void DeleteResMatrix(IList<ResMatrix> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
