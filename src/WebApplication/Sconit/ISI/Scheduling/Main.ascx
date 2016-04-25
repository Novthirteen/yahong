<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_Scheduling_Main" %>
<%@ Register Src="TabNavigator.ascx" TagName="TabNavigator" TagPrefix="uc2" %>
<%@ Register Src="View.ascx" TagName="View" TagPrefix="uc2" %>
<%@ Register Src="Mstr.ascx" TagName="General" TagPrefix="uc2" %>
<%@ Register Src="Mstr.ascx" TagName="Special" TagPrefix="uc2" %>
<uc2:TabNavigator ID="ucTabNavigator" runat="server" Visible="true" />
<div class="ajax__tab_body">
    <uc2:View ID="ucView" runat="server" Visible="true" />
    <uc2:General ID="ucGeneral" runat="server" Visible="false" />
    <uc2:Special ID="ucSpecial" runat="server" Visible="false" />
</div>
</div> 