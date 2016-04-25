<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_ProjectTask_New" %>
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
    <fieldset>
        <legend id="lgd" >${ISI.ProjectTask.NewProjectTask}</legend>
        <table class="mtable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblProjectType" runat="server" Text="${ISI.ProjectTask.ProjectType}:" />
                </td>
                <td class="td02">
                    <cc1:CodeMstrDropDownList ID="ddlProjectType" Code="PSIType" runat="server" IncludeBlankOption="false" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblProjectSubType" runat="server" Text="${ISI.ProjectTask.ProjectSubType}:" />
                </td>
                <td class="td02">
                    <cc1:CodeMstrDropDownList ID="ddlProjectSubType" Code="ISIProjectSubType" runat="server" IncludeBlankOption="false" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblPhase" runat="server" Text="${ISI.ProjectTask.Phase}:" />
                </td>
                <td class="td02">
                    <cc1:CodeMstrDropDownList ID="ddlPhase" Code="ISIPhase" runat="server" IncludeBlankOption="false" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblSeq" runat="server" Text="${ISI.ProjectTask.Seq}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbSeq" runat="server" CssClass="inputRequired" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSeq" runat="server" ErrorMessage="${Common.String.Empty}"
                        Display="Dynamic" ControlToValidate="tbSeq" ValidationGroup="vgSave" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblSubject" runat="server" Text="${ISI.ProjectTask.Subject}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbSubject" runat="server" Text='<%# Bind("Subject") %>' Width="80%"
                        onkeypress="javascript:setMaxLength(this,50);" onpaste="limitPaste(this, 50)" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblIsActive" runat="server" Text="${Common.IsActive}:" />
                </td>
                <td class="td02">
                    <asp:CheckBox ID="ckIsActive" runat="server" Checked='true' />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlDesc" runat="server" Text="${ISI.ProjectTask.Desc}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' Height="60"
                        Width="650" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,2000);"
                        onpaste="limitPaste(this, 2000)" Font-Size="10" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblExpectedResults" runat="server" Text="${ISI.ProjectTask.ExpectedResults}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbExpectedResults" runat="server" Text='<%# Bind("ExpectedResults") %>'
                        Height="50" Width="650" TextMode="MultiLine" MaxLength="5" onkeypress="javascript:setMaxLength(this,500);"
                        onpaste="limitPaste(this, 500)" Font-Size="10" />
                </td>
            </tr>
            <tr>
                <td class="td01"></td>
                <td class="td02"></td>
                <td class="td01"></td>
                <td class="td02">
                    <div class="buttons">
                        <asp:Button ID="btnInsert" runat="server" OnClick="btnSave_Click" Text="${Common.Button.Create}"
                            CssClass="apply" ValidationGroup="vgSave" />
                        <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                            CssClass="back" />
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
</div>
