<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_Bill_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlCode" runat="server" Text="${PSI.Bill.Code}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbCode" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlDesc1" runat="server" Text="${Common.Business.Description}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbDesc1" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlFCID" runat="server" Text="${PSI.Bill.FCID}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbFCID" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlStatus" runat="server" Text="${PSI.Bill.Status}:" />
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
            <td class="td01">
                <asp:Literal ID="ltlPrjCode" runat="server" Text="${PSI.Bill.PrjCode}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbPrjCode" runat="server" Visible="true" Width="250" DescField="Desc"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetProjectTaskSubType" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblMouldUser" runat="server" Text="${PSI.Bill.MouldUser}:" />
            </td>
            <td class="td02">
                 <uc3:textbox ID="tbMouldUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlCustomer" runat="server" Text="${PSI.Bill.Customer}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbCustomer" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" ServicePath="CustomerMgr.service" ServiceMethod="GetAllCustomer" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlSupplier" runat="server" Text="${PSI.Bill.Supplier}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbSupplier" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" ServicePath="SupplierMgr.service" ServiceMethod="GetAllSupplier" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlSOContractNo" runat="server" Text="${PSI.Bill.SOContractNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbSOContractNo" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlSupplierContractNo" runat="server" Text="${PSI.Bill.SupplierContractNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbSupplierContractNo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlSOUser" runat="server" Text="${PSI.Bill.SOUser}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbSOUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlPOUser" runat="server" Text="${PSI.Bill.POUser}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbPOUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
        </tr>
        <tr>
            <td colspan="3"></td>
            <td class="td02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <cc1:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
                        CssClass="add" FunctionId="SavePSIBill" />
                    <cc1:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click"
                        CssClass="query" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
