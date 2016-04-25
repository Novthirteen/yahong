<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_WeekStatistics_Search" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<fieldset>
    <table class="mtable">
        <tr>
            <td align="right">
                <asp:Literal ID="ltlDepartment" runat="server" Text="${ISI.WeekStatistics.Dept}:" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlDept" Code="ISIDepartment" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>
            <td align="right">
                <asp:Literal ID="ltlDept2" runat="server" Text="${ISI.WeekStatistics.Dept2}:" />
            </td>
            <td>
                <uc3:textbox ID="tbDept2" runat="server" Visible="true" DescField="Code" MustMatch="true"
                    ValueField="Name" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetAllTaskSubType"
                    Width="260" ServiceParameter="bool:true"/>
            </td>
            <td align="right">
                <asp:Literal ID="lblPosition" runat="server" Text="${ISI.WeekStatistics.Position}:" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlPosition" Code="ISIPosition" runat="server" IncludeBlankOption="true" DefaultSelectedValue="" />
            </td>
            <td align="right">
                <asp:Literal ID="lblJobNo" runat="server" Text="${ISI.WeekStatistics.JobNo}:" />
            </td>
            <td>
                <asp:TextBox ID="tbJobNo" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Literal ID="lblThisWeek" runat="server" Text="${ISI.WeekStatistics.ThisWeek}:" />
            </td>
            <td>
                <span style="color: blue;">
                    <asp:Literal ID="lblStartEndDate" runat="server" Text="" />
                </span>
            </td>
            <td align="right">
                <asp:Literal ID="lblUser" runat="server" Text="${ISI.WeekStatistics.User}:" />
            </td>
            <td>
                <uc3:textbox ID="tbUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
        </tr>
        <tr>
            <td colspan="6"></td>
            <td align="right">
                <asp:CheckBox runat="server" Text="${Common.IsActive}" ID="ckIsActive" Checked="true" /></td>
            </td>
            <td class="t02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="button2"
                    OnClick="btnSearch_Click" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnExport_Click" />
            </td>
        </tr>
    </table>
</fieldset>
