<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_CheckList_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="TextBox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.ui-zendesk.css" />
<script type="text/javascript" src="Js/ui.core-min.js"></script>
<script type="text/javascript" src="Js/tag-it.js"></script>
<style type="text/css">
    .CssColor {
        text-align: center;
        height: 18px;
        vertical-align: middle;
    }
</style>

<script type="text/javascript" language="javascript">

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

                    $('#<%=tbSubUser.ClientID %>').tagit({
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

<fieldset>
    <legend>${Common.New}</legend>
    <table class="mtable">
        <tr>
            <td class="td01">代码:</td>
            <td class="td02">
                <asp:TextBox ID="tbCode" runat="server" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="tbCode"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
            <td class="td01">名称:</td>
            <td class="td02">
                <asp:TextBox ID="tbName" runat="server" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="tbName"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>


        </tr>
        <tr>
            <td class="td01">测试设备号:</td>
            <td class="td02">
                <uc3:TextBox ID="tbFacilityID" runat="server" Visible="true" Width="250" DescField="AssetNo"
                    ValueField="FCID" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetAllFacilityMaster" />
            </td>
            <td class="td01">设施名称:</td>
            <td class="td02">
                <asp:TextBox ID="tbFacilityName" runat="server" />
            </td>
        </tr>
        <tr>

            <td class="td01">区域:</td>
            <td class="td02">
                <asp:TextBox ID="tbRegion" runat="server" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvRegion" runat="server" ControlToValidate="tbRegion"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
        </tr>
        <tr>
            <td class="td01">分类:</td>
            <td class="td02">
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType" />
            </td>
            <td class="td01">创建异常问题:</td>
            <td class="td02">
                <asp:CheckBox ID="cbNeedCreateTask" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlSubUser" runat="server" Text="执行人:" />
            </td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbSubUser" runat="server" Width="100%" />
            </td>
        </tr>
        <tr>
            <td class="td01">描述:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbDescription" runat="server" TextMode="MultiLine"
                    Width="77%" Height="180" />
            </td>
        </tr>
    </table>
</fieldset>
<div class="tablefooter">
    <asp:Button ID="btnInsert" runat="server" Text="${Common.Button.Save}" OnClick="btnInsert_Click"
        CssClass="button2" ValidationGroup="vgSave" />
    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
        CssClass="button2" />
</div>
