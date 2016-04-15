<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Order_ReceiptNotes_Search" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:DropDownList ID="rblOrderNo" runat="server">
                    <asp:ListItem Selected="True" Text="${MasterData.Bill.ReceiptNo}" Value="OrderNo" />
                    <asp:ListItem Text="${Warehouse.LocTrans.ExtOrderNo}" Value="ExtOrderNo" />
                </asp:DropDownList>
            </td>
            <td class="td02">
                <asp:TextBox ID="tbReceiptNo" runat="server" Visible="true" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlAsnNo" runat="server" Text="${InProcessLocation.IpNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbIpNo" runat="server" Visible="true" />
            </td>
        </tr>
        <tr runat="server" id="trDetails">
            <td class="td01">
                <asp:Literal ID="ltlOrderNo" runat="server" Text="${InProcessLocation.InProcessLocationDetail.OrderNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbOrderNo" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlItem" runat="server" Text="${InProcessLocation.InProcessLocationDetail.Item}:" />
            </td>
            <td class="td02">
                <%-- <uc3:textbox ID="tbItem" runat="server" Visible="true" DescField="Description" ImageUrlField="ImageUrl"
                    Width="280" ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem" />--%>
                <asp:TextBox ID="tbItem" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblFlow" runat="server" Text="${MasterData.Order.OrderHead.Flow}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" ValueField="Code"
                    ServicePath="FlowMgr.service" MustMatch="true" Width="250" ServiceMethod="GetFlowList" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblParty" runat="server" Text="${MasterData.Order.OrderHead.PartyTo.Customer}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbParty" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" MustMatch="true" ServicePath="PartyMgr.service" ServiceMethod="GetPartys" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:RadioButtonList ID="rblListFormat" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="${Common.ListFormat.Group}" Value="Group" Selected="True" />
                    <asp:ListItem Text="${Common.ListFormat.Detail}" Value="Detail" />
                </asp:RadioButtonList>
            </td>
            <td class="td01">
                <asp:RadioButtonList ID="rblDateType" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="创建时间" Value="Create" Selected="True" />
                    <asp:ListItem Text="结算时间" Value="Eff" />
                </asp:RadioButtonList>
            </td>
            <td class="ttd02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                    CssClass="button2" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
</fieldset>
