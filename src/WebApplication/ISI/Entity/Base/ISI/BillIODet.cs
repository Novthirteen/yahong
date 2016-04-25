using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class BillIODet : EntityBase
    {
        #region O/R Mapping Properties
        public string Org { get; set; }
        public string OrgName { get; set; }
        public string OrgType { get; set; }
        public string Type { get; set; }
        public decimal? Amount { get; set; }
        public decimal? BilledAmount { get; set; }
        public decimal? PayAmount { get; set; }
        public decimal? Diff { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (Org != null)
            {
                return Org.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            BillIODet another = obj as BillIODet;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Org == another.Org);
            }
        }
    }

}
