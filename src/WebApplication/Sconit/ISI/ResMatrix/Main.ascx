<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Modules_ISI_ResMatrix_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="Edit.ascx" TagName="Edit" TagPrefix="uc1" %>
<%@ Register Src="New.ascx" TagName="New" TagPrefix="uc1" %>
<asp:RadioButtonList ID="rblType" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="rblType_SelectedIndexChanged" AutoPostBack="true">
    <asp:ListItem Selected="True" Text="单个" Value="0" />
    <asp:ListItem Selected="False" Text="批量" Value="1" />
</asp:RadioButtonList>
<hr />
<div id="single" runat="server">
    <fieldset id="fldSearch" runat="server">
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
                <td class="td01">${ISI.ResMatrix.Responsibility}:</td>
                <td class="td02">
                    <asp:TextBox ID="tbResponsibility" runat="server" /></td>
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
                ShowSeqNo="true" AllowSorting="false" AllowPaging="True" PagerID="gp" Width="100%"
                TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll" SelectCountMethod="FindCount"
                OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="Seq">
                <Columns>
                    <asp:BoundField DataField="WorkShop" HeaderText="${ISI.ResMatrix.WorkShop}" />
                    <asp:BoundField DataField="Operate" HeaderText="${ISI.ResMatrix.Operate}" />
                    <asp:BoundField DataField="Role" HeaderText="${ISI.ResMatrix.Role}" />
                    <asp:BoundField DataField="Responsibility" HeaderText="${ISI.ResMatrix.Responsibility}" />
                    <asp:CheckBoxField DataField="NeedPatrol" HeaderText="巡查" />
                    <asp:BoundField DataField="Seq" HeaderText="序号" />
                    <asp:BoundField DataField="TaskSubType" HeaderText="类型" />
                    <asp:BoundField DataField="NextPatrolTime" HeaderText="下一次巡查时间" />
                    <asp:TemplateField HeaderText="周期">
                        <ItemTemplate>
                            <cc1:CodeMstrLabel ID="ddlTimePeriodType" Code="TimePeriodType" runat="server" Value='<%# Bind("TimePeriodType") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
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
</div>
<div id="batch" runat="server" visible="false">
    <fieldset>
        <table class="mtable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblDirector" runat="server" Text="${ISI.Responsibility.Director}:" />
                </td>
                <td class="td02">
                    <uc3:textbox ID="tbDirector" runat="server" Visible="true" DescField="Name" MustMatch="true"
                        ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" Width="250" />
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
                        <asp:Button ID="btnBatchSearch" runat="server" Text="${Common.Button.Search}" CssClass="query"
                            OnClick="btnBatchSearch_Click" />
                        <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                            OnClick="btnBatchSearch_Click" Visible="false" />
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset id="fs" runat="server" visible="false">
        <table class="mtable">
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblNewUser" runat="server" Text="${ISI.ResMatrix.NewUser}:" /></td>
                <td class="td02">
                    <uc3:textbox ID="tbNewUser" runat="server" DescField="Name" MustMatch="true"
                        ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" Width="250" />
                </td>
                <td class="td01">
                    <asp:Button ID="btnCloneUser" runat="server" Text="${Common.Button.Clone}" OnClick="btnCloneUser_Click" />
                </td>
                <td class="td02">
                    <asp:Literal ID="lblNewUserInfo" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="td01">
                    <asp:Literal ID="lblEndDate" runat="server" Text="${ISI.ResMatrix.EndDate}:" /></td>
                <td class="td02">
                    <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                </td>
                <td class="td01">
                    <asp:Button ID="btnBatchSet" runat="server" Text="${Common.Button.Update}" OnClick="btnBatchSet_Click" />
                </td>
                <td class="td02">
                    <asp:Literal ID="lblEndDateInfo" runat="server" />
                </td>
            </tr>
        </table>

        <asp:GridView ID="Gv_Batch" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_Batch_RowDataBound"
            CellPadding="0" AllowSorting="false">
            <Columns>
                <asp:TemplateField HeaderText="Seq">
                    <ItemTemplate>
                        <%#Container.DataItemIndex + 1%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="WorkShop" HeaderText="${ISI.Responsibility.WorkShop}" />
                <asp:BoundField DataField="Operate" HeaderText="${ISI.Responsibility.Operate}" />
                <asp:BoundField DataField="Role" HeaderText="${ISI.Responsibility.Role}" />
                <asp:BoundField DataField="Responsibility" HeaderText="${ISI.Responsibility.ResDesc}" ItemStyle-Width="60%" ItemStyle-Wrap="True" />
                <asp:BoundField DataField="StartDate" HeaderText="${ISI.ResUser.StartDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="EndDate" HeaderText="${ISI.ResUser.EndDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnAdd" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ResMatrixId") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </fieldset>
</div>

<uc1:Edit ID="ucEdit" runat="server" Visible="False" />
<uc1:New ID="ucNew" runat="server" Visible="False" />

