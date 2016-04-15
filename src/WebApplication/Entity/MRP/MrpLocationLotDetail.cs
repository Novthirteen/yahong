using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP
{
    [Serializable]
    public class MrpLocationLotDetail : MrpLocationLotDetailBase
    {
        #region Non O/R Mapping Properties

        public decimal Qty { get; set; }

        public decimal ActiveQty
        {
            get
            {
                return Qty - SafeQty;
            }
        }

        #endregion
    }
}