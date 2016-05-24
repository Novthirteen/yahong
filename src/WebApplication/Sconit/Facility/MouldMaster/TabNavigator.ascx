<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="Facility_MouldMaster_TabNavigator" %>
        
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
    
        <span class='ajax__tab_active' id='tab_lbFacilityMaster' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityMaster" Text="${Facility.FacilityMaster}" runat="server" OnClick="lbFacilityMaster_Click" /></span></span></span></span>
          <span class='ajax__tab_active' id='tab_lbFacilityMaintain' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityMaintain" Text="${Facility.FacilityMaintain}" runat="server" OnClick="lbFacilityMaintain_Click" /></span></span></span></span>
           <span class='ajax__tab_active' id='tab_lbFacilityTrans' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityTrans" Text="${Facility.FacilityTrans}" runat="server" OnClick="lbFacilityTrans_Click" /></span></span></span></span>
          <span class='ajax__tab_active' id='tab_lbFacilityAttachment' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityAttachment" Text="${Facility.FacilityMaster.Attachment}" runat="server" OnClick="lbFacilityAttachment_Click" /></span></span></span></span>
          <span class='ajax__tab_active' id='tab_lbFacilityTemplate' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityTemplate" Text="${Facility.Template}" runat="server" OnClick="lbFacilityTemplate_Click" /></span></span></span></span>
    </div>
<div class="ajax__tab_body">