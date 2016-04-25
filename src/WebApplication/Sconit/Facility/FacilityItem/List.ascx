<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityItem_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="${Facility.FacilityItem.FCID}" SortExpression="FacilityMaster.FCID">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FCID")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityItem.ItemCode}" SortExpression="Item.Code">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Item.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityItem.ItemDescription}" SortExpression="Item.Desc1">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Item.Desc1")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Qty" HeaderText="${Facility.FacilityItem.Qty}" SortExpression="Qty"
                    DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="Amount" HeaderText="${Facility.FacilityItem.Amount}" SortExpression="Amount"
                    DataFormatString="{0:0.###}" />
                <asp:TemplateField HeaderText="${Facility.FacilityItem.AllocateType}" SortExpression="AllocateType">
                    <ItemTemplate>
                        <asp:Label ID="lblAllocateType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="InitQty" HeaderText="${Facility.FacilityItem.InitQty}"
                    SortExpression="InitQty" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SingleQty" HeaderText="${Facility.FacilityItem.SingleQty}"
                    SortExpression="SingleQty" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="AllocatedQty" HeaderText="${Facility.FacilityItem.AllocatedQty}"
                    SortExpression="AllocatedQty" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="AllocatedAmount" HeaderText="${Facility.FacilityItem.AllocatedAmount}"
                    SortExpression="AllocatedAmount" DataFormatString="{0:0.###}" />
                <asp:TemplateField HeaderText="${Facility.FacilityItem.AllocatedRate}">
                    <ItemTemplate>
                        <asp:Label ID="lblAllocateRate" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityItem.Remark}" SortExpression="Remark">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Remark")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--   <asp:CheckBoxField DataField="IsAllocate" HeaderText="${Facility.FacilityItem.IsAllocate}"
                    SortExpression="IsAllocate" />--%>
                <asp:CheckBoxField DataField="IsActive" HeaderText="${Facility.FacilityItem.IsActive}"
                    SortExpression="IsActive" />
                <asp:BoundField DataField="CreateDate" HeaderText="${Common.Business.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}"
                    SortExpression="CreateDate" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="50">
        </cc1:GridPager>
    </div>
</fieldset>
