using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public class FacilityItem : FacilityItemBase
    {
        public Decimal AllocateRate
        {
            get
            {
                decimal allocateRate = 0;
                if ((this.InitQty > 0 || this.SingleQty > 0) && this.Qty > 0)
                {
                    allocateRate = 100 * (this.InitQty + SingleQty) / this.Qty;
                }
                return allocateRate;
            }

        }
    }

}
