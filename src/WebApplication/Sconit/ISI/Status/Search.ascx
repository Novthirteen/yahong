<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_Status_Search" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc2" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td align="right">
                <asp:CheckBox runat="server" ToolTip="${ISI.Status.Exclude}" Text="${ISI.Task.Type}:" ID="cbExcludeType" Checked="false" />
            </td>
            <td>
                <cc2:CodeMstrDropDownList ID="ddlType" Code="ISIType" runat="server" IncludeBlankOption="true"
                    Width="120px" DefaultSelectedValue="" />
            </td>
            <td align="right">
                <asp:CheckBox runat="server" ToolTip="${ISI.Status.Exclude}" Text="${ISI.Task.TaskSubType}:" ID="cbExclude" Checked="false" />
            </td>
            <td>
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                    Width="260" />

            </td>
            <td align="right">
                <asp:Literal ID="lblFlag" runat="server" Text="${Common.CodeMaster.Flag}&#124;${Common.CodeMaster.Color}&#124;${ISI.Status.Priority}:" />
            </td>
            <td>
                <cc2:CodeMstrDropDownList ID="ddlFlag" Code="ISIFlag" runat="server" IncludeBlankOption="true" DefaultSelectedValue="">
                </cc2:CodeMstrDropDownList>
                <cc2:CodeMstrDropDownList ID="ddlColor" Code="ISIColor" runat="server" IncludeBlankOption="true" DefaultSelectedValue="">
                </cc2:CodeMstrDropDownList>
                <cc2:CodeMstrDropDownList ID="ddlPriority" Code="ISIPriority" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>
            <td align="right">
                <asp:CheckBox runat="server" ToolTip="${ISI.Status.Exclude}" ID="cbPhase" Checked="false" />
            </td>
            <td>
                <cc2:CodeMstrDropDownList ID="ddlPhase" Code="ISIPhase" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" Visible="false" />
                <asp:TextBox ID="tbSeq" runat="server" Visible="false" Width="80"></asp:TextBox>
                <ct:ASDropDownTreeView ID="astvMyTreeOrg" runat="server" BasePath="~/Js/astreeview/"
                    DataTableRootNodeValue="0" EnableRoot="false" EnableNodeSelection="false" EnableCheckbox="true"
                    EnableDragDrop="false" EnableTreeLines="false" EnableNodeIcon="false" EnableCustomizedNodeIcon="false"
                    EnableDebugMode="false" EnableRequiredValidator="false" InitialDropdownText=""
                    Width="170" EnableCloseOnOutsideClick="true" EnableHalfCheckedAsChecked="true"
                    DropdownIconDown="~/Js/astreeview/images/windropdown.gif" EnableContextMenuAdd="false"
                    MaxDropdownHeight="200" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:CheckBox runat="server" Text="${ISI.Status.Highlight}${ISI.Status.Search}:" ID="cbHighlight" Checked="true" />
            </td>
            <td>
                <asp:TextBox ID="tbDesc" runat="server" />
            </td>
            <td align="right">
                <asp:CheckBox runat="server" Text="${Common.CodeMaster.Status}:" ToolTip="${ISI.Status.Exclude}" ID="cbStatus" Checked="false" />
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
                <asp:CheckBox runat="server" Text="${Common.Business.StartDate}" ID="cbIsStatus" ToolTip="${ISI.Status.IsStatus}" />
            </td>
            <td>
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td align="right">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td>
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:CheckBox runat="server" ToolTip="${ISI.Status.Exclude}" Text="${Common.Business.CreateUser}:" ID="cbExcludeSubmitUser" Checked="false" />
            </td>
            <td>
                <uc3:textbox ID="tbCreateUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
            <td align="right">
                <asp:CheckBox runat="server" ToolTip="${ISI.Status.Exclude}" Text="${ISI.Task.AssignUser}:" ID="cbExcludeAssignUser" Checked="false" />
            </td>
            <td>
                <uc3:textbox ID="tbAssignUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
            <td align="right">
                <asp:CheckBox runat="server" Text="${ISI.Status.FirstStartUser}" ID="ckFirst" Checked="true" />
            </td>
            <td>
                <uc3:textbox ID="tbStartUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
                <asp:CheckBox runat="server" Text="${ISI.Status.ApproveUser}" ID="ckApprove" Checked="false" />
            </td>
            <td align="right">
                <asp:Literal ID="lblCommentUser" runat="server" Text="${ISI.Status.CommentUser}:" />
            </td>
            <td>
                <uc3:textbox ID="tbCommentUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Literal ID="lblOrderBy" runat="server" Text="${ISI.Status.OrderBy}:" Visible="false" />
            </td>
            <td colspan="3">
                <asp:RadioButtonList ID="rabOrderBy" runat="server"
                    RepeatDirection="Horizontal" Visible="false">
                    <asp:ListItem Text="${ISI.Status.IsStartDate}" Value="PlanStartDate"></asp:ListItem>
                    <asp:ListItem Text="${ISI.Status.StatusDate}" Value="StatusDate"></asp:ListItem>
                    <asp:ListItem Text="${ISI.Status.TaskSubTypeCodePhaseSeq}" Value="TaskSubTypeCodePhaseSeq" Selected="True"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td align="right" colspan="3">
                <asp:CheckBox runat="server" Text="${ISI.Status.IsWF}" ID="ckIsWF" />
                <asp:CheckBox runat="server" Text="${ISI.Status.IsTrace}" ID="ckIsTrace" Visible="false" />
                <asp:CheckBox runat="server" Text="${ISI.Status.IsUltimate}" ID="ckUltimate" />
                <asp:CheckBox runat="server" Text="${ISI.Status.Focus}" ID="ckIsFocus" />
                <asp:CheckBox runat="server" Text="${ISI.Status.Overdue}" ID="ckIsOverdue" />
                <asp:CheckBox runat="server" Text="${ISI.Status.Overdue2}" ID="ckIsOverdue2" />
                <asp:CheckBox runat="server" Text="${ISI.Status.Mine}" ID="ckIsMine" />
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="button2"
                    OnClick="btnSearch_Click" />
                <cc2:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
                    CssClass="add" FunctionId="" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnExport_Click" />
            </td>
        </tr>
    </table>
</fieldset>
