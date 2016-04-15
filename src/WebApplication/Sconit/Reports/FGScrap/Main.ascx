<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Report_FGScrap_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlProductionLine" runat="server" Text="${Common.Business.ProductionLine}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" Width="280"  CssClass="inputRequired"
                    ValueField="Code" ServicePath="FlowMgr.service" ServiceMethod="GetProductionFlow" />
            </td>
            <td class="td01">
                <%--<asp:Literal ID="lblItem" runat="server" Text="${Common.Business.ItemCode}:" />--%>
            </td>
            <td class="td02">
                <%--<uc3:textbox ID="tbItem" runat="server" Visible="true" DescField="Description" ImageUrlField="ImageUrl"
                    Width="280" ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem" />--%>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
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
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click" />
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
