<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_BillIO_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Org"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="Org"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:BoundField DataField="Org" HeaderText="${PSI.BillIO.Org}"
                    SortExpression="Org" />
                <asp:BoundField DataField="OrgName" HeaderText="${PSI.BillIO.OrgName}"
                    SortExpression="OrgName" />
                <asp:TemplateField HeaderText="${PSI.BillIO.OrgType}" SortExpression="OrgType">
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Amount" HeaderText="${PSI.BillIO.Amount}"
                    SortExpression="Amount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="BilledAmount" HeaderText="${PSI.BillIO.BilledAmount}"
                    SortExpression="BilledAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="PayAmount" HeaderText="${PSI.BillIO.PayAmount}"
                    SortExpression="PayAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="Diff" HeaderText="${PSI.BillIO.Diff}"
                    SortExpression="Diff" DataFormatString="{0:0.###}" />
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="100">
        </cc1:GridPager>
    </div>
</fieldset>



