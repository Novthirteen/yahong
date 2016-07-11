<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityFixOrder_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="FixNo"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="True" AllowPaging="True" PagerID="gp" Width="100%"
            TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll" SelectCountMethod="FindCount"
            OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="CreateDate" DefaultSortDirection="Descending">
            <Columns>
                <asp:BoundField DataField="FixNo" HeaderText="${Facility.FacilityFixOrder.FixNo}"
                    SortExpression="FixNo" />
                <asp:BoundField DataField="FCID" HeaderText="${Facility.FacilityFixOrder.FCID}" SortExpression="FCID" />
                <asp:BoundField DataField="FacilityName" HeaderText="${Facility.FacilityFixOrder.FacilityName}"
                    SortExpression="FacilityName" />
                <asp:BoundField DataField="ReferenceCode" HeaderText="${Facility.FacilityFixOrder.ReferenceCode}"
                    SortExpression="ReferenceCode" />
                <asp:TemplateField HeaderText="${Common.CodeMaster.Status}" SortExpression="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Common.CreateUser}" SortExpression="CreateUser">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CreateUser")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Common.CreateDate}" SortExpression="CreateDate">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CreateDate")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FixNo") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
