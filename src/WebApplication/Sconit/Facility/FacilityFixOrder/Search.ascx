<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Facility_FacilityFixOrder_Search" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="ttd01">
                <asp:Literal ID="lblFixNo" runat="server" Text="${Facility.FacilityFixOrder.FixNo}:" />
            </td>
            <td class="ttd02">
                <asp:TextBox ID="tbFixNo" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblStatus" runat="server" Text="${Common.CodeMaster.Status}:" />
            </td>
            <td class="td02">
                <asp:DropDownList ID="ddlStatus" runat="server" DataTextField="Description" DataValueField="Value" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlStartDate" runat="server" Text="${MasterData.Common.CreateDateFrom}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlEndDate" runat="server" Text="${MasterData.Common.CreateDateTo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblFCID" runat="server" Text="${Facility.FacilityFixOrder.FCID}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbFCID" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblReferenceNo" runat="server" Text="${Facility.FacilityFixOrder.ReferenceNo}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbReferenceNo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblCreateUser" runat="server" Text="${MasterData.Common.CreateUser}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbCreateUser" runat="server" />
            </td>
            <td class="td01">
            </td>
            <td class="ttd02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                    CssClass="button2" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
</fieldset>
