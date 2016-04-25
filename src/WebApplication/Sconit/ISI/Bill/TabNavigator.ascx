<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="ISI_Bill_TabNavigator" %>
        
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
        <span class='ajax__tab_active' id='tab_lbMould' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbMould" Text="${Common.Business.BasicInfo}" runat="server" OnClick="lbMould_Click" /></span></span></span></span><span class='ajax__tab_active' id='tab_lbMouldAttachment' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span 
        class='ajax__tab_tab'><asp:LinkButton ID="lbMouldAttachment" Text="${PSI.Bill.Attachment}" runat="server" OnClick="lbMouldAttachment_Click" /></span></span></span></span>
    </div>
<div class="ajax__tab_body">