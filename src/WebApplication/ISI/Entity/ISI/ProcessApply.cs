using com.Sconit.Entity;
using com.Sconit.ISI.Service.Util;
using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class ProcessApply : ProcessApplyBase
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        private Boolean _isBlankDetail = false;
        /// <summary>
        /// ¸¨Öú×Ö¶Î£¬ÅÐ¶Ï¸ÃDetailÊÇ·ñÐÂ¼ÓµÄ¿ÕÐÐ
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

        /// <summary>
        /// ¸¨Öú×Ö¶Î£¬·Ö¸ôÏß
        /// </summary>
        public Boolean IsSeparator { get; set; }

        public string UOMDesc()
        {
            return UOMDesc(BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN);
        }
        public string UOMDesc(string language)
        {
            if (!string.IsNullOrEmpty(UOM) || !string.IsNullOrEmpty(UOMDesc1))
            {
                if (language == BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN)
                {
                    return " " + ISIUtil.GetDesc(UOMDesc1, UOMDesc2) + CurrencyDesc;
                }
                else
                {
                    return " " + ISIUtil.GetDesc(UOMDesc2, UOMDesc1) + CurrencyDesc;
                }
            }
            return string.Empty;
        }


        public string GetDesc()
        {
            return GetDesc(BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN);
        }

        public string GetDesc(string language)
        {
            if (language == BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN)
            {
                return ISIUtil.GetDesc(Desc1, Desc2);
            }
            else
            {
                return ISIUtil.GetDesc(Desc2, Desc1);
            }
        }

        public string CurrencyDesc
        {
            get
            {
                if (!string.IsNullOrEmpty(Currency))
                {
                    return Currency;
                }
                return string.Empty;
            }
        }

        #endregion
    }
}