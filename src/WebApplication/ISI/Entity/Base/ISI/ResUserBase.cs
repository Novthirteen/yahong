using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class ResUserBase : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public Int32 MatrixId { get; set; }
		public string UserCode { get; set; }
		public string UserName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string SkillLevel { get; set; }
		public DateTime CreateDate { get; set; }
		public string CreateUser { get; set; }
		public DateTime LastModifyDate { get; set; }
		public string LastModifyUser { get; set; }
        public string Priority { get; set; }
        public Boolean NeedPatrol { get; set; }
        
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
            ResUserBase another = obj as ResUserBase;

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
