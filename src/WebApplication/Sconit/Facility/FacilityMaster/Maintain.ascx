<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Maintain.ascx.cs" Inherits="Facility_FacilityMaster_Maintain" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_FacilityMaintain" runat="server" DataSourceID="ODS_FacilityMaintain"
        DefaultMode="Edit" Width="100%" DataKeyNames="FCID" OnDataBound="FV_FacilityMaintain_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityMaintain.UpdateFacilityMaintain}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlMaintainType" runat="server" Text="${Facility.FacilityMaster.MaintainType}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlMaintainType" Code="FacilityMaintainType" runat="server"
                                IncludeBlankOption="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlMaintainStartDate" runat="server" Text="${Facility.FacilityMaster.MaintainStartDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbMaintainStartDate" runat="server" Text='<%# Bind("MaintainStartDate") %>'
                                onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlMaintainPeriod" runat="server" Text="${Facility.FacilityMaster.MaintainPeriod}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbMaintainPeriod" runat="server" Text='<%# Bind("MaintainPeriod") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlMaintainLeadTime" runat="server" Text="${Facility.FacilityMaster.MaintainLeadTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbMaintainLeadTime" runat="server" Text='<%# Bind("MaintainLeadTime") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlNextMaintainTime" runat="server" Text="${Facility.FacilityMaster.NextMaintainTime}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbNextMaintainTime" runat="server" Text='<%# Bind("NextMaintainTime") %>' />
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
                                <asp:Button ID="btnEdit" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" />
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
<asp:ObjectDataSource ID="ODS_FacilityMaintain" runat="server" TypeName="com.Sconit.Web.FacilityMasterMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityMaster" UpdateMethod="UpdateFacilityMasterMaintain"
    OnUpdated="ODS_FacilityMaintain_Updated" SelectMethod="LoadFacilityMaster" OnUpdating="ODS_FacilityMaintain_Updating">
    <SelectParameters>
        <asp:Parameter Name="fcId" Type="String" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Name="MaintainStartDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaintainPeriod" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaintainLeadTime" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="NextMaintainTime" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaintainTypePeriod" Type="Int32" ConvertEmptyStringToNull="true" />
    </UpdateParameters>
</asp:ObjectDataSource>
