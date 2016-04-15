<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Visualization_SupplyChainRouting_Search" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblFlow" runat="server" Text="${Common.Business.Flow}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" Width="280"
                    ValueField="Code" ServicePath="FlowMgr.service" ServiceMethod="GetAllFlow" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvFlow" runat="server" ErrorMessage="${MasterData.SupplyChainRouting.Flow.Empty}"
                    Display="Dynamic" ControlToValidate="tbFlow" ValidationGroup="searchvg" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblItemCode" runat="server" Text="${Common.Business.ItemCode}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbItemCode" runat="server" Visible="true" DescField="Description"
                    Width="280" ValueField="Code" ServicePath="FlowDetailMgr.service" ServiceMethod="GetAllFlowDetailItem"
                    ServiceParameter="string:#tbFlow" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvItem" runat="server" ErrorMessage="${MasterData.SupplyChainRouting.Item.Empty}"
                    Display="Dynamic" ControlToValidate="tbItemCode" ValidationGroup="searchvg" />
            </td>
        </tr>
        <tr>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:RadioButtonList ID="rblupOrdown" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Selected="True" Text="向下" Value="down" />
                    <asp:ListItem Text="向上" Value="up" />
                </asp:RadioButtonList>
            </td>
            <td />
            <td class="ttd02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                    ValidationGroup="searchvg" />
                <asp:Button ID="btnCheck" runat="server" Text="${Common.Button.Check}" OnClick="btnCheck_Click" />
            </td>
        </tr>
    </table>
</fieldset>
