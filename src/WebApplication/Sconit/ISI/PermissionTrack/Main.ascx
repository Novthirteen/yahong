<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_PermissionTrack_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<script type="text/javascript" language="javascript">

    function GVCheckClick(oCheckbox) {

        if (oCheckbox.checked == true) {
            $(".GVRow span[name='cbGroup'] input:checkbox").attr("checked", true);
            $(".GVAlternatingRow span[name='cbGroup'] input:checkbox").attr("checked", true);
        }
        else {
            $(".GVRow span[name='cbGroup'] input:checkbox").attr("checked", false);
            $(".GVAlternatingRow span[name='cbGroup'] input:checkbox").attr("checked", false);
        }
    }
</script>
<fieldset>
    <table class="mtable">
        <tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblCategoryType" runat="server" Text="${Security.Permission.Category.Type}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbCategoryType" runat="server" Visible="true" Width="250" DescField="Description"
                        ValueField="Value" ServicePath="CodeMasterMgr.service" ServiceMethod="GetCachedCodeMaster" ServiceParameter="string: PermissionCategoryType" />
                </td>
                <td class="td01">
                    <asp:Literal ID="lblCategory" runat="server" Text="${Security.Permission.Category.Description}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbCategory" runat="server" Visible="true" Width="250" DescField="Description"
                        ValueField="Code" ServicePath="PermissionCategoryMgr.service" ServiceMethod="GetCategoryByType"
                        ServiceParameter="string:#tbCategoryType" />
                </td>
            </tr>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblPermission" runat="server" Text="${ISI.PermissionTrack.Permission}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbPermission" runat="server" Visible="true" DescField="Description" MustMatch="true"
                    ValueField="Code" ServicePath="PermissionMgr.service" ServiceMethod="GetALlPermissionsByCategory" ServiceParameter="string:#tbCategory" Width="250" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblUser" runat="server" Text="${ISI.PermissionTrack.User}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
        </tr>
        <tr>
            <td class="td01" colspan="3"></td>
            <td class="t02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="button2"
                    OnClick="btnSearch_Click" ValidationGroup="vgSearch" />
                <asp:Button ID="btnRemovePermission" runat="server" Text="${ISI.PermissionTrack.Button.RemovePermission}" CssClass="button2"
                    OnClick="btnRemovePermission_Click" ValidationGroup="vgSearch" />
                <asp:Button ID="btnRemoveRole" runat="server" Text="${ISI.PermissionTrack.Button.RemoveRole}" CssClass="button2"
                    OnClick="btnRemoveRole_Click" ValidationGroup="vgSearch" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                    OnClick="btnExport_Click" ValidationGroup="vgSearch" />
            </td>
        </tr>
    </table>
</fieldset>
<fieldset runat="server" id="fld_Gv_List" visible="false">
    <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
        CellPadding="0" AllowSorting="false" OnDataBound="GV_List_DataBound">
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:CheckBox ID="CheckAll" onclick="GVCheckClick(this)" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HiddenField ID="hfUpId" runat="server" Value='<%# Bind("UP_ID") %>' />
                    <asp:HiddenField ID="hfUrId" runat="server" Value='<%# Bind("UR_ID") %>' />
                    <asp:HiddenField ID="hfType" runat="server" Value='<%# Bind("Type") %>' />
                    <asp:HiddenField ID="hfUserCode" runat="server" Value='<%# Bind("UserCode") %>' />
                    <asp:CheckBox ID="cbGroup" name="cbGroup" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="PMC_Type" HeaderStyle-Wrap="false" SortExpression="PMC_Type" HeaderText="${ISI.PermissionTrack.PMC_Type}" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="PM_CateCode" HeaderStyle-Wrap="false" SortExpression="PM_CateCode" HeaderText="${ISI.PermissionTrack.PM_CateCode}" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="PM_Code" HeaderStyle-Wrap="false" SortExpression="PM_Code" HeaderText="${ISI.PermissionTrack.PM_Code}" ItemStyle-Wrap="false" />
            <asp:CheckBoxField DataField="PM_IsSuper" HeaderStyle-Wrap="false" SortExpression="PM_IsSuper" HeaderText="${ISI.PermissionTrack.PM_IsSuper}" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="Type" Visible="false" HeaderStyle-Wrap="false" SortExpression="Type" HeaderText="${ISI.PermissionTrack.Type}" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="ROLE_Code" HeaderStyle-Wrap="false" SortExpression="ROLE_Code" HeaderText="${ISI.PermissionTrack.ROLE_Code}" ItemStyle-Wrap="false" />
            <asp:CheckBoxField DataField="ROLE_IsSuper" HeaderStyle-Wrap="false" SortExpression="ROLE_IsSuper" HeaderText="${ISI.PermissionTrack.ROLE_IsSuper}" ItemStyle-Wrap="false" />
            <asp:CheckBoxField DataField="ROLE_AllowDel" Visible="false" HeaderStyle-Wrap="false" SortExpression="ROLE_AllowDel" HeaderText="${ISI.PermissionTrack.ROLE_AllowDel}" ItemStyle-Wrap="false" />
            <asp:BoundField DataField="UserCount" HeaderStyle-Wrap="false" SortExpression="UserCount" HeaderText="${ISI.PermissionTrack.UserCount}" ItemStyle-Wrap="false" />
            
            <asp:BoundField DataField="USR_Code" HeaderStyle-Wrap="false" SortExpression="USR_Code" HeaderText="${ISI.PermissionTrack.User}" ItemStyle-Wrap="false" />
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>

