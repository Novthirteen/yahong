<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="ISI_Checkup_Edit" %>
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
    <asp:FormView ID="FV_Checkup" runat="server" DataSourceID="ODS_Checkup" DefaultMode="Edit"
        Width="100%" DataKeyNames="Id" OnDataBound="FV_Checkup_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${ISI.Checkup.UpdateCheckup}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCheckupUser" runat="server" Text="${ISI.Checkup.CheckupUser}:" />
                        </td>
                        <td class="td02">
                            <cc1:ReadonlyTextBox ID="rtbCheckupUser" runat="server" CodeField="CheckupUser"
                                DescField="CheckupUserNm" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblJobNo" runat="server" Text="${ISI.Checkup.JobNo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbJobNo" Text='<%# Bind("JobNo") %>' runat="server" ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblDepartment" runat="server" Text="${ISI.Checkup.Department}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDepartment" Text='<%# Bind("Department") %>' runat="server" ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lbStatus" runat="server" Text="${Common.CodeMaster.Status}:" />
                        </td>
                        <td class="td02">
                            <asp:Literal ID="lblStatus" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblDept2" runat="server" Text="${ISI.Checkup.Dept2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDept2" Text='<%# Bind("Dept2") %>' runat="server" ReadOnly="true" />
                        </td>

                        <td class="td01">
                            <asp:Literal ID="ltlCheckupDate" runat="server" Text="${ISI.Checkup.CheckupDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCheckupDate" Text='<%# Bind("CheckupDate","{0:yyyy-MM-dd}") %>' runat="server" ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlAmount" runat="server" Text="${ISI.Checkup.Amount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbAmount" runat="server" Text='<%# Bind("Amount","{0:0.##}") %>' />
                            <asp:RangeValidator ID="rvAmount" runat="server" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MaximumValue="10000" MinimumValue="-10000" ControlToValidate="tbAmount" ValidationGroup="vgSave" />
                            <span style="color: mediumvioletred;">${ISI.Checkup.AmountMsg}</span>
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblCheckupProject" runat="server" Text="${ISI.Checkup.CheckupProject}:" />
                        </td>
                        <td class="td02">
                            <cc1:ReadonlyTextBox ID="rtbCheckupProject" runat="server" CodeField="CheckupProject.Code"
                                DescField="CheckupProject.Desc" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlContent" runat="server" Text="${ISI.Checkup.Content}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbContent" runat="server" Text='<%# Bind("Content") %>' Height="60"
                                Width="650" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,500);"
                                onpaste="limitPaste(this, 500)" Font-Size="10" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSummaryCode" runat="server" Text="${ISI.Checkup.SummaryCode}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbSummaryCode" Text='<%# Bind("SummaryCode") %>' runat="server" ReadOnly="true" />
                        </td>
                    </tr>

                </table>
                <div id="divMore" style="display: none">
                    <table class="mtable">
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
                            <td class="td01">
                                <asp:Literal ID="lblSubmitDate" runat="server" Text="${ISI.Checkup.SubmitDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbSubmitDate" runat="server" CodeField="SubmitDate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblSubmitUser" runat="server" Text="${ISI.Checkup.SubmitUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbSubmitUser" runat="server" CodeField="SubmitUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblCancelDate" runat="server" Text="${ISI.Checkup.CancelDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCancelDate" runat="server" CodeField="CancelDate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblCancelUser" runat="server" Text="${ISI.Checkup.CancelUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCancelUser" runat="server" CodeField="CancelUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblApprovalDate" runat="server" Text="${ISI.Checkup.ApprovalDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbApprovalDate" runat="server" CodeField="ApprovalDate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblApprovalUser" runat="server" Text="${ISI.Checkup.ApprovalUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbApprovalUser" runat="server" CodeField="ApprovalUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblCloseDate" runat="server" Text="${ISI.Checkup.CloseDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCloseDate" runat="server" CodeField="CloseDate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblCloseUser" runat="server" Text="${ISI.Checkup.CloseUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCloseUser" runat="server" CodeField="CloseUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblLastModifyDate" runat="server" Text="${ISI.Checkup.LastModifyDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbLastModifyDate" runat="server" CodeField="LastModifyDate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblLastModifyUser" runat="server" Text="${ISI.Checkup.LastModifyUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbLastModifyUser" runat="server" CodeField="LastModifyUserNm" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="mtable">
                        <tr>
                            <td class="td01">
                                <a type="text/html" onclick="More()" href="#" visible="true" id="more">More... </a>
                            </td>
                            <td class="td02"></td>
                            <td class="td01"></td>
                            <td class="td02"></td>
                        </tr>
                    </table>
                </div>
            </fieldset>
            <fieldset id="fsApprove" runat="server">
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
                                    onpaste="limitPaste(this, 500)" Font-Size="10" ReadOnly="true" />
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
                            <asp:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                CssClass="apply" ValidationGroup="vgSave" />
                            <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="${Common.Button.Submit}"
                                CssClass="apply" ValidationGroup="vgSave" />
                            <asp:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click" Text="${Common.Button.Delete}"
                                CssClass="delete" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                            <asp:Button ID="btnApprove" runat="server" Text="${ISI.Button.Approve}" OnClick="btnApprove_Click"
                                CssClass="apply" OnClientClick="return confirm('${ISI.Button.Approve.Confirm}')" />
                            <asp:Button ID="btnClose" runat="server" Text="${Common.Button.Close}" OnClick="btnClose_Click"
                                CssClass="apply" OnClientClick="return confirm('${Common.Button.Close.Confirm}')" />
                            <asp:Button ID="btnCancel" runat="server" Text="${Common.Button.Cancel}" CssClass="button2"
                                OnClick="btnCancel_Click" OnClientClick="return confirm('${Common.Button.Cancel.Confirm}')" />
                            <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                CssClass="back" />
                        </div>
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_Checkup" runat="server" TypeName="com.Sconit.Web.CheckupMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.Checkup" UpdateMethod="UpdateCheckup"
    OnUpdated="ODS_Checkup_Updated" SelectMethod="LoadCheckup" OnUpdating="ODS_Checkup_Updating" >
    <SelectParameters>
        <asp:Parameter Name="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
