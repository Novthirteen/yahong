<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Cost_MiscOrder_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView" id="list" runat="server">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="OrderNo"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="false" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="OrderNo"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:BoundField DataField="OrderNo" HeaderText="${MasterData.MiscOrder.OrderNo}"
                    SortExpression="OrderNo" />
                <asp:TemplateField HeaderText="${Common.Business.Region}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Location.Region.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Location}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Location.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Reason}">
                    <ItemTemplate>
                        <asp:Label ID="lblReason" runat="server" ToolTip='<%# Bind("Reason") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Remark}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Remark")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.MiscOrder.CreateUser}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CreateUser.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.CodeMaster.Status}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Status")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="EffectiveDate" DataFormatString="{0:yyyy-MM-dd}" HeaderText="${MasterData.MiscOrder.EffectDate}"
                    SortExpression="EffectiveDate" />
                <asp:BoundField DataField="CreateDate" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderText="${Common.Business.CreateDate}"
                    SortExpression="CreateDate" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnView" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'
                            Text="${Common.Button.View}" OnClick="lbtnView_Click">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
    <div class="GridView" id="detail" runat="server">
        <cc1:GridView ID="gv_Detail" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="False" AllowPaging="True" PagerID="gp_Detail"
            Width="100%" CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" DefaultSortExpression="Id" OnRowDataBound="GV_List_Detail_RowDataBound"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:TemplateField HeaderText="${Common.Business.OrderNo}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MiscOrder.OrderNo")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="${Common.Business.Region}">
                    <ItemTemplate>
                        <asp:Label ID="lblRegion" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "MiscOrder.Location.Region.Code")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "MiscOrder.Location.Region.Name")%>' />
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="${Common.Business.Location}">
                    <ItemTemplate>
                        <asp:Label ID="lblLocation" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "MiscOrder.Location.Code")%>'
                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "MiscOrder.Location.Name")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.ItemCode}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Item.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.ItemDescription}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Item.Description")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Uom}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Item.Uom.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Qty" DataFormatString="{0:0.########}" HeaderText="${Common.Business.Qty}" />
                <asp:TemplateField HeaderText="${Cost.Value}">
                    <ItemTemplate>
                        <asp:Label ID="lblCost" runat="server" Text='<%# Bind("Cost","{0:0.########}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Reason}">
                    <ItemTemplate>
                        <asp:Label ID="lblReason" runat="server" ToolTip='<%# Bind("MiscOrder.Reason") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Remark}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MiscOrder.Remark")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.MiscOrder.CreateUser}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MiscOrder.CreateUser.CodeName")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="${Common.CodeMaster.Status}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MiscOrder.Status")%>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="${MasterData.MiscOrder.EffectDate}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MiscOrder.EffectiveDate", "{0:yyyy-MM-dd}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.CreateDate}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MiscOrder.CreateDate", "{0:yyyy-MM-dd HH:mm}")%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp_Detail" runat="server" GridViewID="gv_Detail" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
