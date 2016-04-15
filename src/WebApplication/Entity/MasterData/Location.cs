using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public class Location : LocationBase
    {
        #region Non O/R Mapping Properties

        public bool Freeze
        {

            get
            {
                return IsFreeze || !IsActive;
            }
        }

        #endregion
    }
}