using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class Evaluation : EvaluationBase
    {
        #region Non O/R Mapping Properties

        /// <summary>
        /// �����ֶΣ��жϸ�Detail�Ƿ��¼ӵĿ���
        /// </summary>
        public Boolean IsBlankDetail { get; set; }

        public string OldUserCode { get; set; }


        /// <summary>
        /// �����ֶΣ��ָ���
        /// </summary>
        public Boolean IsSeparator { get; set; }

        #endregion
    }
}