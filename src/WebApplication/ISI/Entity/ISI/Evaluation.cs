using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class Evaluation : EvaluationBase
    {
        #region Non O/R Mapping Properties

        /// <summary>
        /// 辅助字段，判断该Detail是否新加的空行
        /// </summary>
        public Boolean IsBlankDetail { get; set; }

        public string OldUserCode { get; set; }


        /// <summary>
        /// 辅助字段，分隔线
        /// </summary>
        public Boolean IsSeparator { get; set; }

        #endregion
    }
}