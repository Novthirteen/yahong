<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits=" MRP_Schedule_DmdScheduleRouting_Main" %>
<%@ Register Assembly="Whidsoft.WebControls.OrgChart" Namespace="Whidsoft.WebControls"
    TagPrefix="oc" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset id="fld_Search" runat="server">
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${MRP.Schedule.MRPEffectTime}:" />
            </td>
            <td class="td02">
                <asp:DropDownList ID="ddlDate" runat="server" />
            </td>
            <td class="td01">
                <asp:RadioButtonList ID="rblDateType" runat="server" RepeatDirection="Horizontal"
                    CssClass="floatright">
                    <asp:ListItem Text="${MRP.Schedule.DateType.StartTime}" Value="StartTime" />
                    <asp:ListItem Text="${MRP.Schedule.DateType.WinTime}" Value="WindowTime" Selected="True" />
                </asp:RadioButtonList>
            </td>
            <td class="td02">
                <asp:TextBox ID="tbScheduleTime" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd',isShowWeek:true})" CssClass="inputRequired"
                    Width="100" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Label ID="lblFlow" Text="${MRP.Schedule.Flow}" runat="server" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" Width="280"
                    ValueField="Code" ServicePath="FlowMgr.service" ServiceMethod="GetAllFlow" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvFlow" runat="server" ErrorMessage="${MasterData.SupplyChainRouting.Flow.Empty}"
                    Display="Dynamic" ControlToValidate="tbFlow" ValidationGroup="searchvg" />
            </td>
            <td class="td01">
                ${MRP.Schedule.ItemCode}:
            </td>
            <td class="td02">
                <uc3:textbox ID="tbItemCode" runat="server" Visible="true" DescField="Description"
                    Width="280" ValueField="Code" ServicePath="FlowDetailMgr.service" ServiceMethod="GetAllFlowDetailItem"
                    ServiceParameter="string:#tbFlow" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvItem" runat="server" ErrorMessage="${MasterData.SupplyChainRouting.Item.Empty}"
                    Display="Dynamic" ControlToValidate="tbItemCode" ValidationGroup="searchvg" />
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
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
</fieldset>
<fieldset runat="server" id="fld" visible="false">
    <div class="scrollx" style="width: expression((documentElement.clientWidth-70)+'px');
        text-align: center" id="scrollx">
        <table>
            <tr>
                <td>
                    <oc:OrgChart ID="OrgChartTreeView" runat="server" ChartStyle="Vertical" Font-Size="X-Small"
                        LineColor="Silver"></oc:OrgChart>
                </td>
            </tr>
        </table>
    </div>
</fieldset>
