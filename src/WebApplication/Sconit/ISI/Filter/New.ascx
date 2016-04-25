<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_Filter_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_Filter" runat="server" DataSourceID="ODS_Filter" DefaultMode="Insert"
        Width="100%" DataKeyNames="Id" OnDataBinding="FV_Filter_OnDataBinding" OnDataBound="FV_Filter_DataBound">
        <InsertItemTemplate>
            <fieldset>
                <legend>${ISI.Filter.NewFilter}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblUserCode" runat="server" Text="${ISI.Filter.UserCode}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbUserCode" runat="server" Visible="true" DescField="LongName" MustMatch="true"
                                ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" />
                            <asp:TextBox ID="rtbUserCode" runat="server" Text='<%# Bind("UserCode") %>' ReadOnly="true" Visible="false" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlEmail" runat="server" Text="${ISI.Filter.Email}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbEmail" runat="server" Text='<%# Bind("Email") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblTaskType" runat="server" Text="${ISI.Filter.TaskType}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlTaskType" Code="ISIType" runat="server" IncludeBlankOption="true"
                                Width="120px" DefaultSelectedValue="" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.Filter.TaskSubType}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Desc" MustMatch="true"
                                ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetCacheAllTaskSubType"
                                Width="200" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblTaskCode" runat="server" Text="${ISI.Filter.TaskCode}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbTaskCode" runat="server" Text='<%# Bind("TaskCode") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc" runat="server" Text="${ISI.Filter.Desc}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' Height="80"
                                Width="80%" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,200);"
                                onpaste="limitPaste(this, 200)" Font-Size="10" CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbDesc" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02"></td>
                        <td class="td01"></td>
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
<asp:ObjectDataSource ID="ODS_Filter" runat="server" TypeName="com.Sconit.Web.FilterMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.Filter" InsertMethod="CreateFilter"
    OnInserted="ODS_Filter_Inserted" OnInserting="ODS_Filter_Inserting"></asp:ObjectDataSource>
