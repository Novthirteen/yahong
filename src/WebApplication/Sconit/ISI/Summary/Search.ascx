<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_Summary_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblCode" runat="server" Text="${Common.Business.Code}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbCode" runat="server"></asp:TextBox>
            </td>
            <td class="td01">
                <asp:Literal ID="lblUser" runat="server" Text="${ISI.Evaluation.User}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbUser" runat="server" Visible="true" DescField="LongName" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblDate" runat="server" Text="${ISI.Summary.Date}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM'})" />
                <asp:CheckBox runat="server" ID="cbNoSubmit" Text="${ISI.Summary.NoSubmit}" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblStatus" runat="server" Text="${Common.CodeMaster.Status}:" />
            </td>
            <td class="td02">
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
