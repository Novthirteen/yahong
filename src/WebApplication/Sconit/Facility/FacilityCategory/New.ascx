<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_FacilityCategory_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Src="~/Controls/FileUpload.ascx" TagName="FileUpload" TagPrefix="uc3" %>
<div id="divFV">
    <asp:FormView ID="FV_FacilityCategory" runat="server" DataSourceID="ODS_FacilityCategory"
        DefaultMode="Insert" Width="100%" DataKeyNames="FCID">
        <InsertItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityCategory.NewFacilityCategory}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCode" runat="server" Text="${Facility.FacilityCategory.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvCode" runat="server" ErrorMessage="${Facility.FacilityCategory.Code}"
                                Display="Dynamic" ControlToValidate="tbCode" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvInsert" runat="server" ControlToValidate="tbCode" ErrorMessage="${Facility.FacilityCategory.Code}"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkFacilityCategoryExists" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlDescription" runat="server" Text="${Facility.FacilityCategory.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDescription" runat="server" Text='<%# Bind("Description") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlChargePerson" runat="server" Text="${Facility.FacilityCategory.ChargePerson}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbChargePerson" runat="server" Visible="true" Width="250" DescField="Name"
                                ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlChargeSite" runat="server" Text="${Facility.FacilityCategory.ChargeSite}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbChargeSite" runat="server" Text='<%# Bind("ChargeSite") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlChargeOrganization" runat="server" Text="${Facility.FacilityCategory.ChargeOrganization}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbChargeOrganization" runat="server" Text='<%# Bind("ChargeOrganization") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlParentCategory" runat="server" Text="${Facility.FacilityCategory.ParentCategory}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbParentCategory" runat="server" Visible="true" Width="250" DescField="Description"
                                ValueField="Code" ServicePath="FacilityCategoryMgr.service" ServiceMethod="GetAllFacilityCategory" />
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
<asp:ObjectDataSource ID="ODS_FacilityCategory" runat="server" TypeName="com.Sconit.Web.FacilityCategoryMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityCategory" InsertMethod="CreateFacilityCategory"
    OnInserted="ODS_FacilityCategory_Inserted" OnInserting="ODS_FacilityCategory_Inserting">
</asp:ObjectDataSource>
<asp:ObjectDataSource ID="ODS_ddlFacilityCategory" runat="server" TypeName="com.Sconit.Web.FacilityCategoryMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityCategory" SelectMethod="GetAllFacilityCategory">
</asp:ObjectDataSource>
