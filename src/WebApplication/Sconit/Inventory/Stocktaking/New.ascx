<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Inventory_Stocktaking_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <fieldset>
        <table class="mtable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblRegion" runat="server" Text="${Common.Business.Region}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbRegion" runat="server" Visible="true" Width="280" DescField="Name"
                        ValueField="Code" MustMatch="true" ServicePath="RegionMgr.service" ServiceMethod="GetRegion"
                        CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvRegion" runat="server" ControlToValidate="tbRegion"
                        ErrorMessage="${Common.Business.Error.RegionInvalid}" Display="Dynamic" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblLocation" runat="server" Text="${Common.Business.Location}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbLocation" runat="server" Visible="true" DescField="Name" Width="280"
                        ValueField="Code" ServicePath="LocationMgr.service" ServiceMethod="GetLocation"
                        ServiceParameter="string:#tbRegion" CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvLocation" runat="server" ControlToValidate="tbLocation"
                        ErrorMessage="${Common.Business.Error.LocationInvalid}" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblEffDate" runat="server" Text="${Common.Business.EffDate}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbEffDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})"
                        CssClass="inputRequired" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})"
                        CssClass="inputRequired" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblType" runat="server" Text="${Common.Business.Type}:" />
                </td>
                <td class="td02">
                    <cc1:CodeMstrDropDownList ID="ddlType" runat="server" Code="PhysicalCountType" />
                </td>
                <td class="td01">
                </td>
                <td class="td02">
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblPhyCntRule" runat="server" Text="${Common.Business.PhyCntRule}:" />
                </td>
                <td class="td02">
                    <cc1:CodeMstrDropDownList ID="ddlPhyCntRule" runat="server" Code="PhyCntRule" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblPhyCntGroupBy" runat="server" Text="${Common.Business.PhyCntGroupBy}:" />
                </td>
                <td class="td02">
                    <cc1:CodeMstrDropDownList ID="ddlPhyCntGroupBy" runat="server" Code="PhyCntGroupBy" />
                </td>
            </tr>
            <tr>
                <td colspan="3" />
                <td class="td02">
                    <asp:Button ID="btnSave" runat="server" Text="${Common.Button.Save}" OnClick="btnSave_Click"
                        Width="59px" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        Width="59px" />
                </td>
            </tr>
        </table>
    </fieldset>
</div>
