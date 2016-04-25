<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_RepairOrder_New" %>
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

  
</script>

<fieldset>
    <legend>${Common.New}</legend>
    <table class="mtable">
        <tr>
            <td class="td01">报修部门：</td>
            <td class="td02">
                <asp:TextBox ID="tbSubmitDept" runat="server" CssClass="inputRequired"/>
                <asp:RequiredFieldValidator ID="rfvSubmitDept" runat="server" ControlToValidate="tbSubmitDept"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
            <td class="td01">报修时间：</td>
            <td class="td02">
                <asp:TextBox ID="tbSubmitTime" runat="server" CssClass="inputRequired" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                <asp:RequiredFieldValidator ID="rfvSubmitTime" runat="server" ControlToValidate="tbSubmitTime"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
        </tr>
        <tr>
            <td class="td01">设备编号：</td>
            <td class="td02">
                <uc3:textbox ID="tbFCID" runat="server" Visible="true" Width="250" DescField="AssetNo"
                    ValueField="FCID" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetAllFacilityMaster" />

                <asp:RequiredFieldValidator ID="rfvFCID" runat="server" ControlToValidate="tbFCID"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
            <td class="td01"></td>
            <td class="td02">
            </td>
        </tr>
        <tr>
            <td class="td01">停机开始：</td>
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
            <td class="td01">故障现象描述:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbFaultDescription" runat="server" TextMode="MultiLine" Height="40" Width="77%" />
                <asp:RequiredFieldValidator ID="rfvFaultDescription" runat="server" ControlToValidate="tbFaultDescription"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
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

                    
