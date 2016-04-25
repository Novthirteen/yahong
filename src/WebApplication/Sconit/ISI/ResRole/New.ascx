<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Modules_ISI_ResRole_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<asp:FormView ID="FV_ResRole" runat="server" DataSourceID="ODS_ResRole" DefaultMode="Insert"
    DataKeyNames="Code">
    <InsertItemTemplate>
        <fieldset>
            <legend>${Common.New}</legend>
            <table class="mtable">
                <tr>
                    <td class="td01">${ISI.ResRole.Code}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' CssClass="inputRequired" />
                        <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="tbCode"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                    <td class="td01">${ISI.ResRole.Name}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbName" runat="server" Text='<%# Bind("Name") %>' CssClass="inputRequired" />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="tbName"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResRole.RoleType}:</td>
                    <td class="td02">
                        <cc1:CodeMstrDropDownList runat="server" ID="ddlRoleType" Code="ISIResRoleType">
                        </cc1:CodeMstrDropDownList>
                    </td>
                    <td class="td01">${ISI.ResRole.IsActive}:</td>
                    <td class="td02">
                        <asp:CheckBox ID="cbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' /></td>
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

<asp:ObjectDataSource ID="ODS_ResRole" runat="server" TypeName="com.Sconit.Web.ResRoleMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.ResRole" InsertMethod="CreateResRole"
    OnInserted="ODS_ResRole_Inserted" OnInserting="ODS_ResRole_Inserting" SelectMethod="LoadResRole">
    <SelectParameters>
        <asp:Parameter Name="Code" Type="String" />
    </SelectParameters>
    <InsertParameters>
        <asp:Parameter Name="IsActive" Type="Boolean" ConvertEmptyStringToNull="true" />
        <%--    <asp:Parameter Name="RoleType" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Name" Type="String" ConvertEmptyStringToNull="true" />--%>
    </InsertParameters>
</asp:ObjectDataSource>
