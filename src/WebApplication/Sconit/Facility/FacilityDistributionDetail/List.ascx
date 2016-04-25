<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityDistributionDetail_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="Id"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.FCID}" SortExpression="FacilityDistribution.FCID">
                    <ItemTemplate>
                        <asp:Label ID="lblFCID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.FCID")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.CustomerName}" SortExpression="FacilityDistribution.CustomerName">
                    <ItemTemplate>
                        <asp:Label ID="lblCustomerName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.CustomerName")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.DistributionContractCode}"
                    SortExpression="FacilityDistribution.DistributionContractCode">
                    <ItemTemplate>
                        <asp:Label ID="lblDistributionContractCode" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.DistributionContractCode")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.DistributionContractAmount}"
                    SortExpression="FacilityDistribution.DistributionContractAmount">
                    <ItemTemplate>
                        <asp:Label ID="lblDistributionContractAmount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.DistributionContractAmount","{0:0.###}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.DistributionBilledAmount}"
                    SortExpression="FacilityDistribution.DistributionBilledAmount">
                    <ItemTemplate>
                        <asp:Label ID="lblDistributionBilledAmount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.DistributionBilledAmount","{0:0.###}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.DistributionPayAmount}"
                    SortExpression="FacilityDistribution.DistributionPayAmount">
                    <ItemTemplate>
                        <asp:Label ID="lblDistributionPayAmount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.DistributionPayAmount","{0:0.###}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="${Facility.FacilityDistribution.DistributionContact}"
                    SortExpression="FacilityDistribution.DistributionContact">
                    <ItemTemplate>
                        <asp:Label ID="lblDistributionContact" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.DistributionContact","{0:0.###}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                
                 <asp:TemplateField HeaderText="${Facility.FacilityDistribution.SupplierName}" SortExpression="FacilityDistribution.SupplierName">
                    <ItemTemplate>
                        <asp:Label ID="lblSupplierName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.SupplierName")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.PurchaseContractCode}"
                    SortExpression="FacilityDistribution.PurchaseContractCode">
                    <ItemTemplate>
                        <asp:Label ID="lblPurchaseContractCode" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.PurchaseContractCode")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.PurchaseContractAmount}"
                    SortExpression="FacilityDistribution.PurchaseContractAmount">
                    <ItemTemplate>
                        <asp:Label ID="lblPurchaseContractAmount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.PurchaseContractAmount","{0:0.###}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.PurchaseBilledAmount}"
                    SortExpression="FacilityDistribution.PurchaseBilledAmount">
                    <ItemTemplate>
                        <asp:Label ID="lblPurchaseBilledAmount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.PurchaseBilledAmount","{0:0.###}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.PurchasePayAmount}"
                    SortExpression="FacilityDistribution.PurchasePayAmount">
                    <ItemTemplate>
                        <asp:Label ID="lblPurchasePayAmount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.PurchasePayAmount","{0:0.###}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                   <asp:TemplateField HeaderText="${Facility.FacilityDistribution.PurchaseContact}"
                    SortExpression="FacilityDistribution.PurchaseContact">
                    <ItemTemplate>
                        <asp:Label ID="lblPurchaseContact" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FacilityDistribution.PurchaseContact","{0:0.###}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistributionDetail.Type}" SortExpression="Type">
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Invoice" HeaderText="${Facility.FacilityDistributionDetail.Invoice}"
                    SortExpression="Invoice" />
                <asp:BoundField DataField="BillDate" HeaderText="${Facility.FacilityDistributionDetail.BillDate}"
                    SortExpression="BillDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="BillAmount" HeaderText="${Facility.FacilityDistributionDetail.BillAmount}"
                    SortExpression="BillAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="PayDate" HeaderText="${Facility.FacilityDistributionDetail.PayDate}"
                    SortExpression="PayDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="PayAmount" HeaderText="${Facility.FacilityDistributionDetail.PayAmount}"
                    SortExpression="PayAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="Contact" HeaderText="${Facility.FacilityDistributionDetail.Contact}"
                    SortExpression="Contact" />
                <asp:BoundField DataField="CreateDate" HeaderText="${Facility.FacilityDistributionDetail.CreateDate}"
                    SortExpression="CreateDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="CreateUser" HeaderText="${Facility.FacilityDistributionDetail.CreateUser}"
                    SortExpression="CreateUser" />
                <asp:BoundField DataField="Remark" HeaderText="${Facility.FacilityDistributionDetail.Remark}"
                    SortExpression="Remark" />
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
