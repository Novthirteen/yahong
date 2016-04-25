<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_Scheduling_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr id="isSpecial" runat="server">
            <td class="td01">
                <asp:Literal ID="lblDayOfWeek" runat="server" Text="${ISI.Scheduling.Week}:" />
            </td>
            <td class="td02">
                <asp:DropDownList ID="ddlDayOfWeek" runat="server">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="${Common.Week.Monday}" Value="Monday"></asp:ListItem>
                    <asp:ListItem Text="${Common.Week.Tuesday}" Value="Tuesday"></asp:ListItem>
                    <asp:ListItem Text="${Common.Week.Wednesday}" Value="Wednesday"></asp:ListItem>
                    <asp:ListItem Text="${Common.Week.Thursday}" Value="Thursday"></asp:ListItem>
                    <asp:ListItem Text="${Common.Week.Friday}" Value="Friday"></asp:ListItem>
                    <asp:ListItem Text="${Common.Week.Saturday}" Value="Saturday"></asp:ListItem>
                    <asp:ListItem Text="${Common.Week.Sunday}" Value="Sunday"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="td01">
                <asp:Literal ID="lblShift" runat="server" Text="${ISI.Scheduling.Shift}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbShift" runat="server" Visible="true" DescField="ShiftName" MustMatch="true"
                    ValueField="Code" ServicePath="SchedulingMgr.service" ServiceMethod="GetShift"
                    ServiceParameter="string:#ddlDayOfWeek" Width="250" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.Scheduling.TaskSubType}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetCacheAllTaskSubType"
                    Width="260" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblStartUser" runat="server" Text="${ISI.TaskSubType.User}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbStartUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
            </td>
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
