<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="MRP_Schedule_MPS_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Src="../SupplierSchedule/Main.ascx" TagName="Month" TagPrefix="uc2" %>

<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
        <span class='ajax__tab_active' id='tab_1' runat="server"><span class='ajax__tab_outer'>
            <span class='ajax__tab_inner'><span class='ajax__tab_tab'>
                <asp:LinkButton ID="lbn1" Text="明细" runat="server" OnClick="lbn1_Click" /></span></span></span></span><%--<span 
        id='tab_2' runat="server" ><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbn2" Text="盘点" runat="server" OnClick="lbn2_Click" /></span></span></span></span>--%><span
            id='tab_3' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span
                class='ajax__tab_tab'><asp:LinkButton ID="lbn3" Text="月度" runat="server" OnClick="lbn3_Click" /></span></span></span></span>
    </div>
    <div class="ajax__tab_body">
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
                <asp:RadioButtonList ID="rblFlowOrLoc" runat="server" RepeatDirection="Horizontal"
                    AutoPostBack="true" OnSelectedIndexChanged="rblFlowOrLoc_SelectedIndexChanged"
                    CssClass="floatright">
                    <asp:ListItem Text="${MRP.Schedule.Flow}" Value="Flow" Selected="True" />
                    <asp:ListItem Text="${MRP.Schedule.Location}" Value="Location" />
                </asp:RadioButtonList>
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlowOrLoc" runat="server" Visible="true" DescField="Description"
                    ValueField="Code" CssClass="inputRequired" ServicePath="FlowMgr.service" MustMatch="true"
                    Width="250" ServiceMethod="GetFlowList" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:RadioButtonList ID="rblDateType" runat="server" RepeatDirection="Horizontal"
                    CssClass="floatright">
                    <asp:ListItem Text="${MRP.Schedule.DateType.StartTime}" Value="StartTime" />
                    <asp:ListItem Text="${MRP.Schedule.DateType.WinTime}" Value="WindowTime" Selected="True" />
                </asp:RadioButtonList>
            </td>
            <td class="td02">
                <asp:TextBox ID="tbScheduleTime" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd',isShowWeek:true})"
                    Width="100" />
                <asp:HiddenField ID="hfLastScheduleTime" runat="server" />
            </td>
            <td class="td01">
                ${MRP.Schedule.ItemCode}:
            </td>
            <td class="td02">
                <asp:TextBox ID="tbItemCode" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:CheckBox ID="cbDetail" runat="server" Text="${MasterData.Flow.IsListDetail}" />
            </td>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click" />
                <asp:Button ID="btnBack1" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                    Visible="false" />
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fld_Group" runat="server" visible="false">
    <legend>${MRP.Schedule.Group}</legend>
    <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
        CellPadding="0" DataKeyNames="Item">
        <Columns>
            <asp:BoundField HeaderText="Seq" />
            <asp:BoundField HeaderText="${MRP.Schedule.Item}" DataField="Item" />
            <asp:BoundField HeaderText="${MRP.Schedule.ItemDescription}" DataField="ItemDescription" />
            <asp:BoundField HeaderText="${MRP.Schedule.ItemRef}" DataField="ItemReference"    Visible="false"  />
            <asp:BoundField HeaderText="${Common.Business.Uom}" DataField="Uom" />
            <asp:BoundField HeaderText="${MRP.Schedule.UnitCount}" DataField="UnitCount" DataFormatString="{0:#,##0.##}" />
        </Columns>
    </asp:GridView>
    <asp:Literal ID="ltl_GV_List_Result" runat="server" Text="${Common.GridView.NoRecordFound}" />
</fieldset>
<uc2:Month ID="ucMonth" runat="server" Visible="false" />
    </div>
</div>
