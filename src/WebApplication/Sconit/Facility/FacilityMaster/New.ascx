<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_FacilityMaster_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV">
    <asp:FormView ID="FV_FacilityMaster" runat="server" DataSourceID="ODS_FacilityMaster"
        DefaultMode="Insert" Width="100%" DataKeyNames="FCID">
        <InsertItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityMaster.NewFacilityMaster}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityMaster.FCID.Equipment}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbFCID" runat="server" Text='<%# Bind("FCID") %>' />
                               <asp:RequiredFieldValidator ID="rfvFCID" runat="server" ErrorMessage="${Facility.FacilityMaster.FCID.Equipment.Required}"
                                Display="Dynamic" ControlToValidate="tbFCID" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvInsert" runat="server" ControlToValidate="tbFCID" ErrorMessage="${Facility.FacilityMaster.FCID.Equipment.Exists}"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkFCIDExists" />
                        </td>
                      
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlName" runat="server" Text="${Facility.FacilityMaster.Name.Equipment}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbName" runat="server" Text='<%# Bind("Name") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityMaster.Category.Equipment}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbCategory" runat="server" Visible="true" Width="250" DescField="Description"
                                Text='<%# Bind("Category") %>' ValueField="Code" ServicePath="FacilityCategoryMgr.service"
                                ServiceMethod="GetAllEquipmentCategory" CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvCategory" runat="server" ErrorMessage="${Facility.FacilityMaster.Category.Required}"
                                Display="Dynamic" ControlToValidate="tbCategory" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSpecification" runat="server" Text="${Facility.FacilityMaster.Specification}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSpecification" runat="server" Text='<%# Bind("Specification") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlCapacity" runat="server" Text="${Facility.FacilityMaster.Capacity}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCapacity" runat="server" Text='<%# Bind("Capacity") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlManufactureDate" runat="server" Text="${Facility.FacilityMaster.ManufactureDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbManufactureDate" runat="server" Text='<%# Bind("ManufactureDate") %>'
                                onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlManufacturer" runat="server" Text="${Facility.FacilityMaster.Manufacturer}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbManufacturer" runat="server" Text='<%# Bind("Manufacturer") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSerialNo" runat="server" Text="${Facility.FacilityMaster.SerialNo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSerialNo" runat="server" Text='<%# Bind("SerialNo") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlAssetNo" runat="server" Text="${Facility.FacilityMaster.AssetNo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbAssetNo" runat="server" Text='<%# Bind("AssetNo") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlWarrantyInfo" runat="server" Text="${Facility.FacilityMaster.WarrantyInfo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbWarrantyInfo" runat="server" Text='<%# Bind("WarrantyInfo") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlTechInfo" runat="server" Text="${Facility.FacilityMaster.TechInfo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbTechInfo" runat="server" Text='<%# Bind("TechInfo") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplier" runat="server" Text="${Facility.FacilityMaster.Supplier}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplier" runat="server" Text='<%# Bind("Supplier") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierInfo" runat="server" Text="${Facility.FacilityMaster.SupplierInfo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierInfo" runat="server" Text='<%# Bind("SupplierInfo") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlPONo" runat="server" Text="${Facility.FacilityMaster.PONo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPONo" runat="server" Text='<%# Bind("PONo") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlEffDate" runat="server" Text="${Facility.FacilityMaster.EffDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbEffDate" runat="server" Text='<%# Bind("EffDate") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
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
                            <asp:Literal ID="ltlPrice" runat="server" Text="${Facility.FacilityMaster.Price}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPrice" runat="server" Text='<%# Bind("Price") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblRefenceCode" runat="server" Text="${Facility.FacilityMaster.RefenceCode}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbRefenceCode" runat="server" Text='<%# Bind("RefenceCode") %>' />
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
                            <asp:Literal ID="ltlCurrChargePerson" runat="server" Text="${Facility.FacilityMaster.CurrChargePerson}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbCurrChargePerson" runat="server" Visible="true" Width="250" DescField="Name"
                                MustMatch="true" ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser"
                                Text='<%# Bind("CurrChargePerson") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlChargeSite" runat="server" Text="${Facility.FacilityMaster.ChargeSite}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbChargeSite" runat="server" Visible="true" Width="250" DescField="ChargeSite"
                                ValueField="ChargeSite" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetFacilityChargeSite"
                                Text='<%# Bind("ChargeSite") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlChargeOrganization" runat="server" Text="${Facility.FacilityMaster.ChargeOrganization}:" />
                        </td>
                        <td class="td02">
                            <%-- <asp:TextBox ID="tbChargeOrganization" runat="server" Text='<%# Bind("ChargeOrganization") %>' />--%>
                            <uc3:textbox ID="tbChargeOrganization" runat="server" Visible="true" Width="250"
                                DescField="ChargeOrganization" ValueField="ChargeOrganization" ServicePath="FacilityMasterMgr.service"
                                ServiceMethod="GetFacilityChargeOrganization" Text='<%# Bind("ChargeOrganization") %>' />
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
                            <asp:Literal ID="ltlMaintainType" runat="server" Text="${Facility.FacilityMaster.MaintainType}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbMaintainType" runat="server" Visible="true" Width="250" DescField="MaintainType"
                                ValueField="MaintainType" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetMaintainTypeList"
                                Text='<%# Bind("MaintainType") %>' />
                        </td>
                         <td class="td01">
                        </td>
                        <td class="td02">
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
<asp:ObjectDataSource ID="ODS_FacilityMaster" runat="server" TypeName="com.Sconit.Web.FacilityMasterMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityMaster" InsertMethod="CreateFacilityMaster"
    OnInserted="ODS_FacilityMaster_Inserted" OnInserting="ODS_FacilityMaster_Inserting">
    <InsertParameters>
        <asp:Parameter Name="ManufactureDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="ChargeDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaintainStartDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaintainPeriod" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaintainLeadTime" Type="Int32" ConvertEmptyStringToNull="true" />
    </InsertParameters>
</asp:ObjectDataSource>
