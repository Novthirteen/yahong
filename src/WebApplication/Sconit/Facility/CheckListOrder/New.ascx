<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Facility_CheckListOrder_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="TextBox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.ui-zendesk.css" />
<script type="text/javascript" src="Js/ui.core-min.js"></script>
<script type="text/javascript" src="Js/tag-it.js"></script>
<style type="text/css">
    .CssColor {
        text-align: center;
        height: 18px;
        vertical-align: middle;
    }
    .button2 {
        height: 21px;
    }
</style>

<fieldset>
    <legend>${Common.New}</legend>
    <table class="mtable">
        <tr>
            <td class="td01">代码:</td>
            <td class="td02">
                <asp:DropDownList ID="tbCheckListCode" runat="server" DataTextField="Name" DataValueField="Code" AutoPostBack="true" OnSelectedIndexChanged="tbCheckList_TextChanged" />
                <asp:RequiredFieldValidator ID="rfvCheckListCode" runat="server" ControlToValidate="tbCheckListCode"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
            <td class="td01">巡检项目名称:</td>
            <td class="td02">
                <asp:Label runat="server" ID="lblCheckListDesc" />
            </td>
        </tr>
        <tr>
            <td class="td01">巡检人员:</td>
            <td class="td02">
                <uc3:TextBox ID="tbCheckUser" CssClass="inputRequired"
                    runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" MustMatch="true" />
                <asp:RequiredFieldValidator ID="rfvCheckUser" runat="server" ControlToValidate="tbCheckUser"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
            <td class="td01">巡检时间:</td>
            <td class="td02">
                <asp:TextBox ID="tbCheckDate" runat="server" CssClass="inputRequired" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" />
                <asp:RequiredFieldValidator ID="rfvCheckDate" runat="server" ControlToValidate="tbCheckDate" MustMatch="true"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
        </tr>
        <tr>
            <td class="td01">描述:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="lblDescription" runat="server" ReadOnly="true" TextMode="MultiLine" Height="100" Width="77%" />
            </td>

        </tr>
        <tr>
            <td class="td01">备注:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbRemark" runat="server" TextMode="MultiLine"
                    Width="77%" Height="100" />
            </td>

        </tr>
    </table>
</fieldset>
<div class="tablefooter">
    <asp:Button ID="btnInsert" runat="server" Text="${Common.Button.Save}" OnClick="btnInsert_Click"
        CssClass="button2" ValidationGroup="vgSave" />
    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
        CssClass="button2" />
</div>
<div id="divDetail" runat="server">
    <table class="mtable">
        <tr>
            <td colspan="4">
                <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
                    CellPadding="0" AllowSorting="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Seq">
                            <ItemTemplate>
                                <asp:Label ID="tbSeq" runat="server" Text='<%# Bind("Seq") %>' Width="10" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="项目明细">
                            <ItemTemplate>
                                <asp:Label ID="tbCheckListDetailCode" runat="server" Text='<%# Bind("CheckListDetailCode") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="范围">
                            <ItemTemplate>
                                <asp:Label ID="tbDetailDescription" runat="server" Text='<%# Bind("Description") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="判定">
                            <ItemTemplate>
                                <asp:RadioButton ID="rbNormal" runat="server" Text="正常" GroupName="IsNormal" Checked="true"/>
                                <asp:RadioButton ID="rbAbnormal" runat="server" Text="不正常" GroupName="IsNormal" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="结果">
                            <ItemTemplate>
                                <asp:TextBox ID="tbRemark" runat="server" Text='<%# Bind("Remark") %>' MaxLength="50" />
                                <asp:Label ID="lblIsRequired" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                       
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</div>
