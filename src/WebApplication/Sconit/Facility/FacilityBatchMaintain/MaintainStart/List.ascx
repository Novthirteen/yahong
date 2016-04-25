<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Facility_FacilityMaintain_StartList" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<script language="javascript" type="text/javascript">
    function GVCheckClick() {
        if ($(".GVHeader input:checkbox").attr("checked") == 'checked') {
            $(".GVRow input:checkbox").attr("checked", true);
            $(".GVAlternatingRow input:checkbox").attr("checked", true);
        }
        else {
            $(".GVRow input:checkbox").attr("checked", false);
            $(".GVAlternatingRow input:checkbox").attr("checked", false);
        }
    }
</script>
<fieldset>
    <div class="GridView">
        <asp:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" OnRowDataBound="GV_List_RowDataBound">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div onclick="GVCheckClick()">
                            <asp:CheckBox ID="CheckAll" runat="server" />
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBoxGroup" name="CheckBoxGroup" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.FCID}" SortExpression="FCID">
                    <ItemTemplate>
                        <asp:Literal ID="ltlFCID" runat="server" Text='<%# Eval("FCID")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="AssetNo" HeaderText="${Facility.FacilityMaster.AssetNo}"
                    SortExpression="AssetNo" />
                <asp:BoundField DataField="Name" HeaderText="${Facility.FacilityMaster.Name}" SortExpression="Name" />
                <asp:BoundField DataField="Specification" HeaderText="${Facility.FacilityMaster.Specification}"
                    SortExpression="Specification" />
                <asp:BoundField DataField="Capacity" HeaderText="${Facility.FacilityMaster.Capacity}"
                    SortExpression="Capacity" />
                <asp:BoundField DataField="ManufactureDate" HeaderText="${Facility.FacilityMaster.ManufactureDate}"
                    SortExpression="ManufactureDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="SerialNo" HeaderText="${Facility.FacilityMaster.SerialNo}"
                    SortExpression="SerialNo" />
                <asp:BoundField DataField="WarrantyInfo" HeaderText="${Facility.FacilityMaster.WarrantyInfo}"
                    SortExpression="WarrantyInfo" />
                <asp:BoundField DataField="EffDate" HeaderText="${Facility.FacilityMaster.EffDate}"
                    SortExpression="EffDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Price" HeaderText="${Facility.FacilityMaster.Price}" SortExpression="Price" />
                <asp:TemplateField HeaderText="${Facility.FacilityMaster.Owner}" SortExpression="Owner">
                    <ItemTemplate>
                        <asp:Label ID="lblOwner" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</fieldset>
<div class="tablefooter">
    <asp:Button ID="btnMaintainStart" runat="server" OnClick="btnMaintainStart_Click"
        Text="${Common.Button.MaintainStart}" CssClass="button2" />
</div>
