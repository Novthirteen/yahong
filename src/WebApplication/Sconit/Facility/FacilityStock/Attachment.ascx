<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Attachment.ascx.cs" Inherits="Facility_FacilityStock_Attachment" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<script type="text/javascript">
    function addFile() {
        var div = document.createElement("div");
        var f = document.createElement("input");
        f.setAttribute("type", "file");
        f.setAttribute("name", "file");
        f.setAttribute("id", "file");
        f.setAttribute("size", "50");
        f.setAttribute("ContentEditable", "false");

        div.appendChild(f);
        var d = document.createElement("input");
        d.setAttribute("type", "button");
        d.setAttribute("onclick", "deteFile(this)");
        d.setAttribute("value", "移除");
        div.appendChild(d);
        document.getElementById("_container").appendChild(div);
    }

    function deteFile(o) {
        while (o.tagName != "DIV") o = o.parentNode;
        o.parentNode.removeChild(o);
    }
</script>
<fieldset>
    <legend id="legUpload" runat="server">${Common.Button.Upload}</legend>
    <table class="mtable">
        <tr>
            <td class="td01"></td>
            <td class="td02" colspan="3">
                <div id="_container">
                    <asp:FileUpload ID="file" ContentEditable="false" size="50" runat="server" />
                </div>
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02" colspan="3">
                <asp:Label ID="strStatus" runat="server" Font-Names="宋体" Font-Bold="True" Font-Size="9pt"
                    Width="500px" BorderStyle="None" BorderColor="White"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="t02">
                <div class="buttons">
                    <input type="button" value="${ISI.Attachment.Multifile}" onclick="addFile()" />
                    <asp:Button runat="server" Text="${Common.Button.Upload}" ID="UploadButton" OnClick="UploadButton_Click"></asp:Button>
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                        OnClick="btnExport_Click" Visible="false" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="back" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset>
    <legend id="lgd" runat="server"></legend>
    <asp:GridView ID="GV_List_Attachment" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="CreateUserNm" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Attachment.CreateUser}" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Attachment.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:BoundField DataField="Size" HeaderText="${ISI.TSK.Attachment.Size}"
                    SortExpression="Size" DataFormatString="{0:0.##}" />
            <asp:TemplateField HeaderText="${Common.GridView.Action}">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnDownLoad" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                        OnClick="lbtnDownLoad_Click" />&nbsp;
                    <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                        Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</fieldset>
<fieldset id="isProject" runat="server" visible="False">
    <legend>${ISI.TSK.ProjectTask.Attachment}</legend>
    <asp:GridView ID="GV_ProjectTask" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="CreateUserNm" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Attachment.CreateUser}" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Attachment.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:TemplateField HeaderText="${Common.GridView.Action}">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnDownLoad" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                        OnClick="lbtnDownLoad_Click" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</fieldset>
