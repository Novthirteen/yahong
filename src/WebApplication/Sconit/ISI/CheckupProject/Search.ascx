﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_CheckupProject_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblCheckup" runat="server" Text="${ISI.CheckupProject.CheckupCode}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbCheckup" runat="server"></asp:TextBox>
            </td>
            <td class="td01">
                <asp:Literal ID="ltlDesc" runat="server" Text="${ISI.CheckupProject.CheckupDesc}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbDesc" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlType" runat="server" Text="${Common.Business.Type}:" />
            </td>
            <td class="td02">
                <cc1:codemstrdropdownlist id="ddlType" code="ISICheckupProjectType" IncludeBlankOption="true" DefaultSelectedValue="" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="lbIsActive" runat="server" Text="${Common.Business.IsActive}:" />
            </td>
            <td class="td02">
                <asp:CheckBox ID="cbIsActive" runat="server" Checked="true" />
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
