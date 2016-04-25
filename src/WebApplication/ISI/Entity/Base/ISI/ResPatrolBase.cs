using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class ResPatrolBase : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string WorkShop { get; set; }
        public string Role { get; set; }
        public string WinTime1 { get; set; }
        public string WinTime2 { get; set; }
        public string WinTime3 { get; set; }
        public string WinTime4 { get; set; }
        public string WinTime5 { get; set; }
        public string WinTime6 { get; set; }
        public string WinTime7 { get; set; }
        public DateTime NextOrderTime { get; set; }
        public DateTime NextWinTime { get; set; }
        public Int32 WeekInterval { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyUser { get; set; }
        public double LeadTime { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Id != null)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ResPatrolBase another = obj as ResPatrolBase;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id);
            }
        }
    }

}
