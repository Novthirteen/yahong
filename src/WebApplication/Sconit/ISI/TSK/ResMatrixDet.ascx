<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ResMatrixDet.ascx.cs" Inherits="ISI_TSK_ResMatrixDet" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>

<fieldset>
    <legend id="lgd" runat="server"></legend>
    <asp:GridView ID="GV_List_Detail" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField HeaderText="${ISI.TSK.ResMatrixDet.Subject}" ItemStyle-Width="20%">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "Subject")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${ISI.TSK.ResMatrixDet.Desc1}" ItemStyle-Width="70%">
                <ItemTemplate>
                   <asp:Label ID="lblDesc1" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreateDate" HeaderStyle-Wrap="false" ItemStyle-Width="10%"
                HeaderText="${ISI.TSK.ResMatrixDet.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
        </Columns>
    </asp:GridView>
</fieldset>
<div class="tablefooter">
    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
        CssClass="back" />
</div>
