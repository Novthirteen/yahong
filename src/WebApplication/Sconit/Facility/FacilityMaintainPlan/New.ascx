<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_MaintainPlan_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV">
    <fieldset>
        <legend>${Facility.MaintainPlan.NewMaintainPlan}</legend>
        <table class="mtable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlCode" runat="server" Text="${Facility.MaintainPlan.Code}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbCode" runat="server" Visible="true" Width="250" DescField="Description"
                        ValueField="Code" ServicePath="MaintainPlanMgr.service" ServiceMethod="GetMaintainPlanList"
                        CssClass="inputRequired" />
                </td>
                <td class="td01">
                    <asp:Literal ID="ltlStartDate" runat="server" Text="${Facility.MaintainPlan.StartDate}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbStartDate" runat="server" Text='<%# Bind("StartDate") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlStartQty" runat="server" Text="${Facility.MaintainPlan.StartQty}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbStartQty" runat="server" Text='<%# Bind("StartQty") %>' />
                </td>
                <td class="td01">
                </td>
                <td class="td02">
                </td>
            </tr>
            <tr>
                <td class="td01">
                </td>
                <td class="td02">
                </td>
                <td class="td01">
                </td>
                <td class="td02">
                    <div class="buttons">
                        <asp:Button ID="btnInsert" runat="server" Text="${Common.Button.Save}" OnClick="btnSave_Click"
                            CssClass="apply" />
                        <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                            CssClass="back" />
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
</div>
