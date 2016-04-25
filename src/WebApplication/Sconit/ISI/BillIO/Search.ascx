<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ISI_BillIO_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<fieldset>
    <table class="mtable">
        
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
                <asp:Literal ID="lblType" runat="server" Text="${PSI.BillIO.Type}:" />
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
            <td class="td01">
                <asp:Literal ID="ltlOrgType" runat="server" Text="${PSI.BillIO.OrgType}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlOrgType" Code="PSIBillDetailType" runat="server" IncludeBlankOption="true" DefaultSelectedValue="" />
            </td>
        </tr>
        <tr>
            <td colspan="3"></td>
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
