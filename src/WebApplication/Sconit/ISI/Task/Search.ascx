<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_Task_Search" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc2" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
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
            <td align="right">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td>
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox>
            </td>
            <td align="right">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td>
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="3" />
            <td class="t02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="button2"
                    OnClick="btnSearch_Click" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnExport_Click" Visible="false" />
            </td>
        </tr>
    </table>
</fieldset>
