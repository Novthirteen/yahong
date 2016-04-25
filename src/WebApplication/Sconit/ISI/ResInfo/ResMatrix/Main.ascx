<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_ResInfo_ResMatrix_Main" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">${ISI.ResMatrix.WorkShop}:</td>
            <td class="td02">
                <uc3:textbox ID="tbWorkShop" runat="server" DescField="Name" 
                    ValueField="Code" ServicePath="ResWokShopMgr.service" ServiceMethod="GetAllResWokShop"
                    MustMatch="true" />
            </td>
            <td class="td01">${ISI.ResMatrix.Operate}:</td>
            <td class="td02">
                <asp:TextBox ID="tbOperate" runat="server" /></td>
        </tr>
        <tr>
            <td class="td01">${ISI.ResMatrix.Role}:</td>
            <td class="td02">
                <uc3:textbox ID="tbRole" runat="server" DescField="Name" 
                    ValueField="Code" ServicePath="ResRoleMgr.service" ServiceMethod="GetAllResRole"
                    MustMatch="true" />
            </td>
            <td class="td01"></td>
            <td class="td02"></td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="t02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="query"
                        OnClick="btnSearch_Click" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                        OnClick="btnExport_Click" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fldList" runat="server" visible="false">
    <%--<asp:BoundField DataField="UserCode" HeaderText="${ISI.Responsibility.UserCode}" />--%>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="True" AllowPaging="True" PagerID="gp" Width="100%"
            TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll" SelectCountMethod="FindCount"
            OnRowDataBound="GV_List_RowDataBound"  DefaultSortExpression="Seq">
            <Columns>
                <asp:BoundField DataField="WorkShop" HeaderText="${ISI.ResMatrix.WorkShop}" />
                <asp:BoundField DataField="Operate" HeaderText="${ISI.ResMatrix.Operate}" />
                <asp:BoundField DataField="Role" HeaderText="${ISI.ResMatrix.Role}" />
                <asp:BoundField DataField="Responsibility" HeaderText="${ISI.ResMatrix.Responsibility}" />
                <asp:BoundField DataField="Role" HeaderText="${ISI.Responsibility.Director}" />
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>


<fieldset runat="server" id="fld_Gv_List" style="display:none">
    <asp:GridView ID="Gv_Export" runat="server" AutoGenerateColumns="true" OnRowDataBound="GV_Export_RowDataBound"
        CellPadding="0" AllowSorting="false">
        <Columns>
            <asp:TemplateField HeaderText="Seq">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</fieldset>
