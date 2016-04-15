<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Order_ReceiptNotes_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="ReceiptNo"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" seqtext="No."
            ShowSeqNo="true" AllowSorting="True" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="ReceiptNo"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:BoundField DataField="ReceiptNo" HeaderText="${MasterData.Bill.ReceiptNo}" SortExpression="ReceiptNo" />
                <asp:BoundField DataField="ReferenceIpNo" HeaderText="${InProcessLocation.IpNo}"
                    SortExpression="ReferenceIpNo" />
                <asp:BoundField DataField="ExternalReceiptNo" HeaderText="${MasterData.Order.OrderHead.ExtOrderNo}"
                    SortExpression="ExternalReceiptNo" />
                <asp:TemplateField HeaderText="${InProcessLocation.PartyFrom}" SortExpression="PartyFrom.Name">
                    <ItemTemplate>
                        <asp:Label ID="lblPartyFrom" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PartyFrom.Name")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "ShipFrom.Address")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${InProcessLocation.ShipFrom.Address}" SortExpression="ShipFrom.Address"
                    HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "ShipFrom.Address")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${InProcessLocation.PartyTo}" SortExpression="PartyTo.Name">
                    <ItemTemplate>
                        <asp:Label ID="lblPartyTo" runat="server" Text=' <%# DataBinder.Eval(Container.DataItem, "PartyTo.Name")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "ShipTo.Address")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${InProcessLocation.ShipTo.Address}" SortExpression="ShipTo.Address"
                    HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "ShipTo.Address")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Flow" HeaderText="${Common.Business.Flow}" SortExpression="Flow" />
                <asp:BoundField DataField="CreateDate" HeaderText="${InProcessLocation.CreateDate}"
                    SortExpression="CreateDate" />
                <asp:TemplateField HeaderText="${Common.Business.CreateUser}" SortExpression="CreateDate.FirstName">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CreateUser.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SettleTime" HeaderText="结算时间" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <cc1:LinkButton ID="lbtnView" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ReceiptNo") %>'
                            Text="${Common.Button.View}" OnClick="lbtnView_Click" FunctionId="ViewAsn">
                        </cc1:LinkButton>
                        <cc1:LinkButton ID="lbtnAdjust" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ReceiptNo") %>'
                            Text="${Common.Button.Adjust}" OnClick="lbtnAdjust_Click" FunctionId="AdjustAsn"
                            Visible="false">
                        </cc1:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
    <div class="GridView">
        <cc1:GridView ID="GV_List_Detail" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" seqtext="No."
            ShowSeqNo="true" AllowSorting="True" AllowPaging="True" PagerID="gp_Detail" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_Detail_RowDataBound" DefaultSortExpression="Id"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:TemplateField HeaderText="${MasterData.Bill.ReceiptNo}" SortExpression="Receipt">
                    <ItemTemplate>
                        <cc1:LinkButton ID="lbtnView" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Receipt.ReceiptNo")%>'
                            Text='<%# DataBinder.Eval(Container.DataItem, "Receipt.ReceiptNo")%>' OnClick="lbtnView_Click"
                            FunctionId="ViewAsn">
                        </cc1:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${InProcessLocation.IpNo}" SortExpression="InProcessLocation">
                    <ItemTemplate>
                        <asp:Label ID="lblIpNo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Receipt.ReferenceIpNo")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${InProcessLocation.InProcessLocationDetail.OrderNo}"
                    SortExpression="OrderLocationTransaction.OrderDetail.OrderHead">
                    <ItemTemplate>
                        <asp:Label ID="lblOrderNo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.OrderDetail.OrderHead.OrderNo")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${InProcessLocation.InProcessLocationDetail.Item}"
                    SortExpression="OrderLocationTransaction.Item">
                    <ItemTemplate>
                        <asp:Label ID="lblItemCode" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.Item.Code")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.ItemDescription}" SortExpression="OrderLocationTransaction.Item">
                    <ItemTemplate>
                        <asp:Label ID="lblItemDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.Item.Description")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.ItemRef}" SortExpression="OrderLocationTransaction.OrderDetail.ReferenceItemCode">
                    <ItemTemplate>
                        <asp:Label ID="lblItemRef" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.OrderDetail.ReferenceItemCode")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Uom}" SortExpression="OrderLocationTransaction.Uom.Code">
                    <ItemTemplate>
                        <asp:Label ID="lblUom" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.Uom.Code")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.Uom.Description")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.UnitCount}" SortExpression="OrderLocationTransaction.OrderDetail.UnitCount">
                    <ItemTemplate>
                        <asp:Label ID="lblUnitCount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.OrderDetail.UnitCount", "{0:0.########}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.LocFrom}" SortExpression="OrderLocationTransaction.OrderDetail.LocationFrom.Code">
                    <ItemTemplate>
                        <asp:Label ID="lblLocationFrom" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.OrderDetail.DefaultLocationFrom.Code")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.OrderDetail.DefaultLocationFrom.Name")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.LocTo}" SortExpression="OrderLocationTransaction.OrderDetail.LocationTo.Code">
                    <ItemTemplate>
                        <asp:Label ID="lblLocationTo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.OrderDetail.DefaultLocationTo.Code")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.OrderDetail.DefaultLocationTo.Name")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.Receipt.Date}" SortExpression="Receipt.CreateDate">
                    <ItemTemplate>
                        <asp:Literal ID="lblReceiptDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Receipt.CreateDate","{0:yyyy-MM-dd HH:mm}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ShippedQty" HeaderText="${MasterData.Order.OrderDetail.CurrentShipQty}"
                    SortExpression="ShippedQty" DataFormatString="{0:0.########}" />
                <asp:BoundField DataField="ReceivedQty" HeaderText="${MasterData.Order.OrderDetail.CurrentReceiveQty}"
                    SortExpression="ReceivedQty" DataFormatString="{0:0.########}" />
                <asp:BoundField DataField="RejectedQty" HeaderText="${MasterData.Order.OrderDetail.CurrentRejectQty}"
                    SortExpression="RejectedQty" DataFormatString="{0:0.########}" />
                <asp:BoundField DataField="ScrapQty" HeaderText="${MasterData.Order.OrderDetail.CurrentScrapQty}"
                    SortExpression="ScrapQty" DataFormatString="{0:0.########}" />
                <asp:TemplateField HeaderText="单价">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "OrderLocationTransaction.OrderDetail.UnitPrice", "{0:0.########}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="结算时间">
                    <ItemTemplate>
                        <asp:Literal ID="lblSettleTime" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Receipt.SettleTime","{0:yyyy-MM-dd}")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp_Detail" runat="server" GridViewID="GV_List_Detail" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
