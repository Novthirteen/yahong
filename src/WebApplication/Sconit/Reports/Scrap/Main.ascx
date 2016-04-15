<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Reports_Scrap_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlProductionLine" runat="server" Text="${Common.Business.ProductionLine}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" Width="280"
                    ValueField="Code" ServicePath="FlowMgr.service" ServiceMethod="GetProductionFlow" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblItem" runat="server" Text="${Common.Business.ItemCode}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbItem" runat="server" Visible="true" DescField="Description" ImageUrlField="ImageUrl"
                    Width="280" ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem" />
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
                <asp:RadioButtonList ID="rblListFormat" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="${Common.ListFormat.Group}" Value="Group" Selected="True" />
                    <asp:ListItem Text="${Common.ListFormat.Detail}" Value="Detail" />
                </asp:RadioButtonList>
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
<fieldset>
    <asp:GridView ID="Gv_List_Detail" runat="server" OnRowDataBound="GV_List_RowDataBound" AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField HeaderText="${Common.Business.OrderNo}" >
                <ItemTemplate>
                    <asp:Label ID="lblOrderNo" runat="server" Text='<%# Eval("OrderDetail.OrderHead.OrderNo")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.CreateDate}">
                <ItemTemplate>
                    <asp:Label ID="lblCreateDate" runat="server" Text='<%# Eval("OrderDetail.OrderHead.CreateDate")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.CreateUser}">
                <ItemTemplate>
                    <asp:Label ID="lblCreateUser" runat="server" Text='<%# Eval("OrderDetail.OrderHead.CreateUser.CodeName")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.Business.ItemCode}">
                <ItemTemplate>
                    <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("Item.Code")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.Business.ItemDescription}">
                <ItemTemplate>
                    <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval("Item.Description")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.Business.Location}">
                <ItemTemplate>
                    <asp:Label ID="lblLocation" runat="server" Text='<%# Eval("Location.Code")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${MasterData.Location.Name}">
                <ItemTemplate>
                    <asp:Label ID="lblLocationName" runat="server" Text='<%# Eval("Location.Name")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.Business.Uom}">
                <ItemTemplate>
                    <asp:Label ID="lblUOM" runat="server" Text='<%# Eval("Item.Uom.Code")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="AccumulateQty" HeaderText="${Common.Business.Qty}" DataFormatString="{0:0.###}" />
        </Columns>
    </asp:GridView>
    <asp:GridView ID="GV_List_Group" runat="server" OnRowDataBound="GV_List_RowDataBound" AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField HeaderText="${Common.Business.ItemCode}">
                <ItemTemplate>
                    <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("Item.Code")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.Business.ItemDescription}">
                <ItemTemplate>
                    <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval("Item.Description")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.Business.Location}">
                <ItemTemplate>
                    <asp:Label ID="lblLocation" runat="server" Text='<%# Eval("Location.Code")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${MasterData.Location.Name}">
                <ItemTemplate>
                    <asp:Label ID="lblLocationName" runat="server" Text='<%# Eval("Location.Name")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.Business.Uom}" >
                <ItemTemplate>
                    <asp:Label ID="lblUOM" runat="server" Text='<%# Eval("Item.Uom.Code")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="AccumulateQty" HeaderText="${Common.Business.Qty}" DataFormatString="{0:0.###}" />
        </Columns>
    </asp:GridView>
</fieldset>
