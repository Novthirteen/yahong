<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Wiki.ascx.cs" Inherits="ISI_TSK_Wiki" %>
<fieldset>
    <legend id="lgd" runat="server"></legend>
    <table class="mtable">
        <tr>
            <td colspan="4">
                <asp:TextBox ID="tbWiki" runat="server" Text='<%# Bind("Wiki") %>' Height="400" Width="100%"
                    TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,500);" onpaste="limitPaste(this, 500)" />
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
                    <asp:Button ID="btnSave" runat="server" Text="${Common.Button.Save}" CssClass="query"
                        OnClick="btnSave_Click" ValidationGroup="vgSave" />
                    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                        CssClass="back" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
