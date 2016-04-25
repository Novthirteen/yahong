<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProcessInstance.ascx.cs" Inherits="ISI_TSK_ProcessInstance" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<fieldset>
    <legend id="lgd" runat="server"></legend>
    <asp:GridView ID="GV_List_Detail" runat="server" OnDataBinding="GV_List_DataBound"
        OnRowDataBound="GV_List_RowDataBound" AutoGenerateColumns="false" DefaultSortDirection="Ascending"
        DefaultSortExpression="Level">
        <Columns>
            <asp:CheckBoxField DataField="IsOpt" HeaderText="${ISI.TSK.ProcessInstance.IsOpt}" SortExpression="IsOpt" HeaderStyle-Wrap="false" />
            <asp:TemplateField HeaderText="${Common.Business.Description}" ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblDesc1" runat="server" Text='<%# Bind("Desc1") %>' />
                    <asp:TextBox ID="tbDesc1" Width="80" runat="server" Text='<%# Bind("Desc1") %>' Visible="false" />
                    <asp:RequiredFieldValidator ID="rfvDesc1" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                        Display="Dynamic" ControlToValidate="tbDesc1" Visible="false" ValidationGroup="vgSave" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TaskSubType" SortExpression="TaskSubType" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.TaskSubType}" />
            <asp:BoundField DataField="Status" SortExpression="Status" HeaderStyle-Wrap="false"
                HeaderText="${Common.CodeMaster.Status}" />
            <asp:BoundField DataField="Level" SortExpression="Level" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.Level}" />
            <asp:TemplateField HeaderText="${ISI.TSK.ProcessInstance.IsParallel}" ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:HiddenField runat="server" ID="hfLevel" Value='<%# Bind("Level") %>' />
                    <asp:HiddenField runat="server" ID="hfId" Value='<%# Bind("Id") %>' />
                    <asp:HiddenField runat="server" ID="hfUserNm" Value='<%# Bind("UserNm") %>' />
                    <asp:HiddenField runat="server" ID="hfUserCode" Value='<%# Bind("UserCode") %>' />
                    <asp:HiddenField runat="server" ID="hfStatus" Value='<%# Bind("Status") %>' />
                    <asp:CheckBox runat="server" Checked='<%# Bind("IsParallel") %>' Enabled="false" ID="cbIsParallel" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.ProcessInstance.ATicket}" ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:CheckBox runat="server" Checked='<%# Bind("ATicket") %>' Enabled="false" ID="cbATicket" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.ProcessInstance.IsRemind}" ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:CheckBox runat="server" Checked='<%# Bind("IsRemind") %>' Enabled="false" ID="cbIsRemind" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.ProcessInstance.UserNm}" ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("UserNm") %>' />
                    <uc3:textbox ID="tbUserCode" runat="server" DescField="Name" MustMatch="true" Visible="false"
                        ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" Width="260" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.ProcessInstance.IsCtrl}" ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:CheckBox runat="server" Checked='<%# Bind("IsCtrl") %>' Enabled="false" ID="cbIsCtrl" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.ProcessInstance.IsAccountCtrl}" ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:CheckBox runat="server" Checked='<%# Bind("IsAccountCtrl") %>' Enabled="false" ID="cbIsAccountCtrl" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="UOM" SortExpression="UOM" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.UOM}" />
            <asp:BoundField DataField="UOMDesc" SortExpression="UOMDesc" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.UOMDesc}" />
            <asp:TemplateField HeaderText="${ISI.TSK.ProcessInstance.Qty}" ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblQty" runat="server" Text='<%# Bind("Qty","{0:0.########}") %>' />
                    <asp:TextBox ID="tbQty" Width="50" runat="server" Text='<%# Bind("Qty","{0:0.########}") %>' Visible="false" />
                    <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="tbQty"
                        Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup="vgSave"
                        Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreateUserNm" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateUser}" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" Visible="false" />
            <asp:BoundField DataField="ProcessUserNm" SortExpression="ProcessUserNm" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.ProcessUserNm}" />
            <asp:BoundField DataField="ProcessDate" SortExpression="ProcessDate" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.ProcessInstance.ProcessDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:TemplateField HeaderText="${Common.GridView.Action}" HeaderStyle-Wrap="false" Visible="false">
                <ItemTemplate>
                    <div>
                        <asp:LinkButton ID="lbtnAdd" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Update}" OnClick="lbtnUpdate_Click"
                            ValidationGroup='vgSave' TabIndex="1">
                        </asp:LinkButton>
                    </div>
                    <div>
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')">
                        </asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
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
                HeaderText="${ISI.TSK.Process.PreLevel}" ItemStyle-Width="10%" />
            <asp:BoundField DataField="Level" SortExpression="Level" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Process.Level}" ItemStyle-Width="10%" />
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
