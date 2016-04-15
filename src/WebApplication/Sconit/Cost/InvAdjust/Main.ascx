<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Cost_InvAdjust_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
        <span class='ajax__tab_active' id='tab_1' runat="server"><span class='ajax__tab_outer'>
            <span class='ajax__tab_inner'><span class='ajax__tab_tab'>
                <asp:LinkButton ID="lbn1" Text="查询" runat="server" OnClick="lbn1_Click" /></span></span></span></span><%--<span 
        id='tab_2' runat="server" ><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbn2" Text="盘点" runat="server" OnClick="lbn2_Click" /></span></span></span></span>--%><span
            id='tab_3' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span
                class='ajax__tab_tab'><asp:LinkButton ID="lbn3" Text="调整" runat="server" OnClick="lbn3_Click" /></span></span></span></span>
    </div>
    <div class="ajax__tab_body">
        <table class="mtable">
            <tr>
                <td>
                    <div id="div_1" runat="server">
                        <fieldset>
                            <table class="mtable">
                                <tr>
                                    <td class="td01">
                                        <asp:Literal ID="lblOrderno" runat="server" Text="${MasterData.MiscOrder.OrderNo}:" />
                                    </td>
                                    <td class="td02">
                                        <asp:TextBox ID="tbMiscOrderCode" runat="server" Visible="true" />
                                    </td>
                                    <td class="td01">
                                    </td>
                                    <td class="td02">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td01">
                                        <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
                                    </td>
                                    <td class="td02">
                                        <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker()" />
                                    </td>
                                    <td class="td01">
                                        <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
                                    </td>
                                    <td class="td02">
                                        <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker()" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td01">
                                        <asp:Literal ID="lblMiscOrderLocation" runat="server" Text="${Common.Business.Location}:" />
                                    </td>
                                    <td class="td02">
                                        <uc3:textbox ID="tbLocation" runat="server" Visible="true" DescField="Name" Width="280"
                                            ValueField="Code" ServicePath="LocationMgr.service" ServiceMethod="GetAllLocation"
                                            MustMatch="false" />
                                    </td>
                                    <td class="td01">
                                        <asp:Literal ID="lblItem" runat="server" Text="${MasterData.Item.Code}:" />
                                    </td>
                                    <td class="td02">
                                        <asp:TextBox ID="tbItem" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td01">
                                    </td>
                                    <td class="td02">
                                        <asp:RadioButtonList ID="rblListType" runat="server" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="计划外出入库" Value="Group" Selected="Misc" />
                                            <asp:ListItem Text="盘点" Value="CycleCount" />
                                        </asp:RadioButtonList>
                                    </td>
                                    <td class="td01">
                                    </td>
                                    <td class="td02">
                                        <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click" />
                                        <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch_Click" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <fieldset runat="server" id="fld_Gv_List">
                            <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="true" OnRowDataBound="GV_List_RowDataBound"
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
                    </div>
                    <div id="div_3" runat="server">
                        <fieldset>
                            <table class="mtable">
                                <tr>
                                    <td class="td01">
                                        <asp:Literal ID="ltlCostCenterCode" runat="server" Text="${MasterData.MiscOrder.CostCenterCode}:" />
                                    </td>
                                    <td class="td02">
                                        <uc3:textbox ID="tbCostGroup" runat="server" Visible="true" Width="250" DescField="Description"
                                            ValueField="Code" ServicePath="CostCenterMgr.service" ServiceMethod="GetAllCostCenter"
                                            CssClass="inputRequired" />
                                        <asp:RequiredFieldValidator ID="rfvCostGroup" runat="server" ControlToValidate="tbCostGroup"
                                            Display="Dynamic" ErrorMessage="${Cost.CostAllocateMethod.CostGroup.Required}"
                                            ValidationGroup="vgSave" />
                                    </td>
                                    <td class="td01">
                                        <asp:Literal ID="ltlSelect" runat="server" Text="${Common.FileUpload.PleaseSelect}:" />
                                    </td>
                                    <td class="td02">
                                        <asp:FileUpload ID="fileUpload" ContentEditable="false" runat="server" />
                                        <asp:HyperLink ID="hlTemplate" runat="server" Text="${Common.Business.ClickToDownload}"
                                            NavigateUrl="~/Cost/InvAdjust/InvAdjust.xls" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td01">
                                        <asp:Literal ID="ltlStartDate1" runat="server" Text="${Common.Business.StartDate}:" />
                                    </td>
                                    <td class="td02">
                                        <asp:TextBox ID="tbStartDate1" runat="server" onClick="WdatePicker()" CssClass="inputRequired" />
                                    </td>
                                    <td class="td01">
                                        <asp:Literal ID="ltlEndDate1" runat="server" Text="${Common.Business.EndDate}:" />
                                    </td>
                                    <td class="td02">
                                        <asp:TextBox ID="tbEndDate1" runat="server" onClick="WdatePicker()" CssClass="inputRequired" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td01">
                                    </td>
                                    <td class="td02">
                                    </td>
                                    <td class="td01">
                                    </td>
                                    <td class="td02">
                                        <asp:Button ID="btnImport" runat="server" Text="${Common.Button.Import}" OnClick="btnImport_Click" />
                                        <asp:Button ID="btnSearch1" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch1_Click" />
                                        <asp:Button ID="btnExport1" runat="server" Text="${Common.Button.Export}" OnClick="btnSearch1_Click" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <fieldset runat="server" id="fld_Adj">
                            <asp:GridView ID="GV_Adj" runat="server" AutoGenerateColumns="true" OnRowDataBound="GV_Adj_RowDataBound"
                                CellPadding="0" AllowSorting="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Seq">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <table>
                                <tr>
                                    <td colspan="3" />
                                    <td class="td02">
                                        <div class="buttons">
                                            <asp:Button ID="btnAdj" runat="server" Text="调整" OnClick="btnAdj_Click" Visible="false"/>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>
