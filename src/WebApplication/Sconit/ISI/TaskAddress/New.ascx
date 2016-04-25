<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_TaskAddress_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_TaskAddress" runat="server" DataSourceID="ODS_TaskAddress" DefaultMode="Insert"
        Width="100%" DataKeyNames="Code" OnDataBinding="FV_TaskAddress_OnDataBinding">
        <InsertItemTemplate>
            <fieldset>
                <legend>${ISI.TaskAddress.NewTaskAddress}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCode" runat="server" Text="${ISI.TaskAddress.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvCode" runat="server" ErrorMessage="${ISI.TaskAddress.Code.Empty}"
                                Display="Dynamic" ControlToValidate="tbCode" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvInsert" runat="server" ControlToValidate="tbCode" ErrorMessage="${ISI.TaskAddress.CodeExist}"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkTaskAddress" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblParent" runat="server" Text="${ISI.TaskAddress.Parent}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbParent" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="TaskAddressMgr.service" ServiceMethod="GetCacheAllTaskAddress"
                                Width="200" />
                        </td>
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
                                <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </InsertItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_TaskAddress" runat="server" TypeName="com.Sconit.Web.TaskAddressMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.TaskAddress" InsertMethod="CreateTaskAddress"
    OnInserted="ODS_TaskAddress_Inserted" OnInserting="ODS_TaskAddress_Inserting">
</asp:ObjectDataSource>
