<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_UserSubscription_Main" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<script type="text/javascript" language="javascript">
    //<![CDATA[
    function GVEmailCheckClick(oCheckbox) {

        if (oCheckbox.checked == true) {
            $(".GVRow span[name='cbIsEmail'] input:checkbox").attr("checked", true);
            $(".GVAlternatingRow span[name='cbIsEmail'] input:checkbox").attr("checked", true);
        }
        else {
            $(".GVRow span[name='cbIsEmail'] input:checkbox").attr("checked", false);
            $(".GVAlternatingRow span[name='cbIsEmail'] input:checkbox").attr("checked", false);
        }
    }
    //]]>

    //<![CDATA[
    function GVMobliePhoneCheckClick(oCheckbox) {

        if (oCheckbox.checked == true) {
            $(".GVRow span[name='cbIsSMS'] input:checkbox").attr("checked", true);
            $(".GVAlternatingRow span[name='cbIsSMS'] input:checkbox").attr("checked", true);
        }
        else {
            $(".GVRow span[name='cbIsSMS'] input:checkbox").attr("checked", false);
            $(".GVAlternatingRow span[name='cbIsSMS'] input:checkbox").attr("checked", false);
        }
    }
    //]]>

</script>
<fieldset>
    <asp:GridView ID="GV_UserSubscription" runat="server" AutoGenerateColumns="False"
        DataSourceID="ODS_GV_UserSubscription" AllowSorting="false" DataKeyNames="TaskSubTypeCode"
        EmptyDataText="${ISI.UserSubscription.NotTaskSubTypeInPermission}" OnRowDataBound="GV_List_RowDataBound"
        OnDataBinding="FV_List_OnDataBinding" OnDataBound="FV_List_DataBound">
        <Columns>
            <asp:TemplateField HeaderText="${ISI.UserSubscription.TaskType}">
                <ItemTemplate>
                    <cc1:CodeMstrLabel ID="lblType" runat="server" Code="ISIType" Value='<%# Bind("TaskType") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TaskSubTypeCode" HeaderText="${ISI.UserSubscription.TaskSubTypeCode}"
                SortExpression="TaskSubTypeCode" />
            <asp:BoundField DataField="TaskSubTypeDesc" HeaderText="${ISI.UserSubscription.TaskSubTypeDesc}"
                SortExpression="TaskSubTypeDesc" />
            <asp:TemplateField HeaderText="${ISI.UserSubscription.IsEmail}">
                <HeaderTemplate>
                    <asp:CheckBox ID="CheckAllEmail" onclick="GVEmailCheckClick(this)" runat="server"
                        Text="${ISI.UserSubscription.IsEmail}" TabIndex="-1" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HiddenField ID="hfId" runat="server" Value='<%# Bind("Id") %>' />
                    <asp:CheckBox ID="cbIsEmail" name="cbIsEmail" runat="server" TabIndex="1" Checked='<%# Bind("IsEmail") %>' />
                    <asp:TextBox ID="tbEmail" runat="server" Text='<%# Bind("Email") %>' Width="80%" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="tbEmail"
                        Display="Dynamic" ErrorMessage="${ISI.Validator.Valid}" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ValidationGroup="vgSave" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.UserSubscription.MobilePhone}">
                <HeaderTemplate>
                    <asp:CheckBox ID="CheckAllMobliePhone" onclick="GVMobliePhoneCheckClick(this)" runat="server"
                        Text="${ISI.UserSubscription.IsSMS}" TabIndex="-1" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="cbIsSMS" name="cbIsSMS" runat="server" TabIndex="2" Checked='<%# Bind("IsSMS") %>' />
                    <asp:TextBox ID="tbMobilePhone" runat="server" Text='<%# Bind("MobilePhone") %>'
                        Width="80%" />
                    <asp:RegularExpressionValidator ID="revMobilePhone" runat="server" ErrorMessage="${ISI.Validator.Valid}"
                        ControlToValidate="tbMobilePhone" ValidationGroup="vgSave" ValidationExpression="^1[358]\d{9}$" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="ODS_GV_UserSubscription" runat="server" TypeName="com.Sconit.Web.UserSubscriptionMgrProxy"
        DataObjectTypeName="com.Sconit.ISI.Entity.UserSubView" SelectMethod="LoadUserSubscription"
        OnUpdating="ODS_GV_UserSubscription_OnUpdating">
        <SelectParameters>
            <asp:Parameter Name="userCode" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
    <table class="mtable">
        <tr>
            <td class="td02">
                <asp:Button ID="btnSave" runat="server" Text="${Common.Button.Save}" OnClick="btnSave_Click"
                    Visible="false" ValidationGroup="vgSave" />
            </td>
            <td class="td02">
            </td>
            <td class="td01">
            </td>
            <td class="td02">
            </td>
        </tr>
    </table>
</fieldset>
