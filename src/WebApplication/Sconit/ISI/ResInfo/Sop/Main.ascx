<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_ResInfo_Sop_Main" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblWorkShop" runat="server" Text="${ISI.ResMatrix.WorkShop}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbWorkShop" runat="server" DescField="Name"
                    ValueField="Code" ServicePath="ResWokShopMgr.service" ServiceMethod="GetAllResWokShop"
                    MustMatch="true" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblOperate" runat="server" Text="${ISI.ResMatrix.Operate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbOperate" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="t02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="query"
                        OnClick="btnSearch_Click" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fs" runat="server" visible="false">
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
                <asp:TemplateField HeaderText="${ISI.TSK.Attachment}">
                    <ItemTemplate>
                        <asp:GridView ID="GV_List_Attachment" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_Attachment_RowDataBound"
                            CellPadding="0" AllowSorting="false" ShowHeader="false" RowStyle-VerticalAlign="Top">
                            <Columns>
                                <asp:TemplateField HeaderText="${ISI.TSK.Attachment}">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbtnDownLoad" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                            OnClick="lbtnDownLoad_Click" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
