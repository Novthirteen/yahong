<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Modules_ISI_ResMatrix_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/MRP/PlanSchedule/DateSelect.ascx" TagName="DateSelect" TagPrefix="uc2" %>

<asp:FormView ID="FV_ResMatrix" runat="server" DataSourceID="ODS_ResMatrix" DefaultMode="Insert"
    DataKeyNames="Id">
    <InsertItemTemplate>
        <fieldset>
            <legend>${Common.New}</legend>
            <table class="mtable">
                <tr>
                    <td class="td01">${ISI.ResMatrix.WorkShop}:</td>
                    <td class="td02">
                        <%--<asp:TextBox ID="tbWorkShop" runat="server" Text='<%# Bind("WorkShop") %>' CssClass="inputRequired" />--%>
                        <uc3:textbox ID="tbWorkShop" runat="server" DescField="Name" CssClass="inputRequired"
                            ValueField="Code" ServicePath="ResWokShopMgr.service" ServiceMethod="GetAllResWokShop"
                            MustMatch="true" />
                        <asp:RequiredFieldValidator ID="rfvWorkShop" runat="server" ControlToValidate="tbWorkShop"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                    <td class="td01">${ISI.ResMatrix.Role}:</td>
                    <td class="td02">
                        <%--<asp:TextBox ID="tbRole" runat="server" Text='<%# Bind("Role") %>' CssClass="inputRequired" />--%>
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
                        <asp:TextBox ID="tbOperate" runat="server" Text='<%# Bind("Operate") %>' Width="50" />
                        <asp:RangeValidator ID="rvOperate" runat="server" ControlToValidate="tbOperate" ErrorMessage="${Common.Validator.Valid.Number}"
                            Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" /></td>
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
                            <asp:TextBox ID="tbNextPatrolTime"  CssClass="inputRequired" runat="server" Text='<%# Bind("NextPatrolTime","{0:yyyy-MM-dd HH:mm}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                           <asp:RequiredFieldValidator ID="rfvNextPatrolTime" runat="server" ErrorMessage="${Common.Error.NotNull}"
                                Display="Dynamic" ControlToValidate="tbNextPatrolTime" ValidationGroup="vgCreate" />
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
                        <asp:TextBox ID="tbResponsibility" runat="server" Text='<%# Bind("Responsibility") %>' Width="77%" Height="180" CssClass="inputRequired"
                            TextMode="MultiLine" onkeypress="setMaxLength(this,1000);" Font-Size="10" onpaste="limitPaste(this, 1000)" />
                        <asp:RequiredFieldValidator ID="rfvResponsibility" runat="server" ControlToValidate="tbResponsibility"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="tablefooter">
            <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
                CssClass="button2" ValidationGroup="vgSave" />
            <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                CssClass="button2" />
        </div>
    </InsertItemTemplate>
</asp:FormView>

<asp:ObjectDataSource ID="ODS_ResMatrix" runat="server" TypeName="com.Sconit.Web.ResMatrixMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.ResMatrix" InsertMethod="CreateResMatrix"
    OnInserted="ODS_ResMatrix_Inserted" OnInserting="ODS_ResMatrix_Inserting" SelectMethod="LoadResMatrix">
    <SelectParameters>
        <asp:Parameter Name="Id" Type="Int32" />
    </SelectParameters>
    <InsertParameters>
        <asp:Parameter Name="Operate" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Responsibility" Type="String" ConvertEmptyStringToNull="true" />
    </InsertParameters>
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
