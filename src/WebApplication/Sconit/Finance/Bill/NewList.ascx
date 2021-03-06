﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewList.ascx.cs" Inherits="Finance_Bill_NewList" %>

<script language="javascript" type="text/javascript" src="Js/calcamount.js"></script>

<script type="text/javascript" language="javascript">
    function GVCheckClick() {
        if ($(".GVHeader input:checkbox").prop("checked") == true) {
            $(".GVRow input:checkbox").attr("checked", true);
            $(".GVAlternatingRow input:checkbox").attr("checked", true);
        }
        else {
            $(".GVRow input:checkbox").attr("checked", false);
            $(".GVAlternatingRow input:checkbox").attr("checked", false);
        }
    }
    function CalCulateAmount(obj, qtyField, unitPriceField, amountField) {
        var objId = $(obj).attr("id");
        var parentId = objId.substring(0, objId.length - qtyField.length);
        var qtyId = "#" + parentId + qtyField;
        var amountId = "#" + parentId + amountField;
        var unitPriceId = "#" + parentId + unitPriceField;

        $(amountId).attr('value', ($(unitPriceId).val() * $(qtyId).val()).toFixed(2));
    }
</script>

<fieldset>
    <div class="GridView">
        <asp:GridView ID="GV_List" runat="server" AllowPaging="False" DataKeyNames="Id" OnSorting="GV_List_Sorting"
            AllowSorting="true" AutoGenerateColumns="False" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div onclick="GVCheckClick()">
                            <asp:CheckBox ID="CheckAll" runat="server" />
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:HiddenField ID="hfId" runat="server" Value='<%# Bind("Id") %>' />
                        <asp:CheckBox ID="CheckBoxGroup" name="CheckBoxGroup" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText=" ${MasterData.ActingBill.Supplier}" SortExpression="BillAddress.Party.Name">
                    <ItemTemplate>
                        <asp:Label ID="lblSupplier" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "BillAddress.Party.Code")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "BillAddress.Party.Name")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ReceiptNo" HeaderText="${MasterData.ActingBill.ReceiptNo}"
                    SortExpression="ReceiptNo" />
                <asp:BoundField DataField="ExternalReceiptNo" HeaderText="${MasterData.ActingBill.ExternalReceiptNo}"
                    SortExpression="ExternalReceiptNo" />
                <asp:TemplateField HeaderText=" ${Common.Business.ItemCode}" SortExpression="Item.Code">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Item.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText=" ${Common.Business.ItemDescription}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Item.Description")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Reports.IntransitDetail.ReferenceItemCode}">
                    <%--HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden"--%>
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "ReferenceItemCode")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText=" ${Common.Business.Uom}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Uom.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.ActingBill.EffectiveDate}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "EffectiveDate", "{0:yyyy-MM-dd}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.ActingBill.UnitPrice}">
                    <ItemTemplate>
                        <asp:HiddenField ID="hfUnitPrice" runat="server" Value='<%# Bind("UnitPrice") %>' />
                        <%# DataBinder.Eval(Container.DataItem, "UnitPrice", "{0:0.########}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.ActingBill.Currency}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Currency.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.ActingBill.BillQty}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "BillQty", "{0:0.########}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.ActingBill.BilledQty}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "BilledQty", "{0:0.########}")%>
                    </ItemTemplate>
                </asp:TemplateField>    
                <asp:TemplateField HeaderText="${MasterData.ActingBill.CurrentBillQty}">
                    <ItemTemplate>
                        <asp:TextBox ID="tbQty" runat="server" onmouseup="if(!readOnly)select();" Width="50" Text='<%# DataBinder.Eval(Container.DataItem, "CurrentBillQty", "{0:0.########}")%>'
                            onchange="CalCulateAmount(this,'tbQty','hfUnitPrice','tbAmount');"></asp:TextBox>
                        <span style="display: none">
                            <asp:TextBox ID="tbDiscountRate" runat="server" Text="0" />
                            <asp:TextBox ID="tbDiscount" runat="server" Text="0" />
                        </span>
                        <asp:Literal ID="ltlQty" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.ActingBill.Amount}">
                    <ItemTemplate>
                        <asp:TextBox ID="tbAmount" runat="server" Width="80" />
                        <asp:Literal ID="ltlAmount" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Finance.Bill.IsProvisionalEstimate}">
                    <ItemTemplate>
                        <asp:Label ID="lblIsProvisionalEstimate" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText=" ${MasterData.Address.Code}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "BillAddress.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText=" ${MasterData.Address.Address}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "BillAddress.Address")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText=" ${MasterData.Flow.Code}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FlowCode")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="行">
                    <ItemTemplate>
                       <%# DataBinder.Eval(Container.DataItem, "RowIndex")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="消息">
                    <ItemTemplate>
                       <%# DataBinder.Eval(Container.DataItem, "WarningMessage")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="创建时间">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CreateDate", "{0:yyyy-MM-dd HH:mm}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="出入库时间">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "InvIOTime", "{0:yy-MM-dd HH:mm}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="收货单时间">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "RecTime", "{0:yy-MM-dd HH:mm}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Id" ItemStyle-CssClass="hidden" HeaderStyle-CssClass="hidden">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Id")%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Literal ID="lblNoRecordFound" runat="server" Text="${Common.GridView.NoRecordFound}"
            Visible="false" />
    </div>
</fieldset>

<fieldset>
<legend>未匹配的</legend>
        <asp:GridView ID="GV_NotMatch" runat="server" AllowPaging="False" DataKeyNames="Id"
            AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="ReceiptNo" HeaderText="${MasterData.ActingBill.ReceiptNo}"
                    SortExpression="ReceiptNo" />
                <asp:BoundField DataField="ExternalReceiptNo" HeaderText="${MasterData.ActingBill.ExternalReceiptNo}"
                    SortExpression="ExternalReceiptNo" />
                <asp:TemplateField HeaderText=" ${Common.Business.ItemCode}" SortExpression="Item.Code">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "ItemCode")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText=" ${Common.Business.ItemDescription}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "ItemDescription")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Reports.IntransitDetail.ReferenceItemCode}">
                    <%--HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden"--%>
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "ReferenceItemCode")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.ActingBill.EffectiveDate}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "EffectiveDate", "{0:yyyy-MM-dd}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.ActingBill.UnitPrice}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "UnitPrice", "{0:0.########}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.ActingBill.CurrentBillQty}">
                    <ItemTemplate>
                       <%# DataBinder.Eval(Container.DataItem, "CurrentBillQty", "{0:0.########}")%>
                    </ItemTemplate>
                </asp:TemplateField>  
                <asp:TemplateField HeaderText="行">
                    <ItemTemplate>
                       <%# DataBinder.Eval(Container.DataItem, "RowIndex")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="消息">
                    <ItemTemplate>
                       <%# DataBinder.Eval(Container.DataItem, "ErrorMessage")%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
</fieldset>