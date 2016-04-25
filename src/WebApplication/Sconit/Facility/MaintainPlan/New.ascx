<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_MaintainPlan_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Src="~/Controls/FileUpload.ascx" TagName="FileUpload" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.ui-zendesk.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<script language="javascript" type="text/javascript" src="Js/tag-it.js"></script>
<script language="javascript" type="text/javascript">

    $(document).ready(function () {

        BindAssignStartUser();
    });

    function BindAssignStartUser() {
        Sys.Net.WebServiceProxy.invoke('Webservice/UserMgrWS.asmx', 'GetAllUser', false,
                                {},
                            function OnSucceeded(result, eventArgs) {
                                //alert("第" + times + "次追加数据.");
                                if (result != null) {
                                    var tags = result;

                                    $('#<%=this.FV_MaintainPlan.FindControl("tbAssignStartUser").ClientID %>').tagit({
                                        availableTags: tags,
                                        allowNotDefinedTags: false,
                                        removeConfirmation: true
                                    });

                                }
                            },
            function OnFailed(error) {
                alert(error.get_message());
            }
        );
    }
</script>
<div id="divFV">
    <asp:FormView ID="FV_MaintainPlan" runat="server" DataSourceID="ODS_MaintainPlan"
        DefaultMode="Insert" Width="100%" DataKeyNames="FCID">
        <InsertItemTemplate>
            <fieldset>
                <legend>${Facility.MaintainPlan.NewMaintainPlan}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCode" runat="server" Text="${Facility.MaintainPlan.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvCode" runat="server" ErrorMessage="${Facility.MaintainPlan.Code.Required}"
                                Display="Dynamic" ControlToValidate="tbCode" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvInsert" runat="server" ControlToValidate="tbCode" ErrorMessage="${Facility.MaintainPlan.Code.Exists}"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkMaintainPlanExists" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlDescription" runat="server" Text="${Facility.MaintainPlan.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDescription" runat="server" Text='<%# Bind("Description") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlType" runat="server" Text="${Facility.MaintainPlan.Type}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlType" Code="FacilityMaintainType" runat="server" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlTypePeriod" runat="server" Text="${Facility.MaintainPlan.TypePeriod}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbTypePeriod" runat="server" Text='<%# Bind("TypePeriod") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlPeriod" runat="server" Text="${Facility.MaintainPlan.Period}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPeriod" runat="server" Text='<%# Bind("Period") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlLeadTime" runat="server" Text="${Facility.MaintainPlan.LeadTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbLeadTime" runat="server" Text='<%# Bind("LeadTime") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCategory" runat="server" Text="${Facility.FacilityMaster.Category}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbCategory" runat="server" Visible="true" Width="250" DescField="Description"
                                Text='<%# Bind("FacilityCategory") %>' ValueField="Code" ServicePath="FacilityCategoryMgr.service"
                                ServiceMethod="GetAllFacilityCategory" />
                        </td>
                        <td class="td01">
                        </td>
                        <td class="td02">
                        </td>
                    </tr>
                      <tr>
                        <td class="td01">
                            <asp:Literal ID="lblAssignStartUser" runat="server" Text="${ISI.TSK.AssignStartUser}:" />
                        </td>
                        <td class="td02" colspan="5">
                            <asp:TextBox ID="tbAssignStartUser" runat="server" CssClass="inputRequired" Text='<%# Bind("StartUpUser") %>'
                                Width="100%" />
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
                                <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </InsertItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_MaintainPlan" runat="server" TypeName="com.Sconit.Web.MaintainPlanMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.MaintainPlan" InsertMethod="CreateMaintainPlan"
    OnInserted="ODS_MaintainPlan_Inserted" OnInserting="ODS_MaintainPlan_Inserting">
    <InsertParameters>
        <asp:Parameter Name="TypePeriod" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Period" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="LeadTime" Type="Int32" ConvertEmptyStringToNull="true" />
    </InsertParameters>
</asp:ObjectDataSource>
