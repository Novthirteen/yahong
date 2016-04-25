<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_TaskStatus_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <legend id="lgd" runat="server"></legend>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="false" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" DefaultSortDirection="Descending"
            DefaultSortExpression="CreateDate" OnDataBound="GV_List_DataBound">
            <Columns>
                <asp:BoundField DataField="TaskCode" HeaderText="${ISI.TaskStatus.TaskCode}" SortExpression="TaskCode"
                    HeaderStyle-Width="6%" />
                <asp:TemplateField HeaderText="${ISI.TaskStatus.Desc}">
                    <ItemTemplate>
                        <asp:Label ID="lblDesc" runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="StartDate" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" HeaderText="${Common.Business.StartDate}" SortExpression="StartDate"
                    DataFormatString="{0:yyyy-MM-dd}"  />
                <asp:BoundField DataField="EndDate" HeaderStyle-Wrap="false" ItemStyle-Wrap="false"  HeaderText="${Common.Business.EndDate}" SortExpression="EndDate"
                    DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="CreateUserNm" HeaderText="${Common.Business.CreateUser}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" 
                    SortExpression="CreateUser"  />
                <asp:BoundField DataField="CreateDate" HeaderText="${Common.Business.CreateDate}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" 
                    SortExpression="CreateDate"  DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:TemplateField HeaderText="${ISI.TaskStatus.Flag}" SortExpression="Flag"  HeaderStyle-Wrap="false" ItemStyle-Wrap="false" >
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblFlag" runat="server" Code="ISIFlag" Value='<%# Bind("Flag") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" >
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.View}" OnClick="lbtnEdit_Click" />
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
