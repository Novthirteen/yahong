<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="MRP_Schedule_SupplierSchedule_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                路线:
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" ValueField="Code"
                    CssClass="inputRequired" ServicePath="FlowMgr.service" MustMatch="true" Width="250"
                    ServiceMethod="GetFlowList" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlFinanceYear" runat="server" Text="${Cost.FinanceCalendar.YearMonth}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbFinanceYear" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM'})"
                    CssClass="inputRequired" />
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
            </td>
        </tr>
    </table>
</fieldset>
<br />
<asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
    CellPadding="0" AllowSorting="false">
    <Columns>
        <asp:TemplateField HeaderText="Seq">
            <ItemTemplate>
                <%#Container.DataItemIndex+1%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField HeaderText="采购路线" DataField="Flow" />
        <asp:BoundField HeaderText="${MRP.Schedule.Item}" DataField="Item" />
        <asp:BoundField HeaderText="${MRP.Schedule.ItemDescription}" DataField="ItemDescription" />
        <asp:BoundField HeaderText="${MRP.Schedule.ItemRef}" />
        <asp:BoundField HeaderText="${Common.Business.Uom}" DataField="Uom" />
        <asp:BoundField HeaderText="月度需求" DataField="Qty" DataFormatString="{0:#,##0.##}" />
        <asp:BoundField HeaderText="本月已收" DataField="ReceivedQty" DataFormatString="{0:#,##0.##}" />
    </Columns>
</asp:GridView>
