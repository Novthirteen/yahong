using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class ResRole : ResRoleBase
    {
        #region Non O/R Mapping Properties


        public string CodeName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Name))
                {
                    return this.Code + "[" + this.Name + "]";
                }
                else
                {
                    return this.Code;
                }
            }
        }

        #endregion
    }
}