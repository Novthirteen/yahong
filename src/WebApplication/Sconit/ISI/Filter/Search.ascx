<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_Filter_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>

<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblTaskCode" runat="server" Text="${ISI.Filter.TaskCode}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbTaskCode" runat="server"></asp:TextBox>
            </td>
            <td class="td01">
                <asp:Literal ID="ltlDesc" runat="server" Text="${Common.Business.Description}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbDesc" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblTaskType" runat="server" Text="${ISI.Filter.TaskType}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlTaskType" Code="ISIType" runat="server" IncludeBlankOption="true"
                    Width="120px" DefaultSelectedValue="" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.Filter.TaskSubType}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Desc"
                    MustMatch="true" ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetCacheAllTaskSubType"
                    Width="200" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblUserCode" runat="server" Text="${ISI.Filter.UserCode}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbUserCode" runat="server" Visible="true" DescField="LongName" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEmail" runat="server" Text="${ISI.Filter.Email}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEmail" runat="server"></asp:TextBox>
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
