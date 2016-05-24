<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_Checkup_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="false" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" OnDataBound="GV_List_DataBound"
            DefaultSortDirection="Descending" DefaultSortExpression="CreateDate">
            <Columns>
                <asp:BoundField DataField="Desc" HeaderText="${ISI.Checkup.CheckupUserNm}" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" SortExpression="CheckupUserNm" HeaderStyle-Width="5%" HeaderStyle-Wrap="false" />
                <asp:BoundField DataField="CheckupDate" HeaderText="${ISI.Checkup.CheckupDate}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" SortExpression="CheckupDate" DataFormatString="{0:yyyy-MM-dd}" HeaderStyle-Width="5%" />
                <asp:TemplateField HeaderText="${ISI.Checkup.CheckupProject}" SortExpression="CheckupProject.Code" HeaderStyle-Width="6%" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <div><%# DataBinder.Eval(Container.DataItem, "CheckupProject.Name")%></div>
                        <div><%# DataBinder.Eval(Container.DataItem, "SummaryCode")%>  </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.CodeMaster.Status}" SortExpression="Status" HeaderStyle-Wrap="false"
                    HeaderStyle-Width="4%">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Amount" HeaderText="${ISI.Checkup.Amount}" SortExpression="Amount" DataFormatString="{0:0.##}" HeaderStyle-Width="4%" HeaderStyle-Wrap="false" />
                <asp:TemplateField HeaderText="${ISI.Checkup.Content}" HeaderStyle-Wrap="false" HeaderStyle-Width="28%" ItemStyle-Width="28%">
                    <ItemTemplate>
                        <asp:Label ID="lblContent" runat="server" Width="100%" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Checkup.AuditInstructions}" HeaderStyle-Wrap="false" HeaderStyle-Width="28%" ItemStyle-Width="28%">
                    <ItemTemplate>
                        <asp:Label ID="lblAuditInstructions" Width="100%" runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}" HeaderStyle-Wrap="false" HeaderStyle-Width="5%" ItemStyle-Width="5%" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.View}" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" Visible="false" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
        <script language="javascript" type="text/javascript">
            $(document).ready(function () {
                $('.GV').fixedtableheader();
            });
        </script>
    </div>
</fieldset>
