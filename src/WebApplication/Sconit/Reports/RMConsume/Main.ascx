<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Reports_RMConsume_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlProductionLine" runat="server" Text="${Common.Business.ProductionLine}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" Width="280"
                    ValueField="Code" ServicePath="FlowMgr.service" ServiceMethod="GetProductionFlow"  CssClass="inputRequired"/>
            </td>
            <td class="td01">
                <asp:Literal ID="ltlTranfer" runat="server" Text="移库路线:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbTransfer" runat="server" Visible="true" DescField="Description" Width="280"
                    ValueField="Code" ServicePath="FlowMgr.service" ServiceMethod="GetTransferFlow"  CssClass="inputRequired"/>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlFinanceYear1" runat="server" Text="${Cost.FinanceCalendar.YearMonth}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbFinanceYear1" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM'})"
                    OnTextChanged="tbFinanceYear_TextChange" AutoPostBack="true" />
            </td>
            <td class="td01">
            </td>
            <td class="td02">
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"
                    CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvStartTime" runat="server" ErrorMessage="${MasterData.WorkCalendar.WarningMessage.TimeEmpty}"
                    Display="Dynamic" ControlToValidate="tbStartDate" ValidationGroup="vgSave" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"
                    CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rvfEndDate" runat="server" ErrorMessage="${MasterData.WorkCalendar.WarningMessage.TimeEmpty}"
                    Display="Dynamic" ControlToValidate="tbEndDate" ValidationGroup="vgSave" />
            </td>
        </tr>
        <tr>
            <td class="td01">
            </td>
            <td class="td02">
            </td>
            <td class="td01">
            </td>
            <td class="t02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="query"
                        OnClick="btnSearch_Click" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                        OnClick="btnExport_Click" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset runat="server" id="fld_Gv_List">
    <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="true" OnRowDataBound="GV_List_RowDataBound"
        CellPadding="0" AllowSorting="false">
        <Columns>
            <asp:TemplateField HeaderText="Seq">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</fieldset>
