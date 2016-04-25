<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_Reports_FacilityTransReport_Main" %>
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
                <asp:Literal ID="ltlChargeOrganization" runat="server" Text="${Facility.Reports.FacilityTrans.ChargeOrg}:" />
            </td>
            <td class="td02">
                <uc3:textbox id="tbChargeOrganization" runat="server" visible="true" width="250"
                    descfield="ChargeOrganization" valuefield="ChargeOrganization" servicepath="FacilityMasterMgr.service"
                    servicemethod="GetFacilityChargeOrganization" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlCategoryCode" runat="server" Text="${Facility.Reports.FacilityTrans.CategoryCode}:" />
            </td>
            <td class="td02">
                <uc3:textbox id="tbCategoryCode" runat="server" visible="true" width="250" descfield="Description"
                    valuefield="Code" servicepath="FacilityCategoryMgr.service" servicemethod="GetAllFacilityCategory"/>
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
            <asp:BoundField DataField="ChargeOrg" SortExpression="ChargeOrg" HeaderText="${Facility.Reports.FacilityTrans.ChargeOrg}"
                ItemStyle-Wrap="false" />
            <asp:BoundField DataField="CategoryCode" SortExpression="CategoryCode" HeaderText="${Facility.Reports.FacilityTrans.CategoryCode}"
                ItemStyle-Wrap="false" />
            <asp:BoundField DataField="CategoryDescription" SortExpression="CategoryDescription"
                HeaderText="${Facility.Reports.FacilityTrans.CategoryDescription}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="CreateCount" SortExpression="CreateCount" HeaderText="${Facility.Reports.FacilityTrans.CreateCount}"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="EnableCount" SortExpression="EnableCount" HeaderText="${Facility.Reports.FacilityTrans.EnableCount}"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="ApplyCount" SortExpression="ApplyCount" HeaderText="${Facility.Reports.FacilityTrans.ApplyCount}"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="ReturnCount" SortExpression="ReturnCount" HeaderText="${Facility.Reports.FacilityTrans.ReturnCount}"
                DataFormatString="{0:#}" />
        <%--    <asp:BoundField DataField="MaintainStartCount" SortExpression="MaintainStartCount"
                HeaderText="${Facility.Reports.FacilityTrans.MaintainStartCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="MaintainFinishCount" SortExpression="MaintainFinishCount"
                HeaderText="${Facility.Reports.FacilityTrans.MaintainFinishCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="FixStartCount" SortExpression="FixStartCount" HeaderText="${Facility.Reports.FacilityTrans.FixStartCount}"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="FixFinishCount" SortExpression="FixFinishCount" HeaderText="${Facility.Reports.FacilityTrans.FixFinishCount}"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="InspectStartCount" SortExpression="InspectStartCount"
                HeaderText="${Facility.Reports.FacilityTrans.InspectStartCount}" DataFormatString="{0:#}" />
            <asp:BoundField DataField="InspectFinishCount" SortExpression="InspectFinishCount"
                HeaderText="${Facility.Reports.FacilityTrans.InspectFinishCount}" DataFormatString="{0:#}" />--%>
            <asp:BoundField DataField="LendCount" SortExpression="LendCount" HeaderText="${Facility.Reports.FacilityTrans.LendCount}"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="SellCount" SortExpression="SellCount" HeaderText="${Facility.Reports.FacilityTrans.SellCount}"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="StockTakeCount" SortExpression="StockTakeCount" HeaderText="${Facility.Reports.FacilityTrans.StockTakeCount}"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="LoseCount" SortExpression="LoseCount" HeaderText="${Facility.Reports.FacilityTrans.LoseCount}"
                DataFormatString="{0:#}" />
            <asp:BoundField DataField="ScrapCount" SortExpression="ScrapCount" HeaderText="${Facility.Reports.FacilityTrans.ScrapCount}"
                DataFormatString="{0:#}" />
        </Columns>
    </asp:GridView>
     <script language="javascript" type="text/javascript">
         $(document).ready(function () {
             $('.GV').fixedtableheader();
         });
    </script>
</fieldset>
