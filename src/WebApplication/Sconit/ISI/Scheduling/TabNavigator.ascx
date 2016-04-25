<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs"
    Inherits="ISI_Scheduling_TabNavigator" %>
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
        <span class='ajax__tab_active' id='tab_view' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbView" Text="${Common.Business.View}" runat="server" OnClick="lbView_Click" /></span></span></span></span><span id='tab_general' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbGeneral" Text="${ISI.Scheduling.General}" runat="server" OnClick="lbGeneral_Click" /></span></span></span></span><span id='tab_special' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbSpecial" Text="${ISI.Scheduling.Special}" runat="server" OnClick="lbSpecial_Click" /></span></span></span></span>

    </div>

