<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_MouldUseWarn_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="FCID"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.FCID.Mould}" SortExpression="FCID">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FCID")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Name.Mould}" SortExpression="Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ReferenceCode" HeaderText="${Facility.FacilityMaster.ReferenceCode.Mould}"
                    SortExpression="ReferenceCode" />
                <asp:BoundField DataField="WorkLife" HeaderText="${Facility.FacilityMaster.WorkLife.Mould}"
                    SortExpression="WorkLife" DataFormatString="{0:0.##}" />
                <asp:BoundField DataField="UseQty" HeaderText="${Facility.FacilityMaster.UseQty}"
                    SortExpression="UseQty" DataFormatString="{0:0.##}" />
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
