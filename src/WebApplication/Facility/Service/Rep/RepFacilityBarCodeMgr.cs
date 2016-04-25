using com.Sconit.Service.Ext.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MasterData;
using Castle.Services.Transaction;
using NPOI.HSSF.UserModel;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity;
using com.Sconit.Facility.Entity;
using com.Sconit.Service.Report;

namespace com.Sconit.Facility.Service.Report.Impl
{
    /**
     * 
     * 原材料条码
     * 
     */
    [Transactional]
    public class RepFacilityBarCodeMgr : RepTemplate2
    {
        public override string reportTemplateFolder { get; set; }

        public RepFacilityBarCodeMgr()
        {

            //明细部分的行数
            this.pageDetailRowCount = 7;
            //列数   1起始
            this.columnCount = 3;
            //报表头的行数  1起始
            this.leftColumnHeadCount = 1;
            //报表尾的行数  1起始
            this.bottomRowCount = 0;

            this.headRowCount = 0;

        }


        /**
         * 需要拷贝的数据与合并单元格操作
         * 
         * Param pageIndex 页号
         */
        public override void CopyPageValues(int pageIndex)
        {
            this.SetMergedRegion(pageIndex, 1, 1, 1, 2);
            this.SetMergedRegion(pageIndex, 2, 1, 2, 2);
            //this.SetMergedRegion(pageIndex, 3, 1, 3, 3);
            //this.SetMergedRegion(pageIndex, 4, 1, 4, 3);
            //this.SetMergedRegion(pageIndex, 5, 1, 5, 3);
            //this.SetMergedRegion(pageIndex, 9, 1, 9, 3);

            this.CopyCell(pageIndex, 3, 1, "B4");
            this.CopyCell(pageIndex, 4, 1, "B5");
            this.CopyCell(pageIndex, 5, 1, "B6");
            //this.CopyCell(pageIndex, 0, 2, "C1");
            //this.CopyCell(pageIndex, 6, 2, "C7");
            //this.CopyCell(pageIndex, 7, 2, "C8");
            //this.CopyCell(pageIndex, 8, 2, "C9");
            //this.CopyCell(pageIndex, 10, 2, "C11");

        }

        /**
         * 填充报表
         * 
         * Param list [0]huDetailList
         */
        [Transaction(TransactionMode.Requires)]
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                IList<FacilityMaster> facilityMasterList = null;
                if (list[0] is FacilityMaster)
                {
                    facilityMasterList = new List<FacilityMaster>();
                    facilityMasterList.Add((FacilityMaster)list[0]);
                }
                else if (list[0] is IList<FacilityMaster>)
                {
                    facilityMasterList = (IList<FacilityMaster>)list[0];
                }
                else
                {
                    return false;
                }

                string userName = "";
                if (list.Count == 2)
                {
                    userName = (string)list[1];
                }

                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;

                //this.sheet.DisplayGuts = false;

                int count = facilityMasterList.Count;


                if (count == 0) return false;

                this.barCodeFontName = this.GetBarcodeFontName(2, 1);

                //加页删页
                this.CopyPage(count);


                int pageIndex = 1;

                foreach (FacilityMaster facilityMaster in facilityMasterList)
                {
                    this.SetRowCell(pageIndex, 1, 1, facilityMaster.Name);

                    string barCode = Utility.BarcodeHelper.GetBarcodeStr(facilityMaster.FCID, this.barCodeFontName);
                    this.SetRowCell(pageIndex, 2, 1, barCode);
                    this.SetRowCell(pageIndex, 3, 2, facilityMaster.FCID);

                    this.SetRowCell(pageIndex, 4, 2, facilityMaster.SerialNo);

                    this.SetRowCell(pageIndex, 5, 2, facilityMaster.CreateDate.ToString("yyyy-MM-dd"));

                    //this.SetRowCell(pageIndex, 1, 1, facilityMaster.Name);

                    pageIndex++;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }

}




#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Report.Impl
{
    /**
     * 
     * 原材料条码
     * 
     */
    [Transactional]
    public partial class RepFacilityBarCodeMgrE : com.Sconit.Facility.Service.Report.Impl.RepFacilityBarCodeMgr, IReportBaseMgrE
    {



    }

}

#endregion
