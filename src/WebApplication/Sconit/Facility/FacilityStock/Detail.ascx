<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Detail.ascx.cs" Inherits="Facility_FacilityStock_Detail" %>
<div id="divDetail">
    <fieldset>
        <legend>${Facility.FacilityStockDetail}</legend>
        <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="No." ItemStyle-Width="30">
                    <ItemTemplate>
                        <asp:Literal ID="ltlSeq" runat="server" Text='<%# (Container as GridViewRow).RowIndex+1 %> ' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.FCID}">
                    <ItemTemplate>
                        <asp:Label ID="lblFCID" runat="server" Text='<%# Bind("FacilityMaster.FCID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="${Facility.FacilityMaster.Name}">
                    <ItemTemplate>
                        <asp:Label ID="lblName" runat="server" Text='<%# Bind("FacilityMaster.Name") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.AssetNo}">
                    <ItemTemplate>
                        <asp:Label ID="lblAssetNo" runat="server" Text='<%# Bind("FacilityMaster.AssetNo") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.ChargeSite}">
                    <ItemTemplate>
                        <asp:Label ID="lblChargeSite" runat="server" Text='<%# Bind("FacilityMaster.ChargeSite") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.ChargeOrganization}">
                    <ItemTemplate>
                        <asp:Label ID="lblChargeOrganization" runat="server" Text='<%# Bind("FacilityMaster.ChargeOrganization") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.CurrChargePerson}">
                    <ItemTemplate>
                        <asp:Label ID="lblChargePerson" runat="server" Text='<%# Bind("FacilityMaster.CurrChargePersonName") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityStock.InvQty}">
                    <ItemTemplate>
                        <asp:Label ID="lblInvQty" runat="server" Text='<%# Bind("InvQty", "{0:#.######}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityStock.Qty}">
                    <ItemTemplate>
                        <asp:Label ID="lblQty" runat="server" Text='<%# Bind("Qty", "{0:#.######}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnConfirm" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Confirm}" OnClick="lbtnConfirm_Click" Visible="false">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <div class="tablefooter">
            <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                OnClick="btnExport_Click" />
            <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" CssClass="button2"
                OnClick="btnBack_Click" />
        </div>
    </fieldset>
</div>
