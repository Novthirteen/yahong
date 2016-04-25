<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_RepairOrder_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.ui-zendesk.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<script language="javascript" type="text/javascript" src="Js/tag-it.js"></script>

<style type="text/css">
    .CssColor {
        text-align: center;
        height: 18px;
        vertical-align: middle;
    }
    .button2 {
        height: 21px;
    }
</style>

<script type="text/javascript" language="javascript">
    function qtyChanged(obj) {
        var minValue = $(obj).attr("title").split(",")[0];
        var maxValue = $(obj).attr("title").split(",")[1];
        var thisValue = $(obj).val();
        if (thisValue < minValue || thisValue > maxValue) {
            $(obj).css("background-color", "red");
        }
    }

    $(document).ready(function () {
        BindAssignStartUser();
    });

    function BindAssignStartUser() {
        Sys.Net.WebServiceProxy.invoke('Webservice/UserMgrWS.asmx', 'GetAllUser', false,
                                {},
                            function OnSucceeded(result, eventArgs) {
                                //alert("��" + times + "��׷������.");
                                if (result != null) {
                                    var tags = result;
                                    $('#<%=tbRepairUser.ClientID %>').tagit({
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

    <table class="mtable">
        <tr>
            <td class="td01">�豸���޵���:</td>
            <td class="td02">
                <asp:Label ID="lblCode" runat="server" />
            </td>
            <td class="td01">״̬:</td>
            <td class="td02">
                <asp:Label ID="lblStatus" runat="server" />
            </td>
        </tr>
        </table>
<fieldset>
    <legend>������д</legend>
    <table class="mtable">
        <tr>
            <td class="td01">���޲��ţ�</td>
            <td class="td02">
                <asp:TextBox ID="tbSubmitDept" runat="server" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvSubmitDept" runat="server" ControlToValidate="tbSubmitDept"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
            <td class="td01">����ʱ�䣺</td>
            <td class="td02">
                <asp:TextBox ID="tbSubmitTime" runat="server" CssClass="inputRequired" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                <asp:RequiredFieldValidator ID="rfvSubmitTime" runat="server" ControlToValidate="tbSubmitTime"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
        </tr>
        <tr>
            <td class="td01">�豸��ţ�</td>
            <td class="td02">
                <asp:Label ID="lblFCID" runat="server" />
            </td>
            <td class="td01">�豸���ƣ�</td>
            <td class="td02">
                <asp:Label ID="lblFCName" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">�ڲ��ʲ���</td>
            <td class="td02" colspan="3">
                <asp:Label ID="lblAssetNo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">ͣ����ʼ��</td>
            <td class="td02">
                <asp:TextBox ID="tbHaltStartTime" runat="server" CssClass="inputRequired" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                <asp:RequiredFieldValidator ID="rfvHaltStartTime" runat="server" ControlToValidate="tbHaltStartTime"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
            <td class="td01"></td>
            <td class="td02">
                
            </td>
        </tr>
        <tr>
            <td class="td01">������������:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbFaultDescription" runat="server" TextMode="MultiLine" Height="40" Width="77%"/>
            </td>

        </tr>
        <tr>
            <td class="td01">������Ա:</td>
            <td class="td02">
                <asp:Label ID="lblSubmitUserName" runat="server" />
            </td>
            <td class="td01"></td>
            <td class="td02">
                <asp:Button ID="btnPost" runat="server" Text="����ȷ��" OnClick="btnPost_Click"
                        CssClass="button2" />
                <asp:Button ID="btnDelete" runat="server" Text="${Common.Button.Delete}"
                        CssClass="button2" OnClick="btnDelete_Click" Visible="false"/>
                <asp:Button ID="btnBack2" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="button2" />
            </td>
        </tr>
        </table>
    </fieldset>
    <fieldset id ="fldRepair" runat="server">
    <legend>ά����д</legend>
        <table class="mtable">
            <tr>
            <td class="td01">ά�޿�ʼ��</td>
            <td class="td02">
                <asp:TextBox ID="tbRepairStartTime" runat="server" CssClass="inputRequired" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                <asp:RequiredFieldValidator ID="rfvRepairStartTime" runat="server" ControlToValidate="tbRepairStartTime" 
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
            <td class="td01">ά�޽�����</td>
            <td class="td02">
                <asp:TextBox ID="tbRepairEndTime" runat="server" CssClass="inputRequired" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                <asp:RequiredFieldValidator ID="rfvRepairEndTime" runat="server" ControlToValidate="tbRepairEndTime"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
        </tr>
        <tr>
            <td class="td01" style="width:20%">
                <asp:Literal ID="ltlRepairUser" runat="server" Text="ά����Ա:" />
            </td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbRepairUser" runat="server" Width="100%" />
                <asp:RequiredFieldValidator ID="rfvRepairUser" runat="server" ControlToValidate="tbRepairUser" 
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
        </tr>

        
        <tr>
            <td class="td01">ά�޾��岽��:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbRepairDescription" runat="server" TextMode="MultiLine" Height="40" Width="77%" />
                <asp:RequiredFieldValidator ID="rfvRepairDescription" runat="server" ControlToValidate="tbRepairDescription"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>

        </tr>
        <tr>
            <td class="td01">���Ϸ���ԭ��:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbHaltReason" runat="server" TextMode="MultiLine" Height="40" Width="77%" />
                <asp:RequiredFieldValidator ID="rfvHaltReason" runat="server" ControlToValidate="tbHaltReason" 
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>

        </tr>
        <tr>
            <td class="td01">���ø�������:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbItems" runat="server" TextMode="MultiLine" Height="40" Width="77%" />
                <asp:RequiredFieldValidator ID="rfvItems" runat="server" ControlToValidate="tbItems"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>

        </tr>
        <tr>
            <td class="td01">��������</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbRefOrderNo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">ͣ��������</td>
            <td class="td02">
                <asp:TextBox ID="tbHaltEndTime" runat="server" CssClass="inputRequired" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
            </td>
            <td class="td01">������Ա:</td>
            <td class="td02">
                <asp:Label ID="lblOperateUserName" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">����ʦ/�������:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbSuggestion" runat="server" TextMode="MultiLine" Height="40" Width="77%" />
            </td>

        </tr>
        <tr>
            <td class="td01">����ʦ/����:</td>
            <td class="td02" colspan="3">
                <asp:Label ID="lblSuggestionUserName" runat="server" />
            </td>

        </tr>
        <tr>
            <td colspan="4">
                <div class="tablefooter">
                    <cc1:Button ID="btnUpdate" runat="server" Text="${Common.Button.Update}" ValidationGroup="vgSave"
                        CssClass="button2" OnClick="btnUpdate_Click" FunctionId="RepairOrder_Edit" Visible="false"/>
                    <cc1:Button ID="btnRepair" runat="server" Text="ά�����" ValidationGroup="vgSave"
                        CssClass="button2" OnClick="btnRepair_Click" FunctionId="RepairOrder_Repair" Visible="false"/>
                    <cc1:Button ID="btnAcceptance" runat="server" Text="����ȷ��"
                        CssClass="button2" OnClick="btnAcceptance_Click" FunctionId="RepairOrder_Acceptance" Visible="false" />
                    <cc1:Button ID="btnClose" runat="server" Text="${Common.Button.Close}"
                        CssClass="button2" OnClick="btnClose_Click" FunctionId="RepairOrder_Close" Visible="false" />
                    
                    <asp:Button ID="btnPrint" runat="server" Text="${Common.Button.Print}" OnClick="btnPrint_Click"
                        CssClass="button2" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnExport_Click"
                        CssClass="button2" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="button2" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>

                    
