<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_ProjectTask_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblProjectType" runat="server" Text="${ISI.ProjectTask.ProjectType}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlProjectType" Code="PSIType" runat="server" IncludeBlankOption="true" DefaultSelectedValue=""/>
            </td>
            <td class="td01">
                <asp:Literal ID="lblPhase" runat="server" Text="${ISI.ProjectTask.Phase}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlPhase" Code="ISIPhase" runat="server" IncludeBlankOption="true" DefaultSelectedValue=""/>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblProjectSubType" runat="server" Text="${ISI.ProjectTask.ProjectSubType}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlProjectSubType" Code="ISIProjectSubType" runat="server" IncludeBlankOption="true" DefaultSelectedValue="" />
            </td>

            <td class="td01">
                <asp:Literal ID="lblSeq" runat="server" Text="${ISI.ProjectTask.Seq}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbSeq" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblSubject" runat="server" Text="${ISI.ProjectTask.Subject}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbSubject" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlDesc" runat="server" Text="${ISI.ProjectTask.Desc}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbDesc" runat="server"></asp:TextBox>
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
