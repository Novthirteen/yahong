using System;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.View
{
    [Serializable]
    public class OrderLocTransView : OrderLocTransViewBase
    {
        #region Non O/R Mapping Properties

        private decimal theoryQty;
        public decimal TheoryQty
        {
            get
            {
                theoryQty = 0;
                if (OrderQty != 0)
                {
                    theoryQty = (PlanQty / OrderQty) * (RecQty + RejQty + ScrapQty);
                }
                return theoryQty;
            }
        }

        #endregion
    }
}