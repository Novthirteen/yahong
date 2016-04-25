<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Modules_ISI_ResRole_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<asp:FormView ID="FV_ResRole" runat="server" DataSourceID="ODS_ResRole" DefaultMode="Edit"
    DataKeyNames="Code" OnDataBound="FV_ResRole_DataBound">
    <EditItemTemplate>
        <fieldset>
            <legend>${Common.Edit}</legend>
            <table class="mtable">
                <tr>
                    <td class="td01">${ISI.ResRole.Code}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' ReadOnly="true" />
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
                </tr>
                <tr>
                    <td class="td01">${ISI.ResRole.CreateDate}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCreateDate" runat="server" Text='<%# Bind("CreateDate") %>' ReadOnly="true" />
                    </td>
                    <td class="td01">${ISI.ResRole.CreateUser}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCreateUser" runat="server" Text='<%# Bind("CreateUser") %>' ReadOnly="true" />
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResRole.LastModifyDate}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbLastModifyDate" runat="server" Text='<%# Bind("LastModifyDate") %>' ReadOnly="true" />
                    </td>
                    <td class="td01">${ISI.ResRole.LastModifyUser}:</td>
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
<asp:ObjectDataSource ID="ODS_ResRole" runat="server" TypeName="com.Sconit.Web.ResRoleMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.ResRole" UpdateMethod="UpdateResRole"
    OnUpdated="ODS_ResRole_Updated" OnUpdating="ODS_ResRole_Updating" SelectMethod="LoadResRole"
    DeleteMethod="DeleteResRole" OnDeleted="ODS_ResRole_Deleted">
    <SelectParameters>
        <asp:Parameter Name="Code" Type="String" />
    </SelectParameters>
    <DeleteParameters>
        <asp:Parameter Name="Code" Type="String" />
    </DeleteParameters>
    <UpdateParameters>
        <asp:Parameter Name="IsActive" Type="Boolean" ConvertEmptyStringToNull="true" />
    </UpdateParameters>
</asp:ObjectDataSource>
