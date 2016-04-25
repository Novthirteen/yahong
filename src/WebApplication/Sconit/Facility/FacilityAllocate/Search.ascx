<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Facility_FacilityAllocate_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityAllocate.FCID}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFCID" runat="server" Visible="true" Width="250" DescField="AssetNo"
                    ValueField="FCID" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetAllFacilityMaster" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlItemCode" runat="server" Text="${Facility.FacilityAllocate.ItemCode}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbItemCode" runat="server" Visible="true" Width="250" DescField="Description"
                    ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlRefenceCode" runat="server" Text="${Facility.FacilityMaster.RefenceCode}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbRefenceCode" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityMaster.Category}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbCategory" runat="server" Visible="true" Width="250" DescField="Description"
                    ValueField="Code" ServicePath="FacilityCategoryMgr.service" ServiceMethod="GetAllFacilityCategory" />
            </td>
        </tr>
   
        <tr>
            <td colspan="3">
            </td>
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
