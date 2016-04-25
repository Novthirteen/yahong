<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Modules_ISI_ResSop_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="Edit.ascx" TagName="Edit" TagPrefix="uc1" %>
<%@ Register Src="New.ascx" TagName="New" TagPrefix="uc1" %>
<fieldset id="fldSearch" runat="server">
    <table class="mtable">
        <tr>
            <td class="td01">${ISI.ResSop.WorkShop}:</td>
            <td class="td02">
                <%--<asp:TextBox ID="tbWorkShop" runat="server" />--%>
                <uc3:textbox id="tbWorkShop" runat="server" descfield="Name"
                    valuefield="Code" servicepath="ResWokShopMgr.service" servicemethod="GetAllResWokShop"
                    mustmatch="true" />
            </td>
            <td class="td01">${ISI.ResSop.Operate}:</td>
            <td class="td02">
                <asp:TextBox ID="tbOperate" runat="server" /></td>
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
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="True" AllowPaging="True" PagerID="gp" Width="100%"
            TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll" SelectCountMethod="FindCount"
            OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:BoundField DataField="WorkShop" HeaderText="${ISI.ResSop.WorkShop}" />
                <asp:BoundField DataField="Operate" HeaderText="${ISI.ResSop.Operate}" />
                <asp:BoundField DataField="OperateDesc" HeaderText="${ISI.ResSop.OperateDesc}" />
                <%--<asp:BoundField DataField="Instruction" HeaderText="${ISI.ResSop.Instruction}" />--%>
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
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
<uc1:Edit Id="ucEdit" runat="server" Visible="False" />
<uc1:New ID="ucNew" runat="server" Visible="False" />

