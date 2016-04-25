<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_FacilityItem_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV">
    <asp:FormView ID="FV_FacilityItem" runat="server" DataSourceID="ODS_FacilityItem"
        DefaultMode="Insert" Width="100%" DataKeyNames="FCID">
        <InsertItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityItem.NewFacilityItem}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityItem.FCID}:" />
                        </td>
                        <td class="td02">
                            <%--   <uc3:textbox ID="tbFCID" runat="server" Visible="true" Width="250" DescField="AssetNo"
                                ValueField="FCID" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetAllFacilityMaster" />--%>
                            <asp:TextBox ID="tbFCID" runat="server" Text='<%# Bind("FCID") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvFCID" runat="server" ErrorMessage="${Facility.FacilityItem.FCID.Required}"
                                Display="Dynamic" ControlToValidate="tbFCID" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvInsert" runat="server" ControlToValidate="tbFCID" ErrorMessage="${Facility.FacilityItem.Exists}"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkFacilityItemExists" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlItemCode" runat="server" Text="${Facility.FacilityItem.ItemCode}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbItemCode" runat="server" Visible="true" Width="250" DescField="Description"
                                ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem" CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvItemCode" runat="server" ErrorMessage="${Facility.FacilityItem.ItemCode.Required}"
                                Display="Dynamic" ControlToValidate="tbItemCode" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsActive" runat="server" Text="${Facility.FacilityItem.IsActive}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlAllocateType" runat="server" Text="${Facility.FacilityItem.AllocateType}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlAllocateType" Code="FacilityAllocateType" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlQty" runat="server" Text="${Facility.FacilityItem.Qty}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbQty" runat="server" Text='<%# Bind("Qty") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvQty" runat="server" ErrorMessage="${Facility.FacilityItem.Qty.Required}"
                                Display="Dynamic" ControlToValidate="tbQty" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblAmount" runat="server" Text="${Facility.FacilityItem.Amount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbAmount" runat="server" Text='<%# Bind("Amount") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvAmount" runat="server" ErrorMessage="${Facility.FacilityItem.Amount.Required}"
                                Display="Dynamic" ControlToValidate="tbAmount" ValidationGroup="vgSave" />
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
                            <asp:Literal ID="ltlWarnRate" runat="server" Text="${Facility.FacilityItem.WarnRate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbWarnRate" runat="server" Text='<%# Bind("WarnRate") %>' CssClass="inputRequired" />%
                            <asp:RequiredFieldValidator ID="rfvWarnRate" runat="server" ErrorMessage="${Facility.FacilityItem.WarnRate.Required}"
                                Display="Dynamic" ControlToValidate="tbWarnRate" ValidationGroup="vgSave" />
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
<asp:ObjectDataSource ID="ODS_FacilityItem" runat="server" TypeName="com.Sconit.Web.FacilityItemMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityItem" InsertMethod="CreateFacilityItem"
    OnInserted="ODS_FacilityItem_Inserted" OnInserting="ODS_FacilityItem_Inserting"></asp:ObjectDataSource>
