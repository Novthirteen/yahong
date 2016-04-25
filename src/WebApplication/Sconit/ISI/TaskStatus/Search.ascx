<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_TaskStatus_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <div id="isReport" runat="server">
            <tr>
                <td align="right">
                    <asp:Literal ID="ltlType" runat="server" Text="${ISI.TaskStatus.Type}:" />
                </td>
                <td>
                    <cc1:CodeMstrDropDownList ID="ddlType" Code="ISIType" runat="server" IncludeBlankOption="true"
                        Width="120px" DefaultSelectedValue="" />
                </td>
                <td align="right">
                    <asp:Literal ID="ltlIssueType" runat="server" Text="${ISI.TaskStatus.TaskSubType}:" />
                </td>
                <td>
                    <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                        ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                        Width="260" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblTaskCode" runat="server" Text="${ISI.TaskStatus.TaskCode}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbTaskCode" runat="server"></asp:TextBox>
                </td>
            </tr>
        </div>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblFlag" runat="server" Text="${ISI.TaskStatus.Flag}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlFlag" Code="ISIFlag" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblColor" runat="server" Text="${ISI.TaskStatus.Color}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlColor" Code="ISIColor" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlDesc" runat="server" Text="${ISI.TaskStatus.Desc}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbDesc" runat="server"></asp:TextBox>
            </td>
            <td class="td01">
                <asp:Literal ID="lblUser" runat="server" Text="${Common.Business.CreateUser}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbUser" runat="server" Visible="true" DescField="Name" MustMatch="false"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td colspan="3"></td>
            <td>
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <asp:Button ID="btnNew" runat="server" Text="${Common.Button.New}" CssClass="add"
                        OnClick="btnNew_Click" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="back" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
