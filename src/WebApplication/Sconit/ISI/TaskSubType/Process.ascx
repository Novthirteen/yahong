<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Process.ascx.cs" Inherits="ISI_TaskSubType_Process" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<div>
    <asp:FormView ID="FV_TaskSubType" runat="server" DataSourceID="ODS_TaskSubType" DefaultMode="Edit"
        Width="100%" DataKeyNames="Code" OnDataBound="FV_TaskSubType_DataBound">
        <EditItemTemplate>
            <%# TaskSubTypeCode %><%# TaskSubTypeDesc %>
            <fieldset>
                <legend>${ISI.TaskSubType.ProcessOpt}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsWF" runat="server" Text="${ISI.TaskSubType.IsWF}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsWF" runat="server" Checked='<%# Bind("IsWF") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlProcessNo" runat="server" Text="${ISI.TaskSubType.ProcessNo}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbProcessNo" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetWFSTaskSubType" Width="260" />
                        </td>
                    </tr>
                    <tr>

                        <td class="td01">
                            <asp:Literal ID="lblIsTrace" runat="server" Text="${ISI.TaskSubType.IsTrace}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsTrace" runat="server" Checked='<%# Bind("IsTrace") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsCostCenter" runat="server" Text="${ISI.TaskSubType.IsCostCenter}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsCostCenter" runat="server" Checked='<%# Bind("IsCostCenter") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsAttachment" runat="server" Text="${ISI.TaskSubType.IsAttachment}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsAttachment" runat="server" Checked='<%# Bind("IsAttachment") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsPrint" runat="server" Text="${ISI.TaskSubType.IsPrint}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsPrint" runat="server" Checked='<%# Bind("IsPrint") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsAssignUser" runat="server" Text="${ISI.TaskSubType.IsAssignUser}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsAssignUser" runat="server" Checked='<%# Bind("IsAssignUser") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsCtrl" runat="server" Text="${ISI.TaskSubType.IsCtrl}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsCtrl" runat="server" Checked='<%# Bind("IsCtrl") %>' />
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
    <legend>${ISI.TaskSubType.Flow}</legend>
    <asp:GridView ID="GV_List_ProcessDefinition" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField HeaderText="${Common.Business.Description}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:HiddenField ID="hfId" runat="server" Value='<%# Bind("Id") %>' />
                    <asp:TextBox ID="tbDesc1" runat="server" InputWidth="150" TabIndex="1" CssClass="inputRequired" />
                    <asp:RequiredFieldValidator ID="rfvDesc1" runat="server" ControlToValidate="tbDesc1"
                        Display="Dynamic" ErrorMessage="${Common.String.Empty}" ValidationGroup="vgAdd" Enabled="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TaskSubType.ProcessDefinition.Seq}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <cc1:CodeMstrDropDownList ID="ddlLevel" Code="WFSLevel" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TaskSubType.ProcessDefinition.User}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <uc3:textbox ID="tbUserCode" runat="server" Visible="true" DescField="Name" MustMatch="true"
                        ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TaskSubType.ProcessDefinition.ATicket}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:CheckBox ID="cbATicket" runat="server" Checked='<%# Bind("ATicket") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TaskSubType.ProcessDefinition.IsCtrl}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:CheckBox ID="cbIsCtrl" runat="server" Checked='<%# Bind("IsCtrl") %>' />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="${ISI.TaskSubType.ProcessDefinition.Apply}/${ISI.TaskSubType.ProcessDefinition.UOM}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <div>
                        <uc3:textbox ID="tbApply" runat="server" Visible="true" DescField="Desc1" MustMatch="false"
                            ValueField="Code" ServicePath="ApplyMgr.service" ServiceMethod="GetAllApply" Width="250" />
                    </div>
                    <div>
                        <uc3:textbox ID="tbUOM" runat="server" Visible="true" DescField="Name" MustMatch="true"
                            ValueField="Code" ServicePath="UomMgr.service" ServiceMethod="GetAllUom" Width="250" />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TaskSubType.ProcessDefinition.Qty}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <div>
                        <asp:TextBox ID="tbApplyQty" runat="server" TabIndex="1" Width="80" />
                        <asp:RegularExpressionValidator ID="revApplyQty" ControlToValidate="tbApplyQty" runat="server"
                            ValidationGroup="vgAdd" ErrorMessage="${Common.Validator.Valid.Number}" ValidationExpression="^[0-9]+(.[0-9]{1,8})?$"
                            Display="Dynamic" />
                        <asp:RangeValidator ID="rvApplyQty" ControlToValidate="tbApplyQty" runat="server" Display="Dynamic"
                            ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="999999999" MinimumValue="0.00000001"
                            Type="Double" ValidationGroup="vgAdd" />
                    </div>
                    <div>
                        <asp:TextBox ID="tbQty" runat="server" TabIndex="1" Width="80" />
                        <asp:RegularExpressionValidator ID="revQty" ControlToValidate="tbQty" runat="server"
                            ValidationGroup="vgAdd" ErrorMessage="${Common.Validator.Valid.Number}" ValidationExpression="^[0-9]+(.[0-9]{1,8})?$"
                            Display="Dynamic" />
                        <asp:RangeValidator ID="rvQty" ControlToValidate="tbQty" runat="server" Display="Dynamic"
                            ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="999999999" MinimumValue="0.00000001"
                            Type="Double" ValidationGroup="vgAdd" />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreateUserNm" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateUser}" Visible="false" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" Visible="false" />
            <asp:BoundField DataField="LastModifyUserNm" SortExpression="LastModifyUser" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.LastModifyUser}" Visible="false" />
            <asp:BoundField DataField="LastModifyDate" SortExpression="LastModifyDate" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.LastModifyDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:TemplateField HeaderText="${Common.GridView.Action}" Visible="true" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnAdd" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                        Text="${Common.Button.New}" OnClick="lbtnAdd_Click" Visible="false"
                        ValidationGroup="vgAdd" TabIndex="1">
                    </asp:LinkButton>
                    <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
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


