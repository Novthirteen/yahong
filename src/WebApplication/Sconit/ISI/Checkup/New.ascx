<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_Checkup_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.ui-zendesk.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<script language="javascript" type="text/javascript" src="Js/tag-it.js"></script>
<script language="javascript" type="text/javascript">

    function BindAssignStartUser() {

        Sys.Net.WebServiceProxy.invoke('Webservice/UserMgrWS.asmx', 'GetAllUser', false,
                {},
            function OnSucceeded(result, eventArgs) {
                //alert("第" + times + "次追加数据.");
                if (result != null) {
                    var tags = result;

                    $('#<%=tbCheckupUser.ClientID %>').tagit({
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

        $(document).ready(function () {
            BindAssignStartUser();
            $('textarea').tah({
                moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
                maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
                animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
            });
        });

</script>
<div>
    <fieldset>
        <legend>${ISI.Checkup.NewCheckup}</legend>
        <table class="mtable">

            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlCheckupDate" runat="server" Text="${ISI.Checkup.CheckupDate}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbCheckupDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvCheckupDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbCheckupDate" ValidationGroup="vgSave" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblCheckupProject" runat="server" Text="${ISI.Checkup.CheckupProject}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbCheckupProject" runat="server" Visible="true" DescField="Desc" MustMatch="true"
                        ValueField="Code" ServicePath="CheckupProjectMgr.service" ServiceMethod="GetAllCheckupProject"
                        Width="200" CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvCheckupProject" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbCheckupProject" ValidationGroup="vgSave" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblCheckupUser" runat="server" Text="${ISI.Checkup.CheckupUser}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbCheckupUser" runat="server" CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvCheckupUser" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbCheckupUser" ValidationGroup="vgSave" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlContent" runat="server" Text="${ISI.Checkup.Content}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbContent" runat="server" Text='<%# Bind("Content") %>' Height="60"
                        Width="650" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,500);"
                        onpaste="limitPaste(this, 500)" Font-Size="10" CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvContent" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbContent" ValidationGroup="vgSave" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlAmount" runat="server" Text="${ISI.Checkup.Amount}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbAmount" runat="server" Text='<%# Bind("Amount") %>' />
                    <asp:RangeValidator ID="rvAmount" runat="server" ErrorMessage="${Common.Validator.Valid.Number}"
                        Display="Dynamic" Type="Double" MaximumValue="10000" MinimumValue="-10000" ControlToValidate="tbAmount" ValidationGroup="vgSave" />
                    <span style="color: mediumvioletred;">${ISI.Checkup.AmountMsg}</span>
                </td>
                <td class="td01">
                    <asp:CheckBox runat="server" Text="${ISI.Checkup.IsAutoRelease}" ID="IsAutoRelease" />
                </td>
                <td class="td02"></td>
            </tr>

        </table>
    </fieldset>
    <fieldset id="fsApprove" runat="server" visible="false">
        <legend>${ISI.Checkup.Approve}</legend>
        <table class="mtable">
            <div>
                <tr>
                    <td class="td01">
                        <asp:Literal ID="ltlAuditInstructions" runat="server" Text="${ISI.Checkup.AuditInstructions}:" />
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbAuditInstructions" runat="server" Text='<%# Bind("AuditInstructions") %>' Height="50"
                            Width="650" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,500);"
                            onpaste="limitPaste(this, 500)" Font-Size="10" />
                    </td>
                </tr>
            </div>
        </table>
    </fieldset>
    <table class="mtable">
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="td02">
                <div class="buttons">
                    <asp:Button ID="btnInsert" runat="server" OnClick="btnSave_Click" Text="${Common.Button.Save}"
                        CssClass="apply" ValidationGroup="vgSave" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="back" />
                </div>
            </td>
        </tr>
    </table>
</div>
