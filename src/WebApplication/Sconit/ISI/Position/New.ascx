<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Modules_ISI_Position_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<asp:FormView ID="FV_Position" runat="server" DataSourceID="ODS_Position" DefaultMode="Insert"
        DataKeyNames="Position">
    <InsertItemTemplate>
    <fieldset>
        <legend>${Common.New}</legend>
		<table class="mtable">
			<tr>
					<td class="td01">${ISI.Position.Position}:</td>
					<td class="td02">						
						<asp:TextBox ID="tbPosition" runat="server" Text='<%# Bind("Position") %>' CssClass="inputRequired"/>
						<asp:RequiredFieldValidator ID="rfvPosition" runat="server" ControlToValidate="tbPosition"
								Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
						</td>
					<td class="td01">${ISI.Position.RoleType}:</td>
					<td class="td02">						
						<asp:TextBox ID="tbRoleType" runat="server" Text='<%# Bind("RoleType") %>' CssClass="inputRequired"/>
						<asp:RequiredFieldValidator ID="rfvRoleType" runat="server" ControlToValidate="tbRoleType"
								Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
						</td>
			</tr>
		</table>
	</fieldset>
    <div class="tablefooter">
        <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
            CssClass="button2" ValidationGroup="vgSave" />
        <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
            CssClass="button2" />
    </div>
    </InsertItemTemplate>
</asp:FormView>

<asp:ObjectDataSource ID="ODS_Position" runat="server" TypeName="com.Sconit.Web.PositionMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.Position" InsertMethod="CreatePosition"
    OnInserted="ODS_Position_Inserted" OnInserting="ODS_Position_Inserting" SelectMethod="LoadPosition">
    <SelectParameters>
        <asp:Parameter Name="Position" Type="String" />
    </SelectParameters>
    <InsertParameters>
    </InsertParameters>
</asp:ObjectDataSource>