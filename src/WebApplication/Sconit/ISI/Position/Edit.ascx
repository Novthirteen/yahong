<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="Modules_ISI_Position_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<asp:FormView ID="FV_Position" runat="server" DataSourceID="ODS_Position" DefaultMode="Edit"
        DataKeyNames="Position" OnDataBound="FV_Position_DataBound">
    <EditItemTemplate>
    <fieldset>
        <legend>${Common.Edit}</legend>
		<table class="mtable">
			<tr>
				<td class="td01">${ISI.Position.Position}:</td>
					<td class="td02">
							<asp:TextBox ID="tbPosition" runat="server" Text='<%# Bind("Position") %>' ReadOnly="true"/>
							</td>
				<td class="td01">${ISI.Position.RoleType}:</td>
					<td class="td02">
					<asp:TextBox ID="tbRoleType" runat="server" Text='<%# Bind("RoleType") %>' CssClass="inputRequired"/>
					<asp:RequiredFieldValidator ID="rfvRoleType" runat="server" ControlToValidate="tbRoleType"
							Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
							</td>
			</tr>
			<tr>
				<td class="td01">${ISI.Position.CreateDate}:</td>
					<td class="td02">
							<asp:TextBox ID="tbCreateDate" runat="server" Text='<%# Bind("CreateDate") %>' ReadOnly="true"/>
							</td>
				<td class="td01">${ISI.Position.CreateUser}:</td>
					<td class="td02">
							<asp:TextBox ID="tbCreateUser" runat="server" Text='<%# Bind("CreateUser") %>' ReadOnly="true"/>
							</td>
			</tr>
			<tr>
				<td class="td01">${ISI.Position.LastModifyDate}:</td>
					<td class="td02">
							<asp:TextBox ID="tbLastModifyDate" runat="server" Text='<%# Bind("LastModifyDate") %>' ReadOnly="true"/>
							</td>
				<td class="td01">${ISI.Position.LastModifyUser}:</td>
					<td class="td02">
							<asp:TextBox ID="tbLastModifyUser" runat="server" Text='<%# Bind("LastModifyUser") %>' ReadOnly="true"/>
							</td>
			</tr>
		</table>
	</fieldset>
    <div class="tablefooter">
        <asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="${Common.Button.Save}"
            CssClass="button2" ValidationGroup="vgSaveGroup" />
        <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
            CssClass="button2" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
        <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
            CssClass="button2" />
    </div>
    </EditItemTemplate>
</asp:FormView>
<asp:ObjectDataSource ID="ODS_Position" runat="server" TypeName="com.Sconit.Web.PositionMgrProxy"
DataObjectTypeName="com.Sconit.ISI.Entity.Position" UpdateMethod="UpdatePosition"
OnUpdated="ODS_Position_Updated" OnUpdating="ODS_Position_Updating" SelectMethod="LoadPosition"
DeleteMethod="DeletePosition" OnDeleted="ODS_Position_Deleted" >
	<SelectParameters>
		<asp:Parameter Name="Position" Type="String" />
	</SelectParameters>
    <DeleteParameters>
        <asp:Parameter Name="Position" Type="String" />
    </DeleteParameters>
    <UpdateParameters>
	</UpdateParameters>
</asp:ObjectDataSource>