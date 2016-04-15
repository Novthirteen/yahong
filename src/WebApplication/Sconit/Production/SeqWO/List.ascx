<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Production_SeqWO_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<script type="text/javascript" language="javascript">
    function CheckAll() {
        var GV_ListId = document.getElementById("<%=GV_List.ClientID %>");
        var allselect = GV_ListId.rows[0].cells[0].getElementsByTagName("INPUT")[0].checked;
        for (i = 1; i < GV_ListId.rows.length; i++) {
            GV_ListId.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = allselect;
        }
    }
</script>
<fieldset>
    <legend>${MasterData.Order.OrderHead.AvailableOrder}</legend>
    <div class="GridView">
        <cc1:Button ID="btnToFirstTop" runat="server" Text="${SeqWO.Button.ToFirst}" CssClass="button2"
         FunctionId="SeqWO" OnClick="ToFirst" />
        <cc1:Button ID="btnToEndTop" runat="server" Text="${SeqWO.Button.ToEnd}" CssClass="button2"
         FunctionId="SeqWO" OnClick="ToEnd" />
        <cc1:Button ID="btnPreviousTop" runat="server" Text="${SeqWO.Button.Previous}" CssClass="button2"
         FunctionId="SeqWO" OnClick="Previous"/>
        <cc1:Button ID="btnNextTop" runat="server" Text="${SeqWO.Button.Next}" CssClass="button2"
         FunctionId="SeqWO" OnClick="Next"/>
        <cc1:Button ID="btnSaveTop" runat="server" Text="${Common.Button.Save}" CssClass="button2"
            OnClick="btnSave_Click" FunctionId="SeqWO" />
        <asp:GridView ID="GV_List" runat="server" AllowSorting="True" AutoGenerateColumns="False"
            DataKeyNames="OrderNo" OnRowDataBound="GV_List_RowDataBound" >
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div onclick="CheckAll()">
                            <asp:CheckBox ID="CheckAll" runat="server" />
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="cbOrderNo" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Seq}">
                    <ItemTemplate>
                        <asp:TextBox ID="tbSequence" runat="server" onmouseup="if(!readOnly)select();" Text='<%# (Container.DataItemIndex + 1) * 10%>'
                            Width="50" TabIndex="1"></asp:TextBox>
                        <asp:HiddenField ID="hfIsChanged" runat="server" Value="N" />
                        <asp:RegularExpressionValidator ID="revSeq" runat="server" Display="Dynamic" ControlToValidate="tbSequence"
                            Enabled="false"></asp:RegularExpressionValidator>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="OrderNo" HeaderText="${MasterData.Order.OrderHead.OrderNo.Production}"/>
                
                <asp:TemplateField HeaderText="${MasterData.Order.OrderDetail.Item.Code}">
                        <ItemTemplate>
                            <asp:Label ID="lblItemCode" runat="server"  Width="100" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="${MasterData.Order.OrderDetail.Item.Description}">
                        <ItemTemplate>
                            <asp:Label ID="lblItemDescription" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.Priority}">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblPriority" runat="server" Code="OrderPriority" Value='<%# Bind("Priority") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${MasterData.Order.OrderHead.WindowTime}">
                    <ItemTemplate>
                        <asp:TextBox ID="tbWindowTime" runat="server" onmouseup="if(!readOnly)select();" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})"
                            Text='<%# Bind("WindowTime","{0:yyyy-MM-dd  HH:mm}") %>'  TabIndex="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="revWindowTime" runat="server" Display="Dynamic" ControlToValidate="tbWindowTime"
                            Enabled="false"></asp:RegularExpressionValidator>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</fieldset>
<div class="tablefooter">
        <cc1:Button ID="btnToFirstBotton" runat="server" Text="${SeqWO.Button.ToFirst}" CssClass="button2"
         FunctionId="SeqWO" OnClick="ToFirst" />
        <cc1:Button ID="btnToEndBotton" runat="server" Text="${SeqWO.Button.ToEnd}" CssClass="button2"
         FunctionId="SeqWO" OnClick="ToEnd" />
        <cc1:Button ID="btnPreviousBotton" runat="server" Text="${SeqWO.Button.Previous}" CssClass="button2"
         FunctionId="SeqWO" OnClick="Previous"/>
        <cc1:Button ID="btnNextBotton" runat="server" Text="${SeqWO.Button.Next}" CssClass="button2"
         FunctionId="SeqWO" OnClick="Next"/>
        <cc1:Button ID="btnSaveBotton" runat="server" Text="${Common.Button.Save}" CssClass="button2"
            OnClick="btnSave_Click" FunctionId="SeqWO" />
</div>
