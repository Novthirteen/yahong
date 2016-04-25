<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityBatchTransfer_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<script type="text/javascript" language="javascript">
    function GVCheckClick() {
        if ($(".GVHeader input:checkbox").attr("checked") == 'checked') {
            //debugger
            $(".GVRow input[disabled!='disabled']:checkbox").attr("checked", true);
            $(".GVAlternatingRow input[disabled!='disabled']:checkbox").attr("checked", true);
        }
        else {
            $(".GVRow input[disabled!='disabled']:checkbox").attr("checked", false);
            $(".GVAlternatingRow input[disabled!='disabled']:checkbox").attr("checked", false);
        }
    }
</script>

<fieldset>
    <div class="GridView">
        <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" OnRowDataBound="GV_List_RowDataBound" DataKeyNames="FCID">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div onclick="GVCheckClick()">
                            <asp:CheckBox ID="CheckAll" runat="server" />
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBoxGroup" name="CheckBoxGroup" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.FCID}" SortExpression="FCID">
                    <ItemTemplate>
                        <asp:Literal ID="ltlFCID" runat="server" Text='<%# Eval("FCID")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="AssetNo" HeaderText="${Facility.FacilityMaster.AssetNo}"
                    SortExpression="AssetNo" />
                <asp:BoundField DataField="Name" HeaderText="${Facility.FacilityMaster.Name}" SortExpression="Name" />
                <asp:BoundField DataField="Capacity" HeaderText="${Facility.FacilityMaster.Capacity}"
                    SortExpression="Capacity" />
                <asp:BoundField DataField="ManufactureDate" HeaderText="${Facility.FacilityMaster.ManufactureDate}"
                    SortExpression="ManufactureDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SerialNo" HeaderText="${Facility.FacilityMaster.SerialNo}"
                    SortExpression="SerialNo" />
                <asp:BoundField DataField="WarrantyInfo" HeaderText="${Facility.FacilityMaster.WarrantyInfo}"
                    SortExpression="WarrantyInfo" />
                <asp:BoundField DataField="EffDate" HeaderText="${Facility.FacilityMaster.EffDate}"
                    SortExpression="EffDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Price" HeaderText="${Facility.FacilityMaster.Price}" SortExpression="Price" />
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Owner}" SortExpression="Owner">
                    <ItemTemplate>
                        <asp:Label ID="lblOwner" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Status}" SortExpression="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CurrChargePersonName" HeaderText="${Facility.FacilityMaster.CurrChargePerson}"
                    SortExpression="CurrChargePersonName" />
                <asp:BoundField DataField="ChargeSite" HeaderText="${Facility.FacilityMaster.ChargeSite}"
                    SortExpression="ChargeSite" />
                <asp:BoundField DataField="ChargeDate" HeaderText="${Facility.FacilityMaster.ChargeDate}"
                    SortExpression="ChargeDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Category}" SortExpression="Category">
                    <ItemTemplate>
                        <asp:Label ID="lblCategory" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%-- <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnTransfer" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FCID") %>'
                            Text="${Common.Button.TransferFacility}" OnClick="lbtnTransfer_Click" FunctionId="TransferFacility">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>--%>
            </Columns>
        </asp:GridView>
    </div>
</fieldset>
<div class="tablefooter">
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlTransferPerson" runat="server" Text="${Facility.FacilityTransfer.TransferPerson}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbTransferPerson" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" MustMatch="true" />
                <asp:RequiredFieldValidator ID="rfvTransferPerson" runat="server" ErrorMessage="${Facility.FacilityTransfer.TransferPerson.Required}"
                    Display="Dynamic" ControlToValidate="tbTransferPerson" ValidationGroup="vgSave" />
            </td>
            <td class="td01">
                <asp:Literal ID="ltlTransferSite" runat="server" Text="${Facility.FacilityTransfer.TransferSite}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbTransferSite" runat="server" Visible="true" Width="250" DescField="ChargeSite"
                    ValueField="ChargeSite" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetFacilityChargeSite" />
                <asp:RequiredFieldValidator ID="rfvTransferSite" runat="server" ErrorMessage="${Facility.FacilityTransfer.TransferSite.Required}"
                    Display="Dynamic" ControlToValidate="tbTransferSite" ValidationGroup="vgSave" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltTransferOrg" runat="server" Text="${Facility.FacilityTransfer.TransferOrg}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbTransferOrg" runat="server" Visible="true" Width="250" DescField="ChargeOrganization"
                    ValueField="ChargeOrganization" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetFacilityChargeOrganization" />
                <%-- <asp:TextBox ID="tbApplyOrg" runat="server" Text='' />--%>
                <asp:RequiredFieldValidator ID="rfvTransferOrg" runat="server" ErrorMessage="${Facility.FacilityTransfer.TransferOrg.Required}"
                    Display="Dynamic" ControlToValidate="tbTransferOrg" ValidationGroup="vgSave" />
            </td>
        </tr>

        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="td02">
                <div class="buttons">
                    <asp:Button ID="btnSave" runat="server" OnClick="lbtnTransfer_Click" Text="${Common.Button.TransferFacility}"
                        CssClass="apply" ValidationGroup="vgSave" />

                </div>
            </td>
        </tr>
    </table>
</div>
