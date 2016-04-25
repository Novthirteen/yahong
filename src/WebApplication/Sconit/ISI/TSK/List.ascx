<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_TSK_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" DefaultSortDirection="Descending"
            DefaultSortExpression="CreateDate" OnDataBound="GV_List_DataBound">
            <Columns>
                <asp:TemplateField HeaderText="${ISI.TSK.Code}" ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TSK.Subject}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Subject")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TSK.Priority}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblPriority" runat="server" Code="ISIPriority" Value='<%# Bind("Priority") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TSK.TaskSubType}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "TaskSubType.Desc")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TSK.Phase}" Visible="False">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblPhase" runat="server" Code="ISIPhase" Value='<%# Bind("Phase") %>' />
                        <%# DataBinder.Eval(Container.DataItem, "Seq")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TSK.TaskAddress}" Visible="False" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "TaskAddress")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.CodeMaster.Status}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.CreateUser}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CreateUserNm")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TSK.StartedUser}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblStartedUser" runat="server" Text='<%# Eval("StartedUser")  %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TSK.Flag}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblFlag" runat="server" Code="ISIFlag" Value='<%# Bind("Flag") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.View}" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')"
                            Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
