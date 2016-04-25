<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Process.ascx.cs" Inherits="ISI_TSK_Process" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Assembly="Whidsoft.WebControls.OrgChart" Namespace="Whidsoft.WebControls"
    TagPrefix="oc" %>
<fieldset>
    <table class="mtable">        
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
            <td class="td01">
                <asp:Literal ID="lblViewType" runat="server" Text="${MasterData.BomView.ViewType}:" />
            </td>
            <td class="td02">
                <asp:RadioButtonList ID="rblViewType" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
                    <asp:ListItem Selected="True" Text="${ISI.TSK.Process.ViewType.Normal}" Value="Normal" />
                    <asp:ListItem Text="${ISI.TSK.Process.ViewType.Tree}" Value="Tree" />
                </asp:RadioButtonList>
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
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                        OnClick="btnExport_Click" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="back" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<script type="text/javascript" language="javascript">
    if (!$.browser.msie) {
        // alert(GetFrameWidth()-70);
        $(document).ready(function () {
            $("#scrollx").attr("width", GetFrameWidth() - 70);
        });

    }
</script>
<fieldset runat="server" id="dsTree">
    <div class="scrollx" style="width: expression((documentElement.clientWidth-70)+'px'); text-align: center"
        id="scrollx">
        <table>
            <tr>
                <td>
                    <oc:OrgChart ID="OrgChartTreeView" runat="server" ChartStyle="Vertical" Font-Size="X-Small"
                        LineColor="Silver"></oc:OrgChart>
                </td>
            </tr>
        </table>
    </div>
</fieldset>
<fieldset runat="server" id="dsNormal">
    <legend id="lgd" runat="server"></legend>
    <asp:GridView ID="GV_List_Process" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="Status" SortExpression="Status" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Process.Status}" />
            <asp:BoundField DataField="PreLevel" SortExpression="PreLevel" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Process.PreLevel}" />
            <asp:BoundField DataField="Level" SortExpression="Level" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Process.Level}" />
            <asp:TemplateField HeaderText="${ISI.TSK.Process.ProcessUser}" SortExpression="CreateUser">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "CreateUserNm")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Process.ProcessDate}" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
            <asp:TemplateField HeaderText="${ISI.TSK.Process.Content}" SortExpression="Content">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "Content")%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</fieldset>
