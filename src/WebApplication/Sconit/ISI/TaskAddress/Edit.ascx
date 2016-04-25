<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="ISI_TaskAddress_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc2" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_TaskAddress" runat="server" DataSourceID="ODS_TaskAddress" DefaultMode="Edit"
        Width="100%" DataKeyNames="Code" OnDataBound="FV_TaskAddress_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${ISI.TaskAddress.UpdateTaskAddress}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCode" runat="server" Text="${ISI.TaskAddress.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblParent" runat="server" Text="${ISI.TaskAddress.Parent}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbParent" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="TaskAddressMgr.service" ServiceMethod="GetTaskAddressNotCode"
                                Width="200" ServiceParameter="string:#tbCode" />
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc" runat="server" Text="${Common.Business.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ErrorMessage="${ISI.TaskAddress.Description.Empty}"
                                Display="Dynamic" ControlToValidate="tbDesc" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSeq" runat="server" Text="${ISI.TaskAddress.Sequence}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSeq" runat="server" Text='<%# Bind("Seq") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvSeq" runat="server" ErrorMessage="${ISI.TaskAddress.Sequence.Empty}"
                                Display="Dynamic" ControlToValidate="tbSeq" ValidationGroup="vgSave" />
                            <asp:RangeValidator ID="rvSeq" runat="server" ControlToValidate="tbSeq" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                        </td>
                        <td class="td02">
                        </td>
                        <td class="td01">
                        </td>
                        <td class="td02">
                            <div class="buttons">
                                <asp:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" />
                                <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
                                    CssClass="delete" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
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
<asp:ObjectDataSource ID="ODS_TaskAddress" runat="server" TypeName="com.Sconit.Web.TaskAddressMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.TaskAddress" UpdateMethod="UpdateTaskAddress"
    OnUpdated="ODS_TaskAddress_Updated" SelectMethod="LoadTaskAddress" OnUpdating="ODS_TaskAddress_Updating"
    DeleteMethod="DeleteTaskAddress" OnDeleting="ODS_TaskAddress_Deleting" OnDeleted="ODS_TaskAddress_Deleted">
    <SelectParameters>
        <asp:Parameter Name="code" Type="String" />
    </SelectParameters>
    <DeleteParameters>
        <asp:Parameter Name="code" Type="String" />
    </DeleteParameters>
</asp:ObjectDataSource>
