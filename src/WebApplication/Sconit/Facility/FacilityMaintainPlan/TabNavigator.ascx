<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="Facility_MaintainPlan_TabNavigator" %>
        
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">

           <span class='ajax__tab_active' id='tab_lbMaintainPlan' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbMaintainPlan" Text="${Facility.MaintainPlan}" runat="server" OnClick="lbMaintainPlan_Click" /></span></span></span></span>
          <span class='ajax__tab_active' id='tab_lbMaintainPlanAttachment' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbMaintainPlanAttachment" Text="${Facility.Attachment}" runat="server" OnClick="lbMaintainPlanAttachment_Click" /></span></span></span></span>
    </div>
<div class="ajax__tab_body">