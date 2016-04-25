<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_TaskSubType_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_TaskSubType" runat="server" DataSourceID="ODS_TaskSubType" DefaultMode="Insert"
        Width="100%" DataKeyNames="Code" OnDataBinding="FV_TaskSubType_OnDataBinding" OnDataBound="FV_TaskSubType_DataBound">
        <InsertItemTemplate>
            <fieldset>
                <legend>${ISI.TaskSubType.NewTaskSubType}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCode" runat="server" Text="${ISI.TaskSubType.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvCode" runat="server" ErrorMessage="${ISI.TaskSubType.Code.Empty}"
                                Display="Dynamic" ControlToValidate="tbCode" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvInsert" runat="server" ControlToValidate="tbCode" ErrorMessage="${ISI.TaskSubType.CodeExist}"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkTaskSubType" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc" runat="server" Text="${Common.Business.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ErrorMessage="${ISI.TaskSubType.Description.Empty}"
                                Display="Dynamic" ControlToValidate="tbDesc" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblParent" runat="server" Text="${ISI.TaskSubType.Parent}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbParent" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubTypeList"
                                Width="200" />
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
                            <asp:Literal ID="ltlSeq" runat="server" Text="${ISI.TaskSubType.Sequence}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSeq" runat="server" Text='<%# Bind("Seq") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvSeq" runat="server" ErrorMessage="${ISI.TaskSubType.Sequence.Empty}"
                                Display="Dynamic" ControlToValidate="tbSeq" ValidationGroup="vgSave" />
                            <asp:RangeValidator ID="rvSeq" runat="server" ControlToValidate="tbSeq" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsActive" runat="server" Text="${Common.IsActive}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsActive" runat="server" Checked='<%# Bind("IsActive") %>' />
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
                            <asp:CheckBox ID="ckIsAutoAssign" runat="server" Checked='<%# Bind("IsAutoAssign") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlIsPublic" runat="server" Text="${ISI.TaskSubType.IsPublic}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsPublic" runat="server" Checked='<%# Bind("IsPublic") %>' />
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
                            <asp:TextBox ID="tbAssignUser" runat="server" Text='<%# Bind("AssignUser") %>' Width="600"
                                onkeypress="javascript:setMaxLength(this,300);" onpaste="limitPaste(this, 300)" />
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
                            <asp:TextBox ID="tbStartUser" runat="server" Text='<%# Bind("StartUser") %>' Width="600"
                                onkeypress="javascript:setMaxLength(this,300);" onpaste="limitPaste(this, 300)" />
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
                            <asp:TextBox ID="tbViewUser" runat="server" Text='<%# Bind("ViewUser") %>' Width="600"
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
                                MinimumValue="-100000000" MaximumValue="100000000" ValidationGroup="vgSave" />
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
                            <asp:TextBox ID="tbStartPercent" runat="server" Text='<%# Bind("StartPercent") %>' />
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
                            <asp:CheckBox ID="ckIsAssignUp" runat="server" Checked='<%#Bind("IsAssignUp")%>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblAssignUpTime" runat="server" Text="${ISI.TaskSubType.AssignUpTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbAssignUpTime" runat="server" Text='<%# Bind("AssignUpTime") %>' />
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
                                Width="600" onkeypress="javascript:setMaxLength(this,300);" onpaste="limitPaste(this, 300)" />
                            <asp:RegularExpressionValidator ID="revAssignUpUser" runat="server" Display="Dynamic"
                                ValidationExpression="^[\|]*[a-zA-Z0-9]*([a-zA-Z0-9]+[,]+[a-zA-Z0-9]+|[\|a-zA-Z0-9]*)*$"
                                ControlToValidate="tbAssignUpUser" ErrorMessage="${ISI.Error.NoVerifiedThrough}"
                                ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvAssignUpUser" runat="server" ControlToValidate="tbAssignUpUser"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkUser" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsStartUp" runat="server" Text="${ISI.TaskSubType.IsStartUp}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="ckIsStartUp" runat="server" Checked='<%#Bind("IsStartUp")%>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblStartUpTime" runat="server" Text="${ISI.TaskSubType.StartUpTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbStartUpTime" runat="server" Text='<%# Bind("StartUpTime") %>' />
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
                                Width="600" onkeypress="javascript:setMaxLength(this,300);" onpaste="limitPaste(this, 300)" />
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
                            <asp:CheckBox ID="ckIsCloseUp" runat="server" Checked='<%#Bind("IsCloseUp")%>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlCloseUpTime" runat="server" Text="${ISI.TaskSubType.CloseUpTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCloseUpTime" runat="server" Text='<%# Bind("CloseUpTime") %>' />
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
                                Width="600" onkeypress="javascript:setMaxLength(this,300);" onpaste="limitPaste(this, 300)" />
                            <asp:RegularExpressionValidator ID="revCloseUpUser" runat="server" Display="Dynamic"
                                ValidationExpression="^[\|]*[a-zA-Z0-9]*([a-zA-Z0-9]+[,]+[a-zA-Z0-9]+|[\|a-zA-Z0-9]*)*$"
                                ControlToValidate="tbCloseUpUser" ErrorMessage="${ISI.Error.NoVerifiedThrough}"
                                ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvCloseUpUser" runat="server" ControlToValidate="tbCloseUpUser"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkUser" />
                        </td>
                    </tr>

                </table>
            </fieldset>

            <div class="tablefooter">
                <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
                    CssClass="apply" ValidationGroup="vgSave" />
                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                    CssClass="back" />
            </div>
        </InsertItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_TaskSubType" runat="server" TypeName="com.Sconit.Web.TaskSubTypeMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.TaskSubType" InsertMethod="CreateTaskSubType"
    OnInserted="ODS_TaskSubType_Inserted" OnInserting="ODS_TaskSubType_Inserting">
    <InsertParameters>
        <asp:Parameter Name="AssignUpTime" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Color" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="StartUpTime" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="CloseUpTime" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="CompleteUpTime" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="OpenTime" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="StartPercent" Type="Decimal" ConvertEmptyStringToNull="true" />
    </InsertParameters>
</asp:ObjectDataSource>
