<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_FacilityScrap_Main" %>
<%@ Register Src="~/Facility/FacilityScrap/Scrap.ascx" TagName="Scrap" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityScrap/List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityScrap/Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityTrans/EditMain.ascx" TagName="Trans" TagPrefix="uc2" %>

<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:Scrap ID="ucScrap" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:Trans  ID="ucTrans" runat="server" Visible ="false" />