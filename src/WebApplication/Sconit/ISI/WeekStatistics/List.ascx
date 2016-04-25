<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_WeekStatistics_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="True" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" DefaultSortExpression="Code"
            DefaultSortDirection="Ascending" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Name" SortExpression="Name" HeaderText="${ISI.WeekStatistics.Name}" />
                <asp:BoundField DataField="JobNo" SortExpression="Code" HeaderText="${ISI.WeekStatistics.JobNo}" />
                <asp:BoundField DataField="Dept2" SortExpression="Code" HeaderText="${ISI.WeekStatistics.Dept2}" />
                <asp:BoundField DataField="Dept" SortExpression="Code" HeaderText="${ISI.WeekStatistics.Dept}" />
                <asp:BoundField DataField="CreateCount" SortExpression="CreateCount" HeaderText="${ISI.WeekStatistics.CreateCount}" />
                <asp:BoundField DataField="SubmitCount" SortExpression="SubmitCount" HeaderText="${ISI.WeekStatistics.SubmitCount}" />
                <asp:BoundField DataField="CancelCount" SortExpression="CancelCount" HeaderText="${ISI.WeekStatistics.CancelCount}" />
                <asp:BoundField DataField="AssignCount" SortExpression="AssignCount" HeaderText="${ISI.WeekStatistics.AssignCount}" />
                <asp:BoundField DataField="SubmitFirstCount" SortExpression="SubmitFirstCount" HeaderText="${ISI.WeekStatistics.SubmitFirstCount}" /> 
                <asp:BoundField DataField="FirstCount" SortExpression="FirstCount" HeaderText="${ISI.WeekStatistics.FirstCount}" />
                <asp:BoundField DataField="StatusCount" SortExpression="StatusCount" HeaderText="${ISI.WeekStatistics.StatusCount}" />
                <asp:BoundField DataField="FileCount" SortExpression="FileCount" HeaderText="${ISI.WeekStatistics.FileCount}" />
                <asp:BoundField DataField="CommentCount" SortExpression="CommentCount" HeaderText="${ISI.WeekStatistics.CommentCount}" />
                <asp:BoundField DataField="CloseCount" SortExpression="CloseCount" HeaderText="${ISI.WeekStatistics.CloseCount}" />
                <asp:BoundField DataField="OpenCount" SortExpression="OpenCount" HeaderText="${ISI.WeekStatistics.OpenCount}" />
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
