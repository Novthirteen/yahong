<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="MasterData_MiscOrder_Search" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblCustomerSupplierCode" runat="server" Text="${MasterData.MiscOrder.OrderNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbMiscOrderCode" runat="server" Visible="true" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblMiscOrderEffectDate" runat="server" Text="${MasterData.MiscOrder.EffectDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbMiscOrderEffectDate" runat="server" onClick="WdatePicker()" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblMiscOrderRegion" runat="server" Text="${Common.Business.Region}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbMiscOrderRegion" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" MustMatch="true" ServicePath="RegionMgr.service" ServiceMethod="GetRegion" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblMIscOrderLocation" runat="server" Text="${Common.Business.Location}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbMiscOrderLocation" runat="server" Visible="true" DescField="Name"
                    ValueField="Code" Width="250" ServicePath="LocationMgr.service" ServiceMethod="GetLocation"
                    ServiceParameter="string:#tbMiscOrderRegion" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="startDate" runat="server" onClick="WdatePicker()" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker()" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblItem" runat="server" Text="${MasterData.Item.Code}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbItem" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblMiscReason" runat="server" Text="${Common.Business.Reason}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlReason" Code="StockOutReason" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStatus" runat="server" Text="${Common.CodeMaster.Status}:" />
            </td>
            <td class="td02">
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Selected="True" Text="Submit" Value="Submit" />
                    <asp:ListItem Text="Cancel" Value="Cancel" />
                    <asp:ListItem Text="Close" Value="Close" />
                </asp:DropDownList>
            </td>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:RadioButtonList ID="rblListFormat" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="${Common.ListFormat.Group}" Value="Group" Selected="True" />
                    <asp:ListItem Text="${Common.ListFormat.Detail}" Value="Detail" />
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td class="td01">
            </td>
            <td class="td02">
            </td>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click"
                    Visible="false" />
                <asp:Literal ID="tbTransactionType" runat="server" Visible="false" />
                <asp:Button ID="btnBackAdd" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click" />
            </td>
        </tr>
    </table>
</fieldset>
