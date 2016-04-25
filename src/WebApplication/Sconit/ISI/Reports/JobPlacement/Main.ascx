<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_Reports_JobPlacement_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc2" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<link rel="stylesheet" type="text/css" href="Js/jquery.ganttView/jquery.ganttView.css" />
<script language="javascript" type="text/javascript" src="Js/jquery.ganttView/lib/date.js"></script>
<script language="javascript" type="text/javascript" src="Js/ui.core-min1.js"></script>
<script language="javascript" type="text/javascript" src="Js/jquery.ganttView/jquery.ganttView.js"></script>
<script language="javascript" type="text/javascript">
    function ShowChart() {
        $("#ganttChart").html('加载中...');
        //$("#eventMessage").html('');
        var status = $('#<%=astvMyTree.ClientID%>_txtCheckedValues').val();
        var type = $('#<%=ddlType.ClientID%>').val();
        var backYards = $('#<%=tbBackYards.ClientID%>').val();
        var submitUser = $('#<%=tbSubmitUser.ClientID%>_suggest').val();
        var assignUser = $('#<%=tbAssignUser.ClientID%>_suggest').val();
        var startUser = $('#<%=tbStartUser.ClientID%>_suggest').val();
        var taskSubType = $('#<%=tbTaskSubType.ClientID%>_suggest').val();
        var taskAddress = $('#<%=tbTaskAddress.ClientID%>_suggest').val();
        var startDate = $('#<%=tbStartDate.ClientID%>').val();
        var endDate = $('#<%=tbEndDate.ClientID%>').val();
        var count = $('#<%=tbCount.ClientID%>').val() ? $('#<%=tbCount.ClientID%>').val() : 50;
        Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'GetTask', false,
                    { "type": type, "status": status, "taskSubType": taskSubType, "taskAddress": taskAddress, "assignUser": assignUser, "startUser": startUser, "submitUser": submitUser, "backYards": backYards, "startDate": startDate, "endDate": endDate, "userCode": "<%=CurrentUser.Code%>", "count": count },
                    function OnSucceeded(result, eventArgs) {
                        $("#ganttChart").html('');
                        if (result) {
                            //self.location = 'MainPage/NoPermission.aspx';
                            //$('#fs').slideToggle();
                            $("#ganttChart").ganttView({
                                data: result,
                                slideWidth: '85%',
                                behavior: {
                                    clickable: false,
                                    draggable: false,
                                    resizable: false
                                }
                            });
                        } else {

                        }
                    },
                    function OnFailed(error) {
                        alert(error.get_message());
                    }
                   );
        // $("#ganttChart").ganttView("setSlideWidth", 600);
        }
</script>
<fieldset>
    <table class="mtable">
        <tr>
            <td align="right">
                <asp:Literal ID="ltlType" runat="server" Text="${ISI.Task.Type}:" />
            </td>
            <td>
                <cc2:CodeMstrDropDownList ID="ddlType" Code="ISIType" runat="server" IncludeBlankOption="true"
                    Width="120px" DefaultSelectedValue="" />
            </td>

            <td align="right">
                <asp:Literal ID="lblStatus" runat="server" Text="${Common.CodeMaster.Status}:" />
            </td>
            <td>
                <ct:ASDropDownTreeView ID="astvMyTree" runat="server" BasePath="~/Js/astreeview/"
                    DataTableRootNodeValue="0" EnableRoot="false" EnableNodeSelection="false" EnableCheckbox="true"
                    EnableDragDrop="false" EnableTreeLines="false" EnableNodeIcon="false" EnableCustomizedNodeIcon="false"
                    EnableDebugMode="false" EnableRequiredValidator="false" InitialDropdownText=""
                    Width="170" EnableCloseOnOutsideClick="true" EnableHalfCheckedAsChecked="true"
                    DropdownIconDown="~/Js/astreeview/images/windropdown.gif" EnableContextMenuAdd="false"
                    MaxDropdownHeight="200" />
            </td>
        </tr>

        <tr>
            <td align="right">
                <asp:Literal ID="ltlIssueType" runat="server" Text="${ISI.Task.TaskSubType}:" />
            </td>
            <td>
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                    Width="260" />
            </td>
            <td align="right">
                <asp:Literal ID="lblTaskAddress" runat="server" Text="${ISI.Task.TaskAddress}:" />
            </td>
            <td>
                <uc3:textbox ID="tbTaskAddress" runat="server" Visible="true" DescField="Code" MustMatch="false"
                    ValueField="Name" ServicePath="TaskAddressMgr.service" ServiceMethod="GetCacheAllTaskAddress"
                    Width="200" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblBackYards" runat="server" Text="${ISI.Task.BackYards}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbBackYards" runat="server" />
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
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td align="right"></td>
            <td class="td02"></td>
            <td align="right">
                <asp:Literal ID="lblCount" runat="server" Text="${ISI.JobPlacement.Count}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbCount" runat="server" Text="10" />
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="t02">
                <div class="buttons">
                    <button type="button" id="btnSearch" class="button2" name="btnSearch" onclick="ShowChart()">
                        ${Common.Button.Search}
                    </button>
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fs">
    <div id="ganttChart"></div>
</fieldset>
