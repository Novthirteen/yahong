<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_TaskStatus_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/ISI/TSK/Info.ascx" TagName="Info" TagPrefix="isi" %>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });
    });
</script>
<isi:Info ID="ucInfo" runat="server" />
<div id="divFV" runat="server">
    <asp:FormView ID="FV_TaskStatus" runat="server" DataSourceID="ODS_TaskStatus" DefaultMode="Insert"
        Width="100%" DataKeyNames="Id" OnDataBinding="FV_TaskStatus_OnDataBinding">
        <InsertItemTemplate>
            <fieldset>
                <legend>${ISI.TaskStatus.NewTaskStatus}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblDesc" runat="server" Text="${ISI.TaskStatus.Desc}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' Height="60" Width="77%"
                                TextMode="MultiLine" CssClass="inputRequired" onkeypress="javascript:setMaxLength(this,1000);"
                                onpaste="limitPaste(this, 1000)" Font-Size="10" />
                            <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbDesc" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblFlag" runat="server" Text="${Common.CodeMaster.Flag}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlFlag" Code="ISIFlag" runat="server" IncludeBlankOption="false">
                            </cc1:CodeMstrDropDownList>
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblColor" runat="server" Text="${Common.CodeMaster.Color}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlColor" Code="ISIColor" runat="server" IncludeBlankOption="false">
                            </cc1:CodeMstrDropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbStartDate" runat="server" Text='<%# Bind("StartDate") %>' CssClass="inputRequired"
                                onClick="var ctl01_ucEdit_ucStatus_ucNew_FV_TaskStatus_tbEndDate=$dp.$('ctl01_ucEdit_ucStatus_ucNew_FV_TaskStatus_tbEndDate');WdatePicker({dateFmt:'yyyy-MM-dd',onpicked:function(){ctl01_ucEdit_ucStatus_ucNew_FV_TaskStatus_tbEndDate.click();},maxDate:'#F{$dp.$D(\'ctl01_ucEdit_ucStatus_ucNew_FV_TaskStatus_tbEndDate\')}' })" />
                            <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbStartDate" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbEndDate" runat="server" Text='<%# Bind("EndDate") %>' CssClass="inputRequired"
                                onClick="WdatePicker({doubleCalendar:true,dateFmt:'yyyy-MM-dd',maxDate:'%y-%M-%d',minDate:'#F{$dp.$D(\'ctl01_ucEdit_ucStatus_ucNew_FV_TaskStatus_tbStartDate\')}'})" />
                            <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbEndDate" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvEndDate" runat="server" ControlToValidate="tbEndDate"
                                ErrorMessage="" Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="Save_ServerValidate" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsRemindCreateUser" runat="server" Checked='true' Text="${ISI.TaskStatus.IsRemindCreateUser}" />
                            <asp:CheckBox ID="cbIsRemindAssignUser" runat="server" Checked='false' Text="${ISI.TaskStatus.IsRemindAssignUser}" />
                            <asp:CheckBox ID="cbIsRemindStartUser" runat="server" Checked='true' Text="${ISI.TaskStatus.IsRemindStartUser}" />
                            <asp:CheckBox ID="cbIsRemindCommentUser" runat="server" Checked='true' Text="${ISI.TaskStatus.IsRemindCommentUser}" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsCurrentStatus" runat="server" Text="${ISI.TaskStatus.IsCurrentStatus}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsCurrentStatus" runat="server" Checked='<%#Bind("IsCurrentStatus")%>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02"></td>
                        <td class="td01"></td>
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
<asp:ObjectDataSource ID="ODS_TaskStatus" runat="server" TypeName="com.Sconit.Web.TaskStatusMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.TaskStatus" InsertMethod="CreateTaskStatus"
    OnInserted="ODS_TaskStatus_Inserted" OnInserting="ODS_TaskStatus_Inserting"></asp:ObjectDataSource>
