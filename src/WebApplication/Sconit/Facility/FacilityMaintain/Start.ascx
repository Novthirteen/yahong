<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Start.ascx.cs" Inherits="Facility_FacilityMaintain_Start" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_FacilityMaster" runat="server" DataSourceID="ODS_FacilityMaster"
        DefaultMode="Edit" Width="100%" DataKeyNames="FcId" OnDataBound="FV_FacilityMaster_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityMaster.Maintain}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityMaster.FCID}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblFCID" runat="server" Text='<%# Bind("FCID") %>' />
                            <td class="td01">
                                <asp:Literal ID="ltlName" runat="server" Text="${Facility.FacilityMaster.Name}:" />
                            </td>
                            <td class="td02">
                                <asp:Literal ID="tbName" runat="server" Text='<%# Bind("Name") %>' />
                            </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSpecification" runat="server" Text="${Facility.FacilityMaster.Specification}:" />
                        </td>
                        <td class="td02">
                            <asp:Literal ID="tbSpecification" runat="server" Text='<%# Bind("Specification") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlCapacity" runat="server" Text="${Facility.FacilityMaster.Capacity}:" />
                        </td>
                        <td class="td02">
                            <asp:Literal ID="tbCapacity" runat="server" Text='<%# Bind("Capacity") %>' />
                        </td>
                    </tr>
                    <tr>
                         <td class="td01">
                            <asp:Literal ID="ltlRemark" runat="server" Text="${Facility.FacilityMaintain.StartRemark}:" />
                        </td>
                          <td class="td02" colspan="3">
                            <asp:TextBox ID="tbRemark" runat="server" Text='<%# Bind("Remark") %>' TextMode="MultiLine"
                                Height="50" Width="75%" />
                        </td>
                        <td class="td01">
                           
                        </td>
                        <td class="td02">
                            
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                        </td>
                        <td class="td02">
                        </td>
                        <td class="td01">
                        </td>
                        <td class="td02">
                            <div class="buttons">
                                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="${Common.Button.MaintainStart}"
                                    CssClass="apply" ValidationGroup="vgSave" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_FacilityMaster" runat="server" TypeName="com.Sconit.Web.FacilityMasterMgrProxy"
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityMaster"  SelectMethod="LoadFacilityMaster" >
    <SelectParameters>
        <asp:Parameter Name="fcId" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
