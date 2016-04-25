using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace com.Sconit.ISI.Entity
{
    public partial class OrgChart : EntityBase
    {
        #region O/R Mapping Properties

        public string Org { get; set; }
        public string ParentOrg { get; set; }
        public string UserCodes { get; set; }
        public string UserNames { get; set; }

        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyUser { get; set; }
        #endregion

    }
}
