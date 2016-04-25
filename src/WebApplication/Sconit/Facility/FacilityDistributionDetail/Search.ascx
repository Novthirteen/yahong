<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Facility_FacilityDistributionDetail_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityDistribution.FCID}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbFCID" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlType" runat="server" Text="${Facility.FacilityDistributionDetail.Type}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlType" Code="FacilityAllocateType" runat="server" IncludeBlankOption="true" DefaultSelectedValue=""/>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlSupplierName" runat="server" Text="${Facility.FacilityDistribution.SupplierName}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbSupplierName" runat="server" Visible="true" Width="250" DescField="PurchaseContractCode"
                    ValueField="SupplierName" ServicePath="FacilityDistributionMgr.service" ServiceMethod="GetFacilityDistributionSupplier" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlCustomerName" runat="server" Text="${Facility.FacilityDistribution.CustomerName}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbCustomerName" runat="server" Visible="true" Width="250" DescField="DistributionContractCode"
                    ValueField="CustomerName" ServicePath="FacilityDistributionMgr.service" ServiceMethod="GetFacilityDistributionCustomer" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlPurchaseContractCode" runat="server" Text="${Facility.FacilityDistribution.PurchaseContractCode}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbPurchaseContractCode" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlDistributionContractCode" runat="server" Text="${Facility.FacilityDistribution.DistributionContractCode}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbDistributionContractCode" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblBillStartDate" runat="server" Text="${Facility.FacilityDistribution.BillStartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbBillStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblBillEndDate" runat="server" Text="${Facility.FacilityDistribution.BillEndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbBillEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlPayStartDate" runat="server" Text="${Facility.FacilityDistribution.PayStartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbPayStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlPayEndDate" runat="server" Text="${Facility.FacilityDistribution.PayEndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbPayEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
            </td>
            <td class="td02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click"
                        CssClass="query" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
