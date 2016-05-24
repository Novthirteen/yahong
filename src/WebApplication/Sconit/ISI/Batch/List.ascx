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

    function GVCheckClick() {
        if ($(".GVHeader input:checkbox").attr("checked") == 'checked') {
            $(".GVRow input:checkbox").attr("checked", true);
            $(".GVAlternatingRow input:checkbox").attr("checked", true);
        }
        else {
            $(".GVRow input:checkbox").attr("checked", false);
            $(".GVAlternatingRow input:checkbox").attr("checked", false);
        }
    }

</script>
<fieldset>
    <div class="GridView">
        <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound" AutoGenerateColumns="false"
            RowStyle-CssClass="abc" OnDataBound="GV_List_DataBound">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div onclick="GVCheckClick()">
                            <asp:CheckBox ID="CheckAll" runat="server" />
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:HiddenField ID="hfId" runat="server" Value='<%# Bind("TaskCode") %>' />
                        <asp:HiddenField ID="hfStatus" runat="server" Value='<%# Bind("Status") %>' />
                        <asp:CheckBox ID="CheckBoxGroup" name="CheckBoxGroup" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.Subject}" HeaderStyle-Wrap="false" SortExpression="CreateDate">
                    <ItemTemplate>
                        <div>
                            <asp:Label ID="lblTaskCode" Width="90" runat="server" Text='<%#Eval("TaskCode")%>' />
                        </div>
                        <div>
                            <asp:Label ID="lblSubject" Width="90" runat="server" Text='<%#Eval("Subject")%>' />
                        </div>
                        <div>
                            <asp:Label ID="lblCreateDate" runat="server" Text='<%#Eval("CreateDate","{0:yyyy-MM-dd HH}")%>' />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.Business.Description}" HeaderStyle-Width="22%" ItemStyle-Width="22%" HeaderStyle-Wrap="false" SortExpression="RefTaskCount">
                    <ItemTemplate>
                        <div>
                            <asp:Label ID="lblDesc" runat="server" Text='' />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.TSK.ExpectedResults}" HeaderStyle-Wrap="false" SortExpression="AttachmentCount">
                    <ItemTemplate>
                        <div>
                            <asp:Label ID="lblExpectedResults" runat="server" Text='' />
                        </div>
                        <div>
                            <asp:Label ID="lblEndDate" runat="server" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.SubmitUser}" HeaderStyle-Wrap="false" SortExpression="SubmitDate" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <div>
                            <asp:Label ID="lblSubmitUserNm" runat="server" Text='<%# Eval("SubmitUserNm")%>' />
                        </div>
                        <div>
                            <asp:Label ID="lblTaskAddress" runat="server" Text='<%#Eval("TaskAddress")%>' />
                        </div>
                        <div>
                            <asp:Label ID="lblTaskSubTypeCode" runat="server" Text='' />
                        </div>
                        <div><span style="color: #0000E5;"><%#Eval("Seq")%></span></div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.AssignUser}" HeaderStyle-Wrap="false" SortExpression="AssignDate" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <div>
                            <asp:Label ID="lblAssignUserNm" runat="server" Text='<%# Eval("AssignUserNm")%>' />
                        </div>
                        <div>
                            <asp:Label ID="lblStatus" runat="server" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.StartUser}" HeaderStyle-Wrap="false" SortExpression="StartDate" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblStartedUser" runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.StatusDesc}" HeaderStyle-Width="22%" ItemStyle-Width="22%" HeaderStyle-Wrap="false" SortExpression="StatusDate">
                    <ItemTemplate>
                        <asp:Label ID="lblStatusDesc" runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.CodeMaster.Flag}" HeaderStyle-Wrap="false" SortExpression="TColor" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblFlag" runat="server" Text='<%# Bind("Flag") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.Status.Comment}" HeaderStyle-Wrap="false" SortExpression="CommentCreateDate" HeaderStyle-Width="15%" ItemStyle-Width="15%">
                    <ItemTemplate>
                        <asp:Label ID="lblComment" runat="server" Text='' />
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
