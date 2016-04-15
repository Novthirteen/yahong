<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Report_InvKanBan_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>

<script language="javascript" type="text/javascript">
    $(document).ready(function() {
        $('.GV').fixedtableheader();
    });
</script>

<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlLocation" runat="server" Text="${MasterData.Item.Location}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbLocation" runat="server" Visible="true" DescField="Name" Width="280"
                    ValueField="Code" ServicePath="LocationMgr.service" ServiceMethod="GetAllLocation"
                    MustMatch="false" CssClass="inputRequired" />
            </td>
            <td class="td01">
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
