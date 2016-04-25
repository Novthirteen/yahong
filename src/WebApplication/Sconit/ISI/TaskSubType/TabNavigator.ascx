<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabNavigator.ascx.cs" Inherits="ISI_TaskSubType_TabNavigator" %>


<div class="AjaxClass  ajax__tab_default">
    <div class="ajax__tab_header">

        <span class='ajax__tab_active' id='tab_mstr' runat="server"><span class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'>
            <asp:LinkButton ID="lbMstr" Text="${ISI.TaskSubType.Basic.Info}" runat="server" OnClick="lbMstr_Click" /></span></span></span></span><span id='tab_process' runat="server"><span
                class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbProcessInstance" Text="${ISI.TaskSubType.Process}" runat="server" OnClick="lbProcess_Click" /></span></span></span></span><span id='tab_form' runat="server"><span
                    class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lblForm" Text="${ISI.TaskSubType.Form}" runat="server" OnClick="lbForm_Click" /></span></span></span></span><span id='tab_attachment' runat="server"><span
                        class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lblAttachment" Text="${ISI.TaskSubType.Attachment}" runat="server" OnClick="lbAttachment_Click" /></span></span></span></span><span id='tab_budget' visible="false" runat="server"><span
                            class='ajax__tab_outer'><span class='ajax__tab_inner'><span class='ajax__tab_tab'><asp:LinkButton ID="lbBudget" Text="${ISI.TaskSubType.Budget}" runat="server" OnClick="lbBudget_Click" /></span></span></span></span>
    </div>
