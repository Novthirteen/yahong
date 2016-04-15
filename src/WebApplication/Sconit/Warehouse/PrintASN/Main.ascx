<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Warehouse_PrintASN_Main" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxControlToolkit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<script language="javascript" type="text/javascript" src="Js/DatePicker/WdatePicker.js"></script>

<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblIpNo" runat="server" Text="${InProcessLocation.IpNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbIpNo" runat="server" Visible="true" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblFlow" runat="server" Text="${MasterData.Order.OrderHead.Flow}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" ValueField="Code"
                    ServicePath="FlowMgr.service" MustMatch="true" Width="250" ServiceMethod="GetFlowList" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblPartyFrom" runat="server" Text="${MasterData.Order.OrderHead.PartyFrom.Supplier}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbPartyFrom" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" ServicePath="PartyMgr.service" ServiceMethod="GetFromParty" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblPartyTo" runat="server" Text="${MasterData.Order.OrderHead.PartyTo.Customer}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbPartyTo" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" MustMatch="true" ServicePath="PartyMgr.service" ServiceMethod="GetToParty" />
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
            </td>
            <td class="td02">
            </td>
            <td>
            </td>
            <td class="td02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                    CssClass="button2" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
</fieldset>
<asp:GridView ID="GV_List" runat="server" AllowPaging="False" AutoGenerateColumns="False"
    OnRowDataBound="GV_List_RowDataBound">
    <Columns>
        <asp:TemplateField HeaderText="${Common.GridView.Action}">
            <ItemTemplate>
                <asp:LinkButton ID="lbtnPrint" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IpNo") %>'
                    Text="${MasterData.Bom.NeedPrint}" OnClick="lbtnPrint_Click" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="IpNo" HeaderText="${InProcessLocation.IpNo}" SortExpression="IpNo" />
        <asp:TemplateField HeaderText="${InProcessLocation.Type}">
            <ItemTemplate>
                <cc1:CodeMstrLabel ID="lblType" runat="server" Code="IpType" Value='<%# Bind("Type") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Flow" HeaderText="${MasterData.Order.OrderHead.Flow}" />
        <asp:TemplateField HeaderText="${InProcessLocation.PartyFrom}" SortExpression="PartyFrom">
            <ItemTemplate>
                <asp:Label ID="PartyFromName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PartyFrom.Name")%>'
                    ToolTip='<%# DataBinder.Eval(Container.DataItem, "ShipFrom.Address")%>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="${InProcessLocation.PartyTo}" SortExpression="PartyTo">
            <ItemTemplate>
                <asp:Label ID="PartyToName" runat="server" ToolTip='<%# DataBinder.Eval(Container.DataItem, "PartyTo.Name")%>'
                    Text='<%# DataBinder.Eval(Container.DataItem, "ShipTo.Address")%>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:CheckBoxField DataField="IsPrinted" HeaderText="${MasterData.Bom.NeedPrint}"
            SortExpression="IsPrinted" Visible="false" />
        <asp:TemplateField HeaderText="${InProcessLocation.Status}">
            <ItemTemplate>
                <cc1:CodeMstrLabel ID="lblStatus" runat="server" Code="Status" Value='<%# Bind("Status") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.WindowTime}">
            <ItemTemplate>
                <%# DataBinder.Eval(Container.DataItem, "WindowTime", "{0:yyyy-MM-dd HH:mm}")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="CreateDate" HeaderText="${InProcessLocation.CreateDate}"
            SortExpression="CreateDate" />
        <asp:TemplateField HeaderText="${Common.Business.CreateUser}" SortExpression="CreateUser.FirstName">
            <ItemTemplate>
                <%# DataBinder.Eval(Container.DataItem, "CreateUser.Name")%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
