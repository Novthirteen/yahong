<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FlowDetailList.ascx.cs"
    Inherits="Inventory_PrintHu_FlowDetailList" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/MRP/ShiftPlan/Manual/Shift.ascx" TagName="Shift" TagPrefix="uc" %>
<fieldset>
    <div class="GridView">
        <asp:GridView ID="GV_List" runat="server" AllowSorting="True" AutoGenerateColumns="False"  OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.Flow}">
                    <ItemTemplate>
                        <asp:Label ID="lblFlow" runat="server" Text='<%# Bind("Flow.Code") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.FlowDescription}">
                    <ItemTemplate>
                        <asp:Label ID="lblFlowDescription" runat="server" Text='<%# Bind("Flow.Description") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.FlowType}">
                    <ItemTemplate>
                        <asp:Label ID="lblFlowType" runat="server" Text='<%# Bind("Flow.Type") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.FlowStrategy}">
                    <ItemTemplate>
                        <asp:Label ID="lblFlowStrategy" runat="server" Text='<%# Bind("Flow.FlowStrategy") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Flow.Location.From}">
                    <ItemTemplate>
                        <asp:Label ID="lblLocationFrom" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Flow.Location.To}">
                    <ItemTemplate>
                        <asp:Label ID="lblLocationTo" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.Sequence}">
                    <ItemTemplate>
                        <asp:Label ID="lblSeq" runat="server" Text='<%# Bind("Sequence") %>' />
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.Item.Code}">
                    <ItemTemplate>
                        <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("Item.Code") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.Item.Description}">
                    <ItemTemplate>
                        <asp:Label ID="lblItemDescription" runat="server" Text='<%# Bind("Item.Description") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.Uom}">
                    <ItemTemplate>
                        <asp:Label ID="lblUom" runat="server" Text='<%# Bind("Uom.Code") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.UnitCount}">
                    <ItemTemplate>
                        <asp:Label ID="lblUnitCount" runat="server" Text='<%# Bind("UnitCount","{0:0.########}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.HuLotSize}">
                    <ItemTemplate>
                        <asp:Label ID="lblHuLotSize" runat="server" Text='<%# Bind("HuLotSize","{0:0.########}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.SafeStock}">
                    <ItemTemplate>
                        <asp:Label ID="lblSafeStock" runat="server" Text='<%# Bind("SafeStock","{0:0.########}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Item.FlowDetail.MaxStock}">
                    <ItemTemplate>
                        <asp:Label ID="lblMaxStock" runat="server" Text='<%# Bind("MaxStock","{0:0.########}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="开始时间">
                    <ItemTemplate>
                        <asp:Label ID="lblStartDate" runat="server" Text='<%# Bind("StartDate") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="结束时间">
                    <ItemTemplate>
                        <asp:Label ID="lblEndDate" runat="server" Text='<%# Bind("EndDate") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</fieldset>
