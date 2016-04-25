<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_FacilityCategory_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_FacilityCategory" runat="server" DataSourceID="ODS_FacilityCategory"
        DefaultMode="Edit" Width="100%" DataKeyNames="Code" OnDataBound="FV_FacilityCategory_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityCategory.UpdateFacilityCategory}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCode" runat="server" Text="${Facility.FacilityCategory.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblCode" runat="server" Text='<%# Bind("Code") %>' />
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
                                ValueField="Code" ServicePath="FacilityCategoryMgr.service" ServiceMethod="GetAllFacilityCategory"
                                />
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
                                <cc1:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" FunctionId="CreateFacility" />
                                <cc1:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
                                    CssClass="delete" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')"
                                    FunctionId="CreateFacility" />
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
<asp:ObjectDataSource ID="ODS_FacilityCategory" runat="server" TypeName="com.Sconit.Web.FacilityCategoryMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityCategory" UpdateMethod="UpdateFacilityCategory"
    OnUpdated="ODS_FacilityCategory_Updated" SelectMethod="LoadFacilityCategory"
    OnUpdating="ODS_FacilityCategory_Updating" DeleteMethod="DeleteFacilityCategory"
    OnDeleted="ODS_FacilityCategory_Deleted" OnDeleting="ODS_FacilityCategory_Deleting">
    <SelectParameters>
        <asp:Parameter Name="code" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
