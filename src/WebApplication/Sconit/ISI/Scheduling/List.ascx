<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_Scheduling_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" DefaultSortDirection="Ascending"
            DefaultSortExpression="TaskSubType.Code">
            <Columns>
                <asp:BoundField DataField="DayOfWeek" HeaderText="${ISI.Scheduling.Week}" SortExpression="DayOfWeek" />
                <asp:BoundField DataField="Shift" HeaderText="${ISI.Scheduling.Shift}" SortExpression="Shift" />
                <asp:BoundField DataField="Desc" HeaderText="${Common.Business.Description}" SortExpression="Desc" />
                <asp:TemplateField HeaderText="${ISI.Scheduling.TaskSubTypeCode}" SortExpression="TaskSubType.Code">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "TaskSubType.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Scheduling.TaskSubTypeDesc}" SortExpression="TaskSubType.Code">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "TaskSubType.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="StartUser" HeaderText="${ISI.Scheduling.StartUser}" SortExpression="StartUser" />
                <asp:CheckBoxField DataField="IsSpecial" HeaderText="${ISI.Scheduling.IsSpecial}"
                    Visible="false" SortExpression="IsSpecial" />
                <asp:BoundField DataField="StartDate" HeaderStyle-Width="10%" HeaderText="${Common.Business.StartTime}"
                    SortExpression="StartDate" />
                <asp:BoundField DataField="EndDate" HeaderStyle-Width="10%" HeaderText="${Common.Business.EndTime}"
                    SortExpression="EndDate" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
