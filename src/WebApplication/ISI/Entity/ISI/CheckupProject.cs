using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class CheckupProject : CheckupProjectBase
    {
        #region Non O/R Mapping Properties

        public string Name
        {
            get
            {
                return this.Code + "[" + this.Desc + "]";
            }
       }

        #endregion
    }
}