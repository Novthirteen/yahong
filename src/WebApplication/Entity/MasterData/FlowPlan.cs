using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public class FlowPlan : FlowPlanBase
    {
        #region Non O/R Mapping Properties


        public string FlowCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemReference { get; set; }
        public decimal? UC { get; set; }
        public int Seq { get; set; }
        public string Memo { get; set; }
        public string Shift { get; set; }

        #endregion
    }
}