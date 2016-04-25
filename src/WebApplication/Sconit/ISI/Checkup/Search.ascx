<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_Checkup_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
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
            <td class="td01">
                <asp:Literal ID="ltlDepartment" runat="server" Text="${ISI.Checkup.Department}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlDepartment" Code="ISIDepartment" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblCheckupProject" runat="server" Text="${ISI.Checkup.CheckupProject}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbCheckupProject" runat="server" Visible="true" DescField="Desc"
                    MustMatch="true" ValueField="Code" ServicePath="CheckupProjectMgr.service" ServiceMethod="GetAllCheckupProject"
                    Width="200" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblCheckupUser" runat="server" Text="${ISI.Checkup.CheckupUser}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbCheckupUser" runat="server" Visible="true" DescField="LongName" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${ISI.Checkup.CheckupDateFrom}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${ISI.Checkup.CheckupDateTo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblCreateUser" runat="server" Text="${Common.Business.CreateUser}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbCreateUser" runat="server" Visible="true" DescField="LongName" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblSummaryCode" runat="server" Text="${ISI.Checkup.SummaryCode}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbSummaryCode" runat="server"  />
            </td>
        </tr>
        <tr>
            <td colspan="3"></td>
            <td>
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <cc1:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
                        CssClass="add" FunctionId="CreateCheckup" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
