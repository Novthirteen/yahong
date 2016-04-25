<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_FacilityTrans_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_FacilityTrans" runat="server" DataSourceID="ODS_FacilityTrans"
        DefaultMode="Edit" Width="100%" DataKeyNames="FCID" OnDataBound="FV_FacilityTrans_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityTrans.UpdateFacilityTrans}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityMaster.FCID}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblFCID" runat="server" Text='<%# Bind("FCID") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlTransType" runat="server" Text="${Facility.FacilityTrans.TransType}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbTransType" runat="server" Text='<%# Bind("TransType") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityMaster.Category}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbCategory" runat="server" Text='<%# Bind("FacilityCategory") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlEffDate" runat="server" Text="${Facility.FacilityTrans.EffDate}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbEffDate" runat="server" Text='<%# Bind("EffDate") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFromChargePerson" runat="server" Text="${Facility.FacilityTrans.FromChargePerson}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbFromChargePerson" runat="server" Text='<%# Bind("FromChargePerson") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlToChargePerson" runat="server" Text="${Facility.FacilityTrans.ToChargePerson}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbToChargePerson" runat="server" Visible="true" Width="250" DescField="Name"
                                ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFromChargePersonName" runat="server" Text="${Facility.FacilityTrans.FromChargePersonName}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbFromChargePersonName" runat="server" Text='<%# Bind("FromChargePersonName") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlToChargePersonName" runat="server" Text="${Facility.FacilityTrans.ToChargePersonName}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbToChargePersonName" runat="server" Text='<%# Bind("ToChargePersonName") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFromChargeSite" runat="server" Text="${Facility.FacilityTrans.FromChargeSite}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbFromChargeSite" runat="server" Text='<%# Bind("FromChargeSite") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlToChargeSite" runat="server" Text="${Facility.FacilityTrans.ToChargeSite}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbToChargeSite" runat="server" Text='<%# Bind("ToChargeSite") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFromOrganization" runat="server" Text="${Facility.FacilityTrans.FromOrganization}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbFromOrganization" runat="server" Text='<%# Bind("FromOrganization") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlToOrganization" runat="server" Text="${Facility.FacilityTrans.ToOrganization}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbToOrganization" runat="server" Text='<%# Bind("ToOrganization") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlStartDate" runat="server" Text="${Facility.FacilityTrans.StartDate}:"></asp:Literal>
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" Text='<%# Bind("StartDate") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlEndDate" runat="server" Text="${Facility.FacilityTrans.EndDate}:"></asp:Literal>
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" Text='<%# Bind("EndDate") %>'/>
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlRemark" runat="server" Text="${Facility.FacilityTrans.Remark}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbRemark" runat="server" Text='<%# Bind("Remark") %>' TextMode="MultiLine"
                                Height="50" Width="75%" />
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
                                <asp:Button ID="btnEdit" runat="server" Text="${Common.Button.Save}" OnClick="btnSave_Click"
                                    CssClass="back" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_FacilityTrans" runat="server" TypeName="com.Sconit.Web.FacilityTransMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityTrans" SelectMethod="LoadFacilityTrans">
    <SelectParameters>
        <asp:Parameter Name="Id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
