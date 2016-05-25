<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_FacilityAllocate_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.ui-zendesk.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<script language="javascript" type="text/javascript" src="Js/tag-it.js"></script>

<div id="divFV" runat="server">
    <asp:FormView ID="FV_FacilityAllocate" runat="server" DataSourceID="ODS_FacilityAllocate"
        DefaultMode="Edit" Width="100%" DataKeyNames="Id" OnDataBound="FV_FacilityAllocate_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityAllocate.UpdateFacilityAllocate}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityAllocate.FCID}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblFCID" runat="server" />
                            <asp:HiddenField ID="hfCreateDate" Value='<%# Bind("CreateDate") %>' runat="server" />
                            <asp:HiddenField ID="hfCreateUser" Value='<%# Bind("CreateUser") %>' runat="server" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlItemCode" runat="server" Text="${Facility.FacilityAllocate.ItemCode}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblItemCode" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsActive" runat="server" Text="${Facility.FacilityAllocate.IsActive}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlAllocateType" runat="server" Text="${Facility.FacilityAllocate.AllocateType}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="ddlAllocateType" runat="server" />
                            <asp:HiddenField ID="hfAllocateType" Value='<%# Bind("AllocateType") %>' runat="server" />
                        </td>
                    </tr>
                     <tr>
                       <td class="td01">
                            <asp:Literal ID="ltlMouldCount" runat="server" Text="${Facility.FacilityAllocate.MouldCount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbMouldCount" runat="server" Text='<%# Bind("MouldCount","{0:0.##}") %>' />
                            <asp:RequiredFieldValidator ID="rfvMouldCount" runat="server" ErrorMessage="${Facility.FacilityAllocate.MouldCount.Required}"
                                Display="Dynamic" ControlToValidate="tbMouldCount" ValidationGroup="vgSave" />
                        </td>
                          <td class="td01">
                            <asp:Literal ID="ltlGroupName" runat="server" Text="${Facility.FacilityAllocate.GroupName}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbGroupName" runat="server" Text='<%# Bind("GroupName") %>' />
                            
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
<asp:ObjectDataSource ID="ODS_FacilityAllocate" runat="server" TypeName="com.Sconit.Web.FacilityAllocateMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityAllocate" UpdateMethod="UpdateFacilityAllocate"
    OnUpdated="ODS_FacilityAllocate_Updated" SelectMethod="LoadFacilityAllocate"
    OnUpdating="ODS_FacilityAllocate_Updating" DeleteMethod="DeleteFacilityAllocate"
    OnDeleted="ODS_FacilityAllocate_Deleted" OnDeleting="ODS_FacilityAllocate_Deleting">
    <SelectParameters>
        <asp:Parameter Name="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
