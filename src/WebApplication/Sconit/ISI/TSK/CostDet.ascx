<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CostDet.ascx.cs" Inherits="ISI_TSK_CostDet" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <legend>${WFS.CostDet.List1}</legend>
    <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false" ShowHeader="true">
        <Columns>
            <asp:TemplateField HeaderText="Seq">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CostCenter" HeaderText="${WFS.CostDet.CostCenter}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="Account1Name" HeaderText="${WFS.CostDet.Account1}" />
            <asp:BoundField DataField="Account2Name" HeaderText="${WFS.CostDet.Account2}" />
            <asp:BoundField DataField="Amount" HeaderText="${WFS.CostDet.Amount}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="Taxes" HeaderText="${WFS.CostDet.Taxes}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="TotalAmount" HeaderText="${WFS.CostDet.TotalAmount}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="BudgetCode" HeaderText="${WFS.CostDet.BudgetCode}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="BudgetAmount" HeaderText="${WFS.CostDet.BudgetAmount}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="BudgetAmount2" HeaderText="${WFS.CostDet.BudgetAmount2}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AmountY1" HeaderText="${WFS.CostDet.AmountY1}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AmountY3" HeaderText="${WFS.CostDet.AmountY3}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AmountY4" HeaderText="${WFS.CostDet.AmountY4}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AmountY2" HeaderText="${WFS.CostDet.AmountY2}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
        </Columns>
    </asp:GridView>
</fieldset>
<fieldset>
    <legend>${WFS.CostDet.List2}</legend>
    <asp:GridView ID="GV_ListMonth" runat="server" OnRowDataBound="GV_ListMonth_RowDataBound"
        AutoGenerateColumns="false" ShowHeader="true">
        <Columns>
            <asp:TemplateField HeaderText="Seq">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CostCenter" HeaderText="${WFS.CostDet.CostCenter}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="Account1Name" HeaderText="${WFS.CostDet.Account1}" />
            <asp:BoundField DataField="Account2Name" HeaderText="${WFS.CostDet.Account2}" />
            <asp:BoundField DataField="Amount" HeaderText="${WFS.CostDet.Amount}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="Taxes" HeaderText="${WFS.CostDet.Taxes}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="TotalAmount" HeaderText="${WFS.CostDet.TotalAmount}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="BudgetCode" HeaderText="${WFS.CostDet.BudgetCode}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="BudgetAmountMonth" HeaderText="${WFS.CostDet.BudgetAmountMonth}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AmountM1" HeaderText="${WFS.CostDet.AmountM1}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AmountM3" HeaderText="${WFS.CostDet.AmountM3}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AmountM4" HeaderText="${WFS.CostDet.AmountM4}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
            <asp:BoundField DataField="AmountM2" HeaderText="${WFS.CostDet.AmountM2}" DataFormatString="{0:N}" HeaderStyle-Wrap="false" />
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>
