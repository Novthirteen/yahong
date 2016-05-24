<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_Status_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<script language="javascript" type="text/javascript" src="Js/uploadify/jquery.uploadify.min.js?ver=<%=(new Random()).Next(0, 99999).ToString() %>"></script>
<link rel="stylesheet" type="text/css" href="Js/uploadify/uploadify.css" />
<script language="javascript" type="text/javascript" src="Js/tag-it.js"></script>
<script language="javascript" type="text/javascript" src="Js/jquery-qrcode-min.js"></script>
<style type="text/css">
    .ui-button-text-only .ui-button-text {
        padding: 0.2em 0.3em 0.2em 0.3em;
    }

    .link {
        text-decoration: underline;
        color: blue;
        cursor: pointer;
    }

    .link2 {
        cursor: pointer;
    }
</style>
<script language="javascript" type="text/javascript">
    function showHide(showdiv) {
        //alert(showdiv);
        $("#" + showdiv).toggle();
    }

    function Anchor(displayIndex) {
        this.location.href = "#" + displayIndex + "'";
    }

    function BindAssignStartUser() {

        Sys.Net.WebServiceProxy.invoke('Webservice/UserMgrWS.asmx', 'GetAllUser', false,
                {},
            function OnSucceeded(result, eventArgs) {
                //alert("第" + times + "次追加数据.");
                if (result != null) {
                    var tags = result;

                    $('#tbAssignStartUser').tagit({
                        availableTags: tags,
                        allowNotDefinedTags: false,
                        removeConfirmation: true
                    });
                }
            },
            function OnFailed(error) {
                alert(error.get_message());
            }
           );
    }

    function AddGoodComment(taskCode, subject, displayIndex, currentCount, count) {
        AddComment(taskCode, subject, "${ISI.Status.Good}", displayIndex, currentCount, count);
    }
    function AddBadComment(taskCode, subject, displayIndex, currentCount, count) {
        AddComment(taskCode, subject, "${ISI.Status.Bad}", displayIndex, currentCount, count);
    }
    function AddReadComment(taskCode, subject, displayIndex, currentCount, count) {
        AddComment(taskCode, subject, "${ISI.Status.Read}", displayIndex, currentCount, count);
    }
    function AddComment(taskCode, subject, comment, displayIndex, currentCount, count) {
        displayIndex += 2;
        Sys.Net.WebServiceProxy.invoke('Webservice/CommentMgrWS.asmx', 'CreateComment', false,
                            {
                                "taskCode": taskCode,
                                "subject": subject,
                                "comment": comment,
                                "currentCount": currentCount,
                                "count": count,
                                "displayIndex": displayIndex,
                                "userCode": "<%=CurrentUser.Code%>",
                                "userName": "<%=CurrentUser.Name%>"
                            },
                            function OnSucceeded(result, eventArgs) {
                                var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";

                                $(control + "lblComment").prepend("<div><span style='color:#0000E5;'>" + result.CreateUser + "&#40;<span style='color:fuchsia;'>" + formatDate(result.CreateDate) + "</span>&#41;</span>&#58;&nbsp;" + result.Value + "<span style='color:#0000E5;'>&#40;<span style='color:fuchsia;'><b>" + result.CurrentCount + "</b></span>&#47;" + result.Count + "&#41;</span></div>");

                                initialize();

                                $("#lnkComment" + result.TaskCode).unbind('click').removeAttr('onclick');
                                $("#lnkComment" + result.TaskCode).bind('click',
                                    function () {
                                        ShowComment(result.TaskCode, result.Subject, (result.DisplayIndex - 2), result.CurrentCount, result.Count);
                                    });

                                $("#imGood" + result.TaskCode).unbind('click').removeAttr('onclick');
                                $("#imGood" + result.TaskCode).bind('click',
                                    function () {
                                        AddGoodComment(result.TaskCode, result.Subject, (result.DisplayIndex - 2), result.CurrentCount, result.Count);
                                    });

                                $("#imBad" + result.TaskCode).unbind('click').removeAttr('onclick');
                                $("#imBad" + result.TaskCode).bind('click',
                                    function () {
                                        AddBadComment(result.TaskCode, result.Subject, (result.DisplayIndex - 2), result.CurrentCount, result.Count);
                                    });

                                $("#imRead" + result.TaskCode).unbind('click').removeAttr('onclick');
                                $("#imRead" + result.TaskCode).bind('click',
                                    function () {
                                        AddReadComment(result.TaskCode, result.Subject, (result.DisplayIndex - 2), result.CurrentCount, result.Count);
                                    });
                            },
                            function OnFailed(error) {
                                alert(error.get_message());
                            }
                        );
                        }


                        function TaskUploadify() {
                            
                            for (var i=2;i<22;i++)
                            {
                                //$("input[type='file']").uploadify({
                                var str = "";
                                if(i<10){
                                    str="0"+i;
                                }else
                                {
                                    str = i;
                                }
                                
                                var fileControl= "#ctl01_ucList_GV_List_ctl"+str+"_uploadify";
                                var taskCode= $("#ctl01_ucList_GV_List_ctl"+str+"_lbtnEdit").text();
                                var queueId= $("#ctl01_ucList_GV_List_ctl"+str+"_uploadfileQueue").text();
                                $(fileControl).uploadify({

                                    //$("#uploadify").uploadify({
                                    'debug': false, //开启调试    
                                    'auto': true,  //是否自动上传    
                                    'buttonText': '${ISI.Status.SelectFiles}',  //按钮上的文字
                                    'swf': "/JS/uploadify/uploadify.swf",//flash      
                                    'queueID': queueId,  //文件选择后的容器ID  
                                    'uploader': '/ISI/Attachment/FileHandler.ashx',  //后台action  
                                    'width': '50',  //按钮宽度  
                                    'height': '20',  //按钮高度  
                                    'multi': true,  //是否支持多文件上传，默认就是true  
                                    'fileObjName': 'fileData',//后台接收的参数，如：使用struts2上传时，后台有get,set方法的接收参数  
                                    'fileTypeDesc': '<%=this.FileExtensions%>',//  可选择文件类型说明  
                                    'fileTypeExts': '<%=this.FileExtensions%>',  //允许上传文件的类型
                                    'fileSizeLimit': <%=this.ContentLength%> * 1024 * 1024 ,  //文件上传的最大大小
                                    'queueSizeLimit': 50,
                                    'formData': { 'TaskCode': taskCode, 'UserCode': '<%=this.CurrentUser.Code%>', 'UserName': '<%=this.CurrentUser.Name%>'},
                                    'removeTimeout': 1,
                                    //返回一个错误，选择文件的时候触发    
                                    'onSelectError': function (file, errorCode, errorMsg) {
                                        switch (errorCode) {
                                            case -100:
                                                alter("${UploadTheFileNumberIsBeyondSystemOfFile}" + $('#uploadify').uploadify('settings', 'queueSizeLimit'));
                                                break;
                                            case -110:
                                                alter(file.name + "${ISI.TSK.Error.FileSize}" + $('#uploadify').uploadify('settings', 'fileSizeLimit'));
                                                break;
                                            case -120:
                                                alter(file.name + "${ISI.TSK.Error.FileSizeException}");
                                                break;
                                            case -130:
                                                alter(file.name + "${ISI.TSK.Error.FileType}");
                                                break;
                                        }
                                    },
                                    //检测FLASH失败调用    
                                    'onFallback': function () {
                                        alert("${ISI.TSK.NoFlash}");
                                    },
                                    'onSelect': function (file) {
                                        //$(queueId).append("正在上传...");
                                        //$(queueId).show();
                                    },
                                    //上传到服务器，服务器返回相应信息到data里    
                                    'onUploadSuccess': function (file, data, response) {
                                        //var json = eval("(" + data + ")");
                                        //如需上传后生成预览，可在此操作 
                                        //$(queueId).append('<br>' +  ' - 索引: ' + file.index + ' - 文件名: ' + file.name + ' 上传成功');
                                    },
                                    'onQueueComplete': function (queueData) { //队列里所有的文件处理完成后调用 
                                        //$(queueId).text('');
                                        //$(queueId).append(queueData.uploadsSuccessful + "${ISI.TSK.HasBeenUploadedSuccessfully}").fadeIn(300).delay(2500).fadeOut(300);// 这个是渐渐消失
                                    }
                                });

                                $(fileControl).parent().fadeIn(100);
                            }
                        }



                        $(function () {

                            initialize();

                            if("<%=CurrentUser.Code%>" == "tiansu")
                            {
                                $("img[id^='imGood']").show();
                                $("img[id^='imBad']").show();
                                $("img[id^='imRead']").show();
                            }

                            $("#qrCodeDiv").dialog({
                                autoOpen: false,
                                modal: true
                            });
                            //TaskUploadify();

                            $("#commentDiv").dialog({
                                autoOpen: false,
                                //height: 300,
                                width: 500,
                                modal: true,
                                buttons: {
                                    "${ISI.Status.AddComment}": function () {
                                        var taskCode = $("#hdTaskCode").val();
                                        var subject = $("#hdSubject").val();
                                        var currentCount = $("#hdCurrentCount").val();
                                        var count = $("#hdCount").val();
                                        var displayIndex = 2;
                                        displayIndex += Number($("#hdDisplayIndex").val());
                                        var comment = $("#tbComment").val();
                                        if (comment != '' && comment.replace(' ', '') != '') {
                                            $(this).dialog("close");
                                            Sys.Net.WebServiceProxy.invoke('Webservice/CommentMgrWS.asmx', 'CreateComment', false,
                                                {
                                                    "taskCode": taskCode,
                                                    "subject": subject,
                                                    "comment": comment,
                                                    "currentCount": currentCount,
                                                    "count": count,
                                                    "displayIndex": displayIndex,
                                                    "userCode": "<%=CurrentUser.Code%>",
                                                    "userName": "<%=CurrentUser.Name%>"
                                                },
                            function OnSucceeded(result, eventArgs) {
                                var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                                $(control + "lblComment").prepend("<div><span style='color:#0000E5;'>" + result.CreateUser + "&#40;<span style='color:fuchsia;'>" + formatDate(result.CreateDate) + "</span>&#41;</span>&#58;&nbsp;" + result.Value + "<span style='color:#0000E5;'>&#40;<span style='color:fuchsia;'><b>" + result.CurrentCount + "</b></span>&#47;" + result.Count + "&#41;</span></div>");

                                initialize();

                                $("#lnkComment" + result.TaskCode).unbind('click').removeAttr('onclick');
                                $("#lnkComment" + result.TaskCode).bind('click',
                                    function () {
                                        ShowComment(result.TaskCode, result.Subject, (result.DisplayIndex - 2), result.CurrentCount, result.Count);
                                    });
                            },
                            function OnFailed(error) {
                                alert(error.get_message());
                            }
                        );

                        }

                                    },
                                    "${Common.Button.Return}": function () {
                                        //alert('comment cancel');
                                        $(this).dialog("close");
                                        initialize();
                                    }
                                },
                                close: function () {
                                    initialize();
                                }
                            });

                            $("#statusDiv").dialog({
                                autoOpen: false,
                                //height: 300,
                                width: 750,
                                modal: true,
                                buttons: {
                                    "${ISI.Status.AddStatus}": function () {
                                        handler(this,false);
                                    },
                                    "${Common.Button.Return}": function () {
                                        $(this).dialog("close");
                                        initialize();
                                    }
                                },
                                close: function () {
                                    initialize();
                                }
                            });


                            $("#completeDiv").dialog({
                                autoOpen: false,
                                //height: 300,
                                width: 750,
                                modal: true,
                                buttons: {
                                    "${ISI.TSK.Button.Complete}": function () {
                                        handler(this,true);
                                    },
                                    "${Common.Button.Return}": function () {
                                        $(this).dialog("close");
                                        initialize();
                                    }
                                },
                                close: function () {
                                    initialize();
                                }
                            });


                            $("#approveDiv").dialog({
                                autoOpen: false,
                                //height: 300,
                                width: 500,
                                modal: true,
                                buttons: {
                                    "${ISI.TSK.Button.Approve}": function () {
                                        //alert("${ISI.TSK.Button.Approve}");
                                        approve(this,"Approve");
                                    },
                                    "${ISI.TSK.Button.Dispute}": function () {
                                        //alert("${ISI.TSK.Button.Dispute}");
                                        approve(this,"In-Dispute");
                                    },
                                    "${ISI.TSK.Button.Return}": function () {
                                        //alert("${ISI.TSK.Button.Return}");
                                        approve(this,"Return");
                                    },
                                    "${ISI.TSK.Button.Refuse}": function () {
                                        //alert("${ISI.TSK.Button.Refuse}");
                                        approve(this,"Refuse");
                                    },
                                    "${Common.Button.Return}": function () {
                                        $(this).dialog("close");
                                        initialize();
                                    }
                                },
                                close: function () {
                                    initialize();
                                }
                            });



        <%--
        $("#assignDiv").dialog({
            autoOpen: false,
            //height: 300,
            width: 750,
            modal: true,
            buttons: {
                "${ISI.Status.Assign}": function () {
                    var taskCode = $("#hdTaskCode").val();

                    var planCompleteDate = $("#tbPlanCompleteDate").val();
                    var planStartDate = $("#tbPlanStartDate").val();
                    var expectedResults = $("#tbExpectedResults").val();
                    var desc2 = $("#tbDesc2").val();
                    var backYards = $("#tbBackYards").val();
                    var assignStartUser = $("#tbAssignStartUser").val();
                    var taskSubTypeCode = $("#hdTaskSubTypeCode").val();

                    var displayIndex = 2;
                    displayIndex += Number($("#hdDisplayIndex").val());
                    var statusDesc = $("#tbStatusDesc").val();
                    if (statusDesc != '' && statusDesc.replace(' ', '') != '') {
                        $(this).dialog("close");
                        Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'Assign', false,
                            {
                                "taskCode": taskCode,
                                "backYards": backYards,
                                "taskSubTypeCode": taskSubTypeCode,
                                "desc2": desc2,
                                "expectedResults": expectedResults,
                                "assignStartUser": assignStartUser,
                                "planStartDate": planStartDate, "planCompleteDate": planCompleteDate,
                                "displayIndex": displayIndex,
                                "user": "<%=CurrentUser%>"
                            },
                            function OnSucceeded(result, eventArgs) {
                                var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";

                                $(control + "lblStartedUser").html(result.AssignStartUser);
                                $(control + "lblDesc").html(result.Desc2);
                                $(control + "lblExpectedResults").html(result.ExpectedResults);
                                initialize();
                                $("#lnkAssign" + result.TaskCode).unbind('click').removeAttr('onclick');
                                $("#lnkAssign" + result.TaskCode).bind('click',
                                    function () {
                                        ShowAssign(result.TaskCode, result.TaskSubTypeCode, result.AssignStartUser, (result.DisplayIndex - 2));
                                    });
                            },
                            function OnFailed(error) {
                                alert(error.get_message());
                            }
                        );
                        }
                },
                "${Common.Button.Return}": function () {
                    $(this).dialog("close");
                    initialize();
                }
            },
            close: function () {
                initialize();
            }
        });
    --%>



                        });


    function approve(contronl,status)
    {
        
        var taskCode = $("#hdTaskCode").val();
        var subject = $("#hdSubject").val();
        var currentCount = $("#hdCurrentCount").val();
        var count = $("#hdCount").val();
        
        var displayIndex = 2;
        displayIndex += Number($("#hdDisplayIndex").val());
        var approveDesc = $("#tbApprove").val();
        
        if (approveDesc != '' && approveDesc.replace(' ', '') != '') {
            $(contronl).dialog("close");
            Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'ApproveTask', false,
                {
                    "taskCode": taskCode,
                    "subject": subject,
                    "approveDesc": approveDesc,
                    "color": '',
                    "wfsStatus": status,
                    "currentCount": currentCount,
                    "count": count,
                    "displayIndex": displayIndex,
                    "isiAdmin": "<%=CurrentUser.HasPermission(com.Sconit.ISI.Entity.Util.ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)%>",
                    "userCode": "<%=CurrentUser.Code%>",
                    "userName": "<%=CurrentUser.Name%>"
                },
                            function OnSucceeded(result, eventArgs) {
                                var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                                if($(control + "spanApprove") && !result.IsApprove)
                                {
                                    $(control + "spanApprove").hide();
                                    $(control + "lblLevelDesc1").html('');
                                }else if($(control + "spanApprove") && result.IsApprove)
                                {
                                    $(control + "lblLevelDesc1").html(result.LevelDesc);
                                }
                                $(control + "spanCancel").hide();
                                $(control + "lblLevel").html("<font color='blue'>"+result.Level+"</font>");
                                $(control + "lblFlag").html(result.Flag);
                                if (result.TaskStatus == 'Complete') {
                                    $(control + "lblStatus").html("${ISI.Status.Complete}");
                                    
                                }else if (result.TaskStatus == 'In-Process') {
                                    $(control + "lblStatus").html("${ISI.Status.In-Process}");
                                    //$(control + "spanComplete").show();
                                }else if (result.TaskStatus == 'Close') {
                                    $(control + "lblStatus").html("${ISI.Status.Close}");
                                }else if (result.TaskStatus == 'Refuse') {
                                    $(control + "lblStatus").html("${ISI.Status.Refuse}");
                                }else if (result.TaskStatus == 'Return') {
                                    $(control + "lblStatus").html("${ISI.Status.Return}");
                                }else if (result.TaskStatus == 'In-Dispute') {
                                    $(control + "lblStatus").html("${ISI.Status.In-Dispute}");
                                }else if (result.TaskStatus == 'In-Approve') {
                                    $(control + "lblStatus").html("${ISI.Status.In-Approve}");
                                }

                                $(control + "lblStartedUser").html(result.StartedUser);
                                
                                if ($(control + "lblStatusDesc").text().length == 16) {
                                    $(control + "lblStatusDesc").html("<div><span style='color:#0000E5;'>" + result.CreateUser + "&#40;<span style='color:fuchsia;'>" + formatDate(result.CreateDate) + "</span>&#41;</span>&#58;<br><span style='background:#DFD3D3'>${ISI.TSK.Approve}</span>&nbsp;" + result.Value + "<span style='color:#0000E5;'>&#40;<span style='color:fuchsia;'><b>" + result.CurrentCount + "</b></span>&#47;" + result.Count + "&#41;</span></div>");
                                } else {
                                    $(control + "lblStatusDesc").prepend("<div><span style='color:#0000E5;'>" + result.CreateUser + "&#40;<span style='color:fuchsia;'>" + formatDate(result.CreateDate) + "</span>&#41;</span>&#58;<br><span style='background:#DFD3D3'>${ISI.TSK.Approve}</span>&nbsp;" + result.Value + "<span style='color:#0000E5;'>&#40;<span style='color:fuchsia;'><b>" + result.CurrentCount + "</b></span>&#47;" + result.Count + "&#41;</span></div>");
                                }
                                initialize();
                                
                            },
                            function OnFailed(error) {
                                alert(error.get_message());
                            }
                        );
                        }
                    
                    }


                    function handler(contronl,isComplete)
                    {
                        var str="";
                        if(isComplete){
                            str="Complete";
                        }
                        var taskCode = $("#hdTaskCode").val();
                        var subject = $("#hdSubject").val();
                        var currentCount = $("#hdCurrentCount").val();
                        var count = $("#hdCount").val();
                        var color = $("input[name='ddl"+str+"Color']:checked").val();
                        var flag = $("input[name='ddl"+str+"Flag']:checked").val();
        
                        var startDate = $("#tb"+str+"StartDate").val();
                        var endDate = $("#tb"+str+"EndDate").val();
                        //var isCurrentStatus = $("#cbIsCurrentStatus").attr("checked");
                        var isCurrentStatus = document.getElementById("cb"+str+"IsCurrentStatus").checked;
                        var isRemindCreateUser = document.getElementById("cb"+str+"IsRemindCreateUser").checked;
                        var isRemindAssignUser = document.getElementById("cb"+str+"IsRemindAssignUser").checked;
                        var isRemindStartUser = document.getElementById("cb"+str+"IsRemindStartUser").checked;
                        var isRemindCommentUser = document.getElementById("cb"+str+"IsRemindCommentUser").checked;
            
                        var displayIndex = 2;
                        displayIndex += Number($("#hdDisplayIndex").val());
                        var statusDesc = $("#tb"+str+"StatusDesc").val();
        
                        if (statusDesc != '' && statusDesc.replace(' ', '') != '') {
                            $(contronl).dialog("close");
                            Sys.Net.WebServiceProxy.invoke('Webservice/StatusMgrWS.asmx', 'CreateStatus', false,
                                {
                                    "taskCode": taskCode,
                                    "subject": subject,
                                    "statusDesc": statusDesc,
                                    "flag": flag,
                                    "color": color,
                                    "startDate": startDate, "endDate": endDate,
                                    "isCurrentStatus": isCurrentStatus,
                                    "isRemindCreateUser": isRemindCreateUser,
                                    "isRemindAssignUser": isRemindAssignUser,
                                    "isRemindStartUser": isRemindStartUser,
                                    "isRemindCommentUser": isRemindCommentUser,
                                    "isComplete": isComplete,
                                    "currentCount": currentCount,
                                    "count": count,
                                    "displayIndex": displayIndex,
                                    "userCode": "<%=CurrentUser.Code%>",
                                    "userName": "<%=CurrentUser.Name%>"
                                },
                            function OnSucceeded(result, eventArgs) {
                                var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                                $(control + "spanStart").hide();
                                $(control + "spanCancel").hide();
                                
                                if (result.TaskStatus == 'Complete') {
                                    $(control + "lblStatus").html("${ISI.Status.Complete}");
                                    
                                    $(control + "spanComplete").hide();
                                }else if (result.TaskStatus == 'In-Process') {
                                    $(control + "lblStatus").html("${ISI.Status.In-Process}");
                                    $(control + "spanComplete").show();
                                }else if (result.TaskStatus == 'Close') {
                                    $(control + "lblStatus").html("${ISI.Status.Close}");
                                    $(control + "statusDiv").hide();
                                    $(control + "spanComplete").hide();
                                }

                                if (result.IsCurrentStatus) {
                                    $(control + "lblFlag").parent().css("background-color", result.Color);
                                    $(control + "lblFlag").html(result.Flag);
                                }
                                if ($(control + "lblStatusDesc").text().length == 16) {
                                    $(control + "lblStatusDesc").html("<div><span style='color:#0000E5;'>" + result.CreateUser + "&#40;<span style='color:fuchsia;'>" + formatDate(result.CreateDate) + "</span>&#41;</span>&#58;<br><span style='background:#DFD3D3'>${ISI.TSK.Status}</span>&nbsp;" + result.Value + "<span style='color:#0000E5;'>&#40;<span style='color:fuchsia;'><b>" + result.CurrentCount + "</b></span>&#47;" + result.Count + "&#41;</span></div>");
                                } else {
                                    $(control + "lblStatusDesc").prepend("<div><span style='color:#0000E5;'>" + result.CreateUser + "&#40;<span style='color:fuchsia;'>" + formatDate(result.CreateDate) + "</span>&#41;</span>&#58;<br><span style='background:#DFD3D3'>${ISI.TSK.Status}</span>&nbsp;" + result.Value + "<span style='color:#0000E5;'>&#40;<span style='color:fuchsia;'><b>" + result.CurrentCount + "</b></span>&#47;" + result.Count + "&#41;</span></div>");
                                }
                                initialize();
                                $("#lnkStatus" + result.TaskCode).unbind('click').removeAttr('onclick');
                                $("#lnkStatus" + result.TaskCode).bind('click',
                                    function () {
                                        ShowStatus(result.TaskCode, result.Subject, (result.DisplayIndex - 2), result.CurrentCount, result.Count, result.Flag, result.Color,'False');
                                    });

                                //对象是否存在
                                if($("#lnkCompleteStatus" + result.TaskCode) && !result.IsComplete)
                                {
                                    $("#lnkCompleteStatus" + result.TaskCode).unbind('click').removeAttr('onclick');
                                    $("#lnkCompleteStatus" + result.TaskCode).bind('click',
                                        function () {
                                            ShowStatus(result.TaskCode, result.Subject, (result.DisplayIndex - 2), result.CurrentCount, result.Count, result.Flag, result.Color,'True');
                                        });
                                    
                                }
                            },
                            function OnFailed(error) {
                                alert(error.get_message());
                            }
                        );
                        }
                    
                    }

                    function CloseTask(taskCode, displayIndex)
                    {
                        if(window.confirm('${Common.Button.Close.Confirm}'))
                        {
                            displayIndex += 2;
                            Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'CloseTask', false,
                                    {
                                        "taskCode": taskCode,
                                        "displayIndex": displayIndex,
                                        "userCode": "<%=CurrentUser.Code%>",
                                        "userName": "<%=CurrentUser.Name%>"
                                    },
                function OnSucceeded(result, eventArgs) {
                    
                    if(result.Status == "Close")
                    {
                        var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                        $(control + "lblStatus").html("${ISI.Status.Close}");
                        $(control + "spanClose").hide();
                        $(control + "spanReject").hide();
                        $(control + "statusDiv").hide();
                        $(control + "spanComplete").hide();
                        $(control + "lblFlag").parent().css("background-color", "");
                        $(control + "lblFlag").html("DI5");
                    }
                },
                function OnFailed(error) {
                    alert(error.get_message());
                }
            );
            }
        }

        function RejectTask(taskCode, displayIndex)
        {
            if(window.confirm('${ISI.TSK.Button.Reject.Confirm}'))
            {
                displayIndex += 2;
                Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'RejectTask', false,
                        {
                            "taskCode": taskCode,
                            "displayIndex": displayIndex,
                            "userCode": "<%=CurrentUser.Code%>",
                            "userName": "<%=CurrentUser.Name%>"
                        },
                function OnSucceeded(result, eventArgs) {
                    if(result.Status == "In-Process")
                    {
                        var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                        $(control + "lblStatus").html("${ISI.Status.In-Process}");
                        $(control + "spanReject").hide();
                        $(control + "spanClose").hide();
                        $(control + "statusDiv").show();
                        $(control + "spanComplete").show();
                        $(control + "lblFlag").html("DI3");
                    }
                },
                function OnFailed(error) {
                    alert(error.get_message());
                }
            );
            }
        }

        function OpenTask(taskCode, displayIndex)
        {
            if(window.confirm('${ISI.TSK.Button.Open.Confirm}'))
            {
                displayIndex += 2;
                Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'OpenTask', false,
                        {
                            "taskCode": taskCode,
                            "displayIndex": displayIndex,
                            "userCode": "<%=CurrentUser.Code%>",
                            "userName": "<%=CurrentUser.Name%>"
                        },
                function OnSucceeded(result, eventArgs) {
                    
                    if(result.Status == "Submit")
                    {
                        var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                        $(control + "lblStatus").html("${ISI.Status.Submit}");
                        $(control + "spanOpen").hide();
                        $(control + "lblFlag").parent().css("background-color", "");
                        $(control + "lblFlag").html("DI2");
                    }else if(result.Status == "In-Process")
                    {
                        var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                            $(control + "lblStatus").html("${ISI.Status.In-Process}");
                            $(control + "statusDiv").show();
                            $(control + "spanComplete").show();
                            $(control + "spanOpen").hide();                        
                            $(control + "lblFlag").parent().css("background-color", "");
                            $(control + "lblFlag").html("DI2");
                        }
                },
                function OnFailed(error) {
                    alert(error.get_message());
                }
            );
            }
        }


        function StartTask(taskCode, displayIndex)
        {
            if(window.confirm('${ISI.TSK.Confirm.Start}'))
            {
                displayIndex += 2;
                Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'StartTask', false,
                        {
                            "taskCode": taskCode,
                            "displayIndex": displayIndex,
                            "userCode": "<%=CurrentUser.Code%>",
                            "userName": "<%=CurrentUser.Name%>"
                        },
                function OnSucceeded(result, eventArgs) {
                    
                    if(result.Status == "In-Process")
                    {
                        var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                        $(control + "lblStatus").html("${ISI.Status.In-Process}");
                        $(control + "spanStart").hide();
                        $(control + "spanCancel").hide();
                        $(control + "statusDiv").show();
                        $(control + "spanComplete").show();
                        $(control + "lblFlag").html("DI2");
                    }
                },
                function OnFailed(error) {
                    alert(error.get_message());
                }
            );
            }
        }

        function DeleteTask(taskCode)
        {
            if(window.confirm('${Common.Button.Delete.Confirm}'+taskCode))
            {
                Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'DeleteTask', false,
                        {
                            "taskCode": taskCode,
                            "userCode": "<%=CurrentUser.Code%>",
                            "userName": "<%=CurrentUser.Name%>"
                        },
                    function OnSucceeded(result, eventArgs) {
                        $("#"+result.Code).parent().parent().remove();
                    },
                    function OnFailed(error) {
                        alert(error.get_message());
                    }
                );
            }
        }

        function AssignTask(taskCode, displayIndex, msg)
        {
            if(window.confirm(msg))
            {
                displayIndex += 2;
                Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'AssignTask', false,
                        {
                            "taskCode": taskCode,
                            "displayIndex": displayIndex,
                            "userCode": "<%=CurrentUser.Code%>",
                            "userName": "<%=CurrentUser.Name%>"
                        },
                function OnSucceeded(result, eventArgs) {
                    
                    if(result.Status == "Assign")
                    {
                        var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                        $(control + "lblStatus").html("${ISI.Status.Assign}");                        
                        $(control + "spanAssign").hide();
                        $(control + "spanStart").show();
                        $(control + "statusDiv").show();
                        $(control + "spanComplete").show();
                        
                    }
                },
                function OnFailed(error) {
                    alert(error.get_message());
                }
            );
            }
        }

        function SubmitTask(taskCode, displayIndex, assignStartUserNm)
        {
            if(window.confirm('${Common.Button.Submit.Confirm}'))
            {
                displayIndex += 2;
                Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'SubmitTask', false,
                        {
                            "taskCode": taskCode,
                            "displayIndex": displayIndex,
                            "userCode": "<%=CurrentUser.Code%>"
                        },
                function OnSucceeded(result, eventArgs) {
                   
                    if(result.Status == "Submit")
                    {
                        var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                        
                        $(control + "lblStatus").html("${ISI.Status.Submit}");
                        
                        $(control + "lblStartedUser").html(result.StartedUser);
                        $(control + "lblLevel").html("<font color='blue'>"+result.Level+"</font>");

                        $(control + "spanSubmit").hide();
                        $(control + "spanDelete").hide();
                    }
                    if(result.Status == "In-Approve")
                    {
                        var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                        
                        $(control + "lblStatus").html("${ISI.Status.In-Approve}");
                        
                        $(control + "lblStartedUser").html(result.StartedUser);
                        $(control + "lblLevel").html("<font color='blue'>"+result.Level+"</font>");
                        
                        $(control + "spanSubmit").hide();
                        $(control + "spanCancel").hide();
                        $(control + "spanDelete").hide();
                    }
                },
                function OnFailed(error) {
                    alert(error.get_message());
                }
            );
            }
        }

        function CancelTask(taskCode, displayIndex, assignStartUserNm)
        {
            if(window.confirm('${Common.Button.Cancel.Confirm}'))
            {
                displayIndex += 2;
                Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'CancelTask', false,
                        {
                            "taskCode": taskCode,
                            "displayIndex": displayIndex,
                            "userCode": "<%=CurrentUser.Code%>",
                            "userName": "<%=CurrentUser.Name%>"
                        },
                    function OnSucceeded(result, eventArgs) {
                        
                        if(result.Status == "Cancel")
                        {
                            var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";
                            $(control + "lblStatus").html("${ISI.Status.Cancel}");
                            $(control + "spanCancel").hide();
                            $(control + "spanSubmit").hide();
                            $(control + "spanAssign").hide();
                            $(control + "spanDelete").hide();
                            $(control + "spanStart").hide();
                            $(control + "statusDiv").hide();
                            $(control + "spanComplete").hide();
                        }
                    },
                    function OnFailed(error) {
                        alert(error.get_message());
                    }
                );
                }
            }

            function initialize() {
                //公共部分
                $("#hdTaskCode").val('');
                $("#hdSubject").val('');
                $("#hdCurrentCount").val('');
                $("#hdCount").val('');
                $("#hdDisplayIndex").val('');
                $("#hdTaskSubTypeCode").val('');

                //评论
                $("#tbComment").val('');

                //批示
                $("#tbApprove").val('');

                //进展
                var now = new Date();
                $("#tbEndDate").val(now.format("yyyy-MM-dd"));
                now.setTime(now.getTime() - 7 * 24 * 60 * 60 * 1000);
                $("#tbStartDate").val(now.format("yyyy-MM-dd"));
                $("#tbStatusDesc").val('');
                $("#cbIsCurrentStatus").attr("checked", true);
                $("#cbIsRemindCreateUser").attr("checked", true);
                $("#cbIsRemindAssignUser").attr("checked", false);
                $("#cbIsRemindStartUser").attr("checked", false);
                $("#cbIsRemindCommentUser").attr("checked", true);
        
                $("#ddlFlag").find("option[value='DI3']").attr("selected", true);
                $("#ddlColor").find("option[value='green']").attr("selected", true);
                        
                //完成
                $("#tbCompleteEndDate").val($("#tbEndDate").val());
                $("#tbCompleteStartDate").val($("#tbStartDate").val());
                $("#cbCompleteIsCurrentStatus").attr("checked", true);
                $("#cbCompleteIsRemindCreateUser").attr("checked", true);
                $("#cbCompleteIsRemindAssignUser").attr("checked", false);
                $("#cbCompleteIsRemindStartUser").attr("checked", false);
                $("#cbCompleteIsRemindCommentUser").attr("checked", true);
                $("#ddlCompleteColor").find("option[value='green']").attr("selected", true);

                //分派            
                $("#tbDesc2").val('');
                $("#tbExpectedResults").val('');
                $("#tbAssignStartUser").val('');
                $("#tbBackYards").val('');
                $("#tbPlanCompleteDate").val('');
                $("#tbPlanStartDate").val('');
            }

            function formatDate(d) {
                var str = d.format("yyyy-MM-dd HH:mm");
                //var str = (1900 + d.getYear()) + "-" + (d.getMonth() + 1) + "-" + d.getDate() + " " + d.getHours() + ":" + d.getMinutes();
                return str;
            }

            function ShowComment(taskCode, subject, displayIndex, currentCommentCount, commentCount) {

                $("#ui-id-2").html(taskCode + '&nbsp;' + subject);
                $("#hdTaskCode").val(taskCode);
                $("#hdSubject").val(subject);
                $("#hdDisplayIndex").val(displayIndex);
                $("#commentDiv").dialog("open");
                $("#hdCurrentCount").val(currentCommentCount);
                $("#hdCount").val(commentCount);

                displayIndex += Number('2');
                //var jquery_divID = "#<%=this.GV_List.ClientID%>_ctl" + (displayIndex < 10 ? "0" + displayIndex : "" + displayIndex) + "_lblComment";  // 鼠标点击的对象
                //$("html,body").animate({ scrollTop: $(jquery_divID).offset().top - 280 }, 500);

            }

    function ShowStatus(taskCode, subject, displayIndex, currentStatusCount, statusCount, flag, color, isInProcess) {
        
        $("#hdTaskCode").val(taskCode);
        $("#hdSubject").val(subject);
        $("#hdDisplayIndex").val(displayIndex);
        $("#hdCurrentCount").val(currentStatusCount);
        $("#hdCount").val(statusCount);        
        if(isInProcess == 'True')
        {
            $("#ui-id-4").html(taskCode + '&nbsp;' + subject);
            $("#ddlCompleteColor").find("option[value='" + color + "']").attr("selected", true);
            $("#completeDiv").dialog("open");

        }else{
            $("#ui-id-3").html(taskCode + '&nbsp;' + subject);
            $("#ddlFlag").find("option[value='" + flag + "']").attr("selected", true);
            $("#ddlColor").find("option[value='" + color + "']").attr("selected", true);        
            $("#statusDiv").dialog("open");
        }
        displayIndex += Number('2');
        //var jquery_divID = "#<%=this.GV_List.ClientID%>_ctl" + (displayIndex < 10 ? "0" + displayIndex : "" + displayIndex) + "_lblStatusDesc";  // 鼠标点击的对象
        //alert(displayIndex + ' ==  ' + jquery_divID);
        //$("html,body").animate({ scrollTop: $(jquery_divID).offset().top - 280 }, 500);
    }


    function ShowApprove(taskCode, subject, displayIndex, currentStatusCount, statusCount,buttonIndex) {
        
        $("#hdTaskCode").val(taskCode);
        $("#hdSubject").val(subject);
        $("#hdDisplayIndex").val(displayIndex);
        $("#hdCurrentCount").val(currentStatusCount);
        $("#hdCount").val(statusCount);
        $("#ui-id-4").html(taskCode + '&nbsp;' + subject);
        $(".ui-dialog-buttonset button").eq(buttonIndex).hide();
        $("#approveDiv").dialog("open");        
        displayIndex += Number('2');
    }

    function ShowAssign(taskCode, taskSubTypeCode, assignStartUser, displayIndex) {
        //alert(taskCode + '  ' + displayIndex);
        $("#ui-id-3").text(taskCode);
        $("#hdTaskCode").val(taskCode);
        $("#hdDisplayIndex").val(displayIndex);
        $("#tbAssignStartUser").val(assignStartUser);
        $("#hdTaskSubTypeCode").val(taskSubTypeCode);
        BindAssignStartUser();
        $("#assignDiv").dialog("open");
        displayIndex += Number('2');
        //var jquery_divID = "#<%=this.GV_List.ClientID%>_ctl" + (displayIndex < 10 ? "0" + displayIndex : "" + displayIndex) + "_lblStatusDesc";  // 鼠标点击的对象
        //alert(displayIndex + ' ==  ' + jquery_divID);
        //$("html,body").animate({ scrollTop: $(jquery_divID).offset().top - 280 }, 500);

    }

    //关注、插红旗
    function UpdateFocus(imFlag, taskCode) {
        var isAdd = false;
        var src = $(imFlag).attr('src');
        if (src == "Images/ISI/whiteflag1.png") {
            isAdd = true;
        }

        Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'UpdateFocus', false,
                        {
                            "taskCode": taskCode,
                            "userCode": "<%=CurrentUser.Code%>",
                            "userName": "<%=CurrentUser.Name%>",
                            "isAdd": isAdd
                        },
                            function OnSucceeded(result, eventArgs) {
                                if (isAdd == true) {
                                    $(imFlag).attr('src', 'Images/ISI/redflag1.png');
                                } else {
                                    $(imFlag).attr('src', 'Images/ISI/whiteflag1.png');
                                }
                            },
                            function OnFailed(error) {
                                alert(error.get_message());
                            }
                        );
    }

    //更新计划开始时间
    function UpdatePlanStartDate(taskCode, tbPlanStartDate, srcPlanStartDate, planCompleteDate, lblStartDate) {

        var planStartDate = $('#' + tbPlanStartDate).val();
        if (planStartDate == "") {
            $('#' + tbPlanStartDate).val(srcPlanStartDate);
            return;
        }

        Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'UpdatePlanStartDate', false,
                            {
                                "taskCode": taskCode,
                                "planStartDate": planStartDate,
                                "srcPlanStartDate": srcPlanStartDate,
                                "planCompleteDate": planCompleteDate,
                                "tbControl": tbPlanStartDate,
                                "lblControl": lblStartDate,
                                "userCode": "<%=CurrentUser.Code%>",
                                "userName": "<%=CurrentUser.Name%>"
                            },
                            function OnSucceeded(result, eventArgs) {
                                $('#' + lblStartDate).text("OK").fadeIn(300).delay(1000).fadeOut(300);// 这个是渐渐消失 
                            },
                            function OnFailed(error) {
                                //alert(error.get_message());
                                $('#' + lblStartDate).text("");
                                $('#' + tbPlanStartDate).val(srcPlanStartDate);
                            }
                        );
    }

    //更新计划完成时间
    function UpdatePlanCompleteDate(taskCode, tbPlanCompleteDate, srcPlanCompleteDate, planStartDate, lblEndDate) {

        var planCompleteDate = $('#' + tbPlanCompleteDate).val();
        if (planCompleteDate == "") {
            $('#' + tbPlanCompleteDate).val(srcPlanCompleteDate);
            return;
        }

        Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'UpdatePlanCompleteDate', false,
                            {
                                "taskCode": taskCode,
                                "planCompleteDate": planCompleteDate,
                                "srcPlanCompleteDate": srcPlanCompleteDate,
                                "planStartDate": planStartDate,
                                "tbControl": tbPlanCompleteDate,
                                "lblControl": lblEndDate,
                                "userCode": "<%=CurrentUser.Code%>",
                                "userName": "<%=CurrentUser.Name%>"
                            },
                            function OnSucceeded(result, eventArgs) {
                                $('#' + lblEndDate).text("OK").fadeIn(300).delay(1000).fadeOut(300);// 这个是渐渐消失 
                            },
                            function OnFailed(error) {
                                //alert(error.get_message());
                                $('#' + lblEndDate).text("");
                                $('#' + tbPlanCompleteDate).val(srcPlanCompleteDate);
                            }
                        );
    }

    function ShowSelectFiles(label)
    {
                        
        var labelId =$(label).attr("id");

        var str = labelId.substring(24,26);
                        
        var fileControl= "#ctl01_ucList_GV_List_ctl"+str+"_uploadify";
        var taskCode= $("#ctl01_ucList_GV_List_ctl"+str+"_lbtnEdit").text();
        var queueId= $("#ctl01_ucList_GV_List_ctl"+str+"_uploadfileQueue").text();
                        

        $(fileControl).uploadify({

            //$("#uploadify").uploadify({
            'debug': false, //开启调试    
            'auto': true,  //是否自动上传    
            'buttonText': '${ISI.Status.SelectFiles}',  //按钮上的文字
            'swf': "/JS/uploadify/uploadify.swf",//flash      
            'queueID': queueId,  //文件选择后的容器ID  
            'uploader': '/ISI/Attachment/FileHandler.ashx',  //后台action  
            'width': '50',  //按钮宽度  
            'height': '20',  //按钮高度  
            'multi': true,  //是否支持多文件上传，默认就是true  
            'fileObjName': 'fileData',//后台接收的参数，如：使用struts2上传时，后台有get,set方法的接收参数  
            'fileTypeDesc': '<%=this.FileExtensions%>',//  可选择文件类型说明  
            'fileTypeExts': '<%=this.FileExtensions%>',  //允许上传文件的类型
            'fileSizeLimit': <%=this.ContentLength%> * 1024 * 1024 ,  //文件上传的最大大小
            'queueSizeLimit': 50,
            'formData': { 'TaskCode': taskCode, 'UserCode': '<%=this.CurrentUser.Code%>', 'UserName': '<%=this.CurrentUser.Name%>'},
            'removeTimeout': 1,
            //返回一个错误，选择文件的时候触发    
            'onSelectError': function (file, errorCode, errorMsg) {
                switch (errorCode) {
                    case -100:
                        alter("${UploadTheFileNumberIsBeyondSystemOfFile}" + $('#uploadify').uploadify('settings', 'queueSizeLimit'));
                        break;
                    case -110:
                        alter(file.name + "${ISI.TSK.Error.FileSize}" + $('#uploadify').uploadify('settings', 'fileSizeLimit'));
                        break;
                    case -120:
                        alter(file.name + "${ISI.TSK.Error.FileSizeException}");
                        break;
                    case -130:
                        alter(file.name + "${ISI.TSK.Error.FileType}");
                        break;
                }
            },
            //检测FLASH失败调用    
            'onFallback': function () {
                alert("${ISI.TSK.NoFlash}");
            },
            'onSelect': function (file) {
                //$(queueId).append("正在上传...");
                //$(queueId).show();
            },
            //上传到服务器，服务器返回相应信息到data里    
            'onUploadSuccess': function (file, data, response) {
                //var json = eval("(" + data + ")");
                //如需上传后生成预览，可在此操作 
                //$(queueId).append('<br>' +  ' - 索引: ' + file.index + ' - 文件名: ' + file.name + ' 上传成功');
            },
            'onQueueComplete': function (queueData) { //队列里所有的文件处理完成后调用 
                
                //$(queueId).append(queueData.uploadsSuccessful + "${ISI.TSK.HasBeenUploadedSuccessfully}").fadeIn(300).delay(2500).fadeOut(300);// 这个是渐渐消失
                //$("#fileDiv").html(queueData.uploadsSuccessful + "${ISI.TSK.HasBeenUploadedSuccessfully}").show(300).delay(3000).hide(300); 
                
                var dlg = $("#fileDiv").dialog({ 
                    resizable: false,
                    modal: false,
                    hide: "explode"
                    //open: function (event, ui) {
                    //    $(".ui-dialog-titlebar-close", $(this).parent()).hide();
                    //}
                });
                
                $("#fileDiv").html(queueData.uploadsSuccessful + "${ISI.TSK.HasBeenUploadedSuccessfully}");
                dlg.dialog("option", { title: "${ISI.Status.Attachment}" }); // 设置参数 
                dlg.dialog("open"); // 使用open方法打开对话框
                //$("#fileDiv").parent().delay(1000).hide(300);
            }
        });
                        
        $(label).hide();
        $(fileControl).fadeIn(100);
    }


    function ShowQRCode(taskCode,subject,displayIndex)
    {
        var str="";
        displayIndex += 2;
        var control = "#<%=this.GV_List.ClientID%>_ctl" + (displayIndex < 10 ? "0" + displayIndex : "" + displayIndex) + "_";        
        var lblDesc = control + "lblDesc";
        var lblExpectedResults = control+"lblExpectedResults";
        str += "${ISI.TSK.Code}：" + taskCode;
        if(subject.length>0){
            str+= "\n${ISI.TSK.Subject}：" + subject;
        }
        var desc= $(lblDesc).text();        
        if(desc.length>0){
            str += "\n${ISI.TSK.Desc1}：" + desc.replace("Click","");
        }
        var expectedResults = $(lblExpectedResults).html(); 
        var reg = new RegExp("<br>","g"); //创建正则RegExp对象   
        if(expectedResults.length>0){
            str += "\n${ISI.TSK.ExpectedResults}：" + expectedResults.replace(reg,"\n");
        }
        str = toUtf8(str);        
        $("#qrCodeDiv").html('');      
        $("#qrCodeDiv").qrcode({ 
            render: !!document.createElement('canvas').getContext ? 'canvas' : 'table',
            correctLevel: 0,
            text:str
        }); 
        $("#qrCodeDiv").dialog("open");
    }
</script>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="TaskCode"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="false" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnDataBound="GV_List_DataBound" OnRowDataBound="GV_List_RowDataBound"
            DefaultSortDirection="Descending" DefaultSortExpression="StatusDate">
            <Columns>
                <asp:TemplateField HeaderText="${ISI.Status.Subject}" HeaderStyle-Wrap="false" SortExpression="CreateDate">
                    <ItemTemplate>
                        <div>
                            <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>'
                                Text='<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>' OnClick="lbtnEdit_Click" />
                            <asp:HiddenField ID="hfIsApply" runat="server" Value='<%# Bind("IsApply") %>' />
                        </div>
                        <div>
                            <asp:Label ID="lblSubject" Width="90" runat="server" Text='' />
                        </div>
                        <div>
                            <asp:Label ID="lblCreateDate" runat="server" Text='<%#Eval("CreateDate","{0:yyyy-MM-dd HH}")%>' />
                        </div>
                        <div>
                            <asp:Image ID="imFlag" runat="server" ImageUrl="~/Images/ISI/whiteflag.png" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Description}" HeaderStyle-Width="22%" ItemStyle-Width="22%" HeaderStyle-Wrap="false" SortExpression="RefTaskCount">
                    <ItemTemplate>
                        <div>
                            <asp:Label ID="lblDesc" runat="server" Text='' />
                        </div>
                        <span style='color: #0000E5;'>
                            <asp:LinkButton ID="lbtnNew" runat="server" CommandArgument='<%# "{"+DataBinder.Eval(Container.DataItem, "TaskCode")+ "}{"+DataBinder.Eval(Container.DataItem, "Subject")+ "}{"+DataBinder.Eval(Container.DataItem, "TaskAddress")+ "}{"+DataBinder.Eval(Container.DataItem, "Desc1")+ "}{"+DataBinder.Eval(Container.DataItem, "Desc2")+ "}{"+DataBinder.Eval(Container.DataItem, "ExpectedResults") + "}{"+DataBinder.Eval(Container.DataItem, "StartDate","{0:yyyy-MM-dd HH:mm}")+ "}{"+DataBinder.Eval(Container.DataItem, "PlanCompleteDate","{0:yyyy-MM-dd HH:mm}")+ "}" %>'
                                Text='${ISI.TSK.RefTask}' OnClick="lbtnNew_Click" />
                        </span>
                        <span id="spanQRCode" class="link" onclick="ShowQRCode('<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>','<%# DataBinder.Eval(Container.DataItem, "Subject")%>',<%# Container.DisplayIndex %>);">${ISI.TSK.Button.QRCode}</span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TSK.ExpectedResults}" HeaderStyle-Wrap="false" SortExpression="PlanStartDate">
                    <ItemTemplate>
                        <div>
                            <asp:Label ID="lblExpectedResults" runat="server" Text='' />
                        </div>
                        <div runat="server" id="downLoadDiv">
                            <div>
                                <asp:LinkButton ID="lbtnDownLoad1" runat="server" CommandArgument=''
                                    OnClick="lbtnDownLoad_Click" Visible="False" />
                            </div>
                            <div>
                                <asp:LinkButton ID="lbtnDownLoad2" runat="server" CommandArgument=''
                                    OnClick="lbtnDownLoad_Click" Visible="False" />
                            </div>
                            <div>
                                <asp:LinkButton ID="lbtnDownLoad3" runat="server" CommandArgument=''
                                    OnClick="lbtnDownLoad_Click" Visible="False" />
                            </div>
                            <div>
                                <asp:LinkButton ID="lbtnDownLoad4" runat="server" CommandArgument=''
                                    OnClick="lbtnDownLoad_Click" Visible="False" />
                            </div>
                            <div>
                                <asp:LinkButton ID="lbtnDownLoad5" runat="server" CommandArgument=''
                                    OnClick="lbtnDownLoad_Click" Visible="False" />
                            </div>
                            <div>
                                <asp:LinkButton ID="lbtnDownLoad6" runat="server" CommandArgument=''
                                    OnClick="lbtnDownLoad_Click" Visible="False" />
                            </div>
                            <div>
                                <asp:LinkButton ID="lbtnDownLoad7" runat="server" CommandArgument=''
                                    OnClick="lbtnDownLoad_Click" Visible="False" />
                            </div>
                            <div>
                                <asp:LinkButton ID="lbtnDownLoad8" runat="server" CommandArgument=''
                                    OnClick="lbtnDownLoad_Click" Visible="False" />
                            </div>
                            <div>
                                <asp:LinkButton ID="lbtnDownLoad9" runat="server" CommandArgument=''
                                    OnClick="lbtnDownLoad_Click" Visible="False" />
                            </div>
                            <div>
                                <asp:LinkButton ID="lbtnDownLoad10" runat="server" CommandArgument=''
                                    OnClick="lbtnDownLoad_Click" Visible="False" />
                            </div>
                        </div>
                        <div align="right" style="margin-bottom: -20px;">
                            <span runat="server" class="link" id="lblFile" onclick="ShowSelectFiles(this)">${ISI.Status.Upload}</span>
                            <asp:FileUpload runat="server" ID="uploadify" Style="display: none;" />
                        </div>
                        <div>
                            <asp:Label ID="lblPlanStartDate" Visible="false" runat="server" Text="${ISI.Status.PlanStartDate}:" />
                            <asp:TextBox ID="tbStartDate" Visible="false" runat="server" Width="80" Text='<%#Eval("PlanStartDate","{0:yyyy-MM-dd}")%>' ReadOnly="true" />
                            <asp:Label ID="lblStartDate" runat="server" ForeColor="Red" />
                        </div>
                        <div>
                            <asp:Label ID="lblDeadline" runat="server" Text="${ISI.Status.Deadline}:" />
                            <asp:TextBox ID="tbEndDate" Visible="false" runat="server" Width="80" Text='<%#Eval("PlanCompleteDate","{0:yyyy-MM-dd}")%>' ReadOnly="true" />
                            <asp:Label ID="lblEndDate" runat="server" ForeColor="Red" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.SubmitUser}" HeaderStyle-Wrap="false" SortExpression="SubmitDate" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <div runat="server" id="uploadfileQueue"></div>
                        <div>
                            <asp:Label ID="lblSubmitUserNm" runat="server" Text='<%# Eval("SubmitUserNm")%>' />
                        </div>
                        <div>
                            <asp:Label ID="lblTaskAddress" runat="server" Text='<%# Eval("TaskAddress")%>' />
                        </div>
                        <div>
                            <asp:Label ID="lblTaskSubTypeCode" runat="server" Text='' />
                            <asp:Label ID="lblCostCenter" runat="server" TText='<%# Eval("CostCenterCode")%>' />
                        </div>
                        <div><span style="color: #0000E5;"><%#Eval("Phase")%></span></div>
                        <div><%#Eval("Seq")%></div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.AssignUser}" HeaderStyle-Wrap="false" SortExpression="AssignDate" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <div>
                            <asp:Label ID="lblAssignUserNm" runat="server" Text='<%# Eval("AssignUserNm")%>' />
                        </div>
                        <div>
                            <asp:Label ID="lblStatus" runat="server" />
                        </div>
                        <div>
                            <asp:Label ID="lblLevel" runat="server" />
                        </div>
                        <div>
                            <asp:Label ID="lblLevelDesc1" runat="server" Visible="false" />
                        </div>
                        <div>
                            <asp:LinkButton ID="lbtnEdit2" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>'
                                Text='<${ISI.Status.IsCtrl}>' Visible="false" OnClick="lbtnEdit_Click" />
                        </div>
                        <div id="assignDiv" runat="server" visible="false"></div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.StartUser}" HeaderStyle-Wrap="false" SortExpression="StartDate" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblStartedUser" runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.StatusDesc}" HeaderStyle-Width="22%" ItemStyle-Width="22%" HeaderStyle-Wrap="false" SortExpression="StatusDate">
                    <ItemTemplate>
                        <asp:Label ID="lblStatusDesc" runat="server" Text='' />
                        <div>
                            <span id="spanApprove" style="display: none;" runat="server" />
                            <span id="spanStart" style="display: none;" runat="server" />
                            <span id="statusDiv" style="display: none;" runat="server" />
                            <span id="spanComplete" style="display: none;" runat="server" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.CodeMaster.Flag}" HeaderStyle-Wrap="false" SortExpression="TColor" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblFlag" runat="server" Text='<%# Bind("Flag") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.Comment}" HeaderStyle-Wrap="false" SortExpression="CommentCreateDate" HeaderStyle-Width="15%" ItemStyle-Width="15%">
                    <ItemTemplate>
                        <a id="<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>" name="<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>"></a>
                        <asp:Label ID="lblComment" runat="server" Text='' />
                        <div>
                            <table width="100%">
                                <tr align="justify">
                                    <td>
                                        <span class="link" onclick="javascript:ShowComment('<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>','<%# DataBinder.Eval(Container.DataItem, "Subject")%>',<%# Container.DisplayIndex %>,<%# DataBinder.Eval(Container.DataItem, "CurrentCommentCount1")%>,<%# DataBinder.Eval(Container.DataItem, "CommentCount1")%>);" id="lnkComment<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>" name="lnkComment<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>">${ISI.Status.Comment}</span>
                                    </td>
                                    <td>
                                        <img class="link2" id="imGood<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>" name="imGood<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>"
                                            src="Images/ISI/good.png" title="${ISI.Status.Good}" style="display: none;"
                                            onclick="javascript:AddGoodComment('<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>','<%# DataBinder.Eval(Container.DataItem, "Subject")%>',<%# Container.DisplayIndex %>,<%# DataBinder.Eval(Container.DataItem, "CurrentCommentCount1")%>,<%# DataBinder.Eval(Container.DataItem, "CommentCount1")%>);" /></td>
                                    <td>
                                        <img class="link2" id="imBad<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>" name="imGood<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>"
                                            src="Images/ISI/bad.png" title="${ISI.Status.Bad}" style="display: none;"
                                            onclick="javascript:AddBadComment('<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>','<%# DataBinder.Eval(Container.DataItem, "Subject")%>',<%# Container.DisplayIndex %>,<%# DataBinder.Eval(Container.DataItem, "CurrentCommentCount1")%>,<%# DataBinder.Eval(Container.DataItem, "CommentCount1")%>);" /></td>
                                    <td>
                                        <img class="link2" id="imRead<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>" name="imRead<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>"
                                            src="Images/ISI/read.png" title="${ISI.Status.Read}" style="display: none;"
                                            onclick="javascript:AddReadComment('<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>','<%# DataBinder.Eval(Container.DataItem, "Subject")%>',<%# Container.DisplayIndex %>,<%# DataBinder.Eval(Container.DataItem, "CurrentCommentCount1")%>,<%# DataBinder.Eval(Container.DataItem, "CommentCount1")%>);" /></td>
                                </tr>
                            </table>
                        </div>
                        <div>
                            <table width="100%">
                                <tr align="justify">
                                    <td>
                                        <span visible="false" id="spanSubmit" runat="server" />
                                        <span visible="false" id="spanAssign" runat="server" />
                                        <span visible="false" id="spanOpen" runat="server" />
                                        <span visible="false" id="spanClose" runat="server" />
                                        <span visible="false" id="spanDelete" runat="server" />
                                        <span visible="false" id="spanCancel" runat="server" />
                                        <span visible="false" id="spanReject" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="20" AlwaysShow="true">
        </cc1:GridPager>
        <script language="javascript" type="text/javascript">
            $(document).ready(function () {
                $('.GV').fixedtableheader();
            });
        </script>
    </div>
    <input id="hdSubject" type="hidden">
    <input id="hdTaskCode" type="hidden">
    <input id="hdDisplayIndex" type="hidden">
    <input id="hdCurrentCount" type="hidden">
    <input id="hdCount" type="hidden">
    <input id="hdTaskSubTypeCode" type="hidden">
    <div id="commentDiv" style="display: none; font-size: 13px;" title="${ISI.Status.AddComment}">
        <table class="mtable">
            <tr>
                <td>
                    <textarea rows="10" cols="50" id="tbComment" name="tbComment" style="height: 100%; margin: 0px; padding: 0px; border: 0px none rgb(0, 0, 0); width: 100%;"
                        onkeypress="javascript:setMaxLength(this,160);"
                        onpaste="limitPaste(this, 160)"></textarea>
                </td>
            </tr>
        </table>
    </div>
    <div id="statusDiv" style="display: none; font-size: 13px;" title="${ISI.Status.AddStatus}">
        <table class="mtable">
            <tr>
                <td colspan="3">
                    <textarea rows="10" cols="50" id="tbStatusDesc" name="tbStatusDesc" style="height: 100%; margin: 0px; padding: 0px; border: 0px none rgb(0, 0, 0); width: 100%;"
                        onkeypress="javascript:setMaxLength(this,1000);"
                        onpaste="limitPaste(this, 1000)"></textarea>
                </td>
            </tr>
            <tr id="trColorDate">
                <td>
                    <label for="tbStartDate">${Common.Business.StartTime}:</label>
                    <input type="text" cssclass="inputRequired" id="tbStartDate" name="tbStartDate"
                        onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd' })" value="" style="width: 120px;" />
                </td>
                <td>
                    <label for="lblEndDate">${Common.Business.EndTime}:</label>
                    <input type="text" cssclass="inputRequired" id="tbEndDate" name="tbEndDate"
                        onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd' })" value="" style="width: 120px;" />
                </td>
                <td>
                    <input type="radio" value="DI2" name="ddlFlag" />DI2
                    <input type="radio" value="DI3" checked="checked" name="ddlFlag" />DI3
                    <input type="radio" value="DI4" name="ddlFlag" />DI4
                    <input type="radio" value="green" checked="checked" name="ddlColor" />${Green}
                    <input type="radio" value="yellow" name="ddlColor" />${Yellow}
                    <input type="radio" value="red" name="ddlColor" />${Red}
                </td>
            </tr>
            <tr id="trIsRemind">
                <td colspan="3" align="center">
                    <span>
                        <input type="checkbox" id="cbIsRemindCreateUser" name="cbIsRemindCreateUser" checked="checked" />
                        <label for="cbIsRemindCreateUser">${ISI.TaskStatus.IsRemindCreateUser}</label>
                    </span>
                    <span>
                        <input type="checkbox" id="cbIsRemindAssignUser" name="cbIsRemindAssignUser" />
                        <label for="cbIsRemindAssignUser">${ISI.TaskStatus.IsRemindAssignUser}</label>
                    </span>
                    <span>
                        <input type="checkbox" id="cbIsRemindStartUser" name="cbIsRemindStartUser" />
                        <label for="cbIsRemindStartUser">${ISI.TaskStatus.IsRemindStartUser}</label>
                    </span>
                    <span>
                        <input type="checkbox" id="cbIsRemindCommentUser" name="cbIsRemindCommentUser" checked="checked" />
                        <label for="cbIsRemindCommentUser">${ISI.TaskStatus.IsRemindCommentUser}</label>
                    </span>
                    <span>
                        <input type="checkbox" id="cbIsCurrentStatus" name="cbIsCurrentStatus" checked="checked" />
                        <label for="cbIsCurrentStatus">${ISI.TaskStatus.IsCurrentStatus}</label>
                    </span>
                </td>
            </tr>
        </table>
    </div>
    <div id="completeDiv" style="display: none; font-size: 13px;" title="${ISI.TaskStatus.IsComplete}">
        <table class="mtable">
            <tr>
                <td colspan="3">
                    <textarea rows="10" cols="50" id="tbCompleteStatusDesc" name="tbStatusDesc" style="height: 100%; margin: 0px; padding: 0px; border: 0px none rgb(0, 0, 0); width: 100%;"
                        onkeypress="javascript:setMaxLength(this,1000);"
                        onpaste="limitPaste(this, 1000)"></textarea>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="tbCompleteStartDate">${Common.Business.StartTime}:</label>
                    <input type="text" cssclass="inputRequired" id="tbCompleteStartDate" name="tbStartDate"
                        onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd' })" value="" style="width: 120px;" />
                </td>
                <td>
                    <label for="tbCompleteEndDate">${Common.Business.EndTime}:</label>
                    <input type="text" cssclass="inputRequired" id="tbCompleteEndDate" name="tbEndDate"
                        onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd' })" value="" style="width: 120px;" />
                </td>
                <td>
                    <input type="radio" value="DI4" checked="checked" name="ddlCompleteFlag" />DI4

                    <input type="radio" value="green" checked="checked" name="ddlCompleteColor" id="CompleteGreen" /><label for="CompleteGreen">${Green}</label>
                    <input type="radio" value="yellow" name="ddlCompleteColor" id="CompleteYellow" /><label for="CompleteYellow">${Yellow}</label>
                    <input type="radio" value="red" name="ddlCompleteColor" id="CompleteRed" /><label for="CompleteRed">${Red}</label>
                </td>
            </tr>
            <tr>
                <td colspan="3" align="center">
                    <span>
                        <input type="checkbox" id="cbCompleteIsRemindCreateUser" name="cbIsRemindCreateUser" checked="checked" />
                        <label for="cbCompleteIsRemindCreateUser">${ISI.TaskStatus.IsRemindCreateUser}</label>
                    </span>
                    <span>
                        <input type="checkbox" id="cbCompleteIsRemindAssignUser" name="cbIsRemindAssignUser" />
                        <label for="cbCompleteIsRemindAssignUser">${ISI.TaskStatus.IsRemindAssignUser}</label>
                    </span>
                    <span>
                        <input type="checkbox" id="cbCompleteIsRemindStartUser" name="cbIsRemindStartUser" />
                        <label for="cbCompleteIsRemindStartUser">${ISI.TaskStatus.IsRemindStartUser}</label>
                    </span>
                    <span>
                        <input type="checkbox" id="cbCompleteIsRemindCommentUser" name="cbIsRemindCommentUser" checked="checked" />
                        <label for="cbCompleteIsRemindCommentUser">${ISI.TaskStatus.IsRemindCommentUser}</label>
                    </span>
                    <span>
                        <input type="checkbox" id="cbCompleteIsCurrentStatus" name="cbIsCurrentStatus" checked="checked" />
                        <label for="cbCompleteIsCurrentStatus">${ISI.TaskStatus.IsCurrentStatus}</label>
                    </span>
                </td>
            </tr>
        </table>
    </div>
    <div id="assignDiv" style="display: none; font-size: 13px;" title="${ISI.Status.Assign}">
        <table class="mtable">
            <tr>
                <td>
                    <label for="tbPlanStartDate">${ISI.TSK.PlanStartDate}:</label>
                    <input type="text" cssclass="inputRequired" id="tbPlanStartDate" name="tbPlanStartDate"
                        onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd HH:mm' })" value="" style="width: 150px;" />
                </td>
                <td>
                    <label for="tbPlanCompleteDate">${ISI.TSK.PlanCompleteDate}:</label>
                    <input type="text" cssclass="inputRequired" id="tbPlanCompleteDate" name="tbPlanCompleteDate"
                        onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd HH:mm' })" value="" style="width: 150px;" />
                </td>
                <td>
                    <label for="tbBackYards">${ISI.TSK.BackYards}:</label>
                    <input type="text" cssclass="inputRequired" id="tbBackYards" name="tbBackYards"
                        value="" style="width: 150px;" onkeypress="javascript:setMaxLength(this,50);"
                        onpaste="limitPaste(this, 50)" />
                </td>
            </tr>
            <tr>
                <label for="tbAssignStartUser">${ISI.TSK.AssignStartUser}:</label>
                <input type="text" cssclass="inputRequired" id="tbAssignStartUser" name="tbAssignStartUser"
                    style="width: 100%" />
            </tr>
            <tr>
                <td colspan="3">
                    <label for="tbExpectedResults">${ISI.TSK.ExpectedResults}:</label>
                    <textarea rows="5" cols="50" id="tbExpectedResults" name="tbExpectedResults" style="height: 100%; margin: 0px; padding: 0px; border: 0px none rgb(0, 0, 0); width: 100%;"
                        onkeypress="javascript:setMaxLength(this,500);"
                        onpaste="limitPaste(this, 500)"></textarea>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <label for="tbDesc2">${ISI.TSK.Desc2}:</label>
                    <textarea rows="5" cols="50" id="tbDesc2" name="tbDesc2" style="height: 100%; margin: 0px; padding: 0px; border: 0px none rgb(0, 0, 0); width: 100%;"
                        onkeypress="javascript:setMaxLength(this,500);"
                        onpaste="limitPaste(this, 500)"></textarea>
                </td>
            </tr>
        </table>
    </div>
    <div id="approveDiv" style="display: none; font-size: 13px;" title="${ISI.Status.AddApprove}">
        <table class="mtable">
            <tr>
                <td>
                    <textarea rows="10" cols="50" id="tbApprove" name="tbApprove" style="height: 100%; margin: 0px; padding: 0px; border: 0px none rgb(0, 0, 0); width: 100%;"
                        onkeypress="javascript:setMaxLength(this,160);"
                        onpaste="limitPaste(this, 160)"></textarea>
                </td>
            </tr>
        </table>
    </div>
    <div id="fileDiv" style="display: none; font-size: 13px;"></div>
    <div id="qrCodeDiv" style="display: none;"></div>
</fieldset>
