<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Uom.ascx.cs" Inherits="MasterData_Uom_Uom" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset id="fldSearch" runat="server">
    <table class="mtable">
        <tr>
            <td class="ttd01">
                <asp:Literal ID="ltlCode" runat="server" Text="${MasterData.Uom.Code}:" />
            </td>
            <td class="ttd02">
                <asp:TextBox ID="tbCode" runat="server"></asp:TextBox>
            </td>
            <td class="ttd01">
                <asp:Literal ID="ltlName" runat="server" Text="${MasterData.Uom.Name}:" />
            </td>
            <td class="ttd02">
                <asp:TextBox ID="tbName" runat="server"></asp:TextBox>
            </td>
            <td class="ttd01">
                <asp:Literal ID="ltlDescription" runat="server" Text="${MasterData.Uom.Description}:" />
            </td>
            <td class="ttd02">
                <asp:TextBox ID="tbDescription" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ErrorMessage="${MasterData.Uom.Description.Empty}"
                    Display="Dynamic" ControlToValidate="tbDescription" ValidationGroup="vgSave" />
            </td>
        </tr>
        <tr>
            <td colspan="5">
            </td>
            <td>
                <asp:Button ID="SearchBtn" runat="server" Text="${Common.Button.Search}" OnClick="btnSearch_Click" CssClass="button2" />
                <asp:Button ID="btnInsert" runat="server" Text="${Common.Button.New}" OnClick="btnInsert_Click" CssClass="button2"/>
                 <asp:Button ID="btnUom" runat="server" Text="${Uom.Conversion.Check}" OnClick="btnUom_Click" />
            </td>
        </tr>
    </table>
</fieldset>
<div id="floatdiv">
    <fieldset id="fldInsert" runat="server" visible="false">
        <legend>${MasterData.Uom.NewUom}</legend>
        <asp:FormView ID="FV_Uom" runat="server" DataSourceID="ODS_FV_Uom" DefaultMode="Insert">
            <InsertItemTemplate>
                <table class="mtable">
                    <tr>
                        <td class="ttd01">
                            <asp:Literal ID="ltlCode" runat="server" Text="${MasterData.Uom.Code}:" />
                        </td>
                        <td class="ttd02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' CssClass="inputRequired" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCode" runat="server" ErrorMessage="${MasterData.Uom.Code.Empty}"
                                Display="Dynamic" ControlToValidate="tbCode" ValidationGroup="vgSave" />
                        </td>
                        <td class="ttd01">
                            <asp:Literal ID="ltlName" runat="server" Text="${MasterData.Uom.Name}:" />
                        </td>
                        <td class="ttd02">
                            <asp:TextBox ID="tbName" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                        </td>
                        <td class="ttd01">
                            <asp:Literal ID="ltlDescription" runat="server" Text="${MasterData.Uom.Description}:" />
                        </td>
                        <td class="ttd02">
                            <asp:TextBox ID="tbDescription" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <div class="tablefooter">
                    <asp:Button ID="btnNew" runat="server" CommandName="Insert" Text="${Common.Button.Save}" CssClass="button2" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"  CssClass="button2"/>
                </div>
            </InsertItemTemplate>
        </asp:FormView>
    </fieldset>
</div>
<fieldset id="fldGV" runat="server" visible="false">
    <asp:GridView ID="GV_Uom" runat="server" AutoGenerateColumns="False" DataSourceID="ODS_GV_Uom"
        OnDataBound="GV_Uom_OnDataBind" DataKeyNames="Code" AllowPaging="True" PageSize="10"
        OnRowUpdating="GV_Uom_RowUpdating">
        <Columns>
            <asp:BoundField ReadOnly="true" DataField="Code" HeaderText="${MasterData.Uom.Code}" ItemStyle-Width="25%"/>
            <asp:BoundField DataField="Name" HeaderText="${MasterData.Uom.Name}" ItemStyle-Width="30%" />
            <asp:BoundField DataField="Description" HeaderText="${MasterData.Uom.Description}" ItemStyle-Width="30%"/>
            <asp:CommandField ShowEditButton="True" ShowDeleteButton="true" ItemStyle-Width="15%"
                HeaderText="${Common.GridView.Action}" DeleteText="&lt;span onclick=&quot;JavaScript:return confirm('${Common.Delete.Confirm}?')&quot;&gt;${Common.Button.Delete}&lt;/span&gt;">
            </asp:CommandField>
        </Columns>
    </asp:GridView>
    <asp:Label runat="server" ID="lblResult" Text="${Common.GridView.NoRecordFound}"
        Visible="false" />
</fieldset>

<fieldset id="fldList" runat="server" visible="false">
<legend>${Uom.Conversion.Check}</legend>
    <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
        CellPadding="0">
        <Columns>
            <asp:BoundField HeaderText="Type" DataField="Type" />
            <asp:BoundField HeaderText="Code" DataField="Code" />
            <asp:BoundField HeaderText="Description" DataField="Description" />
            <asp:BoundField HeaderText="SubType" DataField="SubType" />
            <asp:TemplateField HeaderText="ItemCode">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "Item.Code")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ItemDescription">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "Item.Description")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="BaseUom">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "Item.Uom.Code")%>
                    [<%# DataBinder.Eval(Container.DataItem, "Item.Uom.Name")%>]
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="AlterUom">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "AlterUom.Code")%>
                    [<%# DataBinder.Eval(Container.DataItem, "AlterUom.Name")%>]
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</fieldset>


<asp:ObjectDataSource ID="ODS_GV_Uom" runat="server" TypeName="com.Sconit.Web.UomMgrProxy"
    DataObjectTypeName="com.Sconit.Entity.MasterData.Uom" SelectMethod="LoadUom"
    UpdateMethod="UpdateUom" OnUpdating="ODS_GV_Uom_OnUpdating" DeleteMethod="DeleteUom"
    OnDeleting="ODS_GV_Uom_OnDeleting" OnDeleted="ODS_GV_Uom_OnDeleted">
    <SelectParameters>
        <asp:ControlParameter ControlID="tbCode" Name="Code" PropertyName="Text" Type="String" />
        <asp:ControlParameter ControlID="tbName" Name="Name" PropertyName="Text" Type="String" />
        <asp:ControlParameter ControlID="tbDescription" Name="Desc" PropertyName="Text" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>

<asp:ObjectDataSource ID="ODS_FV_Uom" runat="server" TypeName="com.Sconit.Web.UomMgrProxy"
    DataObjectTypeName="com.Sconit.Entity.MasterData.Uom" InsertMethod="CreateUom"
    OnInserted="ODS_Uom_Inserted" OnInserting="ODS_Uom_Inserting"></asp:ObjectDataSource>
