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
using com.Sconit.Service.Report;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Ext;
using NPOI.SS.UserModel;
using com.Sconit.ISI.Service.Util;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using com.Sconit.ISI.Entity.Util;

namespace com.Sconit.ISI.Service.Report.Impl
{
    [Transactional]
    public class RepPlanMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }

        public ITaskSubTypeMgrE taskSubTypeMgrE { get; set; }
        public ICodeMasterMgrE codeMasterMgrE { get; set; }
        public ITaskMstrMgrE taskMstrMgrE { get; set; }
        public ITaskStatusMgrE taskStatusMgrE { get; set; }

        public RepPlanMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 35;
            //列数   1起始
            this.columnCount = 10;
            //报表头的行数  1起始
            this.headRowCount = 3;
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
                if (list == null || list.Count < 1) return false;

                IList<TaskStatusView> taskViews = (IList<TaskStatusView>)(list[0]);
                string period = (string)(list[1]);
                string taskSubTypeCode = string.Empty;
                string type = string.Empty;
                if (list.Count == 3)
                {
                    taskSubTypeCode = (string)(list[2]);
                }
                if (list.Count == 4)
                {
                    type = (string)(list[3]);
                }
                if (taskViews == null || taskViews.Count == 0)
                {
                    return false;
                }

                //this.CopyPage(taskViews.Count);

                this.FillHead(type, taskSubTypeCode, period);


                int pageIndex = 1;
                int rowIndex = 0;

                int i = 1;

                string startUsers = string.Join(";", taskViews.Select(t => t.StartedUser).ToArray<string>());
                IDictionary<string, string> startUserDic = null;
                if (!string.IsNullOrEmpty(startUsers))
                {
                    startUserDic = taskMstrMgrE.GetUser(startUsers);
                }

                foreach (TaskStatusView taskView in taskViews)
                {

                    if (rowIndex != 0)
                    {
                        /*
                        for (int j = 0; j < this.columnCount; j++)
                        {
                            ICell cell = this.GetCell(this.GetRowIndexAbsolute(pageIndex, rowIndex), j);
                            short color = cell.CellStyle.FillForegroundColor;
                            FillPatternType type = cell.CellStyle.FillPattern;
                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.CloneStyleFrom(this.GetCellStyle(this.GetRowIndexAbsolute(pageIndex, 0), j));
                            cell.CellStyle.FillForegroundColor = color;
                            cell.CellStyle.FillPattern = type;
                            //CellStyle cellStyle = this.GetCellStyle(this.GetRowIndexAbsolute(pageIndex, rowIndex), i);
                            //cellStyle.BorderTop = NPOI.SS.UserModel.CellBorderType.THIN;
                            //this.GetCell(this.GetRowIndexAbsolute(pageIndex, rowIndex), i).CellStyle.CloneStyleFrom(cellStyle);
                        }
                        */
                        //this.CopyRowStyle(this.GetRowIndexAbsolute(pageIndex, 0), this.GetRowIndexAbsolute(pageIndex, rowIndex), this.GetColumnIndexAbsolute(1, 0), this.GetColumnIndexAbsolute(pageIndex, columnCount));
                        IRow rowFrom = this.GetRow(this.GetRowIndexAbsolute(pageIndex, 0));
                        IRow rowTo = this.GetRow(this.GetRowIndexAbsolute(pageIndex, rowIndex));
                        for (int j = this.GetColumnIndexAbsolute(pageIndex, 0); j < this.GetColumnIndexAbsolute(pageIndex, columnCount); j++)
                        {
                            this.CopyCellStyle(this.GetCell(rowFrom.RowNum, j), this.GetCell(rowTo.RowNum, j));
                        }
                    }


                    // No.	
                    this.SetRowCell(pageIndex, rowIndex, 0, "" + i++);

                    //"任务名称Task"
                    StringBuilder cell1 = new StringBuilder();
                    cell1.Append(taskView.TaskCode);
                    if (!string.IsNullOrEmpty(taskView.Subject))
                    {
                        cell1.Append(ISIConstants.TEXT_SEPRATOR);
                        cell1.Append(taskView.Subject);
                    }
                    if (!string.IsNullOrEmpty(taskView.TaskAddress))
                    {
                        cell1.Append(ISIConstants.TEXT_SEPRATOR);
                        cell1.Append(taskView.TaskAddress);
                    }
                    if (string.IsNullOrEmpty(taskSubTypeCode))
                    {
                        cell1.Append(ISIConstants.TEXT_SEPRATOR);
                        cell1.Append(taskView.TaskSubType);
                    }
                    if (!string.IsNullOrEmpty(taskView.Phase))
                    {
                        cell1.Append(ISIConstants.TEXT_SEPRATOR);
                        cell1.Append(taskView.Phase + " " + taskView.Seq);
                    }
                    this.SetRowCell(pageIndex, rowIndex, 1, cell1.ToString());

                    //参考号"工作内容Task Content"
                    StringBuilder desc = new StringBuilder();
                    if (!string.IsNullOrEmpty(taskView.Desc1))
                    {
                        desc.Append(taskView.Desc1);
                    }
                    if (!string.IsNullOrEmpty(taskView.Desc2))
                    {
                        desc.Append(ISIConstants.TEXT_SEPRATOR + "补充描述：" + taskView.Desc2.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                    }
                    this.SetRowCell(pageIndex, rowIndex, 2, desc.ToString());

                    //"任务目标TaskObject"
                    this.SetRowCell(pageIndex, rowIndex, 3, taskView.ExpectedResults);

                    //"优先级别Priority"
                    //this.SetRowCell(pageIndex, rowIndex, 3, codeMasterMgrE.LoadCodeMaster(ISIConstants.CODE_MASTER_ISI_PRIORITY, taskView.Priority).Description);

                    //"执行人Responsible"
                    if (startUserDic != null && startUserDic.Count > 0 && !string.IsNullOrEmpty(taskView.StartedUser))
                    {
                        string[] userCodes = taskView.StartedUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                        if (userCodes != null && userCodes.Length > 0)
                        {
                            string users = string.Empty;
                            foreach (string userCode in userCodes)
                            {
                                if (startUserDic.Keys.Contains(userCode))
                                {
                                    if (!string.IsNullOrEmpty(users))
                                    {
                                        users += ISIConstants.TEXT_SEPRATOR;
                                    }
                                    users += startUserDic[userCode].Trim();
                                }
                            }
                            if (!string.IsNullOrEmpty(users))
                            {
                                this.SetRowCell(pageIndex, rowIndex, 4, users);
                            }
                        }

                    }
                    if (taskView.PlanStartDate.HasValue)
                    {
                        //"开始时间Start"
                        this.SetRowCell(pageIndex, rowIndex, 5, taskView.PlanStartDate.Value.ToString("yyyy-MM-dd"));
                    }
                    if (taskView.PlanCompleteDate.HasValue)
                    {
                        //"完成时间Finished"
                        this.SetRowCell(pageIndex, rowIndex, 6, taskView.PlanCompleteDate.Value.ToString("yyyy-MM-dd"));
                    }

                    if (!string.IsNullOrEmpty(taskView.Color))
                    {
                        ICellStyle cellStyle = this.GetCellStyle(this.GetRowIndexAbsolute(pageIndex, rowIndex), 7);
                        cellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                        if (taskView.Color == ISIConstants.CODE_MASTER_ISI_FLAG_RED)
                        {
                            cellStyle.FillForegroundColor = HSSFColor.RED.index;
                        }
                        else if (taskView.Color == ISIConstants.CODE_MASTER_ISI_FLAG_GREEN)
                        {
                            cellStyle.FillForegroundColor = HSSFColor.GREEN.index;
                        }
                        else if (taskView.Color == ISIConstants.CODE_MASTER_ISI_FLAG_YELLOW)
                        {
                            cellStyle.FillForegroundColor = HSSFColor.YELLOW.index;
                        }
                        this.GetCell(this.GetRowIndexAbsolute(pageIndex, rowIndex), 7).CellStyle = workbook.CreateCellStyle();
                        this.GetCell(this.GetRowIndexAbsolute(pageIndex, rowIndex), 7).CellStyle.CloneStyleFrom(cellStyle);
                    }

                    //"当前状态Current status"
                    this.SetRowCell(pageIndex, rowIndex, 7, taskView.Flag);

                    //"当前状态工作描述Current status working description"	
                    if (!string.IsNullOrEmpty(taskView.StatusDesc) && taskView.StatusDate.HasValue)
                    {
                        var taskStatus = this.taskStatusMgrE.GetTaskStatus(taskView.TaskCode, 0, 5);
                        StringBuilder statusStr = new StringBuilder();
                        if (taskStatus != null && taskStatus.Count() > 0)
                        {
                            foreach (var status in taskStatus)
                            {
                                if (statusStr.Length != 0)
                                {
                                    statusStr.Append("\n");
                                }
                                statusStr.Append(status.LastModifyUserNm + "(" + status.LastModifyDate.ToString("yyyy-MM-dd HH:mm") + "):" + status.Desc);
                            }
                            this.SetRowCell(pageIndex, rowIndex, 8, statusStr.ToString());
                        }
                    }
                    //"最新评论Comment"
                    if (!string.IsNullOrEmpty(taskView.Comment) && taskView.CommentCreateDate.HasValue)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 9, taskView.CommentCreateUserNm + "(" + taskView.CommentCreateDate.Value.ToString("yyyy-MM-dd HH:mm") + "):" + taskView.Comment);
                    }

                    rowIndex++;
                }

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
        private void FillHead(string type, string taskSubTypeCode, string period)
        {
            if (type == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
            {
                this.SetRowCell(0, 0, "项目问题清单\nProject Issues List");
                this.SetRowCell(2, 1, "问题\nIssue");
                this.SetRowCell(2, 2, "问题描述\nIssue Description");
                this.SetRowCell(2, 3, "行动措施\nAction steps");
            }
            else if (type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
            {
                this.SetRowCell(0, 0, "工程更改清单\nEngineering Change List");
                this.SetRowCell(2, 1, "工程更改\nEngineering Change");
                this.SetRowCell(2, 2, "更改描述\nChange Description");
                this.SetRowCell(2, 3, "更改结果\nnChange The Results");
            }
            if (!string.IsNullOrEmpty(taskSubTypeCode))
            {
                TaskSubType taskSubType = taskSubTypeMgrE.LoadTaskSubType(taskSubTypeCode);
                if (type == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || type == ISIConstants.ISI_TASK_TYPE_PROJECT || type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
                {
                     this.SetRowCell(1, 0, "项目Project:");
                }
                else
                {
                    this.SetRowCell(1, 0, "任务分类TaskSubType:");
                }
                this.SetRowCell(1, 2, taskSubType.Description);
            }
            if (!string.IsNullOrEmpty(period) && period.Length > 5)
            {
                this.SetRowCell(1, 5, period);
            }
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {

        }

    }
}

#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Report.Impl
{
    [Transactional]
    public partial class RepPlanMgrE : com.Sconit.ISI.Service.Report.Impl.RepPlanMgr, IReportBaseMgrE
    {


    }
}

#endregion
