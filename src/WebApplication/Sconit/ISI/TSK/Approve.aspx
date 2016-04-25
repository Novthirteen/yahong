<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Approve.aspx.cs" Inherits="ISI_TSK_Approve" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="mtable">
                <tr>

                    <td>
                        <asp:TextBox ID="tbApprove" runat="server" Text='<%# Bind("Desc1") %>' Height="60"
                            Width="650" TextMode="MultiLine"  Font-Size="10" />
                    </td>
                </tr>
                <tr>
                    <td class="td02">
                        <div class="buttons">
                            <asp:Button ID="btnApprove" runat="server" Text="" OnClick="btnApprove_Click" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
