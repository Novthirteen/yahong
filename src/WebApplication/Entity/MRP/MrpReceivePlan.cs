using System;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP
{
    [Serializable]
    public class MrpReceivePlan : MrpReceivePlanBase
    {
        #region Non O/R Mapping Properties

        public IList<int> FlowDetailIdList { get; set; }

        public bool ContainFlowDetailId(int id)
        {
            if (FlowDetailIdList == null)
            {
                return false;
            }
            else
            {
                return FlowDetailIdList.Contains(id);
            }
        }

        public DateTime StartTime { get; set; }

        public DateTime WindowTime { get; set; }

        public string Flow { get; set; }

        public string FlowType { get; set; }

        #endregion
    }
}