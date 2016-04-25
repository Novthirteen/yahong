<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="FAC_Reports_FacilityMaintainReport_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})"
                    CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                    Display="Dynamic" ControlToValidate="tbStartDate" ValidationGroup="vgSearch" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})"
                    CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                    Display="Dynamic" ControlToValidate="tbEndDate" ValidationGroup="vgSearch" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlChargeOrganization" runat="server" Text="${Facility.Reports.FacilityMaintain.ChargeOrg}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbChargeOrganization" runat="server" Visible="true" Width="250"
                    DescField="ChargeOrganization" ValueField="ChargeOrganization" ServicePath="FacilityMasterMgr.service"
                    ServiceMethod="GetFacilityChargeOrganization" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlChargeSite" runat="server" Text="${Facility.Reports.FacilityMaintain.ChargeSite}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbChargeSite" runat="server" Visible="true" Width="250" DescField="ChargeSite"
                    ValueField="ChargeSite" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetFacilityChargeSite" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlMaintainType" runat="server" Text="${Facility.Reports.FacilityMaintain.MaintainType}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbMaintainType" runat="server" Visible="true" Width="250" DescField="MaintainType"
                    ValueField="MaintainType" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetMaintainTypeList" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td class="t02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="button2"
                    OnClick="btnSearch_Click" ValidationGroup="vgSearch" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnExport_Click" ValidationGroup="vgSearch" />
            </td>
        </tr>
    </table>
</fieldset>
<fieldset runat="server" id="fld_Gv_List" visible="false">
    <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
        CellPadding="0" AllowSorting="false">
        <Columns>
            <asp:TemplateField HeaderText="No.">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ChargeOrg" SortExpression="ChargeOrg" HeaderText="${Facility.Reports.FacilityMaintain.ChargeOrg}" />
            <asp:BoundField DataField="ChargeSite" SortExpression="ChargeSite" HeaderText="${Facility.Reports.FacilityMaintain.ChargeSite}" />
             <asp:BoundField DataField="MaintainType" SortExpression="MaintainType" HeaderText="${Facility.Reports.FacilityMaintain.MaintainType}" />
              <asp:BoundField DataField="FacilityCount" SortExpression="FacilityCount" HeaderText="${Facility.Reports.FacilityMaintain.FacilityCount}" />
            <asp:BoundField DataField="DownTime" SortExpression="DownTime" HeaderText="${Facility.Reports.FacilityMaintain.DownTime}"
                DataFormatString="{0:#}" />
             <asp:BoundField DataField="WarnDownCount" SortExpression="DownTime" HeaderText="停机次数"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="FixCount" SortExpression="FixCount" HeaderText="${Facility.Reports.FacilityMaintain.FixCount}"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="MaintainTime" SortExpression="MaintainTime" HeaderText="保养时间（小时）"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="PlanMaintainCount" SortExpression="PlanMaintainCount"
                HeaderText="${Facility.Reports.FacilityMaintain.PlanMaintainCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="ActualMaintainCount" SortExpression="ActualMaintainCount"
                HeaderText="${Facility.Reports.FacilityMaintain.ActualMaintainCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="MaintainRate" SortExpression="MaintainRate" HeaderText="${Facility.Reports.FacilityMaintain.MaintainRate}"
                DataFormatString="{0:#}" />
        </Columns>
    </asp:GridView>
     <script language="javascript" type="text/javascript">
         $(document).ready(function () {
             $('.GV').fixedtableheader();
         });
    </script>
</fieldset>
