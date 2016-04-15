<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Import.ascx.cs" Inherits="MRP_ShiftPlan_Import_Import" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ac1" %>
<%@ Register Src="~/MRP/ShiftPlan/Manual/Shift.ascx" TagName="Shift" TagPrefix="uc" %>

<script language="javascript" type="text/javascript" src="Js/DatePicker/WdatePicker.js"></script>

<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlRegion" runat="server" Text="${Common.Business.Region}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbRegion" runat="server" DescField="Name" ValueField="Code" Width="200"
                    ServicePath="RegionMgr.service" ServiceMethod="GetRegion" MustMatch="true" AutoPostBack="true"
                    OnTextChanged="tbRegion_TextChanged" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlProductionLine" runat="server" Text="${Common.Business.ProductionLine}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" Width="280"
                    ValueField="Code" ServicePath="ItemFlowPlanMgr.service" ServiceMethod="GetFlowByPartyAndPlanType"
                    ServiceParameter="string:#tbRegion,string:MRP" MustMatch="true" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlDate" runat="server" Text="${Common.Business.Date}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbDate" runat="server" AutoPostBack="true" OnTextChanged="tbDate_TextChanged"
                    onClick="WdatePicker({dateFmt:'yyyy-MM-dd',isShowWeek:true})" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlShift" runat="server" Text="${MasterData.WorkCalendar.Shift}:" />
            </td>
            <td class="td02">
                <asp:UpdatePanel ID="UP_Shift" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <uc:Shift ID="ucShift" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="tbRegion" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="tbDate" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlSelect" runat="server" Text="${Common.FileUpload.PleaseSelect}:"></asp:Literal>
            </td>
            <td class="td02">
                <asp:FileUpload ID="fileUpload" ContentEditable="false" runat="server" />
                <asp:HyperLink ID="hlTemplate" runat="server" Text="${Common.Business.ClickToDownload}"
                    NavigateUrl="~/Reports/Templates/ImportTemplates/PSModelSample.xls"></asp:HyperLink>
            </td>
            <td class="td01">
                <asp:Literal ID="ltlSettleDate" runat="server" Text="${MasterData.Order.OrderHead.SettleTime}:"  />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbSettleTime" runat="server" CssClass="inputRequired" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:CheckBox ID="cbCreateOpt" runat="server" Text="${MasterData.ShiftPlan.CreateOpt}"
                    Checked="true" />
            </td>
            <td class="td01" />
            <td class="td02">
                <div class="buttons">
                    <asp:Button ID="btnImport" runat="server" Text="${Common.Button.Import}" OnClick="btnImport_Click"
                        CssClass="apply" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="back" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
