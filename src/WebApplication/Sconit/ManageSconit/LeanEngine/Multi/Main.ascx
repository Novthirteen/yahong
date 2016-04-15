<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ManageSconit_LeanEngine_Multi_Main" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td colspan="3" />
            <td class="ttd02">
                <asp:Button ID="btnRun" runat="server" Text="Run MRP" OnClick="btnRun_Click" />
                <asp:Button ID="btnTest" runat="server" Text="BOM" OnClick="btnTest_Click"  />
            </td>
        </tr>
    </table>
</fieldset>
<asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="true" OnRowDataBound="GV_List_RowDataBound"
    CellPadding="0" AllowSorting="true">
    <Columns>
        <asp:TemplateField HeaderText="Seq">
            <ItemTemplate>
                <%#Container.DataItemIndex+1%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
