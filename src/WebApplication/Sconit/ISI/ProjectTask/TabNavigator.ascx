<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="ISI_ProjectTask_TabNavigator" %>
       
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
    
        <span class='ajax__tab_active' id='tab_mstr' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbMstr" Text="${ISI.TSK.Mstr}" runat="server" OnClick="lbMstr_Click" /></span></span></span></span><span id='tab_attachment' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lblAttachment" Text="${ISI.TSK.Attachment}" runat="server" OnClick="lbAttachment_Click" /></span></span></span></span>
    </div>
