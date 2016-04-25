using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class Cost : CostBase
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        private Boolean _isBlankDetail = false;
        /// <summary>
        /// 辅助字段，判断该Detail是否新加的空行
        /// </summary>
        public Boolean IsBlankDetail
        {
            get
            {
                return _isBlankDetail;
            }
            set
            {
                _isBlankDetail = value;
            }
        }

        public string TaskSubTypeName
        {
            get
            {
                if (String.IsNullOrEmpty(this.TaskSubType)) return string.Empty;
                return this.TaskSubType + "[" + this.TaskSubTypeDesc + "]";
            }
        }

        public string Account1Name
        {
            get
            {
                if (String.IsNullOrEmpty(this.Account1)) return string.Empty;
                return this.Account1 + "[" + this.Account1Desc + "]";
            }
        }
        public string Account2Name
        {
            get
            {
                if (String.IsNullOrEmpty(this.Account2)) return string.Empty;
                return this.Account2 + "[" + this.Account2Desc + "]";
            }
        }

        public string User
        {
            get
            {
                if (String.IsNullOrEmpty(this.UserCode)) return string.Empty;
                return this.UserCode + "[" + this.UserName + "]";
            }
        }

        public bool IsAccount1 { get; set; }
        public bool IsAccount2 { get; set; }
        
        #endregion
    }
}