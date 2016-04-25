<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="Facility_FacilityDistribution_TabNavigator" %>
        
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
    
        <span class='ajax__tab_active' id='tab_lbFacilityDistribution' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityDistribution" Text="${Facility.FacilityDistribution}" runat="server" OnClick="lbFacilityDistribution_Click" /></span></span></span></span>
          <span class='ajax__tab_active' id='tab_lbFacilityDistributionDetail' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityDistributionDetail" Text="${Facility.FacilityDistributionDetail}" runat="server" OnClick="lbFacilityDistributionDetail_Click" /></span></span></span></span>
          <span class='ajax__tab_active' id='tab_lbFacilityDistributionAttachment' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityDistributionAttachment" Text="${Facility.FacilityDistribution.Attachment}" runat="server" OnClick="lbFacilityDistributionAttachment_Click" /></span></span></span></span>
    </div>
<div class="ajax__tab_body">