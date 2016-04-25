<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_ProjectTask_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" DefaultSortDirection="Descending"
            DefaultSortExpression="CreateDate" OnDataBound="GV_List_DataBound">
            <Columns>
                <asp:TemplateField HeaderText="${ISI.ProjectTask.ProjectType}" HeaderStyle-Width="10%" >
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblProjectType" runat="server" Code="PSIType" Value='<%# Bind("ProjectType") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.ProjectTask.ProjectSubType}" HeaderStyle-Width="5%" >
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblProjectSubType" runat="server" Code="ISIProjectSubType" Value='<%# Bind("ProjectSubType") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.ProjectTask.Phase}" HeaderStyle-Width="5%" >
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblPhase" runat="server" Code="ISIPhase" Value='<%# Bind("Phase") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.ProjectTask.Seq}" HeaderStyle-Width="5%" >
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Seq")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.ProjectTask.Subject}">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Subject")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}" HeaderStyle-Width="8%">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.View}" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')"
                           />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="100">
        </cc1:GridPager>
    </div>
</fieldset>
