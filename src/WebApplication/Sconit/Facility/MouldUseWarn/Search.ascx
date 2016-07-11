<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Facility_MouldUseWarn_Search" %>
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
                <asp:Literal ID="ltlReferenceCode" runat="server" Text="${Facility.FacilityMaster.ReferenceCode.Mould}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbReferenceCode" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityMaster.Category.Mould}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbCategory" runat="server" Visible="true" Width="250" DescField="Description"
                    ValueField="Code" ServicePath="FacilityCategoryMgr.service" ServiceMethod="GetAllMouldCategory" />
            </td>
             <td class="td01">
                <asp:Literal ID="ltlIsOverUse" runat="server" Text="${Facility.FacilityMaster.OverUse.Mould}:" />
            </td>
            <td class="td02">
              <asp:CheckBox ID="cbIsOverUse" runat="server" Checked="true"/>
            </td>
        </tr>
        <tr>
            <td colspan="3">
            </td>
            <td class="td02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                        CssClass="query" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click"
                        CssClass="query" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
