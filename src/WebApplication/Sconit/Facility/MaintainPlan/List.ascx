<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_MaintainPlan_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Code" HeaderText="${Facility.MaintainPlan.Code}" SortExpression="Code" />
                <asp:BoundField DataField="Description" HeaderText="${Facility.MaintainPlan.Description}"
                    SortExpression="Description" />
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Category}" SortExpression="Category">
                    <ItemTemplate>
                        <asp:Label ID="lblCategory" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.MaintainPlan.Type}" SortExpression="Type">
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TypePeriod" HeaderText="${Facility.MaintainPlan.TypePeriod}"
                    SortExpression="TypePeriod" />
                <asp:BoundField DataField="Period" HeaderText="${Facility.MaintainPlan.Period}" SortExpression="Period" />
                <asp:BoundField DataField="LeadTime" HeaderText="${Facility.MaintainPlan.LeadTime}"
                    SortExpression="LeadTime" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Code") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
