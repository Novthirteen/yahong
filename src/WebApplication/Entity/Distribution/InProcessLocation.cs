using System;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.Distribution
{
    [Serializable]
    public class InProcessLocation : InProcessLocationBase
    {
        #region Non O/R Mapping Properties

        public void AddInProcessLocationDetail(InProcessLocationDetail inProcessLocationDetail)
        {
            if(this.InProcessLocationDetails == null)
            {
                this.InProcessLocationDetails = new List<InProcessLocationDetail>();
            }

            this.InProcessLocationDetails.Add(inProcessLocationDetail);
        }
        public DateTime WindowTime
        {
            get
            {
                if(ArriveTime.HasValue)
                {
                    return ArriveTime.Value;
                }

                if(this.InProcessLocationDetails != null && this.InProcessLocationDetails.Count > 0)
                {
                    return InProcessLocationDetails[0].OrderLocationTransaction.OrderDetail.OrderHead.WindowTime;
                }
                return DateTime.MinValue;
            }
        }
        #endregion
    }
}