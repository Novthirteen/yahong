<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Facility_FacilityDistribution_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
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
                <asp:Literal ID="ltlCode" runat="server" Text="${Facility.FacilityDistribution.Code}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbCode" runat="server" />
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
                <asp:Literal ID="ltlStatus" runat="server" Text="${Facility.FacilityDistribution.Status}:" />
            </td>
            <td class="td02">
                <ct:ASDropDownTreeView ID="astvMyTree" runat="server" BasePath="~/Js/astreeview/"
                    DataTableRootNodeValue="0" EnableRoot="false" EnableNodeSelection="false" EnableCheckbox="true"
                    EnableDragDrop="false" EnableTreeLines="false" EnableNodeIcon="false" EnableCustomizedNodeIcon="false"
                    EnableDebugMode="false" EnableRequiredValidator="false" InitialDropdownText=""
                    Width="170" EnableCloseOnOutsideClick="true" EnableHalfCheckedAsChecked="true"
                    DropdownIconDown="~/Js/astreeview/images/windropdown.gif" EnableContextMenuAdd="false"
                    MaxDropdownHeight="200" />
            </td>

        </tr>
        <tr>
            <td colspan="3"></td>
            <td class="td02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <cc1:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
                        CssClass="add" FunctionId="CreateFacility" />
                    <cc1:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click"
                        CssClass="query" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
