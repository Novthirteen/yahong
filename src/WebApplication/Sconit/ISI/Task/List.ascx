<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_Task_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" OnDataBound="GV_List_DataBound"
            DefaultSortDirection="Descending" DefaultSortExpression="CreateDate">
            <Columns>
                <asp:BoundField DataField="Code" HeaderText="${ISI.Task.Code}" HeaderStyle-Width="8%" />
                <asp:BoundField DataField="TaskSubTypeCode" HeaderText="${ISI.Task.TaskSubTypeCode}"
                    HeaderStyle-Width="6%" />
                <asp:BoundField DataField="Subject" HeaderText="${ISI.Task.Subject}" />
                <asp:BoundField DataField="BackYards" HeaderText="${ISI.Task.BackYards}" HeaderStyle-Width="6%" />
                <asp:BoundField DataField="TaskAddress" HeaderText="${ISI.Task.TaskAddress}" />
                <asp:BoundField DataField="Desc1" HeaderText="${Common.Business.Description}" />
                <asp:TemplateField HeaderText="${Common.CodeMaster.Status}" HeaderStyle-Width="5%">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SubmitUserNm" HeaderText="${ISI.Task.SubmitUser}" />
                <asp:BoundField DataField="AssignUserNm" HeaderText="${ISI.Task.AssignUser}" />
                <asp:BoundField DataField="StartedUser" HeaderText="${ISI.Task.StartUser}" />
                <asp:BoundField DataField="CreateDate" HeaderText="${Common.Business.CreateDate}"
                    DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:TemplateField HeaderText="${ISI.TSK.Flag}" HeaderStyle-Width="3%">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblFlag" runat="server" Code="ISIFlag" Value='<%# Bind("Flag") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}" HeaderStyle-Width="8%">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.View}" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')"
                            Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
