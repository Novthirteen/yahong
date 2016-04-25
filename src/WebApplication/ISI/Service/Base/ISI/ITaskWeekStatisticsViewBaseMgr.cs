using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskWeekStatisticsViewBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateTaskWeekStatisticsView(TaskWeekStatisticsView entity);

        TaskWeekStatisticsView LoadTaskWeekStatisticsView(String code);

        IList<TaskWeekStatisticsView> GetAllTaskWeekStatisticsView();
    
        void UpdateTaskWeekStatisticsView(TaskWeekStatisticsView entity);

        void DeleteTaskWeekStatisticsView(String code);
    
        void DeleteTaskWeekStatisticsView(TaskWeekStatisticsView entity);
    
        void DeleteTaskWeekStatisticsView(IList<String> pkList);
    
        void DeleteTaskWeekStatisticsView(IList<TaskWeekStatisticsView> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
