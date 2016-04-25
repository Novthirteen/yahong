<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_Client_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc2" %>
<%@ Register Assembly="ASTreeView" Namespace="Geekees.Common.Controls" TagPrefix="ct" %>
<script type="text/javascript" language="javascript">
//<![CDATA[
    var flag1 = false;
    function hideTopFrame() {
        var winFrame = top.document.getElementsByName("winFrame")[0];
        var mainFrame = top.document.getElementsByName("mainframeset")[0];
        if (flag1) {
            winFrame.rows = "50,*";
            if (screen.width >= 1024) {
                mainFrame.cols = "200,9,*";
            } else if (screen.width > 800) {
                mainFrame.cols = "195,9,*";
            } else {
                mainFrame.cols = "190,9,*";
            }
        }
        else {
            winFrame.rows = "0,*";
            mainFrame.cols = "0,0,*";
        }
        flag1 = !flag1;
    }

    function ISIClient() {
        $("#tbISIRefresh").val('');
        $("#tbISIRows").val('');
        $("#clientDiv").slideToggle();
        $("#isiClientDiv").slideToggle();
        hideTopFrame();
        $('#<%= hfIsView.ClientID %>').val(flag1);
        $("#tbISIRows").val('');
    }

    //]]>
</script>
<div id="clientDiv">
    <fieldset>
        <table class="mtable">
            <tr>
                <td align="right">
                    <asp:Literal ID="ltlType" runat="server" Text="${ISI.Client.Type}:" />
                </td>
                <td>
                    <cc2:CodeMstrDropDownList ID="ddlType" Code="ISIType" runat="server" IncludeBlankOption="true"
                        Width="120px" DefaultSelectedValue="" />
                </td>
                <td align="right">
                    <asp:Literal ID="lblStatus" runat="server" Text="${Common.CodeMaster.Status}:" />
                </td>
                <td>
                    <ct:ASDropDownTreeView ID="astvMyTree" runat="server" BasePath="~/Js/astreeview/"
                        DataTableRootNodeValue="0" EnableRoot="false" EnableNodeSelection="false" EnableCheckbox="true"
                        EnableDragDrop="false" EnableTreeLines="false" EnableNodeIcon="false" EnableCustomizedNodeIcon="false"
                        EnableDebugMode="false" EnableRequiredValidator="false" InitialDropdownText=""
                        Width="170" EnableCloseOnOutsideClick="true" EnableHalfCheckedAsChecked="true"
                        DropdownIconDown="~/Js/astreeview/images/windropdown.gif" EnableContextMenuAdd="false"
                        MaxDropdownHeight="200" />
                </td>
                <td align="right">
                    <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
                </td>
                <td>
                    <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Literal ID="ltlIssueType" runat="server" Text="${ISI.Client.TaskSubType}:" />
                </td>
                <td>
                    <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                        ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                        Width="260" />
                </td>
                <td align="right">
                    <asp:Literal ID="lblTaskAddress" runat="server" Text="${ISI.Client.TaskAddress}:" />
                </td>
                <td>
                    <uc3:textbox ID="tbTaskAddress" runat="server" Visible="true" DescField="Code" MustMatch="false"
                        ValueField="Name" ServicePath="TaskAddressMgr.service" ServiceMethod="GetCacheAllTaskAddress"
                        Width="200" />
                </td>
                <td align="right">
                    <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
                </td>
                <td>
                    <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td align="right">
                    <asp:HiddenField ID="hfIsView" runat="server" OnValueChanged="hfIsView_OnValueChanged" />
                </td>
                <td>
                    <div class="buttons">
                        <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="query"
                            OnClick="btnSearch_Click" ValidationGroup="vgSearch" />
                        <asp:Button ID="btnHide" runat="server" Text="${ISI.Client.Hide}" CssClass="query"
                            OnClick="btnHide_Click" ValidationGroup="vgSearch" OnClientClick="ISIClient();" />
                        <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                            OnClick="btnExport_Click" />
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
</div>
<fieldset>
    <asp:UpdatePanel ID="UP_GV_List" ChildrenAsTriggers="true" UpdateMode="Always" RenderMode="Block"
        runat="server">
        <ContentTemplate>
            <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound" AutoGenerateColumns="false"
                RowStyle-CssClass="abc">
                <Columns>
                    <asp:BoundField DataField="SubmitDate" HeaderText="提交时间" HeaderStyle-Wrap="false"
                        ItemStyle-Wrap="false" ItemStyle-Width="9%" HeaderStyle-Width="9%" DataFormatString="{0:yyyy-MM-dd HH:mm}"/>
                    <asp:BoundField DataField="Code" HeaderText="任务编号" HeaderStyle-Wrap="false" ItemStyle-Wrap="false"
                        ItemStyle-Width="8%" HeaderStyle-Width="8%" />
                    <asp:BoundField DataField="TaskAddress" HeaderText="地点" HeaderStyle-Wrap="false"
                        ItemStyle-Wrap="false" ItemStyle-Width="5%" HeaderStyle-Width="5%" />
                    <asp:BoundField DataField="Subject" HeaderText="主题" HeaderStyle-Wrap="false" ItemStyle-Wrap="false"
                        ItemStyle-Width="20%" HeaderStyle-Width="20%" />
                    <asp:BoundField DataField="Desc1" HeaderText="描述" HeaderStyle-Wrap="false" ItemStyle-Wrap="false"
                        ItemStyle-Width="37%" HeaderStyle-Width="37%" />
                    <asp:BoundField HeaderText="执行人" HeaderStyle-Wrap="false" ItemStyle-Wrap="false"
                        ItemStyle-Width="18%" HeaderStyle-Width="18%" />
                    <asp:BoundField DataField="Flag" HeaderText="标志" HeaderStyle-Wrap="false" ItemStyle-Wrap="false"
                        ItemStyle-Width="3%" HeaderStyle-Width="3%" />
                </Columns>
            </asp:GridView>
            <asp:Literal ID="lblLoading" Visible="false" runat="server" Text="数据加载中..." />
            <asp:Timer ID="tmr" runat="server" Interval="60000" OnTick="Timer1_Tick" Enabled="false" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="tmr" EventName="Tick" />
            <asp:AsyncPostBackTrigger ControlID="btnHide" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</fieldset>
