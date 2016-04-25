<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_MyTask_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" DefaultSortDirection="Descending"
            DefaultSortExpression="CreateDate" OnDataBound="GV_List_DataBound">
            <Columns>
                <asp:TemplateField HeaderText="${ISI.MyTask.Code}" HeaderStyle-Width="7%">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code")+"|"+DataBinder.Eval(Container.DataItem, "Type") %>'
                            Text='<%# DataBinder.Eval(Container.DataItem, "Code") %>' OnClick="lbtnEdit_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Subject" HeaderText="${ISI.MyTask.Subject}" SortExpression="Subject" />
                <asp:TemplateField HeaderText="${ISI.MyTask.TaskSubType}" SortExpression="TaskSubType.Code"
                    HeaderStyle-Width="8%">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "TaskSubType.Desc")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.MyTask.TaskAddress}" SortExpression="TaskAddress"
                    HeaderStyle-Width="8%">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "TaskAddress")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.CodeMaster.Status}" SortExpression="Status"
                    HeaderStyle-Width="5%">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CreateUserNm" HeaderText="${Common.Business.CreateUser}"
                    SortExpression="CreateUserNm" HeaderStyle-Width="6%" />
                <asp:BoundField DataField="StartedUser" HeaderText="${ISI.MyTask.StartedUser}" />
                <asp:BoundField DataField="PlanStartDate" HeaderText="${ISI.MyTask.PlanStartDate}"
                    SortExpression="PlanStartDate" HeaderStyle-Width="8%" DataFormatString="{0:yyyy-MM-dd HH:ss}" HeaderStyle-Wrap="false" />
                <asp:BoundField DataField="PlanCompleteDate" HeaderText="${ISI.MyTask.PlanCompleteDate}" HeaderStyle-Wrap="false"
                    SortExpression="PlanCompleteDate" HeaderStyle-Width="8%" DataFormatString="{0:yyyy-MM-dd HH:ss}" />
                <asp:TemplateField HeaderText="${ISI.MyTask.Flag}" SortExpression="Flag" HeaderStyle-Width="6%">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblFlag" runat="server" Code="ISIFlag" Value='<%# Bind("Flag") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}" HeaderStyle-Width="5%"
                    Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')"
                            Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="100">
        </cc1:GridPager>
    </div>
</fieldset>
