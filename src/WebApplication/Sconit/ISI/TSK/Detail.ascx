<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Detail.ascx.cs" Inherits="ISI_TSK_Detail" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStatus" runat="server" Text="${Common.CodeMaster.Status}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlStatus" Code="ISIStatus" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblTaskCode" runat="server" Text="${ISI.TSK.Detail.TaskCode}:" Visible="false" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbTaskCode" runat="server" Visible="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblEmailStatus" runat="server" Text="${ISI.TSK.Detail.EmailStatus}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlEmailStatus" Code="ISISendStatus" runat="server"
                    IncludeBlankOption="true" DefaultSelectedValue="" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblSMSStatus" runat="server" Text="${ISI.TSK.Detail.SMSStatus}:" />
            </td>
            <td class="td02">
                <cc1:CodeMstrDropDownList ID="ddlSMSStatus" Code="ISISendStatus" runat="server" IncludeBlankOption="true"
                    DefaultSelectedValue="" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblEmail" runat="server" Text="${ISI.TSK.Detail.Email}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEmail" runat="server" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblMobilePhone" runat="server" Text="${ISI.TSK.Detail.MobilePhone}:" />
            </td>
            <td class="td02" colspan="3">
                <asp:TextBox ID="tbMobilePhone" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
            </td>
            <td class="td02">
            </td>
            <td class="td01">
            </td>
            <td class="t02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="query"
                        OnClick="btnSearch_Click" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                        OnClick="btnExport_Click" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="back" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fs" runat="server">
    <legend id="lgd" runat="server"></legend>
    <asp:GridView ID="GV_List_Detail" runat="server" OnDataBinding="GV_List_DataBound"
        OnRowDataBound="GV_List_RowDataBound" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="TaskCode" SortExpression="TaskCode" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Detail.TaskCode}" />
            <asp:BoundField DataField="TaskSubType" SortExpression="TaskSubType" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.TaskSubType}" />
            <asp:BoundField DataField="Level" SortExpression="Level" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Detail.Level}" />
            <asp:TemplateField HeaderText="${Common.CodeMaster.Status}" SortExpression="Status">
                <ItemTemplate>
                    <asp:Label ID="lblStatus" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="IsEmail" HeaderText="${ISI.TSK.Detail.IsEmail}" SortExpression="IsEmail" />
            <asp:TemplateField HeaderText="${ISI.TSK.Detail.Email}">
                <ItemTemplate>
                    <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="EmailCount" HeaderText="${ISI.TSK.Detail.EmailCount}"
                SortExpression="EmailCount" />
            <asp:TemplateField HeaderText="${ISI.TSK.Detail.EmailStatus}" SortExpression="EmailStatus">
                <ItemTemplate>
                    <cc1:CodeMstrLabel ID="lblEmailStatus" runat="server" Code="ISISendStatus" Value='<%# Bind("EmailStatus") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="IsSMS" HeaderText="${ISI.TSK.Detail.IsSMS}" SortExpression="IsSMS" />
            <asp:TemplateField HeaderText="${ISI.TSK.Detail.MobilePhone}">
                <ItemTemplate>
                    <asp:Label ID="lblMobilePhone" runat="server" Text='<%# Eval("MobilePhone")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="SMSCount" HeaderText="${ISI.TSK.Detail.SMSCount}" SortExpression="SMSCount" />
            <asp:TemplateField HeaderText="${ISI.TSK.Detail.SMSStatus}" SortExpression="SMSStatus">
                <ItemTemplate>
                    <cc1:CodeMstrLabel ID="lblSMSStatus" runat="server" Code="ISISendStatus" Value='<%# Bind("SMSStatus") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreateUserNm" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateUser}" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${Common.Business.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>
