<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_Approve_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });
    });
</script>
<fieldset>
    <div class="GridView">
        <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound" AutoGenerateColumns="false"
            RowStyle-CssClass="abc" OnDataBound="GV_List_DataBound">
            <Columns>
                <asp:BoundField DataField="Desc2" HeaderText="${ISI.Checkup.CheckupUserNm}" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" SortExpression="CheckupUserNm" HeaderStyle-Width="5%" HeaderStyle-Wrap="false" />
                <asp:BoundField DataField="JobNo" HeaderText="${ISI.Checkup.JobNo}" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" SortExpression="CheckupUserNm" HeaderStyle-Width="5%" HeaderStyle-Wrap="false" />
                <asp:TemplateField HeaderText="${ISI.Checkup.CheckupDate}" HeaderStyle-Width="6%" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <div>
                            <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'
                                Text='<%# DataBinder.Eval(Container.DataItem, "CheckupDate","{0:yyyy-MM-dd}") %>' OnClick="lbtnEdit_Click" />
                        </div>
                        <div>
                            <asp:LinkButton ID="lbtnSummary" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "SummaryCode")%>'
                                Text='<%# DataBinder.Eval(Container.DataItem, "SummaryCode") %>' OnClick="lbtnSummary_Click" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.CodeMaster.Status}" SortExpression="Status" HeaderStyle-Wrap="false"
                    HeaderStyle-Width="4%">
                    <ItemTemplate>
                        <asp:HiddenField ID="hfStatus" runat="server" Value='<%# Bind("Status") %>' />
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Checkup.CheckupProject}" HeaderStyle-Width="4%" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CheckupProject.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Checkup.Content}" HeaderStyle-Wrap="false" ItemStyle-VerticalAlign="Top" HeaderStyle-Width="25%" ItemStyle-Width="25%">
                    <ItemTemplate>
                        <asp:Label ID="lblContent" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Checkup.Amount}" HeaderStyle-Wrap="false" HeaderStyle-Width="8%" ItemStyle-Width="8%">
                    <ItemTemplate>
                        <asp:TextBox ID="tbAmount" runat="server" Text='' Width="100%" />
                        <asp:RangeValidator ID="rvAmount" runat="server" ErrorMessage="${Common.Validator.Valid.Number}"
                            Display="Dynamic" Type="Double" MaximumValue="10000" MinimumValue="-10000" ControlToValidate="tbAmount" ValidationGroup="vgApprove" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Checkup.AuditInstructions}" HeaderStyle-Wrap="false" ItemStyle-VerticalAlign="Top"
                    HeaderStyle-Width="25%" ItemStyle-Width="25%">
                    <ItemTemplate>
                        <asp:Label ID="lblAuditInstructions" runat="server" />
                        <asp:TextBox ID="tbAuditInstructions" Width="100%" runat="server" Text='<%# Eval("AuditInstructions") %>' onkeypress="javascript:setMaxLength(this,500);"
                            onpaste="limitPaste(this, 500)" Font-Size="10" Rows="2"
                            TextMode="MultiLine" />
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>
        <script language="javascript" type="text/javascript">
            $(document).ready(function () {
                $('.GV').fixedtableheader();
            });
        </script>
    </div>
</fieldset>
