<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Batch.ascx.cs" Inherits="ISI_TSK_Batch" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<script language="javascript" type="text/javascript">
    //<![CDATA[

    $(document).ready(function () {
        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });
    });

    function GVCheckClick(oCheckbox) {

        if (oCheckbox.checked == true) {
            $(".GVRow span[name='CheckBoxGroup'] input:checkbox").attr("checked", true);
            $(".GVAlternatingRow span[name='CheckBoxGroup'] input:checkbox").attr("checked", true);
        }
        else {
            $(".GVRow span[name='CheckBoxGroup'] input:checkbox").attr("checked", false);
            $(".GVAlternatingRow span[name='CheckBoxGroup'] input:checkbox").attr("checked", false);
        }
    }

    //]]>

</script>
<fieldset>
    <legend id="lgd" runat="server"></legend>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlTaskSubType" runat="server" Text="${ISI.TSK.Project}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetProjectTaskSubType" AutoPostBack="true"
                    CssClass="inputRequired" OnTextChanged="tbTaskSubType_TextChanged" Width="260" />
                <asp:HiddenField runat="server" ID="hfProjectSubType" />
                <asp:RequiredFieldValidator ID="rfvTaskSubType" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                    Display="Dynamic" ControlToValidate="tbTaskSubType" ValidationGroup="vgSave" />
            </td>
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
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblUserName" runat="server" Text="${ISI.TSK.UserName}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbUserName" runat="server" Text='<%# Bind("UserName") %>' onkeypress="javascript:setMaxLength(this,50);"
                    onpaste="limitPaste(this, 50)" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblMobilePhone" runat="server" Text="${ISI.TSK.MobilePhone}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbMobilePhone" runat="server" Text='<%# Bind("MobilePhone") %>'
                    onkeypress="javascript:setMaxLength(this,50);" onpaste="limitPaste(this, 50)" />
                <asp:RegularExpressionValidator ID="revMobilePhone" runat="server" ErrorMessage="${ISI.Error.MobilePhoneIsInvalid}"
                    ControlToValidate="tbMobilePhone" ValidationGroup="vgSave" ValidationExpression="^1[358]\d{9}$" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblEmail" runat="server" Text="${ISI.TSK.Email}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEmail" runat="server" Text='<%# Bind("Email") %>' onkeypress="javascript:setMaxLength(this,50);"
                    onpaste="limitPaste(this, 50)" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ErrorMessage="${ISI.Error.MailIsInvalid}"
                    ControlToValidate="tbEmail" ValidationGroup="vgSave" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEncSubject" runat="server" Text="${ISI.TSK.EncSubject}:" Visible="false" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEncSubject" runat="server" Text='<%# Bind("EncSubject") %>'
                    onkeypress="javascript:setMaxLength(this,500);" onpaste="limitPaste(this, 500)" Visible="false" />
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01">
                <asp:CheckBox runat="server" Text="${ISI.TSK.IsAutoRelease}" ID="IsAutoRelease" Checked="True" /></td>
            <td class="td02">
                <div class="buttons">
                    <asp:Button ID="btnInsert" runat="server" OnClick="btnSave_Click" Text="${Common.Button.Create}"
                        CssClass="apply" ValidationGroup="vgSave" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="back" Visible="false" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fs" runat="server" visible="false">
    <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField HeaderStyle-Width="4%" Visible="False">
                <HeaderTemplate>
                    <asp:CheckBox ID="CheckAll" runat="server" onclick="GVCheckClick(this)" Checked="True" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBoxGroup" name="CheckBoxGroup" runat="server" Checked="True" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.Phase}/${ISI.TSK.Seq}" HeaderStyle-Width="5%" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:HiddenField ID="hfId" runat="server" Value='<%# Bind("Id") %>' />
                    <cc1:CodeMstrDropDownList ID="ddlPhase" runat="server" Code="ISIPhase" IncludeBlankOption="false" />
                    <br>
                    <asp:TextBox ID="tbSeq" runat="server" Text='<%# Bind("Seq") %>' Width="40" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.Subject}/${ISI.TSK.BackYards}" HeaderStyle-Width="10%" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:TextBox ID="tbSubject" runat="server" Text='<%# Bind("Subject") %>' Height="30" TextMode="MultiLine" />
                    <br>
                    <asp:TextBox ID="tbBackYards" runat="server" Text='' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.PlanDate}" HeaderStyle-Wrap="false" HeaderStyle-Width="10%">
                <ItemTemplate>
                    <div>
                        <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                    </div>
                    <div>
                        <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.Desc1}" HeaderStyle-Wrap="false" HeaderStyle-Width="40%">
                <ItemTemplate>
                    <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' Height="60"
                        TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,2000);"
                        onpaste="limitPaste(this, 2000)" Font-Size="10" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.ExpectedResults}" HeaderStyle-Wrap="false" HeaderStyle-Width="35%">
                <ItemTemplate>
                    <asp:TextBox ID="tbExpectedResults" runat="server" Text='<%# Bind("ExpectedResults") %>' Height="60"
                        TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,500);"
                        onpaste="limitPaste(this, 500)" Font-Size="10" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>
