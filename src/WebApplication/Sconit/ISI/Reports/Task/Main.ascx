<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_Reports_Task_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<fieldset>
    <table class="mtable">
        <tr>
            <td align="right">
                <asp:Literal ID="ltlDepartment" runat="server" Text="${ISI.Reports.Task.Dept2}|${ISI.Reports.Task.Dept}:" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlDept" Code="ISIDepartment" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
                <uc3:textbox ID="tbDept2" runat="server" Visible="true" DescField="Code" MustMatch="true"
                    ValueField="Name" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetAllTaskSubType"
                    Width="260" ServiceParameter="bool:true"/>
            </td>
            <td align="right">
                <asp:Literal ID="lblPosition" runat="server" Text="${ISI.Reports.Task.Position}:" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlPosition" Code="ISIPosition" runat="server" IncludeBlankOption="true" DefaultSelectedValue="" />
            </td>
            <td align="right">
                <asp:Literal ID="ltlType" runat="server" Text="${ISI.Reports.Task.Type}:" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlType" Code="ISIType" runat="server" IncludeBlankOption="true"
                    Width="120px" DefaultSelectedValue="" />
            </td>
            <td align="right">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td>
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                    Display="Dynamic" ControlToValidate="tbStartDate" ValidationGroup="vgSearch" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Literal ID="lblUser" runat="server" Text="${ISI.Reports.Task.User}:" />
            </td>
            <td>
                <uc3:textbox ID="tbUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>

            <td align="right">
                <asp:Literal ID="ltlOrg" runat="server" Text="${ISI.Reports.Task.Org}:" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlOrg" Code="ISIDepartment" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>
            <td align="right">
                <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.Reports.Task.TaskSubType}:" />
            </td>
            <td>
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                    Width="260" />
            </td>
            <td align="right">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td>
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                    Display="Dynamic" ControlToValidate="tbEndDate" ValidationGroup="vgSearch" />
            </td>
        </tr>
        <tr>
            <td colspan="5"></td>
            <td align="right" colspan="2">
                <asp:CheckBox runat="server" Text="${ISI.Reports.Task.IsFilter}" ID="ckIsFilter" Checked="true" />
                <asp:CheckBox runat="server" Text="${Common.IsActive}" ID="ckIsActive" Checked="true" />
            </td>
            <td class="t02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="button2"
                    OnClick="btnSearch_Click" ValidationGroup="vgSearch" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnExport_Click" ValidationGroup="vgSearch" />
            </td>
        </tr>
    </table>
</fieldset>
<fieldset runat="server" id="fld_Gv_List" visible="false">
    <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
        CellPadding="0" AllowSorting="true" OnSorting="GV_List_Sorting" OnDataBinding="GV_List_DataBinding">
        <Columns>
            <asp:TemplateField HeaderText="No.">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Name" SortExpression="Code" HeaderText="${ISI.Reports.Task.Name}" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="ProcessCount" SortExpression="ProcessCount" HeaderText="${ISI.Reports.Task.ProcessCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="ProcessStatusCount" SortExpression="ProcessStatusCount" HeaderText="${ISI.Reports.Task.ProcessStatusCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="NoStatusCount" SortExpression="NoStatusCount" HeaderText="${ISI.Reports.Task.NoStatusCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="ProcessCommentCount" SortExpression="ProcessCommentCount" HeaderText="${ISI.Reports.Task.ProcessCommentCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="MyProcessCount" SortExpression="MyProcessCount" HeaderText="${ISI.Reports.Task.MyProcessCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="MyStatusCount" SortExpression="MyStatusCount" HeaderText="${ISI.Reports.Task.MyStatusCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="MyCommentCount" SortExpression="MyCommentCount" HeaderText="${ISI.Reports.Task.MyCommentCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="NoMyProcessCount" SortExpression="NoMyProcessCount" HeaderText="${ISI.Reports.Task.NoMyProcessCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="NoMyStatusCount" SortExpression="NoMyStatusCount" HeaderText="${ISI.Reports.Task.NoMyStatusCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="NoMyCommentCount" SortExpression="NoMyCommentCount" HeaderText="${ISI.Reports.Task.NoMyCommentCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="MyHisProcessCount" SortExpression="MyHisProcessCount" HeaderText="${ISI.Reports.Task.MyHisProcessCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="MyHisStatusCount" SortExpression="MyHisStatusCount" HeaderText="${ISI.Reports.Task.MyHisStatusCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="MyHisCommentCount" SortExpression="MyHisCommentCount" HeaderText="${ISI.Reports.Task.MyHisCommentCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="NoMyHisProcessCount" SortExpression="NoMyHisProcessCount" HeaderText="${ISI.Reports.Task.NoMyHisProcessCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="NoMyHisStatusCount" SortExpression="NoMyHisStatusCount" HeaderText="${ISI.Reports.Task.NoMyHisStatusCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="NoMyHisCommentCount" SortExpression="NoMyHisCommentCount" HeaderText="${ISI.Reports.Task.NoMyHisCommentCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="StatusCount" SortExpression="StatusCount" HeaderText="${ISI.Reports.Task.StatusCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="SubmitCount" SortExpression="SubmitCount" HeaderText="${ISI.Reports.Task.SubmitCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="AssignCount" SortExpression="AssignCount" HeaderText="${ISI.Reports.Task.AssignCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="AttachmentCount" SortExpression="AttachmentCount" HeaderText="${ISI.Reports.Task.AttachmentCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="CancelCount" SortExpression="CancelCount" HeaderText="${ISI.Reports.Task.CancelCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="CloseCount" SortExpression="CloseCount" HeaderText="${ISI.Reports.Task.CloseCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="OpenCount" SortExpression="OpenCount" HeaderText="${ISI.Reports.Task.OpenCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="AssignTaskCount" SortExpression="AssignTaskCount" HeaderText="${ISI.Reports.Task.AssignTaskCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="NewAssignTaskCount" SortExpression="NewAssignTaskCount" HeaderText="${ISI.Reports.Task.NewAssignTaskCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="NewAssignStatusCount" SortExpression="NewAssignStatusCount" HeaderText="${ISI.Reports.Task.NewAssignStatusCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="NewAssignCommentCount" SortExpression="NewAssignCommentCount" HeaderText="${ISI.Reports.Task.NewAssignCommentCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="OldAssignTaskCount" SortExpression="OldAssignTaskCount" HeaderText="${ISI.Reports.Task.OldAssignTaskCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="OldAssignStatusCount" SortExpression="OldAssignStatusCount" HeaderText="${ISI.Reports.Task.OldAssignStatusCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="OldAssignCommentCount" SortExpression="OldAssignCommentCount" HeaderText="${ISI.Reports.Task.OldAssignCommentCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="AssignSubmitCount" SortExpression="AssignSubmitCount" HeaderText="${ISI.Reports.Task.AssignSubmitCount}" Visible="false" DataFormatString="{0:#}" />
            <asp:BoundField DataField="AssignAssignCount" SortExpression="AssignAssignCount" HeaderText="${ISI.Reports.Task.AssignAssignCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="AssignAttachmentCount" SortExpression="AssignAttachmentCount" HeaderText="${ISI.Reports.Task.AssignAttachmentCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="AssignCancelCount" SortExpression="AssignCancelCount" HeaderText="${ISI.Reports.Task.AssignCancelCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="AssignCloseCount" SortExpression="AssignCloseCount" HeaderText="${ISI.Reports.Task.AssignCloseCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="AssignOpenCount" SortExpression="AssignOpenCount" HeaderText="${ISI.Reports.Task.AssignOpenCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="CommentCount" SortExpression="CommentCount" HeaderText="${ISI.Reports.Task.CommentCount}" DataFormatString="{0:#}" />
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>
