using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class ResWokShopBase : EntityBase
    {
        #region O/R Mapping Properties
		
		public string Code { get; set; }
		public string Name { get; set; }
        public string ParentCode { get; set; }
		public Boolean IsActive { get; set; }
		public DateTime CreateDate { get; set; }
		public string CreateUser { get; set; }
		public DateTime LastModifyDate { get; set; }
		public string LastModifyUser { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ResWokShopBase another = obj as ResWokShopBase;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Code == another.Code);
            }
        } 
    }
	
}
