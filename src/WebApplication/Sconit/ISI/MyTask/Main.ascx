<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_MyTask_Main" %>
<%@ Register Src="TabNavigator.ascx" TagName="TabNavigator" TagPrefix="uc2" %>
<%@ Register Src="Mstr.ascx" TagName="Mstr" TagPrefix="uc2" %>
<%@ Register Src="Mstr.ascx" TagName="Create" TagPrefix="uc2" %>
<%@ Register Src="Mstr.ascx" TagName="Submit" TagPrefix="uc2" %>
<%@ Register Src="Mstr.ascx" TagName="Assign" TagPrefix="uc2" %>
<%@ Register Src="Mstr.ascx" TagName="Start" TagPrefix="uc2" %>
<%@ Register Src="Mstr.ascx" TagName="Complete" TagPrefix="uc2" %>
<uc2:TabNavigator ID="ucTabNavigator" runat="server" Visible="true" />
<div class="ajax__tab_body">
    <uc2:Mstr ID="ucMstr" runat="server" Visible="true" TabAction="" />
    <uc2:Create ID="ucCreate" runat="server" Visible="false" TabAction="Create" />
    <uc2:Submit ID="ucSubmit" runat="server" Visible="false" TabAction="Submit" />
    <uc2:Assign ID="ucAssign" runat="server" Visible="false" TabAction="Assign" />
    <uc2:Start ID="ucStart" runat="server" Visible="false" TabAction="In-Process" />
    <uc2:Complete ID="ucComplete" runat="server" Visible="false" TabAction="Complete" />
</div>
</div> 