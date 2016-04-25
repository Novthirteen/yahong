<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="Facility_FacilityTrans_TabNavigator" %>
        
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
    
           <span class='ajax__tab_active' id='tab_lbFacilityTrans' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityTrans" Text="${Facility.FacilityTrans}" runat="server" OnClick="lbFacilityTrans_Click" /></span></span></span></span>
          <span class='ajax__tab_active' id='tab_lbFacilityAttachment' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityAttachment" Text="${Facility.FacilityTrans.Attachment}" runat="server" OnClick="lbFacilityAttachment_Click" /></span></span></span></span>
    </div>
<div class="ajax__tab_body">