<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProcessInstance.ascx.cs" Inherits="ISI_TSK_ProcessInstance" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<fieldset>
    <legend id="lgd" runat="server"></legend>
    <asp:GridView ID="GV_List_Detail" runat="server" OnDataBinding="GV_List_DataBound"
        OnRowDataBound="GV_List_RowDataBound" AutoGenerateColumns="false" DefaultSortDirection="Ascending"
        DefaultSortExpression="Level">
        <Columns>
            <asp:CheckBoxField DataField="IsOpt" HeaderText="${ISI.TSK.ProcessInstance.IsOpt}" SortExpression="IsOpt" />
            <asp:BoundField DataField="Desc1" SortExpression="Desc1" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.Description}" />
            <asp:BoundField DataField="TaskSubType" SortExpression="TaskSubType" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.TaskSubType}" />
            <asp:BoundField DataField="Status" SortExpression="Status" HeaderStyle-Wrap="false"
                HeaderText="${Common.CodeMaster.Status}" />
            <asp:BoundField DataField="Level" SortExpression="Level" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.Level}" />
            <asp:CheckBoxField DataField="IsParallel" HeaderText="${ISI.TSK.ProcessInstance.IsParallel}" SortExpression="IsParallel" />
            <asp:CheckBoxField DataField="ATicket" HeaderText="${ISI.TSK.ProcessInstance.ATicket}" SortExpression="ATicket" />
            <asp:BoundField DataField="UserNm" SortExpression="UserNm" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.UserNm}" />
            <asp:CheckBoxField DataField="IsCtrl" HeaderText="${ISI.TSK.ProcessInstance.IsCtrl}" SortExpression="IsCtrl" />
            <asp:BoundField DataField="UOM" SortExpression="UOM" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.UOM}" />
            <asp:BoundField DataField="UOMDesc" SortExpression="UOMDesc" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.UOMDesc}" />
            <asp:BoundField DataField="Qty" SortExpression="Qty" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.Qty}" DataFormatString="{0:0.########}" />
            <asp:BoundField DataField="CreateUserNm" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateUser}" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:BoundField DataField="ProcessUserNm" SortExpression="ProcessUserNm" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.ProcessUserNm}" />
            <asp:BoundField DataField="ProcessDate" SortExpression="ProcessDate" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.ProcessDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>
<fieldset>
    <legend>${ISI.TSK.Process}</legend>
    <asp:GridView ID="GV_List_Process" runat="server" OnRowDataBound="GV_List_Process_RowDataBound"
        AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="Status" SortExpression="Status" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Process.Status}" ItemStyle-Width="10%" />
            <asp:BoundField DataField="PreLevel" SortExpression="PreLevel" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Process.PreLevel}" ItemStyle-Width="10%"/>
            <asp:BoundField DataField="Level" SortExpression="Level" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Process.Level}" ItemStyle-Width="10%"/>
            <asp:TemplateField HeaderText="${ISI.TSK.Process.ProcessUser}" SortExpression="CreateUser" ItemStyle-Width="10%">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "CreateUserNm")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false" ItemStyle-Width="15%"
                HeaderText="${ISI.TSK.Process.ProcessDate}" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
            <asp:TemplateField HeaderText="${ISI.TSK.Process.Content}" SortExpression="Content" ItemStyle-Width="45%">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "Content")%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</fieldset>
<div class="tablefooter">
    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
        CssClass="back" />
</div>
