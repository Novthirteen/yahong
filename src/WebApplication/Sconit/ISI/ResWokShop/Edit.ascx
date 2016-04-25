<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Modules_ISI_ResWokShop_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<asp:FormView ID="FV_ResWokShop" runat="server" DataSourceID="ODS_ResWokShop" DefaultMode="Edit"
    DataKeyNames="Code" OnDataBound="FV_ResWokShop_DataBound">
    <EditItemTemplate>
        <fieldset>
            <legend>${Common.Edit}</legend>
            <table class="mtable">
                <tr>
                    <td class="td01">${ISI.ResWokShop.Code}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' ReadOnly="true" />
                    </td>
                    <td class="td01">
                            <asp:Literal ID="lblParentCode" runat="server" Text="${ISI.ResWorkShop.ParentCode}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbParentCode" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="ResWokShopMgr.service" ServiceMethod="GetAllResWokShop"
                                Width="200" />
                        </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResWokShop.Name}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbName" runat="server" Text='<%# Bind("Name") %>' CssClass="inputRequired" />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="tbName"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                    <td class="td01">${ISI.ResWokShop.IsActive}:</td>
                    <td class="td02">
                        <asp:CheckBox ID="cbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' /></td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResWokShop.CreateDate}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCreateDate" runat="server" Text='<%# Bind("CreateDate") %>' ReadOnly="true" />
                    </td>
                    <td class="td01">${ISI.ResWokShop.CreateUser}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCreateUser" runat="server" Text='<%# Bind("CreateUser") %>' ReadOnly="true" />
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResWokShop.LastModifyDate}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbLastModifyDate" runat="server" Text='<%# Bind("LastModifyDate") %>' ReadOnly="true" />
                    </td>
                    <td class="td01">${ISI.ResWokShop.LastModifyUser}:</td>
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
<asp:ObjectDataSource ID="ODS_ResWokShop" runat="server" TypeName="com.Sconit.Web.ResWokShopMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.ResWokShop" UpdateMethod="UpdateResWokShop"
    OnUpdated="ODS_ResWokShop_Updated" OnUpdating="ODS_ResWokShop_Updating" SelectMethod="LoadResWokShop"
    DeleteMethod="DeleteResWokShop" OnDeleted="ODS_ResWokShop_Deleted">
    <SelectParameters>
        <asp:Parameter Name="Code" Type="String" />
    </SelectParameters>
    <DeleteParameters>
        <asp:Parameter Name="Code" Type="String" />
    </DeleteParameters>
    <UpdateParameters>
    </UpdateParameters>
</asp:ObjectDataSource>
