<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Facility_FacilityBatchTransfer_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <%--<td class="td01">
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
                <asp:Literal ID="ltlAssetNo" runat="server" Text="${Facility.FacilityMaster.AssetNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbAssetNo" runat="server" />
            </td>--%>
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
                </div>
            </td>
        </tr>
    </table>
</fieldset>

