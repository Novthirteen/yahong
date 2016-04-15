using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public class Item : ItemBase
    {
        #region Non O/R Mapping Properties

        public string Description
        {
            get
            {
                //return Desc1;
                return ((Desc1 != null ? Desc1 : string.Empty) + ((Desc2 != null && Desc2 != string.Empty) ? "[" + Desc2 + "]" : string.Empty));
            }

        }

        public bool IsBlank { get; set; }

        public string DefaultBomCode
        {
            get
            {
                if (this.Bom != null)
                {
                    return Bom.Code;
                }
                else
                {
                    return Code;
                }
            }

        }

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