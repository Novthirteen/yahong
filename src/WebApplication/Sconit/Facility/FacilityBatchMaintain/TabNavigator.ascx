<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="Facility_FacilityBatchMaintain_TabNavigator" %>
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
        <span class='ajax__tab_active' id='tab_start' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbStart" Text="${Common.Button.MaintainStart}" runat="server" OnClick="lbMaintainStart_Click" /></span></span></span></span><span id='tab_finish' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbFinish" Text="${Common.Button.MaintainFinish}" runat="server" OnClick="lbMaintainFinish_Click" /></span></span></span></span>
    </div>
