<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_FacilityStock_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<div id="divFV" runat="server">
    <fieldset>
        <table class="mtable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblChargeOrg" runat="server" Text="${Facility.FacilityStock.ChargeOrg}:" />
                </td>
                <td class="td02">
                  <ct:ASDropDownTreeView ID="astvChargeOrganization" runat="server" BasePath="~/Js/astreeview/"
                        DataTableRootNodeValue="0" EnableRoot="false" EnableNodeSelection="false" EnableCheckbox="true"
                        EnableDragDrop="false" EnableTreeLines="false" EnableNodeIcon="false" EnableCustomizedNodeIcon="false"
                        EnableDebugMode="false" EnableRequiredValidator="false" InitialDropdownText=""
                        Width="170" EnableCloseOnOutsideClick="true" EnableHalfCheckedAsChecked="true"
                        DropdownIconDown="~/Js/astreeview/images/windropdown.gif" EnableContextMenuAdd="false"
                        MaxDropdownHeight="200" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblChargeSite" runat="server" Text="${Facility.FacilityStock.ChargeSite}:" />
                </td>
                <td class="td02">
                    <ct:ASDropDownTreeView ID="astvChargeSite" runat="server" BasePath="~/Js/astreeview/"
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
                    <asp:Literal ID="lblChargePerson" runat="server" Text="${Facility.FacilityStock.ChargePerson}:" />
                </td>
                <td class="td02">
                    <ct:ASDropDownTreeView ID="astvChargePerson" runat="server" BasePath="~/Js/astreeview/"
                        DataTableRootNodeValue="0" EnableRoot="false" EnableNodeSelection="false" EnableCheckbox="true"
                        EnableDragDrop="false" EnableTreeLines="false" EnableNodeIcon="false" EnableCustomizedNodeIcon="false"
                        EnableDebugMode="false" EnableRequiredValidator="false" InitialDropdownText=""
                        Width="170" EnableCloseOnOutsideClick="true" EnableHalfCheckedAsChecked="true"
                        DropdownIconDown="~/Js/astreeview/images/windropdown.gif" EnableContextMenuAdd="false"
                        MaxDropdownHeight="200" />
                </td>
                <td class="td01">
                    <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityStock.FacilityCategory}:" />
                </td>
                <td class="td02">
                    <ct:ASDropDownTreeView ID="astvFacilityCategory" runat="server" BasePath="~/Js/astreeview/"
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
                    <asp:Literal ID="lblEffDate" runat="server" Text="${Facility.FacilityStock.EffDate}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbEffDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                </td>
               <td class="td01">
                    <asp:Literal ID="lblAssetNo" runat="server" Text="${Facility.FacilityStock.AssetNo}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbAssetNo" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="3" />
                <td class="td02">
                    <asp:Button ID="btnSave" runat="server" Text="${Common.Button.Save}" OnClick="btnSave_Click"
                        Width="59px" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        Width="59px" />
                </td>
            </tr>
        </table>
    </fieldset>
</div>
