using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public class Permission : PermissionBase
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here.
        public bool Status { get; set; }

        public string CodeDescription
        {
            get
            {
                return this.Description + " [" + this.Code + "]";
            }
        }

        #endregion
    }
}