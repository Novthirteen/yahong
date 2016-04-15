using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Entity.MasterData;
//TODO: Add other using statements here.

namespace com.Sconit.Persistence.MasterData
{
    public interface IInspectComfirmResultBaseDao
    {
        #region Method Created By CodeSmith

        void CreateInspectComfirmResult(InspectComfirmResult entity);

        InspectComfirmResult LoadInspectComfirmResult(Int32 id);
  
        IList<InspectComfirmResult> GetAllInspectComfirmResult();
  
        void UpdateInspectComfirmResult(InspectComfirmResult entity);
        
        void DeleteInspectComfirmResult(Int32 id);
    
        void DeleteInspectComfirmResult(InspectComfirmResult entity);
    
        void DeleteInspectComfirmResult(IList<Int32> pkList);
    
        void DeleteInspectComfirmResult(IList<InspectComfirmResult> entityList);    
        #endregion Method Created By CodeSmith
    }
}
