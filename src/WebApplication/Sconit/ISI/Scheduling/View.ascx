<%@ Control Language="C#" AutoEventWireup="true" CodeFile="View.ascx.cs" Inherits="ISI_Scheduling_View" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.Scheduling.TaskSubType}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                    Width="260" />
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
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
            </td>
            <td class="td02">
            </td>
            <td class="td01">
            </td>
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
<fieldset id="fs" runat="server" visible="false">
    <asp:GridView ID="GV_List_View" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="Date" HeaderText="${ISI.Scheduling.Date}" DataFormatString="{0:yyyy-MM-dd}" HeaderStyle-Width="8%" />
            <asp:BoundField DataField="DayOfWeek" HeaderText="${ISI.Scheduling.Week}" HeaderStyle-Width="8%" />
            <asp:TemplateField HeaderText="${ISI.Scheduling.TaskSubTypeCode}" HeaderStyle-Width="10%">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "TaskSubTypeCode")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.Scheduling.TaskSubTypeDesc}" HeaderStyle-Width="10%">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "TaskSubTypeDesc")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ShiftName" HeaderText="${ISI.Scheduling.ShiftName}" HeaderStyle-Width="6%"/>
            <asp:BoundField DataField="StartUser" HeaderText="${ISI.Scheduling.StartUser}" />
            <asp:BoundField DataField="StartTime" HeaderText="${Common.Business.StartTime}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Width="10%" />
            <asp:BoundField DataField="EndTime" HeaderText="${Common.Business.EndTime}" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderStyle-Width="10%" />
            <asp:BoundField DataField="WorkdayType" HeaderText="${ISI.Scheduling.WorkdayType}" HeaderStyle-Width="5%"/>
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>
