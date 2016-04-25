<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_Filter_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:BoundField DataField="UserCode" HeaderText="${ISI.Filter.UserCode}" SortExpression="UserCode" />
                <asp:BoundField DataField="UserName" HeaderText="${ISI.Filter.UserName}" SortExpression="UserName" />
                <asp:BoundField DataField="Desc" HeaderText="${Common.Business.Description}" SortExpression="Desc" />
                <asp:BoundField DataField="TaskCode" HeaderText="${ISI.Filter.TaskCode}" SortExpression="TaskCode" />
                <asp:BoundField DataField="TaskType" HeaderText="${ISI.Filter.TaskType}" SortExpression="TaskType" />
                <asp:BoundField DataField="TaskSubType" HeaderText="${ISI.Filter.TaskSubType}" SortExpression="TaskSubType" />
                <asp:BoundField DataField="Email" HeaderText="${ISI.Filter.Email}" SortExpression="Email" />
                <asp:BoundField DataField="CreateDate" HeaderText="${Common.Business.CreateDate}"
                    SortExpression="CreateDate" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="CreateUserNm" HeaderText="${Common.Business.CreateUser}"
                    SortExpression="CreateUser" />
                <asp:BoundField DataField="LastModifyDate" HeaderText="${Common.Business.LastModifyDate}"
                    SortExpression="LastModifyDate" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="LastModifyUserNm" HeaderText="${Common.Business.LastModifyUser}"
                    SortExpression="LastModifyUser" />
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
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="100">
        </cc1:GridPager>
    </div>
</fieldset>
