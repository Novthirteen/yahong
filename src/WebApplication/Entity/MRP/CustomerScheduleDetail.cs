using System;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP
{
    [Serializable]
    public class CustomerScheduleDetail : CustomerScheduleDetailBase
    {
        #region Non O/R Mapping Properties

        public int Sequence { get; set; }

        public DateTime EndDate { get; set; }

        #endregion
    }

    [Serializable]
    public class ScheduleBody
    {
        #region  Properties

        public Int32 Seq { get; set; }
        public string Item { get; set; }
        public string Uom { get; set; }
        public Decimal UnitCount { get; set; }
        public string Location { get; set; }
        public string ItemDescription { get; set; }
        public string ItemReference { get; set; }
        //public string SourceType { get; set; }
        //public string PeriodType { get; set; }

        public decimal Qty0 { get; set; }
        public decimal Qty1 { get; set; }
        public decimal Qty2 { get; set; }
        public decimal Qty3 { get; set; }
        public decimal Qty4 { get; set; }
        public decimal Qty5 { get; set; }
        public decimal Qty6 { get; set; }
        public decimal Qty7 { get; set; }
        public decimal Qty8 { get; set; }
        public decimal Qty9 { get; set; }
        public decimal Qty10 { get; set; }
        public decimal Qty11 { get; set; }
        public decimal Qty12 { get; set; }
        public decimal Qty13 { get; set; }
        public decimal Qty14 { get; set; }
        public decimal Qty15 { get; set; }
        public decimal Qty16 { get; set; }
        public decimal Qty17 { get; set; }
        public decimal Qty18 { get; set; }
        public decimal Qty19 { get; set; }
        public decimal Qty20 { get; set; }
        public decimal Qty21 { get; set; }
        public decimal Qty22 { get; set; }
        public decimal Qty23 { get; set; }
        public decimal Qty24 { get; set; }
        public decimal Qty25 { get; set; }
        public decimal Qty26 { get; set; }
        public decimal Qty27 { get; set; }
        public decimal Qty28 { get; set; }
        public decimal Qty29 { get; set; }
        public decimal Qty30 { get; set; }
        public decimal Qty31 { get; set; }
        public decimal Qty32 { get; set; }
        public decimal Qty33 { get; set; }
        public decimal Qty34 { get; set; }
        public decimal Qty35 { get; set; }
        public decimal Qty36 { get; set; }
        public decimal Qty37 { get; set; }
        public decimal Qty38 { get; set; }
        public decimal Qty39 { get; set; }
        public decimal Qty40 { get; set; }

        public decimal ActQty0 { get; set; }
        public decimal ActQty1 { get; set; }
        public decimal ActQty2 { get; set; }
        public decimal ActQty3 { get; set; }
        public decimal ActQty4 { get; set; }
        public decimal ActQty5 { get; set; }
        public decimal ActQty6 { get; set; }
        public decimal ActQty7 { get; set; }
        public decimal ActQty8 { get; set; }
        public decimal ActQty9 { get; set; }
        public decimal ActQty10 { get; set; }
        public decimal ActQty11 { get; set; }
        public decimal ActQty12 { get; set; }
        public decimal ActQty13 { get; set; }
        public decimal ActQty14 { get; set; }
        public decimal ActQty15 { get; set; }
        public decimal ActQty16 { get; set; }
        public decimal ActQty17 { get; set; }
        public decimal ActQty18 { get; set; }
        public decimal ActQty19 { get; set; }
        public decimal ActQty20 { get; set; }
        public decimal ActQty21 { get; set; }
        public decimal ActQty22 { get; set; }
        public decimal ActQty23 { get; set; }
        public decimal ActQty24 { get; set; }
        public decimal ActQty25 { get; set; }
        public decimal ActQty26 { get; set; }
        public decimal ActQty27 { get; set; }
        public decimal ActQty28 { get; set; }
        public decimal ActQty29 { get; set; }
        public decimal ActQty30 { get; set; }
        public decimal ActQty31 { get; set; }
        public decimal ActQty32 { get; set; }
        public decimal ActQty33 { get; set; }
        public decimal ActQty34 { get; set; }
        public decimal ActQty35 { get; set; }
        public decimal ActQty36 { get; set; }
        public decimal ActQty37 { get; set; }
        public decimal ActQty38 { get; set; }
        public decimal ActQty39 { get; set; }
        public decimal ActQty40 { get; set; }

        public decimal DisconActQty0 { get; set; }
        public decimal DisconActQty1 { get; set; }
        public decimal DisconActQty2 { get; set; }
        public decimal DisconActQty3 { get; set; }
        public decimal DisconActQty4 { get; set; }
        public decimal DisconActQty5 { get; set; }
        public decimal DisconActQty6 { get; set; }
        public decimal DisconActQty7 { get; set; }
        public decimal DisconActQty8 { get; set; }
        public decimal DisconActQty9 { get; set; }
        public decimal DisconActQty10 { get; set; }
        public decimal DisconActQty11 { get; set; }
        public decimal DisconActQty12 { get; set; }
        public decimal DisconActQty13 { get; set; }
        public decimal DisconActQty14 { get; set; }
        public decimal DisconActQty15 { get; set; }
        public decimal DisconActQty16 { get; set; }
        public decimal DisconActQty17 { get; set; }
        public decimal DisconActQty18 { get; set; }
        public decimal DisconActQty19 { get; set; }
        public decimal DisconActQty20 { get; set; }
        public decimal DisconActQty21 { get; set; }
        public decimal DisconActQty22 { get; set; }
        public decimal DisconActQty23 { get; set; }
        public decimal DisconActQty24 { get; set; }
        public decimal DisconActQty25 { get; set; }
        public decimal DisconActQty26 { get; set; }
        public decimal DisconActQty27 { get; set; }
        public decimal DisconActQty28 { get; set; }
        public decimal DisconActQty29 { get; set; }
        public decimal DisconActQty30 { get; set; }
        public decimal DisconActQty31 { get; set; }
        public decimal DisconActQty32 { get; set; }
        public decimal DisconActQty33 { get; set; }
        public decimal DisconActQty34 { get; set; }
        public decimal DisconActQty35 { get; set; }
        public decimal DisconActQty36 { get; set; }
        public decimal DisconActQty37 { get; set; }
        public decimal DisconActQty38 { get; set; }
        public decimal DisconActQty39 { get; set; }
        public decimal DisconActQty40 { get; set; }

        public string DisplayQty0
        {
            get
            {
                return Qty0.ToString("#,##0.##") + "(" + ActQty0.ToString("#,##0.##") + " | " + DisconActQty0.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty1
        {
            get
            {
                return Qty1.ToString("#,##0.##") + "(" + ActQty1.ToString("#,##0.##") + " | " + DisconActQty1.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty2
        {
            get
            {
                return Qty2.ToString("#,##0.##") + "(" + ActQty2.ToString("#,##0.##") + " | " + DisconActQty2.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty3
        {
            get
            {
                return Qty3.ToString("#,##0.##") + "(" + ActQty3.ToString("#,##0.##") + " | " + DisconActQty3.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty4
        {
            get
            {
                return Qty4.ToString("#,##0.##") + "(" + ActQty4.ToString("#,##0.##") + " | " + DisconActQty4.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty5
        {
            get
            {
                return Qty5.ToString("#,##0.##") + "(" + ActQty5.ToString("#,##0.##") + " | " + DisconActQty5.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty6
        {
            get
            {
                return Qty6.ToString("#,##0.##") + "(" + ActQty6.ToString("#,##0.##") + " | " + DisconActQty6.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty7
        {
            get
            {
                return Qty7.ToString("#,##0.##") + "(" + ActQty7.ToString("#,##0.##") + " | " + DisconActQty7.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty8
        {
            get
            {
                return Qty8.ToString("#,##0.##") + "(" + ActQty8.ToString("#,##0.##") + " | " + DisconActQty8.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty9
        {
            get
            {
                return Qty9.ToString("#,##0.##") + "(" + ActQty9.ToString("#,##0.##") + " | " + DisconActQty9.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty10
        {
            get
            {
                return Qty10.ToString("#,##0.##") + "(" + ActQty10.ToString("#,##0.##") + " | " + DisconActQty10.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty11
        {
            get
            {
                return Qty11.ToString("#,##0.##") + "(" + ActQty11.ToString("#,##0.##") + " | " + DisconActQty11.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty12
        {
            get
            {
                return Qty12.ToString("#,##0.##") + "(" + ActQty12.ToString("#,##0.##") + " | " + DisconActQty12.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty13
        {
            get
            {
                return Qty13.ToString("#,##0.##") + "(" + ActQty13.ToString("#,##0.##") + " | " + DisconActQty13.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty14
        {
            get
            {
                return Qty14.ToString("#,##0.##") + "(" + ActQty14.ToString("#,##0.##") + " | " + DisconActQty14.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty15
        {
            get
            {
                return Qty15.ToString("#,##0.##") + "(" + ActQty15.ToString("#,##0.##") + " | " + DisconActQty15.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty16
        {
            get
            {
                return Qty16.ToString("#,##0.##") + "(" + ActQty16.ToString("#,##0.##") + " | " + DisconActQty16.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty17
        {
            get
            {
                return Qty17.ToString("#,##0.##") + "(" + ActQty17.ToString("#,##0.##") + " | " + DisconActQty17.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty18
        {
            get
            {
                return Qty18.ToString("#,##0.##") + "(" + ActQty18.ToString("#,##0.##") + " | " + DisconActQty18.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty19
        {
            get
            {
                return Qty19.ToString("#,##0.##") + "(" + ActQty19.ToString("#,##0.##") + " | " + DisconActQty19.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty20
        {
            get
            {
                return Qty20.ToString("#,##0.##") + "(" + ActQty20.ToString("#,##0.##") + " | " + DisconActQty20.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty21
        {
            get
            {
                return Qty21.ToString("#,##0.##") + "(" + ActQty21.ToString("#,##0.##") + " | " + DisconActQty21.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty22
        {
            get
            {
                return Qty22.ToString("#,##0.##") + "(" + ActQty22.ToString("#,##0.##") + " | " + DisconActQty22.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty23
        {
            get
            {
                return Qty23.ToString("#,##0.##") + "(" + ActQty23.ToString("#,##0.##") + " | " + DisconActQty23.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty24
        {
            get
            {
                return Qty24.ToString("#,##0.##") + "(" + ActQty24.ToString("#,##0.##") + " | " + DisconActQty24.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty25
        {
            get
            {
                return Qty25.ToString("#,##0.##") + "(" + ActQty25.ToString("#,##0.##") + " | " + DisconActQty25.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty26
        {
            get
            {
                return Qty26.ToString("#,##0.##") + "(" + ActQty26.ToString("#,##0.##") + " | " + DisconActQty26.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty27
        {
            get
            {
                return Qty27.ToString("#,##0.##") + "(" + ActQty27.ToString("#,##0.##") + " | " + DisconActQty27.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty28
        {
            get
            {
                return Qty28.ToString("#,##0.##") + "(" + ActQty28.ToString("#,##0.##") + " | " + DisconActQty28.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty29
        {
            get
            {
                return Qty29.ToString("#,##0.##") + "(" + ActQty29.ToString("#,##0.##") + " | " + DisconActQty29.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty30
        {
            get
            {
                return Qty30.ToString("#,##0.##") + "(" + ActQty30.ToString("#,##0.##") + " | " + DisconActQty30.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty31
        {
            get
            {
                return Qty31.ToString("#,##0.##") + "(" + ActQty31.ToString("#,##0.##") + " | " + DisconActQty31.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty32
        {
            get
            {
                return Qty32.ToString("#,##0.##") + "(" + ActQty32.ToString("#,##0.##") + " | " + DisconActQty32.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty33
        {
            get
            {
                return Qty33.ToString("#,##0.##") + "(" + ActQty33.ToString("#,##0.##") + " | " + DisconActQty33.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty34
        {
            get
            {
                return Qty34.ToString("#,##0.##") + "(" + ActQty34.ToString("#,##0.##") + " | " + DisconActQty34.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty35
        {
            get
            {
                return Qty35.ToString("#,##0.##") + "(" + ActQty35.ToString("#,##0.##") + " | " + DisconActQty35.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty36
        {
            get
            {
                return Qty36.ToString("#,##0.##") + "(" + ActQty36.ToString("#,##0.##") + " | " + DisconActQty36.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty37
        {
            get
            {
                return Qty37.ToString("#,##0.##") + "(" + ActQty37.ToString("#,##0.##") + " | " + DisconActQty37.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty38
        {
            get
            {
                return Qty38.ToString("#,##0.##") + "(" + ActQty38.ToString("#,##0.##") + " | " + DisconActQty38.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty39
        {
            get
            {
                return Qty39.ToString("#,##0.##") + "(" + ActQty39.ToString("#,##0.##") + " | " + DisconActQty39.ToString("#,##0.##") + ")";
            }
        }
        public string DisplayQty40
        {
            get
            {
                return Qty40.ToString("#,##0.##") + "(" + ActQty40.ToString("#,##0.##") + " | " + DisconActQty40.ToString("#,##0.##") + ")";
            }
        }
        #endregion
        public decimal TotalQty
        {
            get
            {
                return this.Qty0 + this.Qty1 + this.Qty2 + this.Qty3 + this.Qty4 + this.Qty5 + this.Qty6 + this.Qty7 + this.Qty8 + this.Qty9
                    + this.Qty10 + this.Qty11 + this.Qty12 + this.Qty13 + this.Qty14 + this.Qty15 + this.Qty16 + this.Qty17 + this.Qty18 + this.Qty19
                    + this.Qty20 + this.Qty21 + this.Qty22 + this.Qty23 + this.Qty24 + this.Qty25 + this.Qty26 + this.Qty27 + this.Qty28 + this.Qty29
                    + this.Qty30 + this.Qty31 + this.Qty32 + this.Qty33 + this.Qty34 + this.Qty35 + this.Qty36 + this.Qty37 + this.Qty38 + this.Qty39
                    + this.Qty40;
            }
        }
    }

    public class ScheduleHead
    {
        public string Flow { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public DateTime? LastDateFrom { get; set; }
        public DateTime? LastDateTo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime StartDate { get; set; }
        public string SourceType { get; set; }
        public string PeriodType { get; set; }
        private string _dateHead;
        public string DateHead
        {
            get
            {
                if (Type == BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_DAY)
                {
                    _dateHead = Type + "*" + DateFrom.ToString("ddd") + "*" + DateFrom.ToString("MMdd");
                }
                else if (Type == BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_MONTH
                    || Type == BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_WEEK)
                {
                    _dateHead = Type + "*" + DateFrom.ToString("MMdd") + "-" + DateTo.ToString("MMdd");
                }
                else
                {
                    _dateHead = DateTo.ToString("yyyy-MM-dd");
                }
                return _dateHead;
            }
        }
    }

    public class ScheduleView
    {
        public IList<ScheduleHead> ScheduleHeads { get; set; }
        public IList<ScheduleBody> ScheduleBodys { get; set; }
    }
}