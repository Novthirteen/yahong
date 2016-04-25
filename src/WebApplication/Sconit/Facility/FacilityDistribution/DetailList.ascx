<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DetailList.ascx.cs" Inherits="Facility_FacilityDistributionDetail_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="Id"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:TemplateField HeaderText="${Facility.FacilityDistributionDetail.Type}" SortExpression="Type">
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Invoice" HeaderText="${Facility.FacilityDistributionDetail.Invoice}"
                    SortExpression="Invoice" />
                <asp:BoundField DataField="BillDate" HeaderText="${Facility.FacilityDistributionDetail.BillDate}"
                    SortExpression="BillDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="BillAmount" HeaderText="${Facility.FacilityDistributionDetail.BillAmount}"
                    SortExpression="BillAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="PayDate" HeaderText="${Facility.FacilityDistributionDetail.PayDate}"
                    SortExpression="PayDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="PayAmount" HeaderText="${Facility.FacilityDistributionDetail.PayAmount}"
                    SortExpression="PayAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="Contact" HeaderText="${Facility.FacilityDistributionDetail.Contact}"
                    SortExpression="Contact" />
                <asp:BoundField DataField="CreateDate" HeaderText="${Facility.FacilityDistributionDetail.CreateDate}"
                    SortExpression="CreateDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="CreateUser" HeaderText="${Facility.FacilityDistributionDetail.CreateUser}"
                    SortExpression="CreateUser" />
                      <asp:BoundField DataField="Remark" HeaderText="${Facility.FacilityDistributionDetail.Remark}"
                    SortExpression="Remark" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click" />
                        <cc1:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click"  FunctionId="CreateFacilityDistributionDetail" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
<div class="buttons">
    <cc1:Button ID="btnNew" runat="server" Text="${Common.Button.New}" OnClick="btnNew_Click"
        CssClass="query" FunctionId="CreateFacilityDistributionDetail"/>
    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
        CssClass="back" />
</div>
