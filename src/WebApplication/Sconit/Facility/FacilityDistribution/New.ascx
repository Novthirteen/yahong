<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_FacilityDistribution_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV">
    <asp:FormView ID="FV_FacilityDistribution" runat="server" DataSourceID="ODS_FacilityDistribution"
        DefaultMode="Insert" Width="100%" DataKeyNames="FCID">
        <InsertItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityDistribution.NewFacilityDistribution}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityDistribution.FCID}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbFCID" runat="server" Text='<%# Bind("FCID") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlCode" runat="server" Text="${Facility.FacilityDistribution.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>'  CssClass="inputRequired" />
                            <asp:CustomValidator ID="cvCode" runat="server" ControlToValidate="tbCode"
                                ErrorMessage="${Facility.FacilityDistribution.Code.Exists}" Display="Dynamic"
                                ValidationGroup="vgSave" OnServerValidate="checkCodeExists"  />
                            <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="tbCode"
                                Display="Dynamic" ErrorMessage="${Facility.FacilityDistribution.Code.Required}"
                                ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCustomerName" runat="server" Text="${Facility.FacilityDistribution.CustomerName}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbCustomerName" runat="server" Visible="true" Width="250" DescField="DistributionContractCode"
                                ValueField="CustomerName" ServicePath="FacilityDistributionMgr.service" ServiceMethod="GetFacilityDistributionCustomer"
                                Text='<%# Bind("CustomerName") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierName" runat="server" Text="${Facility.FacilityDistribution.SupplierName}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbSupplierName" runat="server" Visible="true" Width="250" DescField="PurchaseContractCode"
                                ValueField="SupplierName" ServicePath="FacilityDistributionMgr.service" ServiceMethod="GetFacilityDistributionSupplier"
                                Text='<%# Bind("SupplierName") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDistributionContractCode" runat="server" Text="${Facility.FacilityDistribution.DistributionContractCode}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDistributionContractCode" runat="server" Text='<%# Bind("DistributionContractCode") %>' />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPurchaseContractCode" runat="server" Text="${Facility.FacilityDistribution.PurchaseContractCode}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPurchaseContractCode" runat="server" Text='<%# Bind("PurchaseContractCode") %>' />
                            <asp:CustomValidator ID="cvPurchaseContractCode" runat="server" ControlToValidate="tbPurchaseContractCode"
                                ErrorMessage="${Facility.FacilityDistribution.PurchaseContractCode.Exists}" Display="Dynamic"
                                ValidationGroup="vgSave" OnServerValidate="checkPurchaseContractExists" />

                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDistributionContractAmount" runat="server" Text="${Facility.FacilityDistribution.DistributionContractAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDistributionContractAmount" runat="server" Text='<%# Bind("DistributionContractAmount") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPurchaseContractAmount" runat="server" Text="${Facility.FacilityDistribution.PurchaseContractAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPurchaseContractAmount" runat="server" Text='<%# Bind("PurchaseContractAmount") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDistributionContact" runat="server" Text="${Facility.FacilityDistribution.DistributionContact}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDistributionContact" runat="server" Text='<%# Bind("DistributionContact") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPurchaseContact" runat="server" Text="${Facility.FacilityDistribution.PurchaseContact}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPurchaseContact" runat="server" Text='<%# Bind("PurchaseContact") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlRemark" runat="server" Text="${Facility.FacilityMaster.Remark}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbRemark" runat="server" Text='<%# Bind("Remark") %>' TextMode="MultiLine"
                                Height="50" Width="75%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02"></td>
                        <td class="td01"></td>
                        <td class="td02">
                            <div class="buttons">
                                <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </InsertItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_FacilityDistribution" runat="server" TypeName="com.Sconit.Web.FacilityDistributionMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityDistribution" InsertMethod="CreateFacilityDistribution"
    OnInserted="ODS_FacilityDistribution_Inserted" OnInserting="ODS_FacilityDistribution_Inserting">
    <InsertParameters>
        <asp:Parameter Name="PurchaseContractAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="DistributionContractAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
    </InsertParameters>
</asp:ObjectDataSource>
