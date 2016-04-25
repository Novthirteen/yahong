using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class ProcessDefinition : ProcessDefinitionBase
    {
        #region Non O/R Mapping Properties

        private Boolean _isBlankDetail = false;
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

        public string User
        {
            get
            {
                if (string.IsNullOrEmpty(this.UserCode))
                {
                    return string.Empty;
                }
                return this.UserCode + "[" + this.UserNm + "]";
            }
        }
        #endregion
    }
}