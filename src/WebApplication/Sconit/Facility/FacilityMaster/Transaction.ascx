<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Transaction.ascx.cs" Inherits="Facility_FacilityMaster_Trans" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="TransAttachment.ascx" TagName="TransAttachment" TagPrefix="uc2" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="FCID"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound"  DefaultSortExpression="Id"
            DefaultSortDirection="Descending">
            <Columns>
                <asp:BoundField DataField="FCID" HeaderText="${Facility.FacilityMaster.FCID}" SortExpression="FCID" />
                <asp:BoundField DataField="FacilityCategory" HeaderText="${Facility.FacilityMaster.Category}"
                    SortExpression="FacilityCategory" />
                <asp:TemplateField HeaderText="${Facility.FacilityTrans.TransType}" SortExpression="TransType">
                    <ItemTemplate>
                        <asp:Label ID="lblTransType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="EffDate" HeaderText="${Facility.FacilityTrans.EffDate}"
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
                    SortExpression="FromChargeSite" />
                <asp:BoundField DataField="ToChargeSite" HeaderText="${Facility.FacilityTrans.ToChargeSite}"
                    SortExpression="ToChargeSite" />
                <asp:BoundField DataField="FromOrganization" HeaderText="${Facility.FacilityTrans.FromOrganization}"
                    SortExpression="FromOrganization" />
                <asp:BoundField DataField="ToOrganization" HeaderText="${Facility.FacilityTrans.ToOrganization}"
                    SortExpression="ToOrganization" />
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
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnAttachment" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Attachment}" OnClick="lbtnAttachment_Click" Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
<div class="buttons">
    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
        CssClass="back" />
</div>
<uc2:TransAttachment ID="ucTransAttachment" runat="server" Visible="false" />