using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class ResRoleBase : EntityBase
    {
        #region O/R Mapping Properties
		
		public string Code { get; set; }
		public string Name { get; set; }
		public string RoleType { get; set; }
		public Boolean IsActive { get; set; }
		public DateTime CreateDate { get; set; }
		public string CreateUser { get; set; }
		public DateTime LastModifyDate { get; set; }
		public string LastModifyUser { get; set; }
        
        #endregion
 
    }
	
}
