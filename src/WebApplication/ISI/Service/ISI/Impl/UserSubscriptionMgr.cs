using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using NHibernate.SqlCommand;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Service.Ext;
using System.Data;
using com.Sconit.Utility;
using NHibernate.Transform;
using System.Data.SqlClient;
using NHibernate.Type;
using NHibernate;
using System.Net.Mail;
using com.Sconit.Entity;
using System.IO;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class UserSubscriptionMgr : UserSubscriptionBaseMgr, IUserSubscriptionMgr
    {
        public virtual string appDataFolder { get; set; }

        public static IList<UserSub> cachedAllUser;
        public DateTime lastModifyDate;
        public ICodeMasterMgrE codeMasterMgrE { get; set; }
        public ISmtpMgrE smtpMgrE { get; set; }
        public ILanguageMgrE languageMgrE { get; set; }
        public ITaskApplyMgrE taskApplyMgrE { get; set; }
        public IEmppMgrE emppMgrE { get; set; }
        public ISqlHelperMgrE sqlHelperMgrE { get; set; }
        public ITaskDetailMgrE taskDetailMgrE { get; set; }
        public ITaskSubTypeMgrE taskSubTypeMgrE { get; set; }
        public IAttachmentDetailMgrE attachmentDetailMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");
        #region Customized Methods
        [Transaction(TransactionMode.Unspecified)]
        public IList<UserSub> GetCacheAllUser()
        {
            if (cachedAllUser == null)
            {
                cachedAllUser = GetAllUser();
            }
            else
            {
                //检查Item大小是否发生变化
                DetachedCriteria criteria = DetachedCriteria.For(typeof(User));
                criteria.SetProjection(Projections.ProjectionList().Add(Projections.Max("LastModifyDate")));
                IList<DateTime> count = this.criteriaMgrE.FindAll<DateTime>(criteria);

                if (lastModifyDate == null || count[0] > lastModifyDate)
                {
                    cachedAllUser = GetAllUser();
                }
            }

            return cachedAllUser;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<UserSub> GetAllUser()
        {
            lastModifyDate = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append("select USR_Code Code,isnull(USR_FirstName,'') + ' ' + isnull(USR_LastName,'') Name ");
            sql.Append(",Dept2,JobNo,Dept Department ");
            sql.Append("from ACC_User");
            DataSet userDS = sqlHelperMgrE.GetDatasetBySql(sql.ToString());
            var userList = IListHelper.DataTableToList<UserSub>(userDS.Tables[0]);
            return userList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<UserSubView> GetUserAllTaskSubType(string userCode)
        {
            IList<TaskSubType> taskSubTypeList = taskSubTypeMgrE.GetTaskSubType(userCode);

            if (taskSubTypeList == null || taskSubTypeList.Count == 0) return null;

            StringBuilder hql = new StringBuilder();
            hql.Append(@"Select us.TaskSubType,us.Id , us.IsEmail , us.Email , us.IsSMS , us.MobilePhone ,u.Email ,u.MobliePhone ");
            hql.Append(@"From UserSubscription us join us.User u ");
            hql.Append(@"Where u.Code =:UserCode and us.TaskSubType in (:TaskSubType)");
            hql.Append(@"Order by us.TaskSubType Asc,us.Id Asc ");
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("UserCode", userCode);
            param.Add("TaskSubType", taskSubTypeList.Select(t => t.Code).ToArray<string>());

            IList<object[]> userSubscriptionList = this.hqlMgrE.FindAll<object[]>(hql.ToString(), param);

            User user = userMgrE.CheckAndLoadUser(userCode);

            IList<UserSubView> userSubViewList = (from t in taskSubTypeList
                                                  select new UserSubView() { TaskSubTypeCode = t.Code, TaskSubTypeDesc = t.Desc, TaskType = t.Type, Email = user.Email, MobilePhone = user.MobliePhone }).ToList<UserSubView>();
            if (userSubscriptionList != null && userSubscriptionList.Count > 0)
            {
                foreach (UserSubView userSubView in userSubViewList)
                {
                    foreach (object[] userSubscription in userSubscriptionList)
                    {
                        if (userSubscription[0].ToString() == userSubView.TaskSubTypeCode)
                        {
                            if (userSubscription[1] == null)
                            {
                                userSubView.Id = null;
                            }
                            else
                            {
                                userSubView.Id = int.Parse(userSubscription[1].ToString());
                            }
                            userSubView.IsEmail = userSubscription[2] == null ? false : bool.Parse(userSubscription[2].ToString());
                            if (userSubscription[3] != null)
                            {
                                userSubView.Email = userSubscription[3].ToString();
                            }
                            else if (userSubscription[6] != null)
                            {
                                userSubView.Email = userSubscription[6].ToString();
                            }
                            userSubView.IsSMS = userSubscription[4] == null ? false : bool.Parse(userSubscription[4].ToString());

                            if (userSubscription[5] != null)
                            {
                                userSubView.MobilePhone = userSubscription[5].ToString();
                            }
                            else if (userSubscription[7] != null)
                            {
                                userSubView.MobilePhone = userSubscription[7].ToString();
                            }
                        }
                    }
                }

                //userSubViewList.OrderBy(u => u.TaskType);
            }

            return userSubViewList;
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateUserSubscription(IList<UserSubView> userSubViewList, User user)
        {
            if (userSubViewList == null || userSubViewList.Count == 0) return;
            foreach (UserSubView userSubView in userSubViewList)
            {
                this.UpdateUserSubscription(userSubView, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateUserSubscription(UserSubView userSubView, User user)
        {
            DateTime now = DateTime.Now;
            UserSubscription userSubscription = null;
            if (userSubView.Id.HasValue)
            {
                userSubscription = this.LoadUserSubscription(userSubView.Id.Value);
            }
            else
            {
                userSubscription = new UserSubscription();
            }
            userSubscription.LastModifyDate = now;
            userSubscription.LastModifyUser = user.Code;
            userSubscription.Email = userSubView.Email;
            userSubscription.MobilePhone = userSubView.MobilePhone;
            userSubscription.IsEmail = userSubView.IsEmail;
            userSubscription.IsSMS = userSubView.IsSMS;
            if (userSubView.Id.HasValue)
            {
                this.UpdateUserSubscription(userSubscription);
            }
            else
            {
                userSubscription.User = user;
                userSubscription.TaskSubType = userSubView.TaskSubTypeCode;
                userSubscription.CreateUser = user.Code;
                userSubscription.CreateDate = now;
                this.CreateUserSubscription(userSubscription);
            }

        }

        [Transaction(TransactionMode.Requires)]
        public void Check()
        {
            StringBuilder hql = new StringBuilder();
            hql.Append(@" select us.Id from UserSubscription us join us.User u ");
            hql.Append(@" where ");
            hql.Append(@"      not exists (select up.Permission.Code from UserPermission up where up.User.Code = u.Code and ((up.Permission.Category.Code=:TaskSubType and us.TaskSubType = up.Permission.Code) or up.Permission.Code in (:Admin))) ");
            hql.Append(@"  and not exists (select rp.Permission.Code from RolePermission rp join rp.Role r,UserRole ur where r.Code = ur.Role.Code and ur.User.Code = u.Code and ((rp.Permission.Category.Code=:TaskSubType and us.TaskSubType = rp.Permission.Code ) or (rp.Permission.Code in (:Admin)))) ");

            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("TaskSubType", ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
            param.Add("Admin", new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN });

            IList<int> idList = hqlMgrE.FindAll<int>(hql.ToString(), param);

            if (idList == null || idList.Count == 0) return;

            this.DeleteUserSubscription(idList);

        }

        public IList<UserSub> SubmitUserSub(TaskMstr task, User user)
        {
            IList<UserSub> userSubList = null;
            /*
            if ((task.Type == ISIConstants.ISI_TASK_TYPE_PRIVACY
                    || task.Type == ISIConstants.ISI_TASK_TYPE_PLAN))
            {*/
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT
                            && !string.IsNullOrEmpty(ISIUtil.EditUser(task.TaskSubType.AssignUser))
                            && (!task.IsWF || (task.IsWF && task.Level.HasValue && task.Level.Value == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
                                    || (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE && task.IsTrace)))
            {
                userSubList = this.GenerateUserSub(task, task.TaskSubType.AssignUser, false, user);
            }
            else if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
            {
                if (!string.IsNullOrEmpty(ISIUtil.EditUser(task.StartedUser)))
                {
                    userSubList = this.GenerateUserSub(task, task.StartedUser, false, user);
                }
                else if (!string.IsNullOrEmpty(ISIUtil.EditUser(task.TaskSubType.StartUser)))
                {
                    userSubList = this.GenerateUserSub(task, task.TaskSubType.StartUser, false, user);
                }
            }
            /*}
            else
            {
                userSubList = userSubscriptionMgrE.GenerateUserSub(task, user);
            }*/
            return userSubList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<UserSub> GenerateUserSub(TaskMstr task, User user)
        {
            return this.GenerateUserSub(task, string.Empty, true, user);
        }

        //查询所有管理员，不考虑订阅
        [Transaction(TransactionMode.Unspecified)]
        public IList<UserSub> GenerateUserSub(string taskSubType, User user)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(User));
            ProjectionList projectionList = Projections.ProjectionList()
                                .Add(Projections.Property("Code"), "Code")
                                .Add(Projections.Property("IsActive"), "IsEmail")
                                .Add(Projections.Property("IsActive"), "IsSMS")
                                .Add(Projections.Property("Email"), "Email")
                                .Add(Projections.Property("MobliePhone"), "MobilePhone");

            DetachedCriteria[] taCrieteria = ISIUtil.GetTaskAdminPermissionCriteria();

            criteria.Add(
                    Expression.Or(
                        Subqueries.PropertyIn("Code", taCrieteria[0]),
                        Subqueries.PropertyIn("Code", taCrieteria[1])));

            criteria.SetProjection(Projections.Distinct(projectionList));
            criteria.Add(Expression.Eq("IsActive", true));

            criteria.SetResultTransformer(Transformers.AliasToBean(typeof(UserSub)));
            IList<UserSub> userSubList = criteriaMgrE.FindAll<UserSub>(criteria);

            //GenerateUserSub(taskSubType, ref userSubList);
            return userSubList;
        }
        /*
        //从订阅表上读取邮箱与手机号码
        [Transaction(TransactionMode.Unspecified)]
        public void GenerateUserSub(string taskSubType, ref IList<UserSub> userSubList)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(UserSubscription));
            criteria.CreateAlias("User", "u", JoinType.InnerJoin);
            criteria.Add(Expression.Eq("TaskSubType", taskSubType));
            string[] userCodes = (from userSub in userSubList
                                  select userSub.Code).ToArray<string>();
            criteria.Add(Expression.In("u.Code", userCodes));
            IList<UserSubscription> userSubscriptionList = criteriaMgrE.FindAll<UserSubscription>(criteria);

            if (userSubscriptionList != null && userSubscriptionList.Count > 0)
            {
                userSubList = (from userSub in userSubList
                               join
                                   userSubscription in userSubscriptionList
                                   on userSub.Code equals userSubscription.User.Code into us
                               from userSubscription in us.DefaultIfEmpty()
                               select new UserSub
                               {
                                   Code = userSub.Code,
                                   MobilePhone = userSubscription == null || string.IsNullOrEmpty(userSubscription.MobilePhone) ? userSub.MobilePhone : userSubscription.MobilePhone,
                                   Email = userSubscription == null || string.IsNullOrEmpty(userSubscription.Email) ? userSub.Email : userSubscription.Email,
                                   IsSMS = ((userSubscription == null || string.IsNullOrEmpty(userSubscription.MobilePhone) && string.IsNullOrEmpty(userSub.MobilePhone)) || (userSubscription != null && (!userSubscription.IsSMS.HasValue || !userSubscription.IsSMS.Value))) ? false : true,
                                   IsEmail = (userSubscription == null || string.IsNullOrEmpty(userSubscription.Email)) && string.IsNullOrEmpty(userSub.Email) ? false : true,
                               }).ToList<UserSub>();
            }
        }
        */
        /*
         * isUserSub true 考虑是否订阅，false 不考虑是否订阅
         */

        [Transaction(TransactionMode.Unspecified)]
        public IList<UserSub> GenerateUserSub(TaskMstr task, string userCodes, bool isUserSub, User user)
        {
            return GenerateUserSub(task.Type, task.TaskSubType.Code, task.Code, userCodes, isUserSub, user);
        }

        /*
         * isUserSub true 考虑是否订阅，false 不考虑是否订阅
         */
        [Transaction(TransactionMode.Unspecified)]
        public IList<UserSub> GenerateUserSub(string taskType, string taskSubTypeCode, string taskCode, string userCodes, bool isUserSub, User user)
        {
            string[] userCodeArray = null;
            if (!string.IsNullOrEmpty(userCodes))
            {
                userCodeArray = userCodes.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                userCodeArray = userCodeArray.Where(u => !string.IsNullOrEmpty(u)).ToArray<string>();
            }

            if (string.IsNullOrEmpty(userCodes) || userCodeArray == null || userCodeArray.Length == 0)
            {
                return new List<UserSub>();
            }
            IList<UserSub> userSubList = new List<UserSub>();
            if (!isUserSub)
            {
                string users = string.Join("','", userCodeArray);

                GenerateUserSub(taskType, taskSubTypeCode, taskCode, user, userSubList, users);
            }
            else
            {
                #region 废弃
                //考虑是否订阅,有权限才能订阅，因此不考虑权限，
                DetachedCriteria criteria = DetachedCriteria.For(typeof(UserSubscription));
                criteria.CreateAlias("User", "u", JoinType.InnerJoin);
                ProjectionList projectionList = Projections.ProjectionList()
                                    .Add(Projections.Property("u.Code"), "Code")
                                    .Add(Projections.Property("IsEmail"), "IsEmail")
                                    .Add(Projections.SqlProjection(@"isnull(Email,USR_Email) as Email", new String[] { "Email" }, new IType[] { NHibernateUtil.String }))
                                    .Add(Projections.Property("IsSMS"), "IsSMS")
                                    .Add(Projections.SqlProjection(@"isnull(MobilePhone, USR_MPhone) as MobilePhone", new String[] { "MobilePhone" }, new IType[] { NHibernateUtil.String }));
                //
                criteria.SetProjection(Projections.Distinct(projectionList));
                criteria.Add(Expression.Eq("u.IsActive", true));
                criteria.Add(Expression.Eq("TaskSubType", taskSubTypeCode));

                if (userCodeArray != null && userCodeArray.Length > 0)
                {
                    criteria.Add(Expression.In("u.Code", userCodeArray));
                }
                if (user != null)
                {
                    criteria.Add(Expression.Not(Expression.Eq("u.Code", user.Code)));
                }

                criteria.Add(Expression.Or(
                    Expression.And(Expression.Eq("IsEmail", true), Expression.Or(Expression.IsNotNull("Email"), Expression.IsNotNull("u.Email"))),
                    Expression.And(Expression.Eq("IsSMS", true), Expression.Or(Expression.IsNotNull("MobilePhone"), Expression.IsNotNull("u.MobliePhone")))
                    ));

                criteria.SetResultTransformer(Transformers.AliasToBean(typeof(UserSub)));
                userSubList = this.criteriaMgrE.FindAll<UserSub>(criteria);
                #endregion
            }

            return userSubList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public void GenerateUserSub(string taskType, string taskSubTypeCode, string taskCode, User user, IList<UserSub> userSubList, string users)
        {
            IList<SqlParameter> sqlParam = new List<SqlParameter>();
            sqlParam.Add(new SqlParameter("@TaskType", taskType));
            sqlParam.Add(new SqlParameter("@TaskSubType", taskSubTypeCode));
            sqlParam.Add(new SqlParameter("@TaskCode", taskCode));
            sqlParam.Add(new SqlParameter("@UserCodes", users));
            sqlParam.Add(new SqlParameter("@CurrentUser", user.Code));

            DataSet dataSet = sqlHelperMgrE.GetDatasetByStoredProcedure("USP_Search_TaskEmail", sqlParam.ToArray<SqlParameter>());
            DataRow[] drs = dataSet.Tables[0].Select();
            foreach (DataRow dr in drs)
            {
                UserSub userSub = new UserSub();
                userSub.Code = dr["Code"].ToString();
                userSub.IsEmail = true;
                userSub.Email = dr["Email"].ToString();
                userSub.Password = dr["Password"].ToString();
                userSubList.Add(userSub);
            }
        }
        public void Remind(TaskMstr task, IList<UserSub> userSubList, User operationUser)
        {
            Remind(task, ISIConstants.ISI_LEVEL_BASE, 0, userSubList, false, operationUser);
        }

        public void Remind(TaskMstr task, string level, IList<UserSub> userSubList, User operationUser)
        {
            Remind(task, level, 0, userSubList, false, operationUser);
        }

        public void Remind(TaskMstr task, string level, IList<UserSub> userSubList, bool isApprove, User operationUser)
        {
            Remind(task, level, 0, userSubList, isApprove, operationUser);
        }

        public void Remind(TaskMstr task, IList<UserSub> userSubList, string helpContent, User operationUser)
        {
            task.HelpContent = helpContent;
            Remind(task, ISIConstants.ISI_LEVEL_HELP, 0, userSubList, false, operationUser);
        }

        public void Remind(TaskMstr task, string level, double minutes, IList<UserSub> userSubList, User operationUser)
        {
            Remind(task, level, minutes, userSubList, false, operationUser);
        }

        public void Remind(TaskMstr task, string level, double minutes, IList<UserSub> userSubList, bool isApprove, User operationUser)
        {
            if (userSubList == null || userSubList.Count == 0) return;

            StringBuilder toEmail = new StringBuilder();
            StringBuilder toMobliePhone = new StringBuilder();

            foreach (UserSub userSub in userSubList)
            {
                if (userSub.IsEmail)
                {
                    if (toEmail.Length != 0)
                    {
                        toEmail.Append(";");
                    }
                    toEmail.Append(userSub.Email);
                }
                if (userSub.IsSMS)
                {
                    if (toMobliePhone.Length != 0)
                    {
                        toMobliePhone.Append(";");
                    }
                    toMobliePhone.Append(userSub.MobilePhone);
                }
            }

            string emailBody = string.Empty;
            string smsBody = string.Empty;

            #region 获取内容

            if (toEmail.Length > 0)
            {
                emailBody = this.GetEmailBody(task, level, minutes, isApprove, userSubList, operationUser);
            }

            if (toMobliePhone.Length > 0)
            {
                smsBody = this.GetSMSBody(task, level, minutes, operationUser);
            }

            #endregion

            string userMail = string.Empty;
            MailPriority mailPriority;

            #region email标题 回复人 优先级

            string subject = this.GetSubject(operationUser, task.Code, task.Type, task.Priority, task.Subject, level, task.Status);

            if (operationUser != null && !string.IsNullOrEmpty(operationUser.Email) && ISIUtil.IsValidEmail(operationUser.Email))
            {
                userMail = operationUser.Name + "," + operationUser.Email;
            }
            else if (level == ISIConstants.ISI_LEVEL_BASE && !string.IsNullOrEmpty(task.Email) && ISIUtil.IsValidEmail(task.Email))
            {
                if (!string.IsNullOrEmpty(task.UserName))
                {
                    userMail = task.UserName + "," + task.Email;
                }
                else
                {
                    userMail = task.Email;
                }
            }

            if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
            {
                mailPriority = MailPriority.High;
            }
            else
            {
                mailPriority = MailPriority.Normal;
            }

            #endregion

            bool isEmailException = false;
            bool isSMSException = false;

            #region 邮件与短信发送

            if (!string.IsNullOrEmpty(emailBody) && !string.IsNullOrEmpty(toEmail.ToString()))
            {
                if (isApprove && task.Level.HasValue && level == ISIConstants.ISI_LEVEL_APPROVE && task.TaskSubType.IsAttachment)
                {
                    IList<string[]> files = new List<string[]>();
                    var attachmentDetailList = this.attachmentDetailMgrE.GetTaskAttachment(task.Code);
                    foreach (var attachmentDetail in attachmentDetailList)
                    {
                        string appDataAllFolder = appDataFolder + attachmentDetail.Path;
                        if (File.Exists(appDataAllFolder))//判断是否存在
                        {
                            //appDataAllFolder = appDataAllFolder.Replace("/", "\\");
                            files.Add(new string[] { appDataAllFolder, attachmentDetail.FileName });
                        }
                    }
                    isEmailException = this.SendEmail(isApprove, task.Code, subject, emailBody, toEmail.ToString(), userMail, mailPriority, operationUser, files);
                }
                else
                {
                    isEmailException = this.SendEmail(isApprove, task.Code, subject, emailBody, toEmail.ToString(), userMail, mailPriority, operationUser);
                }
                //isEmailException = this.SendEmail(task.Code, subject, emailBody, "tiansu@yfgm.com.cn", userMail, mailPriority, operationUser);
            }
            if (!string.IsNullOrEmpty(smsBody) && !string.IsNullOrEmpty(toMobliePhone.ToString()))
            {
                isSMSException = this.SendSMS(task.Code, toMobliePhone.ToString(), smsBody, operationUser);
            }
            #endregion

            #region 记录上报TaskDetail

            taskDetailMgrE.CreateTaskDetail(task, level, userSubList, isEmailException, isSMSException, operationUser);

            #endregion
        }


        [Transaction(TransactionMode.Unspecified)]
        protected string GetSMSBody(TaskMstr task, string level, double minutes, User operationUser)
        {
            StringBuilder content = new StringBuilder();
            string separator = ISIConstants.SMS_SEPRATOR;
            try
            {
                content.Append("ISI  ");
                string companyName = entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).Value;
                ISIUtil.AppendTestText(companyName, content, separator);
                if (!string.IsNullOrEmpty(task.HelpContent))
                {
                    content.Append("请协助求助处理");
                    content.Append(separator);
                }

                content.Append(this.GetDesc(task.Type, operationUser));
                content.Append(task.Code);
                if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
                {
                    content.Append("[" + codeMasterMgrE.GetCachedCodeMaster(ISIConstants.CODE_MASTER_ISI_PRIORITY, task.Priority).Description + "]");
                }

                if (!string.IsNullOrEmpty(task.Subject))
                {
                    content.Append(separator);
                    content.Append("标题:" + task.Subject);
                }

                content.Append(separator);
                content.Append("类型: " + (task.TaskSubType != null ? task.TaskSubType.Description : task.TaskSubTypeDesc) + "|" + this.codeMasterMgrE.LoadCodeMaster(ISIConstants.CODE_MASTER_ISI_STATUS, task.Status).Description);

                DateTime date = DateTime.Now;

                if (level == ISIConstants.ISI_LEVEL_BASE)//提醒
                {

                    content.Append("|提醒");
                }
                else if (level == ISIConstants.ISI_LEVEL_HELP)
                {

                    content.Append("|求助");
                }
                else if (level == ISIConstants.ISI_LEVEL_COMMENT)
                {

                    content.Append("|评论");
                }
                else if (level == ISIConstants.ISI_LEVEL_STARTPERCENT)
                {

                    content.Append("|执行进度");
                }
                else if (level == ISIConstants.ISI_LEVEL_OPEN)
                {

                    content.Append("|开始");
                }
                else if (level == ISIConstants.ISI_LEVEL_COMPLETE)
                {

                    content.Append("|逾期完成");
                }
                else//上报
                {
                    content.Append("|上报");
                    //content.Append(level + "级");

                    //分派提醒
                    if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT && task.SubmitDate.HasValue)
                    {
                        string diff = ISIUtil.GetDiff(task.SubmitDate.Value.AddMinutes(minutes));
                        if (!string.IsNullOrEmpty(diff))
                        {
                            content.Append(separator);
                            content.Append("分派超时:" + diff);
                        }
                        date = task.SubmitDate.Value;
                    }
                    //开始提醒
                    if ((task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN && task.AssignDate.HasValue))
                    {
                        string diff = ISIUtil.GetDiff(task.AssignDate.Value.AddMinutes(minutes));
                        if (!string.IsNullOrEmpty(diff))
                        {
                            content.Append(separator);
                            content.Append("确认超时:" + diff);
                        }
                        date = task.AssignDate.Value;
                    }
                    //关闭提醒
                    else if ((task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS
                                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE) && task.StartDate.HasValue)
                    {
                        string diff = ISIUtil.GetDiff(task.StartDate.Value.AddMinutes(minutes));
                        if (!string.IsNullOrEmpty(diff))
                        {
                            content.Append(separator);
                            content.Append("关闭超时:" + diff);
                        }
                        if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
                        {
                            date = task.StartDate.Value;
                        }
                        else if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                        {
                            date = task.CompleteDate.Value;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(task.BackYards))
                {
                    content.Append(separator);
                    content.Append("追溯码:" + task.BackYards);
                }
                content.Append(separator);
                content.Append("时间:" + date.ToString("yyyy-MM-dd HH:mm") + separator);
                //content.Append("类型:" + task.TaskSubType.Description + separator);
                if (task.FailureMode != null)
                {
                    content.Append("失效模式:" + task.FailureMode.Code + separator);
                }
                content.Append("地点:" + task.TaskAddress + separator);
                if (task.PlanStartDate.HasValue)
                {
                    content.Append("预计开始时间:" + task.PlanStartDate.Value.ToString("yyyy-MM-dd HH:mm") + separator);
                }
                if (task.PlanCompleteDate.HasValue)
                {
                    content.Append("预计完成时间:" + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd HH:mm") + separator);
                }
                if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                {
                    content.Append("已经完成!");
                    return content.ToString();
                }
                if (level == ISIConstants.ISI_LEVEL_COMMENT)
                {
                    if (task.CommentDetail != null && !string.IsNullOrEmpty(task.CommentDetail.Comment))
                    {
                        content.Append(ISIUtil.GetStrLength(task.CommentDetail.Comment, 20));
                        content.Append(separator);
                        content.Append(task.CommentDetail.CreateUserNm);
                        content.Append(separator);
                        content.Append(task.CommentDetail.CreateDate.ToString("yyyy-MM-dd HH:mm"));

                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(task.Desc1))
                    {
                        content.Append(ISIUtil.GetStrLength(task.Desc1, 20));
                        content.Append(separator);
                    }
                    if (!string.IsNullOrEmpty(task.UserName)
                        || (!string.IsNullOrEmpty(task.MobilePhone) && ISIUtil.IsValidMobilePhone(task.MobilePhone)))
                    {
                        content.Append("[");

                        if (!string.IsNullOrEmpty(task.UserName))
                        {
                            content.Append(task.UserName);
                        }
                        if (!string.IsNullOrEmpty(task.MobilePhone) && ISIUtil.IsValidMobilePhone(task.MobilePhone))
                        {
                            if (!string.IsNullOrEmpty(task.UserName))
                            {
                                content.Append(", ");
                            }
                            content.Append(task.MobilePhone);
                        }
                        content.Append("]");
                    }

                    if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                    {
                        content.Append(separator);
                        content.Append("关闭回复 " + ISIUtil.GetSerialNo(task.Code) + "+空格+Y");
                    }
                }

            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return content.ToString();
        }




        [Transaction(TransactionMode.Unspecified)]
        protected string GetEmailBody(TaskMstr task, string level, double minutes, User operationUser)
        {
            return GetEmailBody(task, level, minutes, false, null, operationUser);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected string GetEmailBody(TaskMstr task, string level, double minutes, bool isApprove, IList<UserSub> userSubList, User operationUser)
        {
            StringBuilder content = new StringBuilder();
            try
            {
                content.Append("<p style='font-size:15px;'>");
                string separator = ISIConstants.EMAIL_SEPRATOR;
                DateTime now = DateTime.Now;
                string companyName = entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).Value;
                ISIUtil.AppendTestText(companyName, content, separator);

                if (level == ISIConstants.ISI_LEVEL_HELP)
                {
                    content.Append(separator);
                    content.Append("<U>求助</U>：" + task.HelpContent);
                    content.Append(separator);
                    content.Append(separator);
                    content.Append(operationUser.Name);
                    content.Append(separator);
                    content.Append(now.ToString("yyyy-MM-dd HH:mm"));
                    content.Append(separator);
                    content.Append(separator);
                }
                string webAddress = "localhost:2013";
                if (!companyName.ToUpper().Contains("TEST"))
                {
                    webAddress = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_WEBADDRESS).Value;
                }

                if (task.IsWF)
                {
                    content.Append(separator);
                    content.Append("<U>流程</U>: " + (task.TaskSubType != null ? task.TaskSubType.Description : task.TaskSubTypeDesc));
                    content.Append(separator);
                }

                if (level == ISIConstants.ISI_LEVEL_STARTPERCENT)
                {
                    content.Append(separator);
                    content.Append("按照预计完成时间 " + (task.PlanCompleteDate.HasValue ? task.PlanCompleteDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty) + " 已经过去 " + ((task.TaskSubType != null ? task.TaskSubType.StartPercent.Value : task.StartPercent).Value * 100).ToString("0.####") + "% 请注意进度。");
                    content.Append(separator);
                    content.Append(separator);
                    content.Append(separator);
                }
                if (level == ISIConstants.ISI_LEVEL_OPEN)
                {
                    content.Append(separator);
                    content.Append("已经到了预计开始时间 " + (task.PlanStartDate.HasValue ? task.PlanStartDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty) + "，提醒开始执行！");
                    content.Append(separator);
                    content.Append(separator);
                    content.Append(separator);
                }
                if (level == ISIConstants.ISI_LEVEL_COMPLETE)
                {
                    content.Append(separator);
                    content.Append("已经超过预计完成时间 " + (task.PlanCompleteDate.HasValue ? task.PlanCompleteDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty) + "。请注意！");
                    content.Append(separator);
                    content.Append(separator);
                    content.Append(separator);
                }

                //评论、进展显示最近5条
                if (level == ISIConstants.ISI_LEVEL_STATUS || level == ISIConstants.ISI_LEVEL_COMMENT || level == ISIConstants.ISI_LEVEL_STARTPERCENT || level == ISIConstants.ISI_LEVEL_COMPLETE)
                {
                    PutTraceView(task.Code, level, content, separator, operationUser);
                }
                /*
                if (level == ISIConstants.ISI_LEVEL_STATUS)
                {
                    PutStatusText(task, content, separator);
                    PutCommentText(task, content, separator);
                }
                if (level == ISIConstants.ISI_LEVEL_COMMENT)
                {
                    PutCommentText(task, content, separator);
                    PutStatusText(task, content, separator);
                }
                 */

                if (!string.IsNullOrEmpty(task.Desc1))
                {
                    content.Append("<U>描述</U>: " + task.Desc1.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                    content.Append(separator);
                }

                if (!string.IsNullOrEmpty(task.Desc2))
                {
                    content.Append("<I>补充描述</I>: " + task.Desc2.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                    content.Append(separator);
                }
                content.Append(separator);
                if (!string.IsNullOrEmpty(task.ExpectedResults))
                {
                    content.Append("<U>预期结果/达成结果</U>: ");
                    content.Append(task.ExpectedResults.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                    content.Append(separator + separator);
                }
                content.Append("<U>" + this.GetDesc(task.Type, operationUser) + "</U>: ");
                content.Append(task.Code);
                if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
                {
                    content.Append("[" + codeMasterMgrE.GetCachedCodeMaster(ISIConstants.CODE_MASTER_ISI_PRIORITY, task.Priority).Description + "]");
                }
                content.Append(separator);
                content.Append("<U>状态</U>: " + this.codeMasterMgrE.LoadCodeMaster(ISIConstants.CODE_MASTER_ISI_STATUS, task.Status).Description);

                if (task.IsWF && task.Level >= ISIConstants.CODE_MASTER_WFS_LEVEL3)
                {
                    var wflevel = task.Level / ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL * ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;

                    content.Append("&nbsp;&nbsp;<font color='blue'>");
                    content.Append(languageMgrE.TranslateMessage(wflevel.ToString(), operationUser));
                    content.Append(wflevel != task.Level ? "&nbsp;" + (task.Level / ISIConstants.CODE_MASTER_WF_COUNTERSIGN_LEVEL_INTERVAL).ToString().Substring(1) : string.Empty);
                    content.Append("</font>");
                }

                if (!string.IsNullOrEmpty(task.Subject))
                {
                    content.Append(separator);
                    content.Append("<U>标题</U>: " + task.Subject);
                }
                DateTime date = DateTime.Now;

                if (level == ISIConstants.ISI_LEVEL_BASE)//提醒
                {
                    content.Append(separator);
                    if (!task.IsWF && task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
                    {
                        content.Append("<U>发送类型</U>: 分派提醒");
                    }
                    else if (task.IsWF && (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE))
                    {
                        content.Append("<U>发送类型</U>: 审批提醒");
                    }
                    else if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
                    {
                        content.Append("<U>发送类型</U>: 执行提醒");
                    }
                    else
                    {
                        content.Append("<U>发送类型</U>: 提醒");
                    }
                }
                else if (level == ISIConstants.ISI_LEVEL_APPROVE)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 审批");
                }
                else if (level == ISIConstants.ISI_LEVEL_COMMENT)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 评论");
                }
                else if (level == ISIConstants.ISI_LEVEL_STATUS)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 进展");
                }
                else if (level == ISIConstants.ISI_LEVEL_HELP)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 求助");
                }
                else if (level == ISIConstants.ISI_LEVEL_STARTPERCENT)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 执行进度提醒");
                }
                else if (level == ISIConstants.ISI_LEVEL_OPEN)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 开始执行提醒");
                }
                else if (level == ISIConstants.ISI_LEVEL_COMPLETE)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 逾期完成提醒");
                }
                else//上报
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 上报" + level + "级");

                    //分派提醒
                    if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT && task.SubmitDate.HasValue)
                    {
                        string diff = ISIUtil.GetDiff(task.SubmitDate.Value.AddMinutes(minutes));
                        if (!string.IsNullOrEmpty(diff))
                        {
                            content.Append(separator);
                            content.Append("<U>分派超时</U>: " + diff);
                        }
                        date = task.SubmitDate.Value;
                    }
                    //开始提醒
                    if ((task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN && task.AssignDate.HasValue))
                    {
                        string diff = ISIUtil.GetDiff(task.AssignDate.Value.AddMinutes(minutes));
                        if (!string.IsNullOrEmpty(diff))
                        {
                            content.Append(separator);
                            content.Append("<U>确认超时</U>: " + diff);
                        }
                        date = task.AssignDate.Value;
                    }
                    //关闭提醒
                    else if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE && task.CompleteDate.HasValue)
                    {
                        string diff = ISIUtil.GetDiff(task.CompleteDate.Value.AddMinutes(minutes));
                        if (!string.IsNullOrEmpty(diff))
                        {
                            content.Append(separator);
                            content.Append("<U>关闭超时</U>: " + diff);
                        }
                        date = task.CompleteDate.Value;
                    }
                }

                if (!string.IsNullOrEmpty(task.BackYards))
                {
                    content.Append(separator);
                    content.Append("<U>追溯码</U>: " + task.BackYards);
                }
                content.Append(separator);
                content.Append("<U>时间</U>: " + date.ToString("yyyy-MM-dd HH:mm") + separator);

                if (task.Type == ISIConstants.ISI_TASK_TYPE_PROJECT)
                {
                    content.Append("<U>项目</U>: " + (task.TaskSubType != null ? task.TaskSubType.Description : task.TaskSubTypeDesc) + separator);
                    content.Append("<U>阶段</U>: " + task.Phase + separator);
                    //content.Append("序号: " + task.Seq + separator);
                }
                else
                {
                    content.Append("<U>类型</U>: " + (task.TaskSubType != null ? task.TaskSubType.Description : task.TaskSubTypeDesc) + separator);
                    if (task.FailureMode != null)
                    {
                        content.Append("<U>失效模式</U>: " + task.FailureMode.Code + separator);
                    }
                }

                content.Append("<U>地点</U>: " + task.TaskAddress + separator);

                if (!string.IsNullOrEmpty(task.ApprovalUserNm))
                {
                    content.Append("<U>申请人</U>: " + task.CreateUserNm + separator);
                    content.Append("<U>审批人</U>: " + task.ApprovalUserNm + separator);
                }

                if (!string.IsNullOrEmpty(task.AssignStartUserNm))
                {
                    content.Append("<U>执行人</U>: " + task.AssignStartUserNm + separator);
                }
                else if (!string.IsNullOrEmpty(task.StartedUser))
                {
                    string principals = this.GetUserName(task.StartedUser);
                    content.Append("<U>执行人</U>: " + principals + separator);
                }

                if (task.PlanStartDate.HasValue)
                {
                    content.Append("<U>预计开始时间</U>: " + task.PlanStartDate.Value.ToString("yyyy-MM-dd HH:mm") + separator);
                }
                if (task.PlanCompleteDate.HasValue)
                {
                    content.Append("<U>预计完成时间</U>: " + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd HH:mm") + separator);
                }

                if (task.IsApply)
                {
                    IList<TaskApply> taskApplyList = null;
                    if ((task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
                                        && task.TaskSubType.IsRemoveForm)
                    {
                        taskApplyList = taskApplyMgrE.GetActiveTaskApply(task.Code);
                    }
                    else
                    {
                        taskApplyList = taskApplyMgrE.GetTaskApply(task.Code);
                    }
                    task.TaskApplyList = taskApplyList;
                    taskApplyMgrE.OutputEmailApply(content, taskApplyList, task.TaskSubType.IsRemoveForm);
                }

                if (!string.IsNullOrEmpty(task.WorkHoursUserNm))
                {
                    content.Append("<U>工时申请人</U>: " + task.WorkHoursUserNm + separator);
                }

                if (isApprove && task.Level.HasValue)
                {
                    content.Append(separator + separator);
                    AppendApprove(task, userSubList[0], content, separator, webAddress, "1");
                    //最终审批
                    if (task.Level.Value == ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE)
                    {
                        AppendApprove(task, userSubList[0], content, separator, webAddress, "4");
                    }
                    else
                    {
                        AppendApprove(task, userSubList[0], content, separator, webAddress, "3");
                    }
                    AppendApprove(task, userSubList[0], content, separator, webAddress, "2");
                }

                content.Append(separator + separator);

                //显示所有批示
                if (level == ISIConstants.ISI_LEVEL_APPROVE)
                {
                    PutTraceView(task.Code, level, content, separator, operationUser);
                }

                if (task.UserName != null && task.UserName.Trim() != string.Empty)
                    content.Append(task.UserName + separator);
                if (task.MobilePhone != null && task.MobilePhone.Trim() != string.Empty && ISIUtil.IsValidMobilePhone(task.MobilePhone))
                    content.Append("Tel: " + task.MobilePhone + separator);
                if (task.Email != null && task.Email.Trim() != string.Empty && ISIUtil.IsValidEmail(task.Email))
                    content.Append("Email: " + task.Email + separator);


                content.Append(separator);
                content.Append(companyName + separator);
                content.Append("<a href='http://" + webAddress + "'>http://" + webAddress + "</a>");
                content.Append(separator);
                content.Append("</p>");
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return content.ToString();
        }

        private static void AppendApprove(TaskMstr task, UserSub userSub, StringBuilder content, string separator, string webAddress, string type)
        {
            string desc = string.Empty;
            content.Append(separator);
            if (type == "1")
            {
                desc = "批准";
            }
            else
            {
                if (type == "2")
                {
                    desc = "退回";
                }
                else if (type == "3")
                {
                    desc = "争议";
                }
                else if (type == "4")
                {
                    desc = "不批准";
                }
                content.Append("或");
            }
            content.Append("您可点击 ");
            content.Append("<a href='http://" + webAddress + "/ISI/TSK/ApproveHandler.ashx?TaskCode=" + task.Code + "&UserCode=" + userSub.Code + "&UserPwd=" + userSub.Password + "&Type=" + type + "' >");
            content.Append("快速" + desc);
            content.Append("<a/>");
            content.Append("、");
            content.Append("<a href='http://" + webAddress + "/ISI/TSK/Approve.aspx?TaskCode=" + task.Code + "&UserCode=" + userSub.Code + "&UserPwd=" + userSub.Password + "&Type=" + type + "' >");
            content.Append(desc + "并编写批示");
            content.Append("<a/>");
            content.Append(separator);
            content.Append(separator);
        }


        private void PutCommentText(TaskMstr task, StringBuilder content, string separator)
        {
            if (task.CommentDetail != null)
            {
                content.Append(separator);
                content.Append("<U>评论</U>：");

                if (!string.IsNullOrEmpty(task.CommentDetail.Comment))
                {
                    content.Append(task.CommentDetail.Comment.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                }
                content.Append(separator);
                content.Append(separator);
                content.Append(task.CommentDetail.CreateUserNm);
                content.Append(separator);
                content.Append(task.CommentDetail.CreateDate.ToString("yyyy-MM-dd HH:mm"));
                content.Append(separator);
                content.Append(separator);
                content.Append(separator);
            }
        }

        private void PutTraceView(string taskCode, string level, StringBuilder content, string separator, User user)
        {
            StringBuilder hql = new StringBuilder("from TraceView where TaskCode ='" + taskCode + "' ");
            if (level != ISIConstants.ISI_LEVEL_APPROVE)
            {
                //进展或者评论显示最近两周的
                hql.Append(" and LastModifyDate > '" + DateTime.Now.AddDays(-14) + "' ");
            }
            hql.Append("order by LastModifyDate desc ");
            IList<TraceView> traceViewList = hqlMgrE.FindAll<TraceView>(hql.ToString());

            if (traceViewList != null && traceViewList.Count > 0)
            {
                int count = traceViewList.Count;
                content.Append(separator);
                for (int i = 0; i < count; i++)
                {
                    var traceView = traceViewList[i];
                    if (!string.IsNullOrEmpty(traceView.Desc))
                    {
                        content.Append("<U style='background:#DFD3D3'>" + languageMgrE.TranslateMessage("ISI.TSK." + traceView.Type, user) + "</U>：");
                        content.Append(traceView.Desc.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));

                        if (traceView.Type == ISIConstants.CODE_MASTER_ISI_MSG_TYPE_STATUS)
                        {
                            content.Append(separator);
                            content.Append("标志: <span style='background-color:" + traceView.Color + "'>" + traceView.Flag + "</span>");
                            if (traceView.StartDate.HasValue)
                            {
                                content.Append(separator);
                                content.Append("开始时间: " + traceView.StartDate.Value.ToString("yyyy-MM-dd"));
                            }
                            if (traceView.EndDate.HasValue)
                            {
                                content.Append(separator);
                                content.Append("结束时间: " + traceView.EndDate.Value.ToString("yyyy-MM-dd"));
                            }
                        }
                        content.Append(separator);
                        content.Append(separator);
                        content.Append(traceView.LastModifyUserNm);
                        content.Append(separator);
                        content.Append(traceView.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));
                        content.Append(separator);
                        content.Append(separator);
                        content.Append(separator);
                    }
                }
            }
        }

        private void PutStatusText(TaskMstr task, StringBuilder content, string separator)
        {
            if (task.TaskStatus != null)
            {
                content.Append(separator);
                content.Append("<U>进展</U>：");

                if (!string.IsNullOrEmpty(task.TaskStatus.Desc))
                {
                    content.Append(task.TaskStatus.Desc.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                    content.Append(separator);
                    content.Append("标志: <span style='background-color:" + task.TaskStatus.Color + "'>" + task.TaskStatus.Flag + "</span>");
                    content.Append(separator);
                    content.Append("开始时间: " + task.TaskStatus.StartDate.ToString("yyyy-MM-dd"));
                    content.Append(separator);
                    content.Append("结束时间: " + task.TaskStatus.EndDate.ToString("yyyy-MM-dd"));
                }
                content.Append(separator);
                content.Append(separator);
                content.Append(task.TaskStatus.LastModifyUserNm);
                content.Append(separator);
                content.Append(task.TaskStatus.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));
                content.Append(separator);
                content.Append(separator);
                content.Append(separator);
            }
        }
        [Transaction(TransactionMode.Unspecified)]
        public string GetUserName(string userCodes)
        {
            return GetUserName(userCodes, ", ");
        }

        [Transaction(TransactionMode.Unspecified)]
        public string GetUserName(string userCodes, string separator)
        {
            if (string.IsNullOrEmpty(userCodes)) return string.Empty;

            IDictionary<string, object> param = new Dictionary<string, object>();
            string[] userCodeArr = userCodes.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            param.Add("UserCode", userCodeArr);
            IList<object[]> userNameObj = hqlMgrE.FindAll<object[]>("select u.Code,u.FirstName,u.LastName from User u where u.Code in (:UserCode) ", param);

            if (userNameObj == null || userNameObj.Count == 0) return string.Empty;

            IDictionary<string, object[]> userNameDic = userNameObj.ToDictionary(u => u[0].ToString());
            StringBuilder userNames = new StringBuilder();

            foreach (string userCode in userCodeArr)
            {
                if (userNameDic.ContainsKey(userCode))
                {
                    object[] u = userNameDic[userCode];

                    if (!string.IsNullOrEmpty(userNames.ToString()))
                    {
                        userNames.Append(separator);
                    }
                    userNames.Append(u[1] != null ? u[1] : string.Empty);
                    userNames.Append(" ");
                    userNames.Append(u[2] != null ? u[2] : string.Empty);
                }
            }
            return userNames.ToString();
        }

        [Transaction(TransactionMode.Requires)]
        private bool SendSMS(string code, string toMobliePhone, string msg, User user)
        {
            bool isSMSException = false;
            try
            {
                emppMgrE.AsyncSend(toMobliePhone, msg, user);
            }
            catch (Exception e)
            {
                isSMSException = true;
                log.Error("Code=" + code + ",toMobliePhone=" + toMobliePhone + ",operator=" + user.Code + ",e=" + e.Message, e);
            }
            return isSMSException;
        }

        [Transaction(TransactionMode.Requires)]
        private bool SendEmail(bool isApprove, string code, string subject, string body, string mailTo, string replyTo, MailPriority priorit, User user, IList<string[]> files)
        {
            bool isEmailException = false;
            try
            {
                smtpMgrE.AsyncSend3(isApprove, subject, body, mailTo, replyTo, priorit, files);
            }
            catch (Exception e)
            {
                isEmailException = true;
                log.Error("Code=" + code + ",toEmail=" + mailTo + ",operator=" + user.Code + ",e=" + e.Message, e);
            }

            return isEmailException;
        }

        [Transaction(TransactionMode.Requires)]
        private bool SendEmail(bool isApprove, string code, string subject, string body, string mailTo, string replyTo, MailPriority priorit, User user)
        {
            bool isEmailException = false;

            try
            {
                smtpMgrE.AsyncSend3(isApprove, subject, body, mailTo, replyTo, priorit);
            }
            catch (Exception e)
            {
                isEmailException = true;
                log.Error("Code=" + code + ",toEmail=" + mailTo + ",operator=" + user.Code + ",e=" + e.Message, e);
            }

            return isEmailException;
        }


        [Transaction(TransactionMode.Unspecified)]
        public string GetSubject(User user, string code, string type, string priority, string value, string level, string status)
        {
            StringBuilder subject = new StringBuilder();
            subject.Append(user.Name + " ");
            if (priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
            {
                subject.Append(codeMasterMgrE.GetCachedCodeMaster(ISIConstants.CODE_MASTER_ISI_PRIORITY, priority).Description + " ");
            }

            if (!string.IsNullOrEmpty(level))
            {
                if (level == ISIConstants.ISI_LEVEL_HELP)
                {
                    subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Help", user));
                }
                else if (level == ISIConstants.ISI_LEVEL_BASE)
                {
                    if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
                    {
                        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Assign", user));
                    }
                    else
                    {
                        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Subscription", user));
                    }
                }
                else if (level == ISIConstants.ISI_LEVEL_COMMENT)
                {
                    subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Comment", user));
                }
                else if (level == ISIConstants.ISI_LEVEL_STATUS)
                {
                    subject.Append(languageMgrE.TranslateMessage("ISI.Remind.TaskStatus", user));
                }
                else if (level == ISIConstants.ISI_LEVEL_APPROVE)
                {
                    if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
                    {
                        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Apply", user));
                    }
                    else
                    {
                        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Approve", user));
                    }
                }
                else if (level == ISIConstants.ISI_LEVEL_STARTPERCENT)
                {
                    subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Schedule", user));
                }
                else if (level == ISIConstants.ISI_LEVEL_OPEN)
                {
                    subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Open", user));
                }
                else if (level == ISIConstants.ISI_LEVEL_COMPLETE)
                {
                    subject.Append(languageMgrE.TranslateMessage("ISI.Remind.OverDue", user));
                }
                else
                {
                    subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Up", user, new string[] { level }));
                }
            }
            subject.Append(" " + this.GetDesc(type, user) + ": ");
            if (string.IsNullOrEmpty(value))
            {
                subject.Append(code);
            }
            else
            {
                subject.Append(value);
            }
            return subject.ToString();
        }


        public string GetDesc(string type, User user)
        {
            if (type == ISIConstants.ISI_TASK_TYPE_PLAN)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.Plan", user);
            }
            if (type == ISIConstants.ISI_TASK_TYPE_ISSUE)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.Issue", user);
            }
            if (type == ISIConstants.ISI_TASK_TYPE_IMPROVE)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.Improve", user);
            }
            if (type == ISIConstants.ISI_TASK_TYPE_CHANGE)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.Change", user);
            }
            if (type == ISIConstants.ISI_TASK_TYPE_PRIVACY)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.Privacy", user);
            }
            if (type == ISIConstants.ISI_TASK_TYPE_RESPONSE)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.Response", user);
            }
            if (type == ISIConstants.ISI_TASK_TYPE_PROJECT)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.Project", user);
            }
            if (type == ISIConstants.ISI_TASK_TYPE_AUDIT)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.Audit", user);
            }
            if (type == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.PrjIss", user);
            }
            if (type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.Enc", user);
            }
            if (type == ISIConstants.ISI_TASK_TYPE_WORKFLOW)
            {
                return languageMgrE.TranslateMessage("ISI.TSK.WFS", user);
            }
            return languageMgrE.TranslateMessage("ISI.TSK.Task", user);
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class UserSubscriptionMgrE : com.Sconit.ISI.Service.Impl.UserSubscriptionMgr, IUserSubscriptionMgrE
    {
    }
}

#endregion Extend Class