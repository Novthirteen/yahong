using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public  class ResTaskDet : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string TaskCode { get; set; }
        public string Subject { get; set; }
        public string Desc1 { get; set; }
        public DateTime CreateDate { get; set; }

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
            ResTaskDet another = obj as ResTaskDet;

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
