<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="Facility_FacilityStock_TabNavigator" %>
        
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
        <span class='ajax__tab_active' id='tab_lbFacilityStockMaster' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityStockMaster" Text="${Facility.FacilityStockMaster}" runat="server" OnClick="lbFacilityStockMaster_Click" /></span></span></span></span>
          <span class='ajax__tab_active' id='tab_lbFacilityStockDetail' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityStockDetail" Text="${Facility.FacilityStockDetail}" runat="server" OnClick="lbFacilityStockDetail_Click" /></span></span></span></span>
            <span class='ajax__tab_active' id='tab_lbFacilityAttachment' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityAttachment" Text="${Facility.FacilityStock.Attachment}" runat="server" OnClick="lbFacilityAttachment_Click" /></span></span></span></span>
    
    
    </div>
<div class="ajax__tab_body" />