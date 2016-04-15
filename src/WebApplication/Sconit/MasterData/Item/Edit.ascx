<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="MasterData_Item_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_Item" runat="server" DataSourceID="ODS_Item" DefaultMode="Edit"
        Width="100%" DataKeyNames="Code" OnDataBound="FV_Item_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${MasterData.Item.UpdateItem}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td02" rowspan="6" style="width: 150px">
                            <asp:Image ID="imgUpload" ImageUrl='<%# Eval("ImageUrl") %>' runat="server" Width="150px" />
                        </td>
                        <td class="td01" style="width: 100px">
                            <asp:Literal ID="lblCode" runat="server" Text="${MasterData.Item.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:Literal ID="tbCode" runat="server" Text='<%# Bind("Code") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlItemImage" runat="server" Text="${MasterData.Item.Image}:" />
                        </td>
                        <td class="td02">
                            <asp:FileUpload ID="fileUpload" ContentEditable="false" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01" style="width: 100px">
                            <asp:Literal ID="ltlDesc1" runat="server" Text="${MasterData.Item.Description}1:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDesc1" runat="server" Text='<%# Bind("Desc1") %>' CssClass="inputRequired"
                                Width="200" />
                            <asp:RequiredFieldValidator ID="rfvDesc1" runat="server" ErrorMessage="${MasterData.Item.Desc1.Empty}"
                                Display="Dynamic" ControlToValidate="tbDesc1" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc2" runat="server" Text="${MasterData.Item.Description}2:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDesc2" runat="server" Text='<%# Bind("Desc2") %>' Width="200" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01" style="width: 100px">
                            <asp:Literal ID="ltlUom" runat="server" Text="${MasterData.Item.Uom}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbUom" runat="server" ReadOnly="true" />
                            <%--                            <uc3:textbox ID="tbUom" runat="server" Visible="true" DescField="Description" CssClass="inputRequired"
                                ValueField="Code" ServicePath="UomMgr.service" ServiceMethod="GetAllUom" MustMatch="true" />
                            <asp:RequiredFieldValidator ID="rfvtbUom" runat="server" ErrorMessage="${MasterData.Item.Uom.Empty}"
                                Display="Dynamic" ControlToValidate="tbUom" ValidationGroup="vgSave" />--%>
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlType" runat="server" Text="${MasterData.Item.Type}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlType" Code="ItemType" runat="server" Enabled="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01" style="width: 100px">
                            <asp:Literal ID="ltlCount" runat="server" Text="${MasterData.Item.Uc}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCount" runat="server" Text='<%# Bind("UnitCount","{0:0.########}") %>'
                                CssClass="inputRequired" />
                            <asp:RegularExpressionValidator ID="revCount" ControlToValidate="tbCount" runat="server"
                                ValidationGroup="vgSave" ErrorMessage="${MasterData.Item.UC.Format}" ValidationExpression="^[0-9]+(.[0-9]{1,8})?$"
                                Display="Dynamic" />
                            <asp:RequiredFieldValidator ID="rfvUC" runat="server" ErrorMessage="${MasterData.Item.UC.Empty}"
                                Display="Dynamic" ControlToValidate="tbCount" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlLocation" runat="server" Text="${MasterData.Item.Location}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbLocation" runat="server" Visible="true" DescField="Name" Width="250"
                                ValueField="Code" ServicePath="LocationMgr.service" ServiceMethod="GetAllLocation"
                                MustMatch="true" />
                        </td>
                    </tr>
                    <%--<tr>
                        <td class="td01" style="width: 100px">
                            <asp:Literal ID="ltlBom" runat="server" Text="${MasterData.Item.Bom}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbBom" runat="server" Visible="true" DescField="Description" ValueField="Code"
                                ServicePath="BomMgr.service" ServiceMethod="GetAllBom" MustMatch="true" Width="250" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlRouting" runat="server" Text="${MasterData.Item.Routing}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbRouting" runat="server" Visible="true" DescField="Description"
                                ValueField="Code" ServicePath="RoutingMgr.service" ServiceMethod="GetAllRouting"
                                MustMatch="true" />
                        </td>
                    </tr>--%>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCategory1" runat="server" Text="${MasterData.Item.Category1}:" />
                        </td>
                        <td class="td02">
                            <asp:DropDownList ID="ddlCategory1" runat="server" DataTextField="FullName" DataValueField="Code"
                                Width="200" DataSourceID="ODS_ddlCategory1">
                            </asp:DropDownList>
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblCategory2" runat="server" Text="${MasterData.Item.Category2}:" />
                        </td>
                        <td class="td02">
                            <asp:DropDownList ID="ddlCategory2" runat="server" DataTextField="FullName" DataValueField="Code"
                                Width="200" DataSourceID="ODS_ddlCategory2">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="td01" style="width: 100px">
                            <asp:Literal ID="lblParty" Text="${Reports.BillAging.PartyFrom}:" runat="server" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbParty" runat="server" Visible="true" Width="250" DescField="Name"
                                ValueField="Code" ServicePath="SupplierMgr.service" ServiceMethod="GetSupplier" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblItemCategory" Text="${MasterData.Item.ItemCategory}:" runat="server" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbItemCategory" runat="server" Visible="true" Width="250" DescField="Description"
                                ValueField="Code" ServicePath="ItemCategoryMgr.service" ServiceMethod="GetCacheAllItemCategory"
                                CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvItemCategory" runat="server" ErrorMessage="${MasterData.Item.Category.Empty}"
                                Display="Dynamic" ControlToValidate="tbItemCategory" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01" style="width: 150px; text-align: center">
                            <asp:Literal ID="ltlDeleteImage" runat="server" Text="${MasterData.Item.DeleteImage}:" />
                            <asp:CheckBox ID="cbDeleteImage" runat="server" />
                        </td>
                        <td class="td01" style="width: 100px">
                            <asp:Literal ID="ltlMemo" runat="server" Text="${MasterData.Item.Memo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbMemo" runat="server" Text='<%# Bind("Memo") %>'></asp:TextBox>
                        </td>
                        <td class="td01">
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="tbIsActive" runat="server" Checked='<%#Bind("IsActive") %>' Text="${MasterData.Item.IsActive}" />
                            <asp:CheckBox ID="cbIsFreeze" runat="server" Checked='<%#Bind("IsFreeze") %>' Text="${MasterData.Item.IsFreeze}" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01" style="width: 150px">
                            &nbsp;
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlMin" runat="server" Text="安全库存:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbMin" runat="server" Text='<%# Bind("NumField1","{0:0.########}")%>' />
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="tbMin"
                                runat="server" ValidationGroup="vgSave" ErrorMessage="${MasterData.Item.UC.Format}"
                                ValidationExpression="^[0-9]+(.[0-9]{1,8})?$" Display="Dynamic" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlMax" runat="server" Text="最大库存:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbMax" runat="server" Text='<%# Bind("NumField2","{0:0.########}")%>' />
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="tbMax"
                                runat="server" ValidationGroup="vgSave" ErrorMessage="${MasterData.Item.UC.Format}"
                                ValidationExpression="^[0-9]+(.[0-9]{1,8})?$" Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01" style="width: 150px">
                            &nbsp;
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlContainer" runat="server" Text="${MasterData.Item.Container}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbContainer" runat="server" Text='<%# Bind("Container")%>' />
                        </td>
                        <td class="td01">
                        </td>
                        <td class="td02">
                            <div class="buttons">
                                <cc1:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" FunctionId="Page_EditItem" />
                                <cc1:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
                                    CssClass="delete" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')"
                                    FunctionId="Page_EditItem" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_Item" runat="server" TypeName="com.Sconit.Web.ItemMgrProxy"
    DataObjectTypeName="com.Sconit.Entity.MasterData.Item" UpdateMethod="UpdateItem"
    OnUpdated="ODS_Item_Updated" SelectMethod="LoadItem" OnUpdating="ODS_Item_Updating"
    DeleteMethod="DeleteItem" OnDeleted="ODS_Item_Deleted" OnDeleting="ODS_Item_Deleting">
    <SelectParameters>
        <asp:Parameter Name="code" Type="String" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Name="NumField1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="NumField2" Type="Decimal" ConvertEmptyStringToNull="true" />
    </UpdateParameters>
</asp:ObjectDataSource>
<asp:ObjectDataSource ID="ODS_ddlCategory1" runat="server" TypeName="com.Sconit.Web.ItemTypeMgrProxy"
    DataObjectTypeName="com.Sconit.Entity.MasterData.ItemType" SelectMethod="GetItemTypeIncludeEmpty">
    <SelectParameters>
        <asp:QueryStringParameter Name="level" DefaultValue="1" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
<asp:ObjectDataSource ID="ODS_ddlCategory2" runat="server" TypeName="com.Sconit.Web.ItemTypeMgrProxy"
    DataObjectTypeName="com.Sconit.Entity.MasterData.ItemType" SelectMethod="GetItemTypeIncludeEmpty">
    <SelectParameters>
        <asp:QueryStringParameter Name="level" DefaultValue="2" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>