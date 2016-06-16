<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_FacilityFix_Main" %>
<%@ Register Src="~/Facility/FacilityFix/Start.ascx" TagName="Start" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityFix/Finish.ascx" TagName="Finish" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityFix/List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityFix/Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityTrans/EditMain.ascx" TagName="Trans" TagPrefix="uc2" %>

<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:Start ID="ucStart" runat="server" Visible="false" />
<uc2:Finish ID="ucFinish" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:Trans  ID="ucTrans" runat="server" Visible ="false" />