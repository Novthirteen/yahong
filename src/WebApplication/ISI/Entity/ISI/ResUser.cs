using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class ResUser : ResUserBase
    {
        #region Non O/R Mapping Properties

        public DateTime OldStartDate { get; set; }
        public DateTime OldEndDate { get; set; }
        public string OldPriority { get; set; }

        #endregion
    }
}