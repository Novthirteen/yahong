<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Modules_ISI_ResMatrix_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<style type="text/css">
    .CssColor {
        text-align: center;
        height: 18px;
        vertical-align: middle;
    }
</style>

<asp:FormView ID="FV_ResMatrix" runat="server" DataSourceID="ODS_ResMatrix" DefaultMode="Edit"
    DataKeyNames="Id" OnDataBound="FV_ResMatrix_DataBound">
    <EditItemTemplate>
        <fieldset>
            <legend>${Common.Edit}</legend>
            <table class="mtable">
                <tr>
                    <td class="td01">${ISI.ResMatrix.WorkShop}:</td>
                    <td class="td02">
                        <uc3:textbox ID="tbWorkShop" runat="server" DescField="Name" CssClass="inputRequired"
                            ValueField="Code" ServicePath="ResWokShopMgr.service" ServiceMethod="GetAllResWokShop"
                            MustMatch="true" />
                        <asp:RequiredFieldValidator ID="rfvWorkShop" runat="server" ControlToValidate="tbWorkShop"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                    <td class="td01">${ISI.ResMatrix.Role}:</td>
                    <td class="td02">
                        <uc3:textbox ID="tbRole" runat="server" DescField="Name" CssClass="inputRequired"
                            ValueField="Code" ServicePath="ResRoleMgr.service" ServiceMethod="GetAllResRole"
                            MustMatch="true" />
                        <asp:RequiredFieldValidator ID="rfvRole" runat="server" ControlToValidate="tbRole"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                </tr>

                <tr>
                    <td class="td01">${ISI.ResMatrix.Operate}:
                    </td>
                    <td class="td02">
                        <asp:TextBox ID="tbOperate" runat="server" Text='<%# Bind("Operate") %>' />
                        <asp:HiddenField ID="hdOldOperate" runat="server" Value='<%# Bind("OldOperate") %>' />
                        <%--<asp:HiddenField ID="hdOldPriority" runat="server" Value='<%# Bind("OldPriority") %>' />--%>
                        <asp:HiddenField ID="hdOldRole" runat="server" Value='<%# Bind("OldRole") %>' />
                        <asp:HiddenField ID="hdOldWorkShop" runat="server" Value='<%# Bind("OldWorkShop") %>' />
                        <asp:HiddenField ID="hdOldResponsibility" runat="server" Value='<%# Bind("OldResponsibility") %>' />
                        <asp:HiddenField ID="hdNextPatrolTime" runat="server" Value='<%# Bind("NextPatrolTime") %>' />
                        <asp:RangeValidator ID="rvOperate" runat="server" ControlToValidate="tbOperate" ErrorMessage="${Common.Validator.Valid.Number}"
                            Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" />
                    </td>
                    <td class="td01">${Common.GridView.Seq}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbSequence" runat="server" Text='<%# Bind("Seq") %>' Width="50" CssClass="inputRequired" />
                        <asp:RequiredFieldValidator ID="rfSequence" runat="server" ControlToValidate="tbSequence"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                        <asp:RangeValidator ID="rvSequence" runat="server" ControlToValidate="tbSequence" ErrorMessage="${Common.Validator.Valid.Number}"
                            Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" /></td>
                </tr>
                <tr>
                    <td class="td01">类型:</td>
                    <td class="td02">
                        <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                            ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetAllTaskSubType" />
                    </td>
                    <td class="td01">
                        <asp:Literal ID="lblNextPatrolTime" runat="server" Text="${ISI.ResMatrix.NextPatrolTime}:" />
                    </td>
                    <td class="td02">
                        <asp:TextBox ID="tbNextPatrolTime" runat="server" Text='<%# Bind("NextPatrolTime","{0:yyyy-MM-dd HH:mm}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})"></asp:TextBox>

                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResMatrix.TimePeriodType}:</td>
                    <td class="td02">
                        <cc1:CodeMstrDropDownList ID="ddlTimePeriodType" Code="TimePeriodType" runat="server" />
                    </td>
                    <td class="td01"></td>
                    <td class="td02">
                        <asp:CheckBox ID="cbNeedPatrol" Text="${ISI.ResMatrix.NeedPatrol}" runat="server" Checked='<%# Bind("NeedPatrol") %>' />
                        <asp:CheckBox ID="cbIsIndependentTask" Text="独立任务" runat="server" Checked='<%# Bind("IsIndependentTask") %>' />
                    </td>
                </tr>

                <tr>
                    <td class="td01">${ISI.ResMatrix.Responsibility}:</td>
                    <td class="td02" colspan="3">
                        <asp:TextBox ID="tbResponsibility" runat="server" Text='<%# Bind("Responsibility") %>' Width="77%" Height="180"
                            TextMode="MultiLine" onkeypress="setMaxLength(this,1000);" Font-Size="10" onpaste="limitPaste(this, 1000)" />
                        <asp:RequiredFieldValidator ID="rfvResponsibility" runat="server" ControlToValidate="tbResponsibility"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                </tr>

                <tr>
                    <td class="td01">${ISI.ResMatrix.Director}:</td>
                    <td class="td02" colspan="3">
                        <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
                            CellPadding="0" AllowSorting="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Seq">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="${ISI.ResUser.UserCode}">
                                    <ItemTemplate>
                                        <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("UserCode") %>' Width="100" />
                                        <uc3:textbox ID="tbUserCode" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                            ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" Width="250" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="UserName" HeaderText="${ISI.ResUser.UserName}" />
                                <asp:TemplateField HeaderText="${ISI.ResUser.SkillLevel}">
                                    <ItemTemplate>
                                        <cc1:CodeMstrDropDownList ID="ddlSkillLevel" Code="ISISkillLevel" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="${ISI.ResUser.Priority}">
                                    <ItemTemplate>
                                        <asp:DropDownList runat="server" ID="ddlPriority">
                                            <asp:ListItem Selected="True" Text="P1" Value="P1" />
                                            <asp:ListItem Text="P2" Value="P2" />
                                            <asp:ListItem Text="P3" Value="P3" />
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="${ISI.ResUser.StartDate}">
                                    <ItemTemplate>
                                        <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="巡查">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbNeedPatrol" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="${ISI.ResUser.EndDate}">
                                    <ItemTemplate>
                                        <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbtnAdd" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                            Text="${Common.Button.New}" OnClick="lbtnAdd_Click" ValidationGroup="vgAdd" TabIndex="1">
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')">
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResMatrix.CreateDate}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCreateDate" runat="server" Text='<%# Bind("CreateDate") %>' ReadOnly="true" />
                    </td>
                    <td class="td01">${ISI.ResMatrix.CreateUser}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCreateUser" runat="server" Text='<%# Bind("CreateUser") %>' ReadOnly="true" />
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResMatrix.LastModifyDate}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbLastModifyDate" runat="server" Text='<%# Bind("LastModifyDate") %>' ReadOnly="true" />
                    </td>
                    <td class="td01">${ISI.ResMatrix.LastModifyUser}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbLastModifyUser" runat="server" Text='<%# Bind("LastModifyUser") %>' ReadOnly="true" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="tablefooter">
            <asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                CssClass="button2" ValidationGroup="vgSave" />
            <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
                CssClass="button2" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
            <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                CssClass="button2" />
        </div>
    </EditItemTemplate>
</asp:FormView>
<asp:ObjectDataSource ID="ODS_ResMatrix" runat="server" TypeName="com.Sconit.Web.ResMatrixMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.ResMatrix" UpdateMethod="UpdateResMatrix"
    OnUpdated="ODS_ResMatrix_Updated" OnUpdating="ODS_ResMatrix_Updating" SelectMethod="LoadResMatrix"
    DeleteMethod="DeleteResMatrix" OnDeleted="ODS_ResMatrix_Deleted">
    <SelectParameters>
        <asp:Parameter Name="Id" Type="Int32" />
    </SelectParameters>
    <DeleteParameters>
        <asp:Parameter Name="Id" Type="Int32" />
    </DeleteParameters>
    <UpdateParameters>
        <asp:Parameter Name="Operate" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="OldOperate" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Responsibility" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="OldResponsibility" Type="String" ConvertEmptyStringToNull="true" />
    </UpdateParameters>
</asp:ObjectDataSource>
<script type="text/javascript">
    $(document).ready(function () {
        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });
    });
</script>

