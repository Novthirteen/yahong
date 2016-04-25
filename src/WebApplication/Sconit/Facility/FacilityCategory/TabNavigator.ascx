<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="Facility_FacilityCategory_TabNavigator" %>
        
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">

           <span class='ajax__tab_active' id='tab_lbFacilityCategory' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityCategory" Text="${Facility.FacilityCategory}" runat="server" OnClick="lbFacilityCategory_Click" /></span></span></span></span>
          <span class='ajax__tab_active' id='tab_lbFacilityCategoryAttachment' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbFacilityCategoryAttachment" Text="${Facility.Attachment}" runat="server" OnClick="lbFacilityCategoryAttachment_Click" /></span></span></span></span>
    </div>
<div class="ajax__tab_body">