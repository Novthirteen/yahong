using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public abstract class FacilityFixOrderBase : EntityBase
    {
        #region O/R Mapping Properties


        public string FixNo { get; set; }

        public string Status { get; set; }

        public string FCID { get; set; }

        public string FacilityName { get; set; }

        public string ReferenceCode { get; set; }

        public string Shift { get; set; }

        public string Customer { get; set; }

        public string Description { get; set; }

        public string Result { get; set; }

        public Boolean IsSample { get; set; }

        public string FixSite { get; set; }

        public Decimal FixExpense { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string CreateUser { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string ReleaseUser { get; set; }

        public DateTime? StartDate { get; set; }

        public string StartUser { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string CompleteUser { get; set; }

        public DateTime? CloseDate { get; set; }

        public string CloseUser { get; set; }

        public DateTime LastModifyDate { get; set; }

        public string LastModifyUser { get; set; }




        #endregion

   

        public override int GetHashCode()
        {
            if (FixNo != null)
            {
                return FixNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            FacilityFixOrderBase another = obj as FacilityFixOrderBase;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.FixNo == another.FixNo);
            }
        }
    }

}
