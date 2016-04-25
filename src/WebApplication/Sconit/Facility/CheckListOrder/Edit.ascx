<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Facility_CheckListOrder_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.css" />
<link rel="stylesheet" type="text/css" href="Js/tag-it/tagit.ui-zendesk.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<script language="javascript" type="text/javascript" src="Js/tag-it.js"></script>

<fieldset>
    <legend>${Common.Edit}</legend>
    <table class="mtable">
        <tr>
            <td class="td01">Ѳ�쵥��:</td>
            <td class="td02">
                <asp:Label ID="lblCode" runat="server" />
            </td>
            <td class="td01">����:</td>
            <td class="td02">
                <asp:Label ID="lblRegion" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">Ѳ����Ŀ����:</td>
            <td class="td02">
                <asp:Label ID="lblCheckListCode" runat="server" />
            </td>
            <td class="td01">Ѳ����Ŀ����:</td>
            <td class="td02">
                <asp:Label ID="lblCheckListName" runat="server" />
            </td>

        </tr>
        <tr>
            <td class="td01">Ѳ���豸��:</td>
            <td class="td02">
                <asp:Label ID="lblFacilityID" runat="server" />
            </td>
            <td class="td01">Ѳ���豸����:</td>
            <td class="td02">
                <asp:Label ID="lblFacilityName" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="td01">Ѳ����Ա:</td>
            <td class="td02">
                <uc3:textbox ID="tbCheckUser" CssClass="inputRequired"
                    runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" MustMatch="true" />
                <asp:RequiredFieldValidator ID="rfvCheckUser" runat="server" ControlToValidate="tbCheckUser"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
            <td class="td01">Ѳ��ʱ��:</td>
            <td class="td02">
                <asp:TextBox ID="tbCheckDate" runat="server" CssClass="inputRequired" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" />
                <asp:RequiredFieldValidator ID="rfvCheckDate" runat="server" ControlToValidate="tbCheckDate" MustMatch="true"
                    Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
            </td>
        </tr>
        <tr>
            <td class="td01">����:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="lblDescription" runat="server" ReadOnly="true" TextMode="MultiLine" Height="100" Width="77%" />
            </td>

        </tr>
        <tr>
            <td class="td01">��ע:</td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbRemark" runat="server" TextMode="MultiLine"
                    Width="77%" Height="100" />
            </td>

        </tr>
        <tr>
            <td colspan="4">
                <div class="tablefooter">
                     <cc1:Button ID="btnDelete" runat="server" Text="${Common.Button.Delete}"
                        CssClass="button2" OnClick="btnDelete_Click" FunctionId="Spc_CheckListOrder_Edit" />
                    <cc1:Button ID="btnUpdate" runat="server" Text="${Common.Button.Update}"
                        CssClass="button2" OnClick="btnUpdate_Click" FunctionId="Spc_CheckListOrder_Edit" />
                    <cc1:Button ID="btnClose" runat="server" Text="${Common.Button.Close}"
                        CssClass="button2" OnClick="btnClose_Click" FunctionId="Spc_CheckListOrder_Edit" />
                    <asp:Button ID="btnPrint" runat="server" Text="${Common.Button.Print}" OnClick="btnPrint_Click"
                        CssClass="button2" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" OnClick="btnExport_Click"
                        CssClass="button2" />
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
                                <asp:Label ID="tbSeq" runat="server" Text='<%# Bind("Seq") %>' Width="10" />
                                <asp:HiddenField ID="hfId" Value='<%# Bind("Id") %>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="��Ŀ��ϸ">
                            <ItemTemplate>
                                <asp:Label ID="tbCheckListDetailCode" runat="server" Text='<%# Bind("CheckListDetailCode") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="��Χ">
                            <ItemTemplate>
                                <asp:Label ID="tbDetailDescription" runat="server" Text='<%# Bind("Description") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="�ж�">
                            <ItemTemplate>
                                <asp:RadioButton ID="rbNormal" runat="server" Text="����" GroupName="IsNormal" Checked='<%# Bind("IsNormal") %>' />
                                <asp:RadioButton ID="rbAbnormal" runat="server" Text="������" GroupName="IsNormal" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="���">
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
</fieldset>


