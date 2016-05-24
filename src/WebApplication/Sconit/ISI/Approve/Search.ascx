<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_Approve_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
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
            <td align="right">
                <asp:Literal ID="ltlDepartment" runat="server" Text="${ISI.Checkup.Department}:" />
            </td>
            <td>
                <ct:ASDropDownTreeView ID="astvMyTreeOrg" runat="server" BasePath="~/Js/astreeview/"
                    DataTableRootNodeValue="0" EnableRoot="false" EnableNodeSelection="false" EnableCheckbox="true"
                    EnableDragDrop="false" EnableTreeLines="false" EnableNodeIcon="false" EnableCustomizedNodeIcon="false"
                    EnableDebugMode="false" EnableRequiredValidator="false" InitialDropdownText=""
                    Width="170" EnableCloseOnOutsideClick="true" EnableHalfCheckedAsChecked="true"
                    DropdownIconDown="~/Js/astreeview/images/windropdown.gif" EnableContextMenuAdd="false"
                    MaxDropdownHeight="200" />
            </td>

            <td align="right">
                <asp:Literal ID="lblCheckupProject" runat="server" Text="${ISI.Checkup.CheckupProject}:" />
            </td>
            <td>
                <uc3:textbox ID="tbCheckupProject" runat="server" Visible="true" DescField="Desc"
                    MustMatch="true" ValueField="Code" ServicePath="CheckupProjectMgr.service" ServiceMethod="GetAllCheckupProject"
                    Width="200" />
            </td>

            <td align="right">
                <asp:Literal ID="lblType" runat="server" Text="${ISI.Checkup.CodeMaster.ISICheckupProjectType}:" />
            </td>
            <td>
                <asp:DropDownList ID="ddlType" runat="server" DataTextField="Description"
                    DataValueField="Value" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${ISI.Checkup.CheckupDateFrom}:" />
            </td>
            <td>
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td align="right">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${ISI.Checkup.CheckupDateTo}:" />
            </td>
            <td>
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>

            <td align="right">
                <asp:Literal ID="lblCreateUser" runat="server" Text="${Common.Business.CreateUser}:" />
            </td>
            <td>
                <uc3:textbox ID="tbCreateUser" runat="server" Visible="true" DescField="LongName" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
            <td align="right">
                <asp:Literal ID="lblCheckupUser" runat="server" Text="${ISI.Checkup.CheckupUser}:" />
            </td>
            <td>
                <uc3:textbox ID="tbCheckupUser" runat="server" Visible="true" DescField="LongName" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
        </tr>
        <tr>
            <td colspan="5"></td>
            <td colspan="3" align="right">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <cc1:Button ID="btnApprove" runat="server" Text="${ISI.Button.Approve}" OnClick="btnApprove_Click"
                        CssClass="add" FunctionId="ApproveCheckup" ValidationGroup="vgApprove" />
                    <cc1:Button ID="btnPublish" runat="server" Text="${ISI.Button.Publish}" OnClick="btnPublish_Click"
                        CssClass="add" FunctionId="PublishCheckup" />
                    <cc1:Button ID="btnClose" runat="server" Text="${Common.Button.Close}" OnClick="btnClose_Click"
                        CssClass="add" FunctionId="CloseCheckup" />
                    <cc1:Button ID="btnCloseRemind" runat="server" Text="${ISI.Button.CloseRemind}" OnClick="btnCloseRemind_Click"
                        CssClass="add" FunctionId="CloseRemindCheckup" />
                    <cc1:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnExport_Click"
                        CssClass="query" FunctionId="ExportCheckup" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
