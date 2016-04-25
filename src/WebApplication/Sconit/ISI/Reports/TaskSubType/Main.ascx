<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_Reports_TaskSubType_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlDepartment" runat="server" Text="${ISI.Reports.TaskSubType.Org}|${ISI.Reports.TaskSubType.Type}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlDept" Code="ISIDepartment" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
                <cc1:CodeMstrDropDownList ID="ddlType" Code="ISIType" runat="server" IncludeBlankOption="true"
                    Width="120px" DefaultSelectedValue="" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.Reports.TaskSubType.TaskSubType}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                    Width="260" />
            </td>
            </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                    Display="Dynamic" ControlToValidate="tbStartDate" ValidationGroup="vgSearch" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                    Display="Dynamic" ControlToValidate="tbEndDate" ValidationGroup="vgSearch" />
            </td>
        </tr>
        <tr>
            <td colspan="2"></td>
            <td class="td01">
                <asp:CheckBox runat="server" Text="${Common.IsActive}" ID="ckIsActive" Checked="true" /></td>
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
            <asp:BoundField DataField="Code" SortExpression="Code" HeaderText="${ISI.Reports.TaskSubType.Code}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="Description" HeaderText="${ISI.Reports.TaskSubType.Description}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AssignUser" HeaderText="${ISI.Reports.TaskSubType.AssignUser}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="ProcessCount" SortExpression="ProcessCount" HeaderText="${ISI.Reports.TaskSubType.ProcessCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="StatusCount" SortExpression="StatusCount" HeaderText="${ISI.Reports.TaskSubType.StatusCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="CommentCount" SortExpression="CommentCount" HeaderText="${ISI.Reports.TaskSubType.CommentCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="SubmitCount" SortExpression="SubmitCount" HeaderText="${ISI.Reports.TaskSubType.SubmitCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="AssignCount" SortExpression="AssignCount" HeaderText="${ISI.Reports.TaskSubType.AssignCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="NoAssignCount" SortExpression="NoAssignCount" HeaderText="${ISI.Reports.TaskSubType.NoAssignCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="InProcessCount" SortExpression="InProcessCount" HeaderText="${ISI.Reports.TaskSubType.InProcessCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="OverStartCount" SortExpression="OverStartCount" HeaderText="${ISI.Reports.TaskSubType.OverStartCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="AttachmentCount" SortExpression="AttachmentCount" HeaderText="${ISI.Reports.TaskSubType.AttachmentCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="CancelCount" SortExpression="CancelCount" HeaderText="${ISI.Reports.TaskSubType.CancelCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="CloseCount" SortExpression="CloseCount" HeaderText="${ISI.Reports.TaskSubType.CloseCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="OpenCount" SortExpression="OpenCount" HeaderText="${ISI.Reports.TaskSubType.OpenCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>
