using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class Checkup : CheckupBase
    {
        #region Non O/R Mapping Properties

        public string Desc
        {
            get
            {
                return this.CheckupUserNm
                    + (!string.IsNullOrEmpty(this.JobNo) ? " " + this.JobNo : string.Empty)
                    + (!string.IsNullOrEmpty(this.Dept2) ? " " + this.Dept2 : string.Empty)
                    + (!string.IsNullOrEmpty(this.Department) ? " " + this.Department : string.Empty);
            }
        }

        #endregion
    }
}