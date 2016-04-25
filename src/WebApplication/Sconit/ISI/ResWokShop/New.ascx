<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Modules_ISI_ResWokShop_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<asp:FormView ID="FV_ResWokShop" runat="server" DataSourceID="ODS_ResWokShop" DefaultMode="Insert"
    DataKeyNames="Code">
    <InsertItemTemplate>
        <fieldset>
            <legend>${Common.New}</legend>
            <table class="mtable">
                <tr>
                    <td class="td01">${ISI.ResWokShop.Code}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' CssClass="inputRequired" />
                        <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="tbCode"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                    <td class="td01">${ISI.ResWokShop.Name}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbName" runat="server" Text='<%# Bind("Name") %>' CssClass="inputRequired" />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="tbName"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResWokShop.IsActive}:</td>
                    <td class="td02">
                        <asp:CheckBox ID="cbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' /></td>
                        <td class="td01">
                            <asp:Literal ID="lblParentCode" runat="server" Text="${ISI.ResWorkShop.ParentCode}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbParentCode" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="ResWokShopMgr.service" ServiceMethod="GetAllResWokShop"
                                Width="200" />
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

<asp:ObjectDataSource ID="ODS_ResWokShop" runat="server" TypeName="com.Sconit.Web.ResWokShopMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.ResWokShop" InsertMethod="CreateResWokShop"
    OnInserted="ODS_ResWokShop_Inserted" OnInserting="ODS_ResWokShop_Inserting" SelectMethod="LoadResWokShop">
    <SelectParameters>
        <asp:Parameter Name="Code" Type="String" />
    </SelectParameters>
    <InsertParameters>
    </InsertParameters>
</asp:ObjectDataSource>
