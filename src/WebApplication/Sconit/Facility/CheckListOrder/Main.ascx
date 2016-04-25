<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_CheckListOrder_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="Edit.ascx" TagName="Edit" TagPrefix="uc1" %>
<%@ Register Src="New.ascx" TagName="New" TagPrefix="uc1" %>
<fieldset id="fldSearch" runat="server">
    <table class="mtable">
        <tr>
            <td class="td01">代码:</td>
            <td class="td02">
                <asp:TextBox ID="tbCode" runat="server" />
            </td>
            <td class="td01">巡检名称:</td>
            <td class="td02">
                <asp:TextBox ID="tbName" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">巡检设备:</td>
            <td class="td02">
                <asp:TextBox ID="tbToolNumber" runat="server" />
            </td>
            <td class="td01">区域:</td>
            <td class="td02">
                <asp:TextBox ID="tbRegion" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
              </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">状态:</td>
            <td class="td02">
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Text="" Value="" Selected="True" />
                    <asp:ListItem Text="创建" Value="Create" />
                    <asp:ListItem Text="关闭" Value="Close" />
                </asp:DropDownList>
            </td>

        </tr>
        <tr>
            <td colspan="3" />
            <td class="td02">
                <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click"
                    CssClass="button2" />
                <asp:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
                    CssClass="button2" />
                <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click"
                    CssClass="query" />
            </td>
        </tr>

    </table>
</fieldset>
<fieldset id="fldList" runat="server" visible="false">
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" seqtext="No."
            ShowSeqNo="true" AllowSorting="false" AllowPaging="True" PagerID="gp" Width="100%"
            TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll" SelectCountMethod="FindCount"
            OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="Code" CellMaxLength="10" SkinID="GV">
            <Columns>
                <asp:BoundField DataField="Code" HeaderText="代码" />
                <asp:BoundField DataField="CheckListCode" HeaderText="巡检代码" />
                <asp:BoundField DataField="CheckListName" HeaderText="巡检名称" />
                <%--<asp:BoundField DataField="FacilityID" HeaderText="巡检设备号" />
                <asp:BoundField DataField="FacilityName" HeaderText="设备名称" />--%>
                <asp:BoundField DataField="Region" HeaderText="区域" />
                <asp:BoundField DataField="Description" HeaderText="描述" />
                <asp:BoundField DataField="Remark" HeaderText="备注" />
                <asp:BoundField DataField="CheckDate" HeaderText="巡检时间" />
                <asp:BoundField DataField="CheckUser" HeaderText="巡检人" />
                <asp:BoundField DataField="Status" HeaderText="状态" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="30">
        </cc1:GridPager>
    </div>
</fieldset>
<uc1:Edit ID="ucEdit" runat="server" Visible="False" />
<uc1:New ID="ucNew" runat="server" Visible="False" />

