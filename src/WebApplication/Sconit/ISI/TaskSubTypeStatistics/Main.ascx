<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_TaskSubTypeStatistics_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td align="right">
                <asp:Literal ID="ltlDepartment" runat="server" Text="${ISI.TaskSubTypeStatistics.Org}|${ISI.TaskSubTypeStatistics.Type}:" />
            </td>
            <td>
                <cc1:CodeMstrDropDownList ID="ddlDept" Code="ISIDepartment" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
                <cc1:CodeMstrDropDownList ID="ddlType" Code="ISIType" runat="server" IncludeBlankOption="true"
                    Width="120px" DefaultSelectedValue="" />
            </td>
            <td align="right">
                <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.TaskSubTypeStatistics.TaskSubType}:" />
            </td>
            <td>
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                    Width="260" />
            </td>
            <td align="right">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td>
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                    Display="Dynamic" ControlToValidate="tbStartDate" ValidationGroup="vgSearch" />
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

            <td></td>
            <td></td>
            <td colspan="4"></td>
            <td align="right">
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
        CellPadding="0" AllowSorting="true" OnSorting="GV_List_Sorting">
        <Columns>
            <asp:TemplateField HeaderText="No.">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Code" SortExpression="Code" HeaderText="${ISI.TaskSubTypeStatistics.Code}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="Description" HeaderText="${ISI.TaskSubTypeStatistics.Description}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="Project" HeaderText="${ISI.TaskSubTypeStatistics.Project}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="Enc" HeaderText="${ISI.TaskSubTypeStatistics.Enc}" HeaderStyle-Wrap="false" />
            <asp:BoundField HeaderText="${ISI.TaskSubTypeStatistics.LastWeekTotalCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="TotalCount" SortExpression="TotalCount" HeaderText="${ISI.TaskSubTypeStatistics.TotalCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="CreateCount" SortExpression="CreateCount" HeaderText="${ISI.TaskSubTypeStatistics.CreateCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}"/>
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderText="${ISI.TaskSubTypeStatistics.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="SubmitDate" SortExpression="SubmitDate" HeaderText="${ISI.TaskSubTypeStatistics.SubmitDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AssignDate" Visible="false" SortExpression="AssignDate" HeaderText="${ISI.TaskSubTypeStatistics.AssignDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="StartDate" Visible="false" SortExpression="StartDate" HeaderText="${ISI.TaskSubTypeStatistics.StartDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="CompleteDate" Visible="false" SortExpression="CompleteDate" HeaderText="${ISI.TaskSubTypeStatistics.CompleteDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="CloseDate" Visible="false" SortExpression="CloseDate" HeaderText="${ISI.TaskSubTypeStatistics.CloseDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="OpenDate" Visible="false" SortExpression="OpenDate" HeaderText="${ISI.TaskSubTypeStatistics.OpenDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="CancelDate" Visible="false" SortExpression="CancelDate" HeaderText="${ISI.TaskSubTypeStatistics.CancelDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="StatusCount" SortExpression="StatusCount" HeaderText="${ISI.TaskSubTypeStatistics.StatusCount}" HeaderStyle-Wrap="false" DataFormatString="{0:#}" />
            <asp:BoundField DataField="StatusDate" SortExpression="StatusDate" HeaderText="${ISI.TaskSubTypeStatistics.StatusDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="CommentCount" SortExpression="CommentCount" HeaderText="${ISI.TaskSubTypeStatistics.CommentCount}" DataFormatString="{0:#}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="CommentDate" SortExpression="CommentDate" HeaderText="${ISI.TaskSubTypeStatistics.CommentDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Wrap="false" />
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>

