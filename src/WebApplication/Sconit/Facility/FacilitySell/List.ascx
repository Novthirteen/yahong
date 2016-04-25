<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilitySell_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="FCID"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:BoundField DataField="FCID" HeaderText="${Facility.FacilityMaster.FCID}" SortExpression="FCID" />
                <asp:BoundField DataField="AssetNo" HeaderText="${Facility.FacilityMaster.AssetNo}"
                    SortExpression="AssetNo" />
                <asp:BoundField DataField="Name" HeaderText="${Facility.FacilityMaster.Name}" SortExpression="Name" />
                <asp:BoundField DataField="Capacity" HeaderText="${Facility.FacilityMaster.Capacity}"
                    SortExpression="Capacity" />
                <asp:BoundField DataField="ManufactureDate" HeaderText="${Facility.FacilityMaster.ManufactureDate}"
                    SortExpression="ManufactureDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SerialNo" HeaderText="${Facility.FacilityMaster.SerialNo}"
                    SortExpression="SerialNo" />
                <asp:BoundField DataField="WarrantyInfo" HeaderText="${Facility.FacilityMaster.WarrantyInfo}"
                    SortExpression="WarrantyInfo" />
                <asp:BoundField DataField="EffDate" HeaderText="${Facility.FacilityMaster.EffDate}"
                    SortExpression="EffDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Price" HeaderText="${Facility.FacilityMaster.Price}" SortExpression="Price" />
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Owner}" SortExpression="Owner">
                    <ItemTemplate>
                        <asp:Label ID="lblOwner" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Status}" SortExpression="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CurrChargePersonName" HeaderText="${Facility.FacilityMaster.CurrChargePerson}"
                    SortExpression="CurrChargePersonName" />
                <asp:BoundField DataField="ChargeSite" HeaderText="${Facility.FacilityMaster.ChargeSite}"
                    SortExpression="ChargeSite" />
                <asp:BoundField DataField="ChargeDate" HeaderText="${Facility.FacilityMaster.ChargeDate}"
                    SortExpression="ChargeDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Category}" SortExpression="Category">
                    <ItemTemplate>
                        <asp:Label ID="lblCategory" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnSell" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FCID") %>'
                            Text="${Common.Button.SellFacility}" OnClick="lbtnSell_Click" >
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
