<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityMaintainPlanDetail_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="ListAttachment.ascx" TagName="ListAttachment" TagPrefix="uc2" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            ShowSeqNo="true" AllowSorting="true" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.FCID}" SortExpression="FacilityMaster.FCID">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FacilityMaster.FCID")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Name}" SortExpression="FacilityMaster.Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FacilityMaster.Name")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.AssetNo}" SortExpression="FacilityMaster.AssetNo">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FacilityMaster.AssetNo")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.MaintainPlan.Code}" SortExpression="MaintainPlan.Code">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MaintainPlan.Code")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.MaintainPlan.Description}" SortExpression="MaintainPlan.Description">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MaintainPlan.Description")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.MaintainPlan.Type}" SortExpression="Type">
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.MaintainPlan.TypePeriod}" SortExpression="MaintainPlan.TypePeriod">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MaintainPlan.TypePeriod")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.MaintainPlan.Period}" SortExpression="MaintainPlan.Period">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MaintainPlan.Period")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.MaintainPlan.LeadTime}" SortExpression="MaintainPlan.LeadTime">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MaintainPlan.LeadTime")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="StartDate" HeaderText="${Facility.MaintainPlan.StartDate}"
                    SortExpression="StartDate" />
                <asp:BoundField DataField="NextMaintainDate" HeaderText="${Facility.MaintainPlan.NextMaintainDate}"
                    SortExpression="Period" />
                <asp:BoundField DataField="NextWarnDate" HeaderText="${Facility.MaintainPlan.NextWarnDate}"
                    SortExpression="NextWarnDate" />
                <asp:BoundField DataField="StartQty" HeaderText="${Facility.MaintainPlan.StartQty}"
                    SortExpression="StartQty" DataFormatString="{0:0.##}" />
                <asp:BoundField DataField="NextMaintainQty" HeaderText="${Facility.MaintainPlan.NextMaintainQty}"
                    SortExpression="NextMaintainQty" DataFormatString="{0:0.##}" />
                <asp:BoundField DataField="NextWarnQty" HeaderText="${Facility.MaintainPlan.NextWarnQty}"
                    SortExpression="NextWarnQty" DataFormatString="{0:0.##}" />
                <asp:TemplateField HeaderText="${Facility.FacilityDistribution.Attachment}">
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
<uc2:ListAttachment ID="ucTransAttachment" runat="server" Visible="false" />
