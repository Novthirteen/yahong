<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityDistribution_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="ListAttachment.ascx" TagName="ListAttachment" TagPrefix="uc2" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:BoundField DataField="FCID" HeaderText="${Facility.FacilityDistribution.FCID}"
                    SortExpression="FCID" />
                <asp:BoundField DataField="CustomerName" HeaderText="${Facility.FacilityDistribution.CustomerName}"
                    SortExpression="CustomerName" />
                <asp:BoundField DataField="Code" HeaderText="${Facility.FacilityDistribution.Code}"
                    SortExpression="Code" />
                <asp:BoundField DataField="DistributionContractCode" HeaderText="${Facility.FacilityDistribution.DistributionContractCode}"
                    SortExpression="DistributionContractCode" />
                <asp:BoundField DataField="DistributionContractAmount" HeaderText="${Facility.FacilityDistribution.DistributionContractAmount}"
                    SortExpression="DistributionContractAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="DistributionBilledAmount" HeaderText="${Facility.FacilityDistribution.DistributionBilledAmount}"
                    SortExpression="DistributionBilledAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="DistributionPayAmount" HeaderText="${Facility.FacilityDistribution.DistributionPayAmount}"
                    SortExpression="DistributionPayAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="DistributionContact" HeaderText="${Facility.FacilityDistribution.DistributionContact}"
                    SortExpression="DistributionContact" />
                <asp:BoundField DataField="SupplierName" HeaderText="${Facility.FacilityDistribution.SupplierName}"
                    SortExpression="SupplierName" />
                <asp:BoundField DataField="PurchaseContractCode" HeaderText="${Facility.FacilityDistribution.PurchaseContractCode}"
                    SortExpression="PurchaseContractCode" />
                <asp:BoundField DataField="PurchaseContractAmount" HeaderText="${Facility.FacilityDistribution.PurchaseContractAmount}"
                    SortExpression="PurchaseContractAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="PurchaseBilledAmount" HeaderText="${Facility.FacilityDistribution.PurchaseBilledAmount}"
                    SortExpression="PurchaseBilledAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="PurchasePayAmount" HeaderText="${Facility.FacilityDistribution.PurchasePayAmount}"
                    SortExpression="PurchasePayAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="PurchaseContact" HeaderText="${Facility.FacilityDistribution.PurchaseContact}"
                    SortExpression="PurchaseContact" />
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.Status}" SortExpression="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.Attachment}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnAttachment" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Attachment}" OnClick="lbtnAttachment_Click" Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>
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
<uc2:ListAttachment ID="ucTransAttachment" runat="server" Visible="false" />
