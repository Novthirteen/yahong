using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IPositionBaseMgr
    {
        #region Method Created By CodeSmith

        void CreatePosition(Position entity);

        Position LoadPosition(String position);

        IList<Position> GetAllPosition();
    
        void UpdatePosition(Position entity);

        void DeletePosition(String position);
    
        void DeletePosition(Position entity);
    
        void DeletePosition(IList<String> pkList);
    
        void DeletePosition(IList<Position> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
