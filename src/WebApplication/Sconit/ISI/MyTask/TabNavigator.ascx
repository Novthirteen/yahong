<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="ISI_MyTask_TabNavigator" %>

        
<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">
    
        <span class='ajax__tab_active' id='tab_mstr' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbMstr" Text="${ISI.MyTask.Mstr}" runat="server" OnClick="lbMstr_Click" /></span></span></span></span><span id='tab_create' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbCreate" Text="${ISI.MyTask.Create}" runat="server" OnClick="lbCreate_Click" /></span></span></span></span><span id='tab_submit' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbSubmit" Text="${ISI.MyTask.Submit}" runat="server" OnClick="lbSubmit_Click" /></span></span></span></span><span id='tab_assign' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbAssign" Text="${ISI.MyTask.Assign}" runat="server" OnClick="lbAssign_Click" /></span></span></span></span><span id='tab_start' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbStart" Text="${ISI.MyTask.Start}" runat="server" OnClick="lbStart_Click" /></span></span></span></span><span id='tab_complete' runat="server"><span 
        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbComplete" Text="${ISI.MyTask.Complete}" runat="server" OnClick="lbComplete_Click" /></span></span></span></span>
    </div>
