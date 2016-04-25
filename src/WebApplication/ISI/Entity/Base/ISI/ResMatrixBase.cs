using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class ResMatrixBase : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string WorkShop { get; set; }
        public Int32? Operate { get; set; }
        public string Role { get; set; }
        public string Responsibility { get; set; }
        //public string Director { get; set; }
        //public string Priority { get; set; }
        //public string SkillLevel { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyUser { get; set; }
        public Boolean NeedPatrol { get; set; }
        public Int32 Seq { get; set; }
        public string TimePeriodType { get; set; }
        public DateTime NextPatrolTime { get; set; }
        public string TaskSubType { get; set; }
        public Boolean IsIndependentTask { get; set; }

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
            ResMatrixBase another = obj as ResMatrixBase;

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
