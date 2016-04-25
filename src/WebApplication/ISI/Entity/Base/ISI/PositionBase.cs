using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class PositionBase : EntityBase
    {
        #region O/R Mapping Properties

        public string Position { get; set; }
        public string RoleType { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyUser { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (Position != null)
            {
                return Position.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            PositionBase another = obj as PositionBase;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Position == another.Position);
            }
        }
    }

}
