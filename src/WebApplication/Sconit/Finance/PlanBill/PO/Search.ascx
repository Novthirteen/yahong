﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Finance_PlanBill_Search" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlOrderNo" runat="server" Text="${MasterData.PlannedBill.OrderNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbOrderNo" runat="server" Visible="true" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlReceiptNo" runat="server" Text="${MasterData.PlannedBill.ReceiptNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbReceiptNo" runat="server" Visible="true" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlIpNo" runat="server" Text="${Inventory.PrintHu.TabName.Asn}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbIpNo" runat="server" Visible="true" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlFlow" runat="server" Text="${Common.Business.Message.Flow}" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" ValueField="Code"
                    Width="250" ServicePath="FlowMgr.service" ServiceMethod="GetProcurementFlow" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlPartyCode" runat="server" Text="${MasterData.PlannedBill.Supplier.Code}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbPartyCode" runat="server" DescField="Name" ValueField="Code" ServicePath="PartyMgr.service"
                    ServiceMethod="GetFromParty" Width="250" />
                <asp:Literal ID="ltlParty" runat="server" Visible="false" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlItem" runat="server" Text="${Common.Business.ItemCode}:" />
            </td>
            <td class="td02">
                <%--<uc3:textbox ID="tbItemCode" runat="server" Visible="true" Width="250" DescField="Description"
                    ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem" />--%>
                <asp:TextBox ID="tbItemCode" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${MasterData.PlannedBill.CreateDateFrom}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${MasterData.PlannedBill.CreateDateTo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlListFormat" runat="server" Text="${Common.ListFormat}:" />
            </td>
            <td class="td02">
                <asp:RadioButtonList ID="rblListFormat" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="${Common.ListFormat.Group}" Value="Group" Selected="True" />
                    <asp:ListItem Text="${Common.ListFormat.Detail}" Value="Detail" />
                </asp:RadioButtonList>
            </td>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                    CssClass="button2" />
                <asp:Button ID="btnConfirm" runat="server" Text="${Common.Button.Settlement}" OnClick="btnConfirm_Click"
                    CssClass="button2" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnExport_Click" />
                <asp:Button ID="btnRecalculate" runat="server" Text="${Common.Button.Recalculate}"
                    OnClick="btnRecalculate_Click" CssClass="button2" Visible="false" />
            </td>
        </tr>
    </table>
</fieldset>
