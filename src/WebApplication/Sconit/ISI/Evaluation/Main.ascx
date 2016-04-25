<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_Evaluation_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<script language="javascript" type="text/javascript">
    //<![CDATA[

    function GenerateUserName(obj) {
        var objId = $(obj).attr("id");

        var parentId = objId.substring(0, objId.length - "tbUserCode_suggest".length);
        if ($(obj).val() != "") {
            Sys.Net.WebServiceProxy.invoke('Webservice/CheckupMgrWS.asmx', 'GetUserName', false,
                {
                    "code": $(obj).val(),
                    "userCode": "<%=CurrentUser.Code%>"
                },
            function OnSucceeded(result, eventArgs) {

                if (result != '' && result != null) {
                    $('#' + parentId + 'lblUserName').html(result);
                }
            },
            function OnFailed(error) {
                //alert(error.get_message());
            }
           );
            }
        }

        //]]>

</script>
<fieldset>
    <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField HeaderText="${ISI.Evaluation.User}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:HiddenField ID="hfUserCode" runat="server" Value='<%# Bind("UserCode") %>' />
                    <uc3:textbox ID="tbUserCode" runat="server" Visible="true" DescField="LongName" MustMatch="true"
                        ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvUserCode" runat="server" ControlToValidate="tbUserCode"
                        Display="Dynamic" ErrorMessage="${Common.String.Empty}" ValidationGroup="vgAdd" Enabled="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.Business.Name}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblUserName" runat="server" Text='<%# Bind("UserName") %>' ReadOnly="true" AutoPostBack="true" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.Evaluation.StandardQty}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <div>
                        <asp:TextBox ID="tbStandardQty" runat="server" TabIndex="1" Width="80" CssClass="inputRequired" Text='<%# Bind("StandardQty") %>' />
                        <asp:RegularExpressionValidator ID="revStandardQty" ControlToValidate="tbStandardQty" runat="server"
                            ValidationGroup="vgAdd" ErrorMessage="${Common.Validator.Valid.Number}" ValidationExpression="^[0-9]+(.[0-9]{1,8})?$"
                            Display="Dynamic" />
                        <asp:RangeValidator ID="rvStandardQty" ControlToValidate="tbStandardQty" runat="server" Display="Dynamic"
                            ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="1000" MinimumValue="0"
                            Type="Integer" ValidationGroup="vgAdd" />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.Evaluation.IsCheckup}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:CheckBox ID="cbIsCheckup" runat="server" Checked='<%# Bind("IsCheckup") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.Evaluation.Amount}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <div>
                        <asp:TextBox ID="tbAmount" runat="server" TabIndex="1" Width="80" CssClass="inputRequired" Text='<%# Bind("Amount","{0:0.########}") %>' />
                        <asp:RegularExpressionValidator ID="revAmount" ControlToValidate="tbAmount" runat="server"
                            ValidationGroup="vgAdd" ErrorMessage="${Common.Validator.Valid.Number}" ValidationExpression="^[0-9]+(.[0-9]{1,8})?$"
                            Display="Dynamic" />
                        <asp:RangeValidator ID="rvAmount" ControlToValidate="tbAmount" runat="server" Display="Dynamic"
                            ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="999999999" MinimumValue="0.00000001"
                            Type="Double" ValidationGroup="vgAdd" />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.Evaluation.IsActive}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:CheckBox ID="cbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreateUserNm" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateUser}" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:BoundField DataField="LastModifyUserNm" SortExpression="LastModifyUser" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.LastModifyUser}" />
            <asp:BoundField DataField="LastModifyDate" SortExpression="LastModifyDate" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.LastModifyDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:TemplateField HeaderText="${Common.GridView.Action}" Visible="true" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnAdd" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserCode") %>'
                        Text="${Common.Button.New}" OnClick="lbtnAdd_Click" Visible="false"
                        ValidationGroup="vgAdd" TabIndex="1">
                    </asp:LinkButton>
                    <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserCode") %>'
                        Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')">
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <div>
        <asp:Button ID="btnSave" runat="server" Text="${Common.Button.Save}"
            CssClass="apply" OnClick="btnUpdate_Click" ValidationGroup="vgAdd" />
    </div>
</fieldset>


