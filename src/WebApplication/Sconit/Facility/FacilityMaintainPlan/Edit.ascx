<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_MaintainPlan_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_MaintainPlan" runat="server" DataSourceID="ODS_MaintainPlan"
        DefaultMode="Edit" Width="100%" DataKeyNames="Id" OnDataBound="FV_MaintainPlan_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.MaintainPlan.UpdateMaintainPlan}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCode" runat="server" Text="${Facility.MaintainPlan.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblCode" runat="server" Text='<%# Eval("MaintainPlan.Code") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlDescription" runat="server" Text="${Facility.MaintainPlan.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbDescription" runat="server" Text='<%# Eval("MaintainPlan.Description") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlType" runat="server" Text="${Facility.MaintainPlan.Type}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbType" runat="server" Text='<%# Eval("MaintainPlan.Type") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlTypePeriod" runat="server" Text="${Facility.MaintainPlan.TypePeriod}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbTypePeriod" runat="server" Text='<%# Eval("MaintainPlan.TypePeriod") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlPeriod" runat="server" Text="${Facility.MaintainPlan.Period}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbPeriod" runat="server" Text='<%# Eval("MaintainPlan.Period") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlLeadTime" runat="server" Text="${Facility.MaintainPlan.LeadTime}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbLeadTime" runat="server" Text='<%# Eval("MaintainPlan.LeadTime") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlStartDate" runat="server" Text="${Facility.MaintainPlan.StartDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbStartDate" runat="server" Text='<%# Bind("StartDate") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlStartQty" runat="server" Text="${Facility.MaintainPlan.StartQty}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbStartQty" runat="server" Text='<%# Bind("StartQty","{0:0.##}") %>'  />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlNextMaintainDate" runat="server" Text="${Facility.MaintainPlan.NextMaintainDate}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbNextMaintainDate" runat="server" Text='<%# Eval("NextMaintainDate") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlNextMaintainQty" runat="server" Text="${Facility.MaintainPlan.NextMaintainQty}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbNextMaintainQty" runat="server" Text='<%# Eval("NextMaintainQty","{0:0.##}") %>' />
                        </td>
                    </tr>
                     <tr>
                      
                          <td class="td01">
                            <asp:Literal ID="ltlNextWarnDate" runat="server" Text="${Facility.MaintainPlan.NextWarnDate}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbNextWarnDate" runat="server" Text='<%# Eval("NextWarnDate") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlNextWarnQty" runat="server" Text="${Facility.MaintainPlan.NextWarnQty}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbNextWarnQty" runat="server" Text='<%# Eval("NextWarnQty","{0:0.##}") %>' />
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
                                    CssClass="edit" FunctionId="CreateFacility" />
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
<asp:ObjectDataSource ID="ODS_MaintainPlan" runat="server" TypeName="com.Sconit.Web.FacilityMaintainPlanMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityMaintainPlan" SelectMethod="LoadFacilityMaintainPlan"
    DeleteMethod="DeleteFacilityMaintainPlan" OnDeleted="ODS_FacilityMaintainPlan_Deleted"
    OnDeleting="ODS_FacilityMaintainPlan_Deleting" OnUpdating="ODS_FacilityMaintainPlan_Updating"
    OnUpdated="ODS_FacilityMaintainPlan_Updated" UpdateMethod="UpdateFacilityMaintainPlan">
    <SelectParameters>
        <asp:Parameter Name="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
