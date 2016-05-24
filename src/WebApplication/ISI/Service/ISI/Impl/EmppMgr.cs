using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using NHibernate.Expression;
using EMPPLib;
using System.Threading;
using com.Sconit.Utility;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Service.Impl;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using System.Linq;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class EmppMgr : IEmppMgr
    {
        /*
       string SMSHost = "211.136.163.68";
       int SMSPort = 9981;//
       string SMSAccountId = "10657109041343001";
       string SMSServiceId = "10657109041343";
       string SMSPassword = "Iloveyanxin"; */

        private string EMPPHost;
        private int EMPPPort;
        private string EMPPAccountId;
        private string EMPPServiceId;
        private string EMPPPassword;
        private EMPPLib.emptcl empp = null;
        private EMPPLib.ConnectResultEnum result = ConnectResultEnum.CONNECT_OTHER_ERROR;
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        public IEmppDetailMgrE emppDetailMgrE { get; set; }
        public ITaskMgrE taskMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }
        public ILanguageMgrE languageMgrE { get; set; }

        public EmppMgr() { }

        public EmppMgr(string emppHost, string emppPort, string emppAccountId, string emppServiceId, string emppPassword)
        {
            try
            {
                this.EMPPPort = string.IsNullOrEmpty(emppPort) ? 0 : int.Parse(emppPort);
                this.EMPPHost = emppHost;
                this.EMPPAccountId = emppAccountId;
                this.EMPPPassword = emppPassword;
                this.EMPPServiceId = emppServiceId;

                empp = new EMPPLib.emptclClass();
                log.Debug("���ǽ��뵽createPro��������**********************************");
                empp.EMPPClosed += (new _IemptclEvents_EMPPClosedEventHandler(EMPPClosed));
                empp.EMPPConnected += (new _IemptclEvents_EMPPConnectedEventHandler(EMPPConnected));
                empp.MessageReceivedInterface += (new _IemptclEvents_MessageReceivedInterfaceEventHandler(MessageReceivedInterface));
                empp.SocketClosed += (new _IemptclEvents_SocketClosedEventHandler(SocketClosed));
                empp.StatusReceivedInterface += (new _IemptclEvents_StatusReceivedInterfaceEventHandler(StatusReceivedInterface));
                empp.SubmitRespInterface += (new _IemptclEvents_SubmitRespInterfaceEventHandler(SubmitRespInterface));
                empp.needStatus = true;

                if (EMPPHost != string.Empty && EMPPPort != 0 && EMPPAccountId != string.Empty
                        && EMPPServiceId != string.Empty && EMPPPassword != string.Empty)
                {
                    try
                    {
                        log.Debug("INDE �����״ν������ӿ�ʼ");
                        result = this.empp.connect(EMPPHost, this.EMPPPort, EMPPAccountId, EMPPPassword);
                    }
                    catch (Exception ex)
                    {
                        log.Error("INDE �����״ν������ӿ�ʼ����ʧ�ܸ���");
                        log.Error(ex.Message, ex);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Send(string mobilePhones,
                            string msg,
                            User user)
        {
            try
            {
                if (EMPPHost != string.Empty && EMPPPort != 0 && EMPPAccountId != string.Empty
                        && EMPPServiceId != string.Empty && EMPPPassword != string.Empty)
                {
                    //createPro(this.empp);

                    int con = 0;
                    log.Debug("result = " + result);
                    while (result != EMPPLib.ConnectResultEnum.CONNECT_OK && result != EMPPLib.ConnectResultEnum.CONNECT_KICKLAST)
                    {
                        log.Debug("�����״�����ʧ�ܣ�����������while����----------" + con);
                        con++;
                        try
                        {
                            result = this.empp.connect(EMPPHost, EMPPPort, EMPPAccountId, EMPPPassword);
                        }
                        catch (Exception ex)
                        {
                            log.Warn(ex.Message, ex);
                        }
                    }
                    IList<EMPPLib.Mobiles> mobList = new List<EMPPLib.Mobiles>();
                    EMPPLib.Mobiles mobs = new EMPPLib.MobilesClass();
                    mobList.Add(mobs);
                    string[] mPhones = mobilePhones.Split(ISIConstants.ISI_SEPRATOR).Distinct().ToArray();

                    foreach (string mPhone in mPhones)
                    {
                        if (!string.IsNullOrEmpty(mPhone) && ISIUtil.IsValidMobilePhone(mPhone))
                        {
                            bool noHas = true;
                            for (int i = 0; i < mobs.count; i++)
                            {
                                if (mobs.get_Item(i) == mPhone)
                                {
                                    noHas = false;
                                    break;
                                }
                            }
                            if (noHas)
                            {
                                if (mobs.count < 99)
                                {
                                    mobs.Add(mPhone);
                                }
                                else
                                {
                                    mobs = new EMPPLib.MobilesClass();
                                    mobs.Add(mPhone);
                                    mobList.Add(mobs);
                                }
                            }
                        }
                    }

                    //Thread.Sleep(3000);
                    //msg = msg.Substring(0, 60);
                    foreach (EMPPLib.Mobiles mobs1 in mobList)
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            EMPPLib.ShortMessage shortMsg = new EMPPLib.ShortMessageClass();
                            shortMsg.srcID = EMPPAccountId;
                            shortMsg.ServiceID = EMPPServiceId;
                            shortMsg.needStatus = true;
                            shortMsg.DestMobiles = mobs1;
                            //shortMsg.content ="lujia  " + i + "  lujia    " +  msg + ":" + "��ʱ��" + DateTime.Now.ToString() + "��";
                            //shortMsg.SequenceID = new Random().Next(100000);
                            shortMsg.content = msg;
                            log.Debug("���Ǵ�ӡԭʼ�Ķ������ݣ�" + msg);
                            shortMsg.SendNow = true;

                            if (empp != null && empp.connected == true)
                            {
                                log.Debug("�������Ͷ���" + i + "diaoyong");
                                log.Debug("���ڵ�����״����: " + empp.connected);

                                log.Debug(ISIConstants.TEXT_SEPRATOR);
                                log.Debug("Ŀǰ������״���ǣ�" + empp.connected);
                                log.Debug("Ŀǰ�Ķ��ŵ�seqid�ǣ�" + shortMsg.SequenceID);
                                log.Debug("the empp.sequceid:" + empp.SequenceID);
                                log.Debug("IF���   ���Ǽ������Ͷ��ţ�" + shortMsg.content);
                                log.Debug("IF���   empp.MsgID:" + empp.MsgID);
                                empp.submit(shortMsg);

                                log.Debug("IF���   �����Ѿ����Ͷ��ţ�" + shortMsg.content);
                                log.Debug("�����ύ����" + i);
                                log.Debug("end empp.MsgID:" + empp.MsgID);
                                log.Debug("end empp.SequenceID:" + empp.SequenceID);
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                log.Debug("�����Ѿ��رգ����Ǽ����������ӣ�����������else�������");
                                Reconnect2();
                                log.Debug("�������Ͷ���" + i + "diaoyong");
                                log.Debug("���ڵ�����״����: " + empp.connected);

                                log.Debug(ISIConstants.TEXT_SEPRATOR);
                                log.Debug("Ŀǰ������״���ǣ�" + empp.connected);
                                log.Debug("Ŀǰ��shortmsgseqid�ǣ�" + shortMsg.SequenceID);
                                log.Debug("Ŀǰ��emppseqid�ǣ�" + empp.SequenceID);

                                log.Debug("ELSE���   ���Ǽ������Ͷ��ţ�" + msg);
                                log.Debug("ELSE���   empp.MsgID:" + empp.MsgID);
                                empp.submit(shortMsg);
                                log.Debug("ELSE���   �����Ѿ����Ͷ��ţ�" + msg);

                                log.Debug("�����ύ����" + i);
                                log.Debug("end empp.MsgID:" + empp.MsgID);
                                log.Debug("end empp.SequenceID:" + empp.SequenceID);
                                log.Debug("");
                                Thread.Sleep(200);
                            }
                        }
                    }
                }
                else
                {
                    log.Error("SMSHost=" + EMPPHost + " SMSPort=" + EMPPPort + " SMSAccountId=" + EMPPAccountId + " SMSServiceId=" + EMPPServiceId + " SMSPassword=" + EMPPPassword);
                }
                //                Thread.Sleep(1000000);
                Thread.Sleep(1000000);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }
        [Transaction(TransactionMode.Requires)]
        public void SubmitRespInterface(SubmitResp sm)
        {
            try
            {
                DateTime now = DateTime.Now;
                string str = "�յ�submitResp:msgId=" + sm.MsgID + ",seqId=" + sm.SequenceID + ",result=" + sm.Result + ",EventHandler=" + ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_SUBMITRESPINTERFACE + ",now=" + now.ToString();

                log.Debug(str);

                EmppDetail s = new EmppDetail();

                s.MsgID = sm.MsgID;
                s.SeqID = sm.SequenceID;
                s.Content = sm.Result.ToString();
                s.EventHandler = ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_SUBMITRESPINTERFACE;
                s.CreateDate = now;
                s.LastModifyDate = now;

                emppDetailMgrE.CreateEmppDetail(s);

                log.Debug("�����յ��ύ���أ�" + str);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        public void EMPPClosed(int errorCode)
        {
            try
            {
                log.Debug("������EMPPClose�¼���");
                result = ConnectResultEnum.CONNECT_OTHER_ERROR;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        public void SocketClosed(int errorCode)
        {
            log.Debug("������socketcolse�¼�");
            string str = "SocketClosed:errorCode=" + errorCode + ",connected=" + empp.connected;
            log.Debug("�������ڷ�����socketclose�¼���" + "errorcoded is :" + errorCode + "���Ǽ���������������");
            log.Debug(str);
            Reconnect2();
        }

        public void Reconnect2()
        {
            //this.empp = new EMPPLib.emptclClass();
            //createPro(this.empp);

            log.Debug("�����쳣������������������");
            result = ConnectResultEnum.CONNECT_OTHER_ERROR;
            try
            {
                result = this.empp.connect(EMPPHost, EMPPPort, EMPPAccountId, EMPPPassword);
            }
            catch (Exception ex)
            {
                log.Warn(ex.Message, ex);
            }

            while (result != EMPPLib.ConnectResultEnum.CONNECT_OK && result != EMPPLib.ConnectResultEnum.CONNECT_KICKLAST)
            {
                log.Debug("WHILE   �����쳣�����ǽ�����������");
                try
                {
                    result = this.empp.connect(EMPPHost, EMPPPort, EMPPAccountId, EMPPPassword);
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    log.Warn(ex.Message, ex);
                }

            }

            log.Debug("congratulation , now the connection is ok or kicklast");
            Thread.Sleep(3000);
        }
        [Transaction(TransactionMode.Requires)]
        public void MessageReceivedInterface(SMDeliverd sm)
        {
            try
            {
                string str = "�յ��ֻ��ظ�:srcId=" + sm.srcID + "               ,content=" + sm.content + "��ҵ��չλ" + sm.DestID;

                /*
                byte[] b = System.Text.Encoding.UTF8.GetBytes(sm.content.Trim());
                System.Text.Encoding gb = System.Text.Encoding.GetEncoding("GB2312");
                string g = gb.GetString(b);
                string content = g;
                */

                DateTime now = DateTime.Now;
                EmppDetail s = new EmppDetail();

                string[] contents = sm.content.Trim().Split(' ');
                string taskCode = string.Empty;
                TaskMstr task = null;
                if (contents.Length > 1)
                {
                    string serialNo = contents[0].ToUpper();
                    DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskMstr));
                    criteria.Add(Expression.Like("Code", ISIUtil.GetTaskCode(serialNo)));
                    task = criteriaMgrE.FindAll<TaskMstr>(criteria).FirstOrDefault<TaskMstr>();
                    if (task != null && !string.IsNullOrEmpty(task.Code))
                    {
                        taskCode = task.Code;
                        s.TaskCode = taskCode;
                        s.Content = contents[1];
                    }
                    else
                    {
                        s.Content = sm.content;
                    }
                }
                else
                {
                    s.Content = sm.content;
                }

                s.MsgID = sm.MsgID;
                s.DestID = sm.DestID;
                s.CreateDate = now;
                s.LastModifyDate = now;
                s.SrcID = sm.srcID;
                s.SubmitDatetime = (DateTime)sm.submitDatetime;
                s.SrcTerminalType = sm.SrcTerminalType;
                s.MsgFmt = sm.MsgFmt;
                s.MsgLength = sm.MsgLength;
                s.EventHandler = ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_MESSAGERECEIVEDINTERFACE;

                emppDetailMgrE.CreateEmppDetail(s);

                if ("Y".Equals(s.Content.ToUpper()) && !string.IsNullOrEmpty(taskCode))
                {
                    try
                    {
                        /*
                        IList<EmppDetail> sList = emppDetailMgrE.GetEmppDetail(sm.DestID);
                        */
                        User user = null;
                        DetachedCriteria criteriaUser = DetachedCriteria.For(typeof(User));
                        criteriaUser.Add(Expression.Eq("MobliePhone", sm.srcID));
                        criteriaUser.Add(Expression.Eq("IsActive", true));
                        IList<User> userList = criteriaMgrE.FindAll<User>(criteriaUser);
                        if (userList != null && userList.Count > 0)
                        {
                            user = userList[0];
                        }

                        if (user != null)
                        {
                            if (task != null)
                            {
                                if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE
                                        && ((user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE)
                                            && task.TaskSubType.Type != ISIConstants.ISI_TASK_TYPE_PRIVACY) ||
                                            taskMgrE.HasPermission(task, user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN),
                                            user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN),
                                            user.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE), user.Code)))
                                {
                                    task.Status = ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE;
                                    task.CloseDate = now;
                                    task.CloseUser = user.Code;
                                    task.CloseUserNm = user.Name;
                                    task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI5;
                                    task.Color = string.Empty;
                                    task.LastModifyDate = now;
                                    task.LastModifyUser = user.Code;
                                    task.LastModifyUserNm = user.Name;
                                    criteriaMgrE.Update(task);

                                }
                            }
                            else
                            {
                                log.Error(sm.srcID + "  " + taskCode + " is not found.");
                            }
                        }
                        else
                        {
                            log.Error("Task=" + taskCode + ",srcID=" + sm.srcID);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Task=" + taskCode + ",Message=" + e.Message + ",MsgID=" + s.MsgID);
                    }

                }

                log.Debug(str);
                log.Debug(sm.content + "���ǵ��˽���");
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }
        [Transaction(TransactionMode.Requires)]
        public void StatusReceivedInterface(StatusReport sm)
        {
            try
            {
                DateTime now = DateTime.Now;
                string str = "seqId=" + sm.SeqID + ",msgId=" + sm.MsgID + ",mobile=" + sm.DestID + ",destId=" + sm.DestID + ",SrcTerminalId=" + sm.SrcTerminalId + ",Status=" + sm.Status + ",EventHandler=" + ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_STATUSRECEIVEDINTERFACE + ",now=" + now.ToString();
                log.Debug(str);

                IList<EmppDetail> emppDetailList = emppDetailMgrE.GetEmppDetail(sm.MsgID);//, sm.SeqID
                if (emppDetailList != null && emppDetailList.Count > 0)
                {
                    foreach (EmppDetail s in emppDetailList)
                    {
                        s.SrcTerminalId = sm.SrcTerminalId;
                        s.DestID = sm.DestID;
                        s.Status = sm.Status;
                        s.LastModifyDate = now;
                        s.ServiceID = sm.ServiceID;
                        s.SubmitDatetime = (DateTime)sm.submitDatetime;
                        s.DoneDatetime = (DateTime)sm.doneDatetime;
                        s.EventHandler = ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_STATUSRECEIVEDINTERFACE;
                        emppDetailMgrE.UpdateEmppDetail(s);
                    }
                }
                else
                {
                    EmppDetail s = new EmppDetail();
                    s.MsgID = sm.MsgID;
                    s.SeqID = sm.SeqID;
                    s.SrcTerminalId = sm.SrcTerminalId;
                    s.DestID = sm.DestID;
                    s.Status = sm.Status;
                    s.LastModifyDate = now;
                    s.SubmitDatetime = (DateTime)sm.submitDatetime;
                    s.DoneDatetime = (DateTime)sm.doneDatetime;
                    s.EventHandler = ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_STATUSRECEIVEDINTERFACE;
                    s.ServiceID = sm.ServiceID;
                    s.LastModifyDate = now;
                    s.CreateDate = now;
                    s.EventHandler = ISIConstants.CODE_MASTER_ISI_SMS_EVENTHANDLER_STATUSRECEIVEDINTERFACE;
                    emppDetailMgrE.CreateEmppDetail(s);
                }
                log.Debug("�յ�״̬����:" + str);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }

        }

        public void EMPPConnected()
        {
            try
            {
                string str = "������";
                log.Debug(str);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void AsyncSend(
                            string mobilePhones,
                            string msg,
                            User user)
        {
            AsyncSendMsg asyncSend = new AsyncSendMsg(this.Send);

            asyncSend.BeginInvoke(mobilePhones, msg, user, null, null);
        }


        public delegate void AsyncSendMsg(
                            string mobilePhones,
                            string msg,
                            User user);
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class EmppMgrE : com.Sconit.ISI.Service.Impl.EmppMgr, IEmppMgrE
    {
        public EmppMgrE() { }

        public EmppMgrE(string EMPPHost, string EMPPPort, string EMPPAccountId, string EMPPServiceId, string EMPPPassword)
            : base(EMPPHost, EMPPPort, EMPPAccountId, EMPPServiceId, EMPPPassword)
        {

        }
    }
}

#endregion Extend Class