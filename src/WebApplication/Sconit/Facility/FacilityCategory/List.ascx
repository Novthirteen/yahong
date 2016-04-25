<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityCategory_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Code"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Code" HeaderText="${Facility.FacilityCategory.Code}" SortExpression="Code" />
                <asp:BoundField DataField="Description" HeaderText="${Facility.FacilityCategory.Description}"
                    SortExpression="Description" />
                <asp:BoundField DataField="ParentCategory" HeaderText="${Facility.FacilityCategory.ParentCategory}"
                    SortExpression="ParentCategory" />
                <asp:BoundField DataField="ChargePersonName" HeaderText="${Facility.FacilityCategory.ChargePerson}"
                    SortExpression="ChargePersonName" />
                <asp:BoundField DataField="ChargeSite" HeaderText="${Facility.FacilityCategory.ChargeSite}"
                    SortExpression="ChargeSite" />
                <asp:BoundField DataField="ChargeOrganization" HeaderText="${Facility.FacilityCategory.ChargeOrganization}"
                    SortExpression="ChargeOrganization" />
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
