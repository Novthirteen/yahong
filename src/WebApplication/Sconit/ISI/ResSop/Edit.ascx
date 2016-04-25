<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Modules_ISI_ResSop_Edit" %>
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
        d.setAttribute("value", "ÒÆ³ý");
        div.appendChild(d);
        document.getElementById("_container").appendChild(div);
    }

    function deteFile(o) {
        while (o.tagName != "DIV") o = o.parentNode;
        o.parentNode.removeChild(o);
    }
</script>

<fieldset>
    <legend>${Common.Edit}</legend>
    <asp:FormView ID="FV_ResSop" runat="server" DataSourceID="ODS_ResSop" DefaultMode="Edit"
        DataKeyNames="Id" OnDataBound="FV_ResSop_DataBound">
        <EditItemTemplate>
            <table class="mtable">
                <tr>
                    <td class="td01">${ISI.ResSop.WorkShop}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbWorkShop" runat="server" Text='<%# Bind("WorkShop") %>' ReadOnly="true" />
                    </td>
                    <td class="td01">${ISI.ResSop.Operate}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbOperate" runat="server" Text='<%# Bind("Operate") %>' CssClass="inputRequired" />
                        <asp:RequiredFieldValidator ID="rfvOperate" runat="server" ControlToValidate="tbOperate"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                        <asp:RangeValidator ID="rvOperate" runat="server" ControlToValidate="tbOperate" ErrorMessage="${Common.Validator.Valid.Number}"
                            Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" />
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResSop.OperateDesc}:</td>
                    <td class="td02" colspan="3">
                        <asp:TextBox ID="tbOperateDesc" runat="server" Text='<%# Bind("OperateDesc") %>' Width="77%" Height="180"
                            TextMode="MultiLine" onkeypress="setMaxLength(this,500);" Font-Size="10" onpaste="limitPaste(this, 500)" />
                        <asp:RequiredFieldValidator ID="rfvOperateDesc" runat="server" ControlToValidate="tbOperateDesc"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResSop.CreateDate}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCreateDate" runat="server" Text='<%# Bind("CreateDate") %>' ReadOnly="true" />
                    </td>
                    <td class="td01">${ISI.ResSop.CreateUser}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbCreateUser" runat="server" Text='<%# Bind("CreateUser") %>' ReadOnly="true" />
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResSop.LastModifyDate}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbLastModifyDate" runat="server" Text='<%# Bind("LastModifyDate") %>' ReadOnly="true" />
                    </td>
                    <td class="td01">${ISI.ResSop.LastModifyUser}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbLastModifyUser" runat="server" Text='<%# Bind("LastModifyUser") %>' ReadOnly="true" />
                    </td>
                </tr>
            </table>

            <hr />
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
                        <asp:Label ID="strStatus" runat="server" Font-Names="ËÎÌå" Font-Bold="True" Font-Size="9pt"
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
                            <asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                CssClass="button2" ValidationGroup="vgSave" />
                            <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
                                CssClass="button2" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                            <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                CssClass="button2" />
                        </div>
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
    </asp:FormView>
    <asp:GridView ID="GV_List_Attachment" runat="server" OnRowDataBound="GV_List_RowDataBound" AllowSorting="false"
        AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="FileName" HeaderText="${ISI.TSK.Attachment.FileName}" />
            <asp:BoundField DataField="CreateUserNm" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Attachment.CreateUser}" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Attachment.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:BoundField DataField="Size" HeaderText="${ISI.TSK.Attachment.Size}"
                SortExpression="Size" DataFormatString="{0:0.##}" />
            <asp:TemplateField HeaderText="${Common.GridView.Action}">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnDownLoad" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                        OnClick="lbtnDownLoad_Click" Text="${Common.Business.ClickToDownload}" />&nbsp;
                    <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                        Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

</fieldset>
<asp:ObjectDataSource ID="ODS_ResSop" runat="server" TypeName="com.Sconit.Web.ResSopMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.ResSop" UpdateMethod="UpdateResSop"
    OnUpdated="ODS_ResSop_Updated" OnUpdating="ODS_ResSop_Updating" SelectMethod="LoadResSop"
    DeleteMethod="DeleteResSop" OnDeleted="ODS_ResSop_Deleted">
    <SelectParameters>
        <asp:Parameter Name="Id" Type="Int32" />
    </SelectParameters>
    <DeleteParameters>
        <asp:Parameter Name="Id" Type="Int32" />
    </DeleteParameters>
    <UpdateParameters>
        <asp:Parameter Name="Instruction" Type="Int32" ConvertEmptyStringToNull="true" />
    </UpdateParameters>
</asp:ObjectDataSource>
