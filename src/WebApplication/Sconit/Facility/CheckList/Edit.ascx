<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_CheckList_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.ui-zendesk.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<script language="javascript" type="text/javascript" src="Js/tag-it.js"></script>


<script type="text/javascript" language="javascript">
    function qtyChanged(obj) {
        var minValue = $(obj).attr("title").split(",")[0];
        var maxValue = $(obj).attr("title").split(",")[1];
        var thisValue = $(obj).val();
        if (thisValue < minValue || thisValue > maxValue) {
            $(obj).css("background-color", "red");
        }
    }

    $(document).ready(function () {
        BindAssignStartUser();
    });

    function BindAssignStartUser() {
        Sys.Net.WebServiceProxy.invoke('Webservice/UserMgrWS.asmx', 'GetAllUser', false,
                                {},
                            function OnSucceeded(result, eventArgs) {
                                //alert("第" + times + "次追加数据.");
                                if (result != null) {
                                    var tags = result;
                                    $('#<%=tbSubUser.ClientID %>').tagit({
                                        availableTags: tags,
                                        allowNotDefinedTags: false,
                                        removeConfirmation: true
                                    });
                                }
                            },
            function OnFailed(error) {
                alert(error.get_message());
            }
        );
 }
</script>
<fieldset>
    <legend>${Common.Edit}</legend>
    <table class="mtable">
        <tr>
            <td class="td01">代码:</td>
            <td class="td02">
                <asp:Label ID="lblCode" runat="server" />
            </td>
            <td class="td01">名称:</td>
            <td class="td02">
                <asp:TextBox ID="tbName" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">测试设备号:</td>
            <td class="td02">
                <uc3:textbox ID="tbFacilityID" runat="server" Visible="true" Width="250" DescField="AssetNo"
                    ValueField="FCID" ServicePath="FacilityMasterMgr.service" ServiceMethod="GetAllFacilityMaster" />

            </td>
            <td class="td01">设施名称:</td>
            <td class="td02">
                <asp:TextBox ID="tbFacilityName" runat="server" />
            </td>

        </tr>
        <tr>
            <td class="td01">区域:</td>
            <td class="td02">
                <asp:TextBox ID="tbRegion" runat="server" CssClass="inputRequired" />
                <asp:RequiredFieldValidator ID="rfvRegion" runat="server" ControlToValidate="tbRegion"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
            <td class="td01"></td>
            <td class="td02"></td>
        </tr>
        <tr>
            <td class="td01">分类:</td>
            <td class="td02">
                <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType" />
            </td>
            <td class="td01">创建异常问题:</td>
            <td class="td02">
                <asp:CheckBox ID="cbNeedCreateTask" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="ltlSubUser" runat="server" Text="执行人:" />
            </td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbSubUser" runat="server" Width="100%" />
            </td>
        </tr>
        <tr>
            <td class="td01">描述:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbDescription" runat="server" TextMode="MultiLine"
                    Width="77%" Height="180" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div class="tablefooter">
                    <asp:Button ID="btnSave" runat="server" Text="${Common.Button.Save}" OnClick="btnSave_Click"
                        CssClass="button2" ValidationGroup="vgSave" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="button2" />
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
                    CellPadding="0" AllowSorting="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Seq">
                            <ItemTemplate>
                                <asp:HiddenField ID="hfSeq" runat="server" Value='<%# Bind("Seq") %>' />
                                <%#Container.DataItemIndex + 1%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="项目">
                            <ItemTemplate>
                                <asp:TextBox ID="tbDetailCode" runat="server" Text='<%# Bind("Code") %>' Width="100" MaxLength="20" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="范围描述">
                            <ItemTemplate>
                                <asp:TextBox ID="tbDetailDescription" runat="server" Text='<%# Bind("Description") %>' Width="400" MaxLength="100" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="最小值">
                            <ItemTemplate>
                                <asp:TextBox ID="tbMinValue" runat="server" Text='<%# Bind("MinValue") %>' Width="100" MaxLength="20" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="最大值">
                            <ItemTemplate>
                                <asp:TextBox ID="tbMaxValue" runat="server" Text='<%# Bind("MaxValue") %>' Width="100" MaxLength="20" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="必填">
                            <ItemTemplate>
                                <asp:CheckBox ID="tbIsRequired" runat="server" Checked='<%# Bind("IsRequired") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="${Common.GridView.Action}">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnAdd" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                    Text="${Common.Button.New}" OnClick="lbtnAdd_Click" ValidationGroup="vgAdd" TabIndex="1">
                                </asp:LinkButton>
                                <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                    Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')">
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</fieldset>


