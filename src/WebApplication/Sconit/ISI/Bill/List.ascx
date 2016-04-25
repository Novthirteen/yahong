<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_Bill_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="ListAttachment.ascx" TagName="ListAttachment" TagPrefix="uc2" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            ShowSeqNo="false" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound">
            <Columns>

                <asp:TemplateField HeaderText="${PSI.Bill.Code}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <div>
                            <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                                OnClick="lbtnEdit_Click" Text='<%# DataBinder.Eval(Container.DataItem, "Code")%>' />
                        </div>
                        <div>
                            <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                                Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FCID" HeaderText="${PSI.Bill.FCID}"
                    SortExpression="FCID" HeaderStyle-Wrap="false" />
                <asp:BoundField DataField="Desc1" HeaderText="${Common.Business.Description}"
                    SortExpression="Desc1" />
                <asp:BoundField DataField="Project" HeaderText="${PSI.Bill.Project}"
                    SortExpression="Project" />
                <asp:BoundField DataField="QS" HeaderText="${PSI.Bill.QS}"
                    SortExpression="QS" HeaderStyle-Wrap="false"  DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SOUserDesc" HeaderText="${PSI.Bill.SOUser}"
                    SortExpression="SOUser" HeaderStyle-Wrap="false" />
                <asp:BoundField DataField="POUserDesc" HeaderText="${PSI.Bill.POUser}"
                    SortExpression="POUser" HeaderStyle-Wrap="false" />
                <asp:BoundField DataField="CustomerDesc" HeaderText="${PSI.Bill.Customer}"
                    SortExpression="Customer" />
                <asp:BoundField DataField="SupplierDesc" HeaderText="${PSI.Bill.Supplier}"
                    SortExpression="Supplier" />
                <asp:BoundField DataField="SOContractNo" HeaderText="${PSI.Bill.SOContractNo}"
                    SortExpression="SOContractNo" />
                <asp:BoundField DataField="SupplierContractNo" HeaderText="${PSI.Bill.SupplierContractNo}"
                    SortExpression="SupplierContractNo" />
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
                <asp:BoundField DataField="SupplierAmount" HeaderText="${PSI.Bill.SupplierAmount}"
                    SortExpression="SupplierAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SupplierBilledAmount" HeaderText="${PSI.Bill.SupplierBilledAmount}"
                    SortExpression="SupplierBilledAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="SupplierPayAmount" HeaderText="${PSI.Bill.SupplierPayAmount}"
                    SortExpression="SupplierPayAmount" DataFormatString="{0:0.###}" />
                <asp:TemplateField HeaderText="${PSI.Bill.Status}" SortExpression="Status" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${PSI.Bill.Attachment}" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnAttachment" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Attachment}" OnClick="lbtnAttachment_Click" Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="50">
        </cc1:GridPager>
    </div>
</fieldset>
<uc2:ListAttachment ID="ucTransAttachment" runat="server" Visible="false" />
