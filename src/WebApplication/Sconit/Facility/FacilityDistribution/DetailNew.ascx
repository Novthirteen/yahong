﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DetailNew.ascx.cs" Inherits="Facility_FacilityDistributionDetail_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV">
    <asp:FormView ID="FV_FacilityDistributionDetail" runat="server" DataSourceID="ODS_FacilityDistributionDetail"
        DefaultMode="Insert" Width="100%" DataKeyNames="FCID">
        <InsertItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityDistributionDetail.NewFacilityDistributionDetail}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlInvoice" runat="server" Text="${Facility.FacilityDistributionDetail.Invoice}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbInvoice" runat="server" Text='<%# Bind("Invoice") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlType" runat="server" Text="${Facility.FacilityDistributionDetail.Type}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlType" Code="FacilityAllocateType" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlBillDate" runat="server" Text="${Facility.FacilityDistributionDetail.BillDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbBillDate" runat="server" Text='<%# Bind("BillDate") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPayDate" runat="server" Text="${Facility.FacilityDistributionDetail.PayDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPayDate" runat="server" Text='<%# Bind("PayDate") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlBillAmount" runat="server" Text="${Facility.FacilityDistributionDetail.BillAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbBillAmount" runat="server" Text='<%# Bind("BillAmount","0.###") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPayAmount" runat="server" Text="${Facility.FacilityDistributionDetail.PayAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPayAmount" runat="server" Text='<%# Bind("PayAmount") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlContact" runat="server" Text="${Facility.FacilityDistributionDetail.Contact}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbContact" runat="server" Text='<%# Bind("Contact") %>' />
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
                                <cc1:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" FunctionId="CreateFacilityDistributionDetail"  />
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
<asp:ObjectDataSource ID="ODS_FacilityDistributionDetail" runat="server" TypeName="com.Sconit.Web.FacilityDistributionDetailMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityDistributionDetail" InsertMethod="CreateFacilityDistributionDetail"
    OnInserted="ODS_FacilityDistributionDetail_Inserted" OnInserting="ODS_FacilityDistributionDetail_Inserting">
    <InsertParameters>
        <asp:Parameter Name="PayAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="BillAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="PayDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="BillDate" Type="DateTime" ConvertEmptyStringToNull="true" />
    </InsertParameters>
</asp:ObjectDataSource>
