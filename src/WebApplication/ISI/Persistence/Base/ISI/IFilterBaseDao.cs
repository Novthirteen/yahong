using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IFilterBaseDao
    {
        #region Method Created By CodeSmith

        void CreateFilter(Filter entity);

        Filter LoadFilter(Int32 id);
  
        IList<Filter> GetAllFilter();
  
        void UpdateFilter(Filter entity);
        
        void DeleteFilter(Int32 id);
    
        void DeleteFilter(Filter entity);
    
        void DeleteFilter(IList<Int32> pkList);
    
        void DeleteFilter(IList<Filter> entityList);    
        #endregion Method Created By CodeSmith
    }
}
