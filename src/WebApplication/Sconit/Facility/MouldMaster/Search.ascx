<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Facility_MouldMaster_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblFCID" runat="server" Text="${Facility.FacilityMaster.FCID}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbFCID" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlName" runat="server" Text="${Facility.FacilityMaster.Name}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndTime}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStatus" runat="server" Text="${Facility.FacilityMaster.Status}:" />
            </td>
            <td>
                <ct:ASDropDownTreeView ID="astvMyTree" runat="server" BasePath="~/Js/astreeview/"
                    DataTableRootNodeValue="0" EnableRoot="false" EnableNodeSelection="false" EnableCheckbox="true"
                    EnableDragDrop="false" EnableTreeLines="false" EnableNodeIcon="false" EnableCustomizedNodeIcon="false"
                    EnableDebugMode="false" EnableRequiredValidator="false" InitialDropdownText=""
                    Width="170" EnableCloseOnOutsideClick="true" EnableHalfCheckedAsChecked="true"
                    DropdownIconDown="~/Js/astreeview/images/windropdown.gif" EnableContextMenuAdd="false"
                    MaxDropdownHeight="200" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityMaster.Category}:" />
            </td>
            <td class="td02">
                <%--  <uc3:textbox ID="tbCategory" runat="server" Visible="true" Width="250" DescField="Description"
                    ValueField="Code" ServicePath="FacilityCategoryMgr.service" ServiceMethod="GetAllFacilityCategory" />--%>
                <ct:ASDropDownTreeView ID="astvMyTree1" runat="server" BasePath="~/Js/astreeview/"
                    DataTableRootNodeValue="0" EnableRoot="false" EnableNodeSelection="false" EnableCheckbox="true"
                    EnableDragDrop="false" EnableTreeLines="false" EnableNodeIcon="false" EnableCustomizedNodeIcon="false"
                    EnableDebugMode="false" EnableRequiredValidator="false" InitialDropdownText=""
                    Width="170" EnableCloseOnOutsideClick="true" EnableHalfCheckedAsChecked="true"
                    DropdownIconDown="~/Js/astreeview/images/windropdown.gif" EnableContextMenuAdd="false"
                    MaxDropdownHeight="200" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlAssetNo" runat="server" Text="${Facility.FacilityMaster.AssetNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbAssetNo" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlIsAsset" runat="server" Text="${Facility.FacilityMaster.IsAsset}:" />
                <asp:CheckBox ID="cbIsAsset" runat="server" Checked="true" />
            </td>
            <td class="td02">
                <asp:Literal ID="ltlIsAssetAll" runat="server" Text="${Facility.FacilityMaster.All}:" />
                <asp:CheckBox ID="cbIsAssetAll" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlChargePerson" runat="server" Text="${Facility.FacilityMaster.CurrChargePerson}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbChargePerson" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlSpecification" runat="server" Text="${Facility.FacilityMaster.Specification}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbSpecification" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlChargeSite" runat="server" Text="${Facility.FacilityMaster.ChargeSite}:" />
            </td>
            <td class="td02">
                <%--<asp:TextBox ID="tbChargeSite" runat="server" />--%>
                <uc3:textbox ID="tbChargeSite" runat="server" Visible="true" Width="250" DescField="ChargeSite"
                    ValueField="ChargeSite" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetFacilityChargeSite" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlRefenceCode" runat="server" Text="${Facility.FacilityMaster.RefenceCode}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbRefenceCode" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlChargeOrganization" runat="server" Text="${Facility.FacilityMaster.ChargeOrganization}:" />
            </td>
            <td class="td02">
                <%-- <asp:TextBox ID="tbChargeOrganization" runat="server" />--%>
                <uc3:textbox ID="tbChargeOrganization" runat="server" Visible="true" Width="250"
                    DescField="ChargeOrganization" ValueField="ChargeOrganization" ServicePath="FacilityMasterMgr.service"
                    ServiceMethod="GetFacilityChargeOrganization" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblIsOffBalance" runat="server" Text="${Facility.FacilityMaster.IsOffBalance}:" />
                <asp:CheckBox ID="cbIsOffBalance" runat="server" />
            </td>
            <td class="td02">
                <asp:Literal ID="lblIsOffBalanceAll" runat="server" Text="${Facility.FacilityMaster.All}:" />
                <asp:CheckBox ID="cbIsOffBalanceAll" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlMaintainGroup" runat="server" Text="${Facility.FacilityMaster.MaintainGroup}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbMaintainGroup" runat="server" Visible="true" Width="250" DescField="MaintainGroup"
                    ValueField="MaintainGroup" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetMaintainGroupList" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlOwnerDescription" runat="server" Text="${Facility.FacilityMaster.OwnerDescription}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbOwnerDescription" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
            </td>
            <td class="td02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <cc1:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <asp:Button ID="btnCreateMaintain" runat="server" Text="${Common.Button.CreateMaintain}"
                        OnClick="btnCreateMaintain_Click" CssClass="query" />
                    <cc1:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
                        CssClass="add" FunctionId="CreateFacility" />
                    <cc1:Button ID="btnImport" runat="server" Text="${Common.Button.ImportMaintainPlan}"
                        OnClick="btnImport_Click" CssClass="button2" FunctionId="CreateFacility" />
                    <cc1:Button ID="btnPrint" runat="server" Text="${Common.Button.Print}" OnClick="btnPrint_Click"
                        CssClass="button2" FunctionId="CreateFacility" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
