using System;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class ResMatrix : ResMatrixBase
    {
        #region Non O/R Mapping Properties

        public ResWokShop ResWokShop { get; set; }

        public string OldWorkShop { get; set; }
        public int? OldOperate { get; set; }
        public string OldRole { get; set; }
        //public string OldPriority { get; set; }
        public string OldResponsibility { get; set; }

        #endregion
    }
}