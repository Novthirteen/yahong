﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_FacilitySell_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_FacilityMaster" runat="server" DataSourceID="ODS_FacilityMaster"
        DefaultMode="Edit" Width="100%" DataKeyNames="FcId" OnDataBound="FV_FacilityMaster_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityApply.Apply}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityMaster.FCID}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblFCID" runat="server" Text='<%# Bind("FCID") %>' />
                            <td class="td01">
                                <asp:Literal ID="ltlName" runat="server" Text="${Facility.FacilityMaster.Name}:" />
                            </td>
                            <td class="td02">
                                <asp:Literal ID="tbName" runat="server" Text='<%# Bind("Name") %>' />
                            </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSpecification" runat="server" Text="${Facility.FacilityMaster.Specification}:" />
                        </td>
                        <td class="td02">
                            <asp:Literal ID="tbSpecification" runat="server" Text='<%# Bind("Specification") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlCapacity" runat="server" Text="${Facility.FacilityMaster.Capacity}:" />
                        </td>
                        <td class="td02">
                            <asp:Literal ID="tbCapacity" runat="server" Text='<%# Bind("Capacity") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSellPerson" runat="server" Text="${Facility.FacilitySell.SellPerson}:" />
                            
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSellPerson" runat="server" Text='' />
                            <asp:RequiredFieldValidator ID="rfvSellPerson" runat="server" ErrorMessage="${Facility.FacilitySell.SellPerson.Required}"
                                Display="Dynamic" ControlToValidate="tbSellPerson" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSellSite" runat="server" Text="${Facility.FacilitySell.SellSite}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSellSite" runat="server" Text='' />
                            <asp:RequiredFieldValidator ID="rfvSellSite" runat="server" ErrorMessage="${Facility.FacilitySell.SellSite.Required}"
                                Display="Dynamic" ControlToValidate="tbSellSite" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSellOrg" runat="server" Text="${Facility.FacilitySell.SellOrg}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSellOrg" runat="server" Text='' />
                            <asp:RequiredFieldValidator ID="rfvSellOrg" runat="server" ErrorMessage="${Facility.FacilitySell.SellOrg.Required}"
                                Display="Dynamic" ControlToValidate="tbSellOrg" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                   <%-- <tr style=" display:none">
                        <td class="td01">
                            <asp:Literal ID="ltlManufactureDate" runat="server" Text="${Facility.FacilityMaster.ManufactureDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbManufactureDate" runat="server" Text='<%# Bind("ManufactureDate") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlManufacturer" runat="server" Text="${Facility.FacilityMaster.Manufacturer}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbManufacturer" runat="server" Text='<%# Bind("Manufacturer") %>' />
                        </td>
                    </tr>
                    <tr style=" display:none">
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
                    <tr style=" display:none">
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
                    <tr style=" display:none">
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
                    <tr style=" display:none">
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
                            <asp:TextBox ID="tbEffDate" runat="server" Text='<%# Bind("EffDate") %>' />
                        </td>
                    </tr>
                    <tr style=" display:none">
                        <td class="td01">
                            <asp:Literal ID="ltlPrice" runat="server" Text="${Facility.FacilityMaster.Price}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPrice" runat="server" Text='<%# Bind("Price") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlRemark" runat="server" Text="${Facility.FacilityMaster.Remark}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbRemark" runat="server" Text='<%# Bind("Remark") %>' />
                        </td>
                    </tr>
                    <tr style=" display:none">
                        <td class="td01">
                            <asp:Literal ID="ltlOwner" runat="server" Text="${Facility.FacilityMaster.Owner}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbOwner" runat="server" Text='<%# Bind("Owner") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlOwnerDescription" runat="server" Text="${Facility.FacilityMaster.OwnerDescription}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbOwnerDescription" runat="server" Text='<%# Bind("OwnerDescription") %>' />
                        </td>
                    </tr>
                    <tr style=" display:none">
                        <td class="td01">
                            <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityMaster.Category}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCategory" runat="server" Text='<%# Bind("Category") %>' />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCreateDate" runat="server" Text='<%# Bind("CreateDate") %>' />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCreateUser" runat="server" Text='<%# Bind("CreateUser") %>' />
                        </td>
                    </tr>--%>
                    <tr>
                        <td class="td01">
                        </td>
                        <td class="td02">
                        </td>
                        <td class="td01">
                        </td>
                        <td class="td02">
                            <div class="buttons">
                                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="${Common.Button.SellFacility}"
                                    CssClass="apply" ValidationGroup="vgSave" />
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
<asp:ObjectDataSource ID="ODS_FacilityMaster" runat="server" TypeName="com.Sconit.Web.FacilityMasterMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityMaster" UpdateMethod="UpdateFacilityMaster"
    OnUpdated="ODS_FacilityMaster_Updated" SelectMethod="LoadFacilityMaster" OnUpdating="ODS_FacilityMaster_Updating"
    DeleteMethod="DeleteFacilityMaster" OnDeleted="ODS_FacilityMaster_Deleted" OnDeleting="ODS_FacilityMaster_Deleting">
    <SelectParameters>
        <asp:Parameter Name="fcId" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
<asp:ObjectDataSource ID="ODS_ddlCategory" runat="server" TypeName="com.Sconit.Web.CodeMasterMgrProxy"
    DataObjectTypeName="com.Sconit.Entity.MasterData.CodeMaster" SelectMethod="GetCachedCodeMaster">
    <SelectParameters>
        <asp:QueryStringParameter Name="code" DefaultValue="FacilityOwner" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
