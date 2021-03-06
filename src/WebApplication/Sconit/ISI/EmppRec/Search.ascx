﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_EmppRec_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblTaskCode" runat="server" Text="${ISI.EmppRec.Task}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbTaskCode" runat="server"></asp:TextBox>
            </td>
            <td class="td01">
                <asp:Literal ID="ltlContent" runat="server" Text="${ISI.EmppRec.Content}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbContent" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblSrcID" runat="server" Text="${ISI.EmppRec.SrcID}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbSrcID" runat="server"></asp:TextBox>
            </td>
            <td class="td01">
                <asp:Literal ID="lblMsgID" runat="server" Text="${ISI.EmppRec.MsgID}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbMsgID" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${ISI.EmppRec.CreateDateFrom}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${ISI.EmppRec.CreateDateTo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
            </td>
            <td>
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
