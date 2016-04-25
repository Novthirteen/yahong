<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityTrans_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound" DefaultSortExpression="Id"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:BoundField DataField="FCID" HeaderText="${Facility.FacilityMaster.FCID}" SortExpression="FCID" />
                <asp:BoundField DataField="AssetNo" HeaderText="${Facility.FacilityMaster.AssetNo}"
                    SortExpression="AssetNo" />
                <asp:BoundField DataField="FacilityName" HeaderText="${Facility.FacilityMaster.Name}"
                    SortExpression="FacilityName" />
                <asp:BoundField DataField="FacilityCategory" HeaderText="${Facility.FacilityMaster.Category}"
                    SortExpression="FacilityCategory" />
                <asp:TemplateField HeaderText="${Facility.FacilityTrans.TransType}" SortExpression="TransType">
                    <ItemTemplate>
                        <asp:Label ID="lblTransType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
             <%--   <asp:BoundField DataField="EffDate" HeaderText="${Facility.FacilityTrans.EffDate}"
                    SortExpression="EffDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="FromChargePerson" HeaderText="${Facility.FacilityTrans.FromChargePerson}"
                    SortExpression="FromChargePerson" />
                <asp:BoundField DataField="FromChargePersonName" HeaderText="${Facility.FacilityTrans.FromChargePersonName}"
                    SortExpression="FromChargePersonName" />
                <asp:BoundField DataField="ToChargePerson" HeaderText="${Facility.FacilityTrans.ToChargePerson}"
                    SortExpression="ToChargePerson" />
                <asp:BoundField DataField="ToChargePersonName" HeaderText="${Facility.FacilityTrans.ToChargePersonName}"
                    SortExpression="ToChargePersonName" />
                <asp:BoundField DataField="FromChargeSite" HeaderText="${Facility.FacilityTrans.FromChargeSite}"
                    SortExpression="FromChargeSite" />--%>
                <asp:BoundField DataField="ToChargeSite" HeaderText="${Facility.FacilityTrans.ToChargeSite}"
                    SortExpression="ToChargeSite" />
             <%--   <asp:BoundField DataField="FromOrganization" HeaderText="${Facility.FacilityTrans.FromOrganization}"
                    SortExpression="FromOrganization" />
                <asp:BoundField DataField="ToOrganization" HeaderText="${Facility.FacilityTrans.ToOrganization}"
                    SortExpression="ToOrganization" />--%>
                <asp:BoundField DataField="Remark" HeaderText="${Facility.FacilityTrans.Remark}"
                    SortExpression="Remark" />
                <asp:BoundField DataField="StartDate" HeaderText="${Facility.FacilityTrans.StartDate}"
                    SortExpression="StartDate" />
                <asp:BoundField DataField="EndDate" HeaderText="${Facility.FacilityTrans.EndDate}"
                    SortExpression="EndDate" />
                <asp:BoundField DataField="CreateDate" HeaderText="${Facility.FacilityTrans.CreateDate}"
                    SortExpression="CreateDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="CreateUser" HeaderText="${Facility.FacilityTrans.CreateUser}"
                    SortExpression="CreateUser" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <cc1:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click" FunctionId="UpdateFacilityTrans">
                        </cc1:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="50">
        </cc1:GridPager>
    </div>
</fieldset>
