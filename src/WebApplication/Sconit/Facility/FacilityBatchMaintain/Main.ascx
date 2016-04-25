<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_FacilityBatchMaintain_Main" %>
<%@ Register Src="TabNavigator.ascx" TagName="Navigator" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityBatchMaintain/MaintainStart/Main.ascx" TagName="Start" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityBatchMaintain/MaintainFinish/Main.ascx" TagName="Finish" TagPrefix="uc2" %>

<uc2:Navigator ID="ucNavigator" runat="server" Visible="true" />
    <div class="ajax__tab_body">
        <uc2:Start ID="ucStart" runat="server" Visible="true" />
        <uc2:Finish ID="ucFinish" runat="server" Visible="false" />
    </div>
</div>