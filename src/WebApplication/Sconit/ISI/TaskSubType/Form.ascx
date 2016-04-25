<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Form.ascx.cs" Inherits="ISI_TaskSubType_Form" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<script language="javascript" type="text/javascript">
    //<![CDATA[

    function GenerateApply(obj) {
        var objId = $(obj).attr("id");

        var parentId = objId.substring(0, objId.length - "tbApply_suggest".length);
        if ($(obj).val() != "") {
            Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'GetApply', false,
                {
                    "code": $(obj).val(),
                    "userCode": "<%=CurrentUser.Code%>"
                },
            function OnSucceeded(result, eventArgs) {
                // alert('#' + parentId + 'tbDesc1');                
                if (result != '' && result != null) {
                    $('#' + parentId + 'tbDesc1').val(result.Desc1);
                    $('#' + parentId + 'tbDesc2').val(result.Desc2);
                }
            },
            function OnFailed(error) {
                //alert(error.get_message());
            }
           );
            }
        }


        function GenerateUOM(obj) {
            var objId = $(obj).attr("id");
            var parentId = objId.substring(0, objId.length - "tbUOM_suggest".length);
            if ($(obj).val() != "") {
                Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'GetUOM', false,
                    {
                        "code": $(obj).val(),
                        "userCode": "<%=CurrentUser.Code%>"
                    },
            function OnSucceeded(result, eventArgs) {
                //alert('#' + parentId + 'lblPartyFromDesc');
                $('#' + parentId + 'tbUOMDesc1').val(result.Name);
                $('#' + parentId + 'tbUOMDesc2').val(result.Code);
            },
            function OnFailed(error) {
                //alert(error.get_message());
            }
           );
                }
            }

            //]]>

</script>
<div>
    <asp:FormView ID="FV_TaskSubType" runat="server" DataSourceID="ODS_TaskSubType" DefaultMode="Edit"
        Width="100%" DataKeyNames="Code" OnDataBound="FV_TaskSubType_DataBound">
        <EditItemTemplate>
            <%# TaskSubTypeCode %><%# TaskSubTypeDesc %>
            <fieldset>
                <legend>${ISI.TaskSubType.FormOpt}</legend>
                <table class="mtable">

                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsApply" runat="server" Text="${ISI.TaskSubType.IsApply}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsApply" runat="server" Checked='<%# Bind("IsApply") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsRemoveForm" runat="server" Text="${ISI.TaskSubType.IsRemoveForm}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsRemoveForm" runat="server" Checked='<%# Bind("IsRemoveForm") %>' />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <div class="tablefooter">
                <asp:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                    CssClass="apply" ValidationGroup="vgSave" />
                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                    CssClass="back" />
            </div>
        </EditItemTemplate>
    </asp:FormView>
</div>

<asp:ObjectDataSource ID="ODS_TaskSubType" runat="server" TypeName="com.Sconit.Web.TaskSubTypeMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.TaskSubType" UpdateMethod="UpdateTaskSubType"
    OnUpdated="ODS_TaskSubType_Updated" SelectMethod="LoadTaskSubType" OnUpdating="ODS_TaskSubType_Updating">
    <SelectParameters>
        <asp:Parameter Name="code" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>

<fieldset>
    <legend>${ISI.TaskSubType.Apply}</legend>
    <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false" ShowHeader="false">
        <Columns>
            <asp:TemplateField HeaderText="${ISI.TaskSubType.Apply.Code}/${ISI.TaskSubType.Apply.UOM}">
                <ItemTemplate>
                    <table class="mtable">
                        <tr>
                            <td style="text-align: right">
                                <asp:Label ID="lblType" runat="server" Text="${ISI.TaskSubType.Apply.Type}:" />
                            </td>
                            <td>
                                <cc1:CodeMstrDropDownList ID="ddlType" Code="WFSType" runat="server" />
                            </td>
                            <td style="text-align: right">
                                <asp:Label ID="lblGroupDesc1" runat="server" Text="${ISI.TaskSubType.Apply.GroupDesc1}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbGroupDesc1" runat="server" Width="150" TabIndex="1" />

                            </td>
                            <td style="text-align: right">
                                <asp:Label ID="lblGroupDesc2" runat="server" Text="${ISI.TaskSubType.Apply.GroupDesc2}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbGroupDesc2" runat="server" Width="150" TabIndex="1" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right">
                                <asp:Label ID="lblSeq" runat="server" Text="${ISI.TaskSubType.Apply.Seq}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbSeq" runat="server" Width="40" TabIndex="1" CssClass="inputRequired" />
                                <asp:RequiredFieldValidator ID="rfvSeq" runat="server" ControlToValidate="tbSeq"
                                    Display="Dynamic" ErrorMessage="${Common.String.Empty}" ValidationGroup="vgAdd" Enabled="false" />
                                <asp:RangeValidator ID="rvSeq" runat="server" ControlToValidate="tbSeq" ErrorMessage="${Common.Validator.Valid.Number}"
                                    Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" />
                            </td>
                            <td style="text-align: right">
                                <asp:Label ID="lblCurrency" runat="server" Text="${ISI.TaskSubType.Apply.Currency}:" />
                            </td>
                            <td>
                                <uc3:textbox ID="tbCurrency" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                    ValueField="Code" ServicePath="CurrencyMgr.service" ServiceMethod="GetAllCurrency" Width="250" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right">
                                <asp:Label ID="lblApply" runat="server" Text="${ISI.TaskSubType.Apply.Code}:" />
                            </td>
                            <td>
                                <uc3:textbox ID="tbApply" runat="server" Visible="true" DescField="Desc1" MustMatch="false"
                                    ValueField="Code" ServicePath="ApplyMgr.service" ServiceMethod="GetAllApply" Width="250" CssClass="inputRequired" />
                            </td>
                            <td style="text-align: right">
                                <asp:Label ID="lblDesc1" runat="server" Text="${ISI.TaskSubType.Apply.Desc1}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbDesc1" runat="server" Width="150" TabIndex="1" CssClass="inputRequired" />
                                <asp:RequiredFieldValidator ID="rfvDesc1" runat="server" ControlToValidate="tbDesc1"
                                    Display="Dynamic" ErrorMessage="${Common.String.Empty}" ValidationGroup="vgAdd" Enabled="false" />
                            </td>
                            <td style="text-align: right">
                                <asp:Label ID="lblDesc2" runat="server" Text="${ISI.TaskSubType.Apply.Desc2}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbDesc2" runat="server" Width="150" TabIndex="1" CssClass="inputRequired" />
                                <asp:RequiredFieldValidator ID="rfvDesc2" runat="server" ControlToValidate="tbDesc2"
                                    Display="Dynamic" ErrorMessage="${Common.String.Empty}" ValidationGroup="vgAdd" Enabled="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right">
                                <asp:Label ID="lblUOM" runat="server" Text="${ISI.TaskSubType.Apply.UOM}:" />

                            </td>
                            <td>
                                <uc3:textbox ID="tbUOM" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                    ValueField="Code" ServicePath="UomMgr.service" ServiceMethod="GetAllUom" Width="250" />
                            </td>

                            <td style="text-align: right">
                                <asp:Label ID="lblUOMDesc1" runat="server" Text="${ISI.TaskSubType.Apply.UOMDesc1}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbUOMDesc1" runat="server" Width="150" />
                            </td>
                            <td style="text-align: right">
                                <asp:Label ID="lblUOMDesc2" runat="server" Text="${ISI.TaskSubType.Apply.UOMDesc2}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbUOMDesc2" runat="server" Width="150" TabIndex="1" />
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TaskSubType.Service}" Visible="false">
                <ItemTemplate>
                    <table class="mtable">
                        <tr>
                            <td style="text-align: right">
                                <asp:Label ID="lblServicePath" runat="server" Text="${ISI.TaskSubType.Apply.ServicePath}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbServicePath" runat="server" Width="150" TabIndex="1" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right">
                                <asp:Label ID="lblServiceMethod" runat="server" Text="${ISI.TaskSubType.Apply.ServiceMethod}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbServiceMethod" runat="server" Width="150" TabIndex="1" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right">
                                <asp:Label ID="lblValueField" runat="server" Text="${ISI.TaskSubType.Apply.ValueField}:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbValueField" runat="server" Width="150" TabIndex="1" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right">
                                <asp:Label ID="lblDescField" runat="server" Text="${ISI.TaskSubType.Apply.DescField}:" />
                            </td>

                            <td>
                                <asp:TextBox ID="tbDescField" runat="server" Width="150" TabIndex="1" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: center" colspan="2">
                                <asp:CheckBox ID="cbMustMatch" runat="server" Text="${ISI.TaskSubType.Apply.MustMatch}" />
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TaskSubType.Opt}">
                <ItemTemplate>
                    <div>
                        <asp:CheckBox ID="cbIsRow" runat="server" Text="${ISI.TaskSubType.Apply.IsRow}" />
                        <asp:CheckBox ID="cbRequired" runat="server" Text="${ISI.TaskSubType.Apply.Required}" />
                    </div>
                    <div>
                        <asp:CheckBox ID="cbIsUser" runat="server" Text="${ISI.TaskSubType.Apply.IsUser}" />
                        <asp:CheckBox ID="cbIsVertical" runat="server" Text="${ISI.TaskSubType.Apply.IsVertical}" />
                    </div>
                    <div>
                        <asp:Label ID="lblRepeatColumns" runat="server" Text="${ISI.TaskSubType.Apply.RepeatColumns}:" />
                        <asp:TextBox ID="tbRepeatColumns" runat="server" Width="40" TabIndex="1" />
                        <asp:RangeValidator ID="rvRepeatColumns" runat="server" ControlToValidate="tbRepeatColumns" ErrorMessage="${Common.Validator.Valid.Number}"
                            Display="Dynamic" Type="Integer" MinimumValue="1" MaximumValue="100" ValidationGroup="vgSave" />
                    </div>
                    <div>
                        <asp:Label ID="lblFontSize" runat="server" Text="${ISI.TaskSubType.Apply.FontSize}:" />
                        <asp:TextBox ID="tbFontSize" runat="server" Width="40" TabIndex="1" />
                        <asp:RangeValidator ID="rvFontSize" runat="server" ControlToValidate="tbFontSize" ErrorMessage="${Common.Validator.Valid.Number}"
                            Display="Dynamic" Type="Integer" MinimumValue="1" MaximumValue="100000" ValidationGroup="vgSave" />
                    </div>
                    <div>
                        <asp:Label ID="lblColor" runat="server" Text="${ISI.TaskSubType.Apply.Color}:" />
                        <asp:TextBox ID="tbColor" runat="server" Width="40" TabIndex="1" />
                    </div>
                    <div>
                        <asp:Label ID="lblAlign" runat="server" Text="${ISI.TaskSubType.Apply.Align}:" />
                        <cc1:CodeMstrDropDownList ID="ddlAlign" Code="WFSAlign" runat="server" IncludeBlankOption="true" DefaultSelectedValue="" />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreateUser" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateUser}" Visible="false" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" Visible="false" />
            <asp:BoundField DataField="LastModifyUserNm" SortExpression="LastModifyUserNm" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.LastModifyUser}" Visible="false" />
            <asp:BoundField DataField="LastModifyDate" SortExpression="LastModifyDate" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.LastModifyDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" Visible="false" />
            <asp:TemplateField HeaderText="${Common.GridView.Action}" Visible="true" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                        Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')">
                    </asp:LinkButton>
                    <asp:HiddenField ID="hfId" runat="server" Value='<%# Bind("Id") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <div>
        <asp:Button ID="btnApplySave" runat="server" Text="${Common.Button.Save}"
            CssClass="apply" OnClick="btnUpdate_Click" ValidationGroup="vgAdd" />
    </div>
</fieldset>

