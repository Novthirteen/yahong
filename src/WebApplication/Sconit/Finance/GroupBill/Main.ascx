<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Finance_GroupBill_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlPartyCode" runat="server" Text="客户代号:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbPartyCode" runat="server" DescField="Name" ValueField="Code" ServicePath="CustomerMgr.service"
                    ServiceMethod="GetAllCustomer" Width="250" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvPartyCode" runat="server" ErrorMessage="*" Display="Dynamic"
                    ControlToValidate="tbPartyCode" ValidationGroup="vgSave" />
            </td>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:RadioButtonList ID="rblTimeType" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="生效时间" Value="0" Selected="True" />
                    <asp:ListItem Text="创建时间" Value="1" />
                </asp:RadioButtonList>
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
                导入:
            </td>
            <td class="td02">
                <asp:FileUpload ID="fileUpload" ContentEditable="false" runat="server" />
                <asp:Button ID="btnImport" runat="server" Text="${Common.Button.Import}" OnClick="btnSearch_Click"
                    ValidationGroup="vgSave" />
                <asp:HyperLink ID="hlTemplate" runat="server" Text="${Common.Business.ClickToDownload}"
                    NavigateUrl="~/Reports/Templates/ExcelTemplates/GroupBillTemplate.xls" />
            </td>
            <td>
            </td>
            <td class="td02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                    ValidationGroup="vgSave" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click"
                    ValidationGroup="vgSave" />
                <asp:Button ID="btnConfirm" runat="server" Text="结算" OnClick="btnConfirm_Click" ValidationGroup="vgSave" />
                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click" />
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
