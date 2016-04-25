<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_BillDetail_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="Id"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:BoundField DataField="Code" HeaderText="${PSI.Bill.Code}"
                    SortExpression="Code" />
                <asp:TemplateField HeaderText="${PSI.MouldDetail.Type}" SortExpression="Type">
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Invoice" HeaderText="${PSI.MouldDetail.Invoice}"
                    SortExpression="Invoice" />
                <asp:BoundField DataField="BillDate" HeaderText="${PSI.MouldDetail.BillDate}"
                    SortExpression="BillDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="BillAmount" HeaderText="${PSI.MouldDetail.BillAmount}"
                    SortExpression="BillAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="PayDate" HeaderText="${PSI.MouldDetail.PayDate}"
                    SortExpression="PayDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="PayAmount" HeaderText="${PSI.MouldDetail.PayAmount}"
                    SortExpression="PayAmount" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="CreateDate" HeaderText="${Common.Business.CreateDate}"
                    SortExpression="CreateDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="CreateUserNm" HeaderText="${Common.Business.CreateUser}"
                    SortExpression="CreateUser" />
                <asp:BoundField DataField="Remark" HeaderText="${PSI.MouldDetail.Remark}"
                    SortExpression="Remark" />
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="100">
        </cc1:GridPager>
    </div>

    <div class="GridView">
        <cc1:GridView ID="GridViewGroup" runat="server" AutoGenerateColumns="False" DataKeyNames="PrjCode"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="false" AllowSorting="True" AllowPaging="True" PagerID="GridPagerGroup" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount" OnRowDataBound="GVGroup_List_RowDataBound" DefaultSortExpression="PrjCode"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:BoundField DataField="Project" HeaderText="${PSI.Bill.Project}"
                    SortExpression="Project" />
                <asp:BoundField DataField="CustomerDesc" HeaderText="${PSI.Bill.Customer}"
                    SortExpression="Customer" />
                <asp:BoundField DataField="QS" HeaderText="${PSI.Bill.QS}"
                    SortExpression="QS" HeaderStyle-Wrap="false" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOUserDesc" HeaderText="${PSI.Bill.SOUser}"
                    SortExpression="SOUser" HeaderStyle-Wrap="false" />
                <asp:BoundField DataField="Type" HeaderText="${PSI.Bill.Type}"
                    SortExpression="Type" />
                <asp:BoundField DataField="SOContractNo" HeaderText="${PSI.Bill.SOContractNo}"
                    SortExpression="SOContractNo" />
                <asp:BoundField DataField="SOAmount" HeaderText="${PSI.Bill.SOAmount}"
                    SortExpression="SOAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOBilledAmount" HeaderText="${PSI.Bill.SOBilledAmount}"
                    SortExpression="SOBilledAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOPayAmount" HeaderText="${PSI.Bill.SOPayAmount}"
                    SortExpression="SOPayAmount" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="POAmount" HeaderText="${PSI.Bill.POAmount}"
                    SortExpression="POAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="POBilledAmount" HeaderText="${PSI.Bill.POBilledAmount}"
                    SortExpression="POBilledAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="POPayAmount" HeaderText="${PSI.Bill.POPayAmount}"
                    SortExpression="POPayAmount" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="SOAmount1" HeaderText="${PSI.Bill.SOAmount1}"
                    SortExpression="SOAmount1" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOBillDate1" HeaderText="${PSI.Bill.SOBillDate1}"
                    SortExpression="SOBillDate1" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SOBilledAmount1" HeaderText="${PSI.Bill.SOBilledAmount1}"
                    SortExpression="SOBilledAmount1" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOPayDate1" HeaderText="${PSI.Bill.SOPayDate1}"
                    SortExpression="SOPayDate1" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SOPayAmount1" HeaderText="${PSI.Bill.SOPayAmount1}"
                    SortExpression="SOPayAmount1" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="POAmount1" HeaderText="${PSI.Bill.POAmount1}"
                    SortExpression="POAmount1" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="POBilledAmount1" HeaderText="${PSI.Bill.POBilledAmount}"
                    SortExpression="POBilledAmount1" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="POPayAmount1" HeaderText="${PSI.Bill.POPayAmount1}"
                    SortExpression="POPayAmount1" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="SOAmount2" HeaderText="${PSI.Bill.SOAmount2}"
                    SortExpression="SOAmount2" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOBillDate2" HeaderText="${PSI.Bill.SOBillDate2}"
                    SortExpression="SOBillDate2" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SOBilledAmount2" HeaderText="${PSI.Bill.SOBilledAmount2}"
                    SortExpression="SOBilledAmount2" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOPayDate2" HeaderText="${PSI.Bill.SOPayDate2}"
                    SortExpression="SOPayDate2" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SOPayAmount2" HeaderText="${PSI.Bill.SOPayAmount2}"
                    SortExpression="SOPayAmount2" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="POAmount2" HeaderText="${PSI.Bill.POAmount2}"
                    SortExpression="POAmount2" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="POBilledAmount2" HeaderText="${PSI.Bill.POBilledAmount2}"
                    SortExpression="POBilledAmount2" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="POPayAmount2" HeaderText="${PSI.Bill.POPayAmount2}"
                    SortExpression="POPayAmount2" DataFormatString="{0:0.###}" />




                <asp:BoundField DataField="SOAmount4" HeaderText="${PSI.Bill.SOAmount4}"
                    SortExpression="SOAmount4" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOBillDate4" HeaderText="${PSI.Bill.SOBillDate4}"
                    SortExpression="SOBillDate4" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SOBilledAmount4" HeaderText="${PSI.Bill.SOBilledAmount4}"
                    SortExpression="SOBilledAmount4" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOPayDate4" HeaderText="${PSI.Bill.SOPayDate4}"
                    SortExpression="SOPayDate4" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SOPayAmount4" HeaderText="${PSI.Bill.SOPayAmount4}"
                    SortExpression="SOPayAmount4" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="POAmount4" HeaderText="${PSI.Bill.POAmount4}"
                    SortExpression="POAmount4" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="POBilledAmount4" HeaderText="${PSI.Bill.POBilledAmount4}"
                    SortExpression="POBilledAmount4" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="POPayAmount4" HeaderText="${PSI.Bill.POPayAmount4}"
                    SortExpression="POPayAmount4" DataFormatString="{0:0.###}" />



                <asp:BoundField DataField="SOAmount3" HeaderText="${PSI.Bill.SOAmount3}"
                    SortExpression="SOAmount3" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOBillDate3" HeaderText="${PSI.Bill.SOBillDate3}"
                    SortExpression="SOBillDate3" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SOBilledAmount3" HeaderText="${PSI.Bill.SOBilledAmount3}"
                    SortExpression="SOBilledAmount3" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOPayDate3" HeaderText="${PSI.Bill.SOPayDate3}"
                    SortExpression="SOPayDate3" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SOPayAmount3" HeaderText="${PSI.Bill.SOPayAmount3}"
                    SortExpression="SOPayAmount3" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="POAmount3" HeaderText="${PSI.Bill.POAmount3}"
                    SortExpression="POAmount3" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="POBilledAmount3" HeaderText="${PSI.Bill.POBilledAmount}"
                    SortExpression="POBilledAmount3" DataFormatString="{0:0.###}" />

                <asp:BoundField DataField="POPayAmount3" HeaderText="${PSI.Bill.POPayAmount3}"
                    SortExpression="POPayAmount3" DataFormatString="{0:0.###}" />


            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="GridPagerGroup" runat="server" GridViewID="GridViewGroup" PageSize="100">
        </cc1:GridPager>
    </div>

</fieldset>



