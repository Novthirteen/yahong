﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_ResInfo_Responsibility_Main" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblDirector" runat="server" Text="${ISI.Responsibility.Director}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbDirector" runat="server" Visible="true" DescField="Name" MustMatch="true" CssClass="inputRequired"
                    ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" Width="250" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblHistoryDate" runat="server" Text="${ISI.Responsibility.HistoryDate}:" /></td>
            <td class="td02">
                <asp:TextBox ID="tbHistoryDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="t02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="query"
                        OnClick="btnSearch_Click" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                        OnClick="btnSearch_Click" Visible="false" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fs" runat="server" visible="false">
    <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_RowDataBound"
        CellPadding="0" AllowSorting="false">
        <Columns>
            <asp:TemplateField HeaderText="Seq">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="WorkShop" HeaderText="${ISI.Responsibility.WorkShop}" />
            <asp:BoundField DataField="Operate" HeaderText="${ISI.Responsibility.Operate}" />
            <asp:TemplateField HeaderText="${ISI.TSK.Attachment}">
                <ItemTemplate>
                    <asp:GridView ID="GV_List_Attachment" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_Attachment_RowDataBound"
                        CellPadding="0" AllowSorting="false" ShowHeader="false" RowStyle-VerticalAlign="Top">
                        <Columns>
                            <asp:TemplateField HeaderText="${ISI.TSK.Attachment}">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtnDownLoad" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                        OnClick="lbtnDownLoad_Click" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="${ISI.Responsibility.ResDesc}">
                <ItemTemplate>
                    <asp:GridView ID="GV_List_Detail" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_List_Detail_RowDataBound"
                        CellPadding="0" AllowSorting="false" ShowHeader="true" RowStyle-VerticalAlign="Top">
                        <Columns>
                            <asp:BoundField DataField="Role" HeaderText="${ISI.Responsibility.Role}" />
                            <asp:BoundField DataField="Priority" HeaderText="${ISI.Responsibility.Priority}" />
                            <asp:BoundField DataField="SkillLevel" HeaderText="${ISI.Responsibility.SkillLevel}" />
                            <asp:BoundField DataField="Responsibility" HeaderText="${ISI.Responsibility.ResDesc}" ItemStyle-Width="60%" ItemStyle-Wrap="True" />
                        </Columns>
                    </asp:GridView>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </asp:GridView>
</fieldset>
<style type="text/css">
    /*#fs .GVRow {
        white-space: pre-wrap;
        word-break: break-all;
        word-wrap: break-word;
    }

    #fs .GVAlternatingRow {
        white-space: pre-wrap;
        word-break: break-all;
        word-wrap: break-word;
    }*/
</style>
