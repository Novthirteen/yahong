<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_FacilityDistribution_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_FacilityDistribution" runat="server" DataSourceID="ODS_FacilityDistribution"
        DefaultMode="Edit" Width="100%" DataKeyNames="Id" OnDataBound="FV_FacilityDistribution_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityDistribution.UpdateFacilityDistribution}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityDistribution.FCID}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbFCID" runat="server" Text='<%# Bind("FCID") %>' />
                            <asp:HiddenField ID="hfCreateDate" Value='<%# Bind("CreateDate") %>' runat="server" />
                            <asp:HiddenField ID="hfCreateUser" Value='<%# Bind("CreateUser") %>' runat="server" />
                            <asp:HiddenField ID="hfStatus" Value='<%# Bind("Status") %>' runat="server" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlCode" runat="server" Text="${Facility.FacilityDistribution.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' Enabled="false" />
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
                            <asp:HiddenField ID="hfDistributionContractCode" Value='<%# Bind("DistributionContractCode") %>' runat="server" />
                          <%--  <asp:CustomValidator ID="cvDistributionContractCode" runat="server" ControlToValidate="tbDistributionContractCode"
                                ErrorMessage="${Facility.FacilityDistribution.DistributionContractCode.Exists}" Display="Dynamic"
                                ValidationGroup="vgSave" OnServerValidate="checkDistributionContractExists" />--%>
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPurchaseContractCode" runat="server" Text="${Facility.FacilityDistribution.PurchaseContractCode}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPurchaseContractCode" runat="server" Text='<%# Bind("PurchaseContractCode") %>' />
                          <%--  <asp:CustomValidator ID="cvPurchaseContractCode" runat="server" ControlToValidate="tbPurchaseContractCode"
                                ErrorMessage="${Facility.FacilityDistribution.PurchaseContractCode.Exists}" Display="Dynamic"
                                ValidationGroup="vgSave" OnServerValidate="checkFacilityDistributionExists" />--%>
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDistributionContractAmount" runat="server" Text="${Facility.FacilityDistribution.DistributionContractAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDistributionContractAmount" runat="server" Text='<%# Bind("DistributionContractAmount","{0:0.########}") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPurchaseContractAmount" runat="server" Text="${Facility.FacilityDistribution.PurchaseContractAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPurchaseContractAmount" runat="server" Text='<%# Bind("PurchaseContractAmount","{0:0.########}") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDistributionBilledAmount" runat="server" Text="${Facility.FacilityDistribution.DistributionBilledAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDistributionBilledAmount" runat="server" Text='<%# Bind("DistributionBilledAmount","{0:0.########}") %>'
                                Enabled="false" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPurchaseBilledAmount" runat="server" Text="${Facility.FacilityDistribution.PurchaseBilledAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPurchaseBilledAmount" runat="server" Text='<%# Bind("PurchaseBilledAmount","{0:0.########}") %>'
                                Enabled="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDistributionPayAmount" runat="server" Text="${Facility.FacilityDistribution.DistributionPayAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDistributionPayAmount" runat="server" Text='<%# Bind("DistributionPayAmount","{0:0.########}") %>'
                                Enabled="false" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPurchasePayAmount" runat="server" Text="${Facility.FacilityDistribution.PurchasePayAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPurchasePayAmount" runat="server" Text='<%# Bind("PurchasePayAmount","{0:0.########}") %>'
                                Enabled="false" />
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
                            <asp:Literal ID="lblStatus" runat="server" Text="${Facility.FacilityDistribution.Status}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbStatusDesc" runat="server" />
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
                                <cc1:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" FunctionId="SaveFacilityDistribution" />
                                <cc1:Button ID="btnCopy" runat="server" Text="${Common.Button.Copy}"
                                    OnClick="btnCopy_Click" ValidationGroup="vgSave" FunctionId="SaveFacilityDistribution" />
                                <cc1:Button ID="btnDelete" runat="server" Text="${Common.Button.Delete}" CssClass="apply"
                                    OnClick="btnDelete_Click" FunctionId="DeleteFacilityDistritbution" />
                                <cc1:Button ID="btnPurchaseComplete" runat="server" Text="${Common.Button.PurchaseComplete}"
                                    CssClass="apply" OnClick="btnPurchaseComplete_Click" FunctionId="PurchaseComplete" />
                                <cc1:Button ID="btnDistributionComplete" runat="server" Text="${Common.Button.DistributionComplete}"
                                    CssClass="apply" OnClick="btnDistributionComplete_Click" FunctionId="DistributionComplete" />
                                <cc1:Button ID="btnClose" runat="server" Text="${Common.Button.Close}" CssClass="apply"
                                    OnClick="btnClose_Click" FunctionId="DistributionClose" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_FacilityDistribution" runat="server" TypeName="com.Sconit.Web.FacilityDistributionMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityDistribution" UpdateMethod="UpdateFacilityDistribution"
    OnUpdated="ODS_FacilityDistribution_Updated" SelectMethod="LoadFacilityDistribution"
    OnUpdating="ODS_FacilityDistribution_Updating" DeleteMethod="DeleteFacilityDistribution"
    OnDeleted="ODS_FacilityDistribution_Deleted" OnDeleting="ODS_FacilityDistribution_Deleting">
    <SelectParameters>
        <asp:Parameter Name="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
