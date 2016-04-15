using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public class ItemType : ItemTypeBase
    {
        #region Non O/R Mapping Properties

        public string FullName
        {
            get
            {
                if(string.IsNullOrEmpty(Name))
                {
                    return ShortName;
                }
                else
                {
                    return ShortName + " [" + Name + "]";
                }
            }
        }

        #endregion
    }
}