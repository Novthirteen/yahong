<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_FailureMode_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_FailureMode" runat="server" DataSourceID="ODS_FailureMode" DefaultMode="Insert"
        Width="100%" DataKeyNames="Code" OnDataBinding="FV_FailureMode_OnDataBinding">
        <InsertItemTemplate>
            <fieldset>
                <legend>${ISI.FailureMode.NewFailureMode}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCode" runat="server" Text="${ISI.FailureMode.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvCode" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbCode" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvInsert" runat="server" ControlToValidate="tbCode" ErrorMessage="${ISI.FailureMode.CodeExist}"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkFailureMode" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.FailureMode.TaskSubType}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetCacheAllTaskSubType"
                                Width="260" CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvTaskSubType" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbTaskSubType" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc" runat="server" Text="${Common.Business.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbDesc" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSeq" runat="server" Text="${ISI.FailureMode.Sequence}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSeq" runat="server" Text='<%# Bind("Seq") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvSeq" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbSeq" ValidationGroup="vgSave" />
                            <asp:RangeValidator ID="rvSeq" runat="server" ControlToValidate="tbSeq" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsActive" runat="server" Text="${Common.IsActive}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' />
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
<asp:ObjectDataSource ID="ODS_FailureMode" runat="server" TypeName="com.Sconit.Web.FailureModeMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.FailureMode" InsertMethod="CreateFailureMode"
    OnInserted="ODS_FailureMode_Inserted" OnInserting="ODS_FailureMode_Inserting">
</asp:ObjectDataSource>
