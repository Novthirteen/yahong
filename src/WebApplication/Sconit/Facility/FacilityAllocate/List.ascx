<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityAllocate_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.FCID.Mould}" SortExpression="FacilityMaster.FCID">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FacilityMaster.FCID")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Name.Mould}" SortExpression="FacilityMaster.Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FacilityMaster.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityAllocate.ReferenceCode}" SortExpression="FacilityMaster.ReferenceCode">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FacilityMaster.ReferenceCode")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityAllocate.ItemCode}" SortExpression="Item.Code">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Item.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityAllocate.ItemDescription}" SortExpression="Item.Desc1">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Item.Desc1")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityAllocate.AllocateType}" SortExpression="AllocateType">
                    <ItemTemplate>
                        <asp:Label ID="lblAllocateType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="MouldCount" HeaderText="${Facility.FacilityAllocate.MouldCount}"
                    SortExpression="MouldCount" />
                <asp:BoundField DataField="GroupName" HeaderText="${Facility.FacilityAllocate.GroupName}"
                    SortExpression="GroupName" />
                <asp:CheckBoxField DataField="IsActive" HeaderText="${Facility.FacilityAllocate.IsActive}"
                    SortExpression="IsActive" />
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
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
