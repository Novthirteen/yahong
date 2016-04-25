<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_CheckupProject_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc2" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_CheckupProject" runat="server" DataSourceID="ODS_CheckupProject"
        DefaultMode="Insert" Width="100%" DataKeyNames="Code">
        <InsertItemTemplate>
            <fieldset>
                <legend>${ISI.CheckupProject.NewCheckupProject}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCode" runat="server" Text="${Common.Business.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rtvCode" runat="server" ErrorMessage="${ISI.CheckupCode.Empty}"
                                Display="Dynamic" ControlToValidate="tbCode" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvInsert" runat="server" ControlToValidate="tbCode" ErrorMessage="${ISI.CheckupCode.Exist}"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="checkCheckupProject" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc" runat="server" Text="${Common.Business.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ErrorMessage="${ISI.CheckupDesc.Empty}"
                                Display="Dynamic" ControlToValidate="tbDesc" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlType" runat="server" Text="${Common.Business.Type}:" />
                        </td>
                        <td class="td02">
                            <cc2:CodeMstrDropDownList ID="ddlType" Code="ISICheckupProjectType" runat="server" IncludeBlankOption="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblIsActive" runat="server" Text="${Common.Business.IsActive}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="tbIsActive" runat="server" Checked='<%#Bind("IsActive") %>' />
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
<asp:ObjectDataSource ID="ODS_CheckupProject" runat="server" TypeName="com.Sconit.Web.CheckupProjectMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.CheckupProject" InsertMethod="CreateCheckupProject"
    OnInserted="ODS_CheckupProject_Inserted" OnInserting="ODS_CheckupProject_Inserting">
</asp:ObjectDataSource>
