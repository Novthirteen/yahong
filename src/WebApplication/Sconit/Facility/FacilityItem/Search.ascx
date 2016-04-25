<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Facility_FacilityItem_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityItem.FCID}:" />
            </td>
            <td class="td02">
                <%-- <uc3:textbox ID="tbFCID" runat="server" Visible="true" Width="250" DescField="AssetNo"
                    ValueField="FCID" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetAllFacilityMaster" />--%>
                <asp:TextBox ID="tbFCID" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlItemCode" runat="server" Text="${Facility.FacilityItem.ItemCode}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbItemCode" runat="server" Visible="true" Width="250" DescField="Description"
                    ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlAllocateType" runat="server" Text="${Facility.FacilityItem.AllocateType}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlAllocateType" Code="FacilityAllocateType" runat="server" IncludeBlankOption="true" DefaultSelectedValue="" />
            </td>
            <td class="td01">
            </td>
            <td class="td02">
                <asp:CheckBox ID="cbIsActive" runat="server" Checked='true' Text="${Facility.FacilityItem.IsActive}" />
                <asp:CheckBox ID="cbIsInitQty" runat="server" Checked='false' Text="${Facility.FacilityItem.IsInitQty}" />
                <asp:CheckBox ID="cbIsWarn" runat="server" Checked='false' Text="${Facility.FacilityItem.IsWarn}" />
            </td>
        </tr>
        <%-- <tr>
            <td class="td01">
                <asp:Literal ID="ltlAssetNo" runat="server" Text="${Facility.FacilityMaster.AssetNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbAssetNo" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityMaster.Category}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbCategory" runat="server" Visible="true" Width="250" DescField="Description"
                    ValueField="Code" ServicePath="FacilityCategoryMgr.service" ServiceMethod="GetAllFacilityCategory" />
            </td>
        </tr>--%>
        <%--  <tr>
            <td class="td01">
                <asp:Literal ID="ltlIsAllocate" runat="server" Text="${Facility.FacilityItem.IsAllocate}:" />
            </td>
            <td class="td02">
                <asp:CheckBox ID="cbIsAllocate" runat="server" Checked="true" />
            </td>
        </tr>--%>
        <tr>
            <td colspan="3"></td>
            <td class="td02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <cc1:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
                        CssClass="add" FunctionId="CreateFacility" />
                    <cc1:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click"
                        CssClass="query" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
