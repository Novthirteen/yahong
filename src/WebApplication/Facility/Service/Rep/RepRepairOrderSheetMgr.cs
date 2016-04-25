using com.Sconit.Service.Ext.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MasterData;
using Castle.Services.Transaction;
using com.Sconit.Utility;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity;
using com.Sconit.Facility.Entity;
using com.Sconit.Service.Report;

namespace com.Sconit.Facility.Service.Report.Impl
{
    [Transactional]
    public class RepRepairOrderSheetMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }

        public IMiscOrderMgrE miscOrderMgrE { get; set; }
        public ICodeMasterMgrE codeMasterMgrE { get; set; }

        public RepRepairOrderSheetMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 0;
            //列数   1起始
            this.columnCount = 6;
            //报表头的行数  1起始
            this.headRowCount = 0;
            //报表尾的行数  1起始
            this.bottomRowCount = 0;
        }

        /**
         * 填充报表
         * 
         * Param list [0]OrderHead
         * Param list [0]IList<OrderDetail>           
         */
        [Transaction(TransactionMode.Requires)]
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {

                if (list == null) return false;

                RepairOrder repairOrder = (RepairOrder)(list[0]);

                if (repairOrder == null)
                {
                    return false;
                }

                this.FillHead(repairOrder);

                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        /*
         * 填充报表头
         * 
         * Param repack 报验单头对象
         */

        private void FillHead(RepairOrder repairOrder)
        {

            //单号：
            this.SetRowCell(3, 5, repairOrder.OrderNo);
            //报修部门：
            this.SetRowCell(4, 1, repairOrder.SubmitDept);
            //报修时间：
            this.SetRowCell(4, 3, repairOrder.SubmitTime.ToString("yyyy-MM-dd HH:mm"));
            //设备编号：
            this.SetRowCell(5, 1, repairOrder.FCID);
            //设备名称：
            this.SetRowCell(5, 3, repairOrder.FCName);
            //内部资产：
            this.SetRowCell(5, 5, repairOrder.AssetNo);
            //停机开始：
            this.SetRowCell(6, 1, repairOrder.HaltStartTime.ToString("yyyy-MM-dd HH:mm"));
            //停机结束：
            if (repairOrder.HaltEndTime.HasValue)
            { 
                this.SetRowCell(6, 3, ((DateTime)repairOrder.HaltEndTime).ToString("yyyy-MM-dd HH:mm"));
            }
            //总计：
            var totalMinute="";
            if (repairOrder.HaltEndTime.HasValue)
            {
                totalMinute=((DateTime)repairOrder.HaltEndTime).Subtract(repairOrder.HaltStartTime).TotalMinutes.ToString();
            }
            this.SetRowCell(6, 5, totalMinute+"(分钟)");
            //报修人：
            this.SetRowCell(7, 3, repairOrder.SubmitUserName);
            //验收人：
            this.SetRowCell(7, 5, repairOrder.OperateUserName);
            //故障现象描述：
            this.SetRowCell(8, 0, repairOrder.FaultDescription);
            //维修人：
            this.SetRowCell(9, 2, repairOrder.RepairUserName);
            //维修具体步骤：
            this.SetRowCell(10, 0, repairOrder.RepairDescription);
            //故障原因：
            this.SetRowCell(12, 0, repairOrder.HaltReason);
            //维修开始时间：
            if (repairOrder.RepairStartTime.HasValue)
            { 
                this.SetRowCell(13, 3, ((DateTime)repairOrder.RepairStartTime).ToString("yyyy-MM-dd HH:mm"));
            }
            //维修结束时间：
            if (repairOrder.RepairEndTime.HasValue)
            { 
                this.SetRowCell(13, 5, ((DateTime)repairOrder.RepairEndTime).ToString("yyyy-MM-dd HH:mm"));
            }
            //领用更换备件：
            this.SetRowCell(14, 0, repairOrder.Items);
            //工程师主管：
            this.SetRowCell(15, 2, repairOrder.SuggestionUserName);
            //工程师主管意见：
            this.SetRowCell(16, 0, repairOrder.Suggestion);
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //this.CopyCell(pageIndex, 48, 2, "C49");
            //this.CopyCell(pageIndex, 49, 0, "A50");
        }

        //public override IList<object> GetDataList(string code)
        //{
        //    IList<object> list = new List<object>();
        //    MiscOrder miscOrder = miscOrderMgrE.ReLoadMiscOrder(code);
        //    if (miscOrder != null && miscOrder.MiscOrderDetails != null && miscOrder.MiscOrderDetails.Count > 0)
        //    {
        //        list.Add(miscOrder);
        //        list.Add(miscOrder.MiscOrderDetails);
        //    }
        //    return list;
        //}
    }
}

#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Report.Impl
{
    [Transactional]
    public partial class RepRepairOrderSheetMgrE : com.Sconit.Facility.Service.Report.Impl.RepRepairOrderSheetMgr, IReportBaseMgrE
    {
        

    }
}

#endregion
