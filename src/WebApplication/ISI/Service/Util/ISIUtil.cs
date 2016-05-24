using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using System.Text.RegularExpressions;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;

namespace com.Sconit.ISI.Service.Util
{
    public static class ISIUtil
    {
        /// <summary>
        /// 是否是手机号码
        /// </summary>
        /// <param name="val"></param>
        public static bool IsValidMobilePhone(string val)
        {
            return Regex.IsMatch(val, @"^1[358]\d{9}$", RegexOptions.IgnoreCase);
        }

        public static string GetAlias(string type, string code, DateTime effDate, string fileExtension)
        {
            string str = string.Empty;
            if (!string.IsNullOrEmpty(type))
            {
                int pos = type.LastIndexOf('.');
                if (pos != -1)
                {
                    str = type.Substring(pos + 1);
                }
                else
                {
                    str = type;
                }
            }
            string alias = str + "_" + code + "_" + effDate.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid() + fileExtension;
            return alias;
        }

        public static string GetPath(DateTime effDate, bool isTemplates)
        {
            int y = effDate.Year;
            string md = effDate.ToString("MM-dd");

            string path = "File/" + (isTemplates ? "Templates/" : string.Empty) + y + "/" + md + "/";
            return path;
        }

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }

        public static DetachedCriteria GetProcessInstanceCriteria(string userCode)
        {
            DetachedCriteria upSubCriteria = DetachedCriteria.For<ProcessInstance>();

            if (!string.IsNullOrEmpty(userCode))
            {
                upSubCriteria.Add(Expression.Eq("UserCode", userCode));
            }

            upSubCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("TaskCode")));

            return upSubCriteria;
        }

        public static DetachedCriteria GetProcessDefinitionCriteria(string userCode)
        {
            DetachedCriteria upSubCriteria = DetachedCriteria.For<ProcessDefinition>();

            if (!string.IsNullOrEmpty(userCode))
            {
                upSubCriteria.Add(Expression.Eq("UserCode", userCode));
            }

            upSubCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("TaskSubType")));

            return upSubCriteria;
        }
        public static DetachedCriteria GetProcessApplyCriteria(string code)
        {
            DetachedCriteria upSubCriteria = DetachedCriteria.For<ProcessApply>();

            if (!string.IsNullOrEmpty(code))
            {
                upSubCriteria.Add(Expression.Eq("Code", code));
            }

            upSubCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("TaskSubType")));

            return upSubCriteria;
        }

        public static DetachedCriteria GetTaskPermissionCriteria(string userCode, string taskCode)
        {
            DetachedCriteria taskCriteria = DetachedCriteria.For<TaskMstr>();
            if (!string.IsNullOrEmpty(taskCode))
            {
                taskCriteria.Add(Expression.Eq("Code", taskCode));
            }
            taskCriteria.CreateAlias("TaskSubType", "tst", NHibernate.SqlCommand.JoinType.InnerJoin);
            //taskCriteria.Add(Expression.Eq("tst.IsActive", true));

            taskCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("Code")));

            DetachedCriteria[] tstCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(userCode,
                            ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
            taskCriteria.Add(
                    Expression.Or(
                        Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                            Subqueries.PropertyIn("tst.Code", tstCrieteria[1])
                ));

            return taskCriteria;
        }

        public static DetachedCriteria[] GetUserPermissionCriteria(string taskSubType)
        {
            return GetUserPermissionCriteria(taskSubType, ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
        }

        public static DetachedCriteria[] GetUserPermissionCriteria(string taskSubType, string permissionCategory)
        {
            DetachedCriteria[] criteria = new DetachedCriteria[2];

            DetachedCriteria upSubCriteria = DetachedCriteria.For<UserPermission>();
            upSubCriteria.CreateAlias("User", "u");
            upSubCriteria.CreateAlias("Permission", "pm");
            upSubCriteria.CreateAlias("pm.Category", "pmc");
            upSubCriteria.Add(Expression.Eq("pmc.Type", ISIConstants.CODE_MASTER_PERMISSION_CATEGORY_TYPE_VALUE_ISI));
            if (!string.IsNullOrEmpty(permissionCategory))
            {
                upSubCriteria.Add(Expression.Eq("pmc.Code", permissionCategory));
            }
            upSubCriteria.Add(Expression.Eq("pm.Code", taskSubType));
            upSubCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("u.Code")));

            DetachedCriteria rpSubCriteria = DetachedCriteria.For<RolePermission>();
            rpSubCriteria.CreateAlias("Role", "r");
            rpSubCriteria.CreateAlias("Permission", "pm");
            rpSubCriteria.CreateAlias("pm.Category", "pmc");
            rpSubCriteria.Add(Expression.Eq("pmc.Type", ISIConstants.CODE_MASTER_PERMISSION_CATEGORY_TYPE_VALUE_ISI));
            if (!string.IsNullOrEmpty(permissionCategory))
            {
                rpSubCriteria.Add(Expression.Eq("pmc.Code", permissionCategory));
            }
            rpSubCriteria.Add(Expression.Eq("pm.Code", taskSubType));
            rpSubCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("r.Code")));

            DetachedCriteria urSubCriteria = DetachedCriteria.For<UserRole>();
            urSubCriteria.CreateAlias("User", "u");
            urSubCriteria.CreateAlias("Role", "r");

            urSubCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("u.Code")));

            urSubCriteria.Add(Subqueries.PropertyIn("r.Code", rpSubCriteria));

            criteria[0] = upSubCriteria;
            criteria[1] = urSubCriteria;

            return criteria;
        }

        public static DetachedCriteria[] GetTaskSubTypePermissionCriteria()
        {
            return GetTaskSubTypePermissionCriteria(string.Empty, ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
        }

        public static DetachedCriteria[] GetTaskSubTypePermissionCriteria(string userCode)
        {
            return GetTaskSubTypePermissionCriteria(userCode, ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
        }

        public static DetachedCriteria[] GetTaskSubTypePermissionCriteria(string userCode, string permissionCategory)
        {
            DetachedCriteria[] criteria = new DetachedCriteria[2];

            DetachedCriteria upSubCriteria = DetachedCriteria.For<UserPermission>();
            upSubCriteria.CreateAlias("User", "u");
            upSubCriteria.CreateAlias("Permission", "pm");
            upSubCriteria.CreateAlias("pm.Category", "pmc");
            upSubCriteria.Add(Expression.Eq("pmc.Type", ISIConstants.CODE_MASTER_PERMISSION_CATEGORY_TYPE_VALUE_ISI));
            if (!string.IsNullOrEmpty(permissionCategory))
            {
                upSubCriteria.Add(Expression.Eq("pmc.Code", permissionCategory));
            }
            if (!string.IsNullOrEmpty(userCode))
            {
                upSubCriteria.Add(Expression.Eq("u.Code", userCode));
            }
            upSubCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("pm.Code")));

            DetachedCriteria rpSubCriteria = DetachedCriteria.For<RolePermission>();
            rpSubCriteria.CreateAlias("Role", "r");
            rpSubCriteria.CreateAlias("Permission", "pm");
            rpSubCriteria.CreateAlias("pm.Category", "pmc");
            rpSubCriteria.Add(Expression.Eq("pmc.Type", ISIConstants.CODE_MASTER_PERMISSION_CATEGORY_TYPE_VALUE_ISI));
            if (!string.IsNullOrEmpty(permissionCategory))
            {
                rpSubCriteria.Add(Expression.Eq("pmc.Code", permissionCategory));
            }
            rpSubCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("pm.Code")));

            DetachedCriteria urSubCriteria = DetachedCriteria.For<UserRole>();
            urSubCriteria.CreateAlias("User", "u");
            urSubCriteria.CreateAlias("Role", "r");
            if (!string.IsNullOrEmpty(userCode))
            {
                urSubCriteria.Add(Expression.Eq("u.Code", userCode));
            }
            urSubCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("r.Code")));

            rpSubCriteria.Add(Subqueries.PropertyIn("r.Code", urSubCriteria));

            criteria[0] = upSubCriteria;
            criteria[1] = rpSubCriteria;

            return criteria;
        }

        //查找所有管理员
        public static DetachedCriteria[] GetTaskAdminPermissionCriteria()
        {
            DetachedCriteria[] criteria = new DetachedCriteria[2];

            DetachedCriteria upSubCriteria = DetachedCriteria.For<UserPermission>();
            upSubCriteria.CreateAlias("User", "u");
            upSubCriteria.CreateAlias("Permission", "pm");
            upSubCriteria.CreateAlias("pm.Category", "pmc"); ;
            upSubCriteria.Add(Expression.Eq("pmc.Code", ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKADMIN));
            upSubCriteria.Add(Expression.In("pm.Code", new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN }));
            upSubCriteria.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.GroupProperty("u.Code"))));

            DetachedCriteria rpSubCriteria = DetachedCriteria.For<RolePermission>();
            rpSubCriteria.CreateAlias("Role", "r");
            rpSubCriteria.CreateAlias("Permission", "pm");
            rpSubCriteria.CreateAlias("pm.Category", "pmc"); ;
            rpSubCriteria.Add(Expression.Eq("pmc.Code", ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKADMIN));
            rpSubCriteria.Add(Expression.In("pm.Code", new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN }));
            rpSubCriteria.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.GroupProperty("r.Code"))));

            DetachedCriteria urSubCriteria = DetachedCriteria.For<UserRole>();
            urSubCriteria.CreateAlias("User", "u");
            urSubCriteria.CreateAlias("Role", "r");
            urSubCriteria.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.GroupProperty("u.Code"))));
            urSubCriteria.Add(Subqueries.PropertyIn("r.Code", rpSubCriteria));

            criteria[0] = upSubCriteria;
            criteria[1] = urSubCriteria;

            return criteria;
        }

        public static bool Contains(string value1, string value2)
        {
            if (string.IsNullOrEmpty(value1) || string.IsNullOrEmpty(value2)) return false;

            string[] users = value2.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
            foreach (var user in users)
            {
                if (value1.Contains(ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR)
                    || value1.Contains(ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR)
                    || value1.Contains(ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR)
                    || value1.Contains(ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR)
                    || value1 == user
                    || value1.StartsWith(user + ISIConstants.ISI_USER_SEPRATOR)
                    || value1.EndsWith(ISIConstants.ISI_USER_SEPRATOR + user))
                {
                    return true;
                }
            }
            return false;
        }

        public static string ShowUser(string users)
        {
            if (!string.IsNullOrEmpty(users))
            {
                if (users.StartsWith(ISIConstants.ISI_LEVEL_SEPRATOR)
                         || users.EndsWith(ISIConstants.ISI_LEVEL_SEPRATOR))
                {
                    string u = users.Substring(1, users.Length - 2);
                    u = u.Replace(",", ", ");
                    return u;
                }
                else
                {
                    return users;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string EditUser(string users)
        {
            if (!string.IsNullOrEmpty(users))
            {
                if (users.StartsWith(ISIConstants.ISI_LEVEL_SEPRATOR)
                         && users.EndsWith(ISIConstants.ISI_LEVEL_SEPRATOR))
                {
                    string u = users.Substring(1, users.Length - 2);
                    //u = u.Replace(",", ", ");
                    return u;
                }
                else
                {
                    return users;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetUser(string users)
        {
            if (!string.IsNullOrEmpty(users))
            {
                string u = users.Replace(" ", "");
                return ISIConstants.ISI_LEVEL_SEPRATOR + u + ISIConstants.ISI_LEVEL_SEPRATOR;
            }
            return string.Empty;
        }
        public static string GetUserName(string assignStartUserNm, string userName, string color)
        {
            return GetUserName(assignStartUserNm, userName, null, string.Empty, string.Empty, color, string.Empty);
        }
        public static string GetUserName(string assignStartUserNm, string userName, int? level, string approvalLevel, string color, string approvalColor)
        {
            return GetUserName(assignStartUserNm, userName, level, approvalLevel, string.Empty, color, approvalColor);
        }
        public static string GetUserName(string assignStartUserNm, string userName, int? level, string approvalLevel, string lastApprovalUser, string color, string approvalColor)
        {
            if (string.IsNullOrEmpty(assignStartUserNm)) return string.Empty;
            else
            {
                StringBuilder html = new StringBuilder();
                var userNames = assignStartUserNm.Split(ISIConstants.ISI_USERNAME_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                var approvalLevels = approvalLevel != null ? approvalLevel.Split(ISIConstants.ISI_USERNAME_SEPRATOR, StringSplitOptions.RemoveEmptyEntries) : new string[0];
                for (int i = 0; i < userNames.Length; i++)
                {
                    var u = userNames[i];
                    html.Append("<div>");
                    if (u == userName)
                    {
                        html.Append("<span style='color:" + color + ";'>");
                    }
                    if (level.HasValue && level.Value != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE && approvalLevels != null && approvalLevels.Length > 0 && level.Value == int.Parse(approvalLevels[i]) && !(approvalLevels.Where(a => int.Parse(a) == level.Value).Count() > 1 && u == lastApprovalUser))
                    {
                        html.Append("<span style='color:" + approvalColor + ";'>");
                    }
                    html.Append(u);
                    if (level.HasValue && level.Value != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE && approvalLevels != null && approvalLevels.Length > 0 && level.Value == int.Parse(approvalLevels[i]) && !(approvalLevels.Where(a => int.Parse(a) == level.Value).Count() > 1 && u == lastApprovalUser))
                    {
                        html.Append("</span>");
                    }
                    if (u == userName)
                    {
                        html.Append("</span>");
                    }
                    html.Append("</div>");
                }
                return html.ToString();
            }
        }

        public static string GetUserMerge(string userCodes, string userNames)
        {
            if (userCodes.Length == 0 || userNames.Length == 0) return string.Empty;
            string[] userCodeArray = userCodes.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
            string[] userNameArray = userNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (userCodeArray.Length == 0 || userNameArray.Length == 0 || userCodeArray.Length != userNameArray.Length) return string.Empty;

            StringBuilder userCodeName = new StringBuilder();
            for (int i = 0; i < userNameArray.Length; i++)
            {
                if (userCodeName.Length != 0)
                {
                    userCodeName.Append(",");
                }
                userCodeName.Append(userCodeArray[i].Trim() + "[" + userNameArray[i].Trim() + "]");
            }
            return userCodeName.ToString();
        }

        public static string[] GetUserSplit(string users)
        {
            string[] userArr = users.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();

            if (userArr == null || userArr.Length == 0)
            {
                return new string[0];
            }
            StringBuilder userCodes = new StringBuilder();
            StringBuilder userNames = new StringBuilder();
            foreach (var u in userArr)
            {
                var t = u.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                if (t.Length >= 1)
                {
                    if (userCodes.Length > 0)
                    {
                        userCodes.Append(",");
                    }
                    userCodes.Append(t[0]);
                }
                else
                {
                    continue;
                }

                if (t.Length >= 2)
                {
                    if (userNames.Length > 0)
                    {
                        userNames.Append(", ");
                    }
                    userNames.Append(t[1]);
                }
            }
            return new string[] { userCodes.ToString(), userNames.ToString() };
        }

        public static string GetDiff(DateTime dateTime)
        {
            return GetDiff(DateTime.Now, dateTime);
        }

        public static string GetDiff(DateTime dt1, DateTime dt2)
        {
            TimeSpan ts1 = new TimeSpan(dt1.Ticks);
            TimeSpan ts2 = new TimeSpan(dt2.Ticks);
            TimeSpan diff = ts1.Subtract(ts2).Duration();
            if (diff.TotalMilliseconds > 0)
            {
                return GetDiff(diff);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetDiff(TimeSpan diff)
        {
            return GetDiff(diff, false, false);
        }

        public static string GetDiff(TimeSpan diff, bool isSeconds, bool isMilliseconds)
        {
            StringBuilder msg = new StringBuilder();
            if (diff.Days != 0)
            {
                msg.Append(diff.Days + "天");
            }
            if (diff.Hours != 0)
            {
                msg.Append(diff.Hours + "小时");
            }
            if (diff.Minutes != 0)
            {
                msg.Append(diff.Minutes + "分");
            }
            if (isSeconds)
            {
                if (diff.Seconds != 0)
                {
                    msg.Append(diff.Seconds + "秒");
                }
            }
            if (isMilliseconds)
            {
                if (diff.Milliseconds != 0)
                {
                    msg.Append(diff.Milliseconds + "毫秒");
                }
            }
            return msg.ToString();
        }

        public static string GetSerialNo(string taskCode)
        {
            return int.Parse(taskCode.Substring(3)).ToString();
        }

        public static string GetTaskCode(string serialNo)
        {
            return serialNo.PadLeft(9, '0');
        }

        #region 获取指定长度的中英文混合字符串
        /// <summary>
        /// 获取指定长度的中英文混合字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="len">要截取的长度</param>
        /// <returns></returns>
        public static string GetStrLength(string str, int len)
        {
            if (string.IsNullOrEmpty(str)) return str;
            string result = string.Empty;// 最终返回的结果
            int byteLen = System.Text.Encoding.Default.GetByteCount(str);// 单字节字符长度
            int charLen = str.Length;// 把字符平等对待时的字符串长度
            int byteCount = 0;// 记录读取进度
            int pos = 0;// 记录截取位置
            len -= 4;
            if (byteLen > len)
            {
                for (int i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                    {
                        byteCount += 2;
                    }
                    else// 按英文字符计算加1
                    {
                        byteCount += 1;
                    }
                    if (byteCount > len)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == len)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }
                if (pos >= 0)
                {
                    result = str.Substring(0, pos) + "...";
                }
            }
            else
            {
                result = str;
            }

            return result;
        }
        #endregion


        /// <summary>
        /// 取得某月的第一天
        /// </summary>
        /// <param name="datetime">要取得月份第一天的时间</param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }

        /**/
        /// <summary>
        /// 取得某月的最后一天
        /// </summary>
        /// <param name="datetime">要取得月份最后一天的时间</param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddMilliseconds(-1);
        }


        public static void AppendTestText(string companyName, StringBuilder content, string separator)
        {
            bool isTest = false;
            StringBuilder text = new StringBuilder();
            if (companyName.ToLower().Contains("test"))
            {
                isTest = true;
            }

            if (isTest)
            {
                if (separator == ISIConstants.SMS_SEPRATOR)
                {
                    text.Append("测试短信");
                    text.Append(separator);
                }
                else
                {
                    text.Append("<span style='font-size:13px;color:#0000E5;'>注&#58;&nbsp;此邮件从测试系统发出&#44;&nbsp;请忽略&#46;</span>");
                    text.Append(separator);
                    text.Append(separator);
                }
            }

            content.Insert(0, text);
        }

        public static void GetReportDetailBody(StringBuilder mailDetailBody, TaskView task, DateTime endDate, IDictionary<string, UserSub> userDic, IDictionary<string, string> statusCodeMstrList)
        {

            mailDetailBody.Append("<tr>");
            mailDetailBody.Append("<td><span");
            if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
            {
                mailDetailBody.Append(" style='color:red' ");
            }
            mailDetailBody.Append(">" + task.Code + "</span><br/>" + task.Subject + (task.Type == ISIConstants.ISI_TASK_TYPE_PROJECT && !string.IsNullOrEmpty(task.TaskType) ? "<br/>[" + task.TaskType + "]" : string.Empty) + "</td>");
            mailDetailBody.Append("<td>");

            if (!string.IsNullOrEmpty(task.Desc1))
            {
                mailDetailBody.Append(task.Desc1.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
            }
            if (!string.IsNullOrEmpty(task.Desc2))
            {
                mailDetailBody.Append((!string.IsNullOrEmpty(task.Desc1) ? "<br/>" : string.Empty) + "<span style='color:#0000E5;'>补充描述</span>&#58;&nbsp;");
                mailDetailBody.Append(task.Desc2.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));

                if (task.RefTaskCount.HasValue && task.RefTaskCount.Value > 0)
                {
                    mailDetailBody.Append("<br/><span style='color:#0000E5;'>相关任务</span>&#58;&nbsp;" + task.RefTaskCount.Value);
                }
            }
            mailDetailBody.Append("</td>");

            mailDetailBody.Append("<td>");
            if (!string.IsNullOrEmpty(task.ExpectedResults))
            {
                mailDetailBody.Append(task.ExpectedResults.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
            }
            mailDetailBody.Append("</td>");
            mailDetailBody.Append("<td nowrap>" + task.SubmitUserNm);

            if (task.Type == ISIConstants.ISI_TASK_TYPE_PROJECT)
            {
                if (!string.IsNullOrEmpty(task.Phase))
                {
                    mailDetailBody.Append("<br>" + task.Phase);
                }
                if (!string.IsNullOrEmpty(task.Seq))
                {
                    mailDetailBody.Append("<br>" + task.Seq);
                }
            }
            mailDetailBody.Append("</td>");
            mailDetailBody.Append("<td nowrap>" + task.AssignUserNm + "<br>" + statusCodeMstrList[task.Status] + "</td>");
            mailDetailBody.Append("<td nowrap>");

            if (userDic != null && userDic.Count > 0 && !string.IsNullOrEmpty(task.StartedUser))
            {
                string[] userCodes = task.StartedUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                if (userCodes != null && userCodes.Length > 0)
                {
                    for (int i = 0; i < userCodes.Length; i++)
                    {
                        if (userDic.Keys.Contains(userCodes[i]))
                        {
                            if (i != 0)
                            {
                                mailDetailBody.Append("<br>");
                            }
                            mailDetailBody.Append(userDic[userCodes[i]].Name);
                        }
                    }
                }
            }
            mailDetailBody.Append("</td>");
            mailDetailBody.Append("<td nowrap>" + (task.StatusDate.HasValue ? (task.StatusDate.Value).ToString("yyyy-MM-dd<br>HH:mm") : string.Empty) + "</td>");
            mailDetailBody.Append("<td>");
            if (!string.IsNullOrEmpty(task.CreateUserNm))
            {
                mailDetailBody.Append("<span style='color:#0000E5;'>" + task.CreateUserNm + "</span>&#58;&nbsp;" + task.StatusDesc + (task.StatusCount.HasValue && task.StatusCount.Value > 1 ? "<span style='color:#0000E5;'>&#40;" + task.StatusCount.Value + "&#41;</span>" : string.Empty));
            }
            mailDetailBody.Append("</td>");
            mailDetailBody.Append("<td nowrap style='background-color:" + task.Color + ";'>" + task.Flag + "</td>");
            mailDetailBody.Append("<td>");
            if (task.CommentCreateDate.HasValue && !string.IsNullOrEmpty(task.CommentCreateUserNm))
            {
                mailDetailBody.Append("<span style='color:#0000E5;'>" + task.CommentCreateUserNm + "&#40;" + task.CommentCreateDate.Value.ToString("yyyy-MM-dd HH:mm") + "&#41;</span>&#58;&nbsp;" + task.Comment.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") + (task.CommentCount.HasValue && task.CommentCount.Value > 1 ? "<span style='color:#0000E5;'>&#40;" + task.CommentCount.Value + "&#41;</span>" : string.Empty));
            }
            mailDetailBody.Append("</td>");
            mailDetailBody.Append("<td nowrap >" + (task.AttachmentCount.HasValue && task.AttachmentCount.Value != 0 ? task.AttachmentCount.Value.ToString() : string.Empty) + "</td>");
            mailDetailBody.Append("<td nowrap >");
            if (task.PlanCompleteDate.HasValue)
            {
                if (task.PlanCompleteDate.Value < endDate)
                {
                    mailDetailBody.Append("<span style='color:red'>" + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd<br>HH:mm") + "</span>");
                }
                else
                {
                    mailDetailBody.Append(task.PlanCompleteDate.Value.ToString("yyyy-MM-dd<br>HH:mm"));
                }
            }
            mailDetailBody.Append("</td>");
            mailDetailBody.Append("</tr>");
        }

        public static void GetColumnHead(StringBuilder taskListBody)
        {
            taskListBody.Append(
                "<table cellspacing='0' cellpadding='4' rules='all' border='1' style='width:100%;border-collapse:collapse;font-size:12px;'>");
            taskListBody.Append("<tr nowrap style='color:#FFFFFF;background-color:#000060;font-weight:bold;line-height:150%;'>");
            taskListBody.Append("<th nowrap scope='col'>任务主题</th>");
            taskListBody.Append("<th nowrap scope='col'>描述</th>");
            taskListBody.Append("<th nowrap scope='col'>预期结果/达成结果</th>");
            taskListBody.Append("<th nowrap scope='col'>提交人</th>");
            taskListBody.Append("<th nowrap scope='col'>分派人</th>");
            taskListBody.Append("<th nowrap scope='col'>责任人</th>");
            taskListBody.Append("<th nowrap scope='col'>跟踪日期</th>");
            taskListBody.Append("<th nowrap scope='col'>进展</th>");
            taskListBody.Append("<th nowrap scope='col'>标志</th>");
            taskListBody.Append("<th nowrap scope='col'>最新评论</th>");
            taskListBody.Append("<th nowrap scope='col'>附件</th>");
            taskListBody.Append("<th nowrap scope='col'>预计完成时间</th>");
            taskListBody.Append("</tr>");
        }
        public static string GetHtmlBody(string targetContent)
        {
            return SetHighlight(targetContent, false, string.Empty);
        }
        public static string SetHighlight(string targetContent, bool isHighlight, string key)
        {
            if (!string.IsNullOrEmpty(targetContent))
            {
                //targetContent = targetContent.Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray()).Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>");
                targetContent = targetContent.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>");
            }
            if (isHighlight && !string.IsNullOrEmpty(key))
            {
                targetContent = targetContent.Replace(key, "<span style='color:blue;'><b>" + key + "</b></span>");
            }
            return targetContent;
        }

        public static string GetHide(string code, string targetContent)
        {
            if (!string.IsNullOrEmpty(code) && targetContent.Length > 360)
            {
                var con1 = targetContent.Substring(0, 300);
                var con2 = targetContent.Substring(300, targetContent.Length - 300);
                return con1 + "<span  id = 'showStatusDescHideDiv" + code + "' style='display:none;'>" + con2 + "</span><a onclick=\"showHide('" + "showStatusDescHideDiv" + code + "')\"'>Click</a>";
            }
            else
            {
                return targetContent;
            }
        }
        public static string FormatUser(string userCode, int seq)
        {
            StringBuilder str = new StringBuilder();
            if (seq == 1)
            {
                str.Append(ISIConstants.ISI_LEVEL_SEPRATOR);
                str.Append(userCode);
                str.Append(ISIConstants.ISI_USER_SEPRATOR);
            }
            else if (seq == 2)
            {
                str.Append(ISIConstants.ISI_USER_SEPRATOR);
                str.Append(userCode);
                str.Append(ISIConstants.ISI_USER_SEPRATOR);
            }
            else if (seq == 3)
            {
                str.Append(ISIConstants.ISI_USER_SEPRATOR);
                str.Append(userCode);
                str.Append(ISIConstants.ISI_LEVEL_SEPRATOR);
            }
            else if (seq == 4)
            {
                str.Append(ISIConstants.ISI_LEVEL_SEPRATOR);
                str.Append(userCode);
                str.Append(ISIConstants.ISI_LEVEL_SEPRATOR);
            }
            else
            {
                return userCode;
            }
            return str.ToString();
        }
        public static void SetNoVierUserCriteria(DetachedCriteria criteria, User user)
        {
            string[] propertyNames = new string[] { "CreateUser", string.Empty, "AssignStartUser", "SchedulingStartUser", "AssignUpUser", "StartUpUser", "CloseUpUser", "TaskSubTypeAssignUser", "ViewUser", "ECUser", "Type", "ApprovalUser", "TaskSubTypeCode" };
            SetNoVierUserCriteria(criteria, user, propertyNames);
        }

        public static IList<TaskApply> GetTaskApplyUOMList(IList<TaskApply> taskApplyList)
        {
            if (taskApplyList != null)
            {
                return taskApplyList.Where(p => !(string.IsNullOrEmpty(p.UOM) && (string.IsNullOrEmpty(p.Type) || p.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || p.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO))).ToList();
            }
            else
            {
                return new List<TaskApply>();
            }
        }

        public static IList<TaskApply> GetTaskApplyNotUOMList(IList<TaskApply> taskApplyList)
        {
            if (taskApplyList != null)
            {
                return taskApplyList.Where(p => string.IsNullOrEmpty(p.UOM) && (string.IsNullOrEmpty(p.Type) || p.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || p.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)).ToList();
            }
            else
            {
                return new List<TaskApply>();
            }
        }

        public static IList<ProcessApply> GetProcessApplyUOMList(IList<ProcessApply> processApplyList)
        {
            if (processApplyList != null)
            {
                return processApplyList.Where(p => !(string.IsNullOrEmpty(p.UOM) && (string.IsNullOrEmpty(p.Type) || p.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || p.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO))).ToList();
            }
            else
            {
                return new List<ProcessApply>();
            }
        }

        public static IList<ProcessApply> GetProcessApplyNotUOMList(IList<ProcessApply> processApplyList)
        {
            if (processApplyList != null)
            {
                return processApplyList.Where(p => string.IsNullOrEmpty(p.UOM) && (string.IsNullOrEmpty(p.Type) || p.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || p.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)).ToList();
            }
            else
            {
                return new List<ProcessApply>();
            }
        }

        public static string GetDesc(string desc1, string desc2)
        {
            if (!string.IsNullOrEmpty(desc1))
            {
                return desc1;
            }
            else if (!string.IsNullOrEmpty(desc2))
            {
                return desc2;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetModuleType(string taskCode)
        {
            return ISIConstants.TaskTypeDic[taskCode.Substring(0, 3)];
        }
        public static void SetNoVierUserCriteria(DetachedCriteria criteria, User user, string[] propertyNames)
        {
            string userCode = user.Code;

            string[] typePermissions = ISIConstants.TaskTypeList.Where(type => user.HasPermission(type)).ToArray();

            DetachedCriteria[] tstCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(userCode);

            //criteria.Add(Expression.Or(Expression.And(Expression.Eq("IsWF", true), Subqueries.PropertyIn(propertyNames[11], ISIUtil.GetProcessInstanceCriteria(user.Code))),
            criteria.Add(Expression.Or(Expression.And(Expression.Eq("IsWF", true),
                                                Expression.Or(Expression.Like(propertyNames[11], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                        Expression.Or(Expression.Like(propertyNames[11], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Like(propertyNames[11], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                    Expression.Or(Expression.Like(propertyNames[11], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                    Expression.Eq(propertyNames[11], userCode)))))),
                                Expression.Or(Expression.In(propertyNames[10], typePermissions),
                                    Expression.Or(Expression.Eq(propertyNames[0], userCode),
                                                            Expression.Or(Expression.And(Expression.Not(Expression.Eq(propertyNames[2], string.Empty)),
                                                                                            Expression.And(Expression.IsNotNull(propertyNames[2]),
                                                                                                        Expression.Or(Expression.Like(propertyNames[2], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like(propertyNames[2], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like(propertyNames[2], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[2], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                Expression.Eq(propertyNames[2], userCode))))))),
                                                                            Expression.Or(Expression.And(Expression.And(Expression.IsNotNull(propertyNames[3]), Expression.Not(Expression.Eq(propertyNames[3], string.Empty))),
                                                                                                                        Expression.And(Expression.Or(Expression.IsNull(propertyNames[2]), Expression.Eq(propertyNames[2], string.Empty)),
                                                                                                                                        Expression.Or(Expression.Like(propertyNames[3], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like(propertyNames[3], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[3], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[3], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Eq(propertyNames[3], userCode))))))),
                                                                                            Expression.Or(Expression.Or(Expression.Like(propertyNames[4], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like(propertyNames[4], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like(propertyNames[4], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[4], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                    Expression.Eq(propertyNames[4], userCode))))),
                                                                                                            Expression.Or(Expression.Or(Expression.Like(propertyNames[5], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like(propertyNames[5], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[5], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[5], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                Expression.Eq(propertyNames[5], userCode))))),
                                                                                                                        Expression.Or(Expression.Or(Expression.Like(propertyNames[6], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like(propertyNames[6], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[6], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[6], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Eq(propertyNames[6], userCode))))),
                                                                                                                                        Expression.Or(Expression.Or(Expression.Like(propertyNames[7], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                                                             Expression.Or(Expression.Like(propertyNames[7], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                                                           Expression.Or(Expression.Like(propertyNames[7], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                                                         Expression.Or(Expression.Like(propertyNames[7], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                                                       Expression.Eq(propertyNames[7], userCode))))),
                                                                                                                                                            Expression.Or(Expression.Like(propertyNames[8], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                                                     Expression.Or(Expression.Like(propertyNames[8], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                                                   Expression.Or(Expression.Like(propertyNames[8], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                                                 Expression.Or(Expression.Like(propertyNames[8], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                                               Expression.Or(Expression.Eq(propertyNames[8], userCode),
                                                                                                                                                                                                                    Expression.Or(
                                                                                                                                                                                                                        Expression.Or(Expression.Like(propertyNames[9], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                                                                                                             Expression.Or(Expression.Like(propertyNames[9], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                                                                                                           Expression.Or(Expression.Like(propertyNames[9], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                                                                                                         Expression.Or(Expression.Like(propertyNames[9], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                                                                                                       Expression.Eq(propertyNames[9], userCode))))),
                                                                                                                                                                                                                                                                               Expression.Or(
                                                                                                                                                                                                                                                                                    Subqueries.PropertyIn(propertyNames[12], tstCrieteria[0]),
                                                                                                                                                                                                                                                                                        Subqueries.PropertyIn(propertyNames[12], tstCrieteria[1]))
                                                                                                                                                                                                                                                                       )))))
                                                                                                                                                                                                               )
                                                                                                                                                                                                ))))))))));
        }

        public static void SetVierUserCriteria(DetachedCriteria criteria, string userCode, string[] propertyNames)
        {
            DetachedCriteria[] tstCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(userCode);

            criteria.Add(Expression.Or(Expression.Not(Expression.Eq(propertyNames[0], ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq(propertyNames[0], ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                        Expression.Or(Expression.Eq(propertyNames[1], userCode),
                                                            Expression.Or(Expression.And(Expression.Not(Expression.Eq(propertyNames[3], string.Empty)),
                                                                                            Expression.And(Expression.IsNotNull(propertyNames[3]),
                                                                                                        Expression.Or(Expression.Like(propertyNames[3], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like(propertyNames[3], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like(propertyNames[3], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[3], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                Expression.Eq(propertyNames[3], userCode))))))),
                                                                            Expression.Or(Expression.And(Expression.And(Expression.IsNotNull(propertyNames[4]), Expression.Not(Expression.Eq(propertyNames[4], string.Empty))),
                                                                                                                        Expression.And(Expression.Or(Expression.IsNull(propertyNames[3]), Expression.Eq(propertyNames[3], string.Empty)),
                                                                                                                                        Expression.Or(Expression.Like(propertyNames[4], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like(propertyNames[4], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[4], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[4], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Eq(propertyNames[4], userCode))))))),
                                                                                            Expression.Or(Expression.Or(Expression.Like(propertyNames[5], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like(propertyNames[5], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like(propertyNames[5], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[5], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                    Expression.Eq(propertyNames[5], userCode))))),
                                                                                                            Expression.Or(Expression.Or(Expression.Like(propertyNames[6], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like(propertyNames[6], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[6], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[6], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                Expression.Eq(propertyNames[6], userCode))))),
                                                                                                                        Expression.Or(Expression.Or(Expression.Like(propertyNames[7], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like(propertyNames[7], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[7], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Or(Expression.Like(propertyNames[7], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Eq(propertyNames[7], userCode))))),
                                                                                                                                        Expression.Or(Expression.Or(Expression.Like(propertyNames[8], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                                                             Expression.Or(Expression.Like(propertyNames[8], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                                                           Expression.Or(Expression.Like(propertyNames[8], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                                                         Expression.Or(Expression.Like(propertyNames[8], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                                                       Expression.Eq(propertyNames[8], userCode))))),
                                                                                                                                                            Expression.Or(Expression.Like(propertyNames[9], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                                                     Expression.Or(Expression.Like(propertyNames[9], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                                                   Expression.Or(Expression.Like(propertyNames[9], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                                                 Expression.Or(Expression.Like(propertyNames[9], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                                               Expression.Or(Expression.Eq(propertyNames[9], userCode),
                                                                                                                                                                                                                    Expression.Or(
                                                                                                                                                                                                                        Expression.Or(Expression.Like(propertyNames[10], FormatUser(userCode, 1), MatchMode.Anywhere),
                                                                                                                                                                                                                                     Expression.Or(Expression.Like(propertyNames[10], FormatUser(userCode, 2), MatchMode.Anywhere),
                                                                                                                                                                                                                                                   Expression.Or(Expression.Like(propertyNames[10], FormatUser(userCode, 3), MatchMode.Anywhere),
                                                                                                                                                                                                                                                                 Expression.Or(Expression.Like(propertyNames[10], FormatUser(userCode, 4), MatchMode.Anywhere),
                                                                                                                                                                                                                                                                              Expression.Eq(propertyNames[10], userCode))))),
                                                                                                                                                                                                                                                                                        Expression.Or(Subqueries.PropertyIn(propertyNames[11], tstCrieteria[0]),
                                                                                                                                                                                                                                                                                                            Subqueries.PropertyIn(propertyNames[11], tstCrieteria[1]))
                                                                                                                                                                                                                                                                              ))))))))))))))));


        }


        public static string GetLevelDesc(int? level)
        {
            if (level.HasValue)
            {
                if (level.Value == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
                {
                    return "${" + level.Value + "}</font>";
                }
                else
                {
                    return "<font color='blue'>${" + level.Value / ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL * ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL + "}</font>";
                }
            }
            return string.Empty;
        }
    }

}
