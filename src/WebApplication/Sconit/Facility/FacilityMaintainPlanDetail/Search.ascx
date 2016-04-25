<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Facility_FacilityMaintainPlanDetail_Search" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityMaster.FCID}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbFCID" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlName" runat="server" Text="${Facility.FacilityMaster.Name}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbName" runat="server" />
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
                <asp:Literal ID="ltlMaintainPlanCode" runat="server" Text="${Facility.MaintainPlan.Code}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbMaintainPlanCode" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td>
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td align="right">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td>
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlMaintainPlanDescription" runat="server" Text="${Facility.MaintainPlan.Description}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbMaintainPlanDescription" runat="server" />
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
