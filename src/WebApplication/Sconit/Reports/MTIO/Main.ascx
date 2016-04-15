<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Reports_MTIO_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
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
            <td class="td01">
            </td>
            <td class="td01">
            </td>
            <td class="td01">
                <asp:RadioButtonList ID="groupType" runat="server" RepeatDirection="Horizontal" CssClass="floatright"
                    AutoPostBack="true">
                    <asp:ListItem Text="按分类" Value="按分类" Selected="True" />
                    <asp:ListItem Text="按物料号" Value="按物料号" />
                    <asp:ListItem Text="按分类保税收发存" Value="按分类保税收发存" />
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
        OnDataBound="GV_List_DataBound" CellPadding="0" AllowSorting="false">
        <Columns>
            <asp:TemplateField HeaderText="Seq">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</fieldset>
