<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RefTask.ascx.cs" Inherits="ISI_TSK_RefTask" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<style type="text/css">
    .ui-button-text-only .ui-button-text {
        padding: 0.2em 0.3em 0.2em 0.3em;
    }
</style>
<script language="javascript" type="text/javascript">

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



    $(function () {
        initialize();

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
                                $(control + "lblComment").html("<span style='color:#0000E5;'>" + result.CreateUser + "&#40;<span style='color:fuchsia;'>" + formatDate(result.CreateDate) + "</span>&#41;</span>&#58;&nbsp;" + result.Value + "<span style='color:#0000E5;'>&#40;<span style='color:fuchsia;'><b>" + result.CurrentCount + "</b></span>&#47;" + result.Count + "&#41;</span>");

                                initialize();

                                $("#lnkComment" + result.TaskCode).unbind('click').removeAttr('onclick');
                                //ShowStatus(taskCode, displayIndex, currentStatusCount, statusCount, flag, color)
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
                "${Common.Button.Cancel}": function () {
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
                    var taskCode = $("#hdTaskCode").val();
                    var subject = $("#hdSubject").val();
                    var currentCount = $("#hdCurrentCount").val();
                    var count = $("#hdCount").val();
                    var color = $("#<%=this.ddlColor.ClientID%>").val();
                    var flag = $("#<%=this.ddlFlag.ClientID%>").val();
                    var startDate = $("#tbStartDate").val();
                    var endDate = $("#tbEndDate").val();
                    var flag = $("#<%=this.ddlFlag.ClientID%>").val();
                    //var isCurrentStatus = $("#cbIsCurrentStatus").attr("checked");
                    var isCurrentStatus = document.getElementById("cbIsCurrentStatus").checked;
                    var isRemindCreateUser = document.getElementById("cbIsRemindCreateUser").checked;
                    var isRemindAssignUser = document.getElementById("cbIsRemindAssignUser").checked;
                    var isRemindStartUser = document.getElementById("cbIsRemindStartUser").checked;
                    var isRemindCommentUser = document.getElementById("cbIsRemindCommentUser").checked;
                    var displayIndex = 2;
                    displayIndex += Number($("#hdDisplayIndex").val());
                    var statusDesc = $("#tbStatusDesc").val();
                    if (statusDesc != '' && statusDesc.replace(' ', '') != '') {
                        $(this).dialog("close");
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
                                "currentCount": currentCount,
                                "count": count,
                                "displayIndex": displayIndex,
                                "userCode": "<%=CurrentUser.Code%>",
                                "userName": "<%=CurrentUser.Name%>"
                            },
                            function OnSucceeded(result, eventArgs) {
                                var control = "#<%=this.GV_List.ClientID%>_ctl" + (result.DisplayIndex < 10 ? "0" + result.DisplayIndex : "" + result.DisplayIndex) + "_";

                                if (result.IsCurrentStatus) {
                                    $(control + "lblFlag").parent().css("background-color", result.Color);
                                    $(control + "lblFlag").html(result.Flag);
                                }
                                $(control + "lblStatusDesc").html("<span style='color:#0000E5;'>" + result.CreateUser + "&#40;<span style='color:fuchsia;'>" + formatDate(result.CreateDate) + "</span>&#41;</span>&#58;&nbsp;" + result.Value + "<span style='color:#0000E5;'>&#40;<span style='color:fuchsia;'><b>" + result.CurrentCount + "</b></span>&#47;" + result.Count + "&#41;</span>");
                                initialize();
                                $("#lnkStatus" + result.TaskCode).unbind('click').removeAttr('onclick');
                                //ShowStatus(taskCode, displayIndex, currentStatusCount, statusCount, flag, color)
                                $("#lnkStatus" + result.TaskCode).bind('click',
                                    function () {
                                        ShowStatus(result.TaskCode, result.Subject, (result.DisplayIndex - 2), result.CurrentCount, result.Count, result.Flag, result.Color);
                                    });
                            },
                            function OnFailed(error) {
                                alert(error.get_message());
                            }
                        );
                        }
                },
                "${Common.Button.Cancel}": function () {
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
                                //ShowStatus(taskCode, displayIndex, currentStatusCount, statusCount, flag, color)
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
                "${Common.Button.Cancel}": function () {
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

        //进展
        var now = new Date();
        $("#tbEndDate").val(now.format("yyyy-MM-dd"));
        now.setTime(now.getTime() - 7 * 24 * 60 * 60 * 1000);
        $("#tbStartDate").val(now.format("yyyy-MM-dd"));
        $("#tbStatusDesc").val('');
        $("#cbIsCurrentStatus").attr("checked", true);
        $("#cbIsRemindCreateUser").attr("checked", true);
        $("#cbIsRemindAssignUser").attr("checked", true);
        $("#cbIsRemindStartUser").attr("checked", true);
        $("#cbIsRemindCommentUser").attr("checked", true);
        //$("#<%=this.ddlFlag.ClientID%>").val("DI3");
        //$("#<%=this.ddlColor.ClientID%> ").val("green");

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

        $("#ui-id-1").text(taskCode + subject);
        $("#hdTaskCode").val(taskCode);
        $("#hdSubject").val(subject);
        $("#hdDisplayIndex").val(displayIndex);
        $("#commentDiv").dialog("open");
        $("#hdCurrentCount").val(currentCommentCount);
        $("#hdCount").val(commentCount);

        displayIndex += Number('2');
        var jquery_divID = "#<%=this.GV_List.ClientID%>_ctl" + (displayIndex < 10 ? "0" + displayIndex : "" + displayIndex) + "_lblComment";  // 鼠标点击的对象
        $("html,body").animate({ scrollTop: $(jquery_divID).offset().top - 280 }, 500);

    }

    function ShowStatus(taskCode, subject, displayIndex, currentStatusCount, statusCount, flag, color) {
        //alert(taskCode + '  ' + displayIndex + '  ' + currentStatusCount + '  ' + statusCount + '  ' + flag + '  ' + color);
        $("#ui-id-2").text(taskCode + subject);
        $("#hdTaskCode").val(taskCode);
        $("#hdSubject").val(subject);
        $("#hdDisplayIndex").val(displayIndex);
        $("#hdCurrentCount").val(currentStatusCount);
        $("#hdCount").val(statusCount);
        $("#<%=this.ddlFlag.ClientID%>").find("option[value='" + flag + "']").attr("selected", true);
        $("#<%=this.ddlColor.ClientID%>").find("option[value='" + color + "']").attr("selected", true);
        $("#statusDiv").dialog("open");
        displayIndex += Number('2');
        var jquery_divID = "#<%=this.GV_List.ClientID%>_ctl" + (displayIndex < 10 ? "0" + displayIndex : "" + displayIndex) + "_lblStatusDesc";  // 鼠标点击的对象
        //alert(displayIndex + ' ==  ' + jquery_divID);
        $("html,body").animate({ scrollTop: $(jquery_divID).offset().top - 280 }, 500);
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
        var jquery_divID = "#<%=this.GV_List.ClientID%>_ctl" + (displayIndex < 10 ? "0" + displayIndex : "" + displayIndex) + "_lblStatusDesc";  // 鼠标点击的对象
        //alert(displayIndex + ' ==  ' + jquery_divID);
        $("html,body").animate({ scrollTop: $(jquery_divID).offset().top - 280 }, 500);

    }

</script>
<fieldset>
    <table class="mtable">
        <tr>
            <td align="right">
                <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.Task.TaskSubType}:" />
            </td>
            <td>
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                    Width="260" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblSubmitUser" runat="server" Text="${ISI.Task.SubmitUser}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbSubmitUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblAssignUser" runat="server" Text="${ISI.Task.AssignUser}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbAssignUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblStartUser" runat="server" Text="${ISI.Task.StartUser}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbStartUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="t02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="query"
                        OnClick="btnSearch_Click" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                        OnClick="btnExport_Click" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="back" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fs" runat="server">
    <legend id="lgd" runat="server"></legend>
    <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound" OnDataBound="GV_List_DataBound"
        DefaultSortDirection="Descending" AutoGenerateColumns="false" DefaultSortExpression="CreateDate">
        <Columns>
            <asp:TemplateField HeaderText="${ISI.Status.Subject}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <div>
                        <asp:Label ID="lblCode" Width="90" runat="server" Text='<%# Eval("TaskCode")%>' />
                    </div>
                    <div>
                        <asp:Label ID="lblSubject" Width="90" runat="server" Text='' />
                    </div>
                    <div>
                        <asp:Label ID="lblCreateDate" runat="server" Text='<%#Eval("CreateDate","{0:yyyy-MM-dd HH}")%>' />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.Business.Description}" HeaderStyle-Width="22%" ItemStyle-Width="22%" HeaderStyle-Wrap="false" SortExpression="RefTaskCount">
                <ItemTemplate>
                    <div>
                        <asp:Label ID="lblDesc" runat="server" Text='' />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.ExpectedResults}" HeaderStyle-Wrap="false" SortExpression="AttachmentCount">
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
                    <div>
                        <asp:Label ID="lblEndDate" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.Status.SubmitUser}" HeaderStyle-Wrap="false" SortExpression="SubmitDate" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <div>
                        <asp:Label ID="lblSubmitUserNm" runat="server" Text='<%# Eval("SubmitUserNm")%>' />
                    </div>
                    <div>
                        <asp:Label ID="lblTaskAddress" runat="server" Text='<%#Eval("TaskAddress")%>' />
                    </div>
                    <div>
                        <asp:Label ID="lblTaskSubTypeCode" runat="server" Text='' />
                    </div>
                    <div><span style="color: #0000E5;"><%#Eval("Seq")%></span></div>
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
                    <div id="statusDiv" runat="server" visible="false"></div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.CodeMaster.Flag}" HeaderStyle-Wrap="false" SortExpression="TColor" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblFlag" runat="server" Text='<%# Bind("Flag") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.Status.Comment}" HeaderStyle-Wrap="false" SortExpression="CommentCreateDate" HeaderStyle-Width="15%" ItemStyle-Width="15%">
                <ItemTemplate>
                    <asp:Label ID="lblComment" runat="server" Text='' />
                    <div>
                        <a id="<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>" name="<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>" />
                        <a onclick="javascript:ShowComment('<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>','<%# DataBinder.Eval(Container.DataItem, "Subject")%>',<%# Container.DisplayIndex %>,<%# DataBinder.Eval(Container.DataItem, "CurrentCommentCount1")%>,<%# DataBinder.Eval(Container.DataItem, "CommentCount1")%>);" id="lnkComment<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>" name="lnkComment<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>" href="#">${ISI.Status.Comment}</a>

                    </div>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
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
            <tr>
                <td>
                    <label for="tbStartDate">${Common.Business.StartTime}:</label>
                    <input type="text" cssclass="inputRequired" id="tbStartDate" name="tbStartDate"
                        onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd' })" value="" style="width: 120px;" />
                </td>
                <td>
                    <label for="tbEndDate">${Common.Business.EndTime}:</label>
                    <input type="text" cssclass="inputRequired" id="tbEndDate" name="tbEndDate"
                        onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd' })" value="" style="width: 120px;" />
                </td>
                <td>
                    <cc1:CodeMstrDropDownList ID="ddlFlag" Code="ISIFlag" runat="server" IncludeBlankOption="false">
                    </cc1:CodeMstrDropDownList>
                    <cc1:CodeMstrDropDownList ID="ddlColor" Code="ISIColor" runat="server" IncludeBlankOption="false">
                    </cc1:CodeMstrDropDownList></td>
            </tr>
            <tr>
                <td colspan="3" align="center">

                    <span>
                        <input type="checkbox" id="cbIsRemindCreateUser" name="cbIsRemindCreateUser" checked="checked" />
                        <label for="cbIsRemindCreateUser">${ISI.TaskStatus.IsRemindCreateUser}</label>
                    </span>
                    <span>
                        <input type="checkbox" id="cbIsRemindAssignUser" name="cbIsRemindAssignUser" checked="checked" />
                        <label for="cbIsRemindAssignUser">${ISI.TaskStatus.IsRemindAssignUser}</label>
                    </span>
                    <span>
                        <input type="checkbox" id="cbIsRemindStartUser" name="cbIsRemindStartUser" checked="checked" />
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
</fieldset>
