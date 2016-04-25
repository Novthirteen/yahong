<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="ISI_TSK_Edit" %>
<%@ Register Src="CostList.ascx" TagName="CostList" TagPrefix="uc2" %>
<%@ Register Src="CostDet.ascx" TagName="CostDet" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.ui-zendesk.css" />
<link rel="stylesheet" type="text/css" href="Js/uploadify/uploadify.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<script language="javascript" type="text/javascript" src="Js/tag-it.js"></script>
<script language="javascript" type="text/javascript" src="Js/uploadify/jquery.uploadify.min.js?ver=<%=(new Random()).Next(0, 99999).ToString() %>"></script>
<script language="javascript" type="text/javascript" src="Js/jquery-qrcode-min.js"></script>

<style type="text/css">
    .link {
        text-decoration: underline;
        color: blue;
        cursor: pointer;
    }

    .CssColor {
        text-align: center;
        height: 18px;
        vertical-align: middle;
    }
</style>
<script language="javascript" type="text/javascript">

    function TaskUploadify() {
        
        $("#uploadify").uploadify({
            'debug': false, //开启调试    
            'auto': true,  //是否自动上传    
            'buttonText': '${ISI.Status.SelectFiles}',  //按钮上的文字
            'swf': "/JS/uploadify/uploadify.swf",//flash      
            'queueID': 'uploadfileQueue',  //文件选择后的容器ID  
            'uploader': '/ISI/Attachment/FileHandler.ashx',  //后台action  
            'width': '50',  //按钮宽度  
            'height': '20',  //按钮高度  
            'multi': true,  //是否支持多文件上传，默认就是true  
            'fileObjName': 'fileData',//后台接收的参数，如：使用struts2上传时，后台有get,set方法的接收参数  
            'fileTypeDesc': '<%=this.FileExtensions%>',//  可选择文件类型说明  
            'fileTypeExts': '<%=this.FileExtensions%>',  //允许上传文件的类型
            'fileSizeLimit': <%=this.ContentLength%> * 1024 * 1024 ,  //文件上传的最大大小
            'queueSizeLimit': 50,
            'formData': { 'TaskCode': '<%=this.TaskCode%>', 'UserCode': '<%=this.CurrentUser.Code%>', 'UserName': '<%=this.CurrentUser.Name%>' },
            'removeTimeout': 1,
            //返回一个错误，选择文件的时候触发    
            'onSelectError': function (file, errorCode, errorMsg) {
                switch (errorCode) {
                    case -100:
                        $("#uploadfileQueue").text("${ISI.TSK.UploadTheFileNumberIsBeyondSystemOfFile}" + $('#uploadify').uploadify('settings', 'queueSizeLimit'));
                        break;
                    case -110:
                        $("#uploadfileQueue").text(file.name + "${ISI.TSK.Error.FileSize}" + $('#uploadify').uploadify('settings', 'fileSizeLimit'));
                        break;
                    case -120:
                        $("#uploadfileQueue").text(file.name + "${ISI.TSK.Error.FileSizeException}");
                        break;
                    case -130:
                        $("#uploadfileQueue").text(file.name + "${ISI.TSK.Error.FileType}");
                        break;
                }
            },
            //检测FLASH失败调用    
            'onFallback': function () {
                alert("${ISI.TSK.NoFlash}");
            },
            'onSelect': function (file) {
                //$("#uploadfileQueue").append("正在上传...");
                //$("#uploadfileQueue").show();
            },
            //上传到服务器，服务器返回相应信息到data里    
            'onUploadSuccess': function (file, data, response) {
                //var json = eval("(" + data + ")");
                //如需上传后生成预览，可在此操作 
                //$("#uploadfileQueue").append('<br>' +  ' - 索引: ' + file.index + ' - 文件名: ' + file.name + ' 上传成功');
            },
            'onQueueComplete': function (queueData) { //队列里所有的文件处理完成后调用 
                $("#uploadfileQueue").text('');
                $("#uploadfileQueue").append(queueData.uploadsSuccessful + "${ISI.TSK.HasBeenUploadedSuccessfully}").fadeIn(300).delay(2500).fadeOut(300);// 这个是渐渐消失
            }
        });

        $("#lblFile").hide();
        $("#uploadify").fadeIn(100);
    }



    function ConfirmAssign() {

        var planStartDate = $('#<%=this.FV_ISI.FindControl("tbPlanStartDate").ClientID %>').val();
        var planCompleteDate = $('#<%=this.FV_ISI.FindControl("tbPlanCompleteDate").ClientID %>').val();

        var beginTimes = planStartDate.substring(0, 10).split('-');
        var endTimes = planCompleteDate.substring(0, 10).split('-');

        planStartDate = beginTimes[1] + '-' + beginTimes[2] + '-' + beginTimes[0] + ' ' + planStartDate.substring(10, 16) + ":00";
        planCompleteDate = endTimes[1] + '-' + endTimes[2] + '-' + endTimes[0] + ' ' + planCompleteDate.substring(10, 16) + ":59";

        var day = ((Date.parse(planCompleteDate) - Date.parse(planStartDate)) / 24 / 3600 / 1000).toFixed(1);

        if (day < 7) {
            return confirm("${Common.Button.Confirm.Assign7}");
        } else {
            return confirm("${Common.Button.Confirm.Assign}");
        }
    }

    function HideComment() {
        $("#lnkComment").hide();
    }

    function ShowComment() {

        $("#commentDiv").slideToggle();
        $("html,body").animate({ scrollTop: $("#commentDiv").offset().top }, 500);
        $(window).scroll();
    }

    var pageCount = 1;
    var pageRowCount = 15;
    var commentTotalCount = 0;
    var times = 1;
    var isOK = true; //记录上次访问是否已经结束，如果ajax也有线程就好了

    $(document).ready(function () {

        //TaskUploadify();

        BindAssignStartUser();
        
        $("#qrCodeDiv").dialog({
            autoOpen: false,
            modal: true
        });

        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });


        $("#loading").css({ "right": "2", "bottom": 0 });
        $("#commentDiv")
            .ajaxStart(function () {
                isOK = false; //执行ajax的时候把isOK设置成false防止第一次没有执行完的情况下执行第二次易出错
                $("#detail").hide();
                $("#loading").show();
                //alert('ajaxStart=' + isOK);
            })
            .ajaxStop(function () {
                isOK = true;
                $("#detail").show();
                $("#loading").hide();
                //alert('ajaxStop=' + isOK);
            });

        SetCommentCount();

        $(window).scroll(Add_Data);
    });

    function SetCommentCount() {
        Sys.Net.WebServiceProxy.invoke('Webservice/CommentMgrWS.asmx', 'GetCommentCount', false,
            { "taskCode": "<%=TaskCode%>" },
            function OnSucceeded(result, eventArgs) {
                $("#lnkComment").html("<font font-size='5px' color='black'>${ISI.TSK.Trace}(<font  color='blue'>" + result + "</font>)${ISI.TSK.CommentDetail}</font>");
            },
            function OnFailed(error) {
                //alert(error.get_message());
            }
        );
        }

        function Add_Data() {
            if (((times == 1 || (pageRowCount * (times - 1)) <= commentTotalCount)) && $("#commentDiv").css("display") != 'none' && $(document).height() == $(window).scrollTop() + document.documentElement.clientHeight) {
                //当前位置比上次的小就是往上滚动不要执行ajax代码块
                if (isOK)//如果是第一次或者上次执行完成了就执行本次
                {
                    // alert('pageRowCount=' + pageRowCount + '  times=' + times + '  commentTotalCount=' + commentTotalCount);

                    if (times == 1 && commentTotalCount == 0) {

                        Sys.Net.WebServiceProxy.invoke('Webservice/CommentMgrWS.asmx', 'GetCommentCount', false,
                            { "taskCode": "<%=TaskCode%>" },
                    function OnSucceeded(result, eventArgs) {
                        commentTotalCount = result;

                        //$("#tbComment").text(commentTotalCount);
                    },
                    function OnFailed(error) {
                        //alert(error.get_message());
                    }
                );
                        }

                        isOK = false;

                        Refresh(pageRowCount * (times - 1), pageRowCount);

                        times++;
                        isOK = true;

                    } else {
                    // $("#loading").append("<br/>~~你是向下滚了，但是上次还没有执行完毕，等等吧<br/>");
                    }
                }
            }

   
            function AddComment() {
                var comment = $("#tbComment").val();
                if (comment != '' && comment.replace(' ', '') != '') {
                    Sys.Net.WebServiceProxy.invoke('Webservice/CommentMgrWS.asmx', 'CreateComment', false,
                        {
                            "taskCode": "<%=TaskCode%>",
                            "subject": "",
                            "comment": comment,
                            "currentCount": 0,
                            "count": 0,
                            "displayIndex": 0, "userCode": "<%=CurrentUser.Code%>", "userName": "<%=CurrentUser.Name%>"
                        },
                function OnSucceeded(result, eventArgs) {
                    $('#detail').prepend(CommentHtml(result.Id, result.Value, result.Type, result.CreateUser, result.CreateDate));
                    SetCommentCount();
                    $("#tbComment").val('');
                },
                function OnFailed(error) {
                    //alert(error.get_message());
                }
                );
                    }
                }

                function CommentHtml(id, value, type, createUser, createDate) {
                    var pstyle='';
                    if(type=='Status'){
                        type='${ISI.TSK.Status}: ';
                    }else if(type=='Approve'){
                        type='${ISI.TSK.Approve}: ';
                    }else if(type=='Comment'){
                        type='${ISI.TSK.Comment}: ';
                        pstyle='text-align:right;';
                    }else
                    {
                        type='';
                    }
                    return "<div id='" + id + type + "' ><p style='"+ pstyle +"'><span style='background:#DFD3D3'>"+type+"</span>" + value + "</p><p  style='"+pstyle+"'><span>" + formatDate(createDate) + "&nbsp;&nbsp;${ISI.TSK.ComeFrom}&nbsp;&nbsp;" + createUser + "</span></p><hr>";
                }

                function formatDate(d) {
                    var str = d.format("yyyy-MM-dd HH:mm");
                    //var str = (1900 + d.getYear()) + "-" + (d.getMonth() + 1) + "-" + d.getDate() + " " + d.getHours() + ":" + d.getMinutes();
                    return str;
                }

                function Refresh(firstRow, maxRows) {
                    Sys.Net.WebServiceProxy.invoke('Webservice/CommentMgrWS.asmx', 'GetComment', false,
                        { "taskCode": "<%=TaskCode%>", "firstRow": firstRow, "maxRows": maxRows },
            function OnSucceeded(result, eventArgs) {
                //alert("第" + times + "次追加数据.");

                if (result != null) {
                    for (var i = 0; i < result.length; i++) {
                        $('#detail').append(CommentHtml(result[i].Id, result[i].Value, result[i].Type, result[i].CreateUser, result[i].CreateDate));
                    }
                }
            },
            function OnFailed(error) {
                //alert(error.get_message());
            }
        );
                    }

                    function BindAssignStartUser() {

                        Sys.Net.WebServiceProxy.invoke('Webservice/UserMgrWS.asmx', 'GetAllUser', false,
                                {},
                            function OnSucceeded(result, eventArgs) {
                                //alert("第" + times + "次追加数据.");
                                if (result != null) {
                                    var tags = result;

                                    $('#<%=this.FV_ISI.FindControl("tbAssignStartUser").ClientID %>').tagit({
                                        availableTags: tags,
                                        allowNotDefinedTags: false,
                                        removeConfirmation: true,
                                        placeholderText: '${ISI.TSK.InputUserCode}',
                                        onTagClicked: function (evt, ui) {

                                            //$('#<%=this.FV_ISI.FindControl("tbAssignStartUser").ClientID %>').tagit('removeTag', ui);
                                            //var lable = $('#<%=this.FV_ISI.FindControl("tbAssignStartUser").ClientID %>').tagit('tagLabel', ui);
                                            //$('#<%=this.FV_ISI.FindControl("tbAssignStartUser").ClientID %>').tagit('prependTag', lable);

                                            $('#<%=this.FV_ISI.FindControl("tbAssignStartUser").ClientID %>').tagit('prependTag', ui);
                                        }
                                    });

                                    $('#<%=this.FV_ISI.FindControl("tbCountersignUser").ClientID %>').tagit({
                                        availableTags: tags,
                                        allowNotDefinedTags: false,
                                        removeConfirmation: true,
                                        placeholderText: '${ISI.TSK.InputUserCode}',
                                        onTagClicked: function (evt, ui) {
                                            $('#<%=this.FV_ISI.FindControl("tbCountersignUser").ClientID %>').tagit('prependTag', ui);
                                        }
                                    });
                                    
                                        $('#<%=this.FV_ISI.FindControl("tbWorkHoursUser").ClientID %>').tagit({
                                        availableTags: tags,
                                        allowNotDefinedTags: false,
                                        removeConfirmation: true,
                                        placeholderText: '${ISI.TSK.InputUserCode}',
                                        onTagClicked: function (evt, ui) {
                                            $('#<%=this.FV_ISI.FindControl("tbWorkHoursUser").ClientID %>').tagit('prependTag', ui);
                                        }
                                        });

                                        $('#<%=tbHelpUser.ClientID %>').tagit({
                                        availableTags: tags,
                                        allowNotDefinedTags: false,
                                        removeConfirmation: true,
                                        placeholderText: '${ISI.TSK.InputUserCode}',
                                        onTagClicked: function (evt, ui) {
                                            $('#<%=tbHelpUser.ClientID %>').tagit('prependTag', ui);
                                        }
                                        });
                                    }
                            },
            function OnFailed(error) {
                //alert(error.get_message());
            }
        );
                            }
     
                            function ShowQRCode()
                            {
                                $("#qrCodeDiv").html('');
                                var str = "";
                                str+='${ISI.TSK.Code}：<%=this.TaskCode%>';
                                var subject=$('#<%=this.FV_ISI.FindControl("tbSubject").ClientID %>').val();
                                if(subject.length>0){
                                    str+="\n${ISI.TSK.Subject}："+subject;
                                }
                                var desc1 = $('#<%=this.FV_ISI.FindControl("tbDesc1").ClientID %>').val();
                                if(desc1.length>0){
                                    str+="\n${ISI.TSK.Desc1}："+desc1;
                                }                                
                                str = toUtf8(str);
                                $("#qrCodeDiv").qrcode({ 
                                    render: !!document.createElement('canvas').getContext ? 'canvas' : 'table',
                                    correctLevel: 0,
                                    text:str
                                });  
                                $("#qrCodeDiv").dialog("open");
                            }
</script>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_ISI" runat="server" DataSourceID="ODS_ISI" DefaultMode="Edit"
        Width="100%" DataKeyNames="Code" OnDataBound="FV_ISI_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Common.Business.BasicInfo}</legend>
                <table class="mtable" runat="server" id="taskTable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlTaskSubType" runat="server" Text="${ISI.TSK.TaskSubType}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                                CssClass="inputRequired" Width="260" OnTextChanged="tbTaskSubType_TextChanged" AutoPostBack="true" />
                            <cc1:ReadonlyTextBox ID="rtbTaskSubType" runat="server" CodeField="TaskSubType.Code"
                                DescField="TaskSubType.Desc" Visible="false" />
                            <asp:RequiredFieldValidator ID="rfvTaskSubType" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbTaskSubType" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblPriority" runat="server" Text="${ISI.TSK.Priority}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlPriority" Code="ISIPriority" runat="server" IncludeBlankOption="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCode" runat="server" Text="${ISI.TSK.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lbStatus" runat="server" Text="${Common.CodeMaster.Status}:" />
                        </td>
                        <td class="td02">
                            <asp:Literal ID="lblStatus" runat="server" />
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Literal ID="lblLevel" runat="server" />
                            <asp:HiddenField runat="server" ID="hfLevel" Value='<%# Bind("Level") %>' />
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <cc1:CodeMstrLabel ID="cmlFlag" Width="50" CssClass="CssColor" Font-Size="11" Code="ISIFlag"
                                runat="server" Value='<%# Bind("Flag") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSubject" runat="server" Text="${ISI.TSK.Subject}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSubject" runat="server" Text='<%# Bind("Subject") %>' Width="80%"
                                onkeypress="javascript:setMaxLength(this,50);" onpaste="limitPaste(this, 50)" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblBackYards" runat="server" Text="${ISI.TSK.BackYards}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbBackYards" runat="server" Text='<%# Bind("BackYards") %>' onkeypress="javascript:setMaxLength(this,50);"
                                onpaste="limitPaste(this, 50)" />
                        </td>
                    </tr>
                    <tr runat="server" id="trWF" visible="false">
                        <td class="td01">
                            <asp:Literal ID="lblExtNo" runat="server" Text="${ISI.TSK.ExtNo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbExtNo" runat="server" Text='<%# Bind("ExtNo") %>'
                                onkeypress="javascript:setMaxLength(this,50);" onpaste="limitPaste(this, 50)" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblCostCenter" runat="server" Text="${ISI.TSK.CostCenter}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbCostCenter" runat="server" Visible="true" DescField="Desc" MustMatch="true"
                                ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetCostCenter" OnTextChanged="tbCostCenter_TextChanged"
                                AutoPostBack="true" CssClass="inputRequired" ServiceParameter="bool:true" />
                            <asp:RequiredFieldValidator ID="rfvCostCenter" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbCostCenter" ValidationGroup="vgSave" />
                            <cc1:ReadonlyTextBox ID="rtbCostCenter" runat="server" CodeField="CostCenterCode"
                                DescField="CostCenterDesc" Visible="false" />
                        </td>
                    </tr>
                    <tr runat="server" id="trAccount" visible="false">
                        <td class="td01">
                            <asp:Label ID="ltlAccount1" runat="server" Text="${WFS.Cost.Account1}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbAccount1" runat="server" Visible="true" DescField="Account1Desc" MustMatch="true"
                                ValueField="Account1" ServicePath="BudgetDetMgr.service" ServiceMethod="GetAccount1"
                                ServiceParameter="string:#tbCostCenter"  CssClass="inputRequired"/>
                            <asp:RequiredFieldValidator ID="rfvAccount1" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbAccount1" ValidationGroup="vgSave" />
                            <cc1:ReadonlyTextBox ID="rtbAccount1" runat="server" Visible="false" CodeField="Account1"
                                DescField="Account1Desc" />
                        </td>
                        <td class="td01">
                            <asp:Label ID="ltlAccount2" runat="server" Text="${WFS.Cost.Account2}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbAccount2" runat="server" Visible="true" DescField="Account2Desc" MustMatch="true"
                                ValueField="Account2" ServicePath="BudgetDetMgr.service" ServiceMethod="GetAccount2"
                                ServiceParameter="string:#tbCostCenter,string:#tbAccount1" CssClass="inputRequired"/>
                            <asp:RequiredFieldValidator ID="rfvAccount2" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbAccount2" ValidationGroup="vgSave" />
                            <cc1:ReadonlyTextBox ID="rtbAccount2" runat="server" Visible="false" CodeField="Account2"
                                DescField="Account2Desc" />
                        </td>
                    </tr>
                    <tr id="isPrj" runat="server" visible="false">
                        <td class="td01">
                            <asp:Literal ID="lblPhase" runat="server" Text="${ISI.TSK.Phase}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlPhase" Code="ISIPhase" runat="server" IncludeBlankOption="false" /></td>
                        <td class="td01">
                            <asp:Literal ID="lblSeq" runat="server" Text="${ISI.TSK.Seq}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSeq" runat="server" Width="80" Text='<%# Bind("Seq") %>'></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblTaskAddress" runat="server" Text="${ISI.TSK.TaskAddress}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbTaskAddress" runat="server" Visible="true" DescField="Code" MustMatch="false"
                                ValueField="Name" ServicePath="TaskAddressMgr.service" ServiceMethod="GetCacheAllTaskAddress"
                                CssClass="inputRequired" />
                            <asp:TextBox ID="rtbTaskAddress" runat="server" Text='<%# Bind("TaskAddress") %>'
                                ReadOnly="true" Visible="false" />
                            <asp:RequiredFieldValidator ID="rfvTaskAddress" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbTaskAddress" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblFailureMode" runat="server" Text="${ISI.TSK.FailureMode}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbFailureMode" runat="server" Visible="true" DescField="Desc" MustMatch="true"
                                ValueField="Code" ServicePath="FailureModeMgr.service" ServiceMethod="GetAllFailureMode"
                                ServiceParameter="string:#tbTaskSubType" />
                            <cc1:ReadonlyTextBox ID="rtbFailureMode" runat="server" CodeField="FailureMode"
                                DescField="FailureMode" Visible="false" />
                        </td>
                    </tr>
                    <tr id="trFormType2" runat="server" visible="false">
                        <td class="td01">
                            <asp:Literal ID="lblTravelType" runat="server" Text="${WFS.Cost.TravelType}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlTravelType" Code="TravelType" runat="server" IncludeBlankOption="false" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblPayee" runat="server" Text="${WFS.Cost.Payee}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbPayeeCode" runat="server" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" Width="260" />
                            <cc1:ReadonlyTextBox ID="rtbPayeeCode" runat="server" Visible="false" CodeField="PayeeCode"
                                DescField="PayeeName" />
                        </td>
                    </tr>
                    <tr id="isImp" runat="server" visible="false">
                        <td class="td01">
                            <asp:Literal ID="lblAmount" runat="server" Text="${ISI.TSK.ImpAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbAmount" runat="server" Text='<%# Bind("Amount","{0:0.########}") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvAmount" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbAmount" Visible="false" ValidationGroup="vgSave" />
                            <asp:RangeValidator ID="rvAmount" runat="server" ControlToValidate="tbAmount"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                                Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                            <asp:Literal ID="ltlAmount" runat="server" Text='<%# Bind("AmountDesc") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblVoucher" runat="server" Text="${WFS.Cost.Voucher}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbVoucher" runat="server" Text='<%# Bind("Voucher") %>'></asp:TextBox>
                            <asp:RangeValidator ID="rvVoucher" runat="server" ControlToValidate="tbVoucher"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                                Type="Integer" MaximumValue="99999999" MinimumValue="-99999999" />
                        </td>
                    </tr>
                    <tr id="trAmount" runat="server" visible="false">
                        <td class="td01">
                            <asp:Literal ID="lblTaxes" runat="server" Text="${WFS.Cost.Taxes}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbTaxes" runat="server" Text='<%# Bind("Taxes","{0:0.########}") %>'></asp:TextBox>
                            <asp:RangeValidator ID="rvTaxes" runat="server" ControlToValidate="tbTaxes"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                                Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                            <asp:Literal ID="ltlTaxes" runat="server" Text='<%# Bind("TaxesDesc") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblTotalAmount" runat="server" Text="${WFS.Cost.TotalAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbTotalAmount" runat="server" Text='<%# Bind("TotalAmount","{0:0.########}") %>' ReadOnly="true"></asp:TextBox>
                            <asp:RangeValidator ID="rvTotalAmount" runat="server" ControlToValidate="tbTotalAmount"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                                Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                            <asp:Literal ID="ltlTotalAmount" runat="server" Text='<%# Bind("TotalAmountDesc") %>' />
                        </td>
                    </tr>
                    <tr id="trQty" runat="server" visible="false">
                        <td class="td01">
                            <asp:Literal ID="lblQty" runat="server" Text="${ISI.TSK.Qty}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbQty" runat="server" Text='<%# Bind("Qty","{0:0.########}") %>'></asp:TextBox>
                            <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="tbQty"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                                Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                        </td>
                    </tr>
                    <tr id="isIss" runat="server" visible="false">
                        <td class="td01">
                            <asp:Literal ID="lblSupplier" runat="server" Text="${Common.Business.Supplier}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbSupplier" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="SupplierMgr.service" ServiceMethod="GetAllSupplier" />
                            <cc1:ReadonlyTextBox ID="rtbSupplier" runat="server" CodeField="SupplierCode"
                                DescField="SupplierName" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblDesc1" runat="server" Text="${ISI.TSK.Desc1}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbDesc1" runat="server" Text='<%# Bind("Desc1") %>' Height="60"
                                Width="77%" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,2000);"
                                onpaste="limitPaste(this, 2000)" Font-Size="10" />
                        </td>
                    </tr>

                    <tr runat="server" id="ctrlTR" visible="false">
                        <td class="td01">
                            <asp:CheckBox runat="server" ID="cbIsCountersignSerial" Text="${ISI.TSK.IsCountersignSerial}:" Enabled="false" />
                        </td>
                        <td class="td02" colspan="5">
                            <asp:TextBox ID="tbCountersignUser" runat="server" CssClass="inputRequired" Text='' Width="100%" Visible="false" />
                            <asp:TextBox ID="rtbCountersignUser" runat="server" Text='<%# Bind("CountersignUser") %>' Height="30"
                                Width="77%" TextMode="MultiLine" Font-Size="10" ReadOnly="true" />
                            <asp:RequiredFieldValidator ID="rfvCountersignUser" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbCountersignUser" ValidationGroup="vgAssign" />
                        </td>
                    </tr>
                    <tr runat="server" id="workHoursTR2" visible="false">
                        <td class="td01">
                            <asp:Literal ID="lblWorkHoursUser" runat="server" Text="${ISI.TSK.User}:" />
                        </td>
                        <td class="td02" colspan="5">
                            <asp:TextBox ID="tbWorkHoursUser" runat="server" Text='' Width="100%" />
                            <asp:TextBox ID="rtbWorkHoursUser" runat="server" Text='<%# Bind("WorkHoursUser") %>' Height="30"
                                Width="77%" TextMode="MultiLine" Font-Size="10" ReadOnly="true" />
                            <asp:RequiredFieldValidator ID="rfvWorkHoursUser" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbWorkHoursUser" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                </table>
                <div id="divMore" style="display: none">
                    <table class="mtable">
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblEmail" runat="server" Text="${ISI.TSK.Email}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbEmail" runat="server" Text='<%# Bind("Email") %>' onkeypress="javascript:setMaxLength(this,50);"
                                    onpaste="limitPaste(this, 50)" />
                                <asp:RegularExpressionValidator ID="revEmail" runat="server" ErrorMessage="${ISI.Error.MailIsInvalid}"
                                    ControlToValidate="tbEmail" ValidationGroup="vgSave" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblTemplate" runat="server" Text="${Common.Business.Template}:" />
                            </td>
                            <td class="td02">
                                <cc1:CodeMstrLabel ID="cmlTemplate" Code="ISITemplate" runat="server" Value='<%# Bind("Template") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblMobilePhone" runat="server" Text="${ISI.TSK.MobilePhone}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbMobilePhone" runat="server" Text='<%# Bind("MobilePhone") %>'
                                    onkeypress="javascript:setMaxLength(this,50);" onpaste="limitPaste(this, 50)" />
                                <asp:RegularExpressionValidator ID="revMobilePhone" runat="server" ErrorMessage="${ISI.Error.MobilePhoneIsInvalid}"
                                    ControlToValidate="tbMobilePhone" ValidationGroup="vgSave" ValidationExpression="^1[358]\d{9}$" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblUserName" runat="server" Text="${ISI.TSK.UserName}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbUserName" runat="server" Text='<%# Bind("UserName") %>' onkeypress="javascript:setMaxLength(this,50);"
                                    onpaste="limitPaste(this, 50)" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblCreateDate" runat="server" Text="${Common.Business.CreateDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCreateDate" runat="server" CodeField="CreateDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblCreateUser" runat="server" Text="${Common.Business.CreateUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCreateUser" runat="server" CodeField="CreateUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblReturnDate" runat="server" Text="${ISI.TSK.ReturnDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbReturnDate" runat="server" CodeField="ReturnDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblReturnUser" runat="server" Text="${ISI.TSK.ReturnUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbReturnUSer" runat="server" CodeField="ReturnUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblSubmitDate" runat="server" Text="${ISI.TSK.SubmitDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbSubmitDate" runat="server" CodeField="SubmitDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblSubmitUser" runat="server" Text="${ISI.TSK.SubmitUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbSubmitUser" runat="server" CodeField="SubmitUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblInApproveDate" runat="server" Text="${ISI.TSK.InApproveDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbInApproveDate" runat="server" CodeField="InApproveDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblInApproveUser" runat="server" Text="${ISI.TSK.InApproveUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbInApproveUser" runat="server" CodeField="InApproveUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblInDisputeDate" runat="server" Text="${ISI.TSK.InDisputeDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbInDisputeDate" runat="server" CodeField="InDisputeDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblInDisputeUser" runat="server" Text="${ISI.TSK.InDisputeUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbInDisputeUser" runat="server" CodeField="InDisputeUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblRefuseDate" runat="server" Text="${ISI.TSK.RefuseDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbRefuseDate" runat="server" CodeField="RefuseDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblRefuseUser" runat="server" Text="${ISI.TSK.RefuseUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbRefuseUser" runat="server" CodeField="RefuseUserNm" />
                            </td>
                        </tr>

                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblApproveDate" runat="server" Text="${ISI.TSK.ApproveDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbApproveDate" runat="server" CodeField="ApproveDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblApproveUser" runat="server" Text="${ISI.TSK.ApproveUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbApproveUser" runat="server" CodeField="ApproveUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblAssignDate" runat="server" Text="${ISI.TSK.AssignDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbAssignDate" runat="server" CodeField="AssignDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblAssignUser" runat="server" Text="${ISI.TSK.AssignUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbAssignUser" runat="server" CodeField="AssignUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblStartDate" runat="server" Text="${ISI.TSK.StartDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbStartDate" runat="server" CodeField="StartDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblStartUser" runat="server" Text="${ISI.TSK.StartUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbStartUser" runat="server" CodeField="StartUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblSuspendDate" runat="server" Text="${ISI.TSK.SuspendDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbSuspendDate" runat="server" CodeField="SuspendDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblSuspendUser" runat="server" Text="${ISI.TSK.SuspendUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbSuspendUser" runat="server" CodeField="SuspendUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblCompleteDate" runat="server" Text="${ISI.TSK.CompleteDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCompleteDate" runat="server" CodeField="CompleteDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblCompleteUser" runat="server" Text="${ISI.TSK.CompleteUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCompleteUser" runat="server" CodeField="CompleteUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblCancelDate" runat="server" Text="${ISI.TSK.CancelDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCancelDate" runat="server" CodeField="CancelDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblCancelUser" runat="server" Text="${ISI.TSK.CancelUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCancelUser" runat="server" CodeField="CancelUserNm" />
                            </td>
                        </tr>

                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblRejectDate" runat="server" Text="${ISI.TSK.RejectDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbRejectDate" runat="server" CodeField="RejectDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblRejectUser" runat="server" Text="${ISI.TSK.RejectUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbRejectUser" runat="server" CodeField="RejectUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblCloseDate" runat="server" Text="${ISI.TSK.CloseDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCloseDate" runat="server" CodeField="CloseDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblCloseUser" runat="server" Text="${ISI.TSK.CloseUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCloseUser" runat="server" CodeField="CloseUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblOpenDate" runat="server" Text="${ISI.TSK.OpenDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbOpenDate" runat="server" CodeField="OpenDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblOpenUser" runat="server" Text="${ISI.TSK.OpenUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbOpenUser" runat="server" CodeField="OpenUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblLastModifyDate" runat="server" Text="${ISI.TSK.LastModifyDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbLastModifyDate" runat="server" CodeField="LastModifyDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblLastModifyUser" runat="server" Text="${ISI.TSK.LastModifyUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbLastModifyUser" runat="server" CodeField="LastModifyUserNm" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="mtable">
                        <tr>
                            <td class="td01">
                                <a type="text/html" onclick="More()" href="#" visible="true" id="more">More... </a>
                            </td>
                            <td class="td02"></td>
                            <td class="td01"></td>
                            <td class="td02"></td>
                        </tr>
                    </table>
                </div>
            </fieldset>
            <fieldset id="fsFlow" runat="server">
                <legend>${ISI.TSK.Flow}</legend>
                <table class="mtable">
                    <div runat="server">
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblPlanStartDate" runat="server" Text="${ISI.TSK.PlanStartDate}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbPlanStartDate" runat="server" onClick="var ctl01_ucEdit_ucEdit_FV_ISI_tbPlanCompleteDate=$dp.$('ctl01_ucEdit_ucEdit_FV_ISI_tbPlanCompleteDate');WdatePicker({startDate:'%y-%M-%d 08:00:00',dateFmt:'yyyy-MM-dd HH:mm',onpicked:function(){ctl01_ucEdit_ucEdit_FV_ISI_tbPlanCompleteDate.click();},maxDate:'#F{$dp.$D(\'ctl01_ucEdit_ucEdit_FV_ISI_tbPlanCompleteDate\')}' })"
                                    Text='<%# Bind("PlanStartDate","{0:yyyy-MM-dd HH:mm}") %>' CssClass="inputRequired" Width="138" />
                                <asp:RequiredFieldValidator ID="rfvPlanStartDate" runat="server" ErrorMessage="${Common.String.Empty}"
                                    Display="Dynamic" ControlToValidate="tbPlanStartDate" ValidationGroup="vgAssign"
                                    Enabled="true" />
                            <td class="ttd01" style="white-space: nowrap;">
                                <asp:Literal ID="lblPlanCompleteDate" runat="server" Text="${ISI.TSK.PlanCompleteDate}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbPlanCompleteDate" runat="server" onClick="WdatePicker({doubleCalendar:true,startDate:'%y-%M-{%d+7} 16:30:00',dateFmt:'yyyy-MM-dd HH:mm',minDate:'#F{$dp.$D(\'ctl01_ucEdit_ucEdit_FV_ISI_tbPlanStartDate\')}'})"
                                    Text='<%# Bind("PlanCompleteDate","{0:yyyy-MM-dd HH:mm}") %>' CssClass="inputRequired" Width="138" />
                                <asp:RequiredFieldValidator ID="rfvPlanCompleteDate" runat="server" ErrorMessage="${Common.String.Empty}"
                                    Display="Dynamic" ControlToValidate="tbPlanCompleteDate" ValidationGroup="vgAssign"
                                    Enabled="true" />
                            </td>
                            <td class="td01" style="width: 10%;">
                                <asp:Literal ID="lblSchedulingStartUser" runat="server" Text="${ISI.TSK.SchedulingStartUser}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbSchedulingStartUser" runat="server" ReadOnly="true" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblAssignStartUser" runat="server" Text="${ISI.TSK.AssignStartUser}:" />
                            </td>
                            <td class="td02" colspan="5">
                                <asp:TextBox ID="tbAssignStartUser" runat="server" CssClass="inputRequired" Text='<%# Bind("AssignStartUser") %>'
                                    Width="100%" />
                                <asp:TextBox ID="rtbAssignStartUser" runat="server" Text='<%# Bind("AssignStartUser") %>' Height="30"
                                    Width="77%" TextMode="MultiLine" Font-Size="10" ReadOnly="true" />
                                <asp:RequiredFieldValidator ID="rfvAssignStartUser" runat="server" ErrorMessage="${Common.String.Empty}"
                                    Display="Dynamic" ControlToValidate="tbAssignStartUser" ValidationGroup="vgAssign" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblExpectedResults" runat="server" Text="${ISI.TSK.ExpectedResults}:" />
                            </td>
                            <td class="td02" colspan="5">
                                <asp:TextBox ID="tbExpectedResults" runat="server" Text='<%# Bind("ExpectedResults") %>'
                                    Height="50" Width="77%" TextMode="MultiLine" MaxLength="5" onkeypress="javascript:setMaxLength(this,500);"
                                    onpaste="limitPaste(this, 500)" Font-Size="10" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="ltlDesc2" runat="server" Text="${ISI.TSK.Desc2}:" />
                            </td>
                            <td class="td02" colspan="5">
                                <asp:TextBox ID="tbDesc2" runat="server" Text='<%# Bind("Desc2") %>' Height="50"
                                    Width="77%" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,500);"
                                    onpaste="limitPaste(this, 500)" Font-Size="10" />
                            </td>
                        </tr>
                    </div>
                </table>
            </fieldset>
            <uc2:CostDet ID="ucCostDet" runat="server" Visible="false" />
            <uc2:CostList ID="ucCostList" runat="server" Visible="false" />
            <fieldset id="fsApprove" runat="server" visible="false">
                <legend>${ISI.TSK.Approve}</legend>
                <table class="mtable">
                    <tr>
                        <td style="width: 20%" rowspan="2"></td>
                        <td rowspan="2">
                            <asp:TextBox ID="tbApprove" runat="server" Text=''
                                Height="50" Width="100%" TextMode="MultiLine" MaxLength="5" onkeypress="javascript:setMaxLength(this,1000);"
                                onpaste="limitPaste(this, 1000)" Font-Size="10" CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvApprove" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbApprove" ValidationGroup="vgApporve" />
                        </td>
                        <td style="width: 20%; vertical-align: top; text-align: center;">
                            <asp:Literal ID="lblColor" runat="server" Text="${Common.CodeMaster.Color}:" Visible="false" />
                            <cc1:CodeMstrDropDownList ID="ddlColor" Code="ISIColor" runat="server" IncludeBlankOption="false" Visible="false">
                            </cc1:CodeMstrDropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20%; vertical-align: bottom; text-align: center;">
                            <div>
                                <cc1:Button ID="btnApprove" runat="server" OnClick="btnApprove_Click" Text="${ISI.TSK.Button.Approve}"
                                    CssClass="button2" OnClientClick="return confirm('${ISI.TSK.Button.Approve.Confirm}')" ValidationGroup="vgApporve" Visible="false" />
                                <cc1:Button ID="btnReturn" runat="server" Text="${ISI.TSK.Button.Return}" CssClass="button2"
                                    OnClick="btnReturn_Click" OnClientClick="return confirm('${ISI.TSK.Button.Return.Confirm}')" ValidationGroup="vgApporve" Visible="true" />
                            </div>
                            <div>
                                <asp:Button ID="btnDispute" runat="server" Text="${ISI.TSK.Button.Dispute}" CssClass="button2"
                                    OnClick="btnDispute_Click" OnClientClick="return confirm('${ISI.TSK.Button.Dispute.Confirm}')" ValidationGroup="vgApporve" Visible="false" />

                                <cc1:Button ID="btnRefuse" runat="server" Text="${ISI.TSK.Button.Refuse}" CssClass="button2"
                                    OnClick="btnRefuse_Click" OnClientClick="return confirm('${ISI.TSK.Button.Refuse.Confirm}')" ValidationGroup="vgApporve" />
                            </div>
                        </td>
                </table>
            </fieldset>

            <div id="uploadfileQueue" name="uploadfileQueue" align="center"></div>
            <table class="mtable">
                <tr>
                    <td class="td01" align="left">
                        <a onclick="javascript:ShowComment();" id="lnkComment" name="lnkComment" href="#"></a>
                    </td>
                    <td class="td01"></td>
                    <td class="td01" style="text-align: center;">
                        <span class="link" id="lblFile" name="lblFile" onclick="TaskUploadify()">${ISI.Status.Upload}</span>
                        <input type="file" id="uploadify" name="uploadify" style="display: none;" />
                    </td>
                    <td class="td01" style="text-align: center;">
                        <span onclick="javascript:ShowQRCode();" class="link" id="lnkQRCode">${ISI.TSK.Button.QRCode}</span>
                    </td>
                    <td style="text-align: center;">
                        <span class="buttons">
                            <cc1:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="${Common.Button.Save}"
                                CssClass="apply" ValidationGroup="vgSave" />
                            <cc1:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="${Common.Button.Submit}"
                                CssClass="apply" ValidationGroup="vgSave" />
                            <cc1:Button ID="btnAssign" runat="server" OnClick="btnAssign_Click" Text="${Common.Button.Assign}"
                                CssClass="button2" ValidationGroup="vgAssign" OnClientClick="return ConfirmAssign()" />
                            <cc1:Button ID="btnStart" runat="server" OnClick="btnStart_Click" Text="${Common.Button.Start}"
                                CssClass="button2" ValidationGroup="vgAssign" OnClientClick="return confirm('${ISI.TSK.Confirm.Start}')" />
                            <cc1:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click" Text="${Common.Button.Delete}"
                                CssClass="delete" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                            <cc1:Button ID="btnComplete" runat="server" Text="${ISI.TSK.Button.Complete}" CssClass="button2"
                                OnClick="btnComplete_Click" OnClientClick="return confirm('${ISI.TSK.Button.Complete.Confirm}')" />
                            <cc1:Button ID="btnReject" runat="server" Text="${ISI.TSK.Button.Reject}" CssClass="button2"
                                OnClick="btnReject_Click" OnClientClick="return confirm('${ISI.TSK.Button.Reject.Confirm}')" />
                            <cc1:Button ID="btnClose" runat="server" Text="${Common.Button.Close}" CssClass="button2"
                                OnClick="btnClose_Click" OnClientClick="return confirm('${Common.Button.Close.Confirm}')" />
                            <cc1:Button ID="btnCancel" runat="server" Text="${Common.Button.Cancel}" CssClass="button2"
                                OnClick="btnCancel_Click" OnClientClick="return confirm('${Common.Button.Cancel.Confirm}')" />
                            <cc1:Button ID="btnOpen" runat="server" Text="${ISI.TSK.Button.Open}" CssClass="button2"
                                OnClick="btnOpen_Click" OnClientClick="return confirm('${ISI.TSK.Button.Open.Confirm}')" Visible="false" />
                            <cc1:Button ID="btnHelp" runat="server" Text="${ISI.TSK.Button.Help}" OnClick="btnHelp_Click"
                                CssClass="button2" />
                            <cc1:Button ID="btnPrint" runat="server" Text="${Common.Button.Print} ?" CssClass="button2"
                                OnClick="btnPrint_Click" Visible="false" ToolTip="${ISI.TSK.Print.Tip}" />
                            <cc1:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                CssClass="back" />
                        </span>
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_ISI" runat="server" TypeName="com.Sconit.Web.TaskMstrMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.TaskMstr" SelectMethod="LoadTaskMstr">
    <SelectParameters>
        <asp:Parameter Name="code" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
<div id="floatdiv">
    <fieldset id="fsHelp" runat="server" visible="false">
        <legend>${ISI.TSK.Help}</legend>
        <table class="mtable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblHelpUser" runat="server" Text="${ISI.TSK.Help.HelpUser}:" />
                </td>
                <td>
                    <asp:TextBox ID="tbHelpUser" runat="server" CssClass="inputRequired" Width="80%" />
                </td>
                <td align="left" valign="bottom">
                    <div class="buttons">
                        <asp:Button ID="btnSend" runat="server" Text="${ISI.TSK.Button.Send}" OnClick="btnSend_Click"
                            CssClass="button2" Visible="true" ValidationGroup="vgHelp" />
                        <asp:Button ID="btnSendClose" runat="server" Width="59px" CssClass="button2" Visible="true"
                            OnClick="btnSendClose_Click" />
                    </div>
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlHelpContent" runat="server" Text="${ISI.TSK.Help.Content}:" />
                </td>
                <td>
                    <asp:TextBox ID="tbHelpContent" runat="server" Height="80" Width="100%" TextMode="MultiLine"
                        onkeypress="javascript:setMaxLength(this,1000);" onpaste="limitPaste(this, 1000)"
                        Font-Size="10" />
                    <asp:RequiredFieldValidator ID="rfvHelpContent" runat="server" ErrorMessage="${Common.String.Empty}"
                        Display="Dynamic" ControlToValidate="tbHelpContent" ValidationGroup="vgHelp" />
                </td>
            </tr>
            <tr>
                <td class="td01"></td>
                <td>
                    <asp:CheckBox ID="cbIsRemindCreateUser" runat="server" Text="${ISI.TaskStatus.IsRemindCreateUser}" />
                    <asp:CheckBox ID="cbIsRemindAssignUser" runat="server" Text="${ISI.TaskStatus.IsRemindAssignUser}" />
                    <asp:CheckBox ID="cbIsRemindStartUser" runat="server" Text="${ISI.TaskStatus.IsRemindStartUser}" />
                    <asp:CheckBox ID="cbIsRemindCommentUser" runat="server" Text="${ISI.TaskStatus.IsRemindCommentUser}" />
                    <asp:CheckBox ID="cbIsRemindAdmin" runat="server" Text="${ISI.TSK.Help.Admin}" />
                </td>
            </tr>
        </table>
    </fieldset>
</div>
<div id="commentDiv" style="display: none">
    <fieldset>
        <legend>${ISI.TSK.Comment}</legend>
        <table class="mtable">
            <tr>
                <td style="width: 20%"></td>
                <td>
                    <div style="border: 1px solid #CCC">
                        <textarea rows="4" cols="50" id="tbComment" name="tbComment" style="height: 60px; margin: 0px; padding: 0px; border: 0px none rgb(0, 0, 0); width: 100%;"
                            onkeypress="javascript:setMaxLength(this,160);"
                            onpaste="limitPaste(this, 160)"></textarea>
                    </div>
                </td>
                <td style="width: 20%"></td>
            </tr>
            <tr>
                <td style="width: 20%"></td>
                <td align="right">
                    <button type="button" id="btnComment" class="button2" name="btnComment" onclick="AddComment()">
                        ${ISI.TSK.Button.Comment}
                    </button>
                </td>
                <td style="width: 20%"></td>
            </tr>
            <tr>
                <td style="width: 20%"></td>
                <td>
                    <div id="detail">
                    </div>
                </td>
                <td style="width: 20%"></td>
            </tr>
            <tr>
                <td style="width: 20%"></td>
                <td>
                    <div id="loading" style="display: none">
                        数据加载中，请稍后！
                    </div>
                </td>
                <td style="width: 20%"></td>
            </tr>
        </table>
    </fieldset>
</div>
<div id="qrCodeDiv" style="display: none;" />
