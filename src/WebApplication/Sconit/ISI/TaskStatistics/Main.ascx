<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_TaskStatistics_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td align="right">
                <asp:Literal ID="ltlDepartment" runat="server" Text="${ISI.TaskStatistics.Dept2}|${ISI.TaskStatistics.Dept}:" />
            </td>
            <td>
                <uc3:textbox ID="tbDept2" runat="server" Visible="true" DescField="Code" MustMatch="true"
                    ValueField="Name" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetAllTaskSubType"
                    Width="260" ServiceParameter="bool:true"/>
                <cc1:CodeMstrDropDownList ID="ddlDept" Code="ISIDepartment" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>

            <td align="right">
                <asp:Literal ID="lblPosition" runat="server" Text="${ISI.TaskStatistics.Position}:" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlPosition" Code="ISIPosition" runat="server" IncludeBlankOption="true" DefaultSelectedValue="" />
            </td>
            <td align="right">
                <asp:Literal ID="ltlType" runat="server" Text="${ISI.TaskStatistics.Type}:" />
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
                <asp:Literal ID="lblUser" runat="server" Text="${ISI.TaskStatistics.User}:" />
            </td>
            <td>
                <uc3:textbox ID="tbUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
            <td align="right">
                <asp:Literal ID="ltlOrg" runat="server" Text="${ISI.TaskStatistics.Org}:" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlOrg" Code="ISIDepartment" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>

            <td align="right">
                <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.TaskStatistics.TaskSubType}:" />
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
                <asp:CheckBox runat="server" Text="${ISI.TaskStatistics.IsFilter}" ID="cbIsFilter" Checked="true" />
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
        CellPadding="0" AllowSorting="true" OnSorting="GV_List_Sorting">
        <Columns>
            <asp:TemplateField HeaderText="No.">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Name" SortExpression="Name" HeaderText="${ISI.TaskStatistics.Name}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="JobNo" SortExpression="JobNo" HeaderText="${ISI.TaskStatistics.JobNo}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="Dept2" SortExpression="Dept2" HeaderText="${ISI.TaskStatistics.Dept2}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="Dept" SortExpression="Dept" HeaderText="${ISI.TaskStatistics.Dept}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="CreateCount" SortExpression="CreateCount" HeaderText="${ISI.TaskStatistics.CreateCount}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="SubmitCount" SortExpression="SubmitCount" HeaderText="${ISI.TaskStatistics.SubmitCount}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="SubmitDate" SortExpression="SubmitDate" HeaderText="${ISI.TaskStatistics.SubmitDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:BoundField DataField="CancelCount" SortExpression="CancelCount" HeaderText="${ISI.TaskStatistics.CancelCount}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AssignCount" SortExpression="AssignCount" HeaderText="${ISI.TaskStatistics.AssignCount}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="SubmitFirstCount" SortExpression="SubmitFirstCount" HeaderText="${ISI.TaskStatistics.SubmitFirstCount}" />
            <asp:BoundField DataField="SubmitInProcessFirstCount" SortExpression="SubmitInProcessFirstCount" HeaderText="${ISI.TaskStatistics.SubmitInProcessFirstCount}" />
            <asp:BoundField DataField="SubmitInProcessFirstStatusCount" SortExpression="SubmitInProcessFirstStatusCount" HeaderText="${ISI.TaskStatistics.SubmitInProcessFirstStatusCount}" />
            <asp:BoundField DataField="NoStatusCount" SortExpression="NoStatusCount" HeaderText="${ISI.TaskStatistics.NoStatusCount}" />
            <asp:BoundField DataField="FirstCount" SortExpression="FirstCount" HeaderText="${ISI.TaskStatistics.FirstCount}" />
            <asp:TemplateField HeaderText="${ISI.TaskStatistics.NoFirstCount}" />
            <asp:BoundField DataField="StatusCount" SortExpression="StatusCount" HeaderText="${ISI.TaskStatistics.StatusCount}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="StatusDate" SortExpression="StatusDate" HeaderText="${ISI.TaskStatistics.StatusDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:BoundField DataField="FileCount" SortExpression="FileCount" HeaderText="${ISI.TaskStatistics.FileCount}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="CommentCount" SortExpression="CommentCount" HeaderText="${ISI.TaskStatistics.CommentCount}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="CloseCount" SortExpression="CloseCount" HeaderText="${ISI.TaskStatistics.CloseCount}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="OpenCount" SortExpression="OpenCount" HeaderText="${ISI.TaskStatistics.OpenCount}" HeaderStyle-Wrap="false" />

            <asp:BoundField DataField="NoStatusAssignCount" SortExpression="NoStatusAssignCount" HeaderText="${ISI.TaskStatistics.NoStatusAssignCount}" />
            <asp:BoundField DataField="NoCommentCount" SortExpression="NoCommentCount" HeaderText="${ISI.TaskStatistics.NoCommentCount}" />
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>
