<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_TSK_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td align="right">
                <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.TSK.TaskSubType}:" />
            </td>
            <td>
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                    Width="260" />
            </td>
            <td align="right">
                <asp:Literal ID="lblCode" runat="server" Text="${ISI.TSK.Code}:" />
            </td>
            <td>
                <asp:TextBox ID="tbCode" runat="server"></asp:TextBox>
            </td>

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
                <asp:Literal ID="lblBackYards" runat="server" Text="${ISI.TSK.BackYards}:" />
            </td>
            <td>
                <asp:TextBox ID="tbBackYards" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Literal ID="lblTaskAddress" runat="server" Text="${ISI.TSK.TaskAddress}:" />
            </td>
            <td>
                <uc3:textbox ID="tbTaskAddress" runat="server" Visible="true" DescField="Code" MustMatch="false"
                    ValueField="Name" ServicePath="TaskAddressMgr.service" ServiceMethod="GetCacheAllTaskAddress"
                    Width="200" />
            </td>
            <td align="right">
                <asp:Literal ID="lblPriority" runat="server" Text="${ISI.TSK.Priority}:" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlPriority" Code="ISIPriority" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>

            <td align="right">
                <asp:Literal ID="lblSubject" runat="server" Text="${ISI.TSK.Subject}:" />
            </td>
            <td>
                <asp:TextBox ID="tbSubject" runat="server" />
            </td>
            <td align="right">
                <asp:Literal ID="lblDesc1" runat="server" Text="${ISI.TSK.Desc1}:" />
            </td>
            <td>
                <asp:TextBox ID="tbDesc1" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>

            <td align="right">
                <asp:Literal ID="lblEmail" runat="server" Text="${ISI.TSK.Email}:" />
            </td>
            <td>
                <asp:TextBox ID="tbEmail" runat="server" />
            </td>
            <td align="right">
                <asp:Literal ID="lblMobilePhone" runat="server" Text="${ISI.TSK.MobilePhone}:" />
            </td>
            <td >
                <asp:TextBox ID="tbMobilePhone" runat="server"></asp:TextBox>
            </td>
            <td align="right">
                <asp:Literal ID="lblSeq" runat="server" Text="${ISI.TSK.Phase}|${ISI.TSK.Seq}:" Visible="false" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlPhase" Code="ISIPhase" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" Visible="false" />
                <asp:TextBox ID="tbSeq" runat="server" Width="80" Visible="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="7"></td>
            <td>
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <cc1:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
                        CssClass="add" />
                    <cc1:Button ID="btnBatch" runat="server" Text="${ISI.TSK.Button.Batch}" OnClick="btnBatch_Click"
                        CssClass="query" Visible="false" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
