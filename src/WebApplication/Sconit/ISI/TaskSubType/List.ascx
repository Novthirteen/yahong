<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_TaskSubType_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="false" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" DefaultSortDirection="Ascending" DefaultSortExpression="Seq"
            OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Seq" HeaderText="${ISI.TaskSubType.Sequence}" SortExpression="Seq" />
                <asp:BoundField DataField="Code" HeaderText="${ISI.TaskSubType.Code}" SortExpression="Code" />
                <asp:BoundField DataField="Desc" HeaderText="${Common.Business.Description}" SortExpression="Desc" />
                <asp:TemplateField HeaderText="${ISI.TaskSubType.Parent}" SortExpression="Parent.Code">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Parent.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CheckBoxField DataField="IsAssignUp" HeaderText="${ISI.TaskSubType.IsAssignUp}"
                    SortExpression="IsAssignUp" />
                <asp:TemplateField HeaderText="${ISI.TaskSubType.AssignUpLevelCount}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "AssignUpLevelCount")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CheckBoxField DataField="IsStartUp" HeaderText="${ISI.TaskSubType.IsStartUp}"
                    SortExpression="IsStartUp" />
                <asp:TemplateField HeaderText="${ISI.TaskSubType.StartUpLevelCount}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "StartUpLevelCount")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CheckBoxField DataField="IsCloseUp" HeaderText="${ISI.TaskSubType.IsCloseUp}"
                    SortExpression="IsCloseUp" />
                <asp:TemplateField HeaderText="${ISI.TaskSubType.CloseUpLevelCount}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CloseUpLevelCount")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TaskSubType.Process}">
                </asp:TemplateField>
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
