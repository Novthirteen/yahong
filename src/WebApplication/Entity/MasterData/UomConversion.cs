using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public class UomConversion : UomConversionBase
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        public decimal? Qty { get; set; }

        public bool IsAsc { get; set; }

        #endregion
    }
}