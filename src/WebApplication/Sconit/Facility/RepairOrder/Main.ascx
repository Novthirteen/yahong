<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_RepairOrder_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="Edit.ascx" TagName="Edit" TagPrefix="uc1" %>
<%@ Register Src="New.ascx" TagName="New" TagPrefix="uc1" %>
<fieldset id="fldSearch" runat="server">
    <table class="mtable">
        <tr>
            <td class="td01">报修单号:</td>
            <td class="td02">
                <asp:TextBox ID="tbOrderNo" runat="server" />
            </td>
            <td class="td01">设施代码:</td>
            <td class="td02">
                <asp:TextBox ID="tbFCID" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">内部资产代码:</td>
            <td class="td02">
                <asp:TextBox ID="tbAssetNo" runat="server" />
            </td>
            <td class="td01">设施名称:</td>
            <td class="td02">
                <asp:TextBox ID="tbFCName" runat="server" />
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
                    <asp:ListItem Text="等待维修" Value="等待维修" />
                    <asp:ListItem Text="等待验收" Value="等待验收" />
                    <asp:ListItem Text="等待关闭" Value="等待关闭" />
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
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="OrderNo"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" seqtext="No."
            ShowSeqNo="true" AllowSorting="false" AllowPaging="True" PagerID="gp" Width="100%"
            TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll" SelectCountMethod="FindCount"
            OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="OrderNo" CellMaxLength="10" SkinID="GV">
            <Columns>
                <asp:BoundField DataField="OrderNo" HeaderText="报修单号" />
                <asp:BoundField DataField="FCID" HeaderText="设施代码" />
                <asp:BoundField DataField="FCName" HeaderText="设施名称" />
                <asp:BoundField DataField="FaultDescription" HeaderText="描述" />
                <asp:BoundField DataField="SubmitTime" HeaderText="报修时间" />
                <asp:BoundField DataField="SubmitUser" HeaderText="报修人" />
                <asp:BoundField DataField="HaltStartTime" HeaderText="停机开始" />
                <asp:BoundField DataField="HaltEndTime" HeaderText="停机结束" />
                <asp:BoundField DataField="Status" HeaderText="状态" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'
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


