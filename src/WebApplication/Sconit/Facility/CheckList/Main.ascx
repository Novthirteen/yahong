<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_CheckList_Main" %>
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
              <td class="td01">名称:</td>
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
        <cc1:gridview id="GV_List" runat="server" autogeneratecolumns="False" datakeynames="Code"
            allowmulticolumnsorting="false" autoloadstyle="false" seqno="0" seqtext="No."
            showseqno="true" allowsorting="True" allowpaging="True" pagerid="gp" width="100%"
            typename="com.Sconit.Web.CriteriaMgrProxy" selectmethod="FindAll" selectcountmethod="FindCount"
            onrowdatabound="GV_List_RowDataBound" defaultsortexpression="Code" >

            <Columns>
                <asp:BoundField DataField="Code" HeaderText="代码" />
                <asp:BoundField DataField="Name" HeaderText="名称" />
                <asp:BoundField DataField="FacilityID" HeaderText="巡检设备号" />
                <asp:BoundField DataField="FacilityName" HeaderText="设备名称" />
                <asp:BoundField DataField="Region" HeaderText="区域" />
                <asp:BoundField DataField="Description" HeaderText="描述" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <cc1:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            FunctionId="Spc_CheckList_Edit" Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click">
                        </cc1:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:gridview>
        <cc1:gridpager id="gp" runat="server" gridviewid="GV_List" pagesize="30">
        </cc1:gridpager>
    </div>
</fieldset>
<uc1:edit id="ucEdit" runat="server" visible="False" />
<uc1:new id="ucNew" runat="server" visible="False" />

