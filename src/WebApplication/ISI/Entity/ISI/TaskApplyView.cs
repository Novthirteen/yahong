using com.Sconit.Entity;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Utility;
using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class TaskApplyView : TaskApplyViewBase
    {
        #region Non O/R Mapping Properties
        public int? ValueCount { get; set; }
        public int? DateCount { get; set; }

        public int? CheckedCount { get; set; }

        public string CostCenter
        {
            get
            {
                if (!string.IsNullOrEmpty(this.CostCenterCode))
                {
                    return this.CostCenterCode + "[" + this.CostCenterDesc + "]";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string TaskSubTypeCodeDesc
        {
            get
            {
                return this.TaskSubType + "[" + this.TaskSubTypeDesc + "]";
            }
        }
        public string CurrencyDesc
        {
            get
            {
                if (!string.IsNullOrEmpty(Currency))
                {
                    if (this.Qty.HasValue)
                    {
                        return " " + StringHelper.MoneyCn(this.Qty) + " " + Currency;
                    }
                    else if (!string.IsNullOrEmpty(this.Value))
                    {
                        return " " + StringHelper.MoneyCn(decimal.Parse(this.Value)) + " " + Currency;
                    }
                }
                return string.Empty;
            }
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



        public string GetValue()
        {
            return GetValue(false, true, BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN);
        }

        public string GetValue(string language)
        {
            return GetValue(false, true, language);
        }

        public string GetValue(bool enabled, string language)
        {
            return GetValue(enabled, false, language);
        }
        public string GetCount()
        {
            if (this.Type == ISIConstants.CODE_MASTER_WFS_TYPE_QTY && this.Qty.HasValue)
            {
                return this.Qty.Value.ToString("0.########");
            }
            else if ((this.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATE || this.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATETIME) && this.DateCount.HasValue)
            {
                return this.DateCount.Value.ToString("0.########");
            }
            else if ((this.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || this.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO) && this.CheckedCount.HasValue)
            {
                return this.CheckedCount.Value.ToString("0.########");
            }
            else if (this.ValueCount.HasValue)
            {
                return this.ValueCount.Value.ToString("0.########");
            }
            return string.Empty;
        }
        public string GetValue(bool enabled, bool hasUom, string language)
        {
            if (this.Type == ISIConstants.CODE_MASTER_WFS_TYPE_QTY && (this.Qty.HasValue || !string.IsNullOrEmpty(this.Value)))
            {
                if (this.Qty.HasValue)
                {
                    if (!enabled && !string.IsNullOrEmpty(Currency) && hasUom)
                    {
                        return this.Qty.Value.ToString("C") + UOMDesc(language);
                    }
                    else
                    {
                        return this.Qty.Value.ToString("0.########");
                    }
                }
                else if (!string.IsNullOrEmpty(this.Value))
                {
                    return decimal.Parse(this.Value).ToString("0.########");
                }
                else
                {
                    return string.Empty;
                }
            }
            else if (DateValue.HasValue && this.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATE)
            {
                return DateValue.Value.ToString("yyyy-MM-dd");
            }
            else if (DateValue.HasValue && this.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATETIME)
            {
                return DateValue.Value.ToString("yyyy-MM-dd HH:mm");
            }
            else if (!enabled && string.IsNullOrEmpty(this.Value))
            {
                return ISIConstants.STRING_EMPTY;
            }
            else
            {
                return this.Value;
            }
        }

        #endregion
    }
}