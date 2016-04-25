using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class FailureMode : FailureModeBase
    {
        #region Non O/R Mapping Properties

        public string Description
        {
            get
            {
                return this.Code + "[" + this.Desc + "]";
            }
        }

        #endregion
    }
}