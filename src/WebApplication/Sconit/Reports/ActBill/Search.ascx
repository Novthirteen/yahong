<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Reports_ActBill_Search" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblOrderNo" runat="server" Text="${Reports.ActBill.OrderNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbOrderNo" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblReceiptNo" runat="server" Text="${Reports.ActBill.ReceiptNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbReceiptNo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblParty" runat="server" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbParty" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code"  />
            </td>   
            <td class="td01">
                <asp:Literal ID="lblItem" runat="server" Text="${Reports.ActBill.ItemCode}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbItem" runat="server" Visible="true" DescField="Description" ImageUrlField="ImageUrl"
                    Width="280" ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStartTime" runat="server" Text="${Common.Business.StartTime}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartTime" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"
                    CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvStartTime" runat="server" ErrorMessage="${MasterData.WorkCalendar.WarningMessage.TimeEmpty}"
                    Display="Dynamic" ControlToValidate="tbStartTime" ValidationGroup="vgSave" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlEndTime" runat="server" Text="${Common.Business.EndTime}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndTime" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"
                    CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvEndTime" runat="server" ErrorMessage="${MasterData.WorkCalendar.WarningMessage.TimeEmpty}"
                    Display="Dynamic" ControlToValidate="tbEndTime" ValidationGroup="vgSave" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:RadioButtonList ID="rblTimeType" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="生效时间" Value="0" />
                    <asp:ListItem Text="创建时间" Value="1" Selected="True" />
                </asp:RadioButtonList>
            </td>
            <td class="td02">
                <asp:CheckBox ID="cbGroupByParty" runat="server" Text="按客户汇总" />
            </td>
            <td>
            </td>
            <td class="t02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="button2"
                    OnClick="btnSearch_Click" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnExport_Click" />
            </td>
        </tr>
    </table>
</fieldset>