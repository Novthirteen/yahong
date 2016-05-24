using System;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using System.Text;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskMgr
    {
        #region Customized Methods
        bool HasAttachmentPermission(string startedUser, string assignUser, string assignUpUser, bool isISIAdmin, bool isTaskFlowAdmin, bool isViewer, bool isAssigner, bool isCloser, bool isDeleteAttachment, string currentUser);
        bool HasPermission(TaskMstr task, bool isISIAdmin, bool isTaskFlowAdmin, bool isCloser, string currentUser);

        bool HasProcessPermission(string status, string taskCode, int? level, bool isWFAdmin, string currentUser);
        WFPermission ProcessPermission(string status, string taskCode, int? level, bool isWFAdmin, string currentUser);

        WFPermission PreProcessPermission(string status, string taskCode, int? preLevel, int? level, bool isWFAdmin, string currentUser);
        bool HasPermissionByComplete(string status, string type,
                                                string createUser, string submitUser, string startedUser,
                                                string ecUser, string assignUser, string assignUpUser,
                                                string closeUpUser, string[] closeUpLevel,
                                                bool isISIAdmin, bool isTaskFlowAdmin, bool isCloser, string currentUser);

        bool HasPermissionByProcess(string status, string startedUser, string startUpUser, string createUser, string submitUser, bool isISIAdmin, bool isTaskFlowAdmin, string currentUser);

        bool HasPermissionByClose(string status, string type, bool isWF, string createUser, string submitUser, bool isISIAdmin, bool isCloser, string currentUser);

        bool HasAssignPermission(TaskMstr task, bool isISIAdmin, bool isTaskFlowAdmin, bool isAssigner, string currentUser);

        bool HasAssignPermission(string createUser, string submitUser, string status, string type,
                                                bool isISIAdmin, bool isTaskFlowAdmin, bool isAssigner, string currentUser, string ecUser,
                                                string assignUser, string assignUpUser, bool isAutoAssign, bool isWF);

        string GetInvalidUser(string users, string userCode);

        string[] GetUserCodeName(string assignStartUser);

        void CreateTask(TaskMstr taskMstr, User user);

        //string GetUserName(string userCodes);
        TaskMstr SubmitTask(string code, string userCode);
        TaskMstr SubmitTask(string code, User user);

        TaskMstr SubmitTask(TaskMstr task, User user);

        string CancelTask(string code, User user);

        void DeleteTask(string code, User user);

        int BatchTask(IList<string> taskList, string create, bool isCancl, bool isComplete, string complete, bool isOpen, User user);
        int BatchTask(string type, IList<TaskMstr> taskList, string taskSubTypeCode, string projectSubType, User user);

        int BatchReplaceTask(IList<TaskMstr> taskList, string oldUser, string newUser, User user);

        int BatchCancelTask(IList<string> taskList, User user);
        int BatchSubmitTask(IList<string> taskList, User user);

        int BatchDeleteTask(IList<string> taskList, User user);
        int BatchCompleteTask(IList<string> taskList, User user);
        int BatchOpenTask(IList<string> taskList, User user);

        int BatchRejectTask(IList<string> taskList, User user);
        int BatchCloseTask(IList<string> taskList, User user);
        void ConfirmTask(string taskCode, User user);
        void ConfirmTask(TaskMstr task, User user);
        void ConfirmTask(string taskCode, DateTime planStartDate, DateTime planCompleteDate, string desc2, string expectedResults, User user);

        void CloseTask(string taskCode, User user);

        void CompleteTask(string taskCode, User user);

        void CompleteTask(string taskCode, string desc2, User user);

        string AssignTask(string taskCode, User user);
        void AssignTask(string taskCode, string backYards, string taskSubTypeCode, string[] assignStartUser, DateTime planStartDate, DateTime planCompleteDate, string desc2, string expectedResults, User user);

        //void ReassignTask(string taskCode, string assignStartUser, User user);

        void HelpTask(string taskCode, string helpUser, bool isRemindCreateUser,
                                                            bool isRemindAssignUser,
                                                            bool isRemindStartUser,
                                                            bool isRemindCommentUser,
                                                            bool isRemindAdmin, string helpContent, User user);

        string RejectTask(string taskCode, User user);

        string OpenTask(string taskCode, User user);
        //int? StartProcessInstance(string taskCode, DateTime effDate, User user);
        void SendUp();

        void CreateCommentDetail(CommentDetail commentDetail, string userCode, string userName);
        string CreateTaskStatus(TaskStatus taskStatus);
        string CreateTaskStatus(TaskStatus taskStatus, bool isComplete);

        void UpdateTaskStatus(TaskStatus taskStatus);

        int CountTask(string action, User user);

        void PlanRemind();

        void Remind(string subject, StringBuilder body);

        void Remind(string subject, StringBuilder body, string mailTo);

        void FiveSRemind();

        string FindEmail(string[] userCodes);

        string FindEmailByPermission(string[] permissionCodes);

        string FindUserNameByPermission(string[] permissionCodes);

        void SendTaskSubTypeReport();

        void CompleteRemind();
        TaskMstr ProcessByEmail(string taskCode, string wfsStatus, string approveDesc, bool isiAdmin, User user);
        TaskMstr ProcessNew(string taskCode, string wfsStatus, string approveDesc, string color, bool isiAdmin, User user);
        TaskMstr ProcessNew(string taskCode, string wfsStatus, string approveDesc, string color, IList<object> countersignList, bool isiAdmin, User user);
        void StartPercentRemind();

        void OpenRemind();

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskMgrE : com.Sconit.ISI.Service.ITaskMgr
    {
    }
}

#endregion Extend Interface