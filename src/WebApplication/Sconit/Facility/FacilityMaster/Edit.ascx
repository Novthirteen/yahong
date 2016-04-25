<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_FacilityMaster_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_FacilityMaster" runat="server" DataSourceID="ODS_FacilityMaster"
        DefaultMode="Edit" Width="100%" DataKeyNames="FCID" OnDataBound="FV_FacilityMaster_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityMaster.UpdateFacilityMaster}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityMaster.FCID}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblFCID" runat="server" Text='<%# Bind("FCID") %>' />
                            <asp:HiddenField ID="hfCreateDate" Value='<%# Bind("CreateDate") %>' runat="server" />
                            <asp:HiddenField ID="hfCreateUser" Value='<%# Bind("CreateUser") %>' runat="server" />
                            <asp:HiddenField ID="hfMaintainStartDate" Value='<%# Bind("MaintainStartDate") %>'
                                runat="server" />
                            <asp:HiddenField ID="hfMaintainPeriod" Value='<%# Bind("MaintainPeriod") %>' runat="server" />
                            <asp:HiddenField ID="hfMaintainLeadTime" Value='<%# Bind("MaintainLeadTime") %>'
                                runat="server" />
                            <asp:HiddenField ID="hfMaintainType" Value='<%# Bind("MaintainType") %>' runat="server" />
                            <asp:HiddenField ID="hfMaintainTypePeriod" Value='<%# Bind("MaintainTypePeriod") %>'
                                runat="server" />
                            <asp:HiddenField ID="hfIsInstore" Value='<%# Bind("IsInstore") %>' runat="server" />
                            <asp:HiddenField ID="hfStatus" runat="server" Value='<%# Bind("Status") %>' />
                            <asp:HiddenField ID="hfOldChargePerson" runat="server" Value='<%# Bind("OldChargePerson") %>' />
                            <asp:HiddenField ID="hfChargeDate" runat="server" Value='<%# Bind("ChargeDate") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlName" runat="server" Text="${Facility.FacilityMaster.Name}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbName" runat="server" Text='<%# Bind("Name") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityMaster.Category}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbCategory" runat="server" Visible="true" Width="250" DescField="Description"
                                Text='<%# Bind("Category") %>' ValueField="Code" ServicePath="FacilityCategoryMgr.service"
                                ServiceMethod="GetAllFacilityCategory" CssClass="inputRequired" ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSpecification" runat="server" Text="${Facility.FacilityMaster.Specification}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSpecification" runat="server" Text='<%# Bind("Specification") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCapacity" runat="server" Text="${Facility.FacilityMaster.Capacity}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCapacity" runat="server" Text='<%# Bind("Capacity") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlManufactureDate" runat="server" Text="${Facility.FacilityMaster.ManufactureDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbManufactureDate" runat="server" Text='<%# Bind("ManufactureDate") %>'
                                onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlManufacturer" runat="server" Text="${Facility.FacilityMaster.Manufacturer}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbManufacturer" runat="server" Text='<%# Bind("Manufacturer") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSerialNo" runat="server" Text="${Facility.FacilityMaster.SerialNo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSerialNo" runat="server" Text='<%# Bind("SerialNo") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlAssetNo" runat="server" Text="${Facility.FacilityMaster.AssetNo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbAssetNo" runat="server" Text='<%# Bind("AssetNo") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlWarrantyInfo" runat="server" Text="${Facility.FacilityMaster.WarrantyInfo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbWarrantyInfo" runat="server" Text='<%# Bind("WarrantyInfo") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlTechInfo" runat="server" Text="${Facility.FacilityMaster.TechInfo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbTechInfo" runat="server" Text='<%# Bind("TechInfo") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplier" runat="server" Text="${Facility.FacilityMaster.Supplier}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplier" runat="server" Text='<%# Bind("Supplier") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierInfo" runat="server" Text="${Facility.FacilityMaster.SupplierInfo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierInfo" runat="server" Text='<%# Bind("SupplierInfo") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPONo" runat="server" Text="${Facility.FacilityMaster.PONo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPONo" runat="server" Text='<%# Bind("PONo") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlEffDate" runat="server" Text="${Facility.FacilityMaster.EffDate}:" />
                        </td>
                        <td class="td02">
                            <%-- <asp:TextBox ID="tbEffDate" runat="server" Text='<%# Bind("EffDate") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />--%>
                            <asp:TextBox ID="tbEffDate" runat="server" Text='<%# Bind("EffDate") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPrice" runat="server" Text="${Facility.FacilityMaster.Price}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPrice" runat="server" Text='<%# Bind("Price") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlOwner" runat="server" Text="${Facility.FacilityMaster.Owner}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlOwner" Code="FacilityOwner" runat="server" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlOwnerDescription" runat="server" Text="${Facility.FacilityMaster.OwnerDescription}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbOwnerDescription" runat="server" Text='<%# Bind("OwnerDescription") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCurrChargePerson" runat="server" Text="${Facility.FacilityMaster.CurrChargePerson}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCurrChargePerson" runat="server" Text='<%# Bind("CurrChargePersonName") %>'
                                ReadOnly="true" />
                            <asp:HiddenField ID="hfCurrChargePerson" runat="server" Value='<%# Bind("CurrChargePerson") %>' />
                            <asp:HiddenField ID="hfCurrChargePersonName" runat="server" Value='<%# Bind("CurrChargePersonName") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlChargeSite" runat="server" Text="${Facility.FacilityMaster.ChargeSite}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbChargeSite" runat="server" Text='<%# Bind("ChargeSite") %>' ReadOnly="true" />
                            <asp:HiddenField ID="hfChargeSite" runat="server" Value='<%# Bind("ChargeSite") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlChargeOrganization" runat="server" Text="${Facility.FacilityMaster.ChargeOrganization}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbChargeOrganization" runat="server" Text='<%# Bind("ChargeOrganization") %>'
                                ReadOnly="true" />
                            <asp:HiddenField ID="hfChargeOrganization" runat="server" Value='<%# Bind("ChargeOrganization") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblStatus" runat="server" Text="${Facility.FacilityMaster.Status}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbStatus" runat="server" Text='<%# Bind("Status") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lbReferenceNo" runat="server" Text="${Facility.FacilityMaster.RefenceCode}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbRefenceCode" runat="server" Text='<%# Bind("RefenceCode") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlMaintainGroup" runat="server" Text="${Facility.FacilityMaster.MaintainGroup}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbMaintainGroup" runat="server" Visible="true" Width="250" DescField="MaintainGroup"
                                ValueField="MaintainGroup" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetMaintainGroupList"
                                Text='<%# Bind("MaintainGroup") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsOffBalance" runat="server" Text="${Facility.FacilityMaster.IsOffBalance}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsOffBalance" runat="server" Checked='<%# Bind("IsOffBalance") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsAsset" runat="server" Text="${Facility.FacilityMaster.IsAsset}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsAsset" runat="server" Checked='<%# Bind("IsAsset") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlMaintainTypePeriod" runat="server" Text="${Facility.FacilityMaster.MaintainTypePeriod}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbMaintainTypePeriod" runat="server" Text='<%# Bind("MaintainTypePeriod") %>'
                                ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlMaintainType" runat="server" Text="${Facility.FacilityMaster.MaintainType}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbMaintainType" runat="server" Visible="true" Width="250" DescField="MaintainType"
                                ValueField="MaintainType" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetMaintainTypeList"
                                Text='<%# Bind("MaintainType") %>' />
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
                        <td class="td01">
                        </td>
                        <td class="td02">
                        </td>
                        <td class="td01">
                        </td>
                        <td class="td02">
                            <div class="buttons">
                                <cc1:Button ID="btnEdit" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" FunctionId="CreateFacility" />
                                <cc1:Button ID="btnAvailable" runat="server" Text="${Common.Button.Available}" CssClass="button2"
                                    OnClick="btnAvailable_Click" />
                                <cc1:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
                                    CssClass="button2" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                                <cc1:Button ID="btnPrint" runat="server" Text="${Common.Button.Print}" CssClass="button2"
                                    OnClick="btnPrint_Click" />
                                <cc1:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_FacilityMaster" runat="server" TypeName="com.Sconit.Web.FacilityMasterMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityMaster" UpdateMethod="UpdateFacilityMaster"
    OnUpdated="ODS_FacilityMaster_Updated" SelectMethod="LoadFacilityMaster" OnUpdating="ODS_FacilityMaster_Updating"
    DeleteMethod="DeleteFacilityMaster" OnDeleted="ODS_FacilityMaster_Deleted" OnDeleting="ODS_FacilityMaster_Deleting">
    <SelectParameters>
        <asp:Parameter Name="fcId" Type="String" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Name="ManufactureDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="ChargeDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaintainStartDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaintainPeriod" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaintainLeadTime" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="NextMaintainTime" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaintainTypePeriod" Type="Int32" ConvertEmptyStringToNull="true" />
    </UpdateParameters>
</asp:ObjectDataSource>
