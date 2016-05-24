<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_Summary_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });
    });

</script>
<div>
    <fieldset>
        <legend>${ISI.Summary.NewSummary}</legend>
        <table class="mtable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblUserCode" runat="server" Text="${ISI.Summary.User}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbUserCode" runat="server" ReadOnly="true" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblDate" runat="server" Text="${ISI.Summary.Date}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM'})" CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbDate" ValidationGroup="vgSave" />
                    <asp:CustomValidator ID="cvInsert" runat="server" ControlToValidate="tbDate" ErrorMessage="${ISI.Summary.DateExist}"
                        Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkSummaryDate" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="ltlStandardQty" runat="server" Text="${ISI.Summary.StandardQty}:" />
                </td>
                <td class="td02">
                    <asp:TextBox ID="tbStandardQty" runat="server" Text='<%# Bind("StandardQty") %>' ReadOnly="true" />
                    <asp:CheckBox runat="server" Checked="true" ID="cbIsCheckup" Enabled="false" Text="${ISI.Summary.IsCheckup}" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblRefCode" runat="server" Text="${Common.Business.RefCode}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbRefCode" runat="server" Visible="true" DescField="Count" MustMatch="true"
                        ValueField="Code" ServicePath="SummaryMgr.service" ServiceMethod="GetSummary" OnTextChanged="tbRefCode_TextChanged" AutoPostBack="true" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Label ID="lblDesc" runat="server" Text="${ISI.Summary.Desc}:" />
                </td>
                <td class="td02" colspan="3">
                    <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' Height="50"
                        Width="80%" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,3000);"
                        onpaste="limitPaste(this, 3000)" Font-Size="10" />
                </td>
            </tr>
            <tr>
                <td class="td01"></td>
                <td class="td02"></td>
                <td class="td01">
                    <asp:CheckBox runat="server" Text="${ISI.Summary.IsAutoRelease}" ID="cbIsAutoRelease" Checked="true" /></td>
                <td class="td02">
                    <div class="buttons">
                        <asp:Button ID="btnInsert" runat="server" Text="${Common.Button.Save}"
                            CssClass="apply" ValidationGroup="vgSave" OnClick="btnSave_Click" />
                        <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                            CssClass="back" />
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
</div>
<div>
    <fieldset>
        <legend runat="server" id="lgd">${ISI.Summary.Detail}</legend>
        <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound"
            AutoGenerateColumns="false" ShowHeader="false">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <table class="mtable">
                            <tr>
                                <td style="text-align: left; width=2%;">
                                    <asp:CheckBox runat="server" ID="cbChecked" Checked="true" Text="<%#Container.DataItemIndex + 1%>" />
                                </td>
                                <td style="text-align: right; ">
                                    <asp:Label ID="lblSubject" runat="server" Text="${ISI.Summary.Detail.Subject}:" />
                                </td>
                                <td class="td02">
                                    <asp:TextBox ID="tbSubject" runat="server" TabIndex="1" Width="80%" Text='<%# Bind("Subject") %>' />${Common.WordCount.100}
                                </td>
                                <td class="td01">
                                    <asp:Label ID="lblTaskCode" runat="server" Text="${ISI.Summary.Detail.TaskCode}:" />
                                </td>
                                <td class="td02">
                                    <asp:TextBox ID="tbTaskCode" runat="server" TabIndex="1" Text='<%# Bind("TaskCode") %>' />
                                </td>
                                <td style="text-align: right; width=2%; vertical-align: bottom;"></td>
                            </tr>
                            <tr>
                                <td class="td01" colspan="2">
                                    <asp:Label ID="lblConment" runat="server" Text="${ISI.Summary.Detail.Conment}:" />
                                </td>
                                <td class="td02" colspan="3">
                                    <asp:TextBox ID="tbConment" runat="server" Text='<%# Bind("Conment") %>' Height="150"
                                        Width="95%" TextMode="MultiLine" Font-Size="10" CssClass="inputRequired" />
                                </td>
                                <td style="text-align: right; width=1%; vertical-align: bottom;">
                                    <div>
                                        <asp:Image ID="descImg" onclick="javascript:scroll(0,0)" ImageUrl="~/Images/ISI/top16.png" runat="server" ToolTip="${Common.Return.Top}" />
                                    </div>
                                    <div>
                                        <asp:Button ID="btnSave" runat="server" Text="${ISI.Summary.Save}"
                                            CssClass="apply" ValidationGroup="vgSave" OnClick="btnSave_Click" Visible="false" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </fieldset>
</div>
