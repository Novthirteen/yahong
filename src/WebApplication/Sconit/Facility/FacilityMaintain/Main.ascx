<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_FacilityMaintain_Main" %>
<%@ Register Src="~/Facility/FacilityMaintain/Finish.ascx" TagName="Finish" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityMaintain/List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityMaintain/Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityTrans/EditMain.ascx" TagName="Trans" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityMaintain/Start.ascx" TagName="Start" TagPrefix="uc2" %>

<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:Finish ID="ucFinish" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:Trans  ID="ucTrans" runat="server" Visible ="false" />
<uc2:Start ID="ucStart" runat="server" Visible="false" />