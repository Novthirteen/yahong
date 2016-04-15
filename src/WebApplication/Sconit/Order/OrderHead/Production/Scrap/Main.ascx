<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Order_OrderHead_Production_Scrap" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblRefOrderNo" runat="server" Text="${MasterData.Order.OrderHead.Flow.RefOrderNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbRefOrderNo" runat="server" CssClass="inputRequired"></asp:TextBox>
                <asp:Button ID="btnRefOrderNo" runat="server" Text="${MasterData.Flow.ReferenceFlow}"
                    OnClick="btnRefOrderNo_Click" CssClass="button2" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblFlow" runat="server" Text="${MasterData.Flow.Flow.Production}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" ValueField="Code"
                    ServicePath="FlowMgr.service" MustMatch="true" Width="250" CssClass="inputRequired"
                    ServiceMethod="GetFlowList" />
                <asp:RequiredFieldValidator ID="rfvFlow" runat="server" ErrorMessage="请选择生产线"
                    Display="Dynamic" ControlToValidate="tbFlow" ValidationGroup="vgCreate" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblEffectDate" runat="server" Text="${MasterData.MiscOrder.EffectDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbMiscOrderEffectDate" runat="server" CssClass="inputRequired" onClick="WdatePicker()" />
                <asp:RequiredFieldValidator ID="rfvEffectDate" runat="server" ErrorMessage="${MasterData.MiscOrder.WarningMessage.EffectDateEmpty}"
                    Display="Dynamic" ControlToValidate="tbMiscOrderEffectDate" ValidationGroup="vgCreate" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlReason" runat="server" Text="${Common.Business.Reason}:" />
            </td>
            <td class="td02">
                <asp:Label ID="lblReason" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblLocation" runat="server" Text="${Common.Business.Location}:" />
            </td>
            <td class="td02">
                <asp:Label ID="tbLocation" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlRemark" runat="server" Text="${Common.Business.Remark}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbRemark" runat="server" Width="200" />
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
                <cc1:Button ID="btnConfirm" runat="server" Text="${Common.Button.Create}" OnClick="btnConfirm_Click"
                    CssClass="button2" ValidationGroup="vgCreate" FunctionId="EditOrder" />
            </td>
        </tr>
    </table>
</fieldset>
<fieldset>
    <legend>${Common.Business.OrderDetails}</legend>
    <asp:GridView ID="GV_List" runat="server" AllowPaging="False" DataKeyNames="Id" AllowSorting="False"
        AutoGenerateColumns="False">
        <Columns>
            <asp:TemplateField HeaderText="${Common.Business.ItemCode}">
                <ItemTemplate>
                    <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("InspectOrderDetail.LocationLotDetail.Item.Code") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${MasterData.Order.OrderDetail.Item.Description}">
                <ItemTemplate>
                    <asp:Label ID="tbItemDescriptionText" runat="server" Text='<%# Bind("InspectOrderDetail.LocationLotDetail.Item.Description") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${MasterData.Order.OrderDetail.Uom}">
                <ItemTemplate>
                    <asp:Label ID="tvlItemUom" runat="server" Text='<%# Bind("InspectOrderDetail.LocationLotDetail.Item.Uom.Code") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.Business.ItemQty}">
                <ItemTemplate>
                    <asp:TextBox ID="tbQty" runat="server" Text='<%# Bind("RejectedQty","{0:0.########}") %>' />
                    <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="tbQty" ErrorMessage="${Common.Validator.Valid.Number}"
                        Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="999999999"
                        ValidationGroup="vgCreate" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</fieldset>
