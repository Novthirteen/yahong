﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_FacilityFixOrder_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <fieldset>
        <legend>${Facility.FixOrder.FixOrder}</legend>
        <table class="mtable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlFixNo" runat="server" Text="${Facility.FacilityFixOrder.FixNo}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbFixNo" runat="server" ReadOnly="true" />
                </td>
                <td class="td01">
                </td>
                <td class="td02">
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FixOrder.FCID}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbFCID" runat="server" ReadOnly="true" />
                </td>
                <td class="td01">
                    <asp:Literal ID="ltlFacilityName" runat="server" Text="${Facility.FixOrder.FacilityName}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbFacilityName" runat="server" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlReferenceNo" runat="server" Text="${Facility.FixOrder.ReferenceNo}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbReferenceNo" runat="server" ReadOnly="true" />
                </td>
                <td class="td01">
                    <asp:Literal ID="ltlEffectiveDate" runat="server" Text="${Facility.FixOrder.EffectiveDate}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbEffectiveDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})"
                        ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlShift" runat="server" Text="${Facility.FixOrder.Shift}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbShift" runat="server" ReadOnly="true" />
                </td>
                <td class="td01">
                    <asp:Literal ID="ltlCustomer" runat="server" Text="${Facility.FixOrder.Customer}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbCustomer" runat="server" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlIsSample" runat="server" Text="${Facility.FixOrder.IsSample}:" />
                </td>
                <td class="td02">
                    <asp:CheckBox ID="cbIsSample" runat="server" Enabled="false" />
                </td>
                <td class="td01">
                    <asp:Literal ID="ltlFixSite" runat="server" Text="${Facility.FixOrder.FixSite}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbFixSite" runat="server"  ReadOnly="true"/>
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlDescription" runat="server" Text="${Facility.FixOrder.Description}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbDescription" runat="server" TextMode="MultiLine" Height="50" Width="75%"
                        ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlResult" runat="server" Text="${Facility.FixOrder.Result}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbResult" runat="server" TextMode="MultiLine" Height="50" Width="75%"  ReadOnly="true"/>
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
                        <cc1:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="${Common.Button.Submit}"
                            CssClass="apply" FunctionId="SubmitFixOrder" ValidationGroup="vgSubmit" />
                        <cc1:Button ID="btnStart" runat="server" OnClick="btnStart_Click" Text="${Common.Button.Start}"
                            CssClass="apply" FunctionId="StartFixOrder" />
                        <cc1:Button ID="btnClose" runat="server" OnClick="btnClose_Click" Text="${Common.Button.Close}"
                            CssClass="apply" FunctionId="CloseFixOrder" />
                        <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                            CssClass="back" />
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
</div>
