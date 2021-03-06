﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Order_OrderHead_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="OrderNo"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" seqtext="No."
            ShowSeqNo="true" AllowSorting="True" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="OrderNo"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:BoundField DataField="OrderNo" HeaderText="${Warehouse.LocTrans.OrderNo}" SortExpression="OrderNo" />
                <asp:BoundField DataField="ExternalOrderNo" HeaderText="${Warehouse.LocTrans.ExtOrderNo}"
                    SortExpression="ExternalOrderNo" />
                <asp:BoundField DataField="ReferenceOrderNo" HeaderText="${Warehouse.LocTrans.RefOrderNo}"
                    SortExpression="ReferenceOrderNo" />
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.SubType}">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblSubType" runat="server" Code="OrderSubType" Value='<%# Bind("SubType") %>' />
                        <asp:Label ID="lbSubtype" runat="server" Text='<%# Eval("SubType")%>' CssClass="hidden" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.PartyFrom.Region}" SortExpression="PartyFrom.Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "PartyFrom.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.PartyTo}" SortExpression="PartyTo.Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "PartyTo.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="StartTime" HeaderText="${Common.Business.ShipTime}" SortExpression="StartTime" />
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.WindowTime}" SortExpression="WindowTime">
                    <ItemTemplate>
                        <asp:Label ID="lblWinTime" Text='<%# Bind("WindowTime") %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.Status}" SortExpression="Status">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblStatus" runat="server" Code="Status" Value='<%# Bind("Status") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.Priority}">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblPriority" runat="server" Code="OrderPriority" Value='<%# Bind("Priority") %>' />
                        <asp:Label ID="lbPriority" runat="server" Text='<%# Eval("Priority")%>' CssClass="hidden" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.CreateUser}" SortExpression="CreateUser.FirstName">
                    <ItemTemplate>
                        <asp:Label ID="lblCreateUser" runat="server" Text='<%# Eval("CreateUser.Name")%>'
                            ToolTip='<%# Eval("CreateUser.Code")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnView" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'
                            Text="${Common.Button.View}" OnClick="lbtnEdit_Click" FunctionId="ViewOrder">
                        </asp:LinkButton>
                        <cc1:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'
                            Visible="false" Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')"
                            FunctionId="DeleteOrder">
                        </cc1:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
    <div id="divDetail">
        <cc1:GridView ID="GV_List_Detail" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" seqtext="No."
            ShowSeqNo="true" AllowSorting="True" AllowPaging="True" PagerID="gp_Detail" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_Detail_RowDataBound" DefaultSortExpression="OrderHead"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:TemplateField HeaderText="${Warehouse.LocTrans.OrderNo}" SortExpression="OrderHead">
                    <ItemTemplate>
                       <asp:LinkButton ID="lbtnView" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderHead.OrderNo") %>'
                            Text='<%# DataBinder.Eval(Container.DataItem, "OrderHead.OrderNo")%>' OnClick="lbtnEdit_Click">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.SubType}">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblSubType" runat="server" Code="OrderSubType" Value='<%# Eval("OrderHead.SubType") %>' />
                        <asp:Label ID="lbSubtype" runat="server" Text='<%# Eval("OrderHead.SubType")%>' CssClass="hidden" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Flow}" SortExpression="OrderHead.Flow.Code">
                    <ItemTemplate>
                        <asp:Label ID="lblFlow" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderHead.Flow")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "OrderHead.Flow")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Flow.Location.From}" SortExpression="LocationFrom.Code">
                    <ItemTemplate>
                        <asp:Label ID="lblLocationTo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DefaultLocationFrom.Code")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "DefaultLocationFrom.Name")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.PartyFrom}" SortExpression="PartyFrom.Name"
                    HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "OrderHead.PartyFrom.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.PartyTo.Region}" SortExpression="PartyTo.Name"
                    HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "OrderHead.PartyTo.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.Status}" SortExpression="Status">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblStatus" runat="server" Code="Status" Value='<%# Bind("OrderHead.Status") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.Priority}">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblPriority" runat="server" Code="OrderPriority" Value='<%# Bind("OrderHead.Priority") %>' />
                        <asp:Label ID="lbPriority" runat="server" Text='<%# Eval("OrderHead.Priority")%>'
                            CssClass="hidden" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.CreateUser}" SortExpression="CreateUser.FirstName">
                    <ItemTemplate>
                        <asp:Label ID="lblCreateUser" runat="server" Text='<%# Eval("OrderHead.CreateUser.Name")%>'
                            ToolTip='<%# Eval("OrderHead.CreateUser.Code")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderDetail.Item.Code}" SortExpression="Item.Code">
                    <ItemTemplate>
                        <asp:Label ID="lblItem" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Item.Code")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "Item.Description")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderDetail.ReferenceItem}" SortExpression="ReferenceItemCode">
                    <ItemTemplate>
                        <asp:Label ID="lblReferenceItem" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ReferenceItemCode")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "ReferenceItemCode")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderDetail.Uom}" SortExpression="Uom.Code">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Uom.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="UnitCount" DataFormatString="{0:0.##}" HeaderText="${MasterData.Order.OrderDetail.UnitCount}"
                    SortExpression="UnitCount" />
                <asp:BoundField DataField="HuLotSize" DataFormatString="{0:0.##}" HeaderText="${MasterData.Order.OrderDetail.HuLotSize}"
                    SortExpression="HuLotSize" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" />
                <asp:BoundField DataField="OrderedQty" DataFormatString="{0:0.##}" HeaderText="${MasterData.Order.OrderDetail.OrderedQty}"
                    SortExpression="OrderedQty" />
                <asp:BoundField DataField="ReceivedQty" DataFormatString="{0:0.##}" HeaderText="${MasterData.Order.OrderDetail.ReceivedQty}"
                    SortExpression="ReceivedQty" />
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.WindowTime}" SortExpression="WindowTime">
                    <ItemTemplate>
                        <asp:Label ID="lblWinTime" Text='<%# Bind("OrderHead.WindowTime") %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp_Detail" runat="server" GridViewID="GV_List_Detail" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
