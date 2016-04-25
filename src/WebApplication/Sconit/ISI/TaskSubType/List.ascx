<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_TaskSubType_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" DefaultSortDirection="Ascending" DefaultSortExpression="Seq"
            OnRowDataBound="GV_List_RowDataBound" OnDataBound="GV_List_DataBound">
            <Columns>
                <asp:BoundField DataField="Seq" HeaderText="${ISI.TaskSubType.Sequence}" SortExpression="Seq" />
                <asp:BoundField DataField="Code" HeaderText="${ISI.TaskSubType.Code}" SortExpression="Code" />
                <asp:BoundField DataField="Desc" HeaderText="${Common.Business.Description}" SortExpression="Desc" />
                <asp:CheckBoxField DataField="IsCost" HeaderText="${ISI.TaskSubType.IsCost}"
                    SortExpression="IsCost" />
                <asp:CheckBoxField DataField="IsCostCenter" HeaderText="${ISI.TaskSubType.IsCostCenter}"
                    SortExpression="IsCostCenter" />
                <asp:CheckBoxField DataField="IsBudget" HeaderText="${ISI.TaskSubType.IsBudget}"
                    SortExpression="IsBudget" />
                <asp:CheckBoxField DataField="IsTrace" HeaderText="${ISI.TaskSubType.IsTrace}"
                    SortExpression="IsTrace" />
                <asp:CheckBoxField DataField="IsAmount" HeaderText="${ISI.TaskSubType.IsAmount}"
                    SortExpression="IsAmount" />
                <asp:CheckBoxField DataField="IsAmountDetail" HeaderText="${ISI.TaskSubType.IsAmountDetail}"
                    SortExpression="IsAmountDetail" />
                <asp:CheckBoxField DataField="IsApply" HeaderText="${ISI.TaskSubType.IsApply}"
                    SortExpression="IsApply" />
                <asp:CheckBoxField DataField="IsPrint" HeaderText="${ISI.TaskSubType.IsPrint}"
                    SortExpression="IsPrint" />
                <asp:CheckBoxField DataField="IsRemoveForm" HeaderText="${ISI.TaskSubType.IsRemoveForm}"
                    SortExpression="IsRemoveForm" />
                <asp:CheckBoxField DataField="IsAttachment" HeaderText="${ISI.TaskSubType.IsAttachment}"
                    SortExpression="IsAttachment" />
                <asp:TemplateField HeaderText="${ISI.TaskSubType.Process}"></asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code")+"||"+DataBinder.Eval(Container.DataItem, "Desc") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="100">
        </cc1:GridPager>
        <script language="javascript" type="text/javascript">
            $(document).ready(function () {
                $('.GV').fixedtableheader();
            });
        </script>
    </div>
</fieldset>
