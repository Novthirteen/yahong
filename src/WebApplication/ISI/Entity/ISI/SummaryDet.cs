using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class SummaryDet : SummaryDetBase
    {
        #region Non O/R Mapping Properties

        private Boolean _isBlankDetail = false;
        //TODO: Add Non O/R Mapping Properties here. 
        /// <summary>
        /// �����ֶΣ��жϸ�Detail�Ƿ��¼ӵĿ���
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

        public bool IsLast { get; set; }
        #endregion
    }
}