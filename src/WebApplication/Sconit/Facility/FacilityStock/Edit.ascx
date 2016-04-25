<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Inventory_Stocktaking_Edit" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<div id="divFV" runat="server">
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStNo" runat="server" Text="${Facility.FacilityStock.StNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStNo" runat="server" ReadOnly="true" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblStatus" runat="server" Text="${Facility.FacilityStock.Status}:" />
            </td>
            <td class="td02">
                <asp:Label ID="tbStatus" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblChargeOrg" runat="server" Text="${Facility.FacilityStock.ChargeOrg}:" />
            </td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbChargeOrganization" runat="server" TextMode="MultiLine" Height="50"
                    Width="100%" ReadOnly="true" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblChargeSite" runat="server" Text="${Facility.FacilityStock.ChargeSite}:" />
            </td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbChargeSite" runat="server" TextMode="MultiLine" Height="50" Width="100%"
                    ReadOnly="true" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblChargePerson" runat="server" Text="${Facility.FacilityStock.ChargePerson}:" />
            </td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbChargePerson" runat="server" TextMode="MultiLine" Height="50"
                    Width="100%" ReadOnly="true" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityStock.FacilityCategory}:" />
            </td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbCategory" runat="server" TextMode="MultiLine" Height="50" Width="100%"
                    ReadOnly="true" />
            </td>
        </tr>
        <tr>
         <td class="td01">
                <asp:Literal ID="lblAssetNo" runat="server" Text="${Facility.FacilityStock.AssetNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbAssetNo" runat="server" ReadOnly="true" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEffDate" runat="server" Text="${Facility.FacilityStock.EffDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEffDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})"
                    ReadOnly="true" />
            </td>
          
        </tr>
    </table>
</div>
<div class="tablefooter">
    <cc1:Button ID="btnDelete" runat="server" Text="${Common.Button.Delete}" CssClass="button2"
        OnClick="btnDelete_Click" FunctionId="EditFacilityStock" />
    <cc1:Button ID="btnSubmit" runat="server" Text="${Common.Button.Submit}" CssClass="button2"
        OnClick="btnSubmit_Click" FunctionId="EditFacilityStock" />
    <cc1:Button ID="btnStart" runat="server" Text="${Common.Button.Start}" CssClass="button2"
        OnClick="btnStart_Click" FunctionId="EditFacilityStock" />
    <cc1:Button ID="btnComplete" runat="server" Text="${Common.Button.Complete}" CssClass="button2"
        OnClick="btnComplete_Click" FunctionId="EditFacilityStock" />
    <cc1:Button ID="btnClose" runat="server" Text="${Common.Button.Close}" CssClass="button2"
        OnClick="btnClose_Click" FunctionId="EditFacilityStock" />
    <asp:Button ID="btnPrint" runat="server" Text="${Common.Button.Print}" CssClass="button2"
        OnClick="btnPrint_Click" />
    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
        CssClass="button2" />
</div>
