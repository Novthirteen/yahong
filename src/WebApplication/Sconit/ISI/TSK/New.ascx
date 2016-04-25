<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_TSK_New" %>
<%@ Register Src="CostList.ascx" TagName="CostList" TagPrefix="uc2" %>
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
</style>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });

        BindAssignStartUser();

    });

    function BindAssignStartUser() {

        Sys.Net.WebServiceProxy.invoke('Webservice/UserMgrWS.asmx', 'GetAllUser', false,
                {},
            function OnSucceeded(result, eventArgs) {
                //alert("第" + times + "次追加数据.");
                if (result != null) {
                    var tags = result;

                    $('#<%=tbAssignStartUser.ClientID %>').tagit({
                        availableTags: tags,
                        allowNotDefinedTags: false,
                        removeConfirmation: true,
                        placeholderText: '${ISI.TSK.InputUserCode}',
                        onTagClicked: function (evt, ui) {
                            $('#<%=tbAssignStartUser.ClientID %>').tagit('prependTag', ui);
                        }
                    });

                        $('#<%=tbWorkHoursUser.ClientID %>').tagit({
                        availableTags: tags,
                        allowNotDefinedTags: false,
                        removeConfirmation: true,
                        placeholderText: '${ISI.TSK.InputUserCode}',
                        onTagClicked: function (evt, ui) {
                            $('#<%=tbWorkHoursUser.ClientID %>').tagit('prependTag', ui);
                        }
                        });
                    }
            },
            function OnFailed(error) {
                alert(error.get_message());
            }
           );
            }
</script>
<div>
    <fieldset>
        <legend id="lgd" runat="server"></legend>
        <table class="mtable" runat="server" id="taskTable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlTaskSubType" runat="server" Text="${ISI.TSK.TaskSubType}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                        ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                        CssClass="inputRequired" Width="260" OnTextChanged="tbTaskSubType_TextChanged" AutoPostBack="true" />
                    <asp:RequiredFieldValidator ID="rfvTaskSubType" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbTaskSubType" ValidationGroup="vgSave" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblPriority" runat="server" Text="${ISI.TSK.Priority}:" />
                </td>
                <td class="td02">
                    <cc1:CodeMstrDropDownList ID="ddlPriority" Code="ISIPriority" runat="server" IncludeBlankOption="false" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblSubject" runat="server" Text="${ISI.TSK.Subject}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbSubject" runat="server" Text='<%# Bind("Subject") %>' Width="80%"
                        onkeypress="javascript:setMaxLength(this,50);" onpaste="limitPaste(this, 50)" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblBackYards" runat="server" Text="${ISI.TSK.BackYards}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbBackYards" runat="server" Text='<%# Bind("BackYards") %>' onkeypress="javascript:setMaxLength(this,50);"
                        onpaste="limitPaste(this, 50)" />
                </td>
            </tr>
            <tr runat="server" id="trWF" visible="false">
                <td class="td01">
                    <asp:Literal ID="lblExtNo" runat="server" Text="${ISI.TSK.ExtNo}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbExtNo" runat="server" Text='<%# Bind("ExtNo") %>'
                        onkeypress="javascript:setMaxLength(this,50);" onpaste="limitPaste(this, 50)" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblCostCenter" runat="server" Text="${ISI.TSK.CostCenter}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbCostCenter" runat="server" Visible="true" DescField="Desc" MustMatch="true"
                        ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetCostCenter" OnTextChanged="tbCostCenter_TextChanged"
                        AutoPostBack="true" ServiceParameter="bool:true" CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvCostCenter" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbCostCenter" ValidationGroup="vgSave" />
                    <cc1:ReadonlyTextBox ID="rtbCostCenter" runat="server" Visible="false" />
                </td>
            </tr>
            <tr runat="server" id="trAccount" visible="false">
                <td class="td01">
                    <asp:Label ID="ltlAccount1" runat="server" Text="${WFS.Cost.Account1}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbAccount1" runat="server" Visible="true" DescField="Account1Desc" MustMatch="true"
                        ValueField="Account1" ServicePath="BudgetDetMgr.service" ServiceMethod="GetAccount1"
                        ServiceParameter="string:#tbCostCenter" CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvAccount1" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbAccount1" ValidationGroup="vgSave"  />
                    
                </td>
                <td class="td01">
                    <asp:Label ID="ltlAccount2" runat="server" Text="${WFS.Cost.Account2}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbAccount2" runat="server" Visible="true" DescField="Account2Desc" MustMatch="true"
                        ValueField="Account2" ServicePath="BudgetDetMgr.service" ServiceMethod="GetAccount2"
                        ServiceParameter="string:#tbCostCenter,string:#tbAccount1"  CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvAccount2" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbAccount2" ValidationGroup="vgSave" />
                    
                </td>
            </tr>
            <tr id="isPrj" runat="server" visible="false">
                <td class="td01">
                    <asp:Literal ID="lblPhase" runat="server" Text="${ISI.TSK.Phase}:" />
                </td>
                <td class="td02">
                    <cc1:CodeMstrDropDownList ID="ddlPhase" Code="ISIPhase" runat="server" IncludeBlankOption="false" /></td>
                <td class="td01">
                    <asp:Literal ID="lblSeq" runat="server" Text="${ISI.TSK.Seq}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbSeq" runat="server" Width="80" Text='<%# Bind("Seq") %>'></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblTaskAddress" runat="server" Text="${ISI.TSK.TaskAddress}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbTaskAddress" runat="server" Visible="true" DescField="Code" MustMatch="false"
                        ValueField="Name" ServicePath="TaskAddressMgr.service" ServiceMethod="GetCacheAllTaskAddress"
                        CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvTaskAddress" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbTaskAddress" ValidationGroup="vgSave" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblFailureMode" runat="server" Text="${ISI.TSK.FailureMode}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbFailureMode" runat="server" Visible="true" DescField="Desc" MustMatch="true"
                        ValueField="Code" ServicePath="FailureModeMgr.service" ServiceMethod="GetAllFailureMode"
                        ServiceParameter="string:#tbTaskSubType" />
                </td>
            </tr>
            <tr id="trFormType2" runat="server" visible="false">
                <td class="td01">
                    <asp:Literal ID="lblPayee" runat="server" Text="${WFS.Cost.Payee}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbPayeeCode" runat="server" DescField="Name" MustMatch="true"
                        ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" Width="260" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblTravelType" runat="server" Text="${WFS.Cost.TravelType}:" />
                </td>
                <td class="td02">
                    <cc1:CodeMstrDropDownList ID="ddlTravelType" Code="TravelType" runat="server" IncludeBlankOption="false" />
                </td>
            </tr>
            <tr id="isImp" runat="server" visible="false">
                <td class="td01">
                    <asp:Literal ID="lblAmount" runat="server" Text="${ISI.TSK.ImpAmount}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbAmount" runat="server" Text='<%# Bind("Amount") %>'></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAmount" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbAmount" Visible="false" ValidationGroup="vgSave" />
                    <asp:RangeValidator ID="rvAmount" runat="server" ControlToValidate="tbAmount"
                        Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                        Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblVoucher" runat="server" Text="${WFS.Cost.Voucher}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbVoucher" runat="server" Text='<%# Bind("Voucher") %>'></asp:TextBox>
                    <asp:RangeValidator ID="rvVoucher" runat="server" ControlToValidate="tbVoucher"
                        Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                        Type="Integer" MaximumValue="99999999" MinimumValue="-99999999" />
                </td>
            </tr>
            <tr id="trAmount" runat="server" visible="false">
                <td class="td01">
                    <asp:Literal ID="lblTaxes" runat="server" Text="${WFS.Cost.Taxes}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbTaxes" runat="server" Text='<%# Bind("Taxes","{0:0.########}") %>'></asp:TextBox>
                    <asp:RangeValidator ID="rvTaxes" runat="server" ControlToValidate="tbTaxes"
                        Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                        Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblTotalAmount" runat="server" Text="${WFS.Cost.TotalAmount}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbTotalAmount" runat="server" Text='<%# Bind("TotalAmount","{0:0.########}") %>' ReadOnly="true"></asp:TextBox>
                    <asp:RangeValidator ID="rvTotalAmount" runat="server" ControlToValidate="tbTotalAmount"
                        Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                        Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                </td>
            </tr>
            <tr id="trQty" runat="server" visible="false">
                <td class="td01">
                    <asp:Literal ID="lblQty" runat="server" Text="${WFS.Cost.Qty}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbQty" runat="server" Text='<%# Bind("Qty","{0:0.########}") %>'></asp:TextBox>
                    <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="tbQty"
                        Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                        Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                </td>
            </tr>
            <tr id="isIss" runat="server" visible="false">
                <td class="td01">
                    <asp:Literal ID="lblSupplier" runat="server" Text="${Common.Business.Supplier}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbSupplier" runat="server" Visible="true" DescField="Name" MustMatch="true"
                        ValueField="Code" ServicePath="SupplierMgr.service" ServiceMethod="GetAllSupplier" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblDesc1" runat="server" Text="${ISI.TSK.Desc1}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbDesc1" runat="server" Text='<%# Bind("Desc1") %>' Height="60"
                        Width="650" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,2000);"
                        onpaste="limitPaste(this, 2000)" Font-Size="10" />
                </td>
            </tr>
            <tr runat="server" id="workHoursTR2" visible="false">
                <td class="td01">
                    <asp:Literal ID="lblWorkHoursUser" runat="server" Text="${ISI.TSK.User}:" />
                </td>
                <td class="td02" colspan="5">
                    <asp:TextBox ID="tbWorkHoursUser" runat="server" Text='' Width="100%" />
                    <asp:RequiredFieldValidator ID="rfvWorkHoursUser" runat="server" ErrorMessage="${Common.String.Empty}"
                        Display="Dynamic" ControlToValidate="tbWorkHoursUser" ValidationGroup="vgSave" />
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset id="fsFlow" runat="server">
        <legend>${ISI.TSK.Flow}</legend>
        <table class="mtable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblPlanStartDate" runat="server" Text="${ISI.TSK.PlanStartDate}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbPlanStartDate" runat="server" onClick="var ctl01_ucNew_tbPlanCompleteDate=$dp.$('ctl01_ucNew_tbPlanCompleteDate');WdatePicker({startDate:'%y-%M-%d 08:00:00',qsEnabled:true,quickSel:['%y-01-01 08:00:00','%y-02-01 08:00:00','%y-%M-01 08:00:00','%y-%M-15 08:00:00','%y-{%M+1}-01 08:00:00'],dateFmt:'yyyy-MM-dd HH:mm',onpicked:function(){ctl01_ucNew_tbPlanCompleteDate.click();},maxDate:'#F{$dp.$D(\'ctl01_ucNew_tbPlanCompleteDate\')}' })"
                        Text='<%# Bind("PlanStartDate","{0:yyyy-MM-dd HH:mm}") %>' />
                <td class="td01">
                    <asp:Literal ID="lblPlanCompleteDate" runat="server" Text="${ISI.TSK.PlanCompleteDate}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbPlanCompleteDate" runat="server" onClick="WdatePicker({doubleCalendar:true,qsEnabled:true,quickSel:['%y-%M-15 16:30:00','%y-%M-%ld 16:30:00','%y-{%M+1}-%ld 16:30:00','%y-12-%ld 16:30:00','{%y+1}-01-%ld 16:30:00'],startDate:'%y-%M-{%d+7} 16:30:00',dateFmt:'yyyy-MM-dd HH:mm',minDate:'#F{$dp.$D(\'ctl01_ucNew_tbPlanStartDate\')}'})"
                        Text='<%# Bind("PlanCompleteDate","{0:yyyy-MM-dd HH:mm}") %>' />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblAssignStartUser" runat="server" Text="${ISI.TSK.AssignStartUser}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbAssignStartUser" runat="server" CssClass="inputRequired" Text='<%# Bind("AssignStartUser") %>'
                        Width="100%" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblExpectedResults" runat="server" Text="${ISI.TSK.ExpectedResults}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbExpectedResults" runat="server" Text='<%# Bind("ExpectedResults") %>'
                        Height="50" Width="77%" TextMode="MultiLine" MaxLength="5" onkeypress="javascript:setMaxLength(this,500);"
                        onpaste="limitPaste(this, 500)" Font-Size="10" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlDesc2" runat="server" Text="${ISI.TSK.Desc2}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbDesc2" runat="server" Text='<%# Bind("Desc2") %>' Height="50"
                        Width="77%" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,500);"
                        onpaste="limitPaste(this, 500)" Font-Size="10" />
                </td>
            </tr>
        </table>
    </fieldset>
</div>
<uc2:CostList ID="ucCostList" runat="server" Visible="false" />
<div>
    <table class="mtable">
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01">
                <asp:CheckBox runat="server" Text="${ISI.TSK.IsAutoRelease}" Visible="false" ID="cbIsAutoRelease" Checked="True" /></td>
            <td class="td02">
                <div class="buttons">
                    <cc1:Button ID="btnInsert" runat="server" OnClick="btnSave_Click" Text="${Common.Button.Create}"
                        CssClass="apply" ValidationGroup="vgSave" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="back" Visible="false" />
                </div>
            </td>
        </tr>
    </table>
</div>
