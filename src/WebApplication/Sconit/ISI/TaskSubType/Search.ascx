<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_TaskSubType_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblCode" runat="server" Text="${ISI.TaskSubType.Code}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbCode" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubTypeList"
                    Width="250" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlDesc" runat="server" Text="${ISI.TaskSubType.Description}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbDesc" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblUser" runat="server" Text="${ISI.TaskSubType.User}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblOrg" runat="server" Text="${ISI.TaskSubType.Org}:" />
            </td>
            <td class="td02">
                <ct:ASDropDownTreeView ID="astvMyTreeOrg" runat="server" BasePath="~/Js/astreeview/"
                    DataTableRootNodeValue="0" EnableRoot="false" EnableNodeSelection="false" EnableCheckbox="true"
                    EnableDragDrop="false" EnableTreeLines="false" EnableNodeIcon="false" EnableCustomizedNodeIcon="false"
                    EnableDebugMode="false" EnableRequiredValidator="false" InitialDropdownText=""
                    Width="170" EnableCloseOnOutsideClick="true" EnableHalfCheckedAsChecked="true"
                    DropdownIconDown="~/Js/astreeview/images/windropdown.gif" EnableContextMenuAdd="false"
                    MaxDropdownHeight="200" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblProcessUser" runat="server" Text="${ISI.TaskSubType.ProcessUser}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbProcessUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblApply" runat="server" Text="${ISI.TaskSubType.Apply}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbApply" runat="server" Visible="true" DescField="Desc1" MustMatch="false"
                    ValueField="Code" ServicePath="ApplyMgr.service" ServiceMethod="GetAllApply" />
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02" colspan="3">
                <asp:CheckBox runat="server" ID="cbIsWF" Text="${ISI.TaskSubType.IsWF}" />
                <asp:CheckBox runat="server" ID="cbIsApply" Text="${ISI.TaskSubType.IsApply}" />
                <asp:CheckBox runat="server" ID="cbIsPrint" Text="${ISI.TaskSubType.IsPrint}" />
                <asp:CheckBox runat="server" ID="cbIsAssignUser" Text="${ISI.TaskSubType.IsAssignUser}" />
                <asp:CheckBox runat="server" ID="cbIsCtrl" Text="${ISI.TaskSubType.IsCtrl}" />
                <asp:CheckBox runat="server" ID="cbIsRemoveForm" Text="${ISI.TaskSubType.IsRemoveForm}" />
                <asp:CheckBox runat="server" ID="cbIsCostCenter" Text="${ISI.TaskSubType.IsCostCenter}" />
                <asp:CheckBox runat="server" ID="cbIsAttachment" Text="${ISI.TaskSubType.IsAttachment}" />
            </td>
        </tr>
        <tr>
            <td colspan="3"></td>
            <td>
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <asp:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
                        CssClass="add" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
