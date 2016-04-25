<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Modules_ISI_ResRole_Main" %>
<%--@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" --%>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="Edit.ascx" TagName="Edit" TagPrefix="uc1" %>
<%@ Register Src="New.ascx" TagName="New" TagPrefix="uc1" %>
<fieldset id="fldSearch" runat="server">
    <table class="mtable">
        <tr>
            <td class="td01">${ISI.ResRole.Code}:</td>
            <td class="td02">
                <asp:TextBox ID="tbCode" runat="server" /></td>
            <td class="td01">${ISI.ResRole.Name}:</td>
            <td class="td02">
                <asp:TextBox ID="tbName" runat="server" /></td>
        </tr>
        <tr>
            <td class="td01">${ISI.ResRole.RoleType}:</td>
            <td class="td02">
                <asp:DropDownList runat="server" ID="ddlRoleType">
                    <asp:ListItem Selected="True" Text="" Value="" />
                    <asp:ListItem Selected="False" Text="一线" Value="Direct" />
                    <asp:ListItem Text="二线" Value="Indirect" />
                    <asp:ListItem Text="白领" Value="Office" />
                    <asp:ListItem Text="管理" Value="Management" />
                </asp:DropDownList>
            </td>
            <td class="td01">${ISI.ResRole.IsActive}:</td>
            <td class="td02">
                <asp:CheckBox ID="cbIsActive" runat="server" Checked="true" /></td>
        </tr>
        <tr>
            <td colspan="3" />
            <td class="td02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                    CssClass="button2" />
                <asp:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
                    CssClass="button2" />
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fldList" runat="server" visible="false">
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="True" AllowPaging="True" PagerID="gp" Width="100%"
            TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll" SelectCountMethod="FindCount"
            OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="LastModifyDate"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:BoundField DataField="Code" HeaderText="${ISI.ResRole.Code}" />
                <asp:BoundField DataField="Name" HeaderText="${ISI.ResRole.Name}" />
                <%--<asp:BoundField DataField="RoleType" HeaderText="${ISI.ResRole.RoleType}" />--%>
                <asp:TemplateField HeaderText="${ISI.ResRole.RoleType}">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblRoleType" runat="server" Code="ISIResRoleType" Value='<%# Bind("RoleType") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CheckBoxField DataField="IsActive" HeaderText="${ISI.ResRole.IsActive}" />
                <asp:BoundField DataField="CreateDate" HeaderText="${ISI.ResRole.CreateDate}" />
                <asp:BoundField DataField="CreateUser" HeaderText="${ISI.ResRole.CreateUser}" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
<uc1:Edit ID="ucEdit" runat="server" Visible="False" />
<uc1:New ID="ucNew" runat="server" Visible="False" />

