<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_Public_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="TaskCode"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="false" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GV_List_RowDataBound" OnDataBound="GV_List_DataBound"
            DefaultSortDirection="Descending" DefaultSortExpression="CreateDate">
            <Columns>
                <asp:TemplateField HeaderText="${ISI.Status.Subject}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <div><asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskCode") %>'
                            Text='<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>' OnClick="lbtnEdit_Click" /></div>
                        <div><asp:Label ID="lblSubject" Width="90" runat="server" Text='' /></div>
                        <div><asp:Label ID="lblCreateDate" runat="server" Text='<%#Eval("CreateDate","{0:yyyy-MM-dd HH}")%>' /></div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Description}" HeaderStyle-Width="22%" ItemStyle-Width="22%"  HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblDesc" runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TSK.ExpectedResults}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblExpectedResults" runat="server" Text='' />
                        <asp:LinkButton ID="lbtnDownLoad" runat="server" CommandArgument='<%#  "{"+DataBinder.Eval(Container.DataItem, "FileName") + "}{"+DataBinder.Eval(Container.DataItem, "ContentType") + "}{"+DataBinder.Eval(Container.DataItem, "Path")+"}" %>'
                            OnClick="lbtnDownLoad_Click" Visible="False" /><br>
                        <asp:Label ID="lblEndDate" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.SubmitUser}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblSubmitUserNm" runat="server" Text='<%# Eval("SubmitUserNm")+ "<br>"+ Eval("TaskAddress") +"<br>[" + Eval("TaskSubTypeCode") + "]<br><span style=\"color:#0000E5;\">" + Eval("Seq") + "</span>" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.AssignUser}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblAssignUserNm" runat="server" Text='<%# Eval("AssignUserNm")+"<br>" %>' />
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.StartUser}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblStartedUser" runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.StatusDesc}" HeaderStyle-Width="22%" ItemStyle-Width="22%"  HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblStatusDesc" runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.CodeMaster.Flag}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <cc1:CodeMstrLabel ID="lblFlag" runat="server" Code="ISIFlag" Value='<%# Bind("Flag") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.Comment}" HeaderStyle-Width="15%" ItemStyle-Width="15%" HeaderStyle-Wrap="false" SortExpression="CommentCreateDate">
                    <ItemTemplate>
                        <asp:Label ID="lblComment" runat="server" Text='' />
                        <div>
                            <a onclick="javascript:ShowComment('<%# DataBinder.Eval(Container.DataItem, "TaskCode")%>',<%# Container.DisplayIndex %>);" id="lnkComment" name="lnkComment" href="#">${ISI.Status.Comment}</a>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
        <script language="javascript" type="text/javascript">
            $(document).ready(function () {
                $('.GV').fixedtableheader();
            });
        </script>
    </div>
</fieldset>
