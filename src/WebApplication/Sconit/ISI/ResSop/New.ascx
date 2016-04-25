<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Modules_ISI_ResSop_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<asp:FormView ID="FV_ResSop" runat="server" DataSourceID="ODS_ResSop" DefaultMode="Insert"
    DataKeyNames="WorkShop">
    <InsertItemTemplate>
        <fieldset>
            <legend>${Common.New}</legend>
            <table class="mtable">
                <tr>
                    <td class="td01">${ISI.ResSop.WorkShop}:</td>
                    <td class="td02">
                        <%--<asp:TextBox ID="tbWorkShop" runat="server" Text='<%# Bind("WorkShop") %>' CssClass="inputRequired" />--%>
                        <uc3:textbox ID="tbWorkShop" runat="server" DescField="Name" CssClass="inputRequired"
                            ValueField="Code" ServicePath="ResWokShopMgr.service" ServiceMethod="GetAllResWokShop"
                            MustMatch="true" />
                        <asp:RequiredFieldValidator ID="rfvWorkShop" runat="server" ControlToValidate="tbWorkShop"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                    <td class="td01">${ISI.ResSop.Operate}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbOperate" runat="server" Text='<%# Bind("Operate") %>' CssClass="inputRequired" />
                        <asp:RequiredFieldValidator ID="rfvOperate" runat="server" ControlToValidate="tbOperate"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                        <asp:RangeValidator ID="rvOperate" runat="server" ControlToValidate="tbOperate" ErrorMessage="${Common.Validator.Valid.Number}"
                            Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" />
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResSop.OperateDesc}:</td>
                    <td class="td02" colspan="3">
                        <asp:TextBox ID="tbOperateDesc" runat="server" Text='<%# Bind("OperateDesc") %>' Width="77%" Height="180"
                            TextMode="MultiLine" onkeypress="setMaxLength(this,500);" Font-Size="10" onpaste="limitPaste(this, 500)" />
                        <asp:RequiredFieldValidator ID="rfvOperateDesc" runat="server" ControlToValidate="tbOperateDesc"
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

<asp:ObjectDataSource ID="ODS_ResSop" runat="server" TypeName="com.Sconit.Web.ResSopMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.ResSop" InsertMethod="CreateResSop"
    OnInserted="ODS_ResSop_Inserted" OnInserting="ODS_ResSop_Inserting" SelectMethod="LoadResSop">
    <SelectParameters>
        <asp:Parameter Name="WorkShop" Type="String" />
    </SelectParameters>
    <InsertParameters>
        <asp:Parameter Name="Instruction" Type="Int32" ConvertEmptyStringToNull="true" />
    </InsertParameters>
</asp:ObjectDataSource>
