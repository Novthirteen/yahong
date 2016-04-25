<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_FacilityItem_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_FacilityItem" runat="server" DataSourceID="ODS_FacilityItem"
        DefaultMode="Edit" Width="100%" DataKeyNames="Id" OnDataBound="FV_FacilityItem_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityItem.UpdateFacilityItem}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityItem.FCID}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblFCID" runat="server" />
                            <asp:HiddenField ID="hfCreateDate" Value='<%# Bind("CreateDate") %>' runat="server" />
                            <asp:HiddenField ID="hfCreateUser" Value='<%# Bind("CreateUser") %>' runat="server" />
                            <asp:HiddenField ID="hfAllocateType" Value='<%# Bind("AllocateType") %>' runat="server" />
                            <asp:HiddenField ID="hfIsAllocate" Value='<%# Bind("IsAllocate") %>' runat="server" />
                            <asp:HiddenField ID="hfPassRate" Value='<%# Bind("PassRate") %>' runat="server" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlItemCode" runat="server" Text="${Facility.FacilityItem.ItemCode}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblItemCode" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <%--  <td class="td01">
                            <asp:Literal ID="lblIsAllocate" runat="server" Text="${Facility.FacilityItem.IsAllocate}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsAllocate" runat="server" Checked='<%# Bind("IsAllocate") %>' />
                        </td>--%>
                        <td class="td01">
                            <asp:Literal ID="ltlAllocateType" runat="server" Text="${Facility.FacilityItem.AllocateType}:" />
                        </td>
                        <td class="td02">
                            <%--  <cc1:CodeMstrDropDownList ID="ddlAllocateType" Code="FacilityAllocateType" runat="server" />--%>
                            <asp:Label ID="ddlAllocateType" runat="server" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsActive" runat="server" Text="${Facility.FacilityItem.IsActive}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlQty" runat="server" Text="${Facility.FacilityItem.Qty}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbQty" runat="server" Text='<%# Bind("Qty","{0:0.##}") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblAmount" runat="server" Text="${Facility.FacilityItem.Amount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbAmount" runat="server" Text='<%# Bind("Amount","{0:0.##}") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlInitQty" runat="server" Text="${Facility.FacilityItem.InitQty}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbInitQty" runat="server" Text='<%# Bind("InitQty","{0:0.##}") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlAllocatedQty" runat="server" Text="${Facility.FacilityItem.AllocatedQty}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbAllocatedQty" runat="server" Text='<%# Bind("AllocatedQty","{0:0.##}") %>' ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSingleQty" runat="server" Text="${Facility.FacilityItem.SingleQty}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSingleQty" runat="server" Text='<%# Bind("SingleQty","{0:0.##}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlAllocatedAmount" runat="server" Text="${Facility.FacilityItem.AllocatedAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbAllocatedAmount" runat="server" Text='<%# Bind("AllocatedAmount","{0:0.##}") %>'
                                ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlWarnRate" runat="server" Text="${Facility.FacilityItem.WarnRate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbWarnRate" runat="server" Text='<%# Bind("WarnRate","{0:0.##}") %>' />%
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsWarn" runat="server" Text="${Facility.FacilityItem.IsWarn}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsWarn" runat="server" Checked='<%# Bind("IsWarn") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlRemark" runat="server" Text="${Facility.FacilityItem.Remark}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbRemark" runat="server" Text='<%# Bind("Remark") %>' TextMode="MultiLine" Height="40" Width="77%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCreateDate" runat="server" Text="${Common.Business.CreateDate}:" />
                        </td>
                        <td class="td02">
                            <cc1:ReadonlyTextBox ID="tbCreateDate" runat="server" CodeField="CreateDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblCreateUser" runat="server" Text="${Common.Business.CreateUser}:" />
                        </td>
                        <td class="td02">
                            <cc1:ReadonlyTextBox ID="tbCreateUser" runat="server" CodeField="CreateUser" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblLastModifyDate" runat="server" Text="${Common.Business.LastModifyDate}:" />
                        </td>
                        <td class="td02">
                            <cc1:ReadonlyTextBox ID="tbLastModifyDate" runat="server" CodeField="LastModifyDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblLastModifyUser" runat="server" Text="${Common.Business.LastModifyUser}:" />
                        </td>
                        <td class="td02">
                            <cc1:ReadonlyTextBox ID="tbLastModifyUser" runat="server" CodeField="LastModifyUser" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02"></td>
                        <td class="td01"></td>
                        <td class="td02">
                            <div class="buttons">
                                <cc1:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" FunctionId="CreateFacility" />
                                <%--   <cc1:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
                                    CssClass="delete" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')"
                                    FunctionId="CreateFacility" />--%>
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
<asp:ObjectDataSource ID="ODS_FacilityItem" runat="server" TypeName="com.Sconit.Web.FacilityItemMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityItem" UpdateMethod="UpdateFacilityItem"
    OnUpdated="ODS_FacilityItem_Updated" SelectMethod="LoadFacilityItem" OnUpdating="ODS_FacilityItem_Updating"
    DeleteMethod="DeleteFacilityItem" OnDeleted="ODS_FacilityItem_Deleted" OnDeleting="ODS_FacilityItem_Deleting">
    <SelectParameters>
        <asp:Parameter Name="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
