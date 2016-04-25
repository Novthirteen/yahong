<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_ResInfo_ResUserChange_Main" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${ISI.ResponChange.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" CssClass="inputRequired" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${ISI.ResponChange.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" CssClass="inputRequired" />
            </td>
        </tr>
        <tr>
            <td class="td01">汇总方式:</td>
            <td class="td02">
                <asp:RadioButtonList ID="rblType" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="工作区" Value="WorkShop" Selected="True" />
                    <asp:ListItem Text="岗位" Value="Role" />
                </asp:RadioButtonList>
            </td>
            <td class="td01"></td>
            <td class="t02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="query"
                        OnClick="btnSearch_Click" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                        OnClick="btnSearch_Click" Visible="false" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fs" runat="server" visible="false">
    <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
        CellPadding="0" AllowSorting="false">
        <Columns>
            <asp:TemplateField HeaderText="Seq">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Code" HeaderText="${ISI.Responsibility.WorkShop}" />
            <asp:BoundField DataField="Name" HeaderText="${ISI.Responsibility.WorkShop}" />
            <asp:BoundField DataField="UserNames" HeaderText="${ISI.Responsibility.Director}" ItemStyle-Width="60%" ItemStyle-Wrap="True" />
            <asp:BoundField DataField="OldCount" HeaderText="期初人数" />
            <asp:BoundField DataField="NewCount" HeaderText="期末人数" />
            <asp:BoundField DataField="Mark" HeaderText="变化" />
        </Columns>
    </asp:GridView>
</fieldset>
