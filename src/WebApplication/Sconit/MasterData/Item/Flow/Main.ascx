<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Item_FLowItem_Main" %>
<%@ Register Src="FlowDetailList.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblItem" runat="server" Text="${MasterData.Item.FlowItem}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbItemCode" runat="server" Visible="true" Width="250" MustMatch="true"
                    DescField="Description" ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem"
                    AutoPostBack="false" />
            </td>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                    CssClass="query" />
            </td>
        </tr>
    </table>
</fieldset>
<uc2:List ID="ucList" runat="server" Visible="false" />
