<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="ISI_TaskStatus_Edit" %>
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
    <asp:FormView ID="FV_TaskStatus" runat="server" DataSourceID="ODS_TaskStatus" DefaultMode="Edit"
        Width="100%" DataKeyNames="Id" OnDataBound="FV_TaskStatus_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend id="lgd" runat="server">${ISI.TaskStatus.UpdateTaskStatus}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc" runat="server" Text="${ISI.TaskStatus.Desc}:" />
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
                                onClick="var ctl01_ucEdit_ucStatus_ucEdit_FV_TaskStatus_tbEndDate=$dp.$('ctl01_ucEdit_ucStatus_ucEdit_FV_TaskStatus_tbEndDate');WdatePicker({dateFmt:'yyyy-MM-dd',onpicked:function(){ctl01_ucEdit_ucStatus_ucEdit_FV_TaskStatus_tbEndDate.click();},maxDate:'#F{$dp.$D(\'ctl01_ucEdit_ucStatus_ucEdit_FV_TaskStatus_tbEndDate\')}' })" />
                            <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbStartDate" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndTime}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbEndDate" runat="server" Text='<%# Bind("EndDate") %>' CssClass="inputRequired"
                                onClick="WdatePicker({doubleCalendar:true,dateFmt:'yyyy-MM-dd',maxDate:'%y-%M-%d',minDate:'#F{$dp.$D(\'ctl01_ucEdit_ucStatus_ucEdit_FV_TaskStatus_tbStartDate\')}'})" />
                            <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbEndDate" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvEndDate" runat="server" ControlToValidate="tbEndDate"
                                ErrorMessage="" Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="Save_ServerValidate" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCreateDate" runat="server" Text="${Common.Business.CreateDate}:" />
                        </td>
                        <td class="td02">
                            <cc1:ReadonlyTextBox ID="tbCreateDate" runat="server" CodeField="CreateDate" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblCreateUser" runat="server" Text="${Common.Business.CreateUser}:" />
                        </td>
                        <td class="td02">
                            <cc1:ReadonlyTextBox ID="tbCreateUser" runat="server" CodeField="CreateUserNm" />
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
                                <asp:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" />
                                <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
                                    CssClass="delete" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_TaskStatus" runat="server" TypeName="com.Sconit.Web.TaskStatusMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.TaskStatus" UpdateMethod="UpdateTaskStatus"
    OnUpdated="ODS_TaskStatus_Updated" SelectMethod="LoadTaskStatus" OnUpdating="ODS_TaskStatus_Updating"
    DeleteMethod="DeleteTaskStatus" OnDeleting="ODS_TaskStatus_Deleting" OnDeleted="ODS_TaskStatus_Deleted">
    <SelectParameters>
        <asp:Parameter Name="id" Type="Int32" />
    </SelectParameters>
    <DeleteParameters>
        <asp:Parameter Name="id" Type="Int32" />
    </DeleteParameters>
</asp:ObjectDataSource>
