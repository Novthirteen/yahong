<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="ISI_TSK_TabNavigator" %>

        
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
    
        <span class='ajax__tab_active' id='tab_mstr' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbMstr" Text="${ISI.TSK.Mstr}" runat="server" OnClick="lbMstr_Click" /></span></span></span></span><span id='tab_attachment' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lblAttachment" Text="${ISI.TSK.Attachment}" runat="server" OnClick="lbAttachment_Click" /></span></span></span></span><span id='tab_processInstance' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbProcessInstance" Text="${ISI.TSK.ProcessInstance}" runat="server" OnClick="lbProcessInstance_Click" /></span></span></span></span><span id='tab_status' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbStatus" Text="${ISI.TSK.Status}" runat="server" OnClick="lbStatus_Click" /></span></span></span></span><span id='tab_reftask' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbRefTask" Text="${ISI.TSK.RefTask}" runat="server" OnClick="lbRefTask_Click" /></span></span></span></span><span id='tab_process' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbProcess" Text="${ISI.TSK.Process}" runat="server" OnClick="lbProcess_Click" /></span></span></span></span><span id='tab_detail' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbDetail" Text="${ISI.TSK.Detail}" runat="server" OnClick="lbDetail_Click" /></span></span></span></span><span id='tab_wiki' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbWiki" Text="${ISI.TSK.Wiki}" runat="server" OnClick="lbWiki_Click" /></span></span></span></span>
    </div>
