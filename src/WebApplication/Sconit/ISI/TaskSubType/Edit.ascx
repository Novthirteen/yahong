<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="ISI_TaskSubType_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });
    });
</script>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_TaskSubType" runat="server" DataSourceID="ODS_TaskSubType" DefaultMode="Edit"
        Width="100%" DataKeyNames="Code" OnDataBound="FV_TaskSubType_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${ISI.TaskSubType.Basic.Info}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCode" runat="server" Text="${ISI.TaskSubType.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc" runat="server" Text="${Common.Business.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ErrorMessage="${ISI.TaskAddress.Description.Empty}"
                                Display="Dynamic" ControlToValidate="tbDesc" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblParent" runat="server" Text="${ISI.TaskSubType.Parent}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbParent" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubTypeNotCode"
                                Width="200" ServiceParameter="string:#tbCode" />
                        </td>
                        <td align="right">
                            <asp:Literal ID="ltlOrg" runat="server" Text="${ISI.TaskSubType.Org}:" />
                        </td>
                        <td>
                            <cc1:CodeMstrDropDownList ID="ddlOrg" Code="ISIDepartment" runat="server" IncludeBlankOption="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSeq" runat="server" Text="${ISI.TaskAddress.Sequence}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSeq" runat="server" Text='<%# Bind("Seq") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvSeq" runat="server" ErrorMessage="${ISI.TaskAddress.Sequence.Empty}"
                                Display="Dynamic" ControlToValidate="tbSeq" ValidationGroup="vgSave" />
                            <asp:RangeValidator ID="rvSeq" runat="server" ControlToValidate="tbSeq" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsActive" runat="server" Text="${Common.IsActive}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsCost" runat="server" Text="${ISI.TaskSubType.IsCost}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsCost" runat="server" Checked='<%# Bind("IsCost") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlIsAutoAssign" runat="server" Text="${ISI.TaskSubType.IsAutoAssign}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsAutoAssign" runat="server" Checked='<%# Bind("IsAutoAssign") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlIsPublic" runat="server" Text="${ISI.TaskSubType.IsPublic}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsPublic" runat="server" Checked='<%# Bind("IsPublic") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlIsAutoStart" runat="server" Text="${ISI.TaskSubType.IsAutoStart}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsAutoStart" runat="server" Checked='<%# Bind("IsAutoStart") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlIsAutoStatus" runat="server" Text="${ISI.TaskSubType.IsAutoStatus}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsAutoStatus" runat="server" Checked='<%# Bind("IsAutoStatus") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlIsAutoComplete" runat="server" Text="${ISI.TaskSubType.IsAutoComplete}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsAutoComplete" runat="server" Checked='<%# Bind("IsAutoComplete") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlIsAutoClose" runat="server" Text="${ISI.TaskSubType.IsAutoClose}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsAutoClose" runat="server" Checked='<%# Bind("IsAutoClose") %>' />
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlAssignUser" runat="server" Text="${ISI.TaskSubType.AssignUser}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:Literal ID="ltlAssignUserFormat" runat="server" Text="${ISI.User.Format}" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbAssignUser" runat="server" Text='<%# Bind("AssignUser") %>' Width="77%"
                                onkeypress="javascript:setMaxLength(this,255);" onpaste="limitPaste(this, 255)" />
                            <asp:RegularExpressionValidator ID="revAssignUser" runat="server" Display="Dynamic"
                                ValidationExpression="^([a-zA-Z0-9]+[,]+[a-zA-Z0-9]+|[a-zA-Z0-9]*)*$" ControlToValidate="tbAssignUser"
                                ErrorMessage="${ISI.Error.NoVerifiedThrough}" ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvAssignUser" runat="server" ControlToValidate="tbAssignUser"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkUser" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlStartUser" runat="server" Text="${ISI.TaskSubType.StartUser}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:Literal ID="ltlStartUserFormat" runat="server" Text="${ISI.User.Format}" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbStartUser" runat="server" Text='<%# Bind("StartUser") %>' Width="77%"
                                onkeypress="javascript:setMaxLength(this,255);" onpaste="limitPaste(this, 255)" />
                            <asp:RegularExpressionValidator ID="revStartUser" runat="server" Display="Dynamic"
                                ValidationExpression="^([a-zA-Z0-9]+[,]+[a-zA-Z0-9]+|[a-zA-Z0-9]*)*$" ControlToValidate="tbStartUser"
                                ErrorMessage="${ISI.Error.NoVerifiedThrough}" ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvStartUser" runat="server" ControlToValidate="tbStartUser"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkUser" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlViewUser" runat="server" Text="${ISI.TaskSubType.ViewUser}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:Literal ID="ltlViewUserFormat" runat="server" Text="${ISI.User.Format}" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbViewUser" runat="server" Text='<%# Bind("ViewUser") %>' Width="77%"
                                onkeypress="javascript:setMaxLength(this,300);" onpaste="limitPaste(this, 300)" />
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic"
                                ValidationExpression="^([a-zA-Z0-9]+[,]+[a-zA-Z0-9]+|[a-zA-Z0-9]*)*$" ControlToValidate="tbViewUser"
                                ErrorMessage="${ISI.Error.NoVerifiedThrough}" ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvViewUser" runat="server" ControlToValidate="tbViewUser"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkUser" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <fieldset>
                <legend>${ISI.TaskSubType.AdvancedPermissions}</legend>
                <table class="mtable" runat="server" id="userTable">
                    <tr runat="server" id="trAdmin">
                        <td class="td01">
                            <asp:Literal ID="lblAdmin" runat="server" Text="${ISI.TaskSubType.Admin}:" /></td>
                        <td>
                            <asp:TextBox ID="tbAdmin" TextMode="MultiLine" runat="server" Height="50" Width="77%" Font-Size="10" ReadOnly="true" />
                        </td>
                    </tr>
                    <tr runat="server" id="trFlowAdmin">
                        <td class="td01">
                            <asp:Literal ID="lblFlowAdmin" runat="server" Text="${ISI.TaskSubType.FlowAdmin}:" /></td>
                        <td>
                            <asp:TextBox ID="tbFlowAdmin" TextMode="MultiLine" runat="server" Height="50" Width="77%" Font-Size="10" ReadOnly="true" />
                        </td>
                    </tr>
                    <tr runat="server" id="trPublicView">
                        <td class="td01">
                            <asp:Literal ID="ltlPublicView" runat="server" Text="${ISI.TaskSubType.PublicView}:" /></td>
                        <td>
                            <asp:TextBox ID="tbPublicView" TextMode="MultiLine" runat="server" Height="50" Width="77%" Font-Size="10" ReadOnly="true" />
                        </td>
                    </tr>
                    <tr runat="server" id="trPublicType">
                        <td class="td01">
                            <asp:Literal ID="ltlPublicType" runat="server" Text="${ISI.TaskSubType.PublicType}:" /></td>
                        <td>
                            <asp:TextBox ID="tbPublicType" TextMode="MultiLine" runat="server" Height="50" Width="77%" Font-Size="10" ReadOnly="true" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <fieldset runat="server" id="fs">
                <legend>${ISI.TaskSubType.Control}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc2" runat="server" Text="${ISI.TaskSubType.Desc2}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbDesc2" runat="server" Text='<%# Bind("Desc2") %>' Height="80"
                                Width="77%" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,1000);"
                                onpaste="limitPaste(this, 1000)" Font-Size="10" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblProjectType" runat="server" Text="${ISI.TaskSubType.ProjectType}:" />
                        </td>

                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlProjectType" Code="ISIProjectType" runat="server" IncludeBlankOption="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblColor" runat="server" Text="${ISI.TaskSubType.Color}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlColor" Code="ISIColor" runat="server" IncludeBlankOption="false">
                            </cc1:CodeMstrDropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsQuote" runat="server" Text="${ISI.TaskSubType.IsQuote}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsQuote" runat="server" Checked='<%# Bind("IsQuote") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsInitiation" runat="server" Text="${ISI.TaskSubType.IsInitiation}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsInitiation" runat="server" Checked='<%# Bind("IsInitiation") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblECType" runat="server" Text="${ISI.TaskSubType.ECType}:" />
                        </td>

                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlECType" Code="ISIECType" runat="server" IncludeBlankOption="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsEC" runat="server" Text="${ISI.TaskSubType.IsEC}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsEC" runat="server" Checked='<%# Bind("IsEC") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblECUser" runat="server" Text="${ISI.TaskSubType.ECUser}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:Literal ID="lblECUserFormat" runat="server" Text="${ISI.User.Format}" />
                        </td>

                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbECUser" runat="server" Text='<%# Bind("ECUser") %>' Width="600"
                                onkeypress="javascript:setMaxLength(this,300);" onpaste="limitPaste(this, 300)" />
                            <asp:RegularExpressionValidator ID="revECUser" runat="server" Display="Dynamic"
                                ValidationExpression="^([a-zA-Z0-9]+[,]+[a-zA-Z0-9]+|[a-zA-Z0-9]*)*$" ControlToValidate="tbECUser"
                                ErrorMessage="${ISI.Error.NoVerifiedThrough}" ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvECUser" runat="server" ControlToValidate="tbECUser"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkUser" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <fieldset>
                <legend>${ISI.TaskSubType.Monitor}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsReport" runat="server" Text="${ISI.TaskSubType.IsReport}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsReport" runat="server" Checked='<%#Bind("IsReport")%>' />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsOpen" runat="server" Text="${ISI.TaskSubType.IsOpen}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsOpen" runat="server" Checked='<%#Bind("IsOpen")%>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblOpenTime" runat="server" Text="${ISI.TaskSubType.OpenTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbOpenTime" runat="server" Text='<%# Bind("OpenTime","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvOpenTime" runat="server" ControlToValidate="tbOpenTime"
                                ErrorMessage="${Common.Validator.Valid.Number}" Display="Dynamic" Type="Double"
                                MinimumValue="0" MaximumValue="100000000" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsStart" runat="server" Text="${ISI.TaskSubType.IsStart}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsStart" runat="server" Checked='<%#Bind("IsStart")%>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblStartPercent" runat="server" Text="${ISI.TaskSubType.StartPercent}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbStartPercent" runat="server" Text='<%# Bind("StartPercent","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvStartPercent" runat="server" ControlToValidate="tbStartPercent"
                                ErrorMessage="${Common.Validator.Valid.Number}" Display="Dynamic" Type="Double"
                                MinimumValue="0.1" MaximumValue="1" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsCompleteUp" runat="server" Text="${ISI.TaskSubType.IsCompleteUp}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsCompleteUp" runat="server" Checked='<%#Bind("IsCompleteUp")%>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblCompleteUpTime" runat="server" Text="${ISI.TaskSubType.CompleteUpTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCompleteUpTime" runat="server" Text='<%# Bind("CompleteUpTime","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvCompleteUpTime" runat="server" ControlToValidate="tbCompleteUpTime"
                                ErrorMessage="${Common.Validator.Valid.Number}" Display="Dynamic" Type="Double"
                                MinimumValue="0" MaximumValue="100000000" ValidationGroup="vgSave" />
                        </td>
                    </tr>

                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlIsAssignUp" runat="server" Text="${ISI.TaskSubType.IsAssignUp}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsAssignUp" runat="server" Checked='<%#Bind("IsAssignUp")%>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblAssignUpTime" runat="server" Text="${ISI.TaskSubType.AssignUpTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbAssignUpTime" runat="server" Text='<%# Bind("AssignUpTime","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvAssignUpTime" runat="server" ControlToValidate="tbAssignUpTime"
                                ErrorMessage="${Common.Validator.Valid.Number}" Display="Dynamic" Type="Double"
                                MinimumValue="0" MaximumValue="100000000" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlAssignUpUser" runat="server" Text="${ISI.TaskSubType.AssignUpUser}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:Literal ID="ltlAssignUpUserFormat" runat="server" Text="${ISI.UpUser.Format}" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbAssignUpUser" runat="server" Text='<%# Bind("AssignUpUser") %>'
                                Width="77%" onkeypress="javascript:setMaxLength(this,255);" onpaste="limitPaste(this, 255)" />
                            <asp:RegularExpressionValidator ID="revAssignUpUser" runat="server" Display="Dynamic"
                                ValidationExpression="^[\|]*[a-zA-Z0-9]*([a-zA-Z0-9]+[,]+[a-zA-Z0-9]+|[\|a-zA-Z0-9]*)*$"
                                ControlToValidate="tbAssignUpUser" ErrorMessage="${ISI.Error.NoVerifiedThrough}"
                                ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvAssignUpUser" runat="server" ControlToValidate="tbAssignUpUser"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkUser" />
                        </td>
                    </tr>
                    <tr runat="server" id="trPublicAssign">
                        <td class="td01">
                            <asp:Literal ID="ltlPublicAssign" runat="server" Text="${ISI.TaskSubType.PublicAssign}:" /></td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbPublicAssign" runat="server" Width="77%" TextMode="MultiLine" ReadOnly="true" Height="50" Font-Size="10" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsStartUp" runat="server" Text="${ISI.TaskSubType.IsStartUp}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsStartUp" runat="server" Checked='<%#Bind("IsStartUp")%>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblStartUpTime" runat="server" Text="${ISI.TaskSubType.StartUpTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbStartUpTime" runat="server" Text='<%# Bind("StartUpTime","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvStartUpTime" runat="server" ControlToValidate="tbStartUpTime"
                                ErrorMessage="${Common.Validator.Valid.Number}" Display="Dynamic" Type="Double"
                                MinimumValue="0" MaximumValue="100000000" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlStartUpUser" runat="server" Text="${ISI.TaskSubType.StartUpUser}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:Literal ID="ltlStartUpUserFormat" runat="server" Text="${ISI.UpUser.Format}" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbStartUpUser" runat="server" Text='<%# Bind("StartUpUser") %>'
                                Width="77%" onkeypress="javascript:setMaxLength(this,255);" onpaste="limitPaste(this, 255)" />
                            <asp:RegularExpressionValidator ID="revStartUpUser" runat="server" Display="Dynamic"
                                ValidationExpression="^[\|]*[a-zA-Z0-9]*([a-zA-Z0-9]+[,]+[a-zA-Z0-9]+|[\|a-zA-Z0-9]*)*$"
                                ControlToValidate="tbStartUpUser" ErrorMessage="${ISI.Error.NoVerifiedThrough}"
                                ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvStartUpUser" runat="server" ControlToValidate="tbStartUpUser"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkUser" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsCloseUp" runat="server" Text="${ISI.TaskSubType.IsCloseUp}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsCloseUp" runat="server" Checked='<%#Bind("IsCloseUp")%>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlCloseUpTime" runat="server" Text="${ISI.TaskSubType.CloseUpTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCloseUpTime" runat="server" Text='<%# Bind("CloseUpTime","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvCloseUpTime" runat="server" ControlToValidate="tbCloseUpTime"
                                ErrorMessage="${Common.Validator.Valid.Number}" Display="Dynamic" Type="Double"
                                MinimumValue="0" MaximumValue="100000000" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCloseUpUser" runat="server" Text="${ISI.TaskSubType.CloseUpUser}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:Literal ID="ltlCloseUpUserFormat" runat="server" Text="${ISI.UpUser.Format}" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbCloseUpUser" runat="server" Text='<%# Bind("CloseUpUser") %>'
                                Width="77%" onkeypress="javascript:setMaxLength(this,255);" onpaste="limitPaste(this, 255)" />
                            <asp:RegularExpressionValidator ID="revCloseUpUser" runat="server" Display="Dynamic"
                                ValidationExpression="^[\|]*[a-zA-Z0-9]*([a-zA-Z0-9]+[,]+[a-zA-Z0-9]+|[\|a-zA-Z0-9]*)*$"
                                ControlToValidate="tbCloseUpUser" ErrorMessage="${ISI.Error.NoVerifiedThrough}"
                                ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvCloseUpUser" runat="server" ControlToValidate="tbCloseUpUser"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkUser" />
                        </td>
                    </tr>
                    <tr runat="server" id="trPublicClose">
                        <td class="td01">
                            <asp:Literal ID="ltlPublicClose" runat="server" Text="${ISI.TaskSubType.PublicClose}:" /></td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbPublicClose" runat="server" Height="50" Width="77%" TextMode="MultiLine" Font-Size="10" ReadOnly="true" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <div class="tablefooter">
                <asp:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                    CssClass="apply" ValidationGroup="vgSave" />
                <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
                    CssClass="delete" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                    CssClass="back" />
            </div>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_TaskSubType" runat="server" TypeName="com.Sconit.Web.TaskSubTypeMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.TaskSubType" UpdateMethod="UpdateTaskSubType"
    OnUpdated="ODS_TaskSubType_Updated" SelectMethod="LoadTaskSubType" OnUpdating="ODS_TaskSubType_Updating"
    DeleteMethod="DeleteTaskSubType" OnDeleting="ODS_TaskSubType_Deleting" OnDeleted="ODS_TaskSubType_Deleted">
    <SelectParameters>
        <asp:Parameter Name="code" Type="String" />
    </SelectParameters>
    <DeleteParameters>
        <asp:Parameter Name="code" Type="String" />
    </DeleteParameters>
    <UpdateParameters>
        <asp:Parameter Name="Color" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="AssignUpTime" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="StartUpTime" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="CloseUpTime" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="CompleteUpTime" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="OpenTime" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="StartPercent" Type="Decimal" ConvertEmptyStringToNull="true" />
    </UpdateParameters>
</asp:ObjectDataSource>
