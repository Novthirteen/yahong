<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_FailureMode_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount">
            <Columns>
                <asp:BoundField DataField="Code" HeaderText="${ISI.FailureMode.Code}" SortExpression="Code" />
                <asp:BoundField DataField="Desc" HeaderText="${Common.Business.Description}" SortExpression="Desc" />
                <asp:BoundField DataField="Seq" HeaderText="${ISI.FailureMode.Sequence}" SortExpression="Seq" />
                <asp:TemplateField HeaderText="${ISI.FailureMode.TaskSubType}" SortExpression="TaskSubType.Code">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "TaskSubType.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CheckBoxField DataField="IsActive" SortExpression="IsActive" HeaderText="${Common.IsActive}" />
                <asp:BoundField DataField="CreateDate" HeaderText="${Common.Business.CreateDate}"
                    SortExpression="CreateDate" />
                <asp:BoundField DataField="CreateUser" HeaderText="${Common.Business.CreateUser}"
                    SortExpression="CreateUser" />
                <asp:BoundField DataField="LastModifyDate" HeaderText="${Common.Business.LastModifyDate}"
                    SortExpression="LastModifyDate" />
                <asp:BoundField DataField="LastModifyUser" HeaderText="${Common.Business.LastModifyUser}"
                    SortExpression="LastModifyUser" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
