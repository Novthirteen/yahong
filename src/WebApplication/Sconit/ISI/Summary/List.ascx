<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_Summary_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset runat="server" visible="false" id="fds">
    <legend>${ISI.Summary.NoSubmit}</legend>
    <div class="GridView">
        <asp:GridView ID="GV_Evaluation" runat="server" AutoGenerateColumns="false" Visible="false">
            <Columns>
                <asp:TemplateField HeaderText="Seq.">
                    <ItemTemplate>
                        <%#Container.DataItemIndex + 1%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="UserCode" SortExpression="UserCode" HeaderStyle-Wrap="false"
                    HeaderText="${ISI.Evaluation.UserCode}" />
                <asp:BoundField DataField="UserName" SortExpression="UserName" HeaderStyle-Wrap="false"
                    HeaderText="${ISI.Evaluation.UserName}" />
                <asp:BoundField DataField="StandardQty" SortExpression="StandardQty" HeaderStyle-Wrap="false"
                    HeaderText="${ISI.Evaluation.StandardQty}" />
                <asp:CheckBoxField DataField="IsCheckup" SortExpression="IsCheckup" HeaderStyle-Wrap="false"
                    HeaderText="${ISI.Evaluation.IsCheckup}" />
                <asp:BoundField DataField="Amount" SortExpression="Amount" HeaderStyle-Wrap="false" DataFormatString="{0:0.########}"
                    HeaderText="${ISI.Evaluation.Amount}" />
            </Columns>
        </asp:GridView>
        <asp:Label ID="lblMessage" runat="server" ForeColor="Blue" />
    </div>
</fieldset>
<fieldset>
    <legend>${ISI.Summary.List}</legend>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" OnDataBound="GV_List_DataBound" DefaultSortDirection="Descending" DefaultSortExpression="CreateDate">
            <Columns>
                <asp:BoundField DataField="Code" HeaderText="${Common.Business.Code}" SortExpression="Code" />
                <asp:BoundField DataField="Desc" HeaderText="${Common.Business.Description}" SortExpression="Desc" />
                <asp:TemplateField HeaderText="${Common.CodeMaster.Status}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Date" HeaderText="${ISI.Summary.Date}"
                    SortExpression="Date" DataFormatString="{0:yyyy-MM}" />
                <asp:BoundField DataField="CreateDate" HeaderText="${Common.Business.CreateDate}"
                    SortExpression="CreateDate" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="CreateUserNm" HeaderText="${Common.Business.CreateUser}"
                    SortExpression="CreateUser" />
                <asp:BoundField DataField="Count" HeaderText="${ISI.Summary.Count}" SortExpression="Count" />
                <asp:BoundField DataField="StandardQty" HeaderText="${ISI.Summary.StandardQty}" SortExpression="StandardQty" />
                <asp:BoundField DataField="Qty" HeaderText="${ISI.Summary.Qty}" SortExpression="Qty" />
                <asp:BoundField DataField="ApproveDesc" HeaderText="${ISI.Summary.ApproveDesc}" SortExpression="ApproveDesc" />
                <asp:BoundField DataField="UltimatelyAmount" HeaderText="${ISI.Summary.UltimatelyAmount}" SortExpression="UltimatelyAmount" DataFormatString="{0:0.########}" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="lbtnCancel" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Cancel}" OnClick="lbtnCancel_Click" Visible="false" OnClientClick="return confirm('${Common.Button.Cancel.Confirm}')" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="100">
        </cc1:GridPager>
    </div>

</fieldset>
