<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_FacilityAllocate_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
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

                                    $('#<%=this.FV_FacilityAllocate.FindControl("tbAssignStartUser").ClientID %>').tagit({
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
    <asp:FormView ID="FV_FacilityAllocate" runat="server" DataSourceID="ODS_FacilityAllocate"
        DefaultMode="Insert" Width="100%" DataKeyNames="FCID">
        <InsertItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityAllocate.NewFacilityAllocate}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityAllocate.FCID}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbFCID" runat="server" Visible="true" Width="250" DescField="AssetNo"
                                ValueField="FCID" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetAllFacilityMaster" />
                            <asp:RequiredFieldValidator ID="rfvFCID" runat="server" ErrorMessage="${Facility.FacilityAllocate.FCID.Required}"
                                Display="Dynamic" ControlToValidate="tbFCID" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvInsert" runat="server" ControlToValidate="tbFCID" ErrorMessage="${Facility.FacilityAllocate.Exists}"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkFacilityAllocateExists" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlItemCode" runat="server" Text="${Facility.FacilityAllocate.ItemCode}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbItemCode" runat="server" Visible="true" Width="250" DescField="Description"
                                ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem" />
                            <asp:RequiredFieldValidator ID="rfvItemCode" runat="server" ErrorMessage="${Facility.FacilityAllocate.ItemCode.Required}"
                                Display="Dynamic" ControlToValidate="tbItemCode" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsActive" runat="server" Text="${Facility.FacilityAllocate.IsActive}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlAllocateType" runat="server" Text="${Facility.FacilityAllocate.AllocateType}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlAllocateType" Code="MouldAllocateType" runat="server" />
                        </td>
                    </tr>
                      <tr>
                       <td class="td01">
                            <asp:Literal ID="ltlWarnQty" runat="server" Text="${Facility.FacilityAllocate.WarnQty}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbWarnQty" runat="server" Text='<%# Bind("WarnQty","{0:0.##}") %>' />
                             <asp:RequiredFieldValidator ID="rfvWarnQty" runat="server" ErrorMessage="${Facility.FacilityAllocate.WarnQty.Required}"
                                Display="Dynamic" ControlToValidate="tbWarnQty" ValidationGroup="vgSave" />
                        </td>
                      </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblAssignStartUser" runat="server" Text="${ISI.TSK.AssignStartUser}:" />
                        </td>
                        <td class="td02" colspan="5">
                            <asp:TextBox ID="tbAssignStartUser" runat="server" CssClass="inputRequired" Text='<%# Bind("StartUpUser") %>'
                                Width="100%" />
                            <asp:RequiredFieldValidator ID="rfvAssignStartUser" runat="server" ErrorMessage="${Facility.FacilityAllocate.AssignStartUser.Required}"
                                Display="Dynamic" ControlToValidate="tbAssignStartUser" ValidationGroup="vgSave" />
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
<asp:ObjectDataSource ID="ODS_FacilityAllocate" runat="server" TypeName="com.Sconit.Web.FacilityAllocateMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityAllocate" InsertMethod="CreateFacilityAllocate"
    OnInserted="ODS_FacilityAllocate_Inserted" OnInserting="ODS_FacilityAllocate_Inserting">
</asp:ObjectDataSource>
