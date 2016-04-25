using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IResWokShopBaseDao
    {
        #region Method Created By CodeSmith

        void CreateResWokShop(ResWokShop entity);

        ResWokShop LoadResWokShop(String code);
  
        IList<ResWokShop> GetAllResWokShop();
  
        IList<ResWokShop> GetAllResWokShop(bool includeInactive);
  
        void UpdateResWokShop(ResWokShop entity);
        
        void DeleteResWokShop(String code);
    
        void DeleteResWokShop(ResWokShop entity);
    
        void DeleteResWokShop(IList<String> pkList);
    
        void DeleteResWokShop(IList<ResWokShop> entityList);    
        #endregion Method Created By CodeSmith
    }
}
