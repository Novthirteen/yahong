﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Lost.ascx.cs" Inherits="Facility_FacilityLost_Finish" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_FacilityTrans" runat="server" DataSourceID="ODS_FacilityMaster"
        DefaultMode="Edit" Width="100%" DataKeyNames="FCID" OnDataBound="FV_FacilityMaster_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${Facility.FacilityMaster.Lost}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${Facility.FacilityMaster.FCID}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblFCID" runat="server" Text='<%# Bind("FCID") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlFacilityCategory" runat="server" Text="${Facility.FacilityMaster.Category}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblFacilityCategory" runat="server" Text='<%# Bind("Category") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCreateDate" runat="server" Text="${Facility.FacilityTrans.CreateDate}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblCreateDate" runat="server" Text='<%# Bind("CreateDate") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlCreateUser" runat="server" Text="${Facility.FacilityTrans.CreateUser}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblCreateUser" runat="server" Text='<%# Bind("CreateUser") %>'  />
                        </td>
                    </tr>
                     <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlRemark" runat="server" Text="${Facility.FacilityLost.Remark}:" />
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
                                <asp:Button ID="btnLost" runat="server" OnClick="btnLost_Click" Text="${Common.Button.Lost}"
                                    CssClass="apply"  />
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
    DataObjectTypeName="com.Sconit.Facility.Entity.FacilityMaster" SelectMethod="LoadFacilityMaster">
    <SelectParameters>
        <asp:Parameter Name="fcId" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
