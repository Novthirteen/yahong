﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_MouldMaster_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="FCID"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:BoundField DataField="FCID" HeaderText="${Facility.FacilityMaster.FCID.Mould}"
                    SortExpression="FCID" />
                <asp:BoundField DataField="Name" HeaderText="${Facility.FacilityMaster.Name.Mould}"
                    SortExpression="Name" />
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Category.Mould}" SortExpression="Category">
                    <ItemTemplate>
                        <asp:Label ID="lblCategory" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="AssetNo" HeaderText="${Facility.FacilityMaster.AssetNo}"
                    SortExpression="AssetNo" />
                <asp:BoundField DataField="ChargeSite" HeaderText="${Facility.FacilityMaster.ChargeSite.Mould}"
                    SortExpression="ChargeSite" />
                <asp:BoundField DataField="ChargeOrganization" HeaderText="${Facility.FacilityMaster.ChargeOrganization}"
                    SortExpression="ChargeOrganization" />
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Status}" SortExpression="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ReferenceCode" HeaderText="${Facility.FacilityMaster.ReferenceCode.Mould}"
                    SortExpression="ReferenceCode" />
                <asp:BoundField DataField="Specification" HeaderText="${Facility.FacilityMaster.Specification.Mould}"
                    SortExpression="Specification" />
                <asp:BoundField DataField="EffDate" HeaderText="${Facility.FacilityMaster.EffDate}"
                    SortExpression="EffDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Price" HeaderText="${Facility.FacilityMaster.Price}" SortExpression="Price" />
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Owner}" SortExpression="Owner">
                    <ItemTemplate>
                        <asp:Label ID="lblOwner" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="OwnerDescription" HeaderText="${Facility.FacilityMaster.OwnerDescription}"
                    SortExpression="OwnerDescription" />
 
                <asp:BoundField DataField="WorkLife" HeaderText="${Facility.FacilityMaster.WarrantyInfo.Mould}"
                    SortExpression="WorkLife" />

                <asp:BoundField DataField="UseQty" HeaderText="${Facility.FacilityMaster.UseQty}"
                    SortExpression="UseQty" DataFormatString="{0:0.##}" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FCID") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="20">
        </cc1:GridPager>
    </div>
</fieldset>
