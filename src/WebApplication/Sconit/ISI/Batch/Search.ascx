<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_Batch_Search" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc2" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td align="right">
                <asp:Literal ID="ltlType" runat="server" Text="${ISI.Batch.Type}:" />
            </td>
            <td>
                <cc2:CodeMstrDropDownList ID="ddlType" Code="ISIType" runat="server" IncludeBlankOption="true"
                    Width="120px" DefaultSelectedValue="" />
            </td>
            <td align="right">
                <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.Batch.TaskSubType}:" />
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
                <asp:Literal ID="lblPhase" runat="server" Text="" />
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
                <asp:Literal ID="lblAssignUser" runat="server" Text="${ISI.Batch.AssignUser}:" />
            </td>
            <td>
                <uc3:textbox ID="tbAssignUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
            <td align="right">
                <asp:CheckBox runat="server" Text="${ISI.Status.FirstStartUser}" ID="ckFirst" Checked="true" />
            </td>
            <td>
                <uc3:textbox ID="tbStartUser" runat="server" Visible="true" DescField="Name" MustMatch="true" CssClass="inputRequired"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
                <asp:RequiredFieldValidator ID="rfvStartUser" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                    Display="Dynamic" ControlToValidate="tbStartUser" ValidationGroup="vgSearch" />
                <asp:RequiredFieldValidator ID="rfvStartUser1" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                    Display="Dynamic" ControlToValidate="tbStartUser" ValidationGroup="vgUpdate" />
            </td>
            <td align="right">
                <asp:Literal ID="lblUser" runat="server" Text="${ISI.Batch.User}:" />
            </td>
            <td>
                <div class="buttons">
                    <uc3:textbox ID="tbUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                        ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
                    <asp:RequiredFieldValidator ID="rfvUser" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbUser" ValidationGroup="vgUpdate" />
                </div>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Literal ID="lblType" runat="server" Text="${ISI.Batch.OPType}:" />
            </td>
            <td colspan="6">
                <asp:RadioButtonList runat="server" ID="rblCreate" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Text="${Common.Button.Nothing}" Value="" Selected="True" />
                    <asp:ListItem Text="${Common.Button.Delete}" Value="IsDelete" />
                    <asp:ListItem Text="${Common.Button.Submit}" Value="IsSubmit" />
                </asp:RadioButtonList>
                <asp:CheckBox runat="server" ID="cbIsCancel" Text="${Common.Button.Cancel}" />
                <asp:CheckBox runat="server" ID="cbIsComplete" Text="${Common.Button.Complete}" />
                <asp:RadioButtonList runat="server" ID="rblComplete" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Text="${Common.Button.Nothing}" Value="" Selected="True" />
                    <asp:ListItem Text="${Common.Button.Reject}" Value="IsReject" />
                    <asp:ListItem Text="${Common.Button.Close}" Value="IsClose" />
                </asp:RadioButtonList>
                <asp:CheckBox runat="server" ID="cbIsOpen" Text="${Common.Button.Open}" />

            </td>
            <td>
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" ValidationGroup="vgSearch" />
                    <asp:Button ID="btnReplace" runat="server" Text="${ISI.Batch.Button.Replace}" OnClick="btnReplace_Click"
                        CssClass="query" ValidationGroup="vgSearch" OnClientClick="return confirm('${Common.Button.Replace.Confirm}')" />
                    <asp:Button ID="btnBatch" runat="server" Text="${ISI.Batch.Button.Batch}" OnClick="btnBatch_Click"
                        CssClass="button2" OnClientClick="return confirm('${Common.Button.Batch.Confirm}')" ValidationGroup="vgSearch"  />
                    <asp:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click" Text="${Common.Button.Delete}"
                        CssClass="delete" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" Visible="false" />
                    <asp:Button ID="btnCancel" runat="server" Text="${Common.Button.Cancel}" CssClass="button2"
                        OnClick="btnCancel_Click" OnClientClick="return confirm('${Common.Button.Cancel.Confirm}')" Visible="false" />
                    <asp:Button ID="btnComplete" runat="server" Text="${Common.Button.Complete}" CssClass="button2"
                        OnClick="btnComplete_Click" OnClientClick="return confirm('${Common.Button.Complete.Confirm}')" Visible="false" />
                    <asp:Button ID="btnReject" runat="server" Text="${ISI.TSK.Button.Reject}" CssClass="button2"
                        OnClick="btnReject_Click" OnClientClick="return confirm('${ISI.TSK.Button.Reject.Confirm}')" Visible="false" />
                    <asp:Button ID="btnClose" runat="server" Text="${Common.Button.Close}" CssClass="button2"
                        OnClick="btnClose_Click" OnClientClick="return confirm('${Common.Button.Close.Confirm}')" Visible="false" />
                    <asp:Button ID="btnOpen" runat="server" Text="${ISI.TSK.Button.Open}" CssClass="button2"
                        OnClick="btnOpen_Click" OnClientClick="return confirm('${ISI.TSK.Button.Open.Confirm}')" Visible="false" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
