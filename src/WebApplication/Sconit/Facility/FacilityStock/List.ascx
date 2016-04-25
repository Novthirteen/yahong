<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityStock_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="StNo"
            AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="True" AllowPaging="True" PagerID="gp" Width="100%"
            TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll" SelectCountMethod="FindCount"
            DefaultSortExpression="StNo" DefaultSortDirection="Descending" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:BoundField DataField="StNo" HeaderText="${Facility.FacilityStock.StNo}" SortExpression="StNo" />
                <asp:BoundField DataField="EffDate" SortExpression="EffDate" HeaderStyle-Wrap="false"
                    HeaderText="${Common.Business.EffDate}" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Status" HeaderText="${Common.CodeMaster.Status}" SortExpression="Status" />
                <asp:TemplateField HeaderText="${Common.Business.CreateUser}" SortExpression="CreateUser">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CreateUser")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ChargeOrg" HeaderText="${Facility.FacilityStock.ChargeOrg}"
                    SortExpression="ChargeOrg" />
                <asp:BoundField DataField="ChargeSite" HeaderText="${Facility.FacilityStock.ChargeSite}"
                    SortExpression="ChargeSite" />
                <asp:BoundField DataField="ChargePersonName" HeaderText="${Facility.FacilityStock.ChargePerson}"
                    SortExpression="ChargePersonName" />
                <asp:BoundField DataField="FacilityCategory" HeaderText="${Facility.FacilityStock.FacilityCategory}"
                    SortExpression="FacilityCategory" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnView" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "StNo") %>'
                            Text="${Common.Button.View}" OnClick="lbtnView_Click">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="50">
        </cc1:GridPager>
    </div>
</fieldset>
